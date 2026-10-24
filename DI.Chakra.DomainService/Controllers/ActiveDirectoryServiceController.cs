using DI.Chakra.Connector;
using DI.Chakra.Connector.ActiveDirectory;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using System;
using System.Web.Http;

namespace DI.Chakra.DomainService.Controllers
{

    using DI.Chakra.DomainModel;
    using Infrastructure.Enums;

    [RoutePrefix("ActiveDirectoryService")]
    public class ActiveDirectoryServiceController : ApiController
    {
        [HttpGet]
        [Route("AuthenticateADUser")]
        public LoginResult AuthenticateADUser(string userName, string password,long domainId)
        {
            LoginResult result = new LoginResult();
            try
            {
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                {
                    result.Success = false;
                    result.Message = "User not found";
                    return result;
                }

                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(adsUser.EndPointSystemId);
                var adConfiguration = EndpointConfiguration.GetByEndpointSystemId(domainId);

                ADPasswordHelper pwdHelper = new ADPasswordHelper(adConfiguration);

                result = pwdHelper.Login(userName, password);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpGet]
        [Route("IsActiveAccount")]
        public bool IsActiveAccount(string userName)
        {
            ADLogin adlogin = new ADLogin();
            try
            {
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                {
                    return false;
                }


               
                var endpoint = EndPoint.GetByIdIncludingOwner(adsUser.EndPointSystemId);
                var logger = new ProvisioningLogger(null, endpoint.Id,endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                ActiveDirectoryConnector adConnector = new ActiveDirectoryConnector(endpoint, null, logger);
                bool bAuthenticated = adConnector.IsActiveAccount(adsUser);

                return bAuthenticated;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format
                    ("Error while authenticating AD user - {0}.", userName), ex);
                return false;
            }
        }

        [HttpGet]
        [Route("UnlockAccount")]
        public bool UnlockAccount(string userName)
        {
            bool bReturn = false;

            try
            {
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                    return false;

                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(adsUser.EndPointSystemId);
                
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Unlock Account For : " + userName +
                   " In : " + ep.DisplayName);

                var adConfiguration = EndpointConfiguration.GetByEndpointSystemId(ep.Id);

                ADPasswordHelper pwdHelper = new ADPasswordHelper(adConfiguration);

                bReturn = pwdHelper.UnLock(userName);

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, bReturn, PasswordActionType.UnlockAccount);
                pwdInfo.Save(MasterConfigHelper.ServiceUser);

                //Please confirm  account recon is locked or not 
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format
                   ("Error while UnlockAccount AD user - {0}.", userName), ex);
                bReturn = false;
            }

            return bReturn;
        }

        private PasswordActionInfo CreatePasswordActionInfoObject(IEndpointUserInfo endpointUserInfo, bool status, PasswordActionType actiontype)
        {
            AccountReconcilation useracc = AccountReconcilation.GetByUser(endpointUserInfo.Id, endpointUserInfo.EndPointSystemId);
            return new PasswordActionInfo()
            {
                ActionDate = DateTime.Now,
                EndpointSystemId = endpointUserInfo.EndPointSystemId,
                EndpointUserId = endpointUserInfo.Id,
                UserId = useracc.UserId.Value,
                IsSuccess = status,
                PasswordActionType = actiontype,
            };
        }

        [HttpGet]
        [Route("PasswordResetAccountVerification")]
        public bool PasswordResetAccountVerification(string userName)
        {
            ADLogin adlogin = new ADLogin();
            try
            {

                var usr = DI.Chakra.DomainModel.User.GetByUserName(userName);
                var adEndpoint = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                ADSUserInfo adsUser = ADSUserInfo.GetActivePrimaryByDIUserId(usr.Id, adEndpoint.Id);

                if (adsUser == null)
                {
                    return false;
                }



                var endpoint = EndPoint.GetByIdIncludingOwner(adsUser.EndPointSystemId);
                var logger = new ProvisioningLogger(null, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                ActiveDirectoryConnector adConnector = new ActiveDirectoryConnector(endpoint, null, logger);
                bool bAuthenticated = adConnector.IsActiveAccount(adsUser);

                return bAuthenticated;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format
                    ("Error while authenticating AD user - {0}.", userName), ex);
                return false;
            }
        }
    }
}

using DI.Chakra.Connector;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceProcess;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using DI.Chakra.Connector.ActiveDirectory;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        public void Init()
        {
            DomainModel.AutoMapperHelper.BootStrap();
        }

        public LoginResult AuthenticateADUser(string userName, string password)
        {
            LoginResult result = new LoginResult();
            try
            {
                Init();

                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                {
                    result.Success = false;
                    result.Message = "User not found";
                }

                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(adsUser.EndPointSystemId);
                var adConfiguration = (ADSConfiguration)EndPoint.GetConfiguration(ep);

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

        public bool IsActiveAccount(string userName)
        {
            ADLogin adlogin = new ADLogin();
            try
            {
                Init();

                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                {
                    return false;
                }
                var endpoint = EndPoint.GetByIdIncludingOwner(adsUser.EndPointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                bool bAuthenticated = connector.IsActiveAccount(adsUser);

                return bAuthenticated;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while authenticating AD user - {0}.", userName), ex);
                return false;
            }
        }

        public bool ResetPassword(string userName, string password, long userId, long endpointSystemId)
        {
            try
            {

                bool setDefaultPassword = true;
                bool status = false;
                string defaultADPassword = "@Bcd1234";

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Reset Password For User" + userName);
                Init();

                var endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Reset Password For User : " + userName +
                    "in :" + endpoint.DisplayName);

                var connector = ConnectorFactory.GetConnector(endpoint, null);

                var lastAction = PasswordActionInfo.GetLastActionByUserId(adsUser.DIUserId.Value, PasswordActionType.ResetPassword);

                if (lastAction != null)
                {
                    if (lastAction.IsSuccess)
                    {
                        setDefaultPassword = true;
                    }
                    else
                    {
                        setDefaultPassword = false;
                    }
                }
                else
                {
                    setDefaultPassword = true;
                }

                if (setDefaultPassword)
                {
                    status = connector.ChangePassword(userName, defaultADPassword, userId, false);

                    if (status)
                    {
                        status = connector.ChangePasswordUsingOldPassword(userName, password, defaultADPassword);
                    }
                }
                else
                {
                    status = connector.ChangePasswordUsingOldPassword(userName, password, defaultADPassword);
                }

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, status, PasswordActionType.ResetPassword);
                pwdInfo.Save();

                return status;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while resetting the password for user - {0}.", userName), ex);
                return false;
            }
        }

        public bool ChangePassword(string userName, string password, string oldPassword, long userId, long endpointSystemId)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Change Password For User" + userName);
                Init();

                var endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Change Password For User : " + userName +
                    "in :" + endpoint.DisplayName);

                var connector = ConnectorFactory.GetConnector(endpoint, null);

                bool status = connector.ChangePasswordUsingOldPassword(userName, password, oldPassword);

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, status, PasswordActionType.ChangePassword);
                pwdInfo.Save();

                return status;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while changing the password for user - {0} using old password.", userName), ex);
                return false;
            }
        }

        public bool UnlockAccount(string userName)
        {
            bool bReturn = false;

            try
            {
                Init();

                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                if (adsUser == null)
                    return false;

                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(adsUser.EndPointSystemId);
                var adConfiguration = (ADSConfiguration)EndPoint.GetConfiguration(ep);

                ADPasswordHelper pwdHelper = new ADPasswordHelper(adConfiguration);

                bReturn = pwdHelper.UnLock(userName);

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, bReturn, PasswordActionType.UnlockAccount);
                pwdInfo.Save();
            }
            catch (Exception ex)
            {
                bReturn = false;
            }

            return bReturn;
        }

        public bool IsEndpointConnected(long endpointSystemId)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                return connector.Connect();
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while testing endpoint connection", ex);
                return false;
            }
        }

        public void CreateUserPostAction(InhouseUserInfo userInfo)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                connector.CreateUserInReconTable(userInfo);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while creating inhouse system post user create action for the user - {0}", userInfo.AccountName), ex);
            }
        }

        public void AddEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                connector.AddEntitlement(userInfo, entitlementType);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while adding entitlement from inhouse system for the user - {0}", userInfo.AccountName), ex);
            }
        }

        public void RemoveEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                connector.RemoveEntitlement(userInfo, entitlementType);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while removing entitlement from inhouse system for the user - {0}", userInfo.AccountName), ex);
            }
        }

        public void DisableUser(InhouseUserInfo userInfo)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                connector.DisableUser(userInfo);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while disabling inhouse system user - {0}", userInfo.AccountName), ex);
            }
        }

        public int DownloadAllUsersFromHR(DateTime scanDate)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetTrustedSource();
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                return connector.DownloadAllUsersFromHR(scanDate);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Exception, "Error while downloading users from HRMS system.", ex);
                return 0;
            }
        }

        public void DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                Init();
                var endpoint = EndPoint.GetTrustedSource();
                var connector = ConnectorFactory.GetConnector(endpoint, null);
                connector.DownloadSpecificUserFromHR(pernr);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Exception, string.Format
                    ("Error while downloading user - {0} from HRMS system.", pernr), ex);
            }
        }

        public void StartStopWinService(string toStartStop, string serviceName, int timeoutMilliseconds)
        {
            if (toStartStop == "Start")
            {
                Init();
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch
                {
                }
            }
            else if (toStartStop == "Stop")
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    Init();
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                catch
                {
                }
            }

            else if (toStartStop == "Restart")
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    int millisec1 = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch
                {
                }
            }
        }

        public ServiceControllerStatus GetServicesStatus(string servicename)
        {
            ServiceController[] services = ServiceController.GetServices();
            ServiceControllerStatus scStatus = new ServiceControllerStatus();
            scStatus = ServiceControllerStatus.StartPending;

            try
            {
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == servicename)
                    {
                        scStatus = service.Status;
                        break;
                    }
                }
                return scStatus;
            }
            catch (Exception ex)
            {
                return scStatus;
            }
        }

        private PasswordActionInfo CreatePasswordActionInfoObject(IEndpointUserInfo endpointUserInfo, bool status, PasswordActionType actiontype)
        {
            return new PasswordActionInfo()
            {
                ActionDate = DateTime.Now,
                EndpointSystemId = endpointUserInfo.EndPointSystemId,
                EndpointUserId = endpointUserInfo.Id,
                UserId = endpointUserInfo.DIUserId.Value,
                IsSuccess = status,
                PasswordActionType = actiontype,
            };
        }

    }
}

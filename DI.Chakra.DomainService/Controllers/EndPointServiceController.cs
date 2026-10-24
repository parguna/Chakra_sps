using DI.Chakra.Connector;
using DI.Chakra.Logger;
using System;
using System.Web.Http;

namespace DI.Chakra.DomainService.Controllers
{ 
    using DI.Chakra.DomainModel;
    using Infrastructure.Enums;
    using System.Collections.Generic;
    using System.Data;

    [RoutePrefix("EndPointService")]
    public class EndPointServiceController : ApiController
    {
        [HttpGet]
        [Route("ResetPassword")]
        public bool ResetPassword(string userName, string password, long userId, long endpointSystemId)
        {
            try
            {
                bool setDefaultPassword = true;
                bool status = false;
                string defaultADPassword = "@Bcd1234";

                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Reset Password For User" + userName);

                var endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Reset Password For User : " + userName +
                    "in :" + endpoint.DisplayName);

                var connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SAP, endpoint.DisplayName));

                /**DO NOT MERGE. Commenting out following code to allow direct password reset.
                 * Fixes issue where AD password reset is not being allowed, probably due to minimum password age being set to 1 in AD system.
                 * **/
                //var lastAction = PasswordActionInfo.GetLastActionByUserId(adsUser.DIUserId.Value, PasswordActionType.ResetPassword);

                //if (lastAction != null)
                //{
                //    if (lastAction.IsSuccess)
                //    {
                //        setDefaultPassword = true;
                //    }
                //    else
                //    {
                //        setDefaultPassword = false;
                //    }
                //}
                //else
                //{
                //    setDefaultPassword = true;
                //}

                //if (setDefaultPassword)
                //{
                status = connector.ChangePassword(userName, password, userId, false);

                //    if (status)
                //    {
                //        status = connector.ChangePasswordUsingOldPassword(userName, password, defaultADPassword);
                //    }
                //}
                //else
                //{
                //    status = connector.ChangePasswordUsingOldPassword(userName, password, defaultADPassword);
                //}

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, status, PasswordActionType.ResetPassword);
                pwdInfo.Save(MasterConfigHelper.ServiceUser);

                return status;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format
                    ("Error while resetting the password for user - {0}.", userName), ex);
                return false;
            }
        }

        //[HttpGet]
        //[Route("ChangePassword")]
       /* public bool ChangePassword(string userName, string password, string oldPassword, long userId, long endpointSystemId)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Change Password For User" + userName);

                var endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(userName);

                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, "Change Password For User : " + userName +
                    "in :" + endpoint.DisplayName);

                var connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SAP, endpoint.DisplayName));

                // NOTE: DO NOT MERGE. Calling Change password instead of ChangePasswordUsingOldPassword
                // to allow for demo. Fixes issue with resetting a password with the minimum password age set greater than 0.
                bool status = connector.ChangePassword(userName, password, userId, false);

                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(adsUser, status, PasswordActionType.ChangePassword);
                pwdInfo.Save(MasterConfigHelper.ServiceUser);

                return status;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, string.Format
                    ("Error while changing the password for user - {0} using old password.", userName), ex);
                return false;
            }
        }*/


         [HttpGet]
        [Route("ChangePassword")]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public string ChangePassword(string userName, string password, long userId, long endpointSystemId, long endPointUserId)
        {
            bool resetPwdFailed = false;
            string errorMessage = String.Empty;
            AccountReconcilation accountRecon = null;
            bool status = false;
            IdmsLogger idmsLogger = null;
            ConnectorBase connector = null;
            DI.Chakra.DomainModel.User user = null;
            DI.Chakra.DomainModel.EndPoint endpoint = null;
            bool isSAP = false;
            try
            {
                user = DI.Chakra.DomainModel.User.GetById(userId);
                endpoint = EndPoint.GetByIdIncludingOwner(endpointSystemId);
                isSAP = (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP) || (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAPHANA);
                idmsLogger = new PasswordResetLogger(endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName, userName);
                accountRecon = AccountReconcilation.GetByUser(endpointSystemId, endPointUserId);
                // in case account recon is null
                if (accountRecon == null)
                {
                    errorMessage = String.Format("Could not Reset Password For User : {0} In : {1}. Account Recon not found.", userName, endpoint.DisplayName);
                    idmsLogger.WriteLog(LogLevelType.Error, errorMessage);
                    return errorMessage;
                }

                idmsLogger.WriteLog(LogLevelType.Info, "Initiated Reset Password For User : " + userName);

                var endpointUserInfo = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetUser(accountRecon.EndpointUserId);
                connector = ConnectorFactory.GetConnector(endpoint, null, idmsLogger);
                if (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory)
                {
                    
                    if (connector.IsActiveAccount(endpointUserInfo) == false)
                    {
                        return connector.UnLockChangePasswordAPIMessage;
                    }
                }
                
                if (connector.IsAdminLock(endpointUserInfo.AccountName))
                {
                    return connector.UnLockChangePasswordAPIMessage;
                }

                if (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.NPAC || endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP || endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAPHANA)
                {
                    connector.IsThroughPasswordReset = true;
                }
                // NOTE: DO NOT MERGE. Calling Change password instead of ChangePasswordUsingOldPassword
                // to allow for demo. Fixes issue with resetting a password with the minimum password age set greater than 0.
                //if(endpoint.EndpointSystemTypeId == (long)DI.Chakra.Infrastructure.Enums.EndpointType.SAP)
                //{
                //    //Check for Lock
                //    var isLocked = connector.IsUserLocked(userName);
                //    if (isLocked)
                //    {
                //        var endPointUserInfo = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetUser(accountRecon.EndpointUserId);
                //        var isUnlocked = connector.UnLockAccount(endPointUserInfo);
                //        if (isUnlocked == false)
                //        {
                //            idmsLogger.WriteLog(LogLevelType.Success, $"UnLockAccount {userName}");
                //        }
                //    }
                //}
                status = connector.ChangePassword(userName, password, userId, false);

                var statusMessage = bool.TrueString;

                if (status)
                {
                    accountRecon.IsLocked = false;
                    accountRecon.Save(MasterConfigHelper.ServiceUser);

                    idmsLogger.WriteLog(LogLevelType.Success, $"Reset Password Is Successful For User : {userName}.");
                }
                else
                {
                    errorMessage = connector.UnLockChangePasswordAPIMessage;
                    idmsLogger.WriteLog(LogLevelType.Error, "Reset Password Failed For User : " + userName);
                    statusMessage = $"Reset Password Failed for the account :{userName}.{connector.UnLockChangePasswordAPIMessage}";

                    // mark that the action failed
                    resetPwdFailed = true;
                    errorMessage = statusMessage;
                }
                return statusMessage;
            }
            catch (Exception ex)
            {
                // mark that the action failed
                resetPwdFailed = true;
                if (idmsLogger == null)
                {
                    DILog.PasswordResetLogger.WriteLog(null, LogLevelType.Exception, string.Format
                        ("Error While Resetting The Password For User - {0}.", userName), ex);

                    errorMessage = $"Reset Password Failed for the account :{userName}.{(connector != null && !string.IsNullOrWhiteSpace(connector.UnLockChangePasswordAPIMessage) ? $"Reason: {connector.UnLockChangePasswordAPIMessage}": "Please contact IAM Team.")}";
                }
                else
                {
                    idmsLogger.WriteLog(LogLevelType.Exception, string.Format
                        ("Error While Resetting The Password For User - {0}.", userName), ex);
                    errorMessage = $"Reset Password Failed for the account :{userName}.{(connector != null && !string.IsNullOrWhiteSpace(connector.UnLockChangePasswordAPIMessage) ? $"Reason: {connector.UnLockChangePasswordAPIMessage}" : "Please contact IAM Team.")}";
                }
                if(isSAP && connector != null && connector.IsResetPasswordSuccessfull && accountRecon != null && !string.IsNullOrWhiteSpace(connector.UnLockChangePasswordAPIMessage))
                {
                    try
                    {
                        
                        errorMessage = $"{errorMessage}.{MasterConfigHelper.PasswordResetSAPErrorMessageDescription}";
                    }
                    catch (Exception innerEx)
                    {
                        DILog.PasswordResetLogger.WriteLog(null, LogLevelType.Exception, string.Format
                       ("Error While Sending Temporary Password For User - {0}.", userName), innerEx);
                    }
                }
                return errorMessage;
                
            }
        }

        [HttpGet]
        [Route("IsEndpointConnected")]
        public bool IsEndpointConnected(long endpointSystemId)
        {
            ConnectorBase connector = null;
            bool returnflag = false;
            try
            {

                var endpoint = EndPoint.GetById(endpointSystemId);

                long endpointType = endpoint.EndpointSystemTypeId.HasValue? endpoint.EndpointSystemTypeId.Value : 3 ;
                                 
                var logger = new DiscoveryLogger(0, endpoint.Id, (EndpointType)endpointType, endpoint.DisplayName);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Checking connector connectivity for the endpoint-[{endpoint.DisplayName}]");
                 
                connector = ConnectorFactory.GetConnector(endpoint, null, logger);

                returnflag = connector.Connect();
                try
                {
                    connector.Disconnect();
                }
                catch
                {
                }
                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: The return value from Connector.Connect() for the endpoint-[{endpoint.DisplayName}] is {returnflag}");

                return returnflag;
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error while testing endpoint connection for the endpointsystemId:[{endpointSystemId}]", ex);
                return returnflag;
            }
            finally
            {
                connector = null;
            }
        }

        [HttpGet]
        [Route("DownloadAllUsersFromHR")]
        public int DownloadAllUsersFromHR(DateTime scanDate)
        {
            try
            {
                int returnValue = 0;
                var endpoint = EndPoint.GetTrustedSourceEndPoint();

                long endpointType = endpoint.EndpointSystemTypeId.HasValue ? endpoint.EndpointSystemTypeId.Value : 3;

                var logger = new DiscoveryLogger(0, endpoint.Id, (EndpointType)endpointType, endpoint.DisplayName);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DownloadAllUsersFromHR on the date -[{scanDate}]");

                var connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SAP, endpoint.DisplayName));

               
                returnValue = connector.DownloadAllUsersFromHR(scanDate);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DownloadAllUsersFromHR on the date -[{scanDate}]. The return value is {returnValue}");

                return returnValue;
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error during DownloadAllUsersFromHR", ex);
                return 0;
            }
        }

        [HttpGet]
        [Route("DownloadSpecificUserFromHR")]
        public int DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                var endpoint = EndPoint.GetTrustedSourceEndPoint(); 

                long endpointType = endpoint.EndpointSystemTypeId.HasValue ? endpoint.EndpointSystemTypeId.Value : 3;

                var logger = new DiscoveryLogger(0, endpoint.Id, (EndpointType)endpointType, endpoint.DisplayName);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DownloadSpecificUserFromHR-[{pernr}]");

                var connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0,endpoint.Id,EndpointType.SAP,endpoint.DisplayName));

                connector.DownloadSpecificUserFromHR(pernr);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Completed DownloadSpecificUserFromHR-[{pernr}]");
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error during DownloadSpecificUserFromHR", ex);
            }
            return 1;
        }

        [HttpGet]
        [Route("DiscoverSpecificUserFromHR")]
        public int DiscoverSpecificUserFromHR(string pernr, DateTime? fromDate = null)
        {
            try
            {

                var endpoint = EndPoint.GetTrustedSourceEndPoint();

                long endpointType = endpoint.EndpointSystemTypeId.HasValue ? endpoint.EndpointSystemTypeId.Value : 3;

                var logger = new DiscoveryLogger(0, endpoint.Id, (EndpointType)endpointType, endpoint.DisplayName);

                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DiscoverSpecificUserFromHR-[{pernr}]");
 
                var connector = ConnectorFactory.GetConnector(endpoint, null, logger);
                connector.Process = Process.GetByProcessTypeKeyId(MasterConfigHelper.GetDailyHRMSProcessId, ProcessType.Discovery);
                connector.DiscoveryProcess = DiscoveryProcess.GetById(connector.Process.ProcessTypeKeyId);
                if (fromDate.HasValue)
                {
                    logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DiscoverSpecificUserFromHR-[{pernr}] changing LastRunDate as per User Input:{fromDate}");
                    connector.Process.LastRunOn = fromDate.Value;
                }
                connector.DiscoverSpecificUserFromHR(pernr);


                logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Completed DiscoverSpecificUserFromHR-[{pernr}]");
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error during DiscoverSpecificUserFromHR", ex);
            }
            return 1;
        }

        private PasswordActionInfo CreatePasswordActionInfoObject(IEndpointUserInfo endpointUserInfo, bool status, PasswordActionType actiontype)
        {
            AccountReconcilation useracc = AccountReconcilation.GetByUser(endpointUserInfo.EndPointSystemId, endpointUserInfo.Id);
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
        [Route("GetHRMSDetail")]
        public DataTable GetHRMSDetail(string pernr, string eventType, string tableName, long endpointSystemId)
        {
            DataTable dt=new DataTable();
            try
            {

                if (!string.IsNullOrWhiteSpace(eventType))
                {
                    switch (eventType)
                    {
                        case "On Board":
                            dt = CreateEventDataTable(MasterConfigHelper.OnBoardTriggers);
                            break;
                        case "Off Board":
                            dt = CreateEventDataTable(MasterConfigHelper.OffBoardTriggers);
                            break;
                        case "Transfer":
                            dt = CreateEventDataTable(MasterConfigHelper.TranferTriggers);
                            break;
                        case "Re-Org":
                            dt = CreateEventDataTable(MasterConfigHelper.ReOrgTriggers);
                            break;
                        case "ReHire":
                            dt = CreateEventDataTable(MasterConfigHelper.RejoinTriggers);
                            break;
                        case "Propagation":
                            dt = CreateEventDataTable(MasterConfigHelper.PropagationTriggers);
                            break;
                        case "Promotion/Demotion":
                            dt = CreateEventDataTable(MasterConfigHelper.PromotionTriggers);
                            break; 
                        default:
                            dt = CreateEventDataTable(new List<EventTypeReason>());
                            break;
                    }
                }

                else if (!string.IsNullOrWhiteSpace(pernr) && !string.IsNullOrWhiteSpace(tableName))
                {

                    var endpoint = EndPoint.GetByEndpointType(endpointSystemId, true);

                    if (endpoint == null) return dt;

                    SAPHRMSConnector sap_connector = null;
                    DI.Chakra.Connector.SuccessFactorConnector sf_Connector = null;
                    if (endpoint.EndpointSystemTypeId == (long)Infrastructure.Enums.EndpointType.SAP)
                    {
                        sap_connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SAP, endpoint.DisplayName)) as SAPHRMSConnector;
                    }

                    if (endpoint.EndpointSystemTypeId == (long)Infrastructure.Enums.EndpointType.SuccessFactor)
                    {
                        sf_Connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SuccessFactor, endpoint.DisplayName)) as SuccessFactorConnector;
                    }
                     
                    if (!string.IsNullOrWhiteSpace(pernr) && !string.IsNullOrWhiteSpace(tableName))
                    {
                        switch (tableName)
                        {
                            // SAP 
                            case "PA0000":
                                if (sap_connector != null)
                                    dt = sap_connector.GetTablePA0000(pernr);
                                break;
                            case "PA0001":
                                if (sap_connector != null)
                                    dt = sap_connector.GetTablePA0001(pernr);
                                break;
                            case "PA0002":
                                if (sap_connector != null)
                                    dt = sap_connector.GetTablePA0002(pernr);
                                break;
                            case "PA0302":
                                if (sap_connector != null)
                                    dt = sap_connector.GetTablePA0302(pernr);
                                break;

                            // SF

                            case "Address Details":
                            case "Contact Details":
                            case "Email Details":
                            case "Personal Details":
                            case "Employeement Details":
                            case "Event Details":
                                if (sf_Connector != null)
                                    dt = sf_Connector.GetTableInfo(tableName, pernr);
                                break;


                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "Error during GetHRMSDetail", ex);
            }
            return dt;
        }


        DataTable CreateEventDataTable(List<EventTypeReason> eventTypeReasons)

        {
            DataTable dt = new DataTable();
            DataColumn dc = null;

            dc = new DataColumn();
            dc.ColumnName = "EventType";
            dc.DataType = typeof(string);
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "EventReason";
            dc.DataType = typeof(string);
            dt.Columns.Add(dc);

            foreach (var evt in eventTypeReasons)
            {
                var dr = dt.NewRow();
                dr["EventType"] = evt.EventType;
                dr["EventReason"] = evt.EventReason;
                dt.Rows.Add(dr);
            }

            return dt;
        }
        [HttpGet]
        [Route("UnlockAccount")]
        public string UnlockAccount(string userName, long endpointSystemId, long endPointUserId)
        {
            bool bReturn = false;
            string statusMessage = bool.FalseString;
            IdmsLogger logger = null;
            EndPoint endpoint = null;
            try
            {
                endpoint = EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId);
                logger = new AccountUnlockLogger(endpoint?.Id, endpoint?.EndpointSystemTypeId, endpoint?.DisplayName, userName);
                var accountRecon = AccountReconcilation.GetByUser(endpointSystemId, endPointUserId);

                if (!accountRecon.UserId.HasValue)
                {
                    logger.WriteLog(LogLevelType.Exception, string.Format
                    ("Error while Unlocking account for user - {0}. Account not mapped", userName));

                    return "Account is not mapped. Please map the account and try again." + userName;
                }

                
                //logger.WriteLog(LogLevelType.Info, $"Unlock Account For User : {userName} In : {endpoint.DisplayName} started",null);
                
                var connector = ConnectorFactory.GetConnector(endpoint, null, logger);
                var endPointUserInfo = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetUser(accountRecon.EndpointUserId);
                if (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory)
                {
                    if(connector.IsActiveAccount(endPointUserInfo) == false)
                    {
                        return connector.UnLockChangePasswordAPIMessage;
                    }
                }
                if (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.NPAC)
                {
                    connector.IsThroughPasswordReset = true;
                }
                if (connector.IsAdminLock(endPointUserInfo.AccountName))
                {
                    return connector.UnLockChangePasswordAPIMessage;
                }
                var usr = DI.Chakra.DomainModel.User.GetById(accountRecon.UserId.Value);

                //endPointUserInfo = GetUpdatedUserInfo(endPointUserInfo, endpoint, usr);
                if (endpoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SSIDS)
                {
                    bReturn = connector.UnLockAccount(endPointUserInfo, accountRecon.UserId.Value);
                }
                else
                {
                    connector.UserId = usr.Id;
                    bReturn = connector.UnLockAccount(endPointUserInfo);
                }
                PasswordActionInfo pwdInfo = this.CreatePasswordActionInfoObject(endPointUserInfo, bReturn, PasswordActionType.UnlockAccount);
                pwdInfo.Save(MasterConfigHelper.ServiceUser);

                if (bReturn)
                {
                    statusMessage = bool.TrueString;
                    //logger.WriteLog(LogLevelType.Success, $"Unlock Account Is Successful For User : {userName} In :{ endpoint.DisplayName}");
                    accountRecon.IsLocked = false;
                    accountRecon.Save(MasterConfigHelper.ServiceUser);

                }
                else
                {
                    logger.WriteLog(LogLevelType.Error, "Unlock Account Failed For User : " + userName +
                        " In : " + endpoint.DisplayName);
                    statusMessage = $"Account unlock failed for the account : {userName} in {endpoint.DisplayName}. {(!string.IsNullOrWhiteSpace(connector.UnLockChangePasswordAPIMessage) ? $"Message From Endpoint:{connector.UnLockChangePasswordAPIMessage}" :"")}";
                }

                return statusMessage;
            }
            catch (Exception ex)
            {
                if(logger != null)
                {
                    logger.WriteLog(LogLevelType.Exception, $"Error while Unlocking account for user - {userName} in {endpoint?.DisplayName}.", ex);
                }
                else
                {
                    DILog.AccountUnlockLogger.WriteLog(null, LogLevelType.Exception, $"Error while Unlocking account for user - {userName} in {endpoint?.DisplayName}.", ex);
                }

                return $"Account unlock failed for the account : {userName} in {endpoint?.DisplayName}. Please contact IAM Team.";
            }
        }
        /// <summary>
        /// Obsolete
        /// </summary>
        /// <param name="endPointUserInfo"></param>
        /// <param name="endPoint"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEndpointUserInfo GetUpdatedUserInfo(IEndpointUserInfo endPointUserInfo, EndPoint endPoint, User user)
        {
            if(endPoint != null)
            {
                if (endPoint.ApplicationType == ApplicationType.NPAC)
                {
                    var npacUsr = endPointUserInfo as NPACUserInfo;
                    npacUsr.Invalid_Date = DateTime.Today.AddDays(7);
                    
                    if (user.TerminationDate.HasValue)
                    {
                        npacUsr.Invalid_Date = user.TerminationDate.Value;
                    }
                    endPointUserInfo = npacUsr;
                }
            }
            
            return endPointUserInfo;
        }
    }
}

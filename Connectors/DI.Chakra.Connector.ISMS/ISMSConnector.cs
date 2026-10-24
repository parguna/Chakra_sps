using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Logger;
using System.Data;
using System.IO;
using CMessage = DI.Chakra.Connector.ISMS.Message;
using System.Globalization;
using System.Dynamic;
using DI.Chakra.DomainModel.Presentation;
using Newtonsoft.Json;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using System.Text;

namespace DI.Chakra.Connector
{
    public class ISMSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _ismsConfiguration = null;
        #endregion
        #region Configuration
        public ISMSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var ismsConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            ismsConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id); ;
            this.ISMSConfiguration = ismsConfiguration;
            this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
        }
        public EndpointConfiguration ISMSConfiguration
        {
            get { return _ismsConfiguration; }
            set { this._ismsConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = ISMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }
        #endregion
        #region Discovery
        public override bool Connect()
        {
            //try
            //{
            //    if (File.Exists(GetConfiguration(EndpointKey.UserFilePath)))
            //    {
            //        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ISMS_100, GetConfiguration(EndpointKey.IPAddress)));
            //        return true;
            //    }
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ISMS_101, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.ISMS_101, GetConfiguration(EndpointKey.IPAddress)));
            //    return false;
            //}

            return true;
        }
        public override void Disconnect()
        {

        }
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                bool isUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ISMS_User) != null;

                if (isUserSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("GetUsers started {0}", GetConfiguration(EndpointKey.IPAddress)));
                    var cseList = GetUser();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("GetUsers Ended {0}", GetConfiguration(EndpointKey.IPAddress)));

                    if (MarkDiscoveryFail) throw new Exception("ISMS Discovery failed.");
                    if (this.DoRecon)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("UpdateUser started {0}", GetConfiguration(EndpointKey.IPAddress)));
                        UpdateUser(cseList);
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("UpdateUser Ended {0}", GetConfiguration(EndpointKey.IPAddress)));

                        if (!MarkDiscoveryFail)
                        {
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("Create AccessRequests started {0}", GetConfiguration(EndpointKey.IPAddress)));
                            CreateAccessRequestForCSEL();
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("Create AccessRequests ended {0}", GetConfiguration(EndpointKey.IPAddress)));
                        }

                        base.DeleteLocalCopyFiles();
                    }
                }

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, CMessage.ISMS_102, ex);
                throw;
            }
        }
        public void UpdateUser(List<CSELUserInfo> cselList)
        {
            string missingUsersList = string.Empty;

            try
            {
                if (cselList != null && cselList.Count() > 0)
                {
                    User.ExecuteQuery("UPDATE DI_M_User SET GoldBar = 0, BadgeType = NULL ");
                    NHRMSUser.ExecuteQuery("UPDATE DI_HRMS_User set GoldBar = 0, BadgeType = NULL");
                }

                missingUsersList += string.Format("  Unmapped User List");
                missingUsersList += Environment.NewLine;
                int unmappedUserListCounter = 0;
                foreach (var listItem in cselList)
                {
                    try
                    {
                        User user = User.GetUserByUserNameorIdNo(listItem.UserName, listItem.IDNo, listItem.IDType);
                        if (user != null)
                        {
                            user.BadgeType = listItem.BadgeType;

                            if (!string.IsNullOrEmpty(listItem.BadgeType))
                            {
                                user.GoldBar = true;
                            }
                            user.LastModifiedBy = MasterConfigHelper.ServiceUser;
                            user.LastModifiedDate = DateTime.Now;
                            user.Save(MasterConfigHelper.ServiceUser);
                        }
                        else
                        {
                            unmappedUserListCounter++;
                            missingUsersList += string.Format("Id No: {0},ID Type:{1},CSEL#:{2}, Badge Type:{3}, User Name:{4},DOB:{5}", listItem.IDNo, listItem.IDType, listItem.CSELLicNo, listItem.BadgeType, listItem.UserName, listItem.DOB);
                            missingUsersList += Environment.NewLine;
                        }
                        NHRMSUser mHRMSUser = NHRMSUser.GetUserByUserNameorIdNo(listItem.UserName, listItem.IDNo);
                        if (mHRMSUser != null)
                        {
                            mHRMSUser.BadgeType = listItem.BadgeType;
                            mHRMSUser.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, CMessage.ISMS_108, ex, true, true, AlertCenterNotificationType.DiscoveryFailed);
                    }
                }

                if (unmappedUserListCounter > 0)
                {
                    string filePath = GetConfiguration(EndpointKey.UserFilePath);
                    missingUsersList += Environment.NewLine;
                    missingUsersList += string.Format("Total Count {0}", unmappedUserListCounter);
                    missingUsersList += Environment.NewLine;
                    missingUsersList += "================================================================";
                    this.AuthenticateNetworkAndWriteDatatoFile(filePath, missingUsersList, false);
                    string message = string.Format("ISMS Goldbar - Total {0} Users UnMapped, Please check the ISM error log folder for more deteails.", unmappedUserListCounter);
                    DILog.AlertNotificationLogger.WriteLog(this.EndPoint == null ? 0 : this.EndPoint.Id, DiscoveryProcess != null ? DiscoveryProcess.Endpointsystemid : (this.EndPoint != null ? this.EndPoint.Id : 0), message, AlertCenterNotificationType.Discovery);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Exception @Update Users ISMS", ex, true, true, AlertCenterNotificationType.DiscoveryFailed);
            }
        }

        public List<CSELUserInfo> GetUser()
        {
            string remarks = string.Empty;
            List<CSELUserInfo> cselList = new List<CSELUserInfo>();
            string errorInfo = string.Empty;
            try
            {
                string mandatoryColsList = CMessage.MANDATORY_COLS_LIST;
                string filePath = GetConfiguration(EndpointKey.UserFilePath);
                string readFileName = string.Empty;
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryColsList);

                if (dataTable != null)
                {
                    this.DoRecon = true;
                    CSELUserInfo cselUserInfo = null;
                    string userName = string.Empty;
                    string idNo = string.Empty;
                    string idType = string.Empty;
                    string cselLicNo = string.Empty;
                    string badgeType = string.Empty;
                    string dob = string.Empty;
                    string lineItem = string.Empty;

                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        userName = string.Empty;
                        idNo = string.Empty;
                        idType = string.Empty;
                        cselLicNo = string.Empty;
                        badgeType = string.Empty;
                        dob = string.Empty;
                        lineItem = string.Empty;
                        try
                        {
                            userName = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_103));
                            idNo = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_113));
                            idType = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_112));
                            cselLicNo = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_104));
                            badgeType = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_111));
                            dob = Convert.ToString(GetColumnValue(currentWorkingRow, CMessage.ISMS_114));
                            lineItem = Convert.ToString(GetColumnValue(currentWorkingRow, "HH"));

                            if (!string.IsNullOrEmpty(lineItem) && lineItem.ToLower() != "bb")
                                continue;

                            cselUserInfo = CSELUserInfo.GetByEndPointsystemId(userName, idNo, _endpoint.Id);
                            if (cselUserInfo == null)
                            {
                                cselUserInfo = new CSELUserInfo();

                                cselUserInfo.UserName = userName;
                                cselUserInfo.IDNo = idNo;
                                cselUserInfo.IDType = idType;
                                cselUserInfo.CSELLicNo = cselLicNo;
                                cselUserInfo.BadgeType = badgeType;
                                cselUserInfo.DOB = dob;
                            }
                            else
                            {
                                cselUserInfo.IDType = idType;
                                cselUserInfo.CSELLicNo = cselLicNo;
                                cselUserInfo.BadgeType = badgeType;
                                cselUserInfo.DOB = dob;
                            }
                            cselUserInfo.EndPointSystemId = _endpoint.Id;
                            cselUserInfo.Save(MasterConfigHelper.ServiceUser);
                            cselList.Add(cselUserInfo);
                        }
                        catch (Exception ex)
                        {
                            errorInfo += "Error While Reading Record ";
                            errorInfo += Environment.NewLine;
                            errorInfo += string.Format("Id No: {0},ID Type:{1},CSEL#:{2}, Badge Type:{3}, User Name:{4},DOB:{5}", idNo, idType, cselLicNo, badgeType, userName, dob);

                            errorInfo += ex.InnerException != null ? ex.InnerException + ex.Message : ex.Message;
                            WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Exception at GetUsers - ISMS ", ex, true, true, AlertCenterNotificationType.DiscoveryFailed);
                        }
                    }


                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        filePath = GetConfiguration(EndpointKey.UserFilePath);
                        errorInfo += "Error While Reading Record ";
                        errorInfo += Environment.NewLine;
                        errorInfo += "================================================================";
                        this.AuthenticateNetworkAndWriteDatatoFile(filePath, errorInfo);
                    }

                    //Delete files
                    base.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    this.DoRecon = false;
                }

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.ISMS_108 + ex.Message);
            }

            return cselList;
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.ISMS_109) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.ISMS_110, columnName), ex);
                return string.Empty;
            }
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }
        public void CreateAccessRequestForCSEL()
        {

            try
            {
                List<AccessRequest> requests = new List<AccessRequest>();
                AccessRequest newRequest = new AccessRequest();
                AccessRequest oldRequest = new AccessRequest();
                AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
                List<AccountReconcilation> listAcs = new List<AccountReconcilation>();
                EndPoint ep = new EndPoint();
                var isRemovalReqCreate = ConfigurationMaster.GetConfigKey("EnableGamingRemovalRequestCreate", Product.IACM);

                List<User> NonCSELUsers = User.GetNonCSELGoldBarUsers();
                List<User> ExcludedUsers = User.GetExclusionUsersByEventType(EmployeeEvents.CSELGoldBar);
                List<User> resultCSELUsers = NonCSELUsers.Except(ExcludedUsers).ToList();
                SubSystemType gamingSystem = SubSystemType.getSubsystemByName(Constants.GamingSubSystem);
                StringBuilder removalRequestList = new StringBuilder();
                string removalRequestFromat = "User ADID : {0}, Employee Id: {1}";

                foreach (var user in resultCSELUsers)
                {
                    try
                    {
                        listAcs = null;
                        requests = new List<AccessRequest>();
                        newRequest = new AccessRequest();
                        adhocrequst = new AdHocAccessRequest();


                        Guid transactionId = Guid.NewGuid();
                        listAcs = AccountReconcilation.GetGamingApplications(user.Id, gamingSystem.Id);

                        if (listAcs != null && listAcs.Count > 0)
                        {
                            listAcs = listAcs.Where(t => t.IsLocked == false && t.IsDeleted == false).ToList();

                            foreach (var listAc in listAcs)
                            {
                                oldRequest = null;
                                ep = null;
                                if (!string.IsNullOrEmpty(isRemovalReqCreate) && isRemovalReqCreate == "0")
                                {
                                    if (string.IsNullOrEmpty(removalRequestList.ToString()))
                                    {
                                        removalRequestList.AppendLine("List of Gaming Application Removal User List:");
                                    }
                                    removalRequestList.AppendLine(string.Format(removalRequestFromat, user.UserName, user.EmployeeId));
                                    continue;
                                }
                                oldRequest = AccessRequest.GetCSELRequestByRequesteeAndEndpointId(user.Id, listAc.EndpointSystemId, listAc.AccountType);
                                if (oldRequest == null)
                                {
                                    //AccessRequest.
                                    newRequest = new AccessRequest();
                                    adhocrequst = new AdHocAccessRequest();
                                    adhocrequst.RequesteeUserId = user.Id;

                                    newRequest.UserId = user.Id;
                                    newRequest.EndpointSystemId = listAc.EndpointSystemId;

                                    newRequest.RequestStatus = AccessRequestStatus.Granted;
                                    newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                    adhocrequst.AdhocActionType = AdHocActionType.DeleteAccount;
                                    newRequest.RequesterId = null;

                                    newRequest.RequestSource = AccessRequestSource.GamingApplicationRemoval;
                                    newRequest.AccountType = listAc.AccountType;

                                    newRequest.ExecutionTime = DateTime.Now;
                                    newRequest.Description = "This request is created automatically for revoking Gaming Application Access due to removal of CSEL / GoldBar Access.";
                                    newRequest.Remarks = string.Empty;
                                    newRequest.TransactionId = transactionId;
                                    //newRequest.EmployeeEventTriggerId = trigger.Id;


                                    ep = EndPoint.GetByIdIncludingEndPointSystemType(listAc.EndpointSystemId);

                                    if (ep != null)
                                    {
                                        adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                                    }

                                    adhocrequst.EndpointSystemId = listAc.EndpointSystemId;
                                    adhocrequst.AccountType = listAc.AccountType;
                                    adhocrequst.ValidFrom = user.JoinDate;
                                    adhocrequst.ValidTo = DateTime.Now;

                                    setRequestItemDetail(adhocrequst, listAc);

                                    adhocrequst.Comments = "AdhocAccessRequest created automatically for revoking Gaming Application Access due to removal of CSEL / GoldBar Access.";
                                    adhocrequst.Save(MasterConfigHelper.ServiceUser);
                                    newRequest.AdHocAccessRequestId = adhocrequst.Id;
                                    requests.Add(newRequest);
                                }
                            }
                        }
                        requests.ForEach(lq => lq.Save(MasterConfigHelper.ServiceUser));
                        //return requests.Count;
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Exception at AccessRequest create for CSEL/GOLD BAR removal for user {0} ", user.UserName), ex, true, true, AlertCenterNotificationType.CSELGOLDBARuserExclusion);
                    }
                }

                if (!string.IsNullOrEmpty(removalRequestList.ToString()))
                {
                    DILog.AlertNotificationLogger.WriteLog(this.EndPoint == null ? 0 : this.EndPoint.Id, DiscoveryProcess != null ? DiscoveryProcess.Endpointsystemid : (this.EndPoint != null ? this.EndPoint.Id : 0), removalRequestList.ToString(), AlertCenterNotificationType.CSELGOLDBARuserExclusion);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Exception at AccessRequest create for CSEL/GOLD BAR removal", ex, true, true, AlertCenterNotificationType.CSELGOLDBARuserExclusion);
            }

        }
        private static void setRequestItemDetail(AdHocAccessRequest adhocRequest, AccountReconcilation accountReconcilation)
        {
            dynamic detail = new ExpandoObject();

            detail = ExpandoHelper.AddProperty(detail, "AccountName", accountReconcilation == null ? "" : accountReconcilation.AccountName);
            string detailString = JsonConvert.SerializeObject(detail);

            PresentationAccessRequestItem arItem = new PresentationAccessRequestItem
            {
                EndpointSystemId = adhocRequest.EndpointSystemId.Value,
                AccessRequestItemType = AccessRequestItemType.Application,
                AdditionalInfo = detailString,
            };
            adhocRequest.SetRequestedItems = new List<PresentationAccessRequestItem>() { arItem };
        }
        #endregion
    }
}




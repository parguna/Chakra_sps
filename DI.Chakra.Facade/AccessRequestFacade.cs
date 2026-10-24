using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.SAP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DI.Chakra.Model.ReportData;
using System.Xml.Serialization;
using System.IO;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model.CustomEntities;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;

namespace DI.Chakra.Facade
{
    public class AccessRequestFacade
    {
        #region Private Members and Constructor
        private AccessRequestDA arDA;

        public AccessRequestFacade()
        {
            arDA = new AccessRequestDA();
        }
        #endregion

        #region ADD
        public bool AddAccessRequest(IList<AccessRequest> list)
        {
            try
            {

                //NOTE: Temporary, will be removed
                string fileName = string.Empty;

                foreach (AccessRequest ar in list)
                {
                    arDA.AddAccessRequest(ar);
                }



                return true;
            }
            catch
            {
                throw;
            }
        }

        public List<AccessRequest> GetAccessRequestsByTransactionId(Guid transactionId)
        {
            try
            {
                return arDA.GetAccessRequestsByTransactionId(transactionId);
            }
            catch
            {
                throw;
            }
        }

        public List<AccessRequest> GetSubmittedAccessRequests()
        {
            try
            {
                return arDA.GetSubmittedAccessRequests();
            }
            catch
            {
                throw;
            }
        }

        public long AddADSUserInfo(ADSUserInfo adUsr)
        {
            return arDA.AddADSUserInfo(adUsr);
        }

        public bool AddADSUserIntoGroup(ADSUserInfo adUsr)
        {
            return arDA.AddADSUserIntoGroup(adUsr);
        }

        public bool AddLotusNoteUserIntoGroup(LotusUserInfo lnUsr)
        {
            return arDA.AddLotusNoteUserIntoGroup(lnUsr);
        }

        #region SAP
        public long AddSAPUserInfo(SAPUserinfo sapUsr)
        {
            sapUsr.validToDate = DateTime.MaxValue;
            sapUsr.validFromDate = DateTime.Parse(sapUsr.UserValidFrom);

            if (sapUsr.UserType == SAPUserType.Communication)
            {
                sapUsr.SapUserType = "C";
            }
            else if (sapUsr.UserType == SAPUserType.Service)
            {
                sapUsr.SapUserType = "S";
            }
            else if (sapUsr.UserType == SAPUserType.System)
            {
                sapUsr.SapUserType = "B";
            }
            else if (sapUsr.UserType == SAPUserType.Reference)
            {
                sapUsr.SapUserType = "L";
            }
            else
            {
                sapUsr.SapUserType = "A";
            }

            return arDA.AddSAPUserInfo(sapUsr);
        }
        public bool AddSAPUserIntoRoles(SAPUserinfo sapUsr)
        {
            return arDA.AddSAPUserIntoRoles(sapUsr);
        }

        public bool AddSAPUserIntoGroups(SAPUserinfo sapUsr)
        {
            return arDA.AddSAPUserIntoGroups(sapUsr);
        }
        #endregion

        #region SAP EP
        public long AddSAPEPUserInfo(SAPEPUserInfo sapEPUsr)
        {
            return arDA.AddSAPEPUserInfo(sapEPUsr);
        }

        public bool AddUserIntoSAPEPUserIntoRoles(SAPEPUserInfo sapEPUsr)
        {
            return arDA.AddUserIntoSAPEPUserIntoRoles(sapEPUsr);
        }

        public bool AddUserIntoSAPEPUserIntoGroup(SAPEPUserInfo sapEPUsr)
        {
            return arDA.AddUserIntoSAPEPUserIntoGroups(sapEPUsr);
        }

        #endregion

        #endregion


        #region DELETE
        public bool DeleteADUser(long ADUserId)
        {
            return arDA.DeleteADUser(ADUserId);
        }
        public bool DeleteADUserGroupMap(ADSUserInfo adUser)
        {
            return arDA.DeleteADUserGroupMap(adUser);
        }
        public bool DeleteADUserGroupMapAll(ADSUserInfo adUser)
        {
            return arDA.DeleteADUserGroupMapAll(adUser);
        }
        public bool DeleteSAPUser(long sapUserId)
        {
            return arDA.DeleteSAPUser(sapUserId);
        }
        public bool DeleteSAPUserRoleMap(SAPUserinfo sapUser)
        {
            return arDA.DeleteSAPUserRoleMap(sapUser);
        }
        public bool DeleteSAPUserGroupMap(SAPUserinfo sapUser)
        {
            return arDA.DeleteSAPUserGroupMap(sapUser);
        }
        public bool DeleteSAEPPUser(int sapEPUserId)
        {
            return arDA.DeleteSAEPPUser(sapEPUserId);
        }
        public bool DeleteSAPEPUserRoleMap(SAPEPUserInfo sapEPUser)
        {
            return arDA.DeleteSAPEPUserRoleMap(sapEPUser);
        }
        public bool DeleteSAPEPUserGroupMap(SAPEPUserInfo sapEPUser)
        {
            return arDA.DeleteSAPEPUserGroupMap(sapEPUser);
        }
        public bool DeleteLotusNotesUser(long LNUserId)
        {
            return arDA.DeleteLotusNotesUser(LNUserId);
        }
        public bool DeleteLotusNotesUserGroupMap(LotusUserInfo lnsUser)
        {
            return arDA.DeleteLotusNotesUserGroupMap(lnsUser);
        }

        #endregion

        #region GET

        public AccessRequest GetAccessRequestById(long accessRequestId, bool deepLoad)
        {
            return arDA.GetAccessRequestById(accessRequestId, deepLoad);
        }
        public IList<AccessRequest> GetallAccessRequestByUserId(int UserId)
        {
            return arDA.GetallAccessRequestByUserId(UserId);
        }

        public bool CheckIfSameRequestsPending(AdHocAccessRequest _obj, out long RequesterID, out long AccessRequestID)
        {
            return arDA.CheckIfSameRequestsPending(_obj, out RequesterID, out AccessRequestID);
        }

        public bool CheckIfSameRequestsPendingSpecialACR(AdHocAccessRequest _obj, out long RequesterID, out long AccessRequestID)
        {
            return arDA.CheckIfSameRequestsPendingSpecialACR(_obj, out RequesterID, out AccessRequestID);
        }

        public IList<AccessRequest> GetallAccessRequests()
        {
            return arDA.GetallAccessRequests();
        }

        public IList<AccessRequest> GetaAccessRequestsByProgressStatus(AccessRequestProgress Status)
        {
            return arDA.GetaAccessRequestsByProgressStatus(Status);
        }

        public IList<AccessRequestGridData> GetallAccessRequestsGridData()
        {
            return arDA.GetallAccessRequestsGridData();
        }
        #endregion

        #region TOBE Implement
        public bool RemoveAccessRequest(IList<AccessRequest> rmCrntRolelist)
        {
            return false;
        }

        public Dictionary<int, AccessRequest> GetAccessRequestForUserByRoleDictionary(int RequestForUserId, bool IsNewUsr)
        {
            return null;
        }

        public bool UpdateAccessRequest(AccessRequest accessRequest)
        {
            return arDA.UpdateAccessRequest(accessRequest);
        }

        #endregion

        #region Provisioning Engine Methods
        public List<AccessRequest> GetApprovedAccessRequestBatch()
        {
            return arDA.GetApprovedAccessRequestBatch();
        }
        public bool isUserIdExist(int[] endPointIds, string usrLogInName, int DIUserId, out string ADUserLogOnName)
        {
            return arDA.isUserIdExist(endPointIds, usrLogInName, DIUserId, out ADUserLogOnName);
        }


        #endregion
      
        public IList<AutoProvisioningDetailsReportData> getAllAutoProvisioningDetailsBtnStartAndEndTimePeriod(DateTime startDate, DateTime endDate)
        {
            try
            {
                return arDA.getAllAutoProvisioningDetailsBtnStartAndEndTimePeriod(startDate, endDate);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CheckNewAccountExist(AdHocAccessRequest AdHocAccessRequest)
        {
            return arDA.CheckNewAccountExist(AdHocAccessRequest);
        }

        public IList<MyRequestsGridData> GetRequestsByActionUserId(long actionUserId)
        {
            try
            {
                return arDA.GetRequestsByActionUserId(actionUserId);
            }
            catch
            {
                throw;
            }
        }

        public IList<ApproversGridData> GetApproveStatusListByRequestId(long rqstId)
        {
            try
            {
                return arDA.GetApproveStatusListByRequestId(rqstId);
            }
            catch
            {
                throw;
            }
        }

        public AccessRequest GetAccessRequestByAdHocRequestId(long adhocAccessRequestId)
        {
            try
            {
                return arDA.GetAccessRequestByAdHocRequestId(adhocAccessRequestId);
            }
            catch
            {
                throw;
            }
        }

        public IList<AccessRequestGridData> GetallAccessRequestsReportData(DateTime startDate, DateTime endDate, AccessRequestStatus approvalStatus, AccessRequestProgress provisioningStatus)
        {
            try
            {
                return arDA.GetallAccessRequestsReportData(startDate, endDate, approvalStatus, provisioningStatus);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public AccessRequest PopulateNewFromOldAccessRequest(AccessRequest oldRequest)
        {
            return new AccessRequest()
            {
                ActivationDate = oldRequest.ActivationDate,
                ApproverType = oldRequest.ApproverType,
                Comments = oldRequest.Comments,
                CreatedBy = oldRequest.CreatedBy,
                CreatedDate = oldRequest.CreatedDate,
                DeactivationDate = oldRequest.DeactivationDate,
                Description = oldRequest.Description,
                ExecutionTime = oldRequest.ExecutionTime,
                IsCurrentLevelApproved = oldRequest.IsCurrentLevelApproved,
                LastModifiedBy = oldRequest.LastModifiedBy,
                LastModifiedDate = oldRequest.LastModifiedDate,
                ProgressStatus = oldRequest.ProgressStatus,
                Remarks = oldRequest.Remarks,
                RequesterId = oldRequest.RequesterId,
                RequestSource = oldRequest.RequestSource,
                RequestStatus = oldRequest.RequestStatus,
                TenantId = oldRequest.TenantId,
                TransactionId = oldRequest.TransactionId,
                UserId = oldRequest.UserId,
            };
        }

        public AdHocAccessRequest PopulateNewFromOldAdhocRequest(AdHocAccessRequest adhocaccessrequest)
        {
            return new AdHocAccessRequest()
            {
                AccountType = adhocaccessrequest.AccountType,
                AdhocActionType = adhocaccessrequest.AdhocActionType,
                Comments = adhocaccessrequest.Comments,
                CreatedBy = adhocaccessrequest.CreatedBy,
                CreatedDate = adhocaccessrequest.CreatedDate,
                EndpointSystemType = adhocaccessrequest.EndpointSystemType,
                IsSelfRequest = adhocaccessrequest.IsSelfRequest,
                RequesteeUserId = adhocaccessrequest.RequesteeUserId,
                RequesterUserId = adhocaccessrequest.RequesterUserId,
                TenantId = adhocaccessrequest.TenantId,
                ValidFrom = adhocaccessrequest.ValidFrom,
                ValidTo = adhocaccessrequest.ValidTo,
                EntitlementDetail = adhocaccessrequest.EntitlementDetail,
                EndpointSystemId = adhocaccessrequest.EndpointSystemId,
            };
        }

        public IList<MyRequestsGridData> GetRequestsByRequester(List<long> RequesterIDs)
        {
            try
            {
                return arDA.GetRequestsByRequester(RequesterIDs);
            }
            catch
            {
                throw;
            }
        }

        public static AdHocAccessRequest GetAdhocRequest(int AdhocRequestID)
        {
            return new AccessRequestDA().GetAdhocRequest(AdhocRequestID);
        }
        public List<AccessRequest> GetValidationFailedRequests()
        {
            try
            {
                return new UserDA().GetValidationFailedRequests();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FailedRequestGridData> GetFailedSystemRequests(AccessRequestProgress progressStatus)
        {
            try
            {
                var requests = new UserDA().GetFailedSystemRequests(progressStatus);

                if (progressStatus == AccessRequestProgress.Executing)
                {
                    requests.RemoveAll(item => (DateTime.Now - item.RequestDate).TotalMinutes < 30);
                }
                return requests;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region OracleEBS
        public bool DeleteOracleEBSUser(string oraUserName)
        {
            return arDA.DeleteOracleEBSUser(oraUserName);
        }

        public bool DeleteOracleEBSUserRoleMap(OracleEBSUserInfo OracleEBSUser, EntitlementType Entitlement)
        {
            return arDA.DeleteOracleEBSUserRoleMap(OracleEBSUser, Entitlement);
        }

        public bool AddOracleEBSUserIntoRole(OracleEBSUserInfo oraEBSUsr, EntitlementType Entitlement)
        {
            return arDA.AddOracleEBSUserIntoRole(oraEBSUsr, Entitlement);
        }

        public bool AddLinuxUserIntoGroup(LinuxUserInfo linuxUsr, EntitlementType Entitlement)
        {
            return arDA.AddLinuxUserIntoGroup(linuxUsr, Entitlement);
        }

        #endregion

        public bool DeleteLinuxUser(string LinuxUser)
        {
            return arDA.DeleteLinuxUser(LinuxUser);
        }

        public void DeleteLinuxUserGroupMap(LinuxUserInfo LinuxUser, EntitlementType EntitlementType)
        {
            arDA.DeleteLinuxUserGroupMap( LinuxUser,EntitlementType);
        }

        public bool DeleteOracleUserRoleMap(OracleUMUserInfo OraUser, EntitlementType EntitlementType = EntitlementType.Role)
        {
            return arDA.DeleteOracleUserRoleMap(OraUser, EntitlementType);
        }

        public bool DeleteOracleUser(string oraUserName)
        {
            return arDA.DeleteOracleUser(oraUserName);
        }

        public bool AddOracleUserIntoRole(OracleUMUserInfo oraUsr, EntitlementType Entitlement)
        {
            return arDA.AddOracleUserIntoRole(oraUsr,Entitlement);
        }

        public WorkflowCriteriaBase GetWorkFlowCriteria(AccessType accessType, string UserName, long TaskId, string Comments, string AccountCreatedNotificationTO)
        {
            try
            {
                bool isRequiredMail = false;
                TaskFacade taskFacade = new TaskFacade();
                DI.Chakra.Model.Task task = taskFacade.GetProvisioningTask(TaskId, true);

                if (task.IsGroupTask && task.AssignedBy == null && task.AssignedTo == null)
                {
                    task.AssignedTo = task.ActionUserId;
                    task.AssignedBy = task.ActionUserId;
                    taskFacade.UpdateTask(task);
                }
                if (accessType == AccessType.Approve)
                {
                    task.AccessRequest.RequestStatus = AccessRequestStatus.Approved;
                }
                else if (accessType == AccessType.Revoke)
                {
                    task.AccessRequest.RequestStatus = AccessRequestStatus.Rejected;
                    isRequiredMail = true;
                }

                task.LastModifiedBy = UserName;
                task.LastModifiedDate = DateTime.Now;

                task.AccessRequest.LastModifiedBy = UserName;
                task.AccessRequest.LastModifiedDate = DateTime.Now;
                task.AccessRequest.Comments = Comments;

                this.AddToInterimStore(task);

                WorkflowCriteriaBase _criteria = new ApprovalActionCriteria()
                {
                    RequestType = (RequestType)task.RequestTypeId,
                    TaskId = task.Id,
                    WorkflowInstanceId = task.WorkflowInstanceId
                };

                if (isRequiredMail)
                    SendEmailForAccessRequestRejected(task, AccountCreatedNotificationTO);

                return _criteria;
            }
            catch
            {
                throw;
            }
        }

        private void AddToInterimStore(DI.Chakra.Model.Task task)
        {
            InterimStoreFacade interimStoreFacade = new InterimStoreFacade();
            InterimStore interimStore = new InterimStore()
            {
                InterimStoreItemType = InterimStoreItemType.Task,
                IsActive = true,
                IsSaved = true,
                TaskId = task.Id,
                XmlData = UtilityMethods.Serialize<DI.Chakra.Model.Task>(task),
            };

            interimStoreFacade.AddToInterimStore(interimStore);
        }

        private void SendEmailForAccessRequestRejected(DI.Chakra.Model.Task task, string AccountCreatedNotificationTO)
        {
            try
            {

                EmailInfo emailInfo = new EmailInfo();

                DiConfigurationFacade diconfigurationfacade = new DiConfigurationFacade();

                MailTemplate mailTemplate=new MailTemplate();// = diconfigurationfacade.GetMailTemplate(Constants.AccessRequestRejected); need to implement
                if (mailTemplate == null) return;

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);


                string toMailid = string.Empty;

                if (task.AccessRequest != null)
                {
                    mailSubject.Replace("@@ApplicationName@@", ApplicationName(task.AccessRequest.AdHocAccessRequest.EndpointSystemType));
                    mailSubject.Replace("@@REQUESTTYPE@@", task.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                    mailSubject.Replace("@@REQUESTID@@", task.AccessRequestId.ToString());
                    mailSubject.Replace("@@USER@@", task.AccessRequest.User.DisplayName);
                    mailSubject.Replace("@@pernrNum@@", task.AccessRequest.User.EmployeeId);

                    UserFacade userfacade = new UserFacade();
                    User user = userfacade.GetUser(task.AccessRequest.RequesterId);
                    body.Replace("@@USER@@", user.DisplayName);
                    body.Replace("@@APPROVER@@", task.AccessRequest.LastModifiedBy);
                    body.Replace("@@DATE@@", DateTime.Now.ToString());
                    body.Replace("@@REQUESTID@@", task.AccessRequestId.ToString());
                    body.Replace("@@ApplicationName@@", Convert.ToString(task.AccessRequest.AdHocAccessRequest.EndpointSystemType));
                    body.Replace("@@REQUESTTYPE@@", task.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                    body.Replace("@@approverComments@@", task.AccessRequest.Comments);
                    body.Replace("@@pernrNum@@", task.AccessRequest.User.EmployeeId);


                    toMailid = (user == null && string.IsNullOrWhiteSpace(user.EmailId)) ? string.Empty : Convert.ToString(user.EmailId);

                }

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);
                if (string.IsNullOrEmpty(toMailid))
                {
                    emailInfo.Cc = string.Format("{0}", AccountCreatedNotificationTO);
                }
                //emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                //EmailHandler.Send(emailInfo);
                return;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, ex.Message, ex);
                return;
            }
        }

        private string ApplicationName(EndpointType endpointType)
        {
            string returnValue = string.Empty;
            switch (endpointType)
            {
                case EndpointType.ActiveDirectory:
                    returnValue = "Active Directory";
                    break;

                case EndpointType.LotusNotes:
                    returnValue = "EMAIL";
                    break;

                case EndpointType.SAP:
                    returnValue = "SAP";
                    break;

                default:
                    returnValue = string.Empty;
                    break;
            }
            return returnValue;
        }
    }
}

using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.SAP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DeepIdentity.Model.ReportData;
using System.Xml.Serialization;
using System.IO;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.Facade
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

        public int AddADSUserInfo(ADSUserInfo adUsr)
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
        public int AddSAPUserInfo(SAPUserinfo sapUsr)
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
        public int AddSAPEPUserInfo(SAPEPUserInfo sapEPUsr)
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
        public bool DeleteADUser(int ADUserId)
        {
            return arDA.DeleteADUser(ADUserId);
        }
        public bool DeleteADUserGroupMap(ADSUserInfo adUser)
        {
            return arDA.DeleteADUserGroupMap(adUser);
        }

        public bool DeleteSAPUser(int sapUserId)
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
        public bool DeleteLotusNotesUser(int LNUserId)
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

        public bool CheckIfSameRequestsPending(AdHocAccessRequest _obj, out int RequesterID, out long AccessRequestID)
        {
            return arDA.CheckIfSameRequestsPending(_obj, out RequesterID, out AccessRequestID);
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

        public IList<MyRequestsGridData> GetRequestsByActionUserId(int actionUserId)
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

        public IList<MyRequestsGridData> GetRequestsByRequester(List<int> RequesterIDs)
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
    }
}

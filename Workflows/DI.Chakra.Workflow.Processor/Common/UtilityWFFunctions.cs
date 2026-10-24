
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Workflow.Processor.Common
{
    public class UtilityWFFunctions
    {
        public DateTime GetTaskCompletionDate(int responseTimeInDays, DateTime taskCreatedDate)
        {
            return taskCreatedDate.AddDays(responseTimeInDays);
        }

        public int GetDepartmentHeadByUserId(long userId)
        {
            NOrganisation organisation = NOrganisation.GetById(GetUserByUserId(userId).NOrganisationId.Value);
            if (organisation != null)
            {
                return (int)NOrganisationHead.GetOrganisationHead(organisation.Id);
            }
            return 0;
        }

        public long GetManagerByUserId(long userId)
        {
            var user = User.GetById(userId);
            return NOrganisationHead.GetImmediateSupervisor(userId).Id;
        }

        public long GetSupervisorByUserId(long userId)
        {
            var user = User.GetById(userId);
            return 0;// User.GetNSupervisorByUser(user);
        }

        public DomainModel.MailTemplate GetMailTemplate(string templateName)
        {
            return MailTemplate.GetByTemplateName(templateName);
        }

        public User GetUserByUserId(long userId)
        {
            return User.GetById(userId);
        }

        public long GetUserIdByEmployeeId(string employeeId)
        {
            var user = User.GetByEmployeeId(employeeId);
            if (user != null && user.IsActive)
            {
                return user.Id;
            }
            return 0;
        }

        public User GetUserByEmployeeId(string employeeId)
        {
            var user = User.GetByEmployeeId(employeeId);
            return user;
        }

        public int GetGroupIdByName(string groupName)
        {
            var group = Group.GetByGroupName(groupName);
            if (group != null)
            {
                return (int)group.Id;
            }
            return 0;
        }

        public string GetGroupNameById(long groupId)
        {
            var group = Group.GetById(groupId);
            if (group != null)
            {
                return group.GroupName;
            }
            return string.Empty;
        }

        public Group GetGroupById(long groupId)
        {
            var group = Group.GetById(groupId);
            if (group != null)
            {
                return group;
            }
            return null;
        }

        public string GetApprover(WorkflowConfigurationDetail currentApprover, bool forDisplay = false)
        {
            string approverType = string.Empty;

            if (currentApprover.ApprovalUserType == ApprovalUserType.User)
            {
                approverType = "User";
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
            {
                approverType = "Group";
                var grpMember = Group.GetByIdIncludingOnlyUser(currentApprover.Approver);
                if (grpMember != null)
                {
                    string groupName = grpMember.GroupName;
                    approverType += " - " + groupName;
                }

                try
                {
                    if (forDisplay && grpMember != null && grpMember.Users != null && grpMember.Users.Count > 0)
                    {
                        var members = grpMember.Users.Select(a => string.Format("{0}({1})", a.DisplayName, a.UserName));
                        approverType += string.Format("<br />({0})", string.Join(", <br />", members));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
            {
                var approver = RoleTypes.GetById(currentApprover.Approver);

                bool isApproverFromOrgScope = NOrganisationScope.GetAll().Where(t => t.Scope == approver.RoleTypeDesc).Any();

                if (isApproverFromOrgScope)
                {
                    approverType = RoleTypes.GetAll().Where(t => t.RoleTypeDesc == approver.RoleTypeDesc).Select(a => a.DisplayName).FirstOrDefault();

                }
                else
                {
                    approverType = UtilityMethods.GetEnumAttribute<Description>((RoleType)currentApprover.Approver);
                }
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
            {
                approverType = UtilityMethods.GetEnumAttribute<Description>((RoleType)currentApprover.Approver);
            }

            return approverType;
        }

        public string GetApprover(string groupName, bool forDisplay = false)
        {
            string approverType = string.Empty;

            if (groupName.ToLower().StartsWith("group") && groupName.Split('-').Length > 1)
                groupName = groupName.Split('-')[1].Trim();

            approverType = "Group";
            var grpMember = Group.GetByGroupName(groupName);
            if (grpMember != null)
            {
                string grpName = grpMember.GroupName;
                approverType += " - " + grpName;
            }

            try
            {
                if (forDisplay && grpMember != null && grpMember.Users != null && grpMember.Users.Count > 0)
                {
                    var members = grpMember.Users.Select(a => string.Format("{0}({1})", a.DisplayName, a.UserName));
                    approverType += string.Format("<br />({0})", string.Join(", <br />", members));
                }
            }
            catch (Exception ex)
            {

            }

            return approverType;
        }

        public List<long> GetUsersByGroupId(long groupId)
        {
            Group group = Group.GetByIdIncludingOnlyUser(groupId);
            if (group != null && group.Users.Count == 0) return new List<long>();
            return group.Users.Select(item => item.Id).ToList();
        }

        public List<long> GetUsersByGroupName(string groupName)
        {
            long groupId = GetGroupIdByName(groupName);
            if (groupId != 0)
            {
                Group group = Group.GetByIdIncludingOnlyUser(groupId);
                if (group != null && group.Users.Count == 0) return new List<long>();
                return group.Users.Select(item => item.Id).ToList();
            }
            else
            {
                return new List<long>();
            }
        }

        public List<long> GetUserIdByGroupName(string groupName)
        {
            var group = Group.GetByGroupName(groupName);
            if (group != null)
            {
                return GetUsersByGroupId(group.Id);
            }
            return new List<long>();
        }

        public void DeleteAutoCreatedADForVPN(AccessRequest request, AdHocAccessRequest adhocReq)
        {
            try
            {
                if (request.IsParentRequest)
                {
                    var endpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var adUser = ADSUserInfo.GetActivePrimaryByDIUserId(adhocReq.RequesteeUserId, endpoint.Id);

                    if (adUser == null)
                    {
                        return;
                    }

                    AccessRequest newRequest = PopulateNewFromOldAccessRequest(request);
                    newRequest.AdHocAccessRequest = PopulateNewFromOldAdhocRequest(adhocReq);
                    newRequest.IsDependentRequest = false;
                    newRequest.ParentRequestId = null;
                    //alter new request
                    newRequest.Id = 0;
                    newRequest.AdHocAccessRequestId = 0;
                    newRequest.AdHocAccessRequest.Id = 0;
                    newRequest.AdHocAccessRequest.AdhocActionType = AdHocActionType.DeleteAccount;
                    newRequest.EndpointSystemId = endpoint.Id;
                    newRequest.AdHocAccessRequest.EndpointSystemId = endpoint.Id;
                    AdHocADRequest adDetail = new AdHocADRequest();
                    adDetail.EndpointUserId = adUser.Id;
                    newRequest.AdHocAccessRequest.EntitlementDetail = newRequest.AdHocAccessRequest.GetSerializedString(adDetail);
                    //save new request
                    List<AccessRequest> newRequests = new List<AccessRequest>();
                    newRequests.Add(newRequest);
                    AccessRequest.SaveAccessRequestList(newRequests);

                    request.IsParentRequest = true;
                    request.IsDependentRequestCreated = true;
                    request.ParentRequestId = newRequest.Id;
                    request.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while creating delete AD account task for rejected VPN request.";
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                throw ex;
            }
        }

        public void DeleteAutoCreatedADForVPN(BulkRequest bulkRequest)
        {
            try
            {
                var childRequests = AccessRequest.GetChildRequestByBulkRequestId(bulkRequest.Id);
                var endpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);

                foreach (AccessRequest childRequest in childRequests)
                {
                    var adUser = ADSUserInfo.GetActivePrimaryByDIUserId(childRequest.UserId, endpoint.Id);

                    if (adUser == null)
                    {
                        return;
                    }

                    AccessRequest newRequest = PopulateNewFromOldAccessRequest(childRequest);
                    newRequest.AdHocAccessRequest = PopulateNewFromOldAdhocRequest(childRequest.AdHocAccessRequest);

                    childRequest.IsParentRequest = true;
                    childRequest.IsDependentRequestCreated = true;

                    newRequest.IsDependentRequest = true;
                    newRequest.ParentRequestId = childRequest.Id;

                    childRequest.Save(MasterConfigHelper.ServiceUser);

                    //alter new request
                    newRequest.Id = 0;
                    newRequest.AdHocAccessRequestId = 0;
                    newRequest.AdHocAccessRequest.Id = 0;

                    newRequest.AdHocAccessRequest.AdhocActionType = AdHocActionType.DeleteAccount;

                    newRequest.AdHocAccessRequest.EndpointSystemId = endpoint.Id;

                    AdHocADRequest adDetail = new AdHocADRequest();
                    adDetail.EndpointUserId = adUser.Id;

                    newRequest.AdHocAccessRequest.EntitlementDetail = newRequest.AdHocAccessRequest.GetSerializedString(adDetail);

                    //save new request
                    List<AccessRequest> newRequests = new List<AccessRequest>();
                    newRequests.Add(newRequest);
                    AccessRequest.SaveAccessRequestList(newRequests);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while creating delete AD account task for rejected VPN bulk request.";
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                throw ex;
            }
        }

        public AccessRequest PopulateNewFromOldAccessRequest(AccessRequest oldRequest)
        {
            return new AccessRequest()
            {
                Id = 0,
                AdHocAccessRequestId = 0,

                AccountType = oldRequest.AccountType,
                ApplicationType = oldRequest.ApplicationType,
                ApproverType = oldRequest.ApproverType,
                Comments = oldRequest.Comments,
                CreatedBy = oldRequest.CreatedBy,
                CreatedDate = oldRequest.CreatedDate,
                DeactivationDate = oldRequest.DeactivationDate,
                Description = oldRequest.Description,
                IsCurrentLevelApproved = oldRequest.IsCurrentLevelApproved,
                LastModifiedBy = oldRequest.LastModifiedBy,
                LastModifiedDate = oldRequest.LastModifiedDate,
                RequesterId = oldRequest.RequesterId,
                RequestSource = oldRequest.RequestSource,
                TransactionId = oldRequest.TransactionId,
                UserId = oldRequest.UserId,

                ActivationDate = DateTime.Now,

                RequestStatus = AccessRequestStatus.Granted,
                ProgressStatus = AccessRequestProgress.ReadytoExecute,
                ExecutionTime = DateTime.Now,

                OrgBranch = oldRequest.OrgBranch,
                OrgCompany = oldRequest.OrgCompany,
                OrgDepartment = oldRequest.OrgDepartment,
                OrgDivision = oldRequest.OrgDivision,
                OrgSection = oldRequest.OrgSection,
                OrgUnit = oldRequest.OrgUnit,

                Remarks = "This request is created by system to handle the auto creation/deletion of AD account for VPN/O365 request - ." + oldRequest.Id
            };
        }

        public AdHocAccessRequest PopulateNewFromOldAdhocRequest(AdHocAccessRequest adhocaccessrequest)
        {
            return new AdHocAccessRequest()
            {
                Id = 0,

                AccountType = adhocaccessrequest.AccountType,
                Comments = adhocaccessrequest.Comments,
                CreatedBy = adhocaccessrequest.CreatedBy,
                CreatedDate = adhocaccessrequest.CreatedDate,
                IsSelfRequest = adhocaccessrequest.IsSelfRequest,
                RequesteeUserId = adhocaccessrequest.RequesteeUserId,
                RequesterUserId = adhocaccessrequest.RequesterUserId,
                ValidFrom = adhocaccessrequest.ValidFrom,
                ValidTo = adhocaccessrequest.ValidTo,

                EndpointSystemType = EndpointType.ActiveDirectory,
                ApplicationType = ApplicationType.ADSingaporePowerLocal,
            };
        }

        public AccessRequest PopulateAccessRequestFromBulkRequest(BulkRequest bulkRequest, BulkRequestDetail bulkRequestDetail)
        {
            return new AccessRequest()
            {
                Id = 0,
                AdHocAccessRequestId = 0,

                AccountType = AccountType.User,
                ApplicationType = bulkRequest.ApplicationType,
                ApproverType = bulkRequest.ApproverType,
                Comments = bulkRequest.Comments,
                CreatedBy = bulkRequest.CreatedBy,
                CreatedDate = bulkRequest.CreatedDate,
                DeactivationDate = bulkRequest.DeactivationDate,
                Description = bulkRequest.Description,
                IsCurrentLevelApproved = bulkRequest.IsCurrentLevelApproved,
                LastModifiedBy = bulkRequest.LastModifiedBy,
                LastModifiedDate = bulkRequest.LastModifiedDate,
                RequesterId = bulkRequest.RequestedBy,
                RequestSource = bulkRequest.RequestSource,
                UserId = bulkRequestDetail.UserId,

                BulkRequestId = bulkRequest.Id,

                ActivationDate = DateTime.Now,

                RequestStatus = AccessRequestStatus.Granted,
                ProgressStatus = AccessRequestProgress.ReadytoExecute,
                ExecutionTime = DateTime.Now,

                OrgBranch = bulkRequestDetail.OrgBranch,
                OrgCompany = bulkRequestDetail.OrgCompany,
                OrgDepartment = bulkRequestDetail.OrgDepartment,
                OrgDivision = bulkRequestDetail.OrgDivision,
                OrgSection = bulkRequestDetail.OrgSection,
                OrgUnit = bulkRequestDetail.OrgUnit,

                Remarks = "This request is created by system to handle the auto creation/deletion of AD account for VPN/O365 bulk request - ." + bulkRequest.Id
            };
        }

        public AdHocAccessRequest PopulateAdhocRequestFromBulkRequest(BulkRequest bulkRequest, BulkRequestDetail bulkRequestDetail)
        {
            return new AdHocAccessRequest()
            {
                Id = 0,

                AccountType = AccountType.User,
                Comments = bulkRequest.Comments,
                CreatedBy = bulkRequest.CreatedBy,
                CreatedDate = bulkRequest.CreatedDate,
                IsSelfRequest = false,
                RequesteeUserId = bulkRequestDetail.UserId,
                RequesterUserId = bulkRequest.RequestedBy,
                ValidFrom = bulkRequest.ActivationDate,
                ValidTo = bulkRequest.DeactivationDate,

                EndpointSystemType = EndpointType.ActiveDirectory,
                ApplicationType = ApplicationType.ADSingaporePowerLocal,
            };
        }

        public void LogDebugStep(string message)
        {
            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, message, null);
        }

    }
}

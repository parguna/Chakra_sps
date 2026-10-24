using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    class SAPEPBase : FormExecutorBase
    {
        public SAPEPBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdhocSAPEPRequest sapEPDetail = null;
            string roles = string.Empty;

            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapEPDetail = (AdhocSAPEPRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (sapEPDetail != null)
                {
                    fields["Roles"].Value = sapEPDetail.Roles;
                    fields["Groups"].Value = sapEPDetail.Groups;
                    endpointUserId = sapEPDetail.EndpointUserId;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccount(fields, user, fields["Roles"].Value, sapEPDetail);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, endpointUserId, fields);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, endpointUserId, fields);
                }
            }

            return isExecutedSuccessfully;
        }
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            try
            {
                var sapepUsrs = SAPEPUserInfo.GetActiveAccounts(user.Id, this.Endpoint.Id, CurrentRequest.AccountType);

                if (sapepUsrs != null)
                {
                    foreach (SAPEPUserInfo sapepUsr in sapepUsrs)
                    {
                        this.TakeOUSnapshot(user, sapepUsr, EmployeeEvents.Resignation);
                        this.LockUser(sapepUsr, LockScenario.OffBoard);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            //if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
            //    return true;    
            try
            {

                bool isUpdated = true;

                var sapepUsrs = SAPEPUserInfo.GetActiveAccountByDIUserIdByEndpointId(user.Id, this.Endpoint.Id);

                if (sapepUsrs != null)
                {
                    foreach (SAPEPUserInfo sapepUsr in sapepUsrs)
                    {
                        sapepUsr.DisplayName = user.KnownAs;
                        sapepUsr.LastName = user.KnownAs;
                        sapepUsr.JobTitle = user.IdentificationNo;
                        sapepUsr.Department = user.OrgDepartment;
                        sapepUsr.Email = user.EmailId;

                        isUpdated = EndpointConnector.UpdateAccount(sapepUsr);

                        if (isUpdated)
                        {
                            sapepUsr.Save(MasterConfigHelper.ServiceUser);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapepUsr.UserName), null);
                        }
                    }
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the propagation request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;
            try
            {


                bool isUpdated = true;
                var sapepUsrs = SAPEPUserInfo.GetActiveAccountByDIUserIdByEndpointId(user.Id, this.Endpoint.Id);

                if (sapepUsrs != null)
                {
                    foreach (SAPEPUserInfo sapepUsr in sapepUsrs)
                    {
                        sapepUsr.Department = user.OrgDepartment;
                        sapepUsr.JobTitle = user.IdentificationNo;

                        isUpdated = EndpointConnector.ChangeContainer(sapepUsr);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} modified  in SAPEP.", sapepUsr.AccountName), null);
                    }
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the re-org request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared || CurrentRequest.AccountType == AccountType.Secondary)
                    return true;

                bool isUpdated = true;

                var sapepUsr = SAPEPUserInfo.GetByIdEndPointSystemId(this.CurrentRequest.EventUndoRequest.EndpointUserId, this.CurrentRequest.EventUndoRequest.EndpointSystemId);

                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    var epUser = SAPEPUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                    if (epUser == null && sapepUsr != null)
                    {
                        this.UnlockUser(sapepUsr);
                        this.ReOrg(user, fields);
                    }
                }
                else if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    this.ReOrg(user, fields);
                    var existingRoles = SAPEPUserInfo.GetByIdIncludeRole(sapepUsr.Id).SAPEPRoles.Select(item => item.UniqueRoleName).ToList();

                    if (existingRoles.Count > 0)
                    {
                        sapepUsr.Roles = existingRoles;

                        bool isSucces = EndpointConnector.RemoveEntitlement(sapepUsr, EntitlementType.Role);

                        if (isSucces)
                        {
                            SAPEPUserInfo.DeleteSAPEPUserRoleMap(sapepUsr);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", existingRoles, sapepUsr.UserName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", existingRoles, sapepUsr.UserName), null);
                        }
                    }

                    var existingGroups = SAPEPUserInfo.GetByIdIncludeGroup(sapepUsr.Id).SAPEPGroups.Select(item => item.GroupUniqueName).ToList();

                    if (existingGroups.Count > 0)
                    {
                        sapepUsr.Groups = existingGroups;

                        bool isSucces = EndpointConnector.RemoveEntitlement(sapepUsr, EntitlementType.Group);

                        if (isSucces)
                        {
                            SAPEPUserInfo.DeleteSAPEPUserGroupMap(sapepUsr);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} removed successfully for the user {1}.", existingGroups, sapepUsr.UserName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing groups - {0} for the user {1}.", existingGroups, sapepUsr.UserName), null);
                        }
                    }
                }

                string roles = this.CurrentRequest.EventUndoRequest.TechnicalRoles;
                string groups = this.CurrentRequest.EventUndoRequest.TechnicalGroups;

                if (!string.IsNullOrEmpty(roles))
                {
                    sapepUsr.Roles = UtilityMethods.GetListFromString(roles);

                    sapepUsr.Roles = sapepUsr.Roles.Distinct().ToList();

                    bool isAdded = EndpointConnector.AddEntitlement(sapepUsr, EntitlementType.Role);
                    if (isAdded)
                    {
                        SAPEPUserInfo.AddEntitlement(sapepUsr.Id, EntitlementType.Role, roles);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} added successfully for the user {1}.", roles, sapepUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding roles - {0} for the user {1}.", roles, sapepUsr.UserName), null);
                    }
                }

                if (!string.IsNullOrEmpty(groups))
                {
                    sapepUsr.Groups = UtilityMethods.GetListFromString(groups);

                    sapepUsr.Groups = sapepUsr.Groups.Distinct().ToList();

                    bool isAdded = EndpointConnector.AddEntitlement(sapepUsr, EntitlementType.Group);
                    if (isAdded)
                    {
                        SAPEPUserInfo.AddEntitlement(sapepUsr.Id, EntitlementType.Group, groups);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} added successfully for the user {1}.", groups, sapepUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding groups - {0} for the user {1}.", groups, sapepUsr.UserName), null);
                    }
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;
            try
            {

                var sapepUsrs = SAPEPUserInfo.GetActiveAccounts(user.Id, this.Endpoint.Id, CurrentRequest.AccountType);

                foreach (var sapepUsr in sapepUsrs)
                //if (sapepUsr != null)
                {
                    sapepUsr.Department = user.OrgDepartment;
                    sapepUsr.JobTitle = user.IdentificationNo;

                    this.TakeOUSnapshot(user, sapepUsr, EmployeeEvents.OUChange);

                    var tempUserRoles = SAPEPUserInfo.GetByIdIncludeRole(sapepUsr.Id);
                    var tempUserGroups = SAPEPUserInfo.GetByIdIncludeGroup(sapepUsr.Id);

                    sapepUsr.Roles = tempUserRoles != null ? tempUserRoles.SAPEPRoles.Where(item => item.IsXLUploaded).Select(item => item.RoleName).ToList() : new List<string>();
                    sapepUsr.Groups = tempUserGroups != null ? tempUserGroups.SAPEPGroups.Where(item => item.IsXLUploaded).Select(item => item.GroupName).ToList() : new List<string>();

                    string roleListStr = string.Join(",", sapepUsr.Roles.Select(item => item));

                    WriteProvisioningLog(LogLevelType.Info, $"Transfer SAPEP - Started Removing EP Roles -{roleListStr} for User -{sapepUsr.AccountName}", null);

                    bool isSuccess = EndpointConnector.RemoveEntitlement(sapepUsr, EntitlementType.Role);
                    if (isSuccess)
                    {
                        SAPEPUserInfo.DeleteSAPEPUserRoleMap(sapepUsr);
                        sapepUsr.Roles = SAPEPUserInfo.GetByIdIncludeRole(sapepUsr.Id).SAPEPRoles.Select(item => item.RoleName).ToList();
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapepUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapepUsr.UserName), null);
                    }

                    roleListStr = string.Join(",", sapepUsr.Groups.Select(item => item));
                    WriteProvisioningLog(LogLevelType.Info, $"Transfer SAPEP - Started Removing EP Groups -{roleListStr} for User -{sapepUsr.AccountName}", null);

                    isSuccess = EndpointConnector.RemoveEntitlement(sapepUsr, EntitlementType.Group);

                    if (isSuccess)
                    {
                        SAPEPUserInfo.DeleteSAPEPUserGroupMap(sapepUsr);
                        sapepUsr.Groups = SAPEPUserInfo.GetByIdIncludeGroup(sapepUsr.Id).SAPEPGroups.Select(item => item.GroupName).ToList();
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} removed successfully for the user {1}.", roleListStr, sapepUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing groups - {0} for the user {1}.", roleListStr, sapepUsr.UserName), null);
                    }

                    WriteProvisioningLog(LogLevelType.Info, $"Transfer SAPEP - Started Updating SAPEP Userinfo {sapepUsr.AccountName}", null);

                    EndpointConnector.ChangeContainer(sapepUsr);

                    //Default Groups Assignment
                    string groupsBasedOnOrg = ProvisioningConfigMatrixSAPOrg.GetSAPEPGroupBasedOnUserOrg(user);

                    WriteProvisioningLog(LogLevelType.Info, $"Transfer SAPEP - Started Assiging Default EP Groups -{groupsBasedOnOrg} for User -{sapepUsr.AccountName}", null);

                    if (!string.IsNullOrEmpty(groupsBasedOnOrg))
                    {
                        var tempGrps = new List<string>();
                        sapepUsr.Groups = SAPEPUserInfo.GetByIdIncludeGroup(sapepUsr.Id).SAPEPGroups.Select(item => item.GroupName).ToList();

                        foreach (var item in groupsBasedOnOrg.Split(','))
                        {
                            if (!sapepUsr.Groups.Contains(item))
                                tempGrps.Add(item);
                        }

                        if (tempGrps.Count > 0)
                        {
                            sapepUsr.Groups = new List<string>();
                            sapepUsr.Groups = tempGrps.Distinct().ToList();
                            EndpointConnector.AddEntitlement(sapepUsr, EntitlementType.Group);
                            groupsBasedOnOrg = string.Join(",", sapepUsr.Groups);

                            SAPEPUserInfo.AddEntitlement(sapepUsr.Id, EntitlementType.Group, groupsBasedOnOrg);
                            WriteProvisioningLog(LogLevelType.Warning, string.Format("New default SAPEP group assigned for the user {0} from FLINS matrix.", sapepUsr.AccountName), null);
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Warning, string.Format("No default SAPEP group found for the user {0} from FLINS matrix.", sapepUsr.AccountName), null);
                    }
                    sapepUsr.Save("System");
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} modified  in SAPEP.", sapepUsr.AccountName), null);
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing Transfer request. {0}", ex.Message), ex);
                return false;
            }
        }
        protected bool LockUser(SAPEPUserInfo sapEPUsr, LockScenario lockScenario)
        {
            try
            {
                bool isusrDisabled = true;

                if (sapEPUsr != null)
                {
                    //EP/CE depends on HRMS Flins. On locking of HRMS account EP/CE will be locked internally
                    isusrDisabled = EndpointConnector.LockAccount(sapEPUsr);
                    WriteProvisioningLog(LogLevelType.Info, "Lock successful");
                    sapEPUsr.Roles = SAPEPUserInfo.GetByIdIncludeRole(sapEPUsr.Id).SAPEPRoles.Select(r => r.UniqueRoleName).ToList();
                    sapEPUsr.Groups = SAPEPUserInfo.GetByIdIncludeGroup(sapEPUsr.Id).SAPEPGroups.Select(r => r.GroupUniqueName).ToList();
                    WriteProvisioningLog(LogLevelType.Info, $"Remove Entitlements Role started userid {sapEPUsr.Id} Roles {string.Join(",", sapEPUsr.Roles)} ");
                    bool isSuccess = EndpointConnector.RemoveEntitlement(sapEPUsr, EntitlementType.Role);
                    string roleListStr = string.Join(",", sapEPUsr.Roles.Select(item => item));

                    if (isSuccess)
                    {
                        SAPEPUserInfo.DeleteSAPEPUserRoleMap(sapEPUsr);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapEPUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapEPUsr.UserName), null);
                    }
                    WriteProvisioningLog(LogLevelType.Info, $"Remove Entitlements Group started  userid {sapEPUsr.Id} Groups {string.Join(", ", sapEPUsr.Groups)} ");
                    isSuccess = EndpointConnector.RemoveEntitlement(sapEPUsr, EntitlementType.Group);
                    roleListStr = string.Join(",", sapEPUsr.Groups.Select(item => item));

                    if (isSuccess)
                    {
                        SAPEPUserInfo.DeleteSAPEPUserGroupMap(sapEPUsr);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} removed successfully for the user {1}.", roleListStr, sapEPUsr.UserName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing groups - {0} for the user {1}.", roleListStr, sapEPUsr.UserName), null);
                    }

                    sapEPUsr.LastModifiedBy = "System";
                    sapEPUsr.LastModifiedDate = DateTime.Now;
                    sapEPUsr.Save("System");


                    if (isusrDisabled)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Locked in EP.", sapEPUsr.UserName), null);
                    }
                }

                return isusrDisabled;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing LockUser. {0}", ex.Message), ex);
                return false;
            }
        }
        protected bool UnlockUser(SAPEPUserInfo sapEPUsr)
        {
            try
            {
                bool isusrEnabledd = true;

                if (sapEPUsr != null)
                {
                    isusrEnabledd = false;
                    isusrEnabledd = EndpointConnector.UnLockAccount(sapEPUsr);
                    if (isusrEnabledd)
                    {
                        sapEPUsr.LastModifiedBy = "System";
                        sapEPUsr.LastModifiedDate = DateTime.Now;
                        sapEPUsr.Save("System");
                        if (isusrEnabledd)
                        {
                            WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Unlocked in Internet portal.", sapEPUsr.UserName), null);
                        }
                    }
                }

                return isusrEnabledd;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing UnlockUser. {0}", ex.Message), ex);
                return false;
            }
        }
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string roles, AdhocSAPEPRequest sapDetail)
        {

            bool isExecutedSuccessfully = false;

            try
            {
                var sapEPUserValue = SAPEPUserInfo.GetByIdEndPointSystemId(sapDetail.EndpointUserId, Endpoint.Id);

                if (sapEPUserValue != null)
                {
                    isExecutedSuccessfully = UnlockUser(sapEPUserValue);

                    if (isExecutedSuccessfully)
                    {
                        this.ReOrg(user, fields);

                        //SAP EP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrEmpty(roles) ? roles.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (tRole.Trim() != string.Empty)
                                    sapRoles.Add(tRole.Trim());
                            }
                        }

                        string[] sapEPRoles = !string.IsNullOrEmpty(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                        string[] sapEPGroups = !string.IsNullOrEmpty(fields["Groups"].Value) ? fields["Groups"].Value.Split(',') : null;

                        if (sapEPRoles != null)
                        {
                            foreach (string roleId in sapEPRoles)
                            {
                                if (!string.IsNullOrEmpty(roleId) && !sapEPUserValue.Roles.Contains(roleId))
                                {
                                    sapEPUserValue.Roles.Add(roleId);
                                }
                            }
                        }

                        if (sapEPGroups != null)
                        {
                            foreach (string groupName in sapEPGroups)
                            {
                                if (!string.IsNullOrEmpty(groupName) && !sapEPUserValue.Groups.Contains(groupName))
                                {
                                    sapEPUserValue.Groups.Add(groupName);
                                }
                            }
                        }

                        bool isRoleUpdated = true; //if no role or group selected then default value is true.
                        bool isGroupUpdated = true; //if no role or group selected then default value is true.
                        string roleListStr = string.Empty;
                        string groupListStr = string.Empty;

                        if (sapEPRoles != null)
                        {
                            isRoleUpdated = EndpointConnector.AddEntitlement(sapEPUserValue, EntitlementType.Role);
                            roleListStr = this.getGroupListAsString(sapEPRoles.ToList<string>());

                            if (isRoleUpdated)
                            {
                                SAPEPUserInfo.AddEntitlement(sapEPUserValue.Id, EntitlementType.Role, string.Join(",", roleListStr));
                                WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} added successfully for the user {1}.", roleListStr, sapEPUserValue.AccountName), null);
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding roles - {0} for the user {1}.", roleListStr, sapEPUserValue.AccountName), null);
                            }
                        }

                        if (sapEPGroups != null)
                        {
                            isGroupUpdated = EndpointConnector.AddEntitlement(sapEPUserValue, EntitlementType.Group);
                            groupListStr = this.getGroupListAsString(sapEPGroups.ToList<string>());

                            if (isGroupUpdated)
                            {
                                SAPEPUserInfo.AddEntitlement(sapEPUserValue.Id, EntitlementType.Group, string.Join(",", groupListStr));
                                WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} added successfully for the user {1}.", groupListStr, sapEPUserValue.AccountName), null);
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding groups - {0} for the user {1}.", groupListStr, sapEPUserValue.AccountName), null);
                            }
                        }

                        sapEPUserValue.LastModifiedBy = MasterConfigHelper.ServiceUser;
                        sapEPUserValue.LastModifiedDate = DateTime.Now;
                        sapEPUserValue.Save(MasterConfigHelper.ServiceUser);

                        sapEPUserValue.Password = ConstructPassword();
                        EndpointConnector.ChangePassword(sapEPUserValue.UserName, sapEPUserValue.Password, user.Id, true);

                        if (isRoleUpdated && isGroupUpdated)
                        {
                            string Entitlment = roleListStr + "," + groupListStr;
                            ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, Entitlment, sapEPUserValue.AccountName);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating SAP EP account for the user {0}", user.DisplayName), ex);
            }

            return isExecutedSuccessfully;
        }
        public bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapEPUserValue = SAPEPUserInfo.GetByIdEndPointSystemId(endpointUserId, Endpoint.Id);
                bool isRemovedSuccessfully = this.LockUser(sapEPUserValue, LockScenario.AdhocRequest);
                if (isRemovedSuccessfully)
                {

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPEPUserDeleted, sapEPUserValue.UserName, sapEPUserValue.EndPointSystemId), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPEPUserDeleted, sapEPUserValue.UserName, sapEPUserValue.EndPointSystemId), null);
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPEPUserDeletionFailed, sapEPUserValue.UserName, sapEPUserValue.EndPointSystemId), null);
                }
                return isRemovedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the SAPEP account. {0}", ex.Message), ex);
                return false;
            }
        }
        protected bool AmendentAdd(DomainModel.User user, long endpointUserId, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;

            try
            {


                var sapEPUserValue = SAPEPUserInfo.GetByIdEndPointSystemId(endpointUserId, Endpoint.Id);

                if (sapEPUserValue != null)
                {

                    string[] sapEPRoles = !string.IsNullOrEmpty(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    string[] sapEPGroups = !string.IsNullOrEmpty(fields["Groups"].Value) ? fields["Groups"].Value.Split(',') : null;

                    if (sapEPRoles != null)
                    {
                        sapEPUserValue.Roles = new List<string>();
                        foreach (string roleId in sapEPRoles)
                        {
                            if (!string.IsNullOrEmpty(roleId) && !sapEPUserValue.Roles.Contains(roleId))
                            {
                                sapEPUserValue.Roles.Add(roleId);
                            }
                        }
                    }

                    if (sapEPGroups != null)
                    {
                        sapEPUserValue.Groups = new List<string>();
                        foreach (string groupName in sapEPGroups)
                        {
                            if (!string.IsNullOrEmpty(groupName) && !sapEPUserValue.Groups.Contains(groupName))
                            {
                                sapEPUserValue.Groups.Add(groupName);
                            }
                        }
                    }

                    bool isRoleUpdated = true; //if no role or group selected then default value is true.
                    bool isGroupUpdated = true; //if no role or group selected then default value is true.
                    string roleListStr = string.Empty;
                    string groupListStr = string.Empty;

                    if (sapEPRoles != null)
                    {
                        isRoleUpdated = EndpointConnector.AddEntitlement(sapEPUserValue, EntitlementType.Role);
                        roleListStr = this.getGroupListAsString(sapEPRoles.ToList<string>());

                        if (isRoleUpdated)
                        {
                            SAPEPUserInfo.AddEntitlement(sapEPUserValue.Id, EntitlementType.Role, string.Join(",", sapEPRoles));
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} added successfully for the user {1}.", roleListStr, sapEPUserValue.AccountName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding roles - {0} for the user {1}.", roleListStr, sapEPUserValue.AccountName), null);
                        }
                    }

                    if (sapEPGroups != null)
                    {
                        isGroupUpdated = EndpointConnector.AddEntitlement(sapEPUserValue, EntitlementType.Group);
                        groupListStr = this.getGroupListAsString(sapEPGroups.ToList<string>());

                        if (isGroupUpdated)
                        {
                            SAPEPUserInfo.AddEntitlement(sapEPUserValue.Id, EntitlementType.Group, string.Join(",", sapEPGroups));
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} added successfully for the user {1}.", groupListStr, sapEPUserValue.AccountName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding groups - {0} for the user {1}.", groupListStr, sapEPUserValue.AccountName), null);
                        }
                    }

                    sapEPUserValue.LastModifiedBy = MasterConfigHelper.ServiceUser;
                    sapEPUserValue.LastModifiedDate = DateTime.Now;
                    sapEPUserValue.Save(MasterConfigHelper.ServiceUser);

                    if (isRoleUpdated && isGroupUpdated)
                    {
                        string Entitlment = "Roles:" + roleListStr + ". Groups:" + groupListStr;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, Entitlment, sapEPUserValue.AccountName);
                        return true;
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Amendment ADD SAP EP - Could not find account for the user {0} RequestId -{1}", user.DisplayName, this.CurrentRequest.Id));
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while Amendment ADD SAP EP account for the user {0}", user.DisplayName), ex);
            }

            return isExecutedSuccessfully;
        }
        protected bool AmendentRemove(DomainModel.User user, long endpointUserId, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var sapEPUserValue = SAPEPUserInfo.GetByIdEndPointSystemId(endpointUserId, Endpoint.Id);

                if (sapEPUserValue != null)
                {
                    string[] sapEPRoles = !string.IsNullOrEmpty(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    string[] sapEPGroups = !string.IsNullOrEmpty(fields["Groups"].Value) ? fields["Groups"].Value.Split(',') : null;

                    if (sapEPRoles != null)
                    {
                        foreach (string roleId in sapEPRoles)
                        {
                            if (!string.IsNullOrEmpty(roleId) && !sapEPUserValue.Roles.Contains(roleId))
                            {
                                sapEPUserValue.Roles.Add(roleId);
                            }
                        }
                    }

                    if (sapEPGroups != null)
                    {
                        foreach (string groupName in sapEPGroups)
                        {
                            if (!string.IsNullOrEmpty(groupName) && !sapEPUserValue.Groups.Contains(groupName))
                            {
                                sapEPUserValue.Groups.Add(groupName);
                            }
                        }
                    }

                    bool isRoleUpdated = true; //if no role or group selected then default value is true.
                    bool isGroupUpdated = true; //if no role or group selected then default value is true.
                    string roleListStr = string.Empty;
                    string groupListStr = string.Empty;

                    if (sapEPRoles != null)
                    {
                        isRoleUpdated = EndpointConnector.RemoveEntitlement(sapEPUserValue, EntitlementType.Role);
                        roleListStr = this.getGroupListAsString(sapEPRoles.ToList<string>());

                        if (isRoleUpdated)
                        {
                            SAPEPUserInfo.DeleteSAPEPUserRoleMap(sapEPUserValue);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapEPUserValue.UserName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapEPUserValue.UserName), null);
                        }
                    }
                    if (sapEPGroups != null)
                    {
                        isGroupUpdated = EndpointConnector.RemoveEntitlement(sapEPUserValue, EntitlementType.Group);
                        groupListStr = this.getGroupListAsString(sapEPGroups.ToList<string>());

                        if (isGroupUpdated)
                        {
                            SAPEPUserInfo.DeleteSAPEPUserGroupMap(sapEPUserValue);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Groups - {0} removed successfully for the user {1}.", groupListStr, sapEPUserValue.UserName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing groups - {0} for the user {1}.", groupListStr, sapEPUserValue.UserName), null);
                        }
                    }

                    if (isRoleUpdated && isGroupUpdated)
                    {
                        string Entitlment = "Roles:" + roleListStr + ". Groups:" + groupListStr;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, Entitlment, sapEPUserValue.UserName);
                        isExecutedSuccessfully = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the SAP EP account. {0}", ex.Message), ex);
                isExecutedSuccessfully = false;
            }

            return isExecutedSuccessfully;
        }
    }
}

using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    public class SAPHANABase : FormExecutorBase
    {
        readonly string MasterConfigHelperServiceUser;
        List<ApplicationType> positionApplicationTypes = new List<ApplicationType> { ApplicationType.MSSL, ApplicationType.EBS };
        public SAPHANABase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public long EndpointSystemId
        {
            get
            {
                return Endpoint.Id;
            }
        }
        public void UpdateProvisioningField(ProvisioningParamCollection fields, SAPHANAUserinfo sapUserValue, DomainModel.User user)
        {
            try
            {
                foreach (ProvisioningParam gData in fields)
                {
                    if (gData.Name.ToUpper() == "Function0".ToUpper() || gData.Name.ToUpper() == "Pers_No".ToUpper())
                    {
                        sapUserValue = setUserInfoByAttibuterbValue(sapUserValue, user, gData) as SAPHANAUserinfo;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"SAPHANA UpdateProvisioningField:Setting default value failed for Function0 & Pers_No.", ex);
            }
        }
        protected Object setUserInfoByAttibuterbValue(Object endPointUsr, DomainModel.User usr, ProvisioningParam field)
        {
            if (field == null || usr == null)
                return endPointUsr;

            object atrbValue = null;
            Type atrbType = null;

            List<PropertyInfo> pInfoList = (from pInfo in usr.GetType().GetProperties()
                                            where pInfo.Name == field.Value
                                            select pInfo).ToList<PropertyInfo>();

            if (pInfoList != null && pInfoList.Count == 1)
            {
                atrbValue = pInfoList[0].GetValue(usr, null);
                atrbType = pInfoList[0].PropertyType;

            }

            List<PropertyInfo> endPtpInfoList = (from pInfo in endPointUsr.GetType().GetProperties()
                                                 where pInfo.Name == field.Name && pInfo.PropertyType == atrbType
                                                 select pInfo).ToList<PropertyInfo>();

            if (endPtpInfoList != null && endPtpInfoList.Count == 1 && atrbValue != null)
            {
                endPtpInfoList[0].SetValue(endPointUsr, atrbValue, null);
            }

            return endPointUsr;
        }
        protected virtual bool AmendentAdd(DomainModel.User user, long endpointUserId, string roles)
        {
            try
            {
                var sapUser = SAPHANAUserinfo.GetById(endpointUserId);

                //SAP Roles
                List<string> sapRoles = new List<string>();
                string[] rolesAry = !string.IsNullOrWhiteSpace(roles) ? roles.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string tRole in rolesAry)
                    {
                        if (!string.IsNullOrEmpty(tRole))
                        {
                            sapRoles.Add(tRole.Trim());
                        }
                    }
                }
                sapUser.Roles = sapRoles.Distinct().ToList();

                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUser.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUser.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                sapUser.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

                bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(sapUser, sapUser.UserValidFrom, sapUser.UserValidTo, EntitlementType.Role);
                string grpListStr = string.Join(",", sapUser.Roles);

                if (isExecutedSuccessfully)
                {
                    SAPHANAUserinfo.AddEntitlement(endpointUserId, EntitlementType.Role, grpListStr);

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssigned, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssignmentFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding roles to the sap account. {0}", ex.Message), ex);
                return false;
            }
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            throw new NotImplementedException();
        }
        
        protected override bool AreEntitlementsValid(string[] entitlements)
        {
            var roles = SAPHANARoleInfo.GetByRoleCodesAndEndpointId(this.Endpoint.Id, entitlements);

            List<string> rolesString = new List<string>();
            if (roles != null && roles.Count > 0)
            {
                rolesString = roles.Select(r => r.ROLE_CODE).Distinct().ToList();
            }

            IEnumerable<string> difference = entitlements.Where(s => !string.IsNullOrWhiteSpace(s) && !rolesString.Contains(s, StringComparer.InvariantCultureIgnoreCase)).Select(s => s);
            if (difference.Any())
            {
                WriteProvisioningLog(LogLevelType.Error, $"The entitlements -[ {string.Join(",", difference)} ] are not exists in the discovery data of the target system {this.Endpoint.DisplayName}");
                return false;
            }
            else
            {
                return true;
            }
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {

                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared ||
                   CurrentRequest.AccountType == AccountType.Secondary)
                    return true;

                bool isUpdated = true;

                var sapUsr = SAPHANAUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);
                if (sapUsr != null)
                {

                    sapUsr.Lastname = user.KnownAs;
                    sapUsr.E_Mail = user.EmailId;
                    sapUsr.Tel1_Numbr = user.TelephoneNumber;

                    //For EBS & MSSL we should never overwite the deparment value. It should have the department information from the matrix.
                    if (!positionApplicationTypes.Contains(CurrentRequest.ApplicationType))
                    {
                        sapUsr.Department = user.OrgDepartment;
                    }

                    sapUsr.Function0 = user.IdentificationNo;
                    sapUsr.CostCenter = user.CostCenter;

                    isUpdated = EndpointConnector.UpdateAccount(sapUsr);

                    if (isUpdated)
                    {
                        sapUsr.Save(MasterConfigHelper.ServiceUser);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAPHANA.", sapUsr.UserLogonName), null);
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
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared ||
                    CurrentRequest.AccountType == AccountType.Secondary)
                    return true;

                if (user == null)
                {
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("User is NULL in ReOrg- SAPHANA."), null);
                    return true;
                }

                bool isUpdated = true;
                var sapUsr = SAPHANAUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

                if (sapUsr != null)
                {
                    sapUsr.CostCenter = string.IsNullOrEmpty(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;
                    sapUsr.Function0 = user.IdentificationNo;

                    //For EBS & MSSL we should never overwite the deparment value. It should have the department information from the matrix.
                    if (!positionApplicationTypes.Contains(CurrentRequest.ApplicationType))
                    {
                        sapUsr.Department = user.OrgDepartment;
                    }

                    //EndpointConnector.ChangeContainer(sapUsr);

                    isUpdated = EndpointConnector.ChangeContainer(sapUsr);

                    if (isUpdated)
                    {
                        sapUsr.Save(MasterConfigHelper.ServiceUser);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAPHANA.", sapUsr.UserLogonName), null);
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
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                bool isUpdated = true;

                var sapUsr = SAPHANAUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                var requestEndpoint = DomainModel.EndPoint.GetById(this.CurrentRequest.EndpointSystemId.Value);
                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    if (sapUsr != null)
                    {
                        isUpdated = this.UnlockUser(sapUsr);
                        if (isUpdated)
                        {
                            EndpointConnector.UpdateSSOAttributes(sapUsr, sapUsr.UserSamAccountName, CurrentRequest.AccountType);
                        }
                        this.ReOrg(user, fields);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Existing user account does not exists. Please raise new account. "));
                        return true;
                    }
                }
                else if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    if (requestEndpoint.ApplicationType != ApplicationType.HRMS_FLINS)
                    {
                        if (sapUsr != null)
                        {
                            isUpdated = this.UnlockUser(sapUsr);
                            if (isUpdated)
                            {
                                EndpointConnector.UpdateSSOAttributes(sapUsr, sapUsr.UserSamAccountName, CurrentRequest.AccountType);
                            }
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Existing user account does not exists. Please raise new account. "));
                            return true;
                        }
                    }
                    this.ReOrg(user, fields);
                    var existingRoles = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

                    if (existingRoles.Count > 0)
                    {
                        sapUsr.Roles = existingRoles;

                        bool isSucces = EndpointConnector.RemoveAllEntitlements(sapUsr);

                        if (isSucces)
                        {
                            SAPHANAUserinfo.RemoveAllEntitlements(sapUsr.Id);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", existingRoles, sapUsr.UserLogonName));
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", existingRoles, sapUsr.UserLogonName));
                        }
                    }
                }

                string roles = this.CurrentRequest.EventUndoRequest.TechnicalRoles;

                if (!string.IsNullOrEmpty(roles))
                {
                    sapUsr.Roles = UtilityMethods.GetListFromString(roles);

                    sapUsr.Roles = sapUsr.Roles.Distinct().ToList();

                    sapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);

                    sapUsr.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

                    bool isAdded = EndpointConnector.AddEntitlement(sapUsr, sapUsr.UserValidFrom, sapUsr.UserValidTo, EntitlementType.Role);
                    if (isAdded)
                    {
                        SAPHANAUserinfo.AddEntitlement(sapUsr.Id, EntitlementType.Role, roles);
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Roles - {0} added successfully for the user {1}.", roles, sapUsr.UserLogonName));
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding roles - {0} for the user {1}.", roles, sapUsr.UserLogonName));
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
            return true;
        }

        protected void AddSAPHANAUser(SAPHANAUserinfo sapUser)
        {
            string currentUser = MasterConfigHelper.ServiceUser;

            if (sapUser.UserType == SAPUserType.Communication)
            {
                sapUser.SapUserType = "C";
            }
            else if (sapUser.UserType == SAPUserType.Service)
            {
                sapUser.SapUserType = "S";
            }
            else if (sapUser.UserType == SAPUserType.System)
            {
                sapUser.SapUserType = "B";
            }
            else if (sapUser.UserType == SAPUserType.Reference)
            {
                sapUser.SapUserType = "L";
            }
            else if (sapUser.UserType == SAPUserType.Dialog)
            {
                sapUser.SapUserType = "A";
            }

            sapUser.Save(currentUser);

            CreateUserInReconTable(sapUser, currentUser);
        }
        protected void TakeOUSnapshot(DomainModel.User user, SAPHANAUserinfo endpointUserInfo, EmployeeEvents employeeEvent)
        {

            try
            {
                var usergroups = SAPHANAUserinfo.GetByIdIncludeSAPGroupInfo(endpointUserInfo.Id);
                var userroles = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(endpointUserInfo.Id);
                var userprofiles = SAPHANAUserinfo.GetByIdIncludeSAPProfileInfo(endpointUserInfo.Id);
                var userpositions = SAPHANAUserinfo.GetPositionTextByUser(endpointUserInfo.UserLogonName, endpointUserInfo.EndPointSystemId);

                string group = string.Join(",", usergroups.SAPGroupInfos.Select(item => item.GROUP_CODE).ToList());
                string role = string.Join(",", userroles.SAPRoleInfo.Select(item => item.ROLE_CODE).ToList());
                string profile = string.Join(",", userprofiles.SAPProfileInfos.Select(item => item.PROFILE_CODE).ToList());
                string position = string.Join(",", userpositions);



                if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(profile) && string.IsNullOrEmpty(position))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(group))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = group;
                    snapshot.EntitlementType = EntitlementType.Group;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(role))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = role;
                    snapshot.EntitlementType = EntitlementType.Role;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(profile))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = profile;
                    snapshot.EntitlementType = EntitlementType.Profile;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(position))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = position;
                    snapshot.EntitlementType = EntitlementType.Position;
                    snapshot.Save("System");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "SAPHANA Exception at TakeOUSnapshot", ex);
                throw ex;
            }
        }
        OUChangeSnapshot getOUChangeSnapshot(DomainModel.User user, SAPHANAUserinfo endpointUserInfo, EmployeeEvents employeeEvent)
        {
            return new OUChangeSnapshot()
            {
                BatchId = DateTime.Today.ToString("yyyyMMdd"),
                EnteredDate = DateTime.Now,
                EndpointSystemId = endpointUserInfo.EndPointSystemId,
                EndpointUserId = endpointUserInfo.Id,
                UserId = user.Id,
                EmployeeeEventType = employeeEvent,
                AccountName = endpointUserInfo.AccountName,
                AccountType = CurrentRequest.AccountType,
                RequestId = CurrentRequest.Id
            };
        }
        protected bool UnlockUser(SAPHANAUserinfo sapUsr, bool updateDependentSystem = true)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                //Valid From and Valid To

                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUsr.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                sapUsr.UserValidTo = this.getSAPDateFormat(GetValidTo(CurrentRequest.UserId).Value);


                if (!string.IsNullOrEmpty(sapUsr.UserValidFrom) && !string.IsNullOrEmpty(sapUsr.UserValidTo))
                {
                    EndpointConnector.UpdateAccount(sapUsr);
                }

                isusrDisabled = EndpointConnector.UnLockAccount(sapUsr);
                if (isusrDisabled)
                {
                    sapUsr.IsUserLocked = false;
                    sapUsr.LastModifiedBy = "System";
                    sapUsr.LastModifiedDate = DateTime.Now;

                    sapUsr.Save(MasterConfigHelper.ServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Unlocked in SAPHANA.", sapUsr.UserLogonName), null);
                }
            }

            return isusrDisabled;
        }
        protected virtual bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPHANAUserinfo.GetById(endpointUserId);

                if (sapUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Exception, $"Could not find SAPHANA User object for the EndpointUserID : {endpointUserId}", null);
                    return false;
                }

                switch (sapUser.SapUserType.Trim().ToUpper())
                {
                    case "C":
                        sapUser.UserType = SAPUserType.Communication;
                        break;
                    case "S":
                        sapUser.UserType = SAPUserType.Service;
                        break;
                    case "B":
                        sapUser.UserType = SAPUserType.System;
                        break;
                    case "L":
                        sapUser.UserType = SAPUserType.Reference;
                        break;
                    case "A":
                        sapUser.UserType = SAPUserType.Dialog;
                        break;
                }

                bool isSuccess = LockUser(sapUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Account is locked for User:{sapUser.UserLogonName}");
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, sapUser.UserLogonName);
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the SAPHANA account. {0}", ex.Message), ex);
                return false;
            }
        }
        protected bool LockUser(SAPHANAUserinfo sapUsr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today);
                EndpointConnector.UpdateAccount(sapUsr);
                isusrDisabled = EndpointConnector.LockAccount(sapUsr);
                if (isusrDisabled)
                {
                    if (this.CurrentRequest != null && this.CurrentRequest.AccountType == AccountType.User)
                    {
                        EndpointConnector.UpdateSSOAttributes(sapUsr, null, this.CurrentRequest.AccountType);
                    }
                    sapUsr.IsUserLocked = true;
                    sapUsr.LastModifiedBy = "System";
                    sapUsr.LastModifiedDate = DateTime.Now;

                    sapUsr.Save(MasterConfigHelper.ServiceUser);
                    if (isusrDisabled)
                    {
                        EndpointConnector.RemoveAllEntitlements(sapUsr);

                        SAPHANAUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAPHANA.", sapUsr.UserLogonName), null);
                    }
                }
            }

            return isusrDisabled;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
        protected bool IsSAPHANAUserGroupExists(string groupCode)
        {

            var group = SAPHANAGroupInfo.GetByEndPointSystemId(Endpoint.Id, groupCode);

            if (group != null)
            {
                return true;
            }

            return false;
        }
        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            LogDebugStep($"Extension Endpoint: {Endpoint.DisplayName}");

            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
            {
                WriteProvisioningLog(LogLevelType.Info, $"Skipping the request, Account Type:{CurrentRequest.AccountType}");
                return true;
            }

            if (!CurrentRequest.EmployeeEventTriggerId.HasValue)
            {
                WriteProvisioningLog(LogLevelType.Info, $"Failing the extension request, EmployeeEventTriggerId:{CurrentRequest.EmployeeEventTriggerId}");
                return true;
            }

            bool isUpdated = true;
            try
            {
                var oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;

                if (!oldTerminationDate.HasValue) return false;

                var accountRecons = AccountReconcilation.GetActiveAccountRecons(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                WriteProvisioningLog(LogLevelType.Info, $"Accounts found for extension count :{accountRecons?.Count}");

                foreach (AccountReconcilation accountRecon in accountRecons)
                {
                    try
                    {
                        if (accountRecon.ValidToDate.HasValue &&
                            (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value))
                        {
                            var sapUserInfo = SAPHANAUserinfo.GetById(accountRecon.EndpointUserId);

                            if (sapUserInfo != null)
                            {
                                sapUserInfo.UserValidTo = UtilityMethods.GetSAPDateString(user.TerminationDate.Value);

                                sapUserInfo.LastModifiedBy = "System";
                                sapUserInfo.LastModifiedDate = DateTime.Now;

                                isUpdated = EndpointConnector.ExtendValidity(sapUserInfo);

                                if (isUpdated)
                                {
                                    sapUserInfo.Save(MasterConfigHelper.ServiceUser);
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in HANA.", sapUserInfo.AccountName, oldTerminationDate, user.TerminationDate), null);
                                }
                            }
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Skipping the account :{accountRecon.AccountName}");
                        }
                    }
                    catch (Exception)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, $"Failed while extending the account {accountRecon.AccountName}, for user {user.UserName}");
                        isUpdated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the extension request. {0}", ex.Message), ex);
                return false;
            }

            return isUpdated;
        }
    }
}

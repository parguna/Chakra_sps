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
    class FLINSBase : FormExecutorBase
    {
        List<ApplicationType> positionApplicationTypes = new List<ApplicationType> { ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_ISU, ApplicationType.EBS_CRM, ApplicationType.EBS, ApplicationType.MSSL };
        public FLINSBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }
        public void UpdateProvisioningField(ProvisioningParamCollection fields, SAPUserinfo sapUserValue, DomainModel.User user)
        {
            try
            {
                foreach (ProvisioningParam gData in fields)
                {
                    if (gData.Name.ToUpper() == "Function0".ToUpper() || gData.Name.ToUpper() == "Pers_No".ToUpper())
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPUserinfo;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"UpdateProvisioningField:Setting default value failed for Function0 & Pers_No.", ex);
            }
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocSAPHRFLINSRequest sapDetail = null;
            string roles = string.Empty;

            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapDetail = (AdHocSAPHRFLINSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (sapDetail != null)
                {
                    roles = this.GetRolesFromAdhocRequest(user, fields["Roles"].Value, sapDetail);
                    endpointUserId = sapDetail.EndpointUserId;
                    WriteProvisioningLog(LogLevelType.Info, $"Roles derived from GetRolesFromAdhocRequest {string.Join(",", roles)} for endpoint userid {endpointUserId} ", null);
                }
            }
            if (!string.IsNullOrEmpty(roles) && !AreEntitlementsValid(roles.Split(',')))
            {
                return false;
            }
            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccount(fields, user, roles, sapDetail);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, endpointUserId, roles);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, endpointUserId, roles, sapDetail);
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
                var sapUsrs = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (SAPUserinfo sapUsr in sapUsrs)
                {
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.Resignation);

                    bool success = this.LockUser(sapUsr, LockScenario.OffBoard);
                    if (success)
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAP.", sapUsr.UserLogonName), null);
                    else
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAP.", sapUsr.UserLogonName), null);


                    //Delimit Email and FLINS Logon
                    var endpoint = EndPoint.GetTrustedSourceEndPoint();
                    var sapEndpoint = EndPoint.GetEndPointByApplicationTypeNonTrusted(ApplicationType.HRMS_FLINS);
                    var hrConnector = ConnectorFactory.GetConnector(endpoint, null);


                    if (sapEndpoint != null && this.CurrentRequest.EndpointSystemId == sapEndpoint.Id)
                    {
                        if (this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding)
                        {
                            DateTime delimitdate = DateTime.Today;
                            bool isDelimited = hrConnector.DelimitFLINSLogonIdInHR(user.EmployeeId, delimitdate);

                            if (isDelimited)
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format("Delimit FLINSLogonUser {0} update in SAP HRMS. Pernr {1}", sapUsr.UserLogonName, user.EmployeeId), null);
                            }
                            isDelimited = false;// hrConnector.DelimitEmailIdInHR(user.EmployeeId, delimitdate);

                            if (isDelimited)
                            {
                                WriteProvisioningLog(LogLevelType.Info, string.Format("Delimit EmailId {0} update in SAP HRMS. Pernr {1}", sapUsr.UserLogonName, user.EmployeeId), null);
                            }
                        }
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
        protected override bool AreEntitlementsValid(string[] entitlements)
        {
            var roles = SAPRoleInfo.GetByRoleCodesAndEndpointId(this.Endpoint.Id, entitlements);

            List<string> rolesString = new List<string>();
            if (roles != null && roles.Count > 0)
            {
                rolesString = roles.Select(r => r.ROLE_CODE).Distinct().ToList();
            }

            IEnumerable<string> difference = entitlements.Where(s => !string.IsNullOrWhiteSpace(s) && !rolesString.Contains(s,StringComparer.InvariantCultureIgnoreCase)).Select(s => s);
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

                var sapUsr = SAPUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);
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
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
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
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("User is NULL in ReOrg- FLINSBase."), null);
                    return true;
                }

                bool isUpdated = true;
                var sapUsr = SAPUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

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
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
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

                var sapUsr = SAPUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
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
                    var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

                    if (existingRoles.Count > 0)
                    {
                        sapUsr.Roles = existingRoles;

                        bool isSucces = EndpointConnector.RemoveAllEntitlements(sapUsr);

                        if (isSucces)
                        {
                            SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);
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
                        SAPUserinfo.AddEntitlement(sapUsr.Id, EntitlementType.Role, roles);
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

        protected void AddSAPUser(SAPUserinfo sapUser)
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

        protected bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today); //lockScenario != LockScenario.AdhocRequest ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today); //delimit the end date to one day less thean the action date
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

                        SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in SAP.", sapUsr.UserLogonName), null);
                    }
                }
            }

            return isusrDisabled;
        }
        /// <summary>
        /// Addtional parameter allowed added because update dependentsystem not required for other system(MSSL ,EBS)
        /// </summary>
        /// <param name="sapUsr"></param>
        /// <param name="updateDependentSystem"></param>
        /// <returns></returns>
        protected bool UnlockUser(SAPUserinfo sapUsr, bool updateDependentSystem = true)
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
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Unlocked in SAP.", sapUsr.UserLogonName), null);
                    if (updateDependentSystem)
                    {
                        this.UnLockDependentSystems(sapUsr);
                    }
                }
            }

            return isusrDisabled;
        }

        //protected DateTime? GetValidTo(Nullable<long> userId)
        // {
        //     var validTo = DateTime.MaxValue;
        //     if ((CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

        //     if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
        //         return CurrentRequest.DeactivationDate.Value;
        //     else
        //     {
        //         if (userId.HasValue && userId.Value > 0)
        //         {
        //             var user = DomainModel.User.GetById(userId.Value);
        //             if (user != null && user.TerminationDate.HasValue) validTo = user.TerminationDate.Value;
        //         }
        //     }
        //     return validTo;
        // }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string roles, AdHocSAPHRFLINSRequest sapDetail)
        {

            bool isExecutedSuccessfully = false;

            try
            {
                var sapUserValue = SAPUserinfo.GetLockedAccountByName(sapDetail.AdHocSAPHRFLINSSharedAccount.AccountName, user.Id, Endpoint.Id, CurrentRequest.AccountType);

                if (sapUserValue != null)
                {
                    UpdateProvisioningField(fields, sapUserValue, user);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Account:{sapUserValue.AccountName} Function0:{sapUserValue.Function0} Pers_No:{sapUserValue.Pers_No} AccountType:{CurrentRequest.AccountType}");

                    var entitlementDetail = ((ISAPUserType)sapDetail.SharedAccountDetail);
                    if (entitlementDetail != null)
                    {
                        sapUserValue.UserType = entitlementDetail.SAPUserType;
                        sapUserValue.Firstname = entitlementDetail.Description;
                        sapUserValue.SapUserType = GetSAPUserType(entitlementDetail.SAPUserType);
                    }

                    isExecutedSuccessfully = UnlockUser(sapUserValue);

                    if (isExecutedSuccessfully)
                    {
                        this.ReOrg(user, fields);

                        //SAP Roles
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

                        sapUserValue.Roles = sapRoles.Distinct().ToList();

                        if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                        }

                        sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", roles));
                        sapUserValue.Save(MasterConfigHelper.ServiceUser);

                        sapUserValue.Password = ConstructPassword();

                        if (this.CurrentRequest.AccountType != AccountType.Service)
                        {
                            var passwordResetStatus = EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                            if (passwordResetStatus == false)
                            {
                                throw new Exception($"Not able to reset password for the account:[{sapUserValue.UserLogonName}]");
                            }
                        }
                        else
                        {
                            var passwordResetStatus = EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
                            if (passwordResetStatus == false)
                            {
                                throw new Exception($"Not able to change password for the account:[{sapUserValue.UserLogonName}]");
                            }
                        }


                        //EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }
                    }
                }
                else
                {
                    sapUserValue = new SAPUserinfo();
                    string accountName = string.Empty;

                    #region Set SAP User object Values

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = ConstructUserName(sapDetail.AdHocSAPHRFLINSSharedAccount.AccountName);
                    sapUserValue.Firstname = sapDetail.AdHocSAPHRFLINSSharedAccount.Description;
                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    sapUserValue.UserType = sapDetail.AdHocSAPHRFLINSSharedAccount.SAPUserType;
                    sapUserValue.SapUserType = GetSAPUserType(sapDetail.AdHocSAPHRFLINSSharedAccount.SAPUserType);

                    if (string.IsNullOrEmpty(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = ConstructPassword();

                    //SAP Roles
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
                    sapUserValue.Roles = sapRoles.Distinct().ToList();

                    //SAP Params
                    Dictionary<string, string> paramsInfoDictionary = new Dictionary<string, string>();
                    if (fields["Parameters"].Value != null)
                    {
                        string[] sapParamList = Regex.Split(fields["Parameters"].Value, "##PP##");
                        string[] tmpParamInfo;
                        if (sapParamList != null)
                        {
                            foreach (string param in sapParamList)
                            {
                                tmpParamInfo = param.Split(',');
                                if (tmpParamInfo.Length == 2)
                                {
                                    paramsInfoDictionary.Add(tmpParamInfo[0], tmpParamInfo[1]);
                                }
                            }

                            //paramsInfoDictionary.Add("CYMP_PERSON", "");
                        }
                    }
                    sapUserValue.Parameters = paramsInfoDictionary;

                    //Valid From and Valid To

                    if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                    }

                    sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

                    UpdateProvisioningField(fields, sapUserValue, user);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Account:{sapUserValue.AccountName} Function0:{sapUserValue.Function0} Pers_No:{sapUserValue.Pers_No} AccountType:{CurrentRequest.AccountType}");

                    #endregion

                    isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }

                        AddSAPUser(sapUserValue);

                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);

                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));

                        CreateDependentSystemEntriesInIDMS(sapUserValue, user);

                        if (CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreated, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, Endpoint.DisplayName), null);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating FLINS account for the user {0}", user.DisplayName), ex);
            }

            return isExecutedSuccessfully;
        }

        protected virtual bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);

                if (sapUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Exception, $"Could not find SAP User object for the EndpointUserID : {endpointUserId}", null);
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
                    DeleteFLINSDependentAccount(user, sapUser.UserLogonName);

                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Account is locked for User:{sapUser.UserLogonName}");
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, sapUser.UserLogonName);
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the SAP account. {0}", ex.Message), ex);
                return false;
            }
        }


        protected virtual bool AmendentAdd(DomainModel.User user, long endpointUserId, string roles)
        {
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);

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
                    SAPUserinfo.AddEntitlement(endpointUserId, EntitlementType.Role, grpListStr);

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

        protected bool AmendentRemove(DomainModel.User user, long endpointUserId, string roles, AdHocSAPHRFLINSRequest flinsRequest)
        {
            try
            {

                var sapUser = SAPUserinfo.GetById(endpointUserId);
                if (sapUser == null)
                    return true;

                //SAP Roles
                var rolesToRemove = UtilityMethods.GetListFromString(roles);

                //retain supplementary and general which are used in other existing business roles
                sapUser.Roles = RetainSupplementaryAndGeneralRoles(rolesToRemove, flinsRequest, endpointUserId);
                if (sapUser.Roles.Count == 0)
                    return true;

                return AmendentRemoveAction(user, sapUser);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in AmendentRemove. {0}", ex.Message), ex);
                return false;
            }
        }

        protected virtual bool AmendentRemoveAction(DomainModel.User user, SAPUserinfo sapUser)
        {
            try
            {


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

                bool isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(sapUser, EntitlementType.Role);

                string grpListStr = string.Join(",", sapUser.Roles);

                if (isExecutedSuccessfully)
                {
                    if (sapUser.Roles.Count > 0)
                    {
                        SAPUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);

                        ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, sapUser.UserLogonName);

                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"No roles to remove for user {sapUser.UserLogonName}");
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in AmendentRemoveAction. {0}", ex.Message), ex);
                return false;
            }
        }

        void UnLockDependentSystems(SAPUserinfo sapUser)
        {

            var sapepEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
            var sapceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);

            AccountReconcilation epRecon = AccountReconcilation.GetByAccountName(sapUser.UserLogonName, sapepEndpoint.Id);
            AccountReconcilation ceRecon = AccountReconcilation.GetByAccountName(sapUser.UserLogonName, sapceEndpoint.Id);
            WriteProvisioningLog(LogLevelType.DebugStep, $"SAP EP Account Exists:{epRecon != null}");
            WriteProvisioningLog(LogLevelType.DebugStep, $"SAP CE Account Exists:{ceRecon != null}");
            if (epRecon != null && sapepEndpoint.IsActive)
            {
                epRecon.IsLocked = false;
                epRecon.IsDeleted = false;
                epRecon.LockScenario = LockScenario.None;
                epRecon.LastModifiedBy = "System";
                epRecon.LastModifiedDate = DateTime.Now;
                epRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                epRecon.ValidFromDate = CurrentRequest.ActivationDate;
                epRecon.DeletedDate = null;
                epRecon.UserId = CurrentRequest.UserId;
                epRecon.Save(MasterConfigHelper.ServiceUser);
            }

            if (ceRecon != null && sapceEndpoint.IsActive)
            {
                ceRecon.IsLocked = false;
                ceRecon.IsDeleted = false;
                ceRecon.LockScenario = LockScenario.None;
                ceRecon.LastModifiedBy = "System";
                ceRecon.LastModifiedDate = DateTime.Now;
                ceRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                ceRecon.ValidFromDate = CurrentRequest.ActivationDate;
                ceRecon.DeletedDate = null;
                ceRecon.UserId = CurrentRequest.UserId;
                ceRecon.Save(MasterConfigHelper.ServiceUser);
            }
        }

        protected string GetRolesFromAdhocRequest(DomainModel.User usr, string existingRoles, AdHocSAPHRFLINSRequest sapDetail)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();
            var isAmendementRemove = CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
            foreach (AdHocSAPHRFLINSRequestDetail detail in sapDetail.AdHocSAPHRFLINSRequestDetails)
            {
                TechnicalRoles.AddRange(AdHocAccessRequest.GetHrFlinsTechnicalRolesByAdhocRequestObject(detail, isAmendementRemove));
            }

            TechnicalRoles = TechnicalRoles.Distinct().ToList(); foreach (string technicalRole in TechnicalRoles)
            {
                if (string.IsNullOrEmpty(ItRoles))
                    ItRoles = technicalRole;
                else
                    ItRoles = ItRoles + "," + technicalRole;
            }

            if (string.IsNullOrEmpty(existingRoles))
            {
                return ItRoles;
            }
            else
            {
                return existingRoles + "," + ItRoles;
            }
        }

        protected string ConstructUserName(string userName)
        {
            userName = CheckUserNameExists(userName.ToString());
            return userName;
        }

        List<string> RetainSupplementaryAndGeneralRoles(List<string> roles, AdHocSAPHRFLINSRequest flinsRequest, long endpointUserId)
        {
            List<string> rolesToRemove = new List<string>();
            List<string> rolesToRetain = new List<string>();

            var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(endpointUserId)?.SAPRoleInfo?.Select(item => item.ROLE_CODE)?.ToList();

            //find all business roles
            List<string> businessRolesToRetain = new List<string>();
            if (existingRoles == null || existingRoles.Count == 0)
                return businessRolesToRetain;

            foreach (string role in existingRoles)
            {
                var matrix = SAPHRFLINSRoleMatrix.GetByRoleCode(role);
                if (matrix != null)
                {
                    bool isMainRole = matrix.MainSupp.Equals("main", StringComparison.CurrentCultureIgnoreCase);

                    if (isMainRole)
                    {
                        if (!flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(item => item.Entitlement.Equals(matrix.BusinessRole, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            businessRolesToRetain.Add(matrix.Company + "," + matrix.BusinessRole);
                        }
                    }
                }
            }
            if (flinsRequest != null && flinsRequest.AdHocSAPHRFLINSRequestDetails.Count > 0)
            {
                //Scenario where only one purchase group for multiple business role and user selected only one business role. Then system should retain the purchase group;
                var onlyPurchaseGroup = flinsRequest.AdHocSAPHRFLINSRequestDetails.Where(i => !string.IsNullOrWhiteSpace(i.PurchaseGroup)).ToList();
                if (onlyPurchaseGroup != null && onlyPurchaseGroup.Count > 0)
                {
                    var bRoles = onlyPurchaseGroup.Select(i => $"{i.Entitlement}").ToList();

                    var existingBusinessRole = DomainModel.SAPHRFLINSRoleMatrix.GetAllExistingBusinessRoles(flinsRequest.EndpointUserId, "PurchaseGroup");
                    var takeExistingPurchasingGroup = SAPHRFLINSRoleMatrix.GetOnlyPurchaseGroup(null, existingBusinessRole.FirstOrDefault().BusinessRole, null, null, null, null, null, null, flinsRequest.EndpointUserId);
                    if (takeExistingPurchasingGroup.Count == 1)
                    {
                        var roleWithCompany = existingBusinessRole.Where(i => !flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(b => i.BusinessRole == b.Entitlement)).Select(i => $"{i.Company}-{i.BusinessRole}").ToList();
                        //existingBusinessRole.Select(i => $"{i.Company}-{i.BusinessRole}").ToList();
                        WriteProvisioningLog(LogLevelType.DebugStep, $"Existing Role:{string.Join(",", roleWithCompany)}");
                        var addedRolesWithCompany = onlyPurchaseGroup.Select(i => $"{i.Company}-{i.Entitlement}").ToList();
                        WriteProvisioningLog(LogLevelType.DebugStep, $"Request to remove Role:{string.Join(",", addedRolesWithCompany)}");
                        if (roleWithCompany.Exists(i => !addedRolesWithCompany.Contains(i, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            foreach (var existingPurchasingGrp in takeExistingPurchasingGroup)
                            {
                                if (roles.Exists(i => i.Equals(existingPurchasingGrp.Value, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"PurchaseGroup to retain:{existingPurchasingGrp.Value}");
                                    roles.Remove(existingPurchasingGrp.Value);
                                }
                            }
                        }
                    }
                    existingBusinessRole = existingBusinessRole.Where(i => !bRoles.Contains(i.BusinessRole, StringComparer.InvariantCultureIgnoreCase) && !flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(b => i.BusinessRole == b.Entitlement)).ToList();
                    if (existingBusinessRole.Count > 0)
                    {
                        if (takeExistingPurchasingGroup.Count == 1)
                        {

                            WriteProvisioningLog(LogLevelType.Info, $"Purchase Group:{string.Join(",", takeExistingPurchasingGroup.Select(i => i.Value))} cannot be removed because its required for business role:{string.Join(",", existingBusinessRole.Select(i => i.BusinessRole))}");
                            foreach (var existingPurchasingGrp in takeExistingPurchasingGroup)
                            {
                                if (roles.Exists(i => i.Equals(existingPurchasingGrp.Value, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"PurchaseGroup to retain:{existingPurchasingGrp.Value}");
                                    roles.Remove(existingPurchasingGrp.Value);
                                }
                            }
                        }
                    }
                }


                //Scenario where only one Cost Center for multiple business role and user selected only one business role. Then system should retain the purchase group;

                var onlyCostCenter = flinsRequest.AdHocSAPHRFLINSRequestDetails.Where(i => !string.IsNullOrWhiteSpace(i.CostCenter)).ToList();
                if (onlyCostCenter != null && onlyCostCenter.Count > 0)
                {
                    var bRoles = onlyCostCenter.Select(i => $"{i.Entitlement}").ToList();

                    var existingBusinessRole = DomainModel.SAPHRFLINSRoleMatrix.GetAllExistingBusinessRoles(flinsRequest.EndpointUserId, "CostCenter");
                    var takeExistingCostCenter = SAPHRFLINSRoleMatrix.GetOnlyCostCenter(null, existingBusinessRole.FirstOrDefault().BusinessRole, null, null, null, null, null, null, flinsRequest.EndpointUserId);
                    if (takeExistingCostCenter.Count == 1)
                    {
                        var roleWithCompany = existingBusinessRole.Where(i=> !flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(b => i.BusinessRole == b.Entitlement)).Select(i => $"{i.Company}-{i.BusinessRole}").ToList();
                        WriteProvisioningLog(LogLevelType.Info, $"Existing Role:{string.Join(",", roleWithCompany)}");
                        var addedRolesWithCompany = onlyCostCenter.Select(i => $"{i.Company}-{i.Entitlement}").ToList();
                        WriteProvisioningLog(LogLevelType.Info, $"Request to remove Role:{string.Join(",", addedRolesWithCompany)}");
                        if (roleWithCompany.Exists(i => !addedRolesWithCompany.Contains(i, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            foreach (var existingCostCenter in takeExistingCostCenter)
                            {
                                if (roles.Exists(i => i.Equals(existingCostCenter.Value, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"CostCenter to retain:{existingCostCenter.Value}");
                                    roles.Remove(existingCostCenter.Value);
                                }
                            }
                        }
                    }
                    existingBusinessRole = existingBusinessRole.Where(i => !bRoles.Contains(i.BusinessRole, StringComparer.InvariantCultureIgnoreCase) 
                    && !flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(b=> i.BusinessRole== b.Entitlement)).ToList();
                    if (existingBusinessRole.Count > 0)
                    {
                        if (takeExistingCostCenter.Count == 1)
                        {
                           // flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(i=> !existingBusinessRole.Exists(j=> j. i.Entitlement))


                            WriteProvisioningLog(LogLevelType.Info, $"Cost Center :{string.Join(",", takeExistingCostCenter.Select(i => i.Value))} cannot be removed because its required for business role:{string.Join(",", existingBusinessRole.Select(i => i.BusinessRole))}");
                            foreach (var existingCostCenter in takeExistingCostCenter)
                            {
                                if (roles.Exists(i => i.Equals(existingCostCenter.Value, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"CostCenter to retain:{existingCostCenter.Value}");
                                    roles.Remove(existingCostCenter.Value);
                                }
                            }
                        }
                    }
                }

            }
            foreach (string bussinessRole in businessRolesToRetain)
            {
                var bussinessRoleInfo = bussinessRole.Split(',');
                var matrices = SAPHRFLINSRoleMatrix.GetByBussinessRoleAndCompany(bussinessRoleInfo[1], bussinessRoleInfo[0]);
                if (matrices != null)
                {
                    foreach (SAPHRFLINSRoleMatrix matrix in matrices)
                    {
                        rolesToRetain.Add(matrix.TechnicalRule);
                    }
                }
            }

            foreach (AdHocSAPHRFLINSRequestDetail sapDetail in flinsRequest.AdHocSAPHRFLINSRequestDetails)
            {
                //if company roles need to be retained then exclude from the remove list
                if (sapDetail.RetainCompanyRoles)
                {
                    var matrices = SAPHRFLINSRoleMatrix.GetByBussinessRoleAndCompany(sapDetail.Entitlement, sapDetail.Company);
                    if (matrices != null)
                    {
                        foreach (SAPHRFLINSRoleMatrix matrix in matrices)
                        {
                            rolesToRetain.Add(matrix.TechnicalRule);
                        }
                    }
                }
            }

            foreach (string role in roles)
            {
                if (!rolesToRetain.Contains(role))
                {
                    rolesToRemove.Add(role);
                }
            }

            return rolesToRemove;
        }

        bool ExtendPositionAsWell()
        {
            return positionApplicationTypes.Contains(Endpoint.ApplicationType);
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
                            var sapUserInfo = SAPUserinfo.GetById(accountRecon.EndpointUserId);

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
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", sapUserInfo.AccountName, oldTerminationDate, user.TerminationDate), null);
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

        private string GetUserADPrimaryAccountName(DomainModel.User user)
        {
            string accName = null;
            var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
            if (adEndpoint != null && user != null)
            {
                var adUserValue = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, adEndpoint.Id);
                accName = adUserValue?.sAMAccountName;
            }
            return accName;
        }

    }
}

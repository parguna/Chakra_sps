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
    class SAPT1SpecialBase : FormExecutorBase
    {
        readonly string MasterConfigHelperServiceUser;
        public SAPT1SpecialBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            IEntitlementDetail sapDetail = null;
            string roles = string.Empty;

            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                sapDetail = CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (sapDetail != null)
                {
                    var defaultRoles = string.Empty;

                    if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        defaultRoles = fields["Roles"].Value;
                    }

                    roles = this.GetRolesFromAdhocRequest(user, defaultRoles, sapDetail);
                    endpointUserId = sapDetail.EndpointUserId;
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
            bool success = false;
            try
            {
                var sapUsrs = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                if(sapUsrs.Count > 0)
                {
                    foreach (SAPUserinfo sapUsr in sapUsrs)
                    {
                        this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.Resignation);

                        success = this.LockUser(sapUsr, LockScenario.OffBoard, user);
                        if (success)
                            WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAP.", sapUsr.UserLogonName), null);
                        else
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAP.", sapUsr.UserLogonName), null);
                    }
                }
                else
                {
                    success = true;
                }
                
                return success;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
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
                    sapUsr.Department = user.OrgDepartment;
                    sapUsr.Function0 = user.IdentificationNo;
                    sapUsr.CostCenter = user.CostCenter;

                    if (CurrentRequest.ApplicationType == ApplicationType.SAP_GRC)
                        sapUsr.CostCenter = string.IsNullOrEmpty(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;

                    isUpdated = EndpointConnector.UpdateAccount(sapUsr);

                    if (isUpdated)
                    {
                        sapUsr.Save(MasterConfigHelperServiceUser);
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

                bool isUpdated = true;
                var sapUsr = SAPUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

                if (CurrentRequest.ApplicationType == ApplicationType.SAP_GRC)
                    sapUsr.CostCenter = string.IsNullOrEmpty(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;
                sapUsr.Function0 = user.IdentificationNo;
                sapUsr.Department = user.OrgDepartment;

                EndpointConnector.ChangeContainer(sapUsr);

                isUpdated = EndpointConnector.ChangeContainer(sapUsr);

                if (isUpdated)
                {
                    sapUsr.Save(MasterConfigHelperServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);
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

                var sapUsr = SAPUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);

                this.UnlockUser(sapUsr);
                this.ReOrg(user, fields);

                string roles = this.CurrentRequest.EventUndoRequest.TechnicalRoles;

                if (!string.IsNullOrEmpty(roles))
                {
                    sapUsr.Roles = UtilityMethods.GetListFromString(roles);

                    sapUsr.Roles = sapUsr.Roles.Distinct().ToList();

                    sapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);

                    sapUsr.UserValidTo = sapUserDefaultValidTo;

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
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            try
            {
                var sapUsrs = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (SAPUserinfo sapUsr in sapUsrs)
                {
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange);

                    bool success = this.LockUser(sapUsr, LockScenario.OUChange);
                    if (success)
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified in SAP.", sapUsr.UserLogonName), null);
                    else
                        WriteProvisioningLog(LogLevelType.Error, string.Format("Error while locking User {0} in SAP.", sapUsr.UserLogonName), null);
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing transfer request. {0}", ex.Message), ex);
                return false;
            }
        }

        protected void AddSAPUser(SAPUserinfo sapUser)
        {
            string currentUser = MasterConfigHelperServiceUser;
            sapUser.Save(currentUser);

            CreateUserInReconTable(sapUser, currentUser);
        }

        protected bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today); //(lockScenario != LockScenario.AdhocRequest ) ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today); //delimit the end date to one day less thean the action date
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

                    sapUsr.Save(MasterConfigHelperServiceUser);
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

        protected bool LockUser(SAPUserinfo sapUsr, LockScenario lockScenario, DomainModel.User user)
        {
            bool isusrDisabled = true;

            if (sapUsr != null)
            {
                isusrDisabled = false;

                sapUsr.UserValidTo = this.getSAPDateFormat(DateTime.Today); //(lockScenario != LockScenario.AdhocRequest && user.FullTimeEmployee) ? this.getSAPDateFormat(DateTime.Today.AddDays(-1)) : this.getSAPDateFormat(DateTime.Today); //delimit the end date to one day less thean the action date
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

                    sapUsr.Save(MasterConfigHelperServiceUser);
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

        protected bool UnlockUser(SAPUserinfo sapUserValue)
        {
            bool isusrDisabled = true;

            if (sapUserValue != null)
            {
                isusrDisabled = false;

                //Valid From and Valid To

                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUserValue.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                
                sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(this.CurrentRequest.UserId));
                

                if (!string.IsNullOrEmpty(sapUserValue.UserValidFrom) && !string.IsNullOrEmpty(sapUserValue.UserValidTo))
                {
                    EndpointConnector.UpdateAccount(sapUserValue);
                }

                isusrDisabled = EndpointConnector.UnLockAccount(sapUserValue);
                if (isusrDisabled)
                {
                    sapUserValue.IsUserLocked = false;
                    sapUserValue.LastModifiedBy = "System";
                    sapUserValue.LastModifiedDate = DateTime.Now;

                    sapUserValue.Save(MasterConfigHelperServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Unlocked in SAP.", sapUserValue.UserLogonName), null);
                }
            }

            return isusrDisabled;
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
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string roles, IEntitlementDetail sapDetail)
        {

            bool isExecutedSuccessfully = false;

            try
            {
                var sapUserValue = SAPUserinfo.GetLockedAccountByName(sapDetail.SharedAccountDetail.AccountName, user.Id, Endpoint.Id, CurrentRequest.AccountType);
                var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(Endpoint.Id) && CurrentRequest.AccountType == AccountType.User;

                if (sapUserValue != null)
                {
                    var entitlementDetail = ((ISAPUserType)sapDetail.SharedAccountDetail);
                    if (entitlementDetail != null)
                    {
                        sapUserValue.UserType = entitlementDetail.SAPUserType;
                        sapUserValue.SapUserType = GetSAPUserType(entitlementDetail.SAPUserType);
                        if (!string.IsNullOrWhiteSpace(entitlementDetail.Description))
                            sapUserValue.Firstname = entitlementDetail.Description;
                    }
                    
                    UpdateProvisioningField(fields, sapUserValue, user);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Account:{sapUserValue.AccountName} Function0:{sapUserValue.Function0} Pers_No:{sapUserValue.Pers_No} AccountType:{CurrentRequest.AccountType}");

                    #region AD AccountName
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(user);
                    }
                    #endregion

                    isExecutedSuccessfully = UnlockUser(sapUserValue);

                    if (isExecutedSuccessfully)
                    {
                        EndpointConnector.UpdateSSOAttributes(sapUserValue, sapUserValue.UserSamAccountName, CurrentRequest.AccountType);
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

                        if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                        {
                            sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                        }
                        else
                        {
                            sapUserValue.UserValidTo = sapUserDefaultValidTo;
                        }

                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", roles));
                        sapUserValue.Save(MasterConfigHelperServiceUser);

                        sapUserValue.Password = ConstructPassword();

                        LogDebugStep($"{Endpoint?.DisplayName} - account reactivation password: {sapUserValue.Password}");
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
                            var passwordResetStatus = EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password,user.Id, true);
                            if (passwordResetStatus == false)
                            {
                                throw new Exception($"Not able to change password for the account:[{sapUserValue.UserLogonName}]");
                            }
                        }

                        

                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType, isForSAPSSO: isSSoEnabled);

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

                    WriteProvisioningLog(LogLevelType.DebugStep, $"Accountname: {sapDetail.SharedAccountDetail?.AccountName}, Description: {((ISAPUserType)sapDetail.SharedAccountDetail).Description}, Type:  {((ISAPUserType)sapDetail.SharedAccountDetail).SAPUserType}");

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = ConstructUserName(sapDetail.SharedAccountDetail.AccountName);
                    sapUserValue.Firstname = ((ISAPUserType)sapDetail.SharedAccountDetail).Description;
                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    sapUserValue.UserType = ((ISAPUserType)sapDetail.SharedAccountDetail).SAPUserType;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"sapUserValue.UserLogonName - {sapUserValue.UserLogonName}");
                    if (string.IsNullOrEmpty(sapUserValue.UserLogonName))
                    {
                        WriteProvisioningLog(LogLevelType.Error, "Account name cannot be empty");
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

                    if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                    {
                        sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                    }
                    else
                    {
                        sapUserValue.UserValidTo = sapUserDefaultValidTo;
                    }
                    UpdateProvisioningField(fields, sapUserValue, user);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Account:{sapUserValue.AccountName} Function0:{sapUserValue.Function0} Pers_No:{sapUserValue.Pers_No} AccountType:{CurrentRequest.AccountType}");
                    #endregion
                    #region AD AccountName
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(user);
                    }
                    #endregion

                    WriteProvisioningLog(LogLevelType.DebugStep, $"EndpointConnector.CreateAccount - Started");
                    isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"EndpointConnector.CreateAccount - Completed, isExecutedSuccessfully:" + isExecutedSuccessfully);

                   

                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType, isForSAPSSO: isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }

                        AddSAPUser(sapUserValue);

                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);

                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));

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

                bool isSuccess = LockUser(sapUser, LockScenario.AdhocRequest);
                 
                if (isSuccess)
                {
                    var ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                    if (this.CurrentRequest.EndpointSystemId.HasValue && ep != null && this.CurrentRequest.EndpointSystemId ==  ep.Id )
                    {
                        DeleteFLINSDependentAccount(user, sapUser.UserLogonName);
                    }
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
                string[] rolesAry = !string.IsNullOrEmpty(roles) ? roles.Split(',') : null;
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

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    sapUser.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    sapUser.UserValidTo = sapUserDefaultValidTo;
                }

                bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(sapUser, sapUser.UserValidFrom, sapUser.UserValidTo, EntitlementType.Role);
                string grpListStr = string.Join(",", sapUser.Roles);

                if (isExecutedSuccessfully)
                {
                    SAPUserinfo.AddEntitlement(endpointUserId, EntitlementType.Role, grpListStr);

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserAddedInToGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserAddedInToGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding roles to the sap account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected bool AmendentRemove(DomainModel.User user, long endpointUserId, string roles, IEntitlementDetail flinsRequest)
        {
            try
            {
                return AmendentRemoveAction(user, endpointUserId, roles);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in SAPT1SpecialBase.AmendentRemove. {0}", ex.Message), ex);
                return false;
            }
        }

        protected virtual bool AmendentRemoveAction(DomainModel.User user, long endpointUserId, string roles)
        {
            try
            {

                var sapUser = SAPUserinfo.GetById(endpointUserId);

                //SAP Roles
                var rolesToRemove = UtilityMethods.GetListFromString(roles);

                //retain supplementary and general which are used in other existing business roles
                sapUser.Roles = rolesToRemove;

                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    sapUser.UserValidFrom = this.getSAPDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    sapUser.UserValidFrom = this.getSAPDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    sapUser.UserValidTo = this.getSAPDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    sapUser.UserValidTo = sapUserDefaultValidTo;
                }

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
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in SAPT1SpecialBase.AmendentRemoveAction. {0}", ex.Message), ex);
                return false;
            }
        }

        protected string GetRolesFromAdhocRequest(DomainModel.User usr, string existingRoles, IEntitlementDetail sapDetail)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();

            var bussinessRoles = UtilityMethods.GetListFromString(sapDetail.Entitlement);

            foreach (string bussinessRole in bussinessRoles)
            {
                List<string> technicalRoles = null;


                if (CurrentRequest.ApplicationType == ApplicationType.FLINS_BI)
                    technicalRoles = SAPBIRoleCategory.BindTechnicalFromFLINSBI(bussinessRole);

                else if (CurrentRequest.ApplicationType == ApplicationType.SAP_GRC)
                    technicalRoles = SAPGRCRole.BindTechnicalFromSAPGRCRole(bussinessRole);

                else if (CurrentRequest.ApplicationType == ApplicationType.SAP_PI)
                    technicalRoles = SAPPIRole.BindTechnicalFromSAPPIRole(bussinessRole);
                if (technicalRoles != null)
                    TechnicalRoles.AddRange(technicalRoles);
            }


            TechnicalRoles = TechnicalRoles.Distinct().ToList();

            foreach (string technicalRole in TechnicalRoles)
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

        string ConstructUserName(string userName)
        {
            userName = CheckUserNameExists(userName.ToString());
            return userName;
        }

        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            LogDebugStep($"Extension Endpoint: {Endpoint.DisplayName}");

            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the request, Account Type:{CurrentRequest.AccountType}");
                return true;
            }

            if (!CurrentRequest.EmployeeEventTriggerId.HasValue)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Failing the extension request, EmployeeEventTriggerId:{CurrentRequest.EmployeeEventTriggerId}");
                return true;
            }

            bool isUpdated = true;
            try
            {
                var oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;

                if (!oldTerminationDate.HasValue) return false;

                var accountRecons = AccountReconcilation.GetActiveAccountRecons(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                WriteProvisioningLog(LogLevelType.DebugStep, $"Accounts found for extension count :{accountRecons?.Count}");

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
                            WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the account :{accountRecon.AccountName}");
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

        protected override bool AreEntitlementsValid(string[] entitlements)
        {
            var roles = SAPRoleInfo.GetByRoleCodesAndEndpointId(this.Endpoint.Id, entitlements);

            List<string> rolesString = new List<string>();
            if (roles != null && roles.Count > 0)
            {
                rolesString = roles.Select(r => r.ROLE_CODE).Distinct().ToList();
            }

            IEnumerable<string> difference = entitlements.Where(s => !string.IsNullOrWhiteSpace(s) && !rolesString.Contains(s, StringComparer.InvariantCultureIgnoreCase)).Select(s => s);
            if (difference.Any())
            {
                WriteProvisioningLog(LogLevelType.Error, $"The entitlements -[ {string.Join(",", difference)} ] are not exists in the discovery data of the target system - {this.Endpoint.DisplayName}");
                return false;
            }
            else
            {
                return true;
            }
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

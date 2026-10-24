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

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    class FLINS : Common.FLINSBase
    {

        public FLINS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
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
                var defaultRoles = string.Empty;

                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    defaultRoles = fields["Roles"].Value.ToString();
                }

                if (sapDetail != null)
                {
                    roles = this.GetRolesFromAdhocRequest(user, defaultRoles, sapDetail, CurrentRequest.AdHocAccessRequest.AdhocActionType);
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
                    isExecutedSuccessfully = CreateAccount(fields, user, roles);
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
            else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, roles);
            }

            return isExecutedSuccessfully;
        }

        protected string GetRolesFromAdhocRequest(DomainModel.User usr, string existingRoles, AdHocSAPHRFLINSRequest sapDetail, AdHocActionType AdhocActionType)
        {
            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();
            var isAmendementRemove = CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
            foreach (AdHocSAPHRFLINSRequestDetail detail in sapDetail.AdHocSAPHRFLINSRequestDetails)
            {
                TechnicalRoles.AddRange(AdHocAccessRequest.GetHrFlinsTechnicalRolesByAdhocRequestObject(detail, isAmendementRemove));
            }


            // should not remove default technical roles 
            if (AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var DefaultRoles_fields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);

                TechnicalRoles = TechnicalRoles.Distinct().ToList();

                TechnicalRoles.AddRange(existingRoles.Split(','));

                var defaultRoles = DefaultRoles_fields["Roles"].Value;

                if (!string.IsNullOrWhiteSpace(defaultRoles))
                {
                    var defaultList = defaultRoles.Split(',').ToList();

                    defaultList.ForEach(d =>
                    {
                        if (TechnicalRoles.Contains(d))
                            TechnicalRoles.Remove(d);
                    });
                }

                return string.Join(",", TechnicalRoles);
            }
            else

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
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Transfer started for FLINS, user:{user.UserName}");

                var sapUsr = SAPUserinfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);
                if (sapUsr == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Transfer request- FLINS User not found for User {user.KnownAs}, Request Id {this.CurrentRequest.Id}");
                }

                //Start Removing Existing AccessLevels

                WriteProvisioningLog(LogLevelType.Info, $"Flins Transfer request- Started Removing Exisitng Entitlements {user.KnownAs}");

                sapUsr.CostCenter = string.IsNullOrEmpty(sapUsr.CostCenter) ? user.CostCenter : sapUsr.CostCenter;
                sapUsr.Function0 = user.IdentificationNo;
                sapUsr.Department = user.OrgDepartment;

                //if (OUChangeSnapshot.GetByUserIdAccessRequestId(user.Id, this.CurrentRequest.Id) == null)
                //{
                    this.TakeOUSnapshot(user, sapUsr, EmployeeEvents.OUChange);
                //}

                var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id).SAPRoleInfo.Select(item => item.ROLE_CODE).ToList();

                //retain groups created using prov form

                var defaultRoles = fields["Roles"].Value;

                if (!string.IsNullOrEmpty(defaultRoles))
                {
                    foreach (string role in existingRoles)
                    {
                        if (!defaultRoles.Contains(role))
                        {
                            sapUsr.Roles.Add(role);
                        }
                    }
                }
                else
                {
                    sapUsr.Roles = existingRoles;
                }

                WriteProvisioningLog(LogLevelType.DebugStep, $"sapUsr.UserLogonName:{sapUsr.UserLogonName}");

                bool isSucces = EndpointConnector.RemoveAllEntitlements(sapUsr);
                string roleListStr = string.Join(",", sapUsr.Roles.Select(item => item));

                if (isSucces)
                {
                    SAPUserinfo.RemoveAllEntitlements(sapUsr.Id);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("Transfer Existing Roles - {0} removed successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                    
                    WriteProvisioningLog(LogLevelType.Info, $"Transfer -Updating userInfo {sapUsr.UserLogonName}, ValidFrom {sapUsr.validFromDate}, Validto {sapUsr.validToDate}.", null);

                    string message = ProvisioningValidator.IsFLINSDefaultsExists(user);
                    if (message.Length > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"FLINSProvisioningMatrix is NULL for  the Transfer request-{this.CurrentRequest.Id} user- {user.KnownAs}. {message}");
                        ProvisioningValidator.AlertITAdmin(user, message, EmployeeEvents.OUChange);

                        return false;
                    }

                    //user group
                    sapUsr.Groups = ConstructSAPUserGroup(user);

                    EndpointConnector.ChangeContainer(sapUsr);

                    WriteProvisioningLog(LogLevelType.Info, string.Format("Transfer - Started Default Roles Assignment for the user {0}.", sapUsr.UserLogonName), null);
                    sapUsr.Roles = new List<string>();

                    //add default roles for the new org.
                    if (user.FullTimeEmployee)
                    {
                        string orgRoles = GetSAPRolesFromMatrix(user);

                        //SAP Roles
                        List<string> sapRoles = new List<string>();
                        string[] rolesAry = !string.IsNullOrEmpty(orgRoles) ? orgRoles.Split(',') : null;
                        if (rolesAry != null)
                        {
                            foreach (string tRole in rolesAry)
                            {
                                if (!string.IsNullOrEmpty(tRole))
                                    sapRoles.Add(tRole.Trim());
                            }
                        }

                        sapUsr.Roles = sapRoles.Distinct().ToList();

                        WriteProvisioningLog(LogLevelType.Info, $"Transfer - SAP FLINS Default Roles -{orgRoles}.");
                        sapUsr.UserValidFrom = this.getSAPDateFormat(DateTime.Now);

                        sapUsr.UserValidTo = sapUserDefaultValidTo;

                        bool isAdded = EndpointConnector.AddEntitlement(sapUsr, sapUsr.UserValidFrom, sapUsr.UserValidTo, EntitlementType.Role);
                        if (isAdded)
                        {
                            SAPUserinfo.AddEntitlement(sapUsr.Id, EntitlementType.Role, string.Join(",", sapUsr.Roles));
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Transfer Default Roles - {0} added successfully for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format("Error while adding Transfer Default  roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                        }

                    }

                    sapUsr.Save(MasterConfigHelper.ServiceUser);

                    WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in SAP.", sapUsr.UserLogonName), null);

                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapUsr.UserLogonName), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the transfer request. {0}", ex.Message), ex);
                return false;
            }
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string roles)
        {

            bool isExecutedSuccessfully = false;

            try
            {
                List<SAPUserinfo> sapUsers = null;
                var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(Endpoint.Id) && CurrentRequest.AccountType == AccountType.User;

                if (CurrentRequest.AccountType == AccountType.User)
                {
                    sapUsers = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                    if (sapUsers.Count > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User already have an active FLINS account - {0}. Please apply for a secondary account in {1}.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName), null);

                        return true;
                    }
                }
                var sapUserValue = SAPUserinfo.GetLockedAccountByName(user.UserName, user.Id, Endpoint.Id, CurrentRequest.AccountType);
                if (sapUserValue != null)
                {
                    foreach (ProvisioningParam gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPUserinfo;
                    }
                    sapUserValue.UserType = SAPUserType.Dialog;
                    
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

                        if (user.FullTimeEmployee)
                        {
                            user.NeedFLINSLoginIdUpdateToHR = true;
                        }
                        
                        user.FLINSLoginId = sapUserValue.UserLogonName;
                        
                        user.Save(MasterConfigHelper.ServiceUser);


                        this.ReOrg(user, fields);

                        sapUserValue.NeedEPGroupAssignment = true;

                        string rolesFromMatrix = GetSAPRolesFromMatrix(user);

                        roles = roles + ',' + fields["Roles"].Value + "," + rolesFromMatrix;

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
                        sapUserValue.Save(MasterConfigHelper.ServiceUser);

                        sapUserValue.Password = ConstructPassword();
                        EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
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

                    foreach (ProvisioningParam gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPUserinfo;
                    }

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = ConstructUserName(user);
                    if (string.IsNullOrEmpty(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = ConstructPassword();

                    sapUserValue.UserType = SAPUserType.Dialog;

                    sapUserValue.NeedEPGroupAssignment = true;

                    string RolesFromMatrix = GetSAPRolesFromMatrix(user);

                    roles = roles + "," + fields["Roles"].Value + "," + RolesFromMatrix;

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
                    if (fields["Parameters"] != null && fields["Parameters"].Value != null)
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

                    //user group
                    sapUserValue.Groups = ConstructSAPUserGroup(user);

                    //OutPut Device - Default Printer
                    sapUserValue.OuputDevice = fields["OutputDevice"].Value;

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
                    #endregion
                    #region AD AccountName
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        sapUserValue.UserSamAccountName = GetUserADPrimaryAccountName(user);
                    }
                    #endregion

                    isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType, isForSAPSSO: isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }

                        //update flins login id for sapep and ce reconcilation.
                        if (user.FullTimeEmployee)
                        {
                            user.NeedFLINSLoginIdUpdateToHR = true;
                            user.FLINSLoginId = sapUserValue.UserLogonName;
                        }
                        else
                        {
                            user.FLINSLoginId = sapUserValue.UserLogonName;
                        }
                        user.Save(MasterConfigHelper.ServiceUser);

                        AddSAPUser(sapUserValue);

                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);

                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", sapUserValue.Roles));

                        CreateDependentSystemEntriesInIDMS(sapUserValue,user);

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
                throw ex;
            }

            return isExecutedSuccessfully;
        }

        string GetSAPRolesFromMatrix(DomainModel.User usr)
        {
            string RolesBasedOnEEGroup = string.Empty;
            string RolesBasedOnPosition = string.Empty;

            string RolesBasedOnOrg = ProvisioningConfigMatrixSAPOrg.GetSAPRolesBasedOnUserOrg(usr);

            if (!string.IsNullOrEmpty(usr.EEGroup))
                RolesBasedOnEEGroup = ProvisioningConfigMatrixSapEEGroup.GetSAPRolesBasedOnEEGroup(usr);
            if (!string.IsNullOrEmpty(usr.PositionName))
                RolesBasedOnPosition = ProvisioningConfigMatrixSAPPosition.GetSAPRolesBasedOnUserPosition(usr);
            string RolesFromMatrix = RolesBasedOnOrg + "," + RolesBasedOnEEGroup + "," + RolesBasedOnPosition;
            RolesFromMatrix = RolesFromMatrix.Replace("\n", "");
            return RolesFromMatrix;
        }
        protected override string ConstructUserName(DomainModel.User user)
        {
            string UserName = null;
            if (user == null)
            {
                throw new Exception("DI User is NULL in ConstructUserName");

            }
            var accountRecons = AccountReconcilation.GetByAccountTypeAndApplicationType(user.Id, AccountType.User, ApplicationType.ADSingaporePowerLocal);
            if (accountRecons != null && accountRecons.Count > 0)
            {
                var account = accountRecons.FirstOrDefault(u => u.IsLocked == false && u.IsDeleted == false);
                if (account != null)
                    UserName = account.AccountName;
            }
            if (string.IsNullOrWhiteSpace(UserName) && user.FullTimeEmployee)
            {
                var str = $"The user does not have AD User account in {UtilityMethods.GetEnumAttribute<Description>(ApplicationType.ADSingaporePowerLocal)}. Thus IDMS cannot be proceed provisioning the request - {CurrentRequest.Id}.";
                var ex = new Exception(str);
                WriteProvisioningLog(LogLevelType.Exception, str, ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(UserName) && !user.FullTimeEmployee)
            {
                UserName = base.ConstructUserName(user);
            }

            return UserName;
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

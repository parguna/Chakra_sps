using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User.NovaFlinsBi.User
{
    class NovaFlinsBi : FLINSBI, INovaFlinsBiSubsystem
    {
        ProvisioningParamCollection novaFields = new ProvisioningParamCollection();
        string _existingroles;
        public NovaFlinsBi(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            EndPoint ep = EndPoint.GetByApplicationType(ApplicationType.EAM);
            novaFields = NProvisioningField.GetProvisioningDefaults(ep.Id, AccountType.User);
            if (novaFields.Any(i => i.Name.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(novaFields["BWRoles"].Value)))
            {
                _existingroles = novaFields["BWRoles"].Value;
            }
        }

        public bool NovaFlinsBiAmendAdd(DomainModel.User user, long endpointUserId, string roles)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(_existingroles))
                    roles = _existingroles + "," + roles;

                success = AmendentAdd(user, endpointUserId, roles);
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendAdd roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsBiAmendAdd. TCodes are - {roles}");

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while Adding roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsBiAmendAdd. {ex.Message}", ex);
                throw ex;
            }
            return success;
        }

        protected override bool AmendentAdd(DomainModel.User user, long endpointUserId, string roles)
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

                   // ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserAddedInToGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserAddedInToGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                    throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserAddedInToGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName));
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding roles to the sap account. {0}", ex.Message), ex);
                return false;
            }
        }

        public bool NovaFlinsBiAmendRemove(DomainModel.User user, long endpointUserId, string roles)
        {
            bool success = false;
            try
            {
                success = AmendentRemoveAction(user, endpointUserId, roles);
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendRemove roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsBiAmendRemove. TCodes are - {roles}");

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while removing roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsBiAmendRemove. {ex.Message}", ex);
                throw ex;
            }
            return success;
        }

        protected override bool AmendentRemoveAction(DomainModel.User user, long endpointUserId, string roles)
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

                string grpListStr = string.Join(",", sapUser.Roles);

                bool isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(sapUser, EntitlementType.Role);
                if (isExecutedSuccessfully)
                {
                    SAPUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);

                   // ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                    throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName));
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in SAPT1SpecialBase.AmendentRemoveAction. {0}", ex.Message), ex);
                throw ex;
            }
        }
        public bool NovaFlinsBiCreateAccount(DomainModel.User user, string roles, out SAPUserinfo sapUserValue)
        {

            _idmsLogger.WriteLog(LogLevelType.DebugStep, "NovaFlinsBiCreateAccount Started..");

            sapUserValue = null;
            

            bool isExecutedSuccessfully = false;

            try
            {
                var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(Endpoint.Id) && this.CurrentRequest.AccountType == AccountType.User;
                string adAccountName = string.Empty;
                if (isSSoEnabled)
                {
                    adAccountName = GetUserADPrimaryAccountName(user);
                }
                ProvisioningParamCollection fields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
                //sapUserValue = SAPUserinfo.GetLockedAccount(user.Id, Endpoint.Id, AccountType.User);
                sapUserValue = SAPUserinfo.GetLockedAccountByName(user.UserName, user.Id, Endpoint.Id, AccountType.User);
                if (sapUserValue != null)
                {
                    sapUserValue.UserSamAccountName = adAccountName;
                    UpdateProvisioningField(fields, sapUserValue, user);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, "NovaFlinsBiCreateAccount before unlock ");

                    isExecutedSuccessfully = UnlockUser(sapUserValue);

                    if (isExecutedSuccessfully)
                    {
                        EndpointConnector.UpdateSSOAttributes(sapUserValue, adAccountName, AccountType.User);
                        this.ReOrg(user, fields);

                        if (novaFields.Any(i => i.Name.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(novaFields["BWRoles"].Value)))
                        {
                            roles = roles + ',' + novaFields["BWRoles"].Value;
                        }

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

                        _idmsLogger.WriteLog(LogLevelType.DebugStep, "NovaFlinsBiCreateAccount Before AddEntitlement..");


                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", roles));
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"NovaFlinsBiCreateAccount Before Save, User - {sapUserValue.AccountName}");
                        sapUserValue.Save(MasterConfigHelper.ServiceUser);
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, "NovaFlinsBiCreateAccount After Save");
                        sapUserValue.Password = ConstructPassword();
                        EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType,null, isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  FLINS Bi User Creation"));
                        }
                    }
                    else
                    {
                        throw new Exception($"Could not unlock the account - {sapUserValue.AccountName}");
                    }
                }
                else
                {
                    var sapUsers = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                    if (sapUsers.Count > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("User already have an active account - {0} in {1}. Please apply for a secondary account in {1}.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName), null);
                        sapUserValue = sapUsers.FirstOrDefault();
                    }

                    if (sapUserValue == null)
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

                    if (novaFields.Any(i=>i.Name.Equals("BWRoles",StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(novaFields["BWRoles"].Value)))
                    {
                        roles = roles + "," + novaFields["BWRoles"].Value;
                    }

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
                    #region sso
                    sapUserValue.UserSamAccountName = adAccountName;
                    #endregion
                    isExecutedSuccessfully = EndpointConnector.CreateAccount(sapUserValue, fields);
                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType,null, isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  FLINS Bi User Creation"));
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

                        throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, Endpoint.DisplayName));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating SAP account for the user {0}. The error is {1}", user.DisplayName, ex.GetOriginalException()), ex);
                throw ex;
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, "NovaFlinsBiCreateAccount End.");
            return isExecutedSuccessfully;
        }

        public bool NovaFlinsBiDeleteAccount(DomainModel.User user, long endpointUserId)
        {
            return DeleteAccount(user, endpointUserId);
        }
        protected override bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);

                bool isSuccess = LockUser(sapUser, LockScenario.AdhocRequest);

                //if (isSuccess)
                //{
                //    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, sapUser.UserLogonName);
                //}
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the EAM NOVA FLINS BI account. {0}", ex.Message), ex);
                throw ex;
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
            else
            {
                WriteProvisioningLog(LogLevelType.Warning, $"AD Accountname not found for Employee:{user?.EmployeeId}");
            }
            return accName;
        }

    }
}

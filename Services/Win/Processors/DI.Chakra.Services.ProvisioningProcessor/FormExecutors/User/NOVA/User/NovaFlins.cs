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

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User.NOVAFlins.User
{
   internal class NovaFlins :  FLINS, INovaFlinsSubsystem
    {
        string _existingroles;
        ProvisioningParamCollection Novafields = new ProvisioningParamCollection();
        public NovaFlins(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            EndPoint ep = EndPoint.GetByApplicationType(ApplicationType.EAM);
            Novafields = NProvisioningField.GetProvisioningDefaults(ep.Id, AccountType.User);
            if (Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
            {
                _existingroles = Novafields["CommonRoles"].Value;
            }
        }

        public bool NovaFlinsDeleteAccount(DomainModel.User user, long endpointUserId)
        {
           return this.DeleteAccount(user, endpointUserId);
        }

        protected override bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);

                bool isSuccess = LockUser(sapUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {
                    DeleteFLINSDependentAccount(user, sapUser.UserLogonName);
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"User.NovaFlins:Account is locked for User:{sapUser.UserLogonName}");
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("User.NovaFlins:Error while deleting the EAM NOVA FLINS account. {0}", ex.Message), ex);
                throw ex;
            }
        }

        public bool NovaFlinsAmendAdd(DomainModel.User user, long endpointUserId, string roles)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(_existingroles))
                    roles = _existingroles + "," + roles;

                success = AmendentAdd(user, endpointUserId, roles);
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendAdd roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsAmendAdd. TCodes are - {roles}");


            }
            catch (Exception ex)
            {
                var err = string.Format("Error while Adding roles from the flins sap account in NovaFlinsAmendAdd. {0}", ex.Message);
                WriteProvisioningLog(LogLevelType.Exception, err, ex);
                throw new NovaException(err);
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

                    //ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssigned, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssignmentFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
                    throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserRoleAssignmentFailed, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName));

                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding roles to the sap account. {0}", ex.Message), ex);
                throw ex;
            }
        }


        public bool NovaFlinsAmendRemove(DomainModel.User user, long endpointUserId, string roles)
        {
            bool success = false;
            try
            {
                var sapUser = SAPUserinfo.GetById(endpointUserId);
                var rolesToRemove = UtilityMethods.GetListFromString(roles);
                sapUser.Roles = rolesToRemove;
                success = AmendentRemoveAction(user, sapUser);
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendRemove roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsAmendRemove. TCodes are - {roles}");

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while removing roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsAmendRemove. {ex.Message}", ex);
                throw ex;
            }
            return success;
        }


        protected override bool AmendentRemoveAction(DomainModel.User user, SAPUserinfo sapUser)
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
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing roles from the sap account in AmendentRemoveAction. {0}", ex.Message), ex);
                throw ex; 
            }
        }

        public bool NovaFlinsCreateAccount(DomainModel.User user, string roles, AdHocSAPSharedAccount sapDetail, out SAPUserinfo sapUserValue)
        {
            ProvisioningParamCollection fields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);

            bool isExecutedSuccessfully = false;
            sapUserValue = null;
            try
            {
                var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(Endpoint.Id) && this.CurrentRequest.AccountType == AccountType.User;
                sapUserValue = SAPUserinfo.GetLockedAccount(user.Id, Endpoint.Id, AccountType.User);
                string adAccountName = string.Empty;
                if (isSSoEnabled)
                {
                    adAccountName = GetUserADPrimaryAccountName(user);
                }
                if (sapUserValue != null)
                {

                    sapUserValue.UserType = SAPUserType.Dialog; 
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    sapUserValue.UserSamAccountName = adAccountName;
                    isExecutedSuccessfully = UnlockUser(sapUserValue);

                    if (isExecutedSuccessfully)
                    {
                        EndpointConnector.UpdateSSOAttributes(sapUserValue, adAccountName, AccountType.User);
                        this.ReOrg(user, fields);

                        sapUserValue.NeedEPGroupAssignment = true;
                        if (this.Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
                            roles = roles + ',' + Novafields["CommonRoles"].Value;

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
                        EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, ApplicationType.HRMS_FLINS, null, isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  FLINS User Creation"));
                        }
                    }
                }
                else
                {
                     
                    var sapUsers = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                    if (sapUsers.Count > 0)
                    { 
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User already have an active FLINS account - {0}. Please apply for a secondary account in {1} - Nova FLINS User Account CreatAccount.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName));
                        sapUserValue = sapUsers.FirstOrDefault();
                    }
                     
                    if(sapUserValue == null)
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
                    if (Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
                        roles = roles + "," + Novafields["CommonRoles"].Value ;

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
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, ApplicationType.HRMS_FLINS,null, isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for Nova FLINS User Account CreatAccount"));
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
                        throw new Exception(string.Format(ProvisioningConstants.MSG_ProvisioningSAPUserCreationFailed, sapUserValue.UserLogonName, Endpoint.DisplayName));
                    }
                }
            }
            catch (Exception ex)
            {
                var error = $"Error while creating FLINS account for the user {user.DisplayName} in  Nova FLINS User Account CreatAccount";
                WriteProvisioningLog(LogLevelType.Exception, error,  ex);
                throw new NovaException(error);
            }

            return isExecutedSuccessfully;
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

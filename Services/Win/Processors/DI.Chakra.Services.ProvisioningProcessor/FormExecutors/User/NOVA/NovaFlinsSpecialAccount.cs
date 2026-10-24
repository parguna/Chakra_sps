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

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User 
{
    internal class NovaFlinsSpecialAccount : DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Service.FLINS, INovaFlinsSubsystem
    {
        string _existingroles;
        public NovaFlinsSpecialAccount(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            EndPoint ep = EndPoint.GetByApplicationType(ApplicationType.EAM);
            var Novafields = NProvisioningField.GetProvisioningDefaults(ep.Id, currentRequest.AccountType);
            if (Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
            {
                _existingroles = Novafields["CommonRoles"].Value;
            }
        }

        public bool NovaFlinsDeleteAccount(DomainModel.User user, long endpointUserId)
        {
            return DeleteAccount(user, endpointUserId);
        }
        public bool NovaFlinsAmendAdd(DomainModel.User user, long endpointUserId, string roles)
        {
            bool success = false;
            try
            {
                success = AmendentAdd(user, endpointUserId, roles);
                WriteProvisioningLog(LogLevelType.Info, $"Successfully AmendAdd roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsAmendAdd. TCodes are - {roles}");


            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while Adding roles from the flins sap account in NovaFlinsAmendAdd. {0}", ex.Message), ex);
                return false;
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

                sapUser.UserValidTo = this.getSAPDateFormat((DateTime) GetValidTo(user.Id));

                bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(sapUser, sapUser.UserValidFrom, sapUser.UserValidTo, EntitlementType.Role);
                string grpListStr = string.Join(",", sapUser.Roles);

                if (isExecutedSuccessfully)
                {
                    SAPUserinfo.AddEntitlement(endpointUserId, EntitlementType.Role, grpListStr);

                   // ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
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
                WriteProvisioningLog(LogLevelType.Exception, $"Successfully AmendRemove roles for the user: {user.DisplayName}-({user.Id}) in NovaFlinsAmendRemove. TCodes are - {roles}");

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

                sapUser.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(user.Id));

                string grpListStr = string.Join(",", sapUser.Roles);

                bool isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(sapUser, EntitlementType.Role);
                if (isExecutedSuccessfully)
                {
                    SAPUserinfo.RemoveEntitlement(sapUser.Id, EntitlementType.Role, grpListStr);

                    //ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, sapUser.UserLogonName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, sapUser.UserLogonName, grpListStr, Endpoint.DisplayName), null);
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
                throw ex;
            }
        }

        public bool NovaFlinsCreateAccount(DomainModel.User user, string roles, AdHocSAPSharedAccount sapDetail, out SAPUserinfo sapUserValue)
        {

            bool isExecutedSuccessfully = false;
            var fields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, CurrentRequest.AccountType);
            
            sapUserValue = null;
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"SapDetails is null - {sapDetail == null}: Account - {sapDetail.AccountName}");
                WriteProvisioningLog(LogLevelType.DebugStep, $"Roles - {roles}");
                WriteProvisioningLog(LogLevelType.DebugStep, $"EndpointId - {Endpoint.Id}");
                 

                sapUserValue = SAPUserinfo.GetLockedAccountByName(sapDetail.AccountName, user.Id, Endpoint.Id, CurrentRequest.AccountType);

                if (sapUserValue != null)
                {
                    sapUserValue.UserType = sapDetail.SAPUserType;
                    sapUserValue.Firstname = sapDetail.Description;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    UpdateProvisioningField(fields, sapUserValue, user);
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
                        if (!string.IsNullOrWhiteSpace(_existingroles))
                        {
                            var sRole = UtilityMethods.GetListFromString(_existingroles);
                            if(sRole != null && sRole.Count > 0)
                            {
                                sapRoles.AddRange(sRole);
                            }
                        }
                        WriteProvisioningLog(LogLevelType.DebugStep, $"SAP Roles - {UtilityMethods.GetStringFromList(sapRoles)}");
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
                            EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        }
                        else
                        {
                            EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
                        }

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
                    UpdateProvisioningField(fields, sapUserValue, user);
                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = ConstructUserName(sapDetail.AccountName);
                    sapUserValue.Firstname = sapDetail.Description;
                    sapUserValue.Lastname = sapUserValue.UserLogonName;
                    sapUserValue.UserType = sapDetail.SAPUserType;

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
                    if (!string.IsNullOrWhiteSpace(_existingroles))
                    {
                        var sRole = UtilityMethods.GetListFromString(_existingroles);
                        if (sRole != null && sRole.Count > 0)
                        {
                            sapRoles.AddRange(sRole);
                        }
                    }
                    WriteProvisioningLog(LogLevelType.DebugStep, $"SAP Roles - {UtilityMethods.GetStringFromList(sapRoles)}");
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

                     
                   sapUserValue.UserValidTo = this.getSAPDateFormat((DateTime)GetValidTo(user.Id));

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
                throw ex;
            }

            return isExecutedSuccessfully;
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
                    WriteProvisioningLog(Endpoint.Id, LogLevelType.Info, $"Account is locked for User:{sapUser.UserLogonName}");
                     
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the EAM NOVA account. {0}", ex.Message), ex);
                throw ex;
            }
        }


    }
}

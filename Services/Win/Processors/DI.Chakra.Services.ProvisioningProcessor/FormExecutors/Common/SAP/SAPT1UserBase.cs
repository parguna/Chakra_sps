using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    class SAPT1UserBase : Common.SAPT1SpecialBase
    {

        public SAPT1UserBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
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

            _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase Execute 2 ");
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

        DateTime? GetValidTo(Nullable<long> userId)
        {
            var validTo = DateTime.MaxValue;
            if ((CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

            if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
                return CurrentRequest.DeactivationDate.Value;
            else
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if (user != null && user.TerminationDate.HasValue) validTo = user.TerminationDate.Value;
                }
            }
            return validTo;
        }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string roles)
        {
            _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase CreateAccount Started..");
            bool isExecutedSuccessfully = false;

            try
            {
                var constructedAccountName = ConstructUserName(user);
                var sapUserValue = SAPUserinfo.GetLockedAccount(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(Endpoint.Id) && CurrentRequest.AccountType == AccountType.User;

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Found locked account - {sapUserValue!=null}");


                if (sapUserValue != null)
                {
                    

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Constructed Account Name - {constructedAccountName}");
                    
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Locked Account Name - {sapUserValue.AccountName}");

                    if (CurrentRequest.AccountType == AccountType.User && sapUserValue != null && !sapUserValue.AccountName.Equals(constructedAccountName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        sapUserValue = null;
                    }
                }

                if (sapUserValue != null)
                {
                    UpdateProvisioningField(fields, sapUserValue, user);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase before unlock ");

                    sapUserValue.UserType =  SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    
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

                        if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                        {
                            roles = roles + ',' + fields["Roles"].Value;
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

                        sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

                        _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase Before AddEntitlement..");


                        EndpointConnector.AddEntitlement(sapUserValue, sapUserValue.UserValidFrom, sapUserValue.UserValidTo, EntitlementType.Role);
                        SAPUserinfo.AddEntitlement(sapUserValue.Id, EntitlementType.Role, string.Join(",", roles));
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAPT1UserBase Before Save, User - {sapUserValue.AccountName}");
                        sapUserValue.Save(MasterConfigHelper.ServiceUser);
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase After Save");
                        sapUserValue.Password = ConstructPassword();
                        EndpointConnector.ResetPassword(sapUserValue.UserLogonName, sapUserValue.Password, string.Empty);
                        //EndpointConnector.ChangePassword(sapUserValue.UserLogonName, sapUserValue.Password, user.Id, true);
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, sapUserValue, user, CurrentRequest.Application.ApplicationType, isForSAPSSO: isSSoEnabled);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }
                    }
                }
                else
                {
                    var sapUsers = SAPUserinfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                    if (sapUsers.Count > 0)
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format("User already have an active account - {0} in {1}. Please apply for a secondary account in {1}.", sapUsers.FirstOrDefault().UserLogonName, Endpoint.DisplayName), null);

                        return false;
                    }

                    sapUserValue = new SAPUserinfo();
                    string accountName = string.Empty;

                    #region Set SAP User object Values

                    foreach (ProvisioningParam gData in fields)
                    {
                        sapUserValue = setUserInfoByAtrbValue(sapUserValue, user, gData) as SAPUserinfo;
                    }

                    sapUserValue.IsActive = true;
                    sapUserValue.EndPointSystemId = Endpoint.Id;
                    sapUserValue.UserLogonName = constructedAccountName;
                    if (string.IsNullOrEmpty(sapUserValue.UserLogonName))
                    {
                        return false;
                    }
                    sapUserValue.Password = ConstructPassword();

                    sapUserValue.UserType = SAPUserType.Dialog;
                    sapUserValue.SapUserType = GetSAPUserType(sapUserValue.UserType);
                    if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                    {
                        roles = roles + "," + fields["Roles"].Value;
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

                    sapUserValue.UserValidTo = this.getSAPDateFormat(GetValidTo(user.Id).Value);

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
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating SAP account for the user {0}. The error is {1}", user.DisplayName, ex.GetOriginalException()), ex);
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, "SAPT1UserBase CreateAccount End.");
            return isExecutedSuccessfully;
        }

        protected override string ConstructUserName(DomainModel.User  user)
        {
            LogDebugStep("ConstructUserName for the user Id:" + user.Id);
            string userName = string.Empty;
            StringBuilder userNameSB = new StringBuilder();

            try
            {
                userName = removeSpecialCharacters(user.DisplayName, false, false, user.Ethinicity);

                var firstLetter = Enum.GetName(typeof(EmployeeType), user.EmployeeType)[0];

                userNameSB.Append(firstLetter);

                if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
                {
                    string[] nameAry = userName.Split(' ');
                    if (nameAry.Length == 1)
                    {
                        if (nameAry[0].Length >= 3)
                            userNameSB.Append(nameAry[0].Substring(0, 3));
                        else
                            userNameSB.Append(nameAry[0]);
                    }
                    else if (nameAry.Length == 2)
                    {
                        if (nameAry[0].Length >= 2)
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 2));
                            userNameSB.Append(nameAry[1].Substring(0, 1));
                        }
                        else if (nameAry[1].Length >= 2)
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 1));
                            userNameSB.Append(nameAry[1].Substring(0, 2));
                        }
                        else
                        {
                            userNameSB.Append(nameAry[0]);
                            userNameSB.Append(nameAry[1]);
                        }
                    }
                    else if (nameAry.Length >= 3)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 1));
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                        userNameSB.Append(nameAry[2].Substring(0, 1));
                    }
                }

                if (!string.IsNullOrEmpty(user.EmployeeId) && user.EmployeeId.Length >= 5)
                {
                    userNameSB.Append(user.EmployeeId.Remove(0, user.EmployeeId.Length - 4));
                }
                userName = userNameSB.ToString().ToUpper();
               // userName = CheckUserNameExists(userName, 8);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while Constructing User Name", ex);
                throw ex;
            }

            LogDebugStep($"Constructed UserName for the user Id:{user.Id} is {userName}");

            return userName;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared || CurrentRequest.AccountType == AccountType.Secondary)
                    return true;
                string roles = this.CurrentRequest.EventUndoRequest.TechnicalRoles;

                bool isUpdated = true;

                var sapUsr = SAPUserinfo.GetById(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                if (sapUsr != null)
                {
                    #region AD AccountName
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        sapUsr.UserSamAccountName = GetUserADPrimaryAccountName(user);
                    }
                    #endregion
                    isExecutedSuccessfully = this.UnlockUser(sapUsr);
                    if (isExecutedSuccessfully)
                    {
                        EndpointConnector.UpdateSSOAttributes(sapUsr, sapUsr.UserSamAccountName, CurrentRequest.AccountType);
                        this.ReOrg(user, fields);

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
                    }
                }
                else
                {
                    CreateAccount(fields, user, roles);
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                return false;
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

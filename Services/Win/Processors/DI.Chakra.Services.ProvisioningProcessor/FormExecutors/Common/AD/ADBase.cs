using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    class ADBase : FormExecutorBase
    {

        public ADBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            IEntitlementDetail adDetail = null;
            string groups = string.Empty;

            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                adDetail = CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (adDetail != null)
                {
                    groups = adDetail.Entitlement;
                    endpointUserId = adDetail.EndpointUserId;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccount(fields, user, groups, adDetail);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, endpointUserId, groups);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, endpointUserId, groups);
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
                string disabledOU = string.Empty;
                if (CurrentRequest.ApplicationType == ApplicationType.SAPLocal)
                {
                    disabledOU = MasterConfigHelper.ADSAPLocalDisabledAccountsOU;
                }
                else if (CurrentRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    disabledOU = MasterConfigHelper.ADDisabledAccountsOU;
                }

                if (!EndpointConnector.IsUserPropertyExists("OU", disabledOU, null))
                {
                    throw new Exception($"OU:{ disabledOU} does not exists. Please contact endpoint system owner");
                }
                var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (ADSUserInfo adUserValue in adUsers)
                {
                    TakeOUSnapshot(user.Id, adUserValue, EmployeeEvents.Resignation, EndpointType.ActiveDirectory, EntitlementType.Group);

                    adUserValue.accountExpires = user.TerminationDate;
                    adUserValue.LastModifiedBy = MasterConfigHelper.ServiceUser;
                    adUserValue.LastModifiedDate = DateTime.Now;

                    EndpointConnector.ExtendValidity(adUserValue);
                    adUserValue.Save(MasterConfigHelper.ServiceUser);

                    bool isSuccess = DisableUser(adUserValue, LockScenario.OffBoard);
                    if (isSuccess)
                        EndpointConnector.MoveOU(adUserValue, disabledOU);
                    else
                        return false;
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
            return true;
        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {

            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            try
            {
                string disabledOU = string.Empty;
                if (CurrentRequest.ApplicationType == ApplicationType.SAPLocal)
                {
                    disabledOU = MasterConfigHelper.ADSAPLocalDisabledAccountsOU;
                }
                else if (CurrentRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    disabledOU = MasterConfigHelper.ADDisabledAccountsOU;
                }

                if (!EndpointConnector.IsUserPropertyExists("OU", disabledOU, null))
                {
                    throw new Exception($"OU:{ disabledOU} does not exists. Please contact endpoint system owner");
                }
                var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (ADSUserInfo adUserValue in adUsers)
                {
                    TakeOUSnapshot(user.Id, adUserValue, EmployeeEvents.OUChange, EndpointType.ActiveDirectory, EntitlementType.Group);

                    bool isSuccess = DisableUser(adUserValue, LockScenario.OUChange);
                    if (isSuccess)
                        EndpointConnector.MoveOU(adUserValue, disabledOU);
                    else
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing transfer request. {0}", ex.Message), ex);
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
                        //if (accountRecon.ValidToDate.HasValue &&
                        //    (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value))
                        if(accountRecon.AccountType == AccountType.User || accountRecon.AccountType == AccountType.Secondary)
                        {
                            var adUsr = ADSUserInfo.GetById(accountRecon.EndpointUserId);

                            if (adUsr != null)
                            {
                                adUsr.accountExpires = user.TerminationDate;

                                adUsr.LastModifiedBy = "System";
                                adUsr.LastModifiedDate = DateTime.Now;

                                isUpdated = EndpointConnector.ExtendValidity(adUsr);

                                if (isUpdated)
                                {
                                    adUsr.Save(MasterConfigHelper.ServiceUser);
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", adUsr.sAMAccountName, oldTerminationDate, user.TerminationDate), null);
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

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                var adUserValue = ADSUserInfo.GetById(CurrentRequest.EventUndoRequest.EndpointUserId);

                if (Endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    if (CurrentRequest.AccountType == AccountType.Secondary)
                        adUserValue.ou = MasterConfigHelper.ADSecondaryAccountOU;
                }
                else if (Endpoint.ApplicationType == ApplicationType.SAPLocal)
                {
                    if (CurrentRequest.AccountType == AccountType.User)
                        adUserValue.ou = MasterConfigHelper.ADSAPLocalUserAccountsOU;
                    else if (CurrentRequest.AccountType == AccountType.Secondary)
                        adUserValue.ou = MasterConfigHelper.ADSAPLocalSecondaryAccountsOU;
                }
                if (!EndpointConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
                {
                    throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
                }
                EnableUser(adUserValue);
                ReOrg(adUserValue);

                if (user.FullTimeEmployee)
                {
                    adUserValue.accountExpires = null;
                }
                else
                {
                    adUserValue.accountExpires = user.TerminationDate;
                }

                EndpointConnector.PostReactivationProcedure(adUserValue);


                string groups = CurrentRequest.EventUndoRequest.TechnicalGroups;

                if (!string.IsNullOrEmpty(groups))
                {
                    adUserValue.Groups = UtilityMethods.GetListFromString(groups);
                }

                //Check if user has group add user in AD Groups
                if (adUserValue.Groups != null && adUserValue.Groups.Count > 0)
                {
                    bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                    string grpListStr = string.Join(",", adUserValue.Groups);

                    if (isExecutedSuccessfully)
                    {
                        ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, groups, Endpoint), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, groups, Endpoint), null);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                return false;
            }
        }

        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string groups, IEntitlementDetail adDetail)
        {

            bool isExecutedSuccessfully = false;
            try
            {
                var accountname = string.Empty;
                var description = string.Empty;
                var displayname = string.Empty;
                var ou = string.Empty;
                if (adDetail is AdHocSAPLocalRequest)
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, "adDetail is AdHocSAPLocalRequest");
                    var tmpDetail = adDetail as AdHocSAPLocalRequest;
                    accountname = string.IsNullOrWhiteSpace(tmpDetail.AccountName) ? tmpDetail.SharedAccountDetail?.AccountName : tmpDetail.AccountName;
                    description = tmpDetail.Description;
                    displayname = tmpDetail.DisplayName;
                    ou = tmpDetail.OU;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, "adDetail is AdHocADRequest");
                    var tmpDetail = adDetail as AdHocADRequest;
                    accountname = tmpDetail.AdHocADSharedAccount.AccountName;
                    description = tmpDetail.AdHocADSharedAccount.Description;
                    displayname = tmpDetail.AdHocADSharedAccount.DisplayName;
                    ou = tmpDetail.AdHocADSharedAccount.OU;
                }

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                var adUserValue = ADSUserInfo.GetLockedAccountByName(accountname, user.Id, Endpoint.Id, CurrentRequest.AccountType);

                if (adUserValue != null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"LockedAccountByName: { adUserValue.sAMAccountName}");
                    adUserValue.description = description;

                    if (!string.IsNullOrWhiteSpace(ou))
                    {
                        adUserValue.ou = ou;
                    }
                    else
                    {
                        if (Endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            if (CurrentRequest.AccountType == AccountType.Shared)
                                adUserValue.ou = MasterConfigHelper.ADSharedAccountOU;
                            else if (CurrentRequest.AccountType == AccountType.Service)
                                adUserValue.ou = MasterConfigHelper.ADServiceAccountOU;
                            else if (CurrentRequest.AccountType == AccountType.Secondary)
                                adUserValue.ou = MasterConfigHelper.ADSecondaryAccountOU;
                        }
                        else if (Endpoint.ApplicationType == ApplicationType.SAPLocal)
                        {
                            if (CurrentRequest.AccountType == AccountType.Shared)
                                adUserValue.ou = MasterConfigHelper.ADSAPLocalSharedAccountsOU;
                            else if (CurrentRequest.AccountType == AccountType.Service)
                                adUserValue.ou = MasterConfigHelper.ADSAPLocalServiceAccountsOU;
                            else if (CurrentRequest.AccountType == AccountType.Secondary)
                                adUserValue.ou = MasterConfigHelper.ADSAPLocalSecondaryAccountsOU;
                        }
                    }
                    if (!ADConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
                    {
                        throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
                    }
                    adUserValue.sAMAccountName = accountname;
                    adUserValue.cn = adUserValue.sAMAccountName;
                    adUserValue.displayName = displayname;
                    adUserValue.sn = displayname;
                    //extensionAttribute11 - Account Type
                    if (this.CurrentRequest.AccountType != AccountType.None)
                        adUserValue.extensionAttribute11 = this.CurrentRequest.AccountType.GetDescription();

                    //extensionAttribute12 - Account Owner                    
                    adUserValue.extensionAttribute12 = !string.IsNullOrWhiteSpace(user.KnownAs) ? user.KnownAs : user.FirstName + user.LastName;

                    //adUserValue.info += $"Enabled on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest.Id};";
                    WriteProvisioningLog(LogLevelType.Info, $"EnableUser for { adUserValue.sAMAccountName}");
                    adUserValue.accountExpires = CurrentRequest.DeactivationDate == DateTime.MinValue ? null : (DateTime?)CurrentRequest.DeactivationDate;

                    isExecutedSuccessfully = EnableUser(adUserValue);

                    if (isExecutedSuccessfully)
                    {

                        isExecutedSuccessfully = ReOrg(adUserValue);
                        WriteProvisioningLog(LogLevelType.Info, $"ReOrg for { adUserValue.sAMAccountName} Success:{isExecutedSuccessfully}");
                        if (isExecutedSuccessfully)
                        {
                            adUserValue.NewUsrPassword = ConstructPassword();

                            isExecutedSuccessfully = ADConnector.ChangePassword(adUserValue.sAMAccountName, adUserValue.NewUsrPassword, user.Id, true);
                            if (isExecutedSuccessfully)
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"ChangePassword { adUserValue.sAMAccountName} Success:{isExecutedSuccessfully}");
                                isExecutedSuccessfully = ADConnector.PostReactivationProcedure(adUserValue);
                                if (isExecutedSuccessfully)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"PostReactivationProcedure { adUserValue.sAMAccountName} Success:{isExecutedSuccessfully}");
                                    bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, CurrentRequest.Application.ApplicationType);

                                    if (!Result)
                                    {
                                        WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                                    }
                                    adUserValue.Save(MasterConfigHelper.ServiceUser);
                                }
                            }
                        }
                    }
                    if (isExecutedSuccessfully)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreated, adUserValue.sAMAccountName, Endpoint.DisplayName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreationFailed, adUserValue.sAMAccountName, Endpoint.DisplayName), null);
                        return false;
                    }
                }
                else
                {
                    var isAccountExists = ADConnector.IsUserExists(accountname);
                    if (isAccountExists)
                    {
                        throw new Exception($"Account:{accountname} already exists. Cannot create an Account:{accountname}");
                    }

                    adUserValue = new ADSUserInfo();

                    #region Set AD Object

                    adUserValue.EndPointSystemId = Endpoint.Id;

                    adUserValue.description = description;

                    if (!string.IsNullOrWhiteSpace(ou))
                    {
                        adUserValue.ou = ou;
                    }
                    else
                    {
                        adUserValue.ou = MasterConfigHelper.ADSharedAccountOU;
                    }
                    if (!ADConnector.IsUserPropertyExists("OU", adUserValue.ou, null))
                    {
                        throw new Exception($"OU:{ adUserValue.ou} does not exists. Please contact endpoint system owner");
                    }
                    
                    string uName = accountname;
                    adUserValue.sAMAccountName = uName;
                    adUserValue.cn = uName;
                    adUserValue.displayName = displayname;
                    adUserValue.sn = displayname;
                    adUserValue.NewUsrPassword = ConstructPassword();
                    adUserValue.accountExpires = CurrentRequest.DeactivationDate == DateTime.MinValue ? null : (DateTime?)CurrentRequest.DeactivationDate;

                    //extensionAttribute11 - Account Type
                    if (this.CurrentRequest.AccountType != AccountType.None)
                        adUserValue.extensionAttribute11 = this.CurrentRequest.AccountType.GetDescription();

                    //extensionAttribute12 - Account Owner                    
                    adUserValue.extensionAttribute12 = !string.IsNullOrWhiteSpace(user.KnownAs) ? user.KnownAs : user.FirstName + user.LastName;

                    adUserValue.info += $"Created on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest.Id};";


                    
                    #endregion

                    var userExists = ADConnector.IsActiveAccount(adUserValue);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"userExists AD = {userExists}");
                    if (userExists)
                        isExecutedSuccessfully = ADConnector.UpdateAccount(adUserValue);
                    else
                        isExecutedSuccessfully = ADConnector.CreateAccount(adUserValue, fields);

                    if (isExecutedSuccessfully)
                    {
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, adUserValue, user, CurrentRequest.Application.ApplicationType);

                        if (!Result)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }

                        AddADUser(adUserValue);
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreated, adUserValue.sAMAccountName, Endpoint.DisplayName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserCreationFailed, adUserValue.sAMAccountName, Endpoint.DisplayName), null);
                        return false;
                    }
                }

                if (isExecutedSuccessfully)
                {
                    #region Add user in to Group if group exist for user

                    string[] defaultADGrps = groups.Split(',');

                    if (defaultADGrps != null)
                    {
                        foreach (string tmpGrpValue in defaultADGrps)
                        {
                            if (!string.IsNullOrEmpty(tmpGrpValue))
                            {
                                adUserValue.Groups.Add(tmpGrpValue.Trim());
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(fields["Groups"].Value))
                    {
                        foreach (var item in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(item) && !adUserValue.Groups.Contains(item.Trim()))
                            {
                                adUserValue.Groups.Add(item);
                            }
                        }
                    }
                     

                    WriteProvisioningLog(LogLevelType.Info, $"AddEntitlement started: { adUserValue.Groups?.Count}");
                    //Check if user has group add user in AD Groups
                    if (adUserValue.Groups != null && adUserValue.Groups.Count > 0)
                    {

                        bool isADGrpAssignment = ADConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                        string grpListStr = string.Join(",", adUserValue.Groups);
                        WriteProvisioningLog(LogLevelType.Info, $"isADGrpAssignment: { isADGrpAssignment}");
                        if (isADGrpAssignment)
                        {
                            ADSUserInfo.AddEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                        }
                        else
                        {

                            WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while creating the AD account. {0}", ex.Message), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }

        protected virtual string CheckUserNameExistsForSpecialAccount(string userName)
        {
            int no = 1;
            var acctRecon = AccountReconcilation.GetByAccountName(userName, this.Endpoint.Id);
            while (acctRecon != null)
            {
                userName = userName.Substring(0, userName.Length - 1) + no;
                acctRecon = AccountReconcilation.GetByAccountName(userName, Endpoint.Id);
                no++;
            }

            return userName;
        }
        bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                string disabledOU = string.Empty;

                if (CurrentRequest.ApplicationType == ApplicationType.SAPLocal)
                {
                    disabledOU = MasterConfigHelper.ADSAPLocalDisabledAccountsOU;
                }
                else if (CurrentRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    disabledOU = MasterConfigHelper.ADDisabledAccountsOU;
                }
                if (!EndpointConnector.IsUserPropertyExists("OU", disabledOU, null))
                {
                    throw new Exception($"OU:{ disabledOU} does not exists. Please contact endpoint system owner");
                }
                var adUser = ADSUserInfo.GetById(endpointUserId);
                

                bool isSuccess = DisableUser(adUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, adUser.sAMAccountName);
                    try
                    {
                        
                        EndpointConnector.MoveOU(adUser, disabledOU);
                        adUser.ou = disabledOU;
                        adUser.Save(MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while MoveOU. {0}", ex.Message), ex);
                        throw ex;
                    }
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected bool AmendentAdd(DomainModel.User user, long endpointUserId, string groups)
        {
            try
            {
                var adUserValue = ADSUserInfo.GetById(endpointUserId);

                adUserValue.Groups = groups.Split(',').ToList();

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                bool isExecutedSuccessfully = ADConnector.AddEntitlement(adUserValue, EntitlementType.Group);
                string grpListStr = string.Join(",", adUserValue.Groups);

                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.AddEntitlement(endpointUserId, EntitlementType.Group, grpListStr);

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {

                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding groups to the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected virtual bool AmendentRemove(DomainModel.User user, long endpointUserId, string groups)
        {
            try
            {

                var adUserValue = ADSUserInfo.GetById(endpointUserId);

                ConnectorBase ADConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

                var groupsToRemove = groups.Split(',').ToList();

                var existingGroups = ADSUserInfo.GetByIdIncludeADSGroups(adUserValue.Id).ADSGroups.Select(item => item.name).ToList();

                foreach (string groupName in groupsToRemove)
                {
                    if (existingGroups.Contains(groupName))
                    {
                        adUserValue.Groups.Add(groupName);
                    }
                }

                string grpListStr = string.Join(",", adUserValue.Groups);

                bool isExecutedSuccessfully = ADConnector.RemoveEntitlement(adUserValue, EntitlementType.Group);
                if (isExecutedSuccessfully)
                {
                    ADSUserInfo.RemoveEntitlement(adUserValue.Id, EntitlementType.Group, grpListStr);

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, grpListStr, adUserValue.sAMAccountName);
                    WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adUserValue.sAMAccountName, grpListStr, Endpoint.DisplayName), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }

        protected bool DisableUser(ADSUserInfo adUsr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (adUsr != null)
            {
                isusrDisabled = false;

                WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Remove Groups Started in Active Directory.", adUsr.sAMAccountName), null);

                adUsr.Groups = ADSUserInfo.GetByIdIncludeADSGroups(adUsr.Id).ADSGroups.Select(item => item.name).ToList();
                EndpointConnector.RemoveEntitlement(adUsr, EntitlementType.Group);


                WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} Disable account Started in Active Directory.", adUsr.sAMAccountName), null);

                adUsr.info += $"Disabled on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest?.Id ?? 0};";
                isusrDisabled = adUsr.IsAccountDisabled;
                WriteProvisioningLog(LogLevelType.Info, $"Is Account already Disable:{isusrDisabled}");
                isusrDisabled = EndpointConnector.DisableAccount((IEndpointUserInfo)adUsr);
                if (isusrDisabled)
                {
                    ADSUserInfo.RemoveEntitlement(adUsr.Id, EntitlementType.Group, string.Join(",", adUsr.Groups));
                    if (!string.IsNullOrWhiteSpace(adUsr.mail))
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"User:{adUsr.sAMAccountName} the following mail:{adUsr.mail} will be set Empty");
                        adUsr.mail = null;
                    }
                    adUsr.IsAccountDisabled = true;
                    adUsr.LastModifiedDate = DateTime.Now;
                    adUsr.LastModifiedBy = "System";
                    adUsr.Save(MasterConfigHelper.ServiceUser);

                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Disabled in Active Directory.", adUsr.sAMAccountName), null);
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Error in User {0} Disabled in Active Directory.", adUsr.sAMAccountName), null);
                }
            }

            return isusrDisabled;
        }

        protected bool EnableUser(ADSUserInfo adUsr)
        {
            bool isusrEnabled = true;

            if (adUsr != null)
            {
                isusrEnabled = false;

                adUsr.info += $"Enabled on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest?.Id ?? 0};";

                isusrEnabled = EndpointConnector.EnableAccount((IEndpointUserInfo)adUsr);
                if (isusrEnabled)
                {
                    adUsr.IsAccountDisabled = false;
                    adUsr.LastModifiedDate = DateTime.Now;
                    adUsr.LastModifiedBy = "System";

                    adUsr.Save(MasterConfigHelper.ServiceUser);
                    if (isusrEnabled)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} Enabled in Active Directory.", adUsr.sAMAccountName), null);
                    }
                }
            }

            return isusrEnabled;
        }

        protected void AddADUser(ADSUserInfo adUser)
        {
            string currentUser = MasterConfigHelper.ServiceUser;
            adUser.Save(currentUser);

            CreateUserInReconTable(adUser, currentUser);
        }

        bool ReOrg(ADSUserInfo adUserValue)
        {
            adUserValue.LastModifiedBy = "System";
            adUserValue.LastModifiedDate = DateTime.Now;

            EndpointConnector.ChangeContainer(adUserValue);

            adUserValue.Save(MasterConfigHelper.ServiceUser);

            return true;
        }
        public override bool UpdateAttributes(DomainModel.User user,long endpointUserId)
        {
            bool isUpdated = false;
            try
            {
                var supervisor = DomainModel.User.GetROByUser(user.Id);
                var adUser = ADSUserInfo.GetById(endpointUserId);
                if(adUser == null)
                {
                    throw new Exception($"AD User not found for User {user.UserName}");
                }
                WriteProvisioningLog(LogLevelType.Info, $"ADUser:{adUser.sAMAccountName} Supervisor:{supervisor.DisplayName}");
                adUser.extensionAttribute12 = !string.IsNullOrWhiteSpace(supervisor.KnownAs) ? supervisor.KnownAs : supervisor.FirstName + supervisor.LastName;
                isUpdated = EndpointConnector.UpdateAccountAttribute(adUser);
                WriteProvisioningLog(LogLevelType.Info, $"Update Account Attribue for User:{user.UserName} extensionAttribute12:{adUser.extensionAttribute12} Status:{isUpdated}");
                if (isUpdated)
                {
                    adUser.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"UpdateAttributes - extensionAttribute12 failed for user:{user.UserName}",ex);
                isUpdated = false;
            }
            return isUpdated;
        }
    }
}

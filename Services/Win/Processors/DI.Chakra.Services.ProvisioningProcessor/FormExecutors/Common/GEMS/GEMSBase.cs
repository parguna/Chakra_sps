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

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    public class GEMSBase : FormExecutorBase
    {
        string _userName = string.Empty;
        public GEMSBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }
        protected string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            //DateTime maxDate = new DateTime(2999, 12, 31);
            string gemsUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

            AdHocGEMSRequest gemsDetail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "GEMS Provisioning failed. User object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string businessRole = string.Empty;
            string adGroups = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                gemsDetail = (AdHocGEMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                fields["Roles"].Value = GetRolesFromAdhocRequest(user, fields["Roles"].Value, gemsDetail);
                adGroups = GetADGroupsFromBusinessRole(user, adGroups, gemsDetail);
                if (gemsDetail != null)
                {
                    endpointUserId = gemsDetail.EndpointUserId;
                    businessRole = gemsDetail.Entitlement;
                }
            }

            if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                //UserName = ConstructUserName(gemsDetail.AdHocGEMSSharedAccount.AccountName);
                isExecutedSuccessfully = CreateAccount(fields, user, businessRole, gemsDetail, adGroups);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = AmendentAdd(user, endpointUserId, businessRole, adGroups, fields);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, endpointUserId, businessRole, adGroups, fields);
            }

            return isExecutedSuccessfully;
        }
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {

            try
            {
                var gemsUsers = GEMSUserInfo.GetActiveAccounts(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                foreach (GEMSUserInfo gmUsr in gemsUsers)
                {
                    TakeOUSnapshot(user.Id, gmUsr, EmployeeEvents.Resignation, (EndpointType)Endpoint.EndpointSystemTypeId.Value, EntitlementType.Role);
                    bool isSuccess = DeleteGEMSUserAccount(gmUsr, LockScenario.OffBoard);

                    if (!isSuccess)
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserDeletionFailed, gmUsr.USER_NAME, gmUsr.EndPointSystemId), null);
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
            if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                return true;

            bool isUpdated = false;

            var gemsUsr = GEMSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
            if (gemsUsr != null)
            {
                gemsUsr.DISPLAY_NAME = user.KnownAs;
                gemsUsr.DESCRIPTION = user.KnownAs;
                gemsUsr.EMAIL = user.EmailId;
                gemsUsr.LASTUSER = MasterConfigHelper.ServiceUser;

                if (!gemsUsr.IsDummyEntry)
                {
                    isUpdated = EndpointConnector.UpdateAccount(gemsUsr);
                }
                else
                {
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    gemsUsr.Save(MasterConfigHelper.ServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} modified  in GEMS.", gemsUsr.USER_NAME), null);
                }

            }
            else
            {
                WriteProvisioningLog(LogLevelType.Info,$"No active record found for Propagation for user:{user.DisplayName}", null);
                isUpdated = true;
            }
            return isUpdated;
        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {

                GEMSUserInfo gemsUserValue = GEMSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (null == gemsUserValue)
                    return true;

                TakeOUSnapshot(user.Id, gemsUserValue, EmployeeEvents.OUChange, EndpointType.GEMS, EntitlementType.Role);

                bool isRemovedSuccessfully = false;

                if (gemsUserValue != null)
                {
                    isRemovedSuccessfully = DeleteGEMSUserAccount(gemsUserValue, LockScenario.OUChange);
                    if (!isRemovedSuccessfully)
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserDeletionFailed, gemsUserValue.USER_NAME, gemsUserValue.EndPointSystemId), null);
                    }
                }

                return isRemovedSuccessfully;

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

            bool isExecutedSuccessfully = false;

            try
            {
                GEMSUserInfo gemsUserValue = null;

                long endpointUserId = 0;
                string entitlement = string.Empty;

                //if adhoc request, get prov details from adhoc request object
                if (CurrentRequest.AdHocAccessRequest != null)
                {
                    var sapDetail = (AdHocGEMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                    if (sapDetail != null)
                    {
                        //Entitlement field holds tech role/group for attestation remediation 
                        entitlement = sapDetail.Entitlement;
                        endpointUserId = sapDetail.EndpointUserId;
                    }
                }

                if (endpointUserId != 0)
                {
                    gemsUserValue = GEMSUserInfo.GetById(endpointUserId);
                }

                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = LockUser(gemsUserValue, user, LockScenario.AttestationRemediation);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    gemsUserValue.Roles = entitlement.Split(',').ToList();
                    isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(gemsUserValue, EntitlementType.Role);
                    GEMSUserInfo.DeleteGEMSUserRoleMap(gemsUserValue, entitlement);
                }

                if (isExecutedSuccessfully)
                {
                    ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, CurrentRequest, entitlement, gemsUserValue.AccountName);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", CurrentRequest.Id), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }
        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {

            try
            {
                GEMSUserInfo gemsUserValue;
                string gemsUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

                AdHocGEMSRequest gemsDetail = null;
                string businessRole = string.Empty;
                bool isGEMSAcntCreated = false;

                gemsDetail = (AdHocGEMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                string gemsFormRoles = gemsDetail.Entitlement;
                WriteProvisioningLog(LogLevelType.Info, $"Snapshot Roles:{gemsFormRoles}");
                gemsUserValue = new GEMSUserInfo();

                #region Set GEMS User object Values

                foreach (var gData in fields)
                {
                    gemsUserValue = setUserInfoByAtrbValue(gemsUserValue, user, gData) as GEMSUserInfo;
                }

                if (string.IsNullOrEmpty(gemsUserValue.DESCRIPTION))
                    gemsUserValue.DESCRIPTION = gemsUserValue.DISPLAY_NAME;

                gemsUserValue.EndPointSystemId = Endpoint.Id;
                gemsUserValue.Password = "Your Windows Logon Password";
                gemsUserValue.LASTUSER = MasterConfigHelper.ServiceUser;

                gemsUserValue.USER_NAME = user.UserName;
                gemsUserValue.IsDummyEntry = false;
                
                var adEP = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adsUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(gemsUserValue.USER_NAME, adEP.Id);
                string adGroups = string.Empty;
                var adGrups = GetADGroupsFromTechnicalRole(user, adGroups, gemsDetail);
                if (!string.IsNullOrWhiteSpace(adGrups))
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Gems: Check for AD Account {gemsUserValue.USER_NAME}");
                    //Add ADGroups from the Matrix
                    if (adsUserValue == null || adsUserValue.IsAccountDisabled)
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"Could not find AD AccountName {gemsUserValue.USER_NAME}/ Account Locked");
                        return false;
                    }
                }
                //var adsUserValue = ADSUserInfo.GetBysAMAccountName(user.UserName);
                if (adsUserValue != null)
                {
                    gemsUserValue.USER_NAME = adsUserValue.sAMAccountName;
                }

                //GEMS Roles
                List<string> gemsRoles = new List<string>();
                string[] rolesAry = !string.IsNullOrEmpty(gemsFormRoles) ? gemsFormRoles.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string tRole in rolesAry)
                    {
                        if (tRole.Trim() != string.Empty)
                            gemsRoles.Add(tRole);
                    }
                }
                gemsUserValue.Roles = gemsRoles.Distinct().ToList();

                if (string.IsNullOrEmpty(gemsUserValue.USER_NAME))
                {
                    return false;
                }

                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidTo = getGEMSDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    gemsUserValue.UserValidTo = gemsUserDefaultValidTo;
                }

                #endregion
                //long maxUserId = GEMSUserInfo.GetMaxUserId();
                var user_id = GenerateGemsUserId();
                while (EndpointConnector.CheckEndPointAccountExists($"{user_id}"))
                {
                    WriteProvisioningLog(LogLevelType.Warning, $"Gems UserId already exists: {user_id}. Generate new Id");
                    user_id = user_id + 1;
                }
                if (!gemsUserValue.IsDummyEntry)
                {
                    gemsUserValue.UserId = user_id;
                    isGEMSAcntCreated = EndpointConnector.CreateAccount(gemsUserValue, fields);
                    WriteProvisioningLog(LogLevelType.Info, $"Create Gems Account:{gemsUserValue.USER_NAME}");
                }
                else
                {
                    gemsUserValue.UserId = user_id;
                    isGEMSAcntCreated = true;
                }

                if (isGEMSAcntCreated)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Gems Account:{gemsUserValue.USER_NAME} created successfully");
                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, gemsUserValue, user, ApplicationType.GEMS);
                    addNewUserintoGEMSUserInfo(gemsUserValue);

                    AddGEMSUser(gemsUserValue);

                    if (!gemsUserValue.IsDummyEntry)
                    {
                        EndpointConnector.AddEntitlement(gemsUserValue, EntitlementType.Role);
                        GEMSUserInfo.SaveUserRoles(gemsUserValue);
                    }
                    if (isGEMSAcntCreated)
                    {
                        CreateUserBusinessRoleMapping(gemsUserValue, gemsDetail.Entitlement);
                    }
                    if (isGEMSAcntCreated && !string.IsNullOrWhiteSpace(adGrups))
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"Gems Account:{gemsUserValue.USER_NAME} Add AD groups:{adGrups}");
                        adsUserValue.Groups = adGrups.Split(',').Distinct().ToList();

                        var adsConnector = GetADConnectorFromAdUser(adsUserValue, ref adEP);
                        if (adsConnector != null)
                        {
                            RemoveAlreadyAddedADGroups(ref adsUserValue);
                            adsConnector.AddEntitlement(adsUserValue, EntitlementType.Group);
                            ADSUserInfo.SaveUserGroup(adsUserValue);
                            // CreateUserBusinessRoleMapping(adsUserValue, adGrups);
                        }
                    }
                    if (CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreated, gemsUserValue.USER_NAME, Endpoint.Id), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreated, gemsUserValue.USER_NAME, Endpoint.Id), null);
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreationFailed, gemsUserValue.USER_NAME, Endpoint.Id), null);
                    return false;
                }
                
                return isGEMSAcntCreated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                return false;
            }
        }
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string businessRoles, AdHocGEMSRequest gemsDetail, string adGrups)
        {
            try
            {
                string gemsUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 


                var gemsUserValue = GEMSUserInfo.GetLockedAccountByName(UserName, user.Id, this.Endpoint.Id, this.CurrentRequest.AccountType);

                #region Set GEMS User object Values
                if (gemsUserValue == null)
                {
                    gemsUserValue = new GEMSUserInfo();
                }

                foreach (ProvisioningParam gData in fields)
                {
                    gemsUserValue = setUserInfoByAtrbValue(gemsUserValue, user, gData) as GEMSUserInfo;
                }

                gemsUserValue.USER_NAME = UserName;
                if (CurrentRequest.AccountType == AccountType.Shared || CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Secondary)
                {
                    gemsUserValue.DESCRIPTION = gemsDetail.AdHocGEMSSharedAccount.Description;
                    gemsUserValue.DISPLAY_NAME = gemsDetail.AdHocGEMSSharedAccount.DisplayName;
                    gemsUserValue.EMAIL = gemsDetail.AdHocGEMSSharedAccount.EmailAddress;
                    gemsUserValue.Password = "";
                }
                else if (CurrentRequest.AccountType == AccountType.User)
                {
                    gemsUserValue.Password = "Your Windows Logon Password";
                    if (string.IsNullOrWhiteSpace(gemsUserValue.DESCRIPTION))
                    {
                        gemsUserValue.DESCRIPTION = gemsUserValue.DISPLAY_NAME;
                    }
                }
                gemsUserValue.DATEMODIFIED = DateTime.Now;
                gemsUserValue.LASTUSER = MasterConfigHelper.ServiceUser;
                gemsUserValue.EndPointSystemId = Endpoint.Id;

                //Is Dummy only for User Account
                if (CurrentRequest.AccountType == AccountType.User)
                {
                    if (string.IsNullOrWhiteSpace(fields["Roles"].Value))
                    {
                        gemsUserValue.IsDummyEntry = true;
                        gemsUserValue.USER_NAME = ConstructUserName(user);
                    }
                    else
                    {
                        gemsUserValue.IsDummyEntry = false;
                    }
                }

                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidTo = getGEMSDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    gemsUserValue.UserValidTo = gemsUserDefaultValidTo;
                }

                //long maxUserId = GEMSUserInfo.GetMaxUserId();
               

                if (string.IsNullOrWhiteSpace(gemsUserValue.USER_NAME))
                {
                    WriteProvisioningLog(LogLevelType.Warning, $"Gems AccountName empty for the User: {user.UserName}");
                    return false;
                }
                #endregion
                if (!string.IsNullOrEmpty(adGrups))
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Gems: Check for AD Account {gemsUserValue.USER_NAME}");
                    var adEP = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    //Add ADGroups from the Matrix
                    var adsUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(gemsUserValue.USER_NAME, adEP.Id);
                    if (adsUserValue == null || adsUserValue.IsAccountDisabled)
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"Could not find AD AccountName {gemsUserValue.USER_NAME}/ Account Locked");
                        return false;
                    }
                }
                
                var user_id = GenerateGemsUserId();
                while (EndpointConnector.CheckEndPointAccountExists($"{user_id}"))
                {
                    WriteProvisioningLog(LogLevelType.Warning, $"Gems UserId already exists: {user_id}. Generate new Id");
                    user_id = user_id + 1;
                }
                bool isGEMSAcntCreated = false;
                if (!gemsUserValue.IsDummyEntry)
                {
                    gemsUserValue.UserId = user_id;
                    WriteProvisioningLog(LogLevelType.Warning, $"Gems UserId: {gemsUserValue.UserId} for account:{gemsUserValue.USER_NAME}");
                    WriteProvisioningLog(LogLevelType.Info, $"CreateAccount {gemsUserValue.USER_NAME} started");
                    isGEMSAcntCreated = EndpointConnector.CreateAccount(gemsUserValue, fields);
                }
                else
                {
                    gemsUserValue.UserId = user_id;
                    isGEMSAcntCreated = true;
                }
                
                if (isGEMSAcntCreated)
                {
                    AddGEMSUser(gemsUserValue);

                    //GEMS Roles
                    List<string> gemsRolesList = new List<string>();
                    gemsUserValue.Roles = new List<string>();
                    string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                    if (rolesAry != null)
                    {
                        foreach (string tRole in rolesAry)
                        {
                            if (tRole.Trim() != string.Empty)
                                gemsRolesList.Add(tRole);
                        }
                    }
                    gemsUserValue.Roles = gemsRolesList.Distinct().ToList();

                    if (!gemsUserValue.IsDummyEntry)
                    {
                        RemoveAlreadyAddedGEMSRoles(ref gemsUserValue);
                        EndpointConnector.AddEntitlement(gemsUserValue, EntitlementType.Role);
                        GEMSUserInfo.SaveUserRoles(gemsUserValue);
                    }

                    CreateUserBusinessRoleMapping(gemsUserValue, businessRoles);

                    if (CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreated, gemsUserValue.USER_NAME, Endpoint.Id), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreated, gemsUserValue.USER_NAME, Endpoint.Id), null);
                    }

                    WriteProvisioningLog(LogLevelType.DebugStep, $"Started GEMS AD Provisioning for user {gemsUserValue.USER_NAME}");

                    if (!string.IsNullOrEmpty(adGrups))
                    {
                        var adEP = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        //Add ADGroups from the Matrix
                        ADSUserInfo adsUserValue = null;
                        if(this.CurrentRequest.AccountType == AccountType.Secondary || this.CurrentRequest.AccountType == AccountType.User)
                            adsUserValue = ADSUserInfo.GetPrimaryByDIUserIdAccountType(user.Id, gemsUserValue.USER_NAME, adEP.Id,this.CurrentRequest.AccountType);
                        else
                        {
                            var account = AccountReconcilation.GetByAccountNameByAccountType(gemsUserValue.USER_NAME,adEP.Id,CurrentRequest.AccountType);
                            adsUserValue = ADSUserInfo.GetById(account.EndpointUserId);
                        }
                        EndPoint adEndpt = null;
                        if (adsUserValue != null)
                        {
                            gemsUserValue.USER_NAME = adsUserValue.sAMAccountName;

                            List<string> adgrps = new List<string>();
                            string[] grpsAry = !string.IsNullOrEmpty(adGrups) ? adGrups.Split(',') : null;
                            if (grpsAry != null)
                            {
                                foreach (string tgrp in grpsAry)
                                {
                                    if (tgrp.Trim() != string.Empty)
                                        adgrps.Add(tgrp);
                                }
                            }
                            adsUserValue.Groups = adgrps.Distinct().ToList();

                            var adsConnector = GetADConnectorFromAdUser(adsUserValue, ref adEndpt);
                            if (adsConnector != null)
                            {
                                RemoveAlreadyAddedADGroups(ref adsUserValue);
                                adsConnector.AddEntitlement(adsUserValue, EntitlementType.Group);
                                ADSUserInfo.SaveUserGroup(adsUserValue);
                                // CreateUserBusinessRoleMapping(adsUserValue, adGrups);
                            }
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserCreationFailed, gemsUserValue.USER_NAME, Endpoint.DisplayName));
                            return false;
                        }
                    }

                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, gemsUserValue, user, ApplicationType.GEMS);

                    return isGEMSAcntCreated;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }

            return false;
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
        bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            try
            {
                var gemsUser = GEMSUserInfo.GetById(endpointUserId);
                bool isSuccess = DeleteGEMSUserAccount(gemsUser, LockScenario.AdhocRequest);

                if (isSuccess)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, gemsUser.USER_NAME);
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the GEMS account. {0}", ex.Message), ex);
                return false;
            }
        }
        bool AmendentAdd(DomainModel.User user, long endpointUserId, string businessroles, string adGroups, ProvisioningParamCollection fields)
        {
            try
            {
                string gemsUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

                var gemsUserValue = GEMSUserInfo.GetById(endpointUserId);
                if (gemsUserValue == null)
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("User not found for the GEMS account. AccessRequest {0}", this.CurrentRequest.Id));
                    return false;
                }
                #region fillData
                //GEMS User Roles
                List<string> gemsRoles = new List<string>();
                string[] rolesAry = !string.IsNullOrWhiteSpace(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string tRole in rolesAry)
                    {
                        if (!string.IsNullOrWhiteSpace(tRole))
                        {
                            gemsRoles.Add(tRole);
                        }
                    }
                }
                gemsUserValue.Roles = gemsRoles.Distinct().ToList();
                var adEP = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                //GEMS ADS User Groups
                var adsUserValue = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(gemsUserValue.USER_NAME, adEP.Id);
                if (adsUserValue != null)
                {
                    adsUserValue.Groups = new List<string>();
                    List<string> adgrps = new List<string>();
                    string[] grpsAry = !string.IsNullOrWhiteSpace(adGroups) ? adGroups.Split(',') : null;
                    if (grpsAry != null)
                    {
                        foreach (string tgrp in grpsAry)
                        {
                            if(!string.IsNullOrWhiteSpace(tgrp))
                                adgrps.Add(tgrp.Trim());
                        }
                    }
                    adsUserValue.Groups = adgrps.Distinct().ToList();
                }

                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidTo = getGEMSDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    gemsUserValue.UserValidTo = gemsUserDefaultValidTo;
                }
                #endregion


                bool isGEMSroleAdded = false;

                var isAnyRoleAlreadyExists = RemoveAlreadyAddedGEMSRoles(ref gemsUserValue);

                if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                {
                    if (gemsUserValue.IsDummyEntry)
                    {
                        EndpointConnector.CreateAccount(gemsUserValue, fields);
                        gemsUserValue.IsDummyEntry = false;
                        gemsUserValue.LastModifiedBy = "System";
                        gemsUserValue.LastModifiedDate = DateTime.Now;
                        gemsUserValue.Save("System");
                    }
                    if (gemsUserValue.Roles.Count() > 0)
                    {
                        isGEMSroleAdded = EndpointConnector.AddEntitlement(gemsUserValue, EntitlementType.Role);
                        WriteProvisioningLog(LogLevelType.Info, $"AddEntitlement - AccountName:{gemsUserValue?.AccountName} IsSuccessful:{isGEMSroleAdded}");
                    }
                    else
                    {
                        if(isAnyRoleAlreadyExists)
                            isGEMSroleAdded = isAnyRoleAlreadyExists;
                        WriteProvisioningLog(LogLevelType.Info, $"AddEntitlement roles not found/already exists AccountName:{gemsUserValue?.AccountName}");
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Info, $"AddEntitlement roles:{fields["Roles"].Value} not found");
                    isGEMSroleAdded = false;
                }


                string roleListStr = getGroupListAsString(gemsUserValue.Roles);
                if (isGEMSroleAdded)
                {
                    if (!string.IsNullOrWhiteSpace(fields["Roles"].Value))
                    {
                        GEMSUserInfo.SaveUserRoles(gemsUserValue);
                    }
                    CreateUserBusinessRoleMapping(gemsUserValue, businessroles);
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, businessroles, gemsUserValue.USER_NAME);
                    if (CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserRoleAssigned, gemsUserValue.USER_NAME, roleListStr, Endpoint.DisplayName), null);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserRoleAssigned, gemsUserValue.USER_NAME, roleListStr, Endpoint.DisplayName), null);
                    }

                    EndPoint adEndpt = null;
                    ConnectorBase ADConnector = GetADConnectorFromAdUser(adsUserValue, ref adEndpt);

                    if (ADConnector != null)
                    {
                        RemoveAlreadyAddedADGroups(ref adsUserValue);
                        bool isADGroupAdded = ADConnector.AddEntitlement(adsUserValue, EntitlementType.Group);
                        if (isADGroupAdded)
                        {
                            string grpListStr = getGroupListAsString(adsUserValue.Groups);
                            ADSUserInfo.SaveUserGroup(adsUserValue);
                            //CreateUserBusinessRoleMapping(adsUserValue, adGroups);

                            if (CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adsUserValue.sAMAccountName, grpListStr, adEndpt.Id), null);
                            }
                            else
                            {
                                WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adsUserValue.sAMAccountName, grpListStr, adEndpt.Id), null);
                            }
                        }
                    }
                }

                return isGEMSroleAdded;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while Roles to the GEMS Account. {0}", ex.Message), ex);
                return false;
            }
        }
        bool AmendentRemove(DomainModel.User user, long endpointUserId, string businessroles, string adGroups, ProvisioningParamCollection fields)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                //DateTime maxDate = new DateTime(2999, 12, 31);
                string gemsUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31"; // maxDate.ToString("yyyy.MM.dd"); // 

                var gemsUserValue = GEMSUserInfo.GetById(endpointUserId);

                if (gemsUserValue == null)
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Could not able to find GEMS Endpoint User for Access RequestId - {0}", CurrentRequest.Id), null);
                    return false;
                }

                //Valid From and Valid To
                if (CurrentRequest.ActivationDate != null && CurrentRequest.ActivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat((DateTime)CurrentRequest.ActivationDate);
                }
                else
                {
                    gemsUserValue.UserValidFrom = getGEMSDateFormat(DateTime.Now);
                }

                if (CurrentRequest.DeactivationDate != null && CurrentRequest.DeactivationDate != DateTime.MinValue)
                {
                    gemsUserValue.UserValidTo = getGEMSDateFormat((DateTime)CurrentRequest.DeactivationDate);
                }
                else
                {
                    gemsUserValue.UserValidTo = gemsUserDefaultValidTo;
                }

                #region GEMSRoles AmendmentReomve

                List<string> gemsRolelist = new List<string>();
                string[] rolesAry = !string.IsNullOrEmpty(fields["Roles"].Value) ? fields["Roles"].Value.Split(',') : null;
                if (rolesAry != null)
                {
                    foreach (string trole in rolesAry)
                    {
                        if (!string.IsNullOrEmpty(trole))
                            gemsRolelist.Add(trole.Trim());
                    }
                }
                gemsUserValue.Roles = gemsRolelist.Distinct().ToList();

                #region ADUser
                var adsUserValue = ADSUserInfo.GetBysAMAccountName(gemsUserValue.USER_NAME);
                if (adsUserValue != null)
                {
                    List<string> adgrps = new List<string>();
                    string[] grpsAry = !string.IsNullOrEmpty(adGroups) ? adGroups.Split(',') : null;
                    if (grpsAry != null)
                    {
                        foreach (string tgrp in grpsAry)
                        {
                            if (tgrp.Trim() != string.Empty)
                                adgrps.Add(tgrp);
                        }
                    }
                    adsUserValue.Groups = adgrps.Distinct().ToList();
                }
                #endregion

                RemoveNonAccessedGEMSRoles(gemsUserValue);
                CheckBusinessRoleUsedForOtherRolesInGEMS(gemsUserValue, businessroles, adsUserValue);

                string roleListStr = getGroupListAsString(gemsUserValue.Roles);

                if (!gemsUserValue.IsDummyEntry)
                {
                    isRemovedSuccessfully = EndpointConnector.RemoveEntitlement(gemsUserValue, EntitlementType.Role);

                    if (isRemovedSuccessfully)
                    {
                        var endptUsrInfo = gemsUserValue;
                        GEMSUserInfo.DeleteGEMSUserRoleMap(gemsUserValue, businessroles);
                        if (CurrentRequest.AdHocAccessRequestId.HasValue && gemsUserValue.Roles.Count() > 0)
                        {
                            WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserRemovedFromRoles, gemsUserValue.USER_NAME, roleListStr, gemsUserValue.EndPointSystemId), null);
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserRemoveFromRoleFailed, gemsUserValue.USER_NAME, roleListStr, gemsUserValue.EndPointSystemId), null);
                    }
                }

                #endregion

                #region ADGroups AmendmentReomve               
                EndPoint adEndpt = null;
                if (adsUserValue != null)
                {
                    var adsConnector = GetADConnectorFromAdUser(adsUserValue, ref adEndpt);
                    if (adsConnector != null)
                    {
                        RemoveNonAccessedADGroups(ref adsUserValue);
                        string grpList = getGroupListAsString(adsUserValue.Groups);

                        CheckBusinessRoleUsedForOtherRolesInAD(adsUserValue, businessroles, gemsUserValue);
                        //DeleteADSUserBusinessRoleMapping(adsUserValue, businessroles);

                        if (adsUserValue.Groups.Count() > 0)
                        {
                            isRemovedSuccessfully = adsConnector.RemoveEntitlement(adsUserValue, EntitlementType.Group);
                            if (isRemovedSuccessfully)
                            {
                                //endptUsrInfo = gemsUserValue;
                                ADSUserInfo.DeleteADUserGroupMap(adsUserValue);
                                if (CurrentRequest.AdHocAccessRequestId.HasValue)
                                {
                                    WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adsUserValue.sAMAccountName, grpList, adsUserValue.EndPointSystemId), null);
                                }
                                else
                                {
                                    WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adsUserValue.sAMAccountName, grpList, adsUserValue.EndPointSystemId), null);
                                }
                            }
                            else
                            {
                                WriteProvisioningLog(adEndpt.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adsUserValue.sAMAccountName, grpList, adsUserValue.EndPointSystemId), null);
                            }
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format("No valid GEMS-AD Groups available to remove for the user - {0} ", gemsUserValue.AccountName), null);
                        isRemovedSuccessfully = true;
                    }


                }
                #endregion


                //Remove BusinessRoles

                if (isRemovedSuccessfully)
                {
                    DeleteUserBusinessRoleMapping(gemsUserValue, businessroles);
                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, businessroles, gemsUserValue.AccountName);
                }

                return true;

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }
        protected bool DeleteGEMSUserAccount(GEMSUserInfo gemsUsr, LockScenario lockScenario)
        {
            bool isusrDeleted = true;

            if (gemsUsr != null)
            {
                isusrDeleted = false;

                if (!gemsUsr.IsDummyEntry)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Gems UserId:{gemsUsr.UserId} UserName:{gemsUsr.USER_NAME}");
                    var userwithRoles = GEMSUserInfo.GetByIdIncludeRoles(gemsUsr.Id);
                    if (userwithRoles != null)
                        gemsUsr.Roles = userwithRoles.GEMSRoles.Select(item => item.Name).ToList();
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, "RemoveEntitlement");
                    EndpointConnector.RemoveEntitlement(gemsUsr, EntitlementType.Role);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, "DeleteAllUserBusinessRoleMapping");
                    DeleteAllUserBusinessRoleMapping(gemsUsr);
                    isusrDeleted = EndpointConnector.DeleteAccount((IEndpointUserInfo)gemsUsr);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"DeleteAccount completed.Status:{isusrDeleted}");
                }
                else
                {
                    isusrDeleted = true;
                }

                if (isusrDeleted)
                {
                    GEMSUserInfo.RemoveGEMSUser(new List<GEMSUserInfo>() { gemsUsr });
                    DeleteGEMSADUserGroups(gemsUsr);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Disabled in GEMS.", gemsUsr.USER_NAME), null);
                }
            }

            return isusrDeleted;
        }
        protected bool DeleteGEMSADUserGroups(GEMSUserInfo gemsUsr)
        {
            bool isusrDeleted = true;

            if (gemsUsr != null)
            {
                isusrDeleted = false;
                ADSUserInfo adsUsrinfo = ADSUserInfo.GetBysAMAccountName(gemsUsr.USER_NAME);
                EndPoint adEndpt = null;
                var adConnector = GetADConnectorFromAdUser(adsUsrinfo, ref adEndpt);

                if (adsUsrinfo != null && adConnector != null)
                {
                    adsUsrinfo.Groups = GEMSRoleMatrix.GetGEMSGroupsFromADGroups(adsUsrinfo);
                    if (adsUsrinfo.Groups == null || adsUsrinfo.Groups.Count == 0)
                    {
                        WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, "No AD Groups to delete.");
                        return true;
                    }
                    string grpListStr = getGroupListAsString(adsUsrinfo.Groups);

                    isusrDeleted = adConnector.RemoveEntitlement(adsUsrinfo, EntitlementType.Group);

                    if (isusrDeleted)
                    {
                        ADSUserInfo.DeleteADUserGroupMap(adsUsrinfo);

                        if (CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adsUsrinfo.sAMAccountName, grpListStr, adsUsrinfo.EndPointSystemId), null);
                        }
                        else
                        {
                            WriteProvisioningLog(adEndpt.Id, LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemovedFromGroup, adsUsrinfo.sAMAccountName, grpListStr, adsUsrinfo.EndPointSystemId), null);
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(adEndpt.Id, LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserRemoveFromGroupFailed, adsUsrinfo.sAMAccountName, grpListStr, adsUsrinfo.EndPointSystemId), null);
                    }
                }
            }

            return isusrDeleted;
        }
        void AddGEMSUser(GEMSUserInfo gemsUser)
        {
            string currentUser = MasterConfigHelper.ServiceUser;
            gemsUser.Save(currentUser);

            CreateUserInReconTable(gemsUser, currentUser);
        }
        private string GetRolesFromAdhocRequest(DomainModel.User usr, string existingRoles, AdHocGEMSRequest gemsDetail)
        {
            GEMSRoleMatrix rmFacade = new GEMSRoleMatrix();

            string ItRoles = String.Empty;
            List<string> TechnicalRoles = new List<string>();
            WriteProvisioningLog(LogLevelType.Info, $"BussinessRoles :{gemsDetail.Entitlement}");
            var bussinessRoles = UtilityMethods.GetListFromString(gemsDetail.Entitlement);
            foreach (string bussinessRole in bussinessRoles)
            {
                var tRoles = GEMSRoleMatrix.GetGEMSTechnicalRolesFromBusinessRole(bussinessRole);
                if(tRoles == null || tRoles.Count == 0)
                {
                    throw new Exception($"No Technical role found for Businessrole:{bussinessRole}. Possible reason: Businessrole does not exists in the matrix or renamed.");
                }
                TechnicalRoles.AddRange(tRoles);
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
        private string GetADGroupsFromBusinessRole(DomainModel.User usr, string existingGroups, AdHocGEMSRequest gemsDetail)
        {
            GEMSRoleMatrix roleMatrx = new GEMSRoleMatrix();

            string groups = String.Empty;
            List<string> adGroups = new List<string>();

            var bussinessRoles = UtilityMethods.GetListFromString(gemsDetail.Entitlement);

            foreach (string bussinessRole in bussinessRoles)
            {
                if (!string.IsNullOrWhiteSpace(bussinessRole))
                {
                    var adGrps = GEMSRoleMatrix.GetGEMSADGroupsFromBusinessRole(bussinessRole);
                    if(adGrps != null && adGrps.Count > 0)
                        adGroups.AddRange(adGrps);
                }
            }
            var ep = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);

            foreach (string grp in adGroups)
            {
                if (!string.IsNullOrWhiteSpace(grp))
                {
                    if (string.IsNullOrWhiteSpace(groups))
                        groups = grp;
                    else
                        groups = groups + "," + grp;
                }
            }

            if (string.IsNullOrWhiteSpace(existingGroups))
            {
                return groups;
            }
            else
            {
                return existingGroups + "," + groups;
            }
        }
        private string GetADGroupsFromTechnicalRole(DomainModel.User usr, string existingGroups, AdHocGEMSRequest gemsDetail)
        {
            GEMSRoleMatrix roleMatrx = new GEMSRoleMatrix();

            string groups = String.Empty;
            List<string> adGroups = new List<string>();

            var technicalRoles = UtilityMethods.GetListFromString(gemsDetail.Entitlement);

            foreach (string technicalRole in technicalRoles)
            {
                if (!string.IsNullOrWhiteSpace(technicalRole))
                {
                    var adGrps = GEMSRoleMatrix.GetADGroupsFromTechnicalRole(technicalRole);
                    if (adGrps != null && adGrps.Count > 0)
                        adGroups.AddRange(adGrps);
                }
            }

            foreach (string grp in adGroups)
            {
                if (string.IsNullOrWhiteSpace(groups))
                    groups = grp;
                else
                    groups = groups + "," + grp;
            }

            if (string.IsNullOrWhiteSpace(existingGroups))
            {
                return groups;
            }
            else
            {
                return existingGroups + "," + groups;
            }
        }
        private string getGEMSDateFormat(DateTime tmpDate)
        {
            string dateStr = string.Empty;
            try
            {
                string yearStr = tmpDate.Year.ToString();
                string monthStr = tmpDate.Month < 10 ? "0" + tmpDate.Month.ToString() : tmpDate.Month.ToString();
                string dayStr = tmpDate.Day < 10 ? "0" + tmpDate.Day.ToString() : tmpDate.Day.ToString();

                dateStr = string.Format(ProvisioningConstants.GEMSDateFormater, yearStr, monthStr, dayStr);
            }
            catch
            { }

            return dateStr;
        }
        private bool RemoveAlreadyAddedGEMSRoles(ref GEMSUserInfo gemsUserValue)
        {
            bool isRoleAlreadyExists = false;
            GEMSUserInfo existingUsrRoles = GEMSUserInfo.GetByIdIncludeRoles(gemsUserValue.Id);
            if (existingUsrRoles != null)
            {
                foreach (GEMSRoleInfo role in existingUsrRoles.GEMSRoles)
                {
                    if (role != null && !string.IsNullOrEmpty(role.Name) && gemsUserValue.Roles.Contains(role.Name,StringComparer.InvariantCultureIgnoreCase))
                    {
                        isRoleAlreadyExists = true;
                        WriteProvisioningLog(LogLevelType.Info, $"Role already exists. RoleName:{role.Name}");
                        gemsUserValue.Roles.Remove(role.Name);
                    }
                }
            }
            return isRoleAlreadyExists;
        }
        private void addNewUserintoGEMSUserInfo(GEMSUserInfo gemsUser)
        {
            gemsUser.CreatedBy = ProvisioningConstants.ProcessName;
            gemsUser.LastModifiedBy = ProvisioningConstants.ProcessName;
            gemsUser.CreatedDate = DateTime.Now;
            gemsUser.LastModifiedDate = DateTime.Now;

            string emptyStr = string.Empty;

            foreach (PropertyInfo pInfo in gemsUser.GetType().GetProperties())
            {
                if (pInfo.GetValue(gemsUser, null) == null && pInfo.PropertyType == typeof(string))
                {
                    pInfo.SetValue(gemsUser, emptyStr, null);
                }
            }
        }
        private void updateUserMaster(DomainModel.User usr, GEMSUserInfo gemsUserValue)
        {
            usr.UserName = gemsUserValue.USER_NAME;
            // UsrFacade.UpdateUser(usr);
        }
        private ConnectorBase GetADConnectorFromAdUser(ADSUserInfo adsUserValue, ref EndPoint adEndPoint)
        {
            ConnectorBase adsConnector = null;
            adEndPoint = null;
            if (adsUserValue != null)
            {
                adEndPoint = EndPoint.GetById(adsUserValue.EndPointSystemId);
                if (adEndPoint != null)
                {
                    adsConnector = ConnectorFactory.GetConnector(adEndPoint, CurrentRequest);
                }
            }
            return adsConnector;
        }
        private void RemoveAlreadyAddedADGroups(ref ADSUserInfo adsUserValue)
        {
            var usrwithGrps = ADSUserInfo.GetByIdIncludeADSGroups(adsUserValue.Id);

            if (usrwithGrps != null && usrwithGrps.ADSGroups != null && usrwithGrps.ADSGroups.Count > 0)
            {
                foreach (ADSGroupInfo adGroup in usrwithGrps.ADSGroups)
                {
                    if (adsUserValue.Groups.Contains(adGroup.cn,StringComparer.InvariantCultureIgnoreCase))
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"ADGroup already exists. GroupName:{adGroup.cn}");
                        adsUserValue.Groups.Remove(adGroup.cn);
                    }
                }
            }
        }
        private void RemoveNonAccessedADGroups(ref ADSUserInfo adsUserValue)
        {
            var usrWithGroups = ADSUserInfo.GetByIdIncludeADSGroups(adsUserValue.Id);
            List<string> existingGroupList = new List<string>();
            if (usrWithGroups != null)
            {
                foreach (ADSGroupInfo adGroup in usrWithGroups.ADSGroups)
                    existingGroupList.Add(adGroup.cn);
            }
            foreach (string groupName in adsUserValue.Groups.ToList())
            {
                if (!existingGroupList.Contains(groupName))
                    adsUserValue.Groups.Remove(groupName);
            }
        }
        private void CheckBusinessRoleUsedForOtherRolesInGEMS(GEMSUserInfo gemsUserValue, string businessRole, ADSUserInfo adsUserValue)
        {
            List<string> tobeReomvedTechRoles = new List<string>();
            List<string> tobeReomvedadGrps = new List<string>();
            GEMSRoleMatrix.CheckBusinessRolesUsedInOtherBusinessRoles(businessRole, ref tobeReomvedTechRoles, ref tobeReomvedadGrps);
            if (gemsUserValue.Roles.Count() > 0)
            {
                tobeReomvedTechRoles.ForEach(item =>
                {
                    gemsUserValue.Roles.Remove(item);
                });
            }

            if (adsUserValue != null && adsUserValue.Groups.Count() > 0)
            {
                tobeReomvedadGrps.ForEach(grp =>
                {
                    adsUserValue.Groups.Remove(grp);
                });
            }

            //IList<string> existingbusroleList = UserBusinessRoleMapping.GetAllBussRoleMappingRecordsForUser(gemsUserValue.Id, gemsUserValue.EndPointSystemId);
            //IList<string> busroleListtoremove = businessRole.Split(',').ToList();
            //foreach (string remrole in busroleListtoremove)
            //{
            //    if (existingbusroleList.Contains(remrole))
            //    {
            //        IList<string> removetechroleList = GEMSRoleMatrix.GetGEMSTechnicalRolesFromBusinessRole(remrole);
            //        foreach (string existbusrole in existingbusroleList)
            //        {
            //            if (!busroleListtoremove.Contains(existbusrole))
            //            {
            //                IList<string> existtechroleList = GEMSRoleMatrix.GetGEMSTechnicalRolesFromBusinessRole(existbusrole);
            //                foreach (string removetechrole in removetechroleList)
            //                {
            //                    if (existtechroleList.Contains(removetechrole))
            //                        gemsUserValue.Roles.Remove(removetechrole);
            //                }
            //            }
            //        }
            //    }
            //}
        }
        private void DeleteUserBusinessRoleMapping(IEndpointUserInfo _userInfo, string businessRole)
        {
            GEMSUserInfo gemsUserValue = (GEMSUserInfo)_userInfo;
            if (!string.IsNullOrEmpty(businessRole))
                RemoveUserBusinessRoleMapping(gemsUserValue, businessRole);
        }
        private void DeleteAllUserBusinessRoleMapping(IEndpointUserInfo _userInfo)
        {
            GEMSUserInfo gemsUserValue = (GEMSUserInfo)_userInfo;

            UserBusinessRoleMapping.RemoveAllUserBussRoleMappingRecord(gemsUserValue.Id, this.Endpoint.Id, this.CurrentRequest.UserId);
        }
        private void RemoveNonAccessedGEMSRoles(GEMSUserInfo gemsUserValue)
        {
            GEMSUserInfo existingUsrRoles = GEMSUserInfo.GetByIdIncludeRoles(gemsUserValue.Id);
            List<string> existingRoleList = new List<string>();
            if (existingUsrRoles != null && existingUsrRoles.GEMSRoles.Count() > 0)
            {
                foreach (GEMSRoleInfo role in existingUsrRoles.GEMSRoles)
                    existingRoleList.Add(role.Name);
            }
            foreach (string roleName in gemsUserValue.Roles.ToList())
            {
                if (!existingRoleList.Contains(roleName))
                    gemsUserValue.Roles.Remove(roleName);
            }
        }
        private void CheckBusinessRoleUsedForOtherRolesInAD(ADSUserInfo adsUserValue, string businessRole, GEMSUserInfo gemsUserValue)
        {
            IList<string> existingbusroleList = UserBusinessRoleMapping.GetAllBussRoleMappingRecordsForUser(gemsUserValue.Id, gemsUserValue.EndPointSystemId);
            IList<string> busroleListtoremove = businessRole.Split(',').ToList();
            foreach (string remrole in busroleListtoremove)
            {
                IList<string> removeadgroupList = GEMSRoleMatrix.GetGEMSADGroupsFromBusinessRole(remrole);
                if (existingbusroleList.Contains(remrole))
                {
                    foreach (string existbusrole in existingbusroleList)
                    {
                        if (!busroleListtoremove.Contains(existbusrole))
                        {
                            IList<string> existtechroleList = GEMSRoleMatrix.GetGEMSADGroupsFromBusinessRole(existbusrole);
                            foreach (string removeadgroup in removeadgroupList)
                            {
                                if (existtechroleList.Contains(removeadgroup))
                                    adsUserValue.Groups.Remove(removeadgroup);
                            }
                        }
                    }
                }
            }
        }
        private void DeleteADSUserBusinessRoleMapping(IEndpointUserInfo _userInfo, string businessRole)
        {
            ADSUserInfo adsUserValue = (ADSUserInfo)_userInfo;
            if (!string.IsNullOrEmpty(businessRole))
                RemoveUserBusinessRoleMapping(adsUserValue, businessRole);
        }
        private bool LockUser(GEMSUserInfo endpointUser, DomainModel.User user, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (endpointUser != null)
            {
                isusrDisabled = false;

                if (user.JoinDate != null)
                {
                    endpointUser.UserValidFrom = getGEMSDateFormat(user.JoinDate.Value);
                }
                if (user.TerminationDate != null)
                {
                    endpointUser.UserValidTo = getGEMSDateFormat(user.TerminationDate.Value);
                }
                if (!string.IsNullOrEmpty(endpointUser.UserValidFrom) && !string.IsNullOrEmpty(endpointUser.UserValidTo))
                {
                    EndpointConnector.UpdateAccount(endpointUser);
                }

                endpointUser.LASTUSER = endpointUser.USER_NAME;
                //endpointUser.DATEMODIFIED = DateTime.Now; // .ToString();
                isusrDisabled = EndpointConnector.LockAccount(endpointUser);
                if (isusrDisabled)
                {
                    endpointUser.LastModifiedBy = "System";
                    endpointUser.LastModifiedDate = DateTime.Now;
                    endpointUser.Save("System");
                    if (isusrDisabled)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("  User {0} Disabled in GEMS.", endpointUser.USER_NAME), null);
                    }
                }
            }

            return isusrDisabled;
        }
        public string ConstructUserName(string userName)
        {
            userName = CheckUserNameExists(userName);
            return userName;
        }
        /// <summary>
        /// Generate User_Id
        /// </summary>
        /// <returns></returns>
        public int GenerateGemsUserId()
        {
            var currentDate = DateTime.Now;
            var Id = $"{currentDate.Month}{currentDate.Day.ToString().PadLeft(2, '0')}{currentDate.Year.ToString().GetLastDigit(2)}{currentDate.Hour.ToString().PadLeft(2, '0')}{currentDate.Minute.ToString().PadLeft(2, '0')}";
            return int.Parse(Id);
        }
    }
}

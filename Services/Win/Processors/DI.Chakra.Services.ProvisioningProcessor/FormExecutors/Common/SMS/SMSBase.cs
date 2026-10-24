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
    public class SMSBase : FormExecutorBase
    {
        public SMSBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }
        #region override Methods
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            List<string> validatetechnicalRoles = new List<string>();
            AdHocSMSRequest smsDetail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "One Card Provisioning failed. User object is null");
                return false;
            }

            if (fields["DefaultAccessLevels"] != null && !string.IsNullOrWhiteSpace(fields["DefaultAccessLevels"].Value))
            {
                validatetechnicalRoles.AddRange(fields["DefaultAccessLevels"].Value.Split(','));
            }


            string leveldetail = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                smsDetail = (AdHocSMSRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (smsDetail != null)
                {
                    endpointUserId = smsDetail.EndpointUserId;
                    fields["DefaultAccessLevels"].Value = smsDetail.Entitlement;
                    if (!string.IsNullOrWhiteSpace(smsDetail.Entitlement))
                        validatetechnicalRoles.AddRange(smsDetail.Entitlement.Split(','));
                }
            }

            if (validatetechnicalRoles.Count > 0 && !AreEntitlementsValid(validatetechnicalRoles.ToArray()))
            {
                return false;
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, leveldetail, smsDetail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = AmendentAdd(user, endpointUserId, leveldetail, fields);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, endpointUserId, leveldetail, fields);
            }

            return isExecutedSuccessfully;
        }
        //public override bool OnBoard(ProvisioningParamCollection fields, DomainModel.User user)
        //{
        //    return true;
        //}
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var smsUser = SMSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (smsUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User Value is NULL for the Access request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                smsUser = SMSUserInfo.GetByIdIncludeAccesslevels(smsUser.Id);

                if (smsUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User Value is NULL for the Access request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                WriteProvisioningLog(LogLevelType.Info, $"OneCard OffBoard request- TakeOUSnapshot Started for {user.KnownAs}");
                this.TakeOUSnapshot(user.Id, smsUser, EmployeeEvents.Resignation, EndpointType.OneCard, EntitlementType.AccessLevel);
                WriteProvisioningLog(LogLevelType.Info, $"OneCard OffBoard request- TakeOUSnapshot completed for {user.KnownAs}");

                isRemovedSuccessfully = this.DisableUser(smsUser, LockScenario.OffBoard);
                ProvisioningEmailHelper.NotifyPassOfficeAboutOffboard(user);
                if (isRemovedSuccessfully)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserDeleted, smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserDeletionFailed, smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                }
                return isRemovedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Propagate(DomainModel.User modifiedUser, ProvisioningParamCollection fields)
        {
            bool isUpdated = true;

            try
            {
                var smsUser = SMSUserInfo.GetPrimaryByDIUserId(modifiedUser.Id, this.Endpoint.Id);
                if (smsUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{this.Endpoint?.DisplayName}: User:{modifiedUser.DisplayName} dont have any active account for propagation.", null);
                    return isUpdated;
                }
                smsUser.FIRSTNAME = modifiedUser.FirstName;
                smsUser.LASTNAME = modifiedUser.LastName;
                smsUser.MIDNAME = modifiedUser.KnownAs;
                smsUser.DisplayName = modifiedUser.DisplayName;
                smsUser.SSNO = modifiedUser.EmployeeId;

                smsUser.Addr1 = string.Format(@"{0} \ {1}", modifiedUser.OrgDepartment, modifiedUser.OrgBranch);
                smsUser.City = modifiedUser.OrgSection;
                smsUser.Zip = modifiedUser.IdentificationNo;
                smsUser.EMAIL = modifiedUser.EmailId;
                smsUser.OPHONE = modifiedUser.UserName;
                if (!string.IsNullOrWhiteSpace(modifiedUser.MobileNumber))
                {
                    if (modifiedUser.MobileNumber.Trim().Length > 8)
                        smsUser.Extension = modifiedUser.MobileNumber.Trim().Substring(modifiedUser.MobileNumber.Trim().Length - 8, 8);
                    else
                        smsUser.Extension = modifiedUser.MobileNumber.Trim();
                }

                var company = SMSCompany.GetSMSCompanyByName(modifiedUser.CompanyName);

                if (company != null)
                {
                    smsUser.DEPT = company.CompanyKey;
                }

                smsUser.LastModifiedBy = "System";
                smsUser.LastModifiedDate = DateTime.Now;

                isUpdated = EndpointConnector.UpdateAccount(smsUser);

                if (isUpdated)
                {
                    smsUser.Save("System");
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} modified in OneCard.", smsUser.MIDNAME), null);
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while UpdateAccount the OneCard account. {0}", ex.Message), ex);
                return false;
            }

        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var smsUser = SMSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (smsUser == null)
                {

                    return false;
                }

                //set new ou values to AD user object
                smsUser.FIRSTNAME = user.FirstName;
                smsUser.LASTNAME = user.LastName;
                smsUser.MIDNAME = user.KnownAs;
                smsUser.DisplayName = user.DisplayName;
                smsUser.SSNO = user.EmployeeId;
                smsUser.Addr1 = string.Format(@"{0} \ {1}", user.OrgDepartment, user.OrgBranch);
                smsUser.City = user.OrgSection;
                smsUser.Zip = user.IdentificationNo;
                smsUser.EMAIL = user.EmailId;
                smsUser.OPHONE = user.UserName;
                if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                {
                    if (user.MobileNumber.Trim().Length > 8)
                        smsUser.Extension = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                    else
                        smsUser.Extension = user.MobileNumber.Trim();
                }
                
                smsUser.LastModifiedBy = "System";
                smsUser.LastModifiedDate = DateTime.Now;

                var company = SMSCompany.GetSMSCompanyByName(user.CompanyName);
                if (company != null)
                {
                    smsUser.DEPT = company.CompanyKey;
                }

                bool isChanged = EndpointConnector.ChangeContainer(smsUser);
                smsUser.Save("System");

                //add default groups for the new org.
                if (user.FullTimeEmployee)
                {
                    string message = ProvisioningValidator.IsSMSDefaultsExists(user);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"SMSXLProvisioningMatrix is NULL for  the ReOrg request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                        ProvisioningValidator.AlertITAdmin(user, message, EmployeeEvents.ReOrganisation);

                        return false;
                    }

                    smsUser.AccessLevels = this.GetAdditionalGroupsByUser(user, smsUser.EndPointSystemId);

                    bool isAdded = EndpointConnector.AddEntitlement(smsUser, EntitlementType.AccessLevel);
                    if (isAdded)
                    {
                        this.InsertSMSUserAccessLevel(smsUser);
                    }
                }

                if (isChanged)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("ReOrg Successfull for the User \"{0}\" in OneCard System \"{1}\"  ", smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("ReOrg Failed for the User \"{0}\" in OneCard System \"{1}\"  ", smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                }
                return isChanged;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while ReOrg the OneCard account. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {

                var smsUser = SMSUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (smsUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Transfer request- OneCard User not found for User {user.KnownAs}, Request Id {this.CurrentRequest.Id}");
                    return true;
                }

                //Start Removing Existing AccessLevels

                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- Started Removing Exisitng Access Levels {user.KnownAs}");
                var levels = SMSUserInfo.GetByIdIncludeAccesslevels(smsUser.Id);
                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- TakeOUSnapshot Started for {user.KnownAs}");
                this.TakeOUSnapshot(user.Id, smsUser, EmployeeEvents.OUChange, EndpointType.OneCard, EntitlementType.AccessLevel);
                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- TakeOUSnapshot completed for {user.KnownAs}");
                var existingGroups = levels.SMSAccessLevels.Select(item => item.Name).ToList();

                //retain levels created using prov form               
                smsUser.AccessLevels = new List<string>();
                if (!string.IsNullOrEmpty(fields["DefaultAccessLevels"].Value))
                {
                    var templevels = fields["DefaultAccessLevels"].Value.Split(',').ToList();
                    foreach (string group in existingGroups)
                    {
                        if (!templevels.Contains(group))
                        {
                            smsUser.AccessLevels.Add(group);
                        }
                    }
                }
                else
                {
                    smsUser.AccessLevels = existingGroups;
                }

                EndpointConnector.RemoveEntitlement(smsUser, EntitlementType.AccessLevel);

                this.DeleteSMSUseLevelMap(smsUser, true);

                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- Completed Removing Exisitng Access Levels {user.KnownAs}");
                //End Removing Existing AccessLevels

                //Starting to update and Add default access levels


                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- Started Updating Latest Access Levels {user.KnownAs}");
                //set new ou values to AD user object
                smsUser.FIRSTNAME = user.FirstName;
                smsUser.LASTNAME = user.LastName;
                smsUser.MIDNAME = user.KnownAs;
                smsUser.DisplayName = user.DisplayName;
                smsUser.SSNO = user.EmployeeId;
                smsUser.Addr1 = string.Format(@"{0} \ {1}", user.OrgDepartment, user.OrgBranch);
                smsUser.City = user.OrgSection;
                smsUser.Zip = user.IdentificationNo;
                smsUser.EMAIL = user.EmailId;
                smsUser.OPHONE = user.UserName;
                if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                {
                    if (user.MobileNumber.Trim().Length > 8)
                        smsUser.Extension = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                    else
                        smsUser.Extension = user.MobileNumber.Trim();
                }
                var company = SMSCompany.GetSMSCompanyByName(user.CompanyName);

                if (company != null)
                {
                    smsUser.DEPT = company.CompanyKey;
                }

                smsUser.LastModifiedBy = "System";
                smsUser.LastModifiedDate = DateTime.Now;

                EndpointConnector.ChangeContainer(smsUser);

                //add default groups for the new org.
                if (user.FullTimeEmployee)
                {
                    string message = ProvisioningValidator.IsSMSDefaultsExists(user);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"SMSXLProvisioningMatrix is NULL for  the Transfer request-{this.CurrentRequest.Id} user- {user.KnownAs}");
                        ProvisioningValidator.AlertITAdmin(user, message, EmployeeEvents.OUChange);

                        return false;
                    }

                    smsUser.AccessLevels = this.GetAdditionalGroupsByUser(user, smsUser.EndPointSystemId);

                    bool isAdded = EndpointConnector.AddEntitlement(smsUser, EntitlementType.AccessLevel);
                    if (isAdded)
                    {
                        this.InsertSMSUserAccessLevel(smsUser);
                    }
                }

                smsUser.Save("System");
                WriteProvisioningLog(LogLevelType.Info, $"OneCard Transfer request- Completed Updating Latest Access Levels {user.KnownAs}");
                //Completed to update and Add default access levels
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
            bool isExecutedSuccessfully = false;

            try
            {
                SMSUserInfo smsUser = null;

                long endpointUserId = 0;
                string entitlement = string.Empty;
                //if adhoc request, get prov details from adhoc request object
                if (this.CurrentRequest.AdHocAccessRequest != null)
                {
                    var smsDetail = (AdHocSMSRequest)this.CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                    if (smsDetail != null)
                    {
                        //Entitlement field holds tech role/group for attestation remediation 
                        entitlement = smsDetail.Entitlement;
                        endpointUserId = smsDetail.EndpointUserId;
                    }
                }

                if (endpointUserId != 0)
                {
                    smsUser = SMSUserInfo.GetById(endpointUserId);
                }

                if (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = this.DisableUser(smsUser, LockScenario.AttestationRemediation);
                }
                else if (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    smsUser.AccessLevels = entitlement.Split(',').ToList();
                    isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(smsUser, EntitlementType.AccessLevel);
                    this.DeleteSMSUseLevelMap(smsUser, false);
                }

                if (isExecutedSuccessfully)
                {
                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, this.CurrentRequest, entitlement, smsUser.AccountName);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while executing access request - {0}.", this.CurrentRequest.Id), ex);
                return false;
            }

            return isExecutedSuccessfully;
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
                        //if (accountRecon.ValidToDate.HasValue == false || (accountRecon.ValidToDate.HasValue &&
                        //    (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value)))
                        {
                            var smsUserInfo = SMSUserInfo.GetById(accountRecon.EndpointUserId);

                            if (smsUserInfo != null && user.TerminationDate.HasValue)
                            {
                                smsUserInfo.UserValidTo = user.TerminationDate.Value.Date.AddDays(1).ToString("yyyyMMddHHmmss.ffffff+480");

                                smsUserInfo.LastModifiedBy = "System";
                                smsUserInfo.LastModifiedDate = DateTime.Now;

                                isUpdated = EndpointConnector.ExtendValidity(smsUserInfo);

                                if (isUpdated)
                                {
                                    smsUserInfo.Save(MasterConfigHelper.ServiceUser);
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", smsUserInfo.AccountName, oldTerminationDate, user.TerminationDate), null);
                                }
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.DebugStep, $"smsUserInfo != null:{smsUserInfo != null}/  TerminationDate HasValue:{ user.TerminationDate.HasValue} account :{accountRecon.AccountName} user:{user.KnownAs}({user.UserName} Termination Date:{user.TerminationDate})");
                            }
                        }
                        //else
                        //{
                        //    WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the account :{accountRecon.AccountName}");
                        //}
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
            bool isUpdated = true;
            try
            {
                var smsUser = SMSUserInfo.GetByIdIncludeAccesslevels(this.CurrentRequest.EventUndoRequest.EndpointUserId);
                smsUser.SMSBadgeInfos = SMSBadgeInfo.GetByBadgesByUserId(smsUser.UserId);

                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    smsUser.NewPernr = user.EmployeeId;
                    this.EnableUser(user, smsUser, fields);
                    this.ReOrg(user, fields);

                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, smsUser, user, ApplicationType.OneCard);
                    ProvisioningEmailHelper.NotifyPassOfficeAboutOnBoard(user);
                }
                else if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    this.ReOrg(user, fields);
                    var existingGroups = SMSUserInfo.GetByIdIncludeAccesslevels(smsUser.Id).SMSAccessLevels.Select(item => item.Name).ToList();

                    if (existingGroups.Count > 0)
                    {
                        smsUser.AccessLevels = existingGroups;

                        EndpointConnector.RemoveEntitlement(smsUser, EntitlementType.AccessLevel);
                        this.DeleteSMSUseLevelMap(smsUser, true);
                    }
                }

                string groups = this.CurrentRequest.EventUndoRequest.TechnicalGroups;

                if (!string.IsNullOrEmpty(groups))
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Snapshot roles: {groups}");
                   smsUser.AccessLevels = UtilityMethods.GetListFromString(groups);
                }
                var defaultRoles = fields["DefaultAccessLevels"].Value;
                if (!string.IsNullOrWhiteSpace(defaultRoles))
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Default role: {defaultRoles}");
                    var templevels = defaultRoles.Split(',').ToList();
                    foreach (string group in templevels)
                    {
                        if (!smsUser.AccessLevels.Contains(group))
                        {
                            smsUser.AccessLevels.Add(group);
                        }
                    }
                }
                //Check if user has group add user in OneCard Groups
                if (smsUser.AccessLevels != null && smsUser.AccessLevels.Count > 0)
                {
                    bool isExecutedSuccessfully = EndpointConnector.AddEntitlement(smsUser, EntitlementType.AccessLevel);
                    string grpListStr = this.getGroupListAsString(smsUser.AccessLevels);

                    if (isExecutedSuccessfully)
                    {
                        this.InsertSMSUserAccessLevel(smsUser);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroup, smsUser.MIDNAME, groups, this.Endpoint), null);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroupFailed, smsUser.MIDNAME, groups, this.Endpoint), null);
                    }
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return isUpdated;
        }

        protected override bool AreEntitlementsValid(string[] entitlements)
        {
            var accessLevels = SMSAccessLevel.GetAllByEndpointSystemId(this.Endpoint.Id, entitlements);

            List<string> rolesString = new List<string>();
            if (accessLevels != null && accessLevels.Count > 0)
            {
                rolesString = accessLevels.Select(r => r.Name).Distinct().ToList();
            }

            IEnumerable<string> difference = entitlements.Where(s => !string.IsNullOrWhiteSpace(s) && !rolesString.Contains(s,StringComparer.InvariantCultureIgnoreCase)).Select(s => s);
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
        #endregion
        #region Private Methods
        private bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User usr, string npacRoles, AdHocSMSRequest smsDetail)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var smsUsers = SMSUserInfo.GetUnLockedSMSUserByDIUserId(usr.Id, this.Endpoint.Id);
                if (smsUsers != null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User already have an active OneCard account - {0}. Cannot request/create secondary account in {1}. Marking current request completed.", smsUsers.MIDNAME, this.Endpoint.DisplayName), null);
                    return true;
                }
                var smsUser = SMSUserInfo.GetLockedSMSUserByDIUserId(usr.Id, this.Endpoint.Id);
                _idmsLogger.WriteLog(LogLevelType.Info, $"smsUser Is null:{smsUser == null}");
                if (smsUser != null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"SMSBadgeInfos is not null:{smsUser.SMSBadgeInfos != null}");
                    if (smsUser.SMSBadgeInfos != null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"SMSBadgeInfos count:{smsUser.SMSBadgeInfos.Count}");
                        if (smsUser.SMSBadgeInfos.Count == 0)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Added exisiting badge");
                            var badgeInFo = SMSBadgeInfo.GetByBadgeUserIdRejoin(smsUser.UserId);
                            if (badgeInFo != null)
                            {
                                smsUser.SMSBadgeInfos.Add(badgeInFo);
                            }
                        }
                    }
                    else
                    {
                        smsUser.SMSBadgeInfos = new List<SMSBadgeInfo>();
                    }
                    smsUser.NewPernr = usr.EmployeeId;
                    smsUser.FIRSTNAME = usr.FirstName;
                    smsUser.LASTNAME = usr.LastName;
                    smsUser.MIDNAME = usr.KnownAs;
                    smsUser.DisplayName = usr.DisplayName;
                    smsUser.SSNO = usr.EmployeeId;

                    smsUser.Addr1 = string.Format(@"{0} \ {1}", usr.OrgDepartment, usr.OrgBranch);
                    smsUser.City = usr.OrgSection;
                    smsUser.Zip = usr.IdentificationNo;
                    smsUser.EMAIL = usr.EmailId;
                    smsUser.OPHONE = usr.UserName;

                    if (!string.IsNullOrWhiteSpace(usr.MobileNumber))
                    {
                        if (usr.MobileNumber.Trim().Length > 8)
                            smsUser.Extension = usr.MobileNumber.Trim().Substring(usr.MobileNumber.Trim().Length - 8, 8);
                        else
                            smsUser.Extension = usr.MobileNumber.Trim();
                    }


                    var company = SMSCompany.GetSMSCompanyByName(usr.CompanyName);

                    if (company != null)
                    {
                        smsUser.DEPT = company.CompanyKey;
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"calling EnableUser");
                    isExecutedSuccessfully = this.EnableUser(usr, smsUser, fields);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"EnableUser:{isExecutedSuccessfully}");
                    if (isExecutedSuccessfully)
                    {
                        this.ReOrg(usr, smsUser);
                        smsUser.Save("System");
                        CreateUserInReconTable(smsUser, MasterConfigHelper.ServiceUser);
                        if (usr.FullTimeEmployee)
                        {
                            smsUser.AccessLevels = this.GetAdditionalGroupsByUser(usr, this.Endpoint.Id);
                        }

                        if (!string.IsNullOrEmpty(fields["DefaultAccessLevels"].Value))
                        {
                            string[] defaultSMSGrps = fields["DefaultAccessLevels"].Value.Split(',');

                            if (defaultSMSGrps != null && defaultSMSGrps.Length > 0)
                            {
                                foreach (string tmpGrpValue in defaultSMSGrps)
                                {
                                    if (!string.IsNullOrEmpty(tmpGrpValue))
                                    {
                                        smsUser.AccessLevels.Add(tmpGrpValue.Trim());
                                    }
                                }
                            }
                        }
                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, smsUser, usr, ApplicationType.OneCard);
                        ProvisioningEmailHelper.NotifyPassOfficeAboutOnBoard(usr);
                        if (!Result)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for  Active Directory User Creation"));
                        }
                    }
                    //new SMSDA().UpdateSMSUserInfoFromProvisioning(smsUser);
                }
                else
                {
                   

                    smsUser = new SMSUserInfo();
                    string accountName = string.Empty;

                    #region Set AD Object

                    foreach (ProvisioningParam gData in fields)
                    {
                        smsUser = setUserInfoByAtrbValue(smsUser, usr, gData) as SMSUserInfo;
                    }

                    smsUser.NewPernr = usr.EmployeeId;
                    smsUser.EndPointSystemId = this.Endpoint.Id;
                    smsUser.Password = this.constructUserPassword(fields);

                    smsUser.FIRSTNAME = usr.FirstName;
                    smsUser.LASTNAME = usr.LastName;
                    smsUser.MIDNAME = usr.KnownAs;
                    smsUser.DisplayName = usr.DisplayName;
                    smsUser.SSNO = usr.EmployeeId;

                    smsUser.Addr1 = string.Format(@"{0} \ {1}", usr.OrgDepartment, usr.OrgBranch);
                    smsUser.City = usr.OrgSection;
                    smsUser.Zip = usr.IdentificationNo;
                    smsUser.EMAIL = usr.EmailId;
                    smsUser.OPHONE = usr.UserName;
                    if (!string.IsNullOrWhiteSpace(usr.MobileNumber))
                    {
                        if (usr.MobileNumber.Trim().Length > 8)
                            smsUser.Extension = usr.MobileNumber.Trim().Substring(usr.MobileNumber.Trim().Length - 8, 8);
                        else
                            smsUser.Extension = usr.MobileNumber.Trim();
                    }
                    var company = SMSCompany.GetSMSCompanyByName(usr.CompanyName);

                    if (company != null)
                    {
                        smsUser.DEPT = company.CompanyKey;
                    }

                    smsUser.SMSBadgeInfos = new List<SMSBadgeInfo>();
                    var badge = this.CreateBadgeShell(usr, smsUser, fields);
                    smsUser.SMSBadgeInfos.Add(badge);

                    if (usr.FullTimeEmployee)
                    {
                        smsUser.AccessLevels = this.GetAdditionalGroupsByUser(usr, smsUser.EndPointSystemId);
                    }

                    if (!string.IsNullOrEmpty(fields["DefaultAccessLevels"].Value))
                    {
                        string[] defaultSMSGrps = fields["DefaultAccessLevels"].Value.Split(',');

                        if (defaultSMSGrps != null)
                        {
                            foreach (string tmpGrpValue in defaultSMSGrps)
                            {
                                if (!string.IsNullOrEmpty(tmpGrpValue))
                                {
                                    smsUser.AccessLevels.Add(tmpGrpValue.Trim());
                                }
                            }
                        }
                    }

                    #endregion

                    isExecutedSuccessfully = EndpointConnector.CreateAccount(smsUser, fields);

                    if (isExecutedSuccessfully)
                    {
                        smsUser.Save(MasterConfigHelper.ServiceUser);
                        CreateUserInReconTable(smsUser, MasterConfigHelper.ServiceUser);
                        foreach (var item in smsUser.SMSBadgeInfos)
                        {
                            item.Save(MasterConfigHelper.ServiceUser);
                        }

                        bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, smsUser, usr, ApplicationType.OneCard);
                        ProvisioningEmailHelper.NotifyPassOfficeAboutOnBoard(usr);

                        if (!Result)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, "Email cannot be sent", new Exception("Error While Sending EMail in HR System for   User Creation"));
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserCreationFailed, smsUser.MIDNAME, this.Endpoint.DisplayName), null);
                        return false;
                    }
                }

                if (isExecutedSuccessfully)
                {
                    #region Add user in to Group if group exist for user
                    //Check if user has group add user in AD Groups
                    if (smsUser.AccessLevels != null && smsUser.AccessLevels.Count > 0)
                    {
                        isExecutedSuccessfully = EndpointConnector.AddEntitlement(smsUser, EntitlementType.AccessLevel);
                        string grpListStr = this.getGroupListAsString(smsUser.AccessLevels);

                        if (isExecutedSuccessfully)
                        {
                            this.InsertSMSUserAccessLevel(smsUser);

                            if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroup, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroup, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                            }
                        }
                        else
                        {

                            _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroupFailed, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                        }
                    }
                    #endregion

                    if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserCreated, smsUser.MIDNAME, this.Endpoint.DisplayName), null);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserCreated, smsUser.MIDNAME, this.Endpoint.DisplayName), null);
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }

            return isExecutedSuccessfully;
        }
        private bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var smsUser = SMSUserInfo.GetByIdIncludeAccesslevels(endpointUserId);
                if (smsUser != null)
                {
                    isRemovedSuccessfully = this.DisableUser(smsUser, LockScenario.AdhocRequest);

                    if (isRemovedSuccessfully)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserDeleted, smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningNPACUserDeletionFailed, smsUser.MIDNAME, smsUser.EndPointSystemId), null);
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_FormUserNull, "NPAC"), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the OneCard account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }
        private bool AmendentAdd(DomainModel.User user, long endpointUserId, string levels, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var smsUser = SMSUserInfo.GetById(endpointUserId);

                if (smsUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User value is NULL for the Access Request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                if (string.IsNullOrEmpty(fields["DefaultAccessLevels"].Value))
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("DefaultAccessLevels value is NULL for the Access Request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                try
                {

                    smsUser.AccessLevels = fields["DefaultAccessLevels"].Value.Split(',').ToList();

                    isExecutedSuccessfully = EndpointConnector.AddEntitlement(smsUser, EntitlementType.AccessLevel);
                    string grpListStr = UtilityMethods.GetStringFromList(smsUser.AccessLevels);
                    if (isExecutedSuccessfully)
                    {
                        this.InsertSMSUserAccessLevel(smsUser);

                        ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, grpListStr, smsUser.MIDNAME);

                        if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroup, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroup, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserAddedInToGroupFailed, smsUser.MIDNAME, grpListStr, this.Endpoint.DisplayName), null);
                    }
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                    return false;
                }
                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while adding groups to the AD account. {0}", ex.Message), ex);
                return false;
            }
        }
        private bool AmendentRemove(DomainModel.User user, long endpointUserId, string levels, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var smsUser = SMSUserInfo.GetByIdIncludeAccesslevels(endpointUserId);

                if (smsUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User value is NULL for the Access Request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                if (string.IsNullOrEmpty(fields["DefaultAccessLevels"].Value))
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("DefaultAccessLevels value is NULL for the Access Request -{0}", this.CurrentRequest.Id), null);
                    return false;
                }

                var groupsToRemove = fields["DefaultAccessLevels"].Value.Split(',').ToList();

                var existingGroups = smsUser.SMSAccessLevels.Select(a => a.Name).ToList();

                smsUser.AccessLevels = new List<string>();
                foreach (string groupName in groupsToRemove)
                {
                    if (existingGroups.Contains(groupName,StringComparer.InvariantCultureIgnoreCase))
                    {
                        smsUser.AccessLevels.Add(groupName);
                    }
                }

                string grpListStr = this.getGroupListAsString(smsUser.AccessLevels);
                isExecutedSuccessfully = EndpointConnector.RemoveEntitlement(smsUser, EntitlementType.AccessLevel);
                if (isExecutedSuccessfully)
                {
                    this.DeleteSMSUseLevelMap(smsUser, false);

                    ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, grpListStr, smsUser.MIDNAME);

                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserRemovedFromGroup, smsUser.MIDNAME, grpListStr, smsUser.EndPointSystemId), null);
                }
                else
                {

                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningSMSUserRemoveFromGroupFailed, smsUser.MIDNAME, grpListStr, smsUser.EndPointSystemId), null);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }
        private string constructUserPassword(ProvisioningParamCollection fields)
        {
            try
            {

                Random rnd = new Random();
                int pin = rnd.Next(1001, 9999);

                return pin.ToString();

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, "Exception in constructUserPassword()", ex);
                throw ex;
            }
        }
        private bool EnableUser(DomainModel.User user, SMSUserInfo smsUser, ProvisioningParamCollection fields)
        {
            bool isusrEnabled = true;
            try
            {
                if (smsUser != null)
                {
                    var currentDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                    isusrEnabled = false;
                    if (smsUser.SMSBadgeInfos != null && smsUser.SMSBadgeInfos.Count > 0 &&  smsUser.SMSBadgeInfos.All(i => (!string.IsNullOrWhiteSpace(i.DEACTIVATE) && Convert.ToInt32(i.DEACTIVATE.GetSubString(8)) <= currentDate)))
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"EnableUser: found exsiting Batch status:3");
                        smsUser.SMSBadgeInfos.ForEach(i =>
                        {
                            i.STATUS = 1;
                            i.ACTIVATE = DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                            i.DEACTIVATE = user.TerminationDate.HasValue ? user.TerminationDate.Value.Date.AddDays(1).ToString("yyyyMMddHHmmss.ffffff+480") : DateTime.MaxValue.Date.ToString("yyyyMMddHHmmss.ffffff+480");
                        });
                    }
                    else
                    {
                        bool flag = false;
                        var badge = this.CreateBadgeShell(user, smsUser, fields);
                        smsUser.SMSBadgeInfos.ForEach(i =>
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Check existing Badge to re-activate: BadgeNumber:{i.BadgeNumber} BadgeKey:{i.BadgeKey}");
                            if (i.BadgeNumber == badge.BadgeNumber)
                            {
                                flag = true;
                                i.BadgeType = badge.BadgeType;
                                i.DEST_EXEMPT = badge.DEST_EXEMPT;
                                i.EndPointSystemId = badge.EndPointSystemId;
                                i.LastModifiedBy = "System";
                                i.LastModifiedDate = DateTime.Now;
                                i.UserId = badge.UserId;
                                i.ACTIVATE = badge.ACTIVATE;
                                i.DEACTIVATE = badge.DEACTIVATE;
                                i.STATUS = 1;
                            }
                        });

                        if (!flag)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Creating new Badge to user:{smsUser.AccountName} BadgeNumber:{badge.BadgeNumber}");
                            smsUser.SMSBadgeInfos.Add(badge);
                        }
                    }

                    smsUser.Password = this.constructUserPassword(fields);

                    isusrEnabled = EndpointConnector.UnLockAccount((IEndpointUserInfo)smsUser);

                    if (isusrEnabled)
                    {
                        smsUser.SMSBadgeInfos.ForEach(i => { i.Save(MasterConfigHelper.ServiceUser); });

                        _idmsLogger.WriteLog(LogLevelType.Info, $"UnLockAccount Successfull");
                        smsUser.LastModifiedDate = DateTime.Now;
                        smsUser.LastModifiedBy = "System";
                        if (isusrEnabled)
                        {
                            SMSBadgeInfo.SaveBadges(smsUser.SMSBadgeInfos);
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} unlocked in OneCard.", smsUser.MIDNAME), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, "Exception in EnableUser()", ex);
                throw ex;
            }

            return isusrEnabled;
        }
        private bool DisableUser(SMSUserInfo smsUser, LockScenario lockScenario)
        {
            bool isusrDisabled = true;
            try
            {
                if (smsUser != null)
                {
                    isusrDisabled = false;

                    smsUser.AccessLevels = smsUser.SMSAccessLevels.Select(item => item.Name).ToList();
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Removing Access Level:{(string.Join(",", smsUser.AccessLevels))}");
                    EndpointConnector.RemoveEntitlement(smsUser, EntitlementType.AccessLevel);

                    this.DeleteSMSUseLevelMap(smsUser, true);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"LockAccount for {smsUser.AccountName} and Deactivate Badge");
                    isusrDisabled = EndpointConnector.LockAccount((IEndpointUserInfo)smsUser);
                    var acctRecon = AccountReconcilation.GetByUser(smsUser.EndPointSystemId, smsUser.Id);
                    if (isusrDisabled)
                    {

                        smsUser.LastModifiedDate = DateTime.Now;
                        smsUser.LastModifiedBy = "System";
                        smsUser.Save("System");

                        acctRecon.IsDeleted = true;
                        acctRecon.Save(MasterConfigHelper.ServiceUser);

                        smsUser.SMSBadgeInfos = SMSBadgeInfo.GetByBadgesByUserId(smsUser.UserId);

                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Found {smsUser.SMSBadgeInfos.Count} badges to deactivate in IDMS for {smsUser.UserId}");

                        foreach (SMSBadgeInfo badge in smsUser.SMSBadgeInfos)
                        {
                            if (badge.STATUS == 1)
                            {
                                badge.STATUS = 3;
                                badge.DEACTIVATE = DateTime.Now.ToString("yyyyMMddHHmmss.ffffff+480");
                                badge.LastModifiedBy = "System";
                                badge.LastModifiedDate = DateTime.Now;
                                badge.Save("System");
                                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Deactivated badge {badge.BadgeNumber} successfully");
                            }
                        }

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} locked in OneCard.", smsUser.MIDNAME), null);
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, "Exception in DisableUser()", ex);
                throw ex;
            }
            return isusrDisabled;
        }
        private SMSBadgeInfo CreateBadgeShell(DomainModel.User user, SMSUserInfo smsUser, ProvisioningParamCollection fields)
        {
            try
            {
                SMSBadgeInfo badge = new SMSBadgeInfo();

                badge.ACTIVATE = DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                badge.DEACTIVATE = user.TerminationDate.HasValue ? user.TerminationDate.Value.Date.AddDays(1).ToString("yyyyMMddHHmmss.ffffff+480") : DateTime.MaxValue.Date.ToString("yyyyMMddHHmmss.ffffff+480");
                badge.BadgeType = Convert.ToInt32(fields["DummyBadgeTypeId"].Value);
                long badgeNumber = MasterConfigHelper.SMSDummyBadgeStartRange + Convert.ToInt64(user.Id);
                badge.BadgeNumber = badgeNumber;
                badge.DEST_EXEMPT = 0;
                badge.EndPointSystemId = this.Endpoint.Id;
                badge.CreatedBy = "System";
                badge.CreatedDate = DateTime.Now;
                badge.LastModifiedBy = "System";
                badge.LastModifiedDate = DateTime.Now;
                badge.STATUS = 1;
                badge.UserId = smsUser.UserId;
                return badge;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, "Exception in CreateBadgeShell()", null);
                throw ex;
            }
        }
        private bool ReOrg(DomainModel.User user, SMSUserInfo smsUser)
        {
            try
            {
                smsUser.FIRSTNAME = user.FirstName;
                smsUser.LASTNAME = user.LastName;
                smsUser.MIDNAME = user.KnownAs;
                smsUser.DisplayName = user.DisplayName;
                smsUser.SSNO = user.EmployeeId;

                smsUser.Addr1 = string.Format(@"{0} \ {1}", user.OrgDepartment, user.OrgBranch);
                smsUser.City = user.OrgSection;
                smsUser.Zip = user.IdentificationNo;
                smsUser.EMAIL = user.EmailId;
                smsUser.OPHONE = user.UserName;
                if (!string.IsNullOrWhiteSpace(user.MobileNumber))
                {
                    if(user.MobileNumber.Trim().Length > 8)
                        smsUser.Extension = user.MobileNumber.Trim().Substring(user.MobileNumber.Trim().Length - 8, 8);
                    else
                        smsUser.Extension = user.MobileNumber.Trim();
                }

                var company = SMSCompany.GetSMSCompanyByName(user.CompanyName);

                if (company != null)
                {
                    smsUser.DEPT = company.CompanyKey;
                }

                smsUser.LastModifiedBy = "System";
                smsUser.LastModifiedDate = DateTime.Now;

                EndpointConnector.ChangeContainer(smsUser);

                smsUser.Save("System");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, "Exception in ReOrg()", ex);
                throw ex;
            }
            return true;
        }
        private List<string> GetAdditionalGroupsByUser(DomainModel.User user, long endPointSystemId)
        {
            List<string> groups = new List<string>();
            var smsmatrix = SMSXLProvisioningMatrix.GetByUser(user);
            if (smsmatrix != null)
            {
                string[] groupsFromMatrix = !string.IsNullOrEmpty(smsmatrix.DefaultGroups) ? smsmatrix.DefaultGroups.Split(',') : null;

                if (groupsFromMatrix != null)
                {
                    foreach (string grp in groupsFromMatrix)
                    {
                        if (!string.IsNullOrEmpty(grp))
                        {
                            groups.Add(grp.Trim());
                        }
                    }
                }
            }

            return groups;
        }
        public bool InsertSMSUserAccessLevel(SMSUserInfo smsUser)
        {
            var userWithExistingLevels = SMSUserInfo.GetByIdIncludeAccesslevels(smsUser.Id);

            foreach (string level in smsUser.AccessLevels)
            {

                SMSAccessLevel smsLevel = SMSAccessLevel.GetAccessLevelByName(level.Trim());
                if (smsLevel != null && !userWithExistingLevels.SMSAccessLevels.Any(l => l.Id == smsLevel.Id))
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"SMSAccessLevels: {level}:{smsLevel.AccessLevelId} And smsUser:{smsUser.Id}");
                    SMSUserInfo.InsertUserLevel(smsUser.Id, smsLevel.Id, smsUser.EndPointSystemId);
                }
            }

            return true;
        }
        public bool DeleteSMSUseLevelMap(SMSUserInfo smsUser, bool removeAll)
        {
            if (removeAll)
            {
                foreach (var smsLevel in smsUser.SMSAccessLevels)
                {
                    if (smsLevel != null)
                    {
                        SMSUserInfo.DeleteUserLevel(smsUser.Id, smsLevel.Id, smsUser.EndPointSystemId);
                    }
                }
            }
            else
            {
                foreach (var accessLevel in smsUser.AccessLevels)
                {
                    SMSAccessLevel smsLevel = SMSAccessLevel.GetAccessLevelByName(accessLevel.Trim());
                    if (smsLevel != null)
                    {
                        SMSUserInfo.DeleteUserLevel(smsUser.Id, smsLevel.Id, smsUser.EndPointSystemId);
                    }
                }
            }
            return true;
        }
        #endregion
        public override bool CreateUserInReconTable(IEndpointUserInfo epUser, string currentUser)
        {
            EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(epUser.EndPointSystemId);
            if (ep != null)
            {
                var existingAcctRecon = AccountReconcilation.GetByAccountName(epUser.AccountName, ep.Id);
                if (existingAcctRecon != null)
                {
                    existingAcctRecon.AccessRequestId = this.CurrentRequest.Id;
                    existingAcctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                    if (CurrentRequest.AdHocAccessRequestId.HasValue && CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        existingAcctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        existingAcctRecon.UserId = CurrentRequest.UserId;
                    }
                    existingAcctRecon.AccountCreatedDate = DateTime.Now;
                    if (existingAcctRecon.IsDeleted)
                    {
                        existingAcctRecon.IsLocked = false;
                        existingAcctRecon.IsDeleted = false;
                        existingAcctRecon.AccountCreatedDate = DateTime.Now;
                        existingAcctRecon.DeletedDate = null;
                        existingAcctRecon.LastAttestedOn = null;
                        existingAcctRecon.LastLogonDate = null;
                        existingAcctRecon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;

                        existingAcctRecon.EndpointUserId = epUser.Id;

                    }
                    // set mapping for global user
                    existingAcctRecon.FullName = !string.IsNullOrWhiteSpace(epUser.DisplayName) ? epUser.DisplayName : epUser.AccountName;

                    existingAcctRecon.MappingType = existingAcctRecon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                    existingAcctRecon.OffboardingAccountAction = (existingAcctRecon.AccountType == AccountType.User || existingAcctRecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;

                    //existingAcctRecon.UserId = CurrentRequest.UserId;
                    existingAcctRecon.Save(currentUser);
                }
                else
                {
                    AccountReconcilation recon = new AccountReconcilation();
                    recon.AccessRequestId = this.CurrentRequest.Id;
                    recon.UserId = CurrentRequest.UserId;
                    recon.AccountName = epUser.AccountName;
                    recon.FullName = !string.IsNullOrWhiteSpace(epUser.DisplayName) ? epUser.DisplayName : epUser.AccountName;
                    recon.EndpointSystemId = epUser.EndPointSystemId;
                    recon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;
                    recon.MappingType = recon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                    recon.OffboardingAccountAction = (recon.AccountType == AccountType.User || recon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    recon.EndpointUserId = epUser.Id;
                    recon.ValidFromDate = CurrentRequest.ActivationDate;
                    recon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    recon.AccountCreatedDate = DateTime.Now;
                    recon.Save(currentUser);

                }

            }
            return true;
        }

    }
}

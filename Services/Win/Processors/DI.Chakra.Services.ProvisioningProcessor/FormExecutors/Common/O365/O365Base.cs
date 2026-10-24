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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common
{
    public class O365Base : FormExecutorBase
    {
        string _userName = string.Empty;

        public O365Base(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
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

        #region OverrideMethods
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocO365Request o365Detail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "Office365 Provisioning failed. User object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string groups = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                o365Detail = (AdHocO365Request)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (o365Detail != null)
                {
                    endpointUserId = o365Detail.EndpointUserId;
                    // groups = o365Detail.Entitlement;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                isExecutedSuccessfully = CreateAccount(fields, user, groups, o365Detail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                isExecutedSuccessfully = DeleteAccount(user, endpointUserId, fields);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                isExecutedSuccessfully = AmendentAdd(user, endpointUserId, groups, fields, o365Detail);
            }
            else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                isExecutedSuccessfully = AmendentRemove(user, endpointUserId, groups, o365Detail, fields);
            }
            return isExecutedSuccessfully;
        }
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                var o365UserValue = Office365UserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (o365UserValue != null)
                {
                    this.TakeOUSnapshot(user.Id, o365UserValue, EmployeeEvents.Resignation, EndpointType.Office365, EntitlementType.Group);
                    this.DisableUser(o365UserValue, LockScenario.OffBoard);

                    if (CurrentRequest.AccountType == AccountType.Shared)
                    {
                        ProvisioningEmailHelper.NotifyOffice365SharedAccessRemovalToHelpDeskTeam(user, this.CurrentRequest, o365UserValue.WindowsEmailAddress, "Offboard");
                    }
                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Warning, "Office 365/OffBoard User object is null. Hence marking this Request as Completed", null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                return false;
            }
        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            //if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
            return true;

        }
        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }
        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {

            if (CurrentRequest.AccountType == AccountType.User)
            {
                try
                {
                    if (user != null)
                    {


                        var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        ConnectorBase adconnector = ConnectorFactory.GetConnector(adEndpoint, this.CurrentRequest);

                        var adUsr = ADSUserInfo.GetByDIUserIdByUserAccount(user.Id, adEndpoint.Id);

                        if (adUsr == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error while creating user {0} created in Office365. AD account does not exists.", user.KnownAs), null);
                            return false;
                        }
                        this.CurrentRequest.Remarks = adUsr.sAMAccountName;
                        try
                        {
                            adUsr.Groups = new List<string>();
                            var adGroupdField = fields.FirstOrDefault(i => i.Name == "ADGroups");
                            if (adGroupdField != null && !string.IsNullOrWhiteSpace(adGroupdField.Value))
                            {
                                foreach (var item in adGroupdField.Value.Split(','))
                                {
                                    if (!string.IsNullOrWhiteSpace(item) && !adUsr.Groups.Contains(item.Trim()))
                                    {
                                        adUsr.Groups.Add(item);
                                    }
                                }

                                WriteProvisioningLog(LogLevelType.Info, $"AD Groups count:{adUsr.Groups.Count}");
                                #region Add user in to Group if group exist for user
                                //Check if user has group add user in AD Groups
                                if (adUsr.Groups != null && adUsr.Groups.Count > 0)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, "AddEntitlement started");
                                    var isExecutedSuccessfully = adconnector.AddEntitlement(adUsr, EntitlementType.Group);
                                    string grpListStr = string.Join(",", adUsr.Groups);

                                    if (isExecutedSuccessfully)
                                    {
                                        ADSUserInfo.AddEntitlement(adUsr.Id, EntitlementType.Group, grpListStr);

                                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUsr.sAMAccountName, grpListStr, Endpoint.Description), null);
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUsr.sAMAccountName, grpListStr, Endpoint.Description), null);
                                    }
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteProvisioningLog(LogLevelType.Error, $"Add License via AD Groups failed{ex}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Exception occurred during Transfer event in Office 365 for the  User {0}.", user.KnownAs), ex);
                    return false;
                }

            }
            return true;


            //if user account  the assign ad groups. follow enable method
        }
        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }
        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            bool IsSuccess = true;
            List<bool> allExecuted = new List<bool>();
            try
            {
                if (user.TerminationDate.HasValue == false) return IsSuccess;

                if (accountType == AccountType.User || accountType == AccountType.Secondary)
                {
                    var accountRecons = AccountReconcilation.GetActiveAccountRecons(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                    if (accountRecons != null && accountRecons.Count > 0)
                    {
                        foreach (var accountRecon in accountRecons)
                        {
                            try
                            {
                                if (user.TerminationDate.HasValue && accountRecon.ValidToDate.HasValue && accountRecon.ValidToDate == user.TerminationDate)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Validity of User: {user.DisplayName} and the account:{accountRecon.AccountName} is same(Current:{accountRecon.ValidToDate}/ New {user.TerminationDate}) in Office365.");
                                }
                                else
                                {
                                    base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                                    WriteProvisioningLog(LogLevelType.Info, $"Validity of User: {user.DisplayName} account:{accountRecon.AccountName} is extended from:{accountRecon.ValidToDate} to {user.TerminationDate} in Office365.");

                                }
                                allExecuted.Add(true);
                            }
                            catch (Exception ex)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, $"RequestId:{this.CurrentRequest.Id} extension failed for the user:{user?.DisplayName} and account:{accountRecon.AccountName}.", ex);
                                allExecuted.Add(false);
                            }
                        }
                        if (allExecuted.All(i => i == false))
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"RequestId:{this.CurrentRequest.Id} extension failed for the all user acounts.{user.DisplayName}. Please check the log");
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"Zero account found for extension. Hence marking current request as completed.", null);
                    }
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Info, $"User:{user?.DisplayName} - accountType:{accountType}: No account found for extension", null);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"RequestId:{this.CurrentRequest.Id} extension failed for the user:{user?.DisplayName}.", ex);
                IsSuccess = false;
            }
            finally
            {
                allExecuted = null;
            }
            return IsSuccess;
        }
        public override bool AssignO365License(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                Dictionary<string, string> licenses = new Dictionary<string, string>();


                if (fields["Licenses"] == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, "fields{Licenses].Value == null", null);
                }

                var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                //remarks is holding samaccountname incase of o365 license assignment.
                var adUsr = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(this.CurrentRequest.Remarks, adEndpoint.Id);

                if (adUsr == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, "adUsr == null", null);
                }

                WriteProvisioningLog(LogLevelType.Info, $"Started assigning O365 license for user - {this.CurrentRequest.Remarks}.", null);

                if (adUsr != null)
                {
                    if (user == null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"assigning O365- UserObject NULL for accessRequestId {this.CurrentRequest.Id} .", null);
                    }

                    if (fields["Licenses"].Value == null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, "fields{Licenses].Value == null", null);
                    }

                    WriteProvisioningLog(LogLevelType.Info, "Started assigning O365 license for user - ." + user.DisplayName, null);

                    ConnectorBase o365Connector = ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest);

                    var licenseInfo = UtilityMethods.Deserialize<ProvLicenseInfo>(fields["Licenses"].Value);

                    WriteProvisioningLog(LogLevelType.Info, $"Licenses- {fields["Licenses"].Value}", null);

                    foreach (ProvLicenseInfoDetail detail in licenseInfo.LicenseDetails)
                    {
                        string key = detail.License;
                        string value = string.Empty;
                        List<string> disabledPlans = new List<string>();
                        List<string> enabledPlans = detail.ServiceName.Split(',').ToList();
                        var servicePlans = Office365ServicePlan.GetServicePlanByAccountSkuId(key);

                        foreach (Office365ServicePlan plan in servicePlans)
                        {
                            if (!enabledPlans.Contains(plan.ServiceName))
                            {
                                disabledPlans.Add(plan.ServiceName);
                            }
                        }

                        value = string.Join(",", disabledPlans);

                        licenses.Add(key, value);
                    }

                    WriteProvisioningLog(LogLevelType.Info, $"UPN- {string.Format("{0}@{1}", adUsr.sAMAccountName, MasterConfigHelper.ADUPNDomainName)}", null);

                    string upn = GetUpn(adUsr.sAMAccountName);
                    bool isLicenseAssigned = o365Connector.AssignLicense(upn, licenses);

                    if (isLicenseAssigned)
                    {
                        WriteProvisioningLog(LogLevelType.Info, "Completed assigning O365 license for user - ." + user.DisplayName, null);

                        //this.PopulateO365MailBoxSettingsRequest(user, fields, adUsr);
                        Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                        o365Connector.DownloadUser(upn);
                        if (this.CurrentRequest.EventUndoRequestId.HasValue == false)
                        {
                            ProvisioningEmailHelper.SendEmailForAccountProvisioningForOffice365(upn, user, "O365", "Office 365");
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, "Assigning O365 license failed for user - ." + user.DisplayName, null);
                        ProvisioningEmailHelper.NotifyAdminOffice365Failed(adUsr.mail, this.CurrentRequest);
                        return false;
                    }
                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, "Primary AD account not found. Assigning O365 license failed for user - ." + user.DisplayName, null);
                    ProvisioningEmailHelper.NotifyAdminOffice365Failed(string.Format("{0} ({1})", user.DisplayName, user.EmployeeId), this.CurrentRequest);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while assigning O365 license for user - ." + user.DisplayName, ex);
                return false;
            }
        }

        private static string GetUpn(string sAMAccountName)
        {
            return string.Format("{0}@{1}", sAMAccountName, MasterConfigHelper.ADUPNDomainName);
        }

        public override bool UpdateO365MailBoxSettings(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                Dictionary<string, string> licenses = new Dictionary<string, string>();

                var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                //remarks is holding samaccountname incase of o365 license assignment.
                var adUsr = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(this.CurrentRequest.Remarks, adEndpoint.Id);

                if (adUsr != null)
                {

                    ConnectorBase o365Connector = ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest);

                    string upn = string.Format("{0}@{1}", adUsr.sAMAccountName, MasterConfigHelper.ADUPNDomainName);

                    WriteProvisioningLog(LogLevelType.Info, "Started updating mailbox settings for user - ." + user.DisplayName, null);

                    bool mailboxUpdated = o365Connector.UpdateMailBoxSettings(upn, fields);

                    if (mailboxUpdated)
                    {
                        WriteProvisioningLog(LogLevelType.Info, "Completed updating mailbox settings for user - ." + user.DisplayName, null);
                        Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                        o365Connector.DownloadUser(upn);
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Error, "Error while updating mailbox settings failed for user - ." + user.DisplayName, null);
                        ProvisioningEmailHelper.NotifyAdminOffice365Failed(adUsr.mail, this.CurrentRequest);
                        return false;
                    }

                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, "Primary AD account not found. Updating O365 mailbox settings failed for user - ." + user.DisplayName, null);
                    ProvisioningEmailHelper.NotifyAdminOffice365Failed(string.Format("{0} ({1})", user.DisplayName, user.EmployeeId), this.CurrentRequest);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while updating O365 mailbox settings for user - ." + user.DisplayName, ex);
                return false;
            }
        }
        #endregion

        #region Private Methods
        bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string groups, AdHocO365Request o365Detail)
        {
            bool isExecutedSuccessfully = false;

            try
            {
                if (this.CurrentRequest.AccountType == AccountType.User)
                {
                    isExecutedSuccessfully = this.EnableUser(user, o365Detail, fields);
                }
                else if (this.CurrentRequest.AccountType == AccountType.Shared)
                {
                    Office365UserInfo o365UserValue = new Office365UserInfo();
                    #region Set O365 Object

                    o365UserValue.EndPointSystemId = this.Endpoint.Id;
                    o365UserValue.Alias = o365Detail.AdHocO365SharedAccount.AccountName.Trim();
                    o365UserValue.DisplayName = o365Detail.AdHocO365SharedAccount.DisplayName?.Trim();
                    o365UserValue.WindowsEmailAddress = string.Format("{0}@{1}", o365Detail.AdHocO365SharedAccount.AccountName, MasterConfigHelper.O365EmailDomainName);
                    var onlineDomainName = MasterConfigHelper.O365OnlineDomainName;
                    o365UserValue.UserPrincipalName = string.Format("{0}@{1}", o365Detail.AdHocO365SharedAccount.AccountName, onlineDomainName); ;

                    if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                    {
                        o365UserValue.NewUsrPassword = this.ConstructPassword();
                        o365UserValue.IsShared = false;
                    }
                    else
                    {
                        o365UserValue.NewUsrPassword = string.Format(ProvisioningConstants.PasswordMessage, "AD/Windows");
                        o365UserValue.IsShared = true;
                    }

                    o365UserValue.IsShared = true;

                    string[] defaultO365Grps = !string.IsNullOrWhiteSpace(fields["Groups"].Value) ? fields["Groups"].Value.Split(',') : null;
                    o365UserValue.Groups = new List<string>();

                    if (defaultO365Grps != null)
                    {
                        foreach (string tmpGrpValue in defaultO365Grps)
                        {
                            if (!string.IsNullOrWhiteSpace(tmpGrpValue))
                            {
                                o365UserValue.Groups.Add(tmpGrpValue.Trim());
                            }
                        }
                    }

                    #endregion

                    try
                    {
                        //Adding No Internet Mail Group for Internal Shared Account
                        if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                        {
                            var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                            string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;

                            if (nointernetO365Grps != null)
                            {
                                foreach (string tmpGrp in nointernetO365Grps)
                                {
                                    if (!string.IsNullOrWhiteSpace(tmpGrp))
                                    {
                                        o365UserValue.Groups.Add(tmpGrp.Trim());
                                    }
                                }
                            }
                        }

                        isExecutedSuccessfully = EndpointConnector.CreateAccount(o365UserValue, fields);


                        if (isExecutedSuccessfully)
                        {
                            AddUsertoOffice365(o365UserValue);

                            ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, o365UserValue, user, ApplicationType.Office365);
                            Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                            EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);
                            if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                            {
                                var groupsAdded = EndpointConnector.AddEntitlement(o365UserValue, EntitlementType.Group);
                                if (groupsAdded)
                                {
                                    try
                                    {
                                        Office365UserInfo.InsertOffic365UserGroupMapping(this.Endpoint.Id, o365UserValue.Id, o365UserValue.Groups);
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"Adding No Internet Email Group to User:{o365UserValue.AccountName} completed");
                                    }
                                    catch (Exception ex)
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while adding user group:{o365UserValue.AccountName}", ex);
                                    }
                                }

                            }
                            bool licenseAssigned = true;
                            bool membersAssigned = true;

                            if (o365Detail.SharedMailBoxAction == SharedMailBoxActionType.MemberAddition)
                            {
                                membersAssigned = EndpointConnector.AssignMembers(o365UserValue.AccountName, o365Detail.Members);

                                if (membersAssigned)
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully assigned members {0} from the mailbox {1}", string.Join(",", o365Detail.Members), o365UserValue.AccountName), null);
                            }

                            // if (!o365UserValue.IsShared)
                            if (o365Detail.MailBoxConversionType == MailBoxConversionType.UserToShared)
                            {
                                Dictionary<string, string> licenses = new Dictionary<string, string>();

                                var licenseInfo = UtilityMethods.Deserialize<ProvLicenseInfo>(fields["Licenses"].Value);

                                if (fields["Licenses"].Value != null)
                                {
                                    foreach (ProvLicenseInfoDetail detail in licenseInfo.LicenseDetails)
                                    {
                                        string key = detail.License;
                                        string value = string.Empty;
                                        List<string> disabledPlans = new List<string>();
                                        List<string> enabledPlans = detail.ServiceName.Split(',').ToList();
                                        var servicePlans = Office365ServicePlan.GetServicePlanByAccountSkuId(key);

                                        foreach (Office365ServicePlan plan in servicePlans)
                                        {
                                            if (!enabledPlans.Contains(plan.ServiceName))
                                            {
                                                disabledPlans.Add(plan.ServiceName);
                                            }
                                        }

                                        value = string.Join(",", disabledPlans);

                                        licenses.Add(key, value);
                                    }

                                    licenseAssigned = EndpointConnector.AssignLicense(o365UserValue.UserPrincipalName, licenses);
                                    Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                                    EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                                    if (licenseAssigned)
                                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully assigned licenses {0} for the user {1}", string.Join(",", licenses), o365UserValue.UserPrincipalName), null);
                                }
                                else
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Deafult Provisiong licenses not availablt for the user {0}", o365UserValue.UserPrincipalName), null);
                                }
                            }

                            UpdateOffice365UserInfo(o365Detail, o365UserValue);

                            isExecutedSuccessfully = licenseAssigned && membersAssigned;

                            if (this.CurrentRequest.AdHocAccessRequestId.HasValue)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Shared mailbox \"{0}\" is created successfully.", o365UserValue.AccountName), null);
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Shared mailbox \"{0}\" is created successfully.", o365UserValue.AccountName), null);
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Shared mailbox creation is failed for user \"{0}\".", o365UserValue.AccountName), null);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                isExecutedSuccessfully = false;
            }

            return isExecutedSuccessfully;
        }
        /// <summary>
        /// This is required to set the service plan null to differentiate the Internal shared MailBox. All internal shared mailbox service plan will be null.
        /// </summary>
        /// <param name="o365Detail"></param>
        /// <param name="o365UserValue"></param>
        private void UpdateOffice365UserInfo(AdHocO365Request o365Detail, Office365UserInfo o365UserValue)
        {
            try
            {
                //Converting to regular mailbox. Regular mailbox dont have service plan
                if (o365Detail != null && o365UserValue != null && o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                {
                    var updatedUserInfo = Office365UserInfo.GetByUserPrincipleName(o365UserValue.UserPrincipalName);
                    if (updatedUserInfo != null)
                    {
                        updatedUserInfo.ServicePlanInfo = null;
                        updatedUserInfo.Save("System");
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger?.WriteLog(LogLevelType.Exception, $"UpdateOffice365UserInfo failed:{ex.Message}", ex);
            }
        }

        bool DeleteAccount(DomainModel.User user, long endpointUserId, ProvisioningParamCollection fields)
        {
            bool isRemovedSuccessfully = false;
            try
            {
                var o365UserValue = Office365UserInfo.GetById(endpointUserId);
                isRemovedSuccessfully = this.DisableUser(o365UserValue, LockScenario.AdhocRequest);
                if (isRemovedSuccessfully)
                {
                    //clean licenses
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        try
                        {
                            if (user != null)
                            {
                                var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                                ConnectorBase adconnector = ConnectorFactory.GetConnector(adEndpoint, this.CurrentRequest);

                                var adUsr = ADSUserInfo.GetByDIUserIdByUserAccount(user.Id, adEndpoint.Id);

                                if (adconnector != null && adUsr != null)
                                {
                                    adUsr.Groups = new List<string>();
                                    var assignedGroups = ADSUserInfo.GetByIdIncludeADSGroups(adUsr.Id);
                                    if(assignedGroups!=null && assignedGroups.ADSGroups?.Count>0)
                                    {
                                        var adGroupdField = fields.FirstOrDefault(i => i.Name == "ADGroups");
                                        if (adGroupdField != null && !string.IsNullOrWhiteSpace(adGroupdField.Value))
                                        {
                                            foreach (var item in adGroupdField.Value.Split(','))
                                            {
                                                if (!string.IsNullOrWhiteSpace(item) && assignedGroups.ADSGroups.Any(s=>s.name.Trim()==item.Trim()))
                                                {
                                                    adUsr.Groups.Add(item);
                                                }
                                            }
                                            if (adUsr.Groups.Count > 0)
                                            {
                                                WriteProvisioningLog(LogLevelType.Info, $"O365 License AD Groups count:{adUsr.Groups.Count}");

                                                var isExecutedSuccessfully = adconnector.RemoveEntitlement(adUsr, EntitlementType.Group);
                                                string grpListStr = string.Join(",", adUsr.Groups);

                                                if (isExecutedSuccessfully)
                                                {
                                                    ADSUserInfo.RemoveEntitlement(adUsr.Id, EntitlementType.Group, grpListStr);

                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error encountered during the license removal step of a Delete Account operation for user {0} in Office 365.", user.KnownAs), ex);
                            return false;
                        }

                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserDeleted, o365UserValue.UserPrincipalName, o365UserValue.EndPointSystemId), null);
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserDeletionFailed, o365UserValue.UserPrincipalName, o365UserValue.EndPointSystemId), null);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting the Office 365 account. {0}", ex.Message), ex);
                return false;
            }

            return isRemovedSuccessfully;
        }
        bool AmendentAdd(DomainModel.User user, long endpointUserId, string groups, ProvisioningParamCollection fields, AdHocO365Request o365Detail)
        {
            try
            {
                var emailEntitlementContent = string.Empty;
                bool isExecutedSuccessfully = false;
                bool extEmail = true;
                bool licenseAssigned = true;
                var o365UserValue = Office365UserInfo.GetById(endpointUserId);

                if (o365UserValue != null)
                {
                    if (this.CurrentRequest.AccountType == AccountType.User)
                    {
                        var enableExternalEmail = o365Detail.ExternalEmailAction == ExternalEmailActionType.Enable;
                        emailEntitlementContent = $@"Enable External EmailAccess:{(enableExternalEmail ? "Yes" : "No")}";
                        if (enableExternalEmail)
                        {
                            string access = MasterConfigHelper.O365AllowExternalEmail;
                            string accountName = o365UserValue.UserPrincipalName.Split('@')[0];
                            extEmail = this.UpdateExternalEmailAccess(accountName, access);

                            if (extEmail)
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("External Email access assigned for user {0}", o365UserValue.UserPrincipalName), null);
                        }

                        if (o365Detail.LicenseAction == LicenseActionType.Add)
                        {
                            Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                            EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                            Dictionary<string, string> licenses = new Dictionary<string, string>();
                            Dictionary<string, List<string>> exDisabledPlans = new Dictionary<string, List<string>>();

                            var licenseToAdd = o365Detail.LicenseInfo;

                            if (o365UserValue.ServicePlanObject != null)
                            {
                                List<string> existingLicenses = o365UserValue.ServicePlanObject.PlanDetails.Select(item => item.AccountSkuId).Distinct().ToList();

                                foreach (string lic in existingLicenses)
                                {
                                    var plans = o365UserValue.ServicePlanObject.PlanDetails.Where(item => item.AccountSkuId == lic && item.ProvisioningStatus == "Disabled").Select(item => item.ServiceName).ToList();
                                    exDisabledPlans.Add(lic, plans);
                                }
                            }

                            foreach (AdHocLicenseInfo detail in licenseToAdd)
                            {
                                string key = detail.AccountSkuId;
                                string value = string.Empty;

                                List<string> disabledPlans = new List<string>();
                                List<string> plansToAdd = detail.ServiceName.Split(',').ToList();

                                if (exDisabledPlans.ContainsKey(key))
                                {
                                    List<string> tmpDisabledPlans = exDisabledPlans[key];

                                    foreach (string plan in tmpDisabledPlans)
                                    {
                                        if (!plansToAdd.Contains(plan))
                                        {
                                            disabledPlans.Add(plan);
                                        }
                                    }
                                }
                                else
                                {
                                    var servicePlans = Office365ServicePlan.GetServicePlanByAccountSkuId(key);

                                    foreach (Office365ServicePlan plan in servicePlans)
                                    {
                                        if (!plansToAdd.Contains(plan.ServiceName))
                                        {
                                            disabledPlans.Add(plan.ServiceName);
                                        }
                                    }
                                }

                                value = string.Join(",", disabledPlans);
                                licenses.Add(key, value);
                            }

                            licenseAssigned = EndpointConnector.AssignLicense(o365UserValue.UserPrincipalName, licenses);
                            Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                            EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                            if (licenseAssigned)
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully assigned licenses {0} for the user {1}", string.Join(",", licenses), o365UserValue.UserPrincipalName), null);
                        }

                        isExecutedSuccessfully = extEmail && licenseAssigned;
                    }
                    else if (this.CurrentRequest.AccountType == AccountType.Shared)
                    {
                        bool converted = false;
                        EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                        o365UserValue = Office365UserInfo.GetByIdIncludeGroups(endpointUserId);

                        if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                        {
                            emailEntitlementContent = @"Converted License to internal mailbox";
                            converted = EndpointConnector.ConvertSharedToRegularMailBox(o365UserValue.AccountName);

                            if (converted)
                            {
                                //o365UserValue.IsShared = false;
                                o365UserValue.LastModifiedBy = "System";
                                o365UserValue.LastModifiedDate = DateTime.Now;
                                o365UserValue.Save("System");
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Converted shared to regular mailbox {0}", o365UserValue.AccountName), null);

                                try
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Adding No Internet shared to regular mailbox {0}", o365UserValue.AccountName), null);
                                    if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                                    {
                                        var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                                        string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;
                                        o365UserValue.Groups = new List<string>();
                                        if (nointernetO365Grps != null && nointernetO365Grps.Length > 0)
                                        {
                                            foreach (string tmpGrp in nointernetO365Grps)
                                            {
                                                if (!string.IsNullOrWhiteSpace(tmpGrp))
                                                {
                                                    o365UserValue.Groups.Add(tmpGrp.Trim());
                                                }
                                            }
                                        }
                                    }
                                    if (o365UserValue.Groups != null && o365UserValue.Groups.Count > 0)
                                    {
                                        converted = EndpointConnector.AddEntitlement(o365UserValue, EntitlementType.Group);
                                        if (converted)
                                        {
                                            try
                                            {
                                                Office365UserInfo.InsertOffic365UserGroupMapping(this.Endpoint.Id, o365UserValue.Id, o365UserValue.Groups);
                                            }
                                            catch (Exception ex)
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while adding groups to user:{o365UserValue.AccountName}", ex);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error in Adding No Internet shared to regular mailbox {0}", o365UserValue.AccountName), ex);
                                }
                            }
                        }

                        // if (!o365UserValue.IsShared)
                        if (o365Detail.MailBoxConversionType == MailBoxConversionType.UserToShared)
                        {
                            emailEntitlementContent = @"Converted internal to License mailbox";
                            Dictionary<string, string> licenses = new Dictionary<string, string>();

                            var licenseInfo = UtilityMethods.Deserialize<ProvLicenseInfo>(fields["Licenses"].Value);

                            foreach (ProvLicenseInfoDetail detail in licenseInfo.LicenseDetails)
                            {
                                string key = detail.License;
                                string value = string.Empty;
                                List<string> disabledPlans = new List<string>();
                                List<string> enabledPlans = detail.ServiceName.Split(',').ToList();
                                var servicePlans = Office365ServicePlan.GetServicePlanByAccountSkuId(key);

                                foreach (Office365ServicePlan plan in servicePlans)
                                {
                                    if (!enabledPlans.Contains(plan.ServiceName))
                                    {
                                        disabledPlans.Add(plan.ServiceName);
                                    }
                                }

                                value = string.Join(",", disabledPlans);

                                licenses.Add(key, value);
                            }

                            licenseAssigned = EndpointConnector.AssignLicense(o365UserValue.UserPrincipalName, licenses);
                            Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                            EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                            if (licenseAssigned)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully assigned licenses {0} for the user {1}", string.Join(",", licenses), o365UserValue.UserPrincipalName), null);

                                try
                                {

                                    if (o365Detail.MailBoxConversionType == MailBoxConversionType.UserToShared)
                                    {
                                        var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                                        string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;

                                        if (nointernetO365Grps != null && nointernetO365Grps.Length > 0)
                                        {
                                            o365UserValue.Groups = new List<string>();
                                            foreach (string tmpGrp in nointernetO365Grps)
                                            {
                                                if (!string.IsNullOrWhiteSpace(tmpGrp))
                                                {

                                                    _idmsLogger.WriteLog(LogLevelType.Info, $"Removing O365SharedMailNoInternetGroup {o365UserValue.AccountName}", null);
                                                    o365UserValue.Groups = new List<string>();
                                                    o365UserValue.Groups.Add(tmpGrp);
                                                    try
                                                    {
                                                        converted = EndpointConnector.RemoveEntitlement(o365UserValue, EntitlementType.Group);
                                                        if (converted)
                                                        {
                                                            Office365UserInfo.DeleteOffic365UserGroupMapping(this.Endpoint.Id, o365UserValue.Id, o365UserValue.Groups);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _idmsLogger.WriteLog(LogLevelType.Exception, $"Error in Removing O365SharedMailNoInternetGroup:{o365UserValue?.AccountName}", ex);
                                                    }
                                                }
                                                else
                                                {
                                                    converted = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            converted = true;
                                        }
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error in Removing No Internet shared mailbox {0}", o365UserValue.AccountName), ex1);
                                }
                            }
                        }
                        UpdateOffice365UserInfo(o365Detail, o365UserValue);
                        isExecutedSuccessfully = converted && licenseAssigned;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Warning, string.Format(ProvisioningConstants.MSG_FormUserNull, "Office365AddEntitlement"), null);
                    return false;
                }

                if (isExecutedSuccessfully)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, this.CurrentRequest, emailEntitlementContent, o365UserValue.UserPrincipalName);
                }
                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }
        bool AmendentRemove(DomainModel.User user, long endpointUserId, string groups, AdHocO365Request o365Detail, ProvisioningParamCollection fields)
        {
            try
            {
                var emailEntitlement = string.Empty;
                bool extEmail = true;
                bool licenseAssigned = true;
                bool isExecutedSuccessfully = false;
                var o365UserValue = Office365UserInfo.GetById(endpointUserId);

                if (this.CurrentRequest.AccountType == AccountType.User)
                {
                    if (o365Detail.ExternalEmailAction == ExternalEmailActionType.Disable)
                    {
                        emailEntitlement = "Disable External Email:Yes";
                        string access = MasterConfigHelper.O365DenyExternalEmail;
                        string accountName = o365UserValue.UserPrincipalName.Split('@')[0];
                        extEmail = this.UpdateExternalEmailAccess(accountName, access);

                        if (extEmail)
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("External Email access removed for user {0}", o365UserValue.UserPrincipalName), null);
                    }

                    if (o365Detail.LicenseAction == LicenseActionType.Remove)
                    {
                        emailEntitlement = "Remove Licenses";
                        Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                        EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                        Dictionary<string, string> licenses = new Dictionary<string, string>();
                        Dictionary<string, List<string>> exDisabledPlans = new Dictionary<string, List<string>>();

                        List<string> licensesToRemove = o365Detail.LicenseInfo.Where(item => item.IsRemoveLicense).Select(item => item.AccountSkuId).ToList();

                        var licensesRequestedToRemove = o365Detail.LicenseInfo;

                        if (o365UserValue.ServicePlanObject != null)
                        {
                            List<string> existingLicenses = o365UserValue.ServicePlanObject.PlanDetails.Select(item => item.AccountSkuId).Distinct().ToList();

                            foreach (string lic in existingLicenses)
                            {
                                var plans = o365UserValue.ServicePlanObject.PlanDetails.Where(item => item.AccountSkuId == lic && item.ProvisioningStatus == "Disabled").Select(item => item.ServiceName).ToList();
                                exDisabledPlans.Add(lic, plans);
                            }
                        }

                        foreach (AdHocLicenseInfo detail in licensesRequestedToRemove)
                        {
                            string key = detail.AccountSkuId;
                            string value = string.Empty;

                            List<string> plansToRemove = detail.ServiceName.Split(',').ToList();

                            if (exDisabledPlans.ContainsKey(key))
                            {
                                List<string> disabledPlans = exDisabledPlans[key];

                                foreach (string plan in plansToRemove)
                                {
                                    if (!disabledPlans.Contains(plan))
                                    {
                                        disabledPlans.Add(plan);
                                    }
                                }
                                value = string.Join(",", disabledPlans);
                            }

                            licenses.Add(key, value);
                        }

                        licenseAssigned = EndpointConnector.RemoveLicense(o365UserValue.UserPrincipalName, licenses, licensesToRemove);
                        Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                        EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                        if (licenseAssigned)
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully removed licenses {0} for the user {1}", string.Join(",", licenses), o365UserValue.UserPrincipalName), null);
                    }

                    isExecutedSuccessfully = extEmail && licenseAssigned;
                }
                else if (this.CurrentRequest.AccountType == AccountType.Shared)
                {
                    bool converted = true;
                    licenseAssigned = true;

                    if (o365Detail.MailBoxConversionType == MailBoxConversionType.UserToShared)
                    {
                        emailEntitlement = "Converted internal to License mailbox";
                        converted = EndpointConnector.ConvertRegularToSharedMailBox(o365UserValue.AccountName);

                        if (converted)
                        {
                            o365UserValue.IsShared = true;
                            o365UserValue.LastModifiedBy = "System";
                            o365UserValue.LastModifiedDate = DateTime.Now;
                            o365UserValue.Save("System");
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Converted regular to shared mailbox {0}", o365UserValue.AccountName), null);

                            try
                            {

                                if (o365Detail.MailBoxConversionType == MailBoxConversionType.UserToShared)
                                {
                                    var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                                    string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;

                                    if (nointernetO365Grps != null && nointernetO365Grps.Length > 0)
                                    {
                                        foreach (string tmpGrp in nointernetO365Grps)
                                        {
                                            if (!string.IsNullOrWhiteSpace(tmpGrp))
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removing No Internet shared to Shared mailbox {0}", o365UserValue.AccountName), null);
                                                o365UserValue.Groups = new List<string>();
                                                o365UserValue.Groups.Add(tmpGrp);
                                                converted = EndpointConnector.RemoveEntitlement(o365UserValue, EntitlementType.Group);
                                                try
                                                {
                                                    Office365UserInfo.DeleteOffic365UserGroupMapping(this.Endpoint.Id, o365UserValue.Id, o365UserValue.Groups);
                                                }
                                                catch (Exception ex)
                                                {
                                                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while adding user group:{o365UserValue.AccountName} Group:{tmpGrp}", ex);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex1)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error in Removing No Internet shared mailbox {0}", o365UserValue.AccountName), ex1);
                            }
                        }
                    }

                    // if (o365UserValue.IsShared)
                    if (o365Detail.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                    {
                        emailEntitlement = "Converted License to internal mailbox";
                        Dictionary<string, string> licenses = new Dictionary<string, string>();
                        List<string> licensesToRemove = new List<string>();

                        var licenseInfo = UtilityMethods.Deserialize<ProvLicenseInfo>(fields["Licenses"].Value);

                        foreach (ProvLicenseInfoDetail detail in licenseInfo.LicenseDetails)
                        {
                            licenses.Add(detail.License, detail.ServiceName);
                            licensesToRemove.Add(detail.License);
                        }

                        licenseAssigned = EndpointConnector.RemoveLicense(o365UserValue.UserPrincipalName, licenses, licensesToRemove);
                        Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);
                        EndpointConnector.DownloadUser(o365UserValue.UserPrincipalName);

                        if (licenseAssigned)
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Successfully removed licenses {0} for the user {1}", string.Join(",", licenses), o365UserValue.AccountName), null);

                        try
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, "Adding No Internet shared to Shared mailbox");
                            var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                            string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;

                            if (nointernetO365Grps != null && nointernetO365Grps.Length > 0)
                            {
                                foreach (string tmpGrp in nointernetO365Grps)
                                {
                                    if (!string.IsNullOrWhiteSpace(tmpGrp))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Adding No Internet shared to Shared mailbox {0}", o365UserValue.AccountName), null);
                                        o365UserValue.Groups = new List<string>();
                                        o365UserValue.Groups.Add(tmpGrp);
                                        converted = EndpointConnector.AddEntitlement(o365UserValue, EntitlementType.Group);
                                        if (converted)
                                        {
                                            try
                                            {
                                                Office365UserInfo.InsertOffic365UserGroupMapping(this.Endpoint.Id, o365UserValue.Id, o365UserValue.Groups);
                                            }
                                            catch (Exception ex)
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while adding user group:{o365UserValue.AccountName} Group:{tmpGrp}", ex);
                                            }
                                        }
                                    }
                                }
                            }
                            UpdateOffice365UserInfo(o365Detail, o365UserValue);
                        }
                        catch (Exception ex)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Adding No Internet shared to Shared mailbox User:{o365UserValue.AccountName}", ex);
                        }
                    }

                    isExecutedSuccessfully = converted && licenseAssigned;

                    if (CurrentRequest.AccountType == AccountType.Shared)
                    {
                        ProvisioningEmailHelper.NotifyOffice365SharedAccessRemovalToHelpDeskTeam(user, this.CurrentRequest, o365UserValue.WindowsEmailAddress, "AmendmentRemove");
                    }
                }

                if (isExecutedSuccessfully)
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, this.CurrentRequest, emailEntitlement, o365UserValue.UserPrincipalName);
                }

                return isExecutedSuccessfully;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while removing groups from the AD account. {0}", ex.Message), ex);
                return false;
            }
        }
        public bool EnableUser(DomainModel.User user, AdHocO365Request o365Detail, ProvisioningParamCollection fields)
        {
            bool isusrEnabled = true;
            try
            {
                if (user != null)
                {
                    isusrEnabled = false;

                    var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    ConnectorBase adconnector = ConnectorFactory.GetConnector(adEndpoint, this.CurrentRequest);

                    var adUsr = ADSUserInfo.GetByDIUserIdByUserAccount(user.Id, adEndpoint.Id);

                    if (adUsr == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error while creating user {0} created in Office365. AD account does not exists.", user.KnownAs), null);
                        return false;
                    }
                    this.CurrentRequest.Remarks = adUsr.sAMAccountName;

                    if (user.EmployeeType != EmployeeType.Permanent)
                    {
                        var o365Usr = AccountReconcilation.GetAccountByDIUserId(user.Id, this.Endpoint.Id);
                        var email = string.Empty;
                        email = adUsr.mail;
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AccountName:{adUsr.sAMAccountName} AD Email:{email} & IDMS User Email:{user.EmailId}");
                            email = email.Trim();
                            var domain = MasterConfigHelper.ADUPNDomainName.Trim();
                            if (!string.IsNullOrWhiteSpace(domain) && !email.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"AccountName:{adUsr.sAMAccountName} email:{email} is not the same domain: {domain}. Hence new EmailId will be generated.");
                                email = null;
                            }
                            if (email != null)
                            {
                                //Possiblity that For NonEmployee they might add mailid as SharedMailBox
                                var sharedAccount = AccountReconcilation.GetByAccountNameByAccountType(email, this.Endpoint.Id, AccountType.Shared);
                                if (sharedAccount != null)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"The given mailId is Shared mailbox:{sharedAccount.AccountName}. Hence new EmailId will be generated.");
                                    email = null;
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(email))
                        {
                            adUsr.mail = this.ConstructEmail(user.KnownAs);
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AccountName:{adUsr.sAMAccountName} New EmailId generated:{adUsr.mail}");
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AccountName:{adUsr.sAMAccountName} Existing EmailId:{adUsr.mail}");
                        }

                        isusrEnabled = adconnector.UpdatePOBoxAttribute(adUsr, true);

                        if (isusrEnabled)
                        {
                            if (o365Detail != null)
                            {
                                if (o365Detail.ExternalEmailAction == ExternalEmailActionType.Enable)
                                {
                                    string access = MasterConfigHelper.O365AllowExternalEmail;
                                    bool extEmail = this.UpdateExternalEmailAccess(adUsr.sAMAccountName, access);

                                    if (extEmail)
                                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("External Email access assigned for user {0}", adUsr.sAMAccountName), null);
                                }
                            }
                            if (user.FullTimeEmployee == false && o365Detail != null && this.CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
                            {
                                adUsr.NeedO365License = true;
                                adUsr.WogUserId = Constants.DontAllowSFBAccountCreation;
                            }
                            else
                            {
                                adUsr.NeedO365License = true;
                            }
                            this.CurrentRequest.Remarks = adUsr.sAMAccountName;
                            adUsr.LastModifiedDate = DateTime.Now;
                            adUsr.LastModifiedBy = "System";
                            adUsr.Save("System");

                            user.EmailId = adUsr.mail;
                            if (user.FullTimeEmployee)
                                user.NeedEmailUpdateToHR = true;

                            user.LastModifiedDate = DateTime.Now;
                            user.LastModifiedBy = "System";
                            user.Save("System");

                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} created in Office365.", user.KnownAs), null);

                        }
                    }
                    else
                    {
                        isusrEnabled = true;
                        WriteProvisioningLog(LogLevelType.Info, $"Since AD Account request set AD attribute to enable office365,IDMS Changing the Office365 Request Status to WaitingForLicenseAssignment");
                    }
                    if (isusrEnabled)
                    {
                        try
                        {
                            adUsr.Groups = new List<string>();
                            var adGroupdField = fields.FirstOrDefault(i => i.Name == "ADGroups");
                            if (adGroupdField != null && !string.IsNullOrWhiteSpace(adGroupdField.Value))
                            {
                                foreach (var item in adGroupdField.Value.Split(','))
                                {
                                    if (!string.IsNullOrWhiteSpace(item) && !adUsr.Groups.Contains(item.Trim()))
                                    {
                                        adUsr.Groups.Add(item);
                                    }
                                }

                                WriteProvisioningLog(LogLevelType.Info, $"AD Groups count:{adUsr.Groups.Count}");
                                #region Add user in to Group if group exist for user
                                //Check if user has group add user in AD Groups
                                if (adUsr.Groups != null && adUsr.Groups.Count > 0)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, "AddEntitlement started");
                                    var isExecutedSuccessfully = adconnector.AddEntitlement(adUsr, EntitlementType.Group);
                                    string grpListStr = string.Join(",", adUsr.Groups);

                                    if (isExecutedSuccessfully)
                                    {
                                        ADSUserInfo.AddEntitlement(adUsr.Id, EntitlementType.Group, grpListStr);

                                        WriteProvisioningLog(LogLevelType.Info, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroup, adUsr.sAMAccountName, grpListStr, Endpoint.Description), null);
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningADUserAddedInToGroupFailed, adUsr.sAMAccountName, grpListStr, Endpoint.Description), null);
                                    }
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteProvisioningLog(LogLevelType.Error, $"Add License via AD Groups failed{ex}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Exception occured during Create account in Office 365 for the  User {0}.", user.KnownAs), ex);
                return false;
            }

            return true;
        }
        public bool DisableUser(Office365UserInfo o365Usr, LockScenario lockScenario)
        {
            bool isusrDisabled = true;

            if (o365Usr != null)
            {
                isusrDisabled = false;

                if (this.CurrentRequest.AccountType == AccountType.User)
                {
                    var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    ConnectorBase adconnector = ConnectorFactory.GetConnector(adEndpoint, this.CurrentRequest);

                    string accountName = o365Usr.UserPrincipalName.Split('@')[0];

                    var adUser = ADSUserInfo.GetBysAMAccountName(accountName);

                    isusrDisabled = adconnector.UpdatePOBoxAttribute(adUser, false);
                    if (isusrDisabled)
                    {
                        o365Usr.LastModifiedDate = DateTime.Now;
                        o365Usr.LastModifiedBy = "System";
                        o365Usr.Save("System");
                        if (isusrDisabled)
                        {

                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} Disabled in Office365.", o365Usr.UserPrincipalName), null);
                        }
                        var acctRecon = AccountReconcilation.GetByEndpointIdActiveAndEndpointUserId(o365Usr.EndPointSystemId, o365Usr.Id);
                        if (null != acctRecon)
                        {
                            EntitlementReconcilation.DeleteEntitlementsByAcountReconId(acctRecon.Id, o365Usr.EndPointSystemId, "System");
                            acctRecon.IsLocked = true;
                            acctRecon.IsDeleted = true;
                            acctRecon.DeletedDate = DateTime.Now;
                            acctRecon.AccessRequestId = this.CurrentRequest.Id;
                            acctRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                else if (this.CurrentRequest.AccountType == AccountType.Shared)
                {
                    try
                    {
                        if (o365Usr.ServicePlanObject == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, "Removing No Internet shared to Shared mailbox");
                            var noInternetGroups = MasterConfigHelper.O365SharedMailNoInternetGroup;

                            string[] nointernetO365Grps = !string.IsNullOrWhiteSpace(noInternetGroups) ? noInternetGroups.Split(',') : null;

                            if (nointernetO365Grps != null && nointernetO365Grps.Length > 0)
                            {
                                foreach (string tmpGrp in nointernetO365Grps)
                                {
                                    if (!string.IsNullOrWhiteSpace(tmpGrp))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"Removing No Internet shared to Shared mailbox {o365Usr.AccountName} Group:{tmpGrp}");
                                        o365Usr.Groups = new List<string>();
                                        o365Usr.Groups.Add(tmpGrp);
                                        var completed = EndpointConnector.RemoveEntitlement(o365Usr, EntitlementType.Group);
                                        if (completed)
                                        {
                                            try
                                            {
                                                Office365UserInfo.DeleteOffic365UserGroupMapping(this.Endpoint.Id, o365Usr.Id, o365Usr.Groups);
                                            }
                                            catch (Exception ex)
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while creating user group:{tmpGrp} for user:{o365Usr.AccountName}", ex);
                                            }
                                        }
                                        else
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.Info, $"Removing No Internet shared to Shared mailbox {o365Usr.AccountName} is Completed:{completed}");
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, "Delete Account - Removing No Internet shared to Shared mailbox", ex);
                    }

                    isusrDisabled = false;
                    isusrDisabled = EndpointConnector.DeleteAccount(o365Usr);
                    if (isusrDisabled)
                    {
                        o365Usr.LastModifiedDate = DateTime.Now;
                        o365Usr.LastModifiedBy = "System";
                        o365Usr.Save("System");

                        if (isusrDisabled)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} removed in Office365.", o365Usr.UserPrincipalName), null);

                        }
                    }
                }
            }

            return isusrDisabled;
        }
        public string ConstructEmail(string UName)
        {

            string retString = string.Empty;

            string spaceRemovedString = RemoveSplCharsWithEmptyString(UName);

            int len = spaceRemovedString.Length;

            if (len <= 12)
            {
                retString = spaceRemovedString.ToLower();
            }
            else
            {
                if (UName.Contains(" "))
                {
                    string firstWord = UName.Split(' ').FirstOrDefault();

                    string remainingWords = UName.Substring(firstWord.Length + 1);

                    var v = remainingWords.Split(' ').Select(y => y[0]).ToArray();

                    string initials = string.Join("", v);

                    retString = string.Format("{0}{1}", firstWord, initials).ToLower();

                    retString = RemoveSplCharsWithEmptyString(retString);

                }
                else
                {
                    retString = UName.Substring(0, 12).ToLower();

                    retString = RemoveSplCharsWithEmptyString(retString);

                }
            }
            return this.CheckMailAddressExists(retString);
        }
        private string CheckMailAddressExists(string nickName)
        {
            string email = nickName;
            string domain = MasterConfigHelper.O365EmailDomainName;

            var tempEmail = email + "@" + domain;
            int no = 1;

            bool isExist = AccountReconcilation.IsEmailIdAlreadyExists(this.Endpoint.Id, tempEmail);

            while (isExist == true)
            {
                WriteProvisioningLog(LogLevelType.Warning, $"EmailId:{tempEmail} already exists. Hence try regenerate the new EmailId.");
                email = nickName;

                email = string.Format("{0}{1}", email, no.ToString());
                tempEmail = email + "@" + domain;
                isExist = AccountReconcilation.IsEmailIdAlreadyExists(this.Endpoint.Id, tempEmail);
                no++;
            }

            return email + "@" + domain;
        }
        public string RemoveSplCharsWithEmptyString(string UserName)
        {
            UserName = Regex.Replace(UserName, "[^0-9A-Za-z]+", "");
            return UserName;
        }
        private bool UpdateExternalEmailAccess(string accountName, string access)
        {
            var adEndpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
            ConnectorBase _connector = ConnectorFactory.GetConnector(adEndpoint, this.CurrentRequest);
            var isUpdated = _connector.UpdateExterEmailAccecss(accountName, access);
            if (isUpdated)
            {
                var adUserInfo = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(accountName, adEndpoint.Id);
                if (adUserInfo != null)
                {
                    adUserInfo.extensionAttribute4 = access;
                    adUserInfo.Save("System");
                }
            }
            return isUpdated;
        }
        private void AddUsertoOffice365(Office365UserInfo o365user)
        {
            var userInfo = Office365UserInfo.GetByUserPrincipleName(o365user.UserPrincipalName);
            if (userInfo != null)
            {
                o365user.Id = userInfo.Id;
            }
            o365user.CreatedBy = MasterConfigHelper.ServiceUser;
            o365user.CreatedDate = DateTime.Now;
            o365user.Save(MasterConfigHelper.ServiceUser);
            CreateUserInReconTable(o365user, MasterConfigHelper.ServiceUser);

        }

        private bool PopulateO365MailBoxSettingsRequest(DomainModel.User user, ProvisioningParamCollection fields, ADSUserInfo adUser)
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            AccessRequest newRequest = new AccessRequest();

            Guid transactionId = Guid.NewGuid();

            try
            {
                newRequest = new AccessRequest();

                newRequest.UserId = user.Id;

                newRequest.EndpointSystemId = this.Endpoint.Id;

                newRequest.RequestStatus = AccessRequestStatus.Granted;
                newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                newRequest.RequestSource = AccessRequestSource.OnBoarding;
                newRequest.ApplicationType = ApplicationType.Office365;
                newRequest.RequesterId = null;
                newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                newRequest.DeactivationDate = user.TerminationDate;
                newRequest.Description = "This request is created automatically for On-Boarding/New Account creation O365MailBoxSettingsRequest.";
                newRequest.Remarks = adUser.sAMAccountName;
                newRequest.CreatedBy = "System";
                newRequest.CreatedDate = DateTime.Now;
                newRequest.TransactionId = transactionId;
                newRequest.AdditionalInfo = "MailBox";
                newRequest.ExecutionTime = DateTime.Now.AddHours(MasterConfigHelper.O365MailBoxWaitTime);
                newRequest.AccountType = AccountType.User;
                requests.Add(newRequest);

                AccessRequest.AddAccessRequest(requests);

                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while saving O365 mailbox settings request.", ex);
                return false;
            }
        }
        public override bool IsUserExistsInOnline(string accountName)
        {
            bool isExists = false;
            ConnectorBase o365Connector = null;
            try
            {
                o365Connector = ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest);
                isExists = o365Connector.CheckOnlineAccountExists(GetUpn(accountName));
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while IsUserExistsInOnline", ex);
                throw ex;
                // isExists = false;
            }
            finally
            {
                o365Connector = null;
            }
            return isExists;
        }
        public override bool IsMailBoxExists(string accountName)
        {
            bool isExists = false;
            ConnectorBase o365Connector = null;
            try
            {
                o365Connector = ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest);
                isExists = o365Connector.MailBoxExists(GetUpn(accountName));
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while IsMailBoxExists", ex);
                throw ex;
                //isExists = false;
            }
            finally
            {
                o365Connector = null;
            }
            return isExists;
        }
        #endregion
    }
}

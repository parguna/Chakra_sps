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
using System.Web.Script.Serialization;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class NOVA : FormExecutorBase
    {
        readonly EamNovaHelper _novaHelper;
        readonly INovaFlinsSubsystem novaFlins;
        readonly INovaFlinsBiSubsystem novaFlinsBi;
        readonly INovaAdSubsystem novaAd;
        readonly JavaScriptSerializer _serializer;
        readonly IdmsLogger __idmsLogger;

        public NOVA(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
            __idmsLogger = idmsLogger;
            _novaHelper = new EamNovaHelper(NovaMatrixType.All, endpoint, null, idmsLogger);
            novaFlins = getNovaFlinsSubsystem(CurrentRequest.AccountType);
            novaFlinsBi = getNovaFlinsBiSubsystem(CurrentRequest.AccountType);
            novaAd = getNovaAdSubsystem(CurrentRequest.AccountType);
            _serializer = new JavaScriptSerializer();
        }

        public bool IsFlinsBIActive
        {
            get
            {
                return this._novaHelper.FlinsBiEndpoint.IsActive;
            }
        }
        INovaFlinsSubsystem getNovaFlinsSubsystem(AccountType accountType)
        {
            switch (accountType)
            {

                case AccountType.Service:
                    return new User.NOVAFlins.Service.NovaFlins(_novaHelper.FlinsEndpoint, this.CurrentRequest, __idmsLogger);
                case AccountType.Shared:
                    return new User.NOVAFlins.Shared.NovaFlins(_novaHelper.FlinsEndpoint, this.CurrentRequest, __idmsLogger);
                case AccountType.Secondary:
                    return new User.NOVAFlins.Secondary.NovaFlins(_novaHelper.FlinsEndpoint, this.CurrentRequest, __idmsLogger);
                default: return new User.NOVAFlins.User.NovaFlins(_novaHelper.FlinsEndpoint, this.CurrentRequest, __idmsLogger);
            }
        }

        INovaFlinsBiSubsystem getNovaFlinsBiSubsystem(AccountType accountType)
        {
            return new User.NovaFlinsBi.User.NovaFlinsBi(_novaHelper.FlinsBiEndpoint, this.CurrentRequest, __idmsLogger);
        }


        INovaAdSubsystem getNovaAdSubsystem(AccountType accountType)
        {
            return new User.NovaAd.User.NovaAd(_novaHelper.ADEndpoint, this.CurrentRequest, __idmsLogger);
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
                            base.Extension(user, fields, CurrentRequest.AccountType, accountRecon.Id);
                            WriteProvisioningLog(LogLevelType.Info, string.Format("Validity of User {0} is extended from {1} to {2} in Active Directory.", accountRecon.AccountName, oldTerminationDate, user.TerminationDate), null);
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

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocNovaRequest novaDetail = null;
            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                novaDetail = (AdHocNovaRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (novaDetail != null)
                {
                    endpointUserId = novaDetail.EndpointUserId;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                isExecutedSuccessfully = Execute(user, novaDetail, CurrentRequest.AdHocAccessRequest.AdhocActionType, CurrentRequest.AccountType);
            }
            else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding)
            {
                return true;
            }

            return isExecutedSuccessfully;
        }

        private bool AmendentRemove(DomainModel.User user, AdHocNovaRequest detail)
        {
            throw new NotImplementedException();
        }

        private bool AmendentAdd(DomainModel.User user, AdHocNovaRequest detail)
        {
            throw new NotImplementedException();
        }

        private bool DeleteAccount(DomainModel.User user, AdHocNovaRequest detail)
        {
            throw new NotImplementedException();
        }


        bool canExecute(DomainModel.User user, AdHocNovaRequest novaDetail, AdHocActionType actionType)
        {
            if (_novaHelper == null)
            {
                throw new Exception("Nova Helper is null.");
            }

            if ((actionType == AdHocActionType.NewAccount || actionType == AdHocActionType.AmendmentAdd)
                && (_novaHelper.NovaMatrixResponseData == null ||
                     _novaHelper.NovaMatrixResponseData.NovaMatrixResponseDetails == null ||
                     _novaHelper.NovaMatrixResponseData.NovaMatrixResponseDetails.Count == 0)
                )
            {
                throw new Exception("No Matrix found for NOVA");

            }

            if (novaDetail == null && actionType != AdHocActionType.DeleteAccount)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception,
                    $"AdHocNovaRequest- Entitlement Detail is null for the user : {user.DisplayName} - {(user.Id)} in NOVA Execute");
                return false;
            }

            if (actionType != AdHocActionType.DeleteAccount && (novaDetail.RequestDetails == null || novaDetail.RequestDetails.Count == 0))
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AdHocNovaRequest.RequestDetails - is null/Empty for the user : {user.DisplayName} - {(user.Id)} in NOVA Execute");
                return false;
            }
            return true;
        }
        bool Execute(DomainModel.User user, AdHocNovaRequest novaDetail, AdHocActionType actionType, AccountType accountType)
        {
            bool success = true;

            SAPEamNovaUserInfo novaUser = null;
            AdHocSAPSharedAccount sharedAccountDetail = null;

            try
            {
                if (!canExecute(user, novaDetail, actionType)) return false;

                if (actionType == AdHocActionType.NewAccount)
                {
                    novaUser = new SAPEamNovaUserInfo();//.GetPrimaryByDIUserId(user.Id, Endpoint.Id, this.CurrentRequest.AccountType);
                    novaUser.EndPointSystemId = this.Endpoint.Id;
                    novaUser.IsActive = true;
                    novaUser.UserId = user.Id;
                    novaUser.IsLocked = false;
                }
                else if (actionType == AdHocActionType.AmendmentAdd || actionType == AdHocActionType.AmendmentRemove || actionType == AdHocActionType.DeleteAccount)
                {
                    novaUser = SAPEamNovaUserInfo.GetByIdIncludeRoles(novaDetail.EndpointUserId);
                    if (novaUser == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception,
                            $"AdHocNovaRequest- No User Accound found in EAM NOVA for the user : {user.DisplayName} - {(user.Id)} in NOVA Execute");
                        return false;
                    }
                }

                List<AccountEntitlementRecon> AccountRecons = DomainModel.EntitlementReconcilation.GetAccountEntitlementRecons(0, user.Id,
                             new List<long> { _novaHelper.FlinsEndpoint.Id,
                                              _novaHelper.FlinsBiEndpoint.Id,
                                              _novaHelper.ADEndpoint.Id }, accountType);

                string accountName = string.Empty;

                if (CurrentRequest.AccountType != AccountType.User && actionType == AdHocActionType.NewAccount && novaDetail != null && novaDetail.SharedAccountDetail != null && !string.IsNullOrWhiteSpace(novaDetail.SharedAccountDetail.AccountName))
                {
                    sharedAccountDetail = novaDetail.AdHocSAPSharedAccountDetail;
                    accountName = novaDetail.SharedAccountDetail.AccountName;
                }

                if (actionType != AdHocActionType.NewAccount)
                {
                    accountName = novaUser.FLINS_UserLogonName;
                }

                AccountReconcilation flinsaccountReconcilation = AccountRecons
                .Where(acc => !acc.AccountRecon.IsDeleted && acc.AccountRecon.AccountType == accountType &&
                        acc.AccountRecon.EndpointSystemId == _novaHelper.FlinsEndpoint.Id && (string.IsNullOrWhiteSpace(accountName) || acc.AccountRecon.AccountName.Equals(accountName,StringComparison.InvariantCultureIgnoreCase)))
                .Select(acc => acc.AccountRecon).FirstOrDefault();


                AccountReconcilation flinsBiaccountReconcilation = AccountRecons
                    .Where(acc => !acc.AccountRecon.IsDeleted && acc.AccountRecon.AccountType == accountType &&
                            acc.AccountRecon.EndpointSystemId == _novaHelper.FlinsBiEndpoint.Id && (string.IsNullOrWhiteSpace(accountName) || acc.AccountRecon.AccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(acc => acc.AccountRecon).FirstOrDefault();

                AccountReconcilation aDaccountReconcilation = AccountRecons
                    .Where(acc => !acc.AccountRecon.IsDeleted && acc.AccountRecon.AccountType == accountType &&
                                    acc.AccountRecon.EndpointSystemId == _novaHelper.ADEndpoint.Id && (string.IsNullOrWhiteSpace(accountName) || acc.AccountRecon.AccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(acc => acc.AccountRecon).FirstOrDefault();

                if (actionType == AdHocActionType.NewAccount || actionType == AdHocActionType.AmendmentAdd)
                {
                    var parameter = new NovaActionParameter()
                    {
                        actionType = actionType,
                        novaUser = novaUser,
                        user = user,
                        adHocNovaRequestDetails = novaDetail,
                        AccountType = accountType
                    };

                    if (actionType == AdHocActionType.NewAccount)
                    {
                        if (accountType != AccountType.User)
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - NewAccount Execute -Not an User Account");

                            if (novaDetail.FlinsAccountEndpointUserId > 0)
                            {
                                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - Execute -  FlinsAccountEndpointUserId-{novaDetail.FlinsAccountEndpointUserId}");

                                var acc = AccountRecons.FirstOrDefault(a => a.AccountRecon.EndpointUserId == novaDetail.FlinsAccountEndpointUserId &&
                                                                            a.AccountRecon.EndpointSystemId == _novaHelper.FlinsEndpoint.Id);

                                parameter.flins_AccountReconcilation = acc.AccountRecon;
                            }
                            else
                            {
                                if (flinsaccountReconcilation != null)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - Execute - Selected Existing Account Recon Id : -{flinsaccountReconcilation.Id} and the account name is {flinsaccountReconcilation.AccountName} ");
                                }
                                else
                                {
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - Execute - No Existing account found in FLINS in the name - {accountName} ");
                                }

                                parameter.flins_AccountReconcilation = flinsaccountReconcilation;
                                //parameter.ad_AccountReconcilation = aDaccountReconcilation;
                                //parameter.flinsBi_AccountReconcilation = flinsBiaccountReconcilation;
                            }
                        }
                        else
                        {


                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - NewAccount Execute - User Account");
                            var id = aDaccountReconcilation != null ? aDaccountReconcilation.EndpointUserId : 0;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - NewAccount Execute -  AD Account EndpointUserId-{id} ");
                            id = flinsaccountReconcilation != null ? flinsaccountReconcilation.EndpointUserId : 0;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - NewAccount Execute -  FLINS Account EndpointUserId-{id}");
                            id = flinsBiaccountReconcilation != null ? flinsBiaccountReconcilation.EndpointUserId : 0;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - NewAccount Execute -  FLINS BI Account EndpointUserId-{id}");

                            parameter.ad_AccountReconcilation = aDaccountReconcilation;
                            parameter.flinsBi_AccountReconcilation = flinsBiaccountReconcilation;
                            parameter.flins_AccountReconcilation = flinsaccountReconcilation;
                        }
                    }
                    else if (actionType == AdHocActionType.AmendmentAdd)
                    {
                        var flinsbiAccount = AccountRecons.FirstOrDefault(a => (novaUser.FLINS_BI_SAPUserInfoId==0 || a.AccountRecon.EndpointUserId == novaUser.FLINS_BI_SAPUserInfoId) &&
                                                                            a.AccountRecon.EndpointSystemId == _novaHelper.FlinsBiEndpoint.Id);

                        var flinsAccount = AccountRecons.FirstOrDefault(a => a.AccountRecon.EndpointUserId == novaUser.FLINS_SAPUserInfoId &&
                                                                                a.AccountRecon.EndpointSystemId == _novaHelper.FlinsEndpoint.Id);

                        var adAccount = AccountRecons.FirstOrDefault(a => (novaUser.ADS_UserInfoId==0 || a.AccountRecon.EndpointUserId == novaUser.ADS_UserInfoId) &&
                                                                                a.AccountRecon.EndpointSystemId == _novaHelper.ADEndpoint.Id);


                        if (IsFlinsBIActive && novaUser.FLINS_BI_SAPUserInfoId > 0 && flinsbiAccount == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find FLINS BI account : EndpointUserId-{novaUser.FLINS_BI_SAPUserInfoId}, AccountName -{novaUser.FLINS_BI_UserLogonName} for AmendAdd in NOVA Account Id:{novaUser.Id}");
                            return false;
                        }
                        else if (IsFlinsBIActive && novaUser.FLINS_BI_SAPUserInfoId == 0 && flinsbiAccount != null)
                        {
                            novaUser.FLINS_BI_SAPUserInfoId = flinsbiAccount.AccountRecon.Id;
                            novaUser.FLINS_BI_UserLogonName = flinsbiAccount.AccountRecon.AccountName;
                        }

                        if (accountType== AccountType.User && novaUser.FLINS_SAPUserInfoId > 0 && flinsAccount == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find FLINS account : EndpointUserId-{novaUser.FLINS_SAPUserInfoId}, AccountName -{novaUser.FLINS_UserLogonName} for AmendAdd in NOVA Account Id:{novaUser.Id}");
                            return false;
                        }

                        if (accountType == AccountType.User && novaUser.ADS_UserInfoId > 0 && adAccount == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find AD account : EndpointUserId-{novaUser.ADS_UserInfoId}, AccountName -{novaUser.AD_UserLogonName} for AmendAdd in NOVA Account Id:{novaUser.Id}");
                            return false;
                        }
                        else if (novaUser.ADS_UserInfoId == 0 && adAccount != null)
                        {
                            novaUser.ADS_UserInfoId = adAccount.AccountRecon.Id;
                            novaUser.AD_UserLogonName = adAccount.AccountRecon.AccountName;
                        }


                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - AmendmentAdd Execute");

                        if (flinsAccount != null)
                        {
                            parameter.flins_AccountReconcilation = flinsAccount.AccountRecon;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - AmendmentAdd Execute -  FLINS Account EndpointUserId-{flinsAccount.AccountRecon.EndpointUserId}");
                        }
                        if (adAccount != null)
                        {
                            parameter.ad_AccountReconcilation = adAccount.AccountRecon;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - AmendmentAdd Execute -  AD Account EndpointUserId-{adAccount.AccountRecon.EndpointUserId}");
                        }
                        if (flinsbiAccount != null)
                        {
                            parameter.flinsBi_AccountReconcilation = flinsbiAccount.AccountRecon;
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"EAM NOVA - AmendmentAdd Execute -  FLINS BI Account EndpointUserId-{flinsbiAccount.AccountRecon.EndpointUserId}");
                        }
                    }

                    novaUser = CreateOrAddAccount(parameter);
                    novaUser = SAPEamNovaUserInfo.GetById(novaUser.Id, true);
                }
                else if (actionType == AdHocActionType.AmendmentRemove || actionType == AdHocActionType.DeleteAccount)
                {
                    var flinsbiAccount = AccountRecons.FirstOrDefault(a => a.AccountRecon.EndpointUserId == novaUser.FLINS_BI_SAPUserInfoId &&
                                                                             a.AccountRecon.EndpointSystemId == _novaHelper.FlinsBiEndpoint.Id);

                    var flinsAccount = AccountRecons.FirstOrDefault(a => a.AccountRecon.EndpointUserId == novaUser.FLINS_SAPUserInfoId &&
                                                                            a.AccountRecon.EndpointSystemId == _novaHelper.FlinsEndpoint.Id);

                    var adAccount = AccountRecons.FirstOrDefault(a => a.AccountRecon.EndpointUserId == novaUser.ADS_UserInfoId &&
                                                                            a.AccountRecon.EndpointSystemId == _novaHelper.ADEndpoint.Id);

                    var action = actionType == AdHocActionType.AmendmentRemove ? "AmendRemove" : "Delete";

                    //if (novaUser.FLINS_BI_SAPUserInfoId > 0 && flinsbiAccount == null)
                    //{
                    //    _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find FLINS BI account : EndpointUserId-{novaUser.FLINS_BI_SAPUserInfoId}, AccountName -{novaUser.FLINS_BI_UserLogonName} for {action} in NOVA Account Id:{novaUser.Id}");
                    //    return false;
                    //}

                    if (novaUser.FLINS_SAPUserInfoId > 0 && flinsAccount == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find FLINS account : EndpointUserId-{novaUser.FLINS_SAPUserInfoId}, AccountName -{novaUser.FLINS_UserLogonName} for {action} in NOVA Account Id:{novaUser.Id}");
                        return false;
                    }

                    //if (novaUser.ADS_UserInfoId > 0 && adAccount == null)
                    //{
                    //    _idmsLogger.WriteLog(LogLevelType.Error, $"Exception : Could not find AD account : EndpointUserId-{novaUser.ADS_UserInfoId}, AccountName -{novaUser.AD_UserLogonName} for {action} in NOVA Account Id:{novaUser.Id}");
                    //    return false;
                    //}

                    if (actionType == AdHocActionType.AmendmentRemove)
                    {
                        novaUser = AmendRemove(new NovaActionParameter()
                        {
                            actionType = actionType,
                            adHocNovaRequestDetails = novaDetail,
                            ad_AccountReconcilation = adAccount != null ? adAccount.AccountRecon : null,
                            flinsBi_AccountReconcilation = flinsbiAccount != null ? flinsbiAccount.AccountRecon : null,
                            flins_AccountReconcilation = flinsAccount != null ? flinsAccount.AccountRecon : null,
                            novaUser = novaUser,
                            user = user
                        });
                    }
                    else
                    {
                        novaUser = DeleteNOVA(new NovaActionParameter()
                        {
                            actionType = actionType,
                            adHocNovaRequestDetails = novaDetail,
                            ad_AccountReconcilation = adAccount != null ? adAccount.AccountRecon : null,
                            flinsBi_AccountReconcilation = flinsbiAccount != null ? flinsbiAccount.AccountRecon : null,
                            flins_AccountReconcilation = flinsAccount != null ? flinsAccount.AccountRecon : null,
                            novaUser = novaUser,
                            user = user
                        });

                    }

                }
                if (novaUser != null && success)
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation -  Before");

                    AddOrUpdateAccountReconciliation(user,novaUser, null, actionType, accountType, sharedAccountDetail);

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation -  After");
                }
            }
            catch (Exception ex)
            {
                success = false;
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA Account Execute : {ex.GetOriginalException()}", ex);
            }
            return success;
        }

        SAPEamNovaUserInfo CreateOrAddAccount(NovaActionParameter novaCreateOrAmendAddParameter)
        {
            try
            {
                if (novaCreateOrAmendAddParameter.actionType == AdHocActionType.NewAccount)
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount NewAccount - User:{novaCreateOrAmendAddParameter.user == null}");
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount NewAccount - User:{novaCreateOrAmendAddParameter.user == null}");


                    if (novaCreateOrAmendAddParameter.novaUser == null)
                    {
                        novaCreateOrAmendAddParameter.novaUser = new SAPEamNovaUserInfo();
                    }

                    if (novaCreateOrAmendAddParameter.novaUser.SAPEamNovaUserRoles == null)
                        novaCreateOrAmendAddParameter.novaUser.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();

                    novaCreateOrAmendAddParameter.novaUser.IsActive = true;
                    novaCreateOrAmendAddParameter.novaUser.IsLocked = false;
                    novaCreateOrAmendAddParameter.novaUser.UserId = novaCreateOrAmendAddParameter.user.Id;

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount FLINS Account exist:{novaCreateOrAmendAddParameter.flins_AccountReconcilation != null}");

                    if (novaCreateOrAmendAddParameter.flins_AccountReconcilation != null)
                    {
                        novaCreateOrAmendAddParameter.novaUser.FLINS_UserLogonName = novaCreateOrAmendAddParameter.flins_AccountReconcilation.AccountName;
                        novaCreateOrAmendAddParameter.novaUser.FLINS_SAPUserInfoId = novaCreateOrAmendAddParameter.flins_AccountReconcilation.EndpointUserId;
                    }

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount FLINS BI Account exist:{novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation != null}");

                    if (novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation != null)
                    {
                        novaCreateOrAmendAddParameter.novaUser.FLINS_BI_UserLogonName = novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation.AccountName;
                        novaCreateOrAmendAddParameter.novaUser.FLINS_BI_SAPUserInfoId = novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation.EndpointUserId;
                    }

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount Ad BI Account exist:{novaCreateOrAmendAddParameter.ad_AccountReconcilation != null}");

                    if (novaCreateOrAmendAddParameter.ad_AccountReconcilation != null)
                    {
                        novaCreateOrAmendAddParameter.novaUser.AD_UserLogonName = novaCreateOrAmendAddParameter.ad_AccountReconcilation.AccountName;
                        novaCreateOrAmendAddParameter.novaUser.ADS_UserInfoId = novaCreateOrAmendAddParameter.ad_AccountReconcilation.EndpointUserId;
                    }
                    novaCreateOrAmendAddParameter.novaUser.EndPointSystemId = Endpoint.Id;
                }
                else if (novaCreateOrAmendAddParameter.actionType == AdHocActionType.AmendmentAdd)
                {
                    novaCreateOrAmendAddParameter.novaUser = SAPEamNovaUserInfo.GetById(novaCreateOrAmendAddParameter.adHocNovaRequestDetails.EndpointUserId, true);

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount FLINS BI Account exist:{novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation != null}");

                    if (IsFlinsBIActive && (novaCreateOrAmendAddParameter.novaUser.FLINS_BI_SAPUserInfoId==0 || string.IsNullOrWhiteSpace( novaCreateOrAmendAddParameter.novaUser.FLINS_BI_UserLogonName)) && novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation != null)
                    {
                        novaCreateOrAmendAddParameter.novaUser.FLINS_BI_UserLogonName = novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation.AccountName;
                        novaCreateOrAmendAddParameter.novaUser.FLINS_BI_SAPUserInfoId = novaCreateOrAmendAddParameter.flinsBi_AccountReconcilation.EndpointUserId;
                    }

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount Ad BI Account exist:{novaCreateOrAmendAddParameter.ad_AccountReconcilation != null}");

                    if ((novaCreateOrAmendAddParameter.novaUser.ADS_UserInfoId == 0 || string.IsNullOrWhiteSpace(novaCreateOrAmendAddParameter.novaUser.AD_UserLogonName)) && novaCreateOrAmendAddParameter.ad_AccountReconcilation != null)
                    {
                        novaCreateOrAmendAddParameter.novaUser.AD_UserLogonName = novaCreateOrAmendAddParameter.ad_AccountReconcilation.AccountName;
                        novaCreateOrAmendAddParameter.novaUser.ADS_UserInfoId = novaCreateOrAmendAddParameter.ad_AccountReconcilation.EndpointUserId;
                    }
                }

                foreach (var detail in novaCreateOrAmendAddParameter.adHocNovaRequestDetails.RequestDetails)
                {

                    novaCreateOrAmendAddParameter.novaUser = createSAPEamNovaUserRoleInfo(novaCreateOrAmendAddParameter.novaUser,
                                                    detail,
                                                    novaCreateOrAmendAddParameter.user, novaCreateOrAmendAddParameter.actionType, novaCreateOrAmendAddParameter.adHocNovaRequestDetails.AdHocSAPSharedAccountDetail, novaCreateOrAmendAddParameter.AccountType);


                }

                if (novaCreateOrAmendAddParameter.novaUser == null || novaCreateOrAmendAddParameter.novaUser.SAPEamNovaUserRoles == null || novaCreateOrAmendAddParameter.novaUser.SAPEamNovaUserRoles.Count == 0)
                {
                    throw new Exception("NOVA User is not created.");
                }

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount NewAccount - Nova User:{novaCreateOrAmendAddParameter.novaUser.DisplayName}");

                novaCreateOrAmendAddParameter.novaUser.Save(_novaHelper.Currentuser);

                bool mailSent = false;
                try
                {
                    if (novaCreateOrAmendAddParameter.actionType == AdHocActionType.NewAccount)
                    {
                        var flinsEp = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                        var isSSoEnabled = EndpointConfiguration.IsSSOEnableForEndpoint(flinsEp.Id) && this.CurrentRequest.AccountType == AccountType.User;
                        mailSent = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, novaCreateOrAmendAddParameter.novaUser, novaCreateOrAmendAddParameter.user, ApplicationType.EAM,isForSAPSSO: isSSoEnabled);
                    }
                    else
                    {
                        var entitlements = novaCreateOrAmendAddParameter.adHocNovaRequestDetails.RequestDetails.Select(r => r.ToString());
                        mailSent = ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(novaCreateOrAmendAddParameter.user, this.CurrentRequest, string.Join(",", entitlements), novaCreateOrAmendAddParameter.novaUser.FLINS_UserLogonName);

                    }
                }
                catch (Exception ee)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, $"Nova - {novaCreateOrAmendAddParameter.actionType.ToString()} Email has not sent successfully:{novaCreateOrAmendAddParameter.novaUser.DisplayName}. The exception is {ee.Message} ",ee);

                }
                if (mailSent)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Nova - {novaCreateOrAmendAddParameter.actionType.ToString()} Email sent successfully:{novaCreateOrAmendAddParameter.novaUser.DisplayName}"); 
                }

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"CreateOrAddAccount NewAccount - Nova User saved successfully:{novaCreateOrAmendAddParameter.novaUser.DisplayName}");

                return novaCreateOrAmendAddParameter.novaUser;
            }
            catch (Exception e)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in CreateOrAddAccount NewAccount", e);

                throw e;
            }
        }

        SAPEamNovaUserInfo DeleteNOVA(NovaActionParameter novaDeleteOrAmendRemoveParameter)
        {
          
            try
            {
                var Novafields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
                var NovaFLINSCommonRoles = string.Empty;
                var NovaFlinsBiCommonRoles = string.Empty;

                if (Novafields!=null && Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
                {
                    NovaFLINSCommonRoles = Novafields["CommonRoles"].Value;
                } 

                if (Novafields != null && Novafields.Any(i => i.Name.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["BWRoles"]!=null &&  !string.IsNullOrWhiteSpace(Novafields["BWRoles"].Value)))
                {
                    NovaFlinsBiCommonRoles = Novafields["BWRoles"].Value;
                }



                if (novaDeleteOrAmendRemoveParameter.novaUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception,
                        $"AdHocNovaRequest- Nova User Account is null for the user : {novaDeleteOrAmendRemoveParameter.user.DisplayName} - {(novaDeleteOrAmendRemoveParameter.user.Id)} in NOVA AmendRemove");
                    throw new Exception($"AdHocNovaRequest - Nova User Account is null for the user : { novaDeleteOrAmendRemoveParameter.user.DisplayName} - { (novaDeleteOrAmendRemoveParameter.user.Id)} in NOVA AmendRemove");
                }

                var list = new List<AdHocNovaRequestDetails>();
                if (novaDeleteOrAmendRemoveParameter.novaUser.SAPEamNovaUserRoles != null)
                {
                    foreach (var d in novaDeleteOrAmendRemoveParameter.novaUser.SAPEamNovaUserRoles)
                    {
                        if (d.IsActive)
                        {
                            list.Add(new AdHocNovaRequestDetails()
                            {
                                CompanyCode = d.Company,
                                DepartmentCode = d.DepartmentBranch,
                                SectionCode = d.Section,
                                NovaMatrixType = d.NovaPositionMatrixType,
                                RoleName = d.RoleName
                            });
                        }
                    }
                }

                foreach (var detail in list)
                {

                    novaDeleteOrAmendRemoveParameter.novaUser = RemoveSAPEamNovaUserRoleInfo(novaDeleteOrAmendRemoveParameter.novaUser,
                                                   detail,
                                                    novaDeleteOrAmendRemoveParameter.user);
                }

                // Removing Common roles of FLINS Account 


                if (novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId > 0 && !string.IsNullOrWhiteSpace(NovaFLINSCommonRoles))
                {
                    novaFlins.NovaFlinsAmendRemove(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId, NovaFLINSCommonRoles);
                }
                // Removing Common Roles of FLINS BI Account
                if (IsFlinsBIActive && novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId > 0 && !string.IsNullOrWhiteSpace(NovaFlinsBiCommonRoles))
                {
                    novaFlinsBi.NovaFlinsBiAmendRemove(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId, NovaFlinsBiCommonRoles);
                }

                // To Delete all EAM roles in FLINS Account
                if (novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId > 0)
                {
                    var flinsAllentitlements = EntitlementReconcilation.GetByEndpointUser(novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId, _novaHelper.FlinsEndpoint.Id);

                    var entitlements = flinsAllentitlements.Where(e => e.IsDeleted == false).Select(e => e.Entitlement).ToList();

                    var eamRoles = _novaHelper.FlinsExculsiveEAM.Intersect(entitlements).ToList();

                    if (eamRoles.Any())
                    {
                        novaFlins.NovaFlinsAmendRemove(novaDeleteOrAmendRemoveParameter.user,
                            novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId,
                            string.Join(",", eamRoles));
                    } 
                }

                // To Delete all EAM roles in FLINS BI Account
                if (IsFlinsBIActive && novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId > 0)
                {
                    var flinsBIAllentitlements = EntitlementReconcilation.GetByEndpointUser(novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId, _novaHelper.FlinsBiEndpoint.Id);

                    var entitlements = flinsBIAllentitlements.Where(e => e.IsDeleted == false).Select(e => e.Entitlement).ToList();

                    var eamRoles = _novaHelper.FlinsBIExculsiveEAM.Intersect(entitlements).ToList();

                    if (eamRoles.Any())
                    {
                        novaFlinsBi.NovaFlinsBiAmendRemove(novaDeleteOrAmendRemoveParameter.user,
                            novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId,
                            string.Join(",", eamRoles));
                    }

                }

                // To Delete all EAM roles in AD Account
                if (novaDeleteOrAmendRemoveParameter.novaUser.ADS_UserInfoId > 0)
                {
                    var adAllentitlements = EntitlementReconcilation.GetByEndpointUser(novaDeleteOrAmendRemoveParameter.novaUser.ADS_UserInfoId, _novaHelper.ADEndpoint.Id);

                    var entitlements = adAllentitlements.Where(e => e.IsDeleted == false).Select(e => e.Entitlement).ToList();

                    var eamRoles = _novaHelper.ADGroupExculsiveEAM.Intersect(entitlements).ToList();

                    if (eamRoles.Any())
                    {
                        novaAd.NovaAdAmendRemove(novaDeleteOrAmendRemoveParameter.user,
                            novaDeleteOrAmendRemoveParameter.novaUser.ADS_UserInfoId,
                            string.Join(",", eamRoles));
                    } 
                }


                // NOVA FLINS User account can be deleted only for non-employee and iff the account has only NOVA FLINS Common roles. If other roles exist, not to be deleted.

                if (CurrentRequest.AccountType == AccountType.User && novaDeleteOrAmendRemoveParameter.user.FullTimeEmployee == false)
                {
                    if (novaDeleteOrAmendRemoveParameter.flins_AccountReconcilation != null)
                    {
                        var entitlements = EntitlementReconcilation.GetAllByAccountReconcilationId(novaDeleteOrAmendRemoveParameter.flins_AccountReconcilation.Id)?.Where(i => i.IsDeleted == false).ToList();

                        if (entitlements != null && entitlements.Count > 0)
                        {
                           
                            if (!string.IsNullOrWhiteSpace(NovaFLINSCommonRoles))
                            { 
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINS entitlements {string.Join(",", entitlements.Select(i => i.Entitlement))}");

                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINS CommonRoles {NovaFLINSCommonRoles}");
                                    var roles = UtilityMethods.GetListFromString(NovaFLINSCommonRoles);
                                    if (roles.Count > 0)
                                    {
                                        var otherThanDefaultRoles = entitlements.Where(i => !roles.Contains(i.Entitlement, StringComparer.InvariantCultureIgnoreCase))?.ToList();
                                        if (otherThanDefaultRoles != null && otherThanDefaultRoles.Count > 0)
                                        {
                                            //Should not delete the account.
                                        }
                                        else
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS Account initiated"); 

                                            //Delete the FLINS Account 

                                            novaFlins.NovaFlinsDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId);

                                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS Account completed");
                                        }
                                    }
                                
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS Account initiated");
                           

                            //Delete the FLINS Account
                            novaFlins.NovaFlinsDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId);

                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS Account completed");
                        }
                    }
                    if (IsFlinsBIActive && novaDeleteOrAmendRemoveParameter.flinsBi_AccountReconcilation != null)
                    {
                        var entitlements = EntitlementReconcilation.GetAllByAccountReconcilationId(novaDeleteOrAmendRemoveParameter.flinsBi_AccountReconcilation.Id)?.Where(i => i.IsDeleted == false).ToList();
                        if (entitlements != null && entitlements.Count > 0)
                        {
                            var novaFields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
                            if (novaFields != null && novaFields.Any(i => novaFields["BWRoles"] !=null && i.Name.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(novaFields["BWRoles"].Value)))
                            {
                                var existingroles = novaFields["BWRoles"].Value;
                                if (!string.IsNullOrWhiteSpace(existingroles))
                                {
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINSBI entitlements {string.Join(",", entitlements.Select(i => i.Entitlement))}");
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINSBI BWRoles {existingroles}");
                                    var roles = UtilityMethods.GetListFromString(existingroles);
                                    if (roles.Count > 0)
                                    {
                                        var otherThanDefaultRoles = entitlements.Where(i => !roles.Contains(i.Entitlement, StringComparer.InvariantCultureIgnoreCase))?.ToList();
                                        if (otherThanDefaultRoles != null && otherThanDefaultRoles.Count > 0)
                                        {
                                            //Should not delete the account.
                                        }
                                        else
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS-BI Account initiated");
                                            //Delete the FLINSBI Account
                                            novaFlinsBi.NovaFlinsBiDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId);

                                            _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS-BI Account completed");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, "No Entitlement found for FLINSBI hence, initiated Delete FLINS-BI Account");
                            //Delete the FLINSBI Account
                            novaFlinsBi.NovaFlinsBiDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId);
                            _idmsLogger.WriteLog(LogLevelType.Info, "No Entitlement found for FLINSBI hence, completed Delete FLINS-BI Account");
                        }
                    }
                }
                if (CurrentRequest.AccountType != AccountType.User)
                {
                    // For other than User account type, NOVA accounts can be deleted.

                    if (novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId > 0)
                    {
                        // Deleteing FLINS account
                        novaFlins.NovaFlinsDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId);
                    }
                    if (IsFlinsBIActive && novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId > 0)
                    {                       
                        novaFlinsBi.NovaFlinsBiDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId);
                    }
                } 
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA Account Amend Remove : {ex.GetOriginalException()}", ex);
                throw ex;
            }
            return novaDeleteOrAmendRemoveParameter.novaUser;
        }
        SAPEamNovaUserInfo AmendRemove(NovaActionParameter novaDeleteOrAmendRemoveParameter)
        {
            try
            {
                if (novaDeleteOrAmendRemoveParameter.novaUser == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception,
                        $"AdHocNovaRequest- Nova User Account is null for the user : {novaDeleteOrAmendRemoveParameter.user.DisplayName} - {(novaDeleteOrAmendRemoveParameter.user.Id)} in NOVA AmendRemove");
                    throw new Exception($"AdHocNovaRequest- Nova User Account is null for the user : {novaDeleteOrAmendRemoveParameter.user.DisplayName} - {(novaDeleteOrAmendRemoveParameter.user.Id)} in NOVA AmendRemove");

                }

                List<string> entitlementsRemovedList = new List<string>();
                foreach (var detail in novaDeleteOrAmendRemoveParameter.adHocNovaRequestDetails.RequestDetails)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info,
                        $"AdHocNovaRequest- Nova AmendRemove :  {detail.ToString()} - Removing started");

                    entitlementsRemovedList.Add(detail.ToString());
                    novaDeleteOrAmendRemoveParameter.novaUser = RemoveSAPEamNovaUserRoleInfo(novaDeleteOrAmendRemoveParameter.novaUser,
                                                    detail,
                                                    novaDeleteOrAmendRemoveParameter.user);

                    _idmsLogger.WriteLog(LogLevelType.Info,
                        $"AdHocNovaRequest- Nova AmendRemove :  {detail.ToString()} - Removing ended");
                } 

                novaDeleteOrAmendRemoveParameter.novaUser.Save(_novaHelper.Currentuser);
                bool mailSent = false;
                try
                {
                    mailSent = ProvisioningUtility.ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(novaDeleteOrAmendRemoveParameter.user, CurrentRequest, string.Join(",", entitlementsRemovedList), novaDeleteOrAmendRemoveParameter.novaUser.FLINS_UserLogonName);
                }
                catch (Exception ee)
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, $"Nova - {novaDeleteOrAmendRemoveParameter.actionType.ToString()} Email has not sent successfully:{novaDeleteOrAmendRemoveParameter.novaUser.DisplayName}. The exception is {ee.Message} ", ee);

                }
                if (mailSent)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Nova - {novaDeleteOrAmendRemoveParameter.actionType.ToString()} Email sent successfully:{novaDeleteOrAmendRemoveParameter.novaUser.DisplayName}");
                }


                if (novaDeleteOrAmendRemoveParameter.novaUser.SAPEamNovaUserRoles.Count() == 0)
                {
                    //Tempstaff
                    if (CurrentRequest.AccountType == AccountType.User)
                    {
                        if (novaDeleteOrAmendRemoveParameter.user.FullTimeEmployee == false)
                        {
                            if (novaDeleteOrAmendRemoveParameter.flins_AccountReconcilation != null)
                            {
                                var entitlements = EntitlementReconcilation.GetAllByAccountReconcilationId(novaDeleteOrAmendRemoveParameter.flins_AccountReconcilation.Id)?.Where(i => i.IsDeleted == false).ToList();
                                if (entitlements != null && entitlements.Count > 0)
                                {
                                    var Novafields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
                                    if (Novafields.Any(i => i.Name.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && Novafields["CommonRoles"] != null && !string.IsNullOrWhiteSpace(Novafields["CommonRoles"].Value)))
                                    {
                                        var existingroles = Novafields["CommonRoles"].Value;
                                        if (!string.IsNullOrWhiteSpace(existingroles))
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINS entitlements {string.Join(",", entitlements.Select(i => i.Entitlement))}");
                                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINS CommonRoles {existingroles}");
                                            var roles = UtilityMethods.GetListFromString(existingroles);
                                            if (roles.Count > 0)
                                            {

                                                var otherThanDefaultRoles = entitlements.Where(i => !roles.Contains(i.Entitlement, StringComparer.InvariantCultureIgnoreCase))?.ToList();
                                                if (otherThanDefaultRoles != null && otherThanDefaultRoles.Count > 0)
                                                {
                                                    //Should not delete the account.
                                                }
                                                else
                                                {
                                                    _idmsLogger.WriteLog(LogLevelType.Info, $"Since no more roles exists for the account {novaDeleteOrAmendRemoveParameter.flins_AccountReconcilation.AccountName}, Delete FLINS Account initiated");
                                                    //Delete the FLINS Account
                                                    novaFlins.NovaFlinsDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId);
                                                    _idmsLogger.WriteLog(LogLevelType.Info, "Delete FLINS Account completed");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (novaDeleteOrAmendRemoveParameter.flinsBi_AccountReconcilation != null)
                        {
                            var entitlements = EntitlementReconcilation.GetAllByAccountReconcilationId(novaDeleteOrAmendRemoveParameter.flinsBi_AccountReconcilation.Id)?.Where(i => i.IsDeleted == false).ToList();
                            if (entitlements != null && entitlements.Count > 0)
                            {
                                var novaFields = NProvisioningField.GetProvisioningDefaults(this.Endpoint.Id, AccountType.User);
                                if (novaFields.Any(i => i.Name.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(novaFields["BWRoles"].Value)))
                                {
                                    var existingroles = novaFields["BWRoles"].Value;
                                    if (!string.IsNullOrWhiteSpace(existingroles))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINSBI entitlements {string.Join(",", entitlements.Select(i => i.Entitlement))}");
                                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"FLINSBI BWRoles {existingroles}");
                                        var roles = UtilityMethods.GetListFromString(existingroles);
                                        if (roles.Count > 0)
                                        {
                                            var otherThanDefaultRoles = entitlements.Where(i => !roles.Contains(i.Entitlement, StringComparer.InvariantCultureIgnoreCase))?.ToList();
                                            if (otherThanDefaultRoles != null && otherThanDefaultRoles.Count > 0)
                                            {
                                                //Should not delete the account.
                                            }
                                            else
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Info, $"Since no more roles exists for the account {novaDeleteOrAmendRemoveParameter.flinsBi_AccountReconcilation.AccountName}, Delete FLINS-BI Account initiated");
                                                //Delete the FLINSBI Account
                                                novaFlinsBi.NovaFlinsBiDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId);
                                                _idmsLogger.WriteLog(LogLevelType.Info, $"Delete FLINS-BI Account completed");

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (CurrentRequest.AccountType != AccountType.User)
                    {
                        if (novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId > 0)
                        {
                            novaFlins.NovaFlinsDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_SAPUserInfoId);
                        }
                        if (novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId > 0)
                        {
                            novaFlinsBi.NovaFlinsBiDeleteAccount(novaDeleteOrAmendRemoveParameter.user, novaDeleteOrAmendRemoveParameter.novaUser.FLINS_BI_SAPUserInfoId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA Account Amend Remove : {ex.GetOriginalException()}", ex);
                throw ex;
            }
            return novaDeleteOrAmendRemoveParameter.novaUser;
        }

        SAPEamNovaUserInfo RemoveSAPEamNovaUserRoleInfo(SAPEamNovaUserInfo novaUser, AdHocNovaRequestDetails detail, DomainModel.User user)
        {
            try
            {
                NovaMatrixResponseDetail matrixResponseDetail = null;
                bool isSuccessFullyAmendInFlins = false, isSuccessFullyAmendInFlinsBI = false, isSuccessFullyAmendInAD = false;
                List<string> flinsCodes = new List<string>();
                List<string> flinsBiCodes = new List<string>();
                List<string> adgroups = new List<string>();
                SAPEamNovaUserRoleInfo existingRole = null;
                if (detail.NovaMatrixType == NovaMatrixType.User)
                {
                    existingRole = novaUser.SAPEamNovaUserRoles.Where(r => r.NovaPositionMatrixType == NovaMatrixType.User && r.RoleName.Trim() == detail.RoleName.Trim() && r.Section == detail.SectionCode && r.Company == detail.CompanyCode
                    ).Select(ee => ee).FirstOrDefault();
                }
                else if (detail.NovaMatrixType == NovaMatrixType.Management)
                {
                    existingRole = novaUser.SAPEamNovaUserRoles.Where(r => r.NovaPositionMatrixType == NovaMatrixType.Management && r.RoleName.Trim() == detail.RoleName.Trim() && r.DepartmentBranch == detail.DepartmentCode
                   ).Select(ee => ee).FirstOrDefault();
                }
                else if (detail.NovaMatrixType == NovaMatrixType.Exceptional)
                {
                    existingRole = novaUser.SAPEamNovaUserRoles.Where(r => r.NovaPositionMatrixType == NovaMatrixType.Exceptional && r.RoleName.Trim() == detail.RoleName.Trim() && r.Company == detail.CompanyCode
                   ).Select(ee => ee).FirstOrDefault();
                }
                if (existingRole == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception,
                        $"AdHocNovaRequest- Nova User Account Role {detail.RoleName} is not exists for the user : {user.DisplayName} - {(user.Id)} in NOVA RemoveSAPEamNovaUserRoleInfo");
                    throw new Exception($"AdHocNovaRequest- Nova User Account Role {detail.RoleName} is not exists for the user : {user.DisplayName} - {(user.Id)} in NOVA RemoveSAPEamNovaUserRoleInfo");
                }

                var flins_existingCode = string.IsNullOrWhiteSpace(existingRole.FLINS_TCodes) ? new List<string>() : existingRole.FLINS_TCodes.Split(',').Where(s => s != null).Select(s => s.Trim()).ToList();
                var flinsBi_existingCode = string.IsNullOrWhiteSpace(existingRole.FLINS_BI_TCodes) ? new List<string>() : existingRole.FLINS_BI_TCodes.Split(',').Where(s => s != null).Select(s => s.Trim()).ToList();
                var ad_existingCode = string.IsNullOrWhiteSpace(existingRole.AD_Groups) ? new List<string>() : existingRole.AD_Groups.Split(',').Where(s => s != null).Select(s => s.Trim()).ToList();

                //flinsCodes = EntitlementReconcilation.GetExistingEntitlementsByEndpointSystemId(_novaHelper.FlinsEndpoint.Id, novaUser.FLINS_SAPUserInfoId, flins_existingCode);
                //flinsBiCodes = EntitlementReconcilation.GetExistingEntitlementsByEndpointSystemId(_novaHelper.FlinsBiEndpoint.Id, novaUser.FLINS_BI_SAPUserInfoId, flinsBi_existingCode);
                //adgroups = EntitlementReconcilation.GetExistingEntitlementsByEndpointSystemId(_novaHelper.ADEndpoint.Id, novaUser.ADS_UserInfoId, ad_existingCode);

                isSuccessFullyAmendInFlins = true;
                if (flins_existingCode != null && flins_existingCode.Count > 0)
                {
                    var roles = string.Join(",", flins_existingCode);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"flins_existingCode: {roles}");
                    isSuccessFullyAmendInFlins = false;
                    isSuccessFullyAmendInFlins = novaFlins.NovaFlinsAmendRemove(user, novaUser.FLINS_SAPUserInfoId, roles);
                }

                isSuccessFullyAmendInFlinsBI = true;
                if (IsFlinsBIActive && flinsBi_existingCode != null && flinsBi_existingCode.Count > 0)
                {
                    var roles = string.Join(",", flinsBi_existingCode);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"flinsBi_existingCode: {roles}");
                    isSuccessFullyAmendInFlinsBI = false;
                    isSuccessFullyAmendInFlinsBI = novaFlinsBi.NovaFlinsBiAmendRemove(user, novaUser.FLINS_BI_SAPUserInfoId, string.Join(",", roles));
                }

                isSuccessFullyAmendInAD = true;
                if (ad_existingCode != null && ad_existingCode.Count > 0)
                {
                    var groups = string.Join(",", ad_existingCode);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"ad_existingCode: {groups}");
                    isSuccessFullyAmendInAD = false;
                    isSuccessFullyAmendInAD = novaAd.NovaAdAmendRemove(user, novaUser.ADS_UserInfoId, groups);
                }

                if (isSuccessFullyAmendInFlins && isSuccessFullyAmendInFlinsBI && isSuccessFullyAmendInAD)
                {
                    novaUser.SAPEamNovaUserRoles.Remove(existingRole);
                }
                return novaUser;
            }
            catch (Exception ex)
            { throw ex; }
        }

        SAPEamNovaUserInfo createSAPEamNovaUserRoleInfo(SAPEamNovaUserInfo novaUser, AdHocNovaRequestDetails detail, DomainModel.User user, AdHocActionType actionType, AdHocSAPSharedAccount sharedAccountDetail, AccountType accountType)
        {
            try
            {
                NovaMatrixResponseDetail matrixResponseDetail = null;
                bool isSuccessFullyAmendInFlins = true, isSuccessFullyAmendInFlinsBI = true, isSuccessFullyAmendInAD = true;
                List<string> flinsCodes = new List<string>();
                List<string> flinsBiCodes = new List<string>();
                List<string> adgroups = new List<string>();




                IEnumerable<NovaMatrixResponseDetail> matrixcollection = _novaHelper.NovaMatrixResponseData.
                                                               NovaMatrixResponseDetails.Where(matrix => !string.IsNullOrWhiteSpace(matrix.Position) && !string.IsNullOrWhiteSpace(detail.RoleName) && matrix.Position.Trim().Equals(detail.RoleName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                                                                           && matrix.novaMatrixType == detail.NovaMatrixType).Select(matrix => matrix);

                #region matrix selection
                //_idmsLogger.WriteLog(LogLevelType.DebugStep, $"matrixcollection count before filter {matrixcollection.Count()}");

                //matrixcollection.ToList().ForEach(m => {
                //    _idmsLogger.WriteLog(LogLevelType.DebugStep, m.ToString());
                //    });

                if (detail.NovaMatrixType == NovaMatrixType.User)
                {
                    matrixResponseDetail = matrixcollection.Where(matrix => matrix.Section != null && matrix.Section.Trim() == detail.SectionCode.Trim() && matrix.Company.Trim() == detail.CompanyCode.Trim()).FirstOrDefault();
                }
                else if (detail.NovaMatrixType == NovaMatrixType.Management)
                {
                    matrixResponseDetail = matrixcollection.Where(matrix => matrix.Department != null && matrix.Department.Trim() == detail.DepartmentCode.Trim()).FirstOrDefault();
                }
                else if (detail.NovaMatrixType == NovaMatrixType.Exceptional)
                {
                    matrixResponseDetail = matrixcollection.Where(matrix => matrix.Company != null && matrix.Company.Trim() == detail.CompanyCode.Trim()).FirstOrDefault();
                }

                if (matrixResponseDetail == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"No relevant matrix found for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. Tha matrix detail is,{detail.ToString()}");
                    throw new Exception($"No relevant matrix found for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. Tha matrix detail is,{detail.ToString()}");
                }

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected Matrix is :{ matrixResponseDetail.ToString()}");

                #endregion matrix selection

                #region AD
                try
                {
                    if (matrixResponseDetail.ADGroup != null && matrixResponseDetail.ADGroup.Count > 0)
                    {
                        isSuccessFullyAmendInAD = false;
                        if (novaUser.ADS_UserInfoId != 0)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - Amend Ad account  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The EndpointUserId is {novaUser.ADS_UserInfoId}");

                            adgroups = EntitlementReconcilation.GetNonExistingEntitlementsByEndpointSystemId(_novaHelper.ADEndpoint.Id, novaUser.ADS_UserInfoId, matrixResponseDetail.ADGroup);
                            if (adgroups != null && adgroups.Count > 0)
                            {
                                isSuccessFullyAmendInAD = novaAd.NovaAdAmendAdd(user, novaUser.ADS_UserInfoId, string.Join(",", adgroups));
                                if (!isSuccessFullyAmendInAD) throw new Exception($"Could not Amend AD Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                            }
                            else
                                isSuccessFullyAmendInAD = true;

                        }
                        else
                        {
                            ADSUserInfo aDSUserInfo = null;

                            if (accountType == AccountType.User)
                            {
                                var message = ProvisioningValidator.IsADDefaultsExists(user);
                                if (!string.IsNullOrWhiteSpace(message))
                                {
                                    throw new Exception($"AD Default provisioning is not exist. Could not Create AD Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                                }
                            }

                            isSuccessFullyAmendInAD = novaAd.NovaAdCreateAccount(user, string.Join(",", matrixResponseDetail.ADGroup), out aDSUserInfo);

                            if (!isSuccessFullyAmendInAD)
                            {
                                __idmsLogger.WriteLog(LogLevelType.Error, $"Could not create AD Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                                throw new Exception($"Could not create AD Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");

                            }
                            novaUser.ADS_UserInfoId = aDSUserInfo.Id;
                            novaUser.AD_UserLogonName = aDSUserInfo.sAMAccountName;
                            _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - Ad account is created  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The account is {aDSUserInfo.sAMAccountName}");

                        }
                    }
                    else
                    {
                        isSuccessFullyAmendInAD = true;
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, $" The AD account is {novaUser.AD_UserLogonName} and EndpointUserId is {novaUser.ADS_UserInfoId}");

                }
                catch (NovaException ex)
                {
                    var accessRequest = AccessRequest.GetById(this.CurrentRequest.Id);
                    accessRequest.Remarks = ex.Message;
                    accessRequest.Save(_novaHelper.Currentuser);
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - AD", ex);
                    throw ex;

                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - AD", ex);
                    throw ex;

                }



                #endregion AD

                #region FLINS
                try
                {
                    if (matrixResponseDetail.FlinsTCode != null && matrixResponseDetail.FlinsTCode.Count > 0)
                    {
                        isSuccessFullyAmendInFlins = false;
                        if (novaUser.FLINS_SAPUserInfoId != 0)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - Amend FLINS aacount  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The EndpointUserId is {novaUser.FLINS_SAPUserInfoId}");
                            flinsCodes = EntitlementReconcilation.GetNonExistingEntitlementsByEndpointSystemId(_novaHelper.FlinsEndpoint.Id, novaUser.FLINS_SAPUserInfoId, matrixResponseDetail.FlinsTCode);
                            if (flinsCodes != null && flinsCodes.Count > 0)
                                isSuccessFullyAmendInFlins = novaFlins.NovaFlinsAmendAdd(user, novaUser.FLINS_SAPUserInfoId, string.Join(",", flinsCodes));
                            else
                                isSuccessFullyAmendInFlins = true;
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - create FLINS aacount  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");

                            _idmsLogger.WriteLog(LogLevelType.Info, $"The sharedAccountDetail is NULL : { sharedAccountDetail == null} in FLINS");

                            if (sharedAccountDetail != null)
                                _idmsLogger.WriteLog(LogLevelType.Info, $"The Account Name is { sharedAccountDetail.AccountName} in FLINS");


                            SAPUserinfo sAPUserinfo = null;

                            isSuccessFullyAmendInFlins = novaFlins.NovaFlinsCreateAccount(user, string.Join(",", matrixResponseDetail.FlinsTCode), sharedAccountDetail, out sAPUserinfo);
                            if (!isSuccessFullyAmendInFlins)
                            {
                                __idmsLogger.WriteLog(LogLevelType.Error, $"Could not create FLINS Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                                throw new Exception($"Could not create FLINS Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                            }
                            novaUser.FLINS_SAPUserInfoId = sAPUserinfo.Id;
                            novaUser.FLINS_UserLogonName = sAPUserinfo.AccountName;

                            _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - FLINS aacount is created  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The account is {sAPUserinfo.AccountName}");

                        }


                    }
                    else
                    {
                        isSuccessFullyAmendInFlins = true;
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $" The FLINS account is {novaUser.FLINS_UserLogonName} and EndpointUserId is {novaUser.FLINS_SAPUserInfoId}");

                }
                catch (NovaException ex)
                {
                    var accessRequest = AccessRequest.GetById(this.CurrentRequest.Id);
                    accessRequest.Remarks = ex.Message;
                    accessRequest.Save(_novaHelper.Currentuser);
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - AD", ex);
                    throw ex;

                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - FLINS", ex);
                    throw ex;

                }



                #endregion FLINS

                #region FLINS BI
                try
                {
                    if (IsFlinsBIActive)
                    {
                        if (matrixResponseDetail.FlinsBiTCode != null && matrixResponseDetail.FlinsBiTCode.Count > 0)
                        {
                            isSuccessFullyAmendInFlinsBI = false;
                            if (novaUser.FLINS_BI_SAPUserInfoId != 0)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - Amend FLINS BI aacount  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The EndpointUserId is {novaUser.FLINS_BI_SAPUserInfoId}");

                                flinsBiCodes = EntitlementReconcilation.GetNonExistingEntitlementsByEndpointSystemId(_novaHelper.FlinsBiEndpoint.Id, novaUser.FLINS_BI_SAPUserInfoId, matrixResponseDetail.FlinsBiTCode);
                                if (flinsBiCodes != null && flinsBiCodes.Count > 0)
                                    isSuccessFullyAmendInFlinsBI = novaFlinsBi.NovaFlinsBiAmendAdd(user, novaUser.FLINS_BI_SAPUserInfoId, string.Join(",", flinsBiCodes));
                                else
                                    isSuccessFullyAmendInFlinsBI = true;
                            }
                            else
                            {
                                SAPUserinfo sAPUserinfo = null;
                                isSuccessFullyAmendInFlinsBI = novaFlinsBi.NovaFlinsBiCreateAccount(user, string.Join(",", matrixResponseDetail.FlinsBiTCode), out sAPUserinfo);
                                if (!isSuccessFullyAmendInFlinsBI)
                                {
                                    __idmsLogger.WriteLog(LogLevelType.Error, $"Could not create FLINS BI Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                                    throw new Exception($"Could not create FLINS BI Account for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount.");
                                }
                                novaUser.FLINS_BI_SAPUserInfoId = sAPUserinfo.Id;
                                novaUser.FLINS_BI_UserLogonName = sAPUserinfo.AccountName;
                                _idmsLogger.WriteLog(LogLevelType.Info, $"createSAPEamNovaUserRoleInfo - FLINS BI aacount is created  for the user : {user.DisplayName} - {(user.Id)} in NOVA CreateAccount. The account is {sAPUserinfo.AccountName}");

                            }


                        }
                        else
                        {
                            isSuccessFullyAmendInFlinsBI = true;
                        }

                        _idmsLogger.WriteLog(LogLevelType.Info, $" The FLINS account is {novaUser.FLINS_BI_UserLogonName} and EndpointUserId is {novaUser.FLINS_BI_SAPUserInfoId}");
                    }
                    else
                    {
                        isSuccessFullyAmendInFlinsBI = true;
                    }

                }
                catch (NovaException ex)
                {
                    var accessRequest = AccessRequest.GetById(this.CurrentRequest.Id);
                    accessRequest.Remarks = ex.Message;
                    accessRequest.Save(_novaHelper.Currentuser);
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - FLINS BI", ex);
                    throw ex;

                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in createSAPEamNovaUserRoleInfo - FLINS BI", ex);
                    throw ex;

                }



                #endregion FLINS BI

                if (isSuccessFullyAmendInFlins && isSuccessFullyAmendInFlinsBI && isSuccessFullyAmendInAD)
                {
                    SAPEamNovaUserRoleInfo detailinfo = new SAPEamNovaUserRoleInfo
                    {
                        EndPointSystemId = this.Endpoint.Id,
                        IsActive = true,
                        FLINS_TCodes = matrixResponseDetail.FlinsTCode == null ? "" : string.Join(",", matrixResponseDetail.FlinsTCode),
                        FLINS_BI_TCodes = matrixResponseDetail.FlinsBiTCode == null ? "" : string.Join(",", matrixResponseDetail.FlinsBiTCode),
                        AD_Groups = matrixResponseDetail.ADGroup == null ? "" : string.Join(",", matrixResponseDetail.ADGroup),
                        RoleName = string.IsNullOrWhiteSpace(matrixResponseDetail.Position) ? string.Join(",", matrixResponseDetail.FlinsTCode) : matrixResponseDetail.Position.Trim(),
                        Company = string.IsNullOrWhiteSpace(matrixResponseDetail.Company) ? "" : matrixResponseDetail.Company.Trim(),
                        DepartmentBranch = string.IsNullOrWhiteSpace(matrixResponseDetail.Department) ? "" : matrixResponseDetail.Department.Trim(),
                        Section = string.IsNullOrWhiteSpace(matrixResponseDetail.Section) ? "" : matrixResponseDetail.Section.Trim(),
                        NovaPositionMatrixType = matrixResponseDetail.novaMatrixType
                    };

                    novaUser.SAPEamNovaUserRoles.Add(detailinfo);

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Nova User RoleInfo Added : {detailinfo.EntitlementName}");

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return novaUser;
        }

        List<string> getSingleRoles(List<SAPNovaTCodes> roles)
        {
            var list = new List<string>();
            if (roles == null || roles.Count == 0) return list;
            roles.ForEach(r =>
            {
                if (r.IsCompositeRole)
                {
                    list.AddRange(r.CompositeRoles);
                }
                else
                {
                    list.Add(r.TCode);
                }
            });
            return list;
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

        bool AddOrUpdateAccountReconciliation(DomainModel.User user, SAPEamNovaUserInfo userinfo, long? AccessRequestId, AdHocActionType actionType, AccountType accountType, AdHocSAPSharedAccount sharedAccountDetail)
        {
            bool success = true;
            try
            {
                if (actionType == AdHocActionType.NewAccount)
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - New Account started");
                    AccountReconcilation accountrecon = null;
                    var FLinsAccount = AccountReconcilation.GetByAccountName(userinfo.FLINS_UserLogonName, _novaHelper.FlinsEndpoint.Id);

                    var existingAccountRecon = AccountReconcilation.GetByAccountName(userinfo.FLINS_UserLogonName, this.Endpoint.Id);
                    if (existingAccountRecon == null)
                    {
                        accountrecon = new AccountReconcilation();
                    }
                    else
                    {
                        accountrecon = existingAccountRecon;
                    }

                    accountrecon.EndpointSystemId = Endpoint.Id;
                    accountrecon.EndpointUserId = userinfo.Id;
                    accountrecon.FullName = FLinsAccount.FullName;
                    accountrecon.DeletedDate = null;
                    accountrecon.AccessRequestId = AccessRequestId;
                    accountrecon.UserId = userinfo.UserId;
                    accountrecon.IsLocked = false;
                    accountrecon.IsDeleted = false;
                    accountrecon.AccountName = userinfo.FLINS_UserLogonName;
                    accountrecon.AccountType = accountType;
                    accountrecon.AccountCreatedDate = DateTime.Now;
                    accountrecon.ValidFromDate = CurrentRequest.ActivationDate.HasValue ? CurrentRequest.ActivationDate.Value : DateTime.Now;
                    accountrecon.ValidToDate = FLinsAccount.ValidToDate; // GetValidTo(user.Id);
                    accountrecon.OffboardingAccountAction = (accountrecon.AccountType == AccountType.User || accountrecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    accountrecon.Save(_novaHelper.Currentuser);

                    EntitlementReconcilation entRecon = null;
                    SAPEamNovaUserRoleInfo userRoleInfo = null;
                    foreach (IEndpointEntitlementInfo entitlement in userinfo.Entitlements)
                    {
                        userRoleInfo = (SAPEamNovaUserRoleInfo)entitlement;

                        var desc = userRoleInfo.EntitlementName.Trim();

                        if (userRoleInfo.NovaPositionMatrixType == NovaMatrixType.Exceptional)
                        {
                            var matrix = _novaHelper.NovaMatrixResponseData.NovaMatrixResponseDetails.FirstOrDefault(m =>
                                                m.novaMatrixType == NovaMatrixType.Exceptional &&
                                                m.Position.Trim().Equals(entitlement.EntitlementName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                );
                            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.JobDescription))
                            {
                                desc = matrix.JobDescription.Trim();
                            }
                        }

                        entRecon = _novaHelper.CreateEntitlementReconcilationShell(accountrecon, EntitlementType.Role, string.Empty);
                        entRecon.Entitlement = userRoleInfo.RoleName.Trim();
                        entRecon.EntitlementDescription = desc;
                        entRecon.EntitlementType = (int)EntitlementType.Role;
                        entRecon.AccountReconcilationId = accountrecon.Id;
                        entRecon.EntitlementId = entitlement.Id;
                        entRecon.EnteredDate = DateTime.Now;
                        entRecon.CreatedDate = DateTime.Now;
                        entRecon.UserId = userinfo.UserId;
                        entRecon.AccessRequestId = CurrentRequest.Id;
                        entRecon.EndpointSystemId = this.Endpoint.Id;
                        entRecon.EndpointType = EndpointType.EAM_NOVA;
                        //EntitlementReconcilation.UpdateException(entRecon, true);
                        entRecon.IsDeleted = false;
                        entRecon.Functions = _serializer.Serialize(userRoleInfo);
                        entRecon.EndpointUserId = userinfo.Id;
                        entRecon.ValidFromDate = CurrentRequest.ActivationDate.HasValue ? CurrentRequest.ActivationDate.Value : DateTime.Now;
                        entRecon.ValidToDate = GetValidTo(user.Id);
                        entRecon.Save(_novaHelper.Currentuser);
                    }

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - New Account ended");

                }

                if (actionType == AdHocActionType.AmendmentAdd)
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentAdd started");

                    var accountrecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(Endpoint.Id, userinfo.Id, userinfo.UserId);
                    List<EntitlementReconcilation> entReconList = EntitlementReconcilation.GetAllByAccountReconcilationId(accountrecon.Id);

                    EntitlementReconcilation entRecon = null;
                    SAPEamNovaUserRoleInfo userRoleInfo = null;
                    foreach (IEndpointEntitlementInfo entitlement in userinfo.Entitlements)
                    {
                        userRoleInfo = (SAPEamNovaUserRoleInfo)entitlement;

                        var desc = userRoleInfo.EntitlementName.Trim();

                        if (userRoleInfo.NovaPositionMatrixType == NovaMatrixType.Exceptional)
                        {
                            var matrix = _novaHelper.NovaMatrixResponseData.NovaMatrixResponseDetails.FirstOrDefault(m =>
                                                m.novaMatrixType == NovaMatrixType.Exceptional &&
                                                m.Position.Trim().Equals(entitlement.EntitlementName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                );
                            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.JobDescription))
                            {
                                desc = matrix.JobDescription.Trim();
                            }
                        }

                        entRecon = null; 

                        if (entReconList != null && entReconList.Count > 0 && entReconList.Any(e => e.Entitlement.Equals(userRoleInfo.RoleName.Trim(), StringComparison.InvariantCultureIgnoreCase) &&
                         e.EntitlementDescription.Equals(userRoleInfo.EntitlementName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            entRecon = entReconList.Where(e => e.Entitlement.Equals(userRoleInfo.RoleName.Trim(), StringComparison.InvariantCultureIgnoreCase) &&
                                                               e.EntitlementDescription.Equals(desc, StringComparison.InvariantCultureIgnoreCase)).Select(e => e).FirstOrDefault();

                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AmendmentAdd - Existing entitlement entRecon is not null- {entRecon != null}");

                            if (entRecon != null)
                                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - Entitlement Id:{entRecon.Id} is taken.");

                        }


                        if (entRecon == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"new entitlement recon created");

                            entRecon = _novaHelper.CreateEntitlementReconcilationShell(accountrecon, EntitlementType.Role, string.Empty);
                            entRecon.Entitlement = userRoleInfo.RoleName.Trim();
                            entRecon.EntitlementType = (int)EntitlementType.Role;
                            entRecon.AccountReconcilationId = accountrecon.Id;
                            entRecon.EntitlementId = entitlement.Id;
                            entRecon.EntitlementDescription = desc;
                            entRecon.EnteredDate = DateTime.Now;
                            entRecon.CreatedDate = DateTime.Now;
                        }


                        entRecon.ValidFromDate = CurrentRequest.ActivationDate.HasValue ? CurrentRequest.ActivationDate.Value : DateTime.Now;
                        entRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        entRecon.EntitlementId = entitlement.Id;
                        entRecon.UserId = userinfo.UserId;
                        entRecon.AccessRequestId = CurrentRequest.Id;
                        entRecon.EndpointSystemId = this.Endpoint.Id;
                        entRecon.EndpointType = EndpointType.EAM_NOVA;
                        // EntitlementReconcilation.UpdateException(entRecon, true);
                        entRecon.IsDeleted = false;
                        entRecon.Functions = _serializer.Serialize(userRoleInfo);
                        entRecon.EndpointUserId = userinfo.Id;
                        entRecon.Save(_novaHelper.Currentuser);
                    }
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentAdd end");
                }
                else if (actionType == AdHocActionType.AmendmentRemove || actionType == AdHocActionType.DeleteAccount)
                {
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentRemove/Delete started");

                    var accountrecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(Endpoint.Id, userinfo.Id, userinfo.UserId);
                    List<EntitlementReconcilation> entReconList = EntitlementReconcilation.GetAllByAccountReconcilationId(accountrecon.Id).Where(e => e.IsDeleted == false).ToList();

                    if (userinfo.Entitlements == null || userinfo.Entitlements.Count == 0 || actionType == AdHocActionType.DeleteAccount)
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - Delete EntitlementRecon start");

                         userinfo.Delete(this.CurrentRequest.Id);

                        ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, userinfo.FLINS_UserLogonName);

                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - Delete EntitlementRecon end");

                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentRemove/Delete setting IsDelete = true in EntitlementRecon");
                        foreach (EntitlementReconcilation entRecon in entReconList)
                        {
                            bool exist = userinfo.SAPEamNovaUserRoles.Exists(e => 
                            {

                                var desc = e.EntitlementName;
                                if (e.NovaPositionMatrixType == NovaMatrixType.Exceptional)
                                {
                                    var matrix = _novaHelper.NovaMatrixResponseData.NovaMatrixResponseDetails.FirstOrDefault(m =>
                                                m.novaMatrixType == NovaMatrixType.Exceptional &&
                                                m.Position.Trim().Equals(e.EntitlementName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                );
                                    if (matrix != null && !string.IsNullOrWhiteSpace(matrix.JobDescription))
                                    {
                                        desc = matrix.JobDescription.Trim();
                                    } 
                                }

                                return desc.Equals(entRecon.EntitlementDescription, StringComparison.InvariantCultureIgnoreCase)
                                 && e.RoleName.Trim().Equals(entRecon.Entitlement.Trim(), StringComparison.InvariantCultureIgnoreCase);
                            });


                            if (!exist)
                            {
                                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - EntitlementName :{entRecon.Entitlement}, Entitlement Recon Id:{entRecon.Id} is deleted.");
                                entRecon.AccessRequestId = this.CurrentRequest.Id;
                                entRecon.IsDeleted = true;
                                entRecon.ValidToDate = DateTime.Now;
                                entRecon.Save(_novaHelper.Currentuser);
                            }
                        }
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentRemove/Delete setting IsDelete = true in EntitlementRecon end"); 
                    }
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AddOrUpdateAccountReconciliation - AmendmentRemove/Delete end");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA AddOrUpdateAccountReconciliation : {ex.GetOriginalException()}", ex);
                return false;
            }

            return success;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return ClearAllAccount(user, EmployeeEvents.Resignation);
        }

        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isExecutedSuccessfully = false;
            string entitlement = string.Empty;
            try
            {
                if (CurrentRequest.AdHocAccessRequest != null)
                {
                    var novaDetail = (AdHocNovaRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    isExecutedSuccessfully = this.Execute(user, novaDetail, this.CurrentRequest.AdHocAccessRequest.AdhocActionType, CurrentRequest.AccountType);
                    entitlement = novaDetail.Entitlement;

                    ProvisioningEmailHelper.SendEmailAfterAttestationRemediation(user, CurrentRequest, entitlement, user.AccountName);
                }
            }
            catch (Exception ex)
            {
                isExecutedSuccessfully = false;
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA RemediateAttestation : {ex.GetOriginalException()}", ex);

            }
            return isExecutedSuccessfully;

        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true; //ClearAllAccount(user, EmployeeEvents.ReOrganisation);
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return ClearAllAccount(user, EmployeeEvents.OUChange);
        }


        bool ClearAllAccount(DomainModel.User user, EmployeeEvents employeeEvents)
        {
            bool success = false;
            try
            {
                List<SAPEamNovaUserInfo> novaUsers = SAPEamNovaUserInfo.GetAllByDIUserId(user.Id, Endpoint.Id,new List<AccountType> {AccountType.User,AccountType.Secondary });
                foreach (var novaUser in novaUsers)
                {
                    try
                    {
                        this.TakeOUSnapshot(user.Id, novaUser, employeeEvents, EndpointType.EAM_NOVA, EntitlementType.Role);
                        novaUser.Delete(this.CurrentRequest.Id);
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in while deleting EAM Nova Account:{novaUser.AccountName} for the User Id - {user.Id} : {ex.GetOriginalException()}", ex);
                    }
                }

                success = true;

            }
            catch (Exception ex)
            {
                success = false;
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception occcured in NOVA  ClearAllAccount(user) for the User Id - {user.Id} : {ex.GetOriginalException()}", ex);
            }
            return success;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared || CurrentRequest.AccountType == AccountType.Secondary)
                    return true;

                var novaConnector = Connector.ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest, this._idmsLogger);

                novaConnector.UndoOffBoardEvent(null, null);
                
                return true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the undo request. {0}", ex.Message), ex);
                return false;
            }
        }

        void SendEmail(string responseGroupNames)
        {
            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            //DiscoveryProcess discoveryProcess = null;
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.NotifySkypeForBusinessNewResponseGroup);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", this.Endpoint.DisplayName);
                //subject.Replace("@@PROCESSNAME@@", discoveryProcess.ProcessName);
                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@ENDPOINTTYPE@@", Endpoint.EndPointSystemType.DisplayName);
                body.Replace("@@ENDPOINTNAME@@", Endpoint.DisplayName);
                body.Replace("@@RESPONSEGROUPNAMES@@", responseGroupNames);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Format("{0}", template.MailTo);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template", ex);
            }
        }

        class NovaActionParameter
        {
            public SAPEamNovaUserInfo novaUser { get; set; }
            public AdHocNovaRequest adHocNovaRequestDetails { get; set; }
            public DomainModel.User user { get; set; }

            public AccountReconcilation flins_AccountReconcilation { get; set; }
            public AccountReconcilation flinsBi_AccountReconcilation { get; set; }
            public AccountReconcilation ad_AccountReconcilation { get; set; }
            public AdHocActionType actionType { get; set; }
            public AccountType AccountType { get; set; }
        }
    }


    interface INovaFlinsSubsystem
    {
        bool NovaFlinsAmendAdd(DomainModel.User user, long endpointUserId, string roles);
        bool NovaFlinsAmendRemove(DomainModel.User user, long endpointUserId, string roles);
        bool NovaFlinsCreateAccount(DomainModel.User user, string roles, AdHocSAPSharedAccount sapDetail, out SAPUserinfo sapUserValue);

        bool NovaFlinsDeleteAccount(DomainModel.User user, long endpointUserId);
    }

    interface INovaFlinsBiSubsystem
    {
        bool NovaFlinsBiAmendAdd(DomainModel.User user, long endpointUserId, string roles);
        bool NovaFlinsBiAmendRemove(DomainModel.User user, long endpointUserId, string roles);
        bool NovaFlinsBiCreateAccount(DomainModel.User user, string roles, out SAPUserinfo sapUserValue);
        bool NovaFlinsBiDeleteAccount(DomainModel.User user, long endpointUserId);
    }

    interface INovaAdSubsystem
    {
        bool NovaAdAmendAdd(DomainModel.User user, long endpointUserId, string groups);
        bool NovaAdAmendRemove(DomainModel.User user, long endpointUserId, string groups);
        bool NovaAdCreateAccount(DomainModel.User user, string roles, out ADSUserInfo sapUserValue);
        bool NovaAdDeleteAccount(DomainModel.User user, long endpointUserId);
    }
}

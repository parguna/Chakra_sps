using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DI.Chakra.Services.ProvisioningProcessor
{
    public class SkypeForBusinessBase : FormExecutorBase
    {
        private const string functionMasked = "Masked";
        private const string userStatusOnline = "OnLine";
        private const string userStatusOnPrem = "On-Premises";
        private string defaulthostProviderString = "SRV:";
        private string[] delimiter = new string[] { ";ext="};
        private string currentUser = MasterConfigHelper.ServiceUser;
        public SkypeForBusinessBase(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                AdhocSkypeForBusinessRequest sfbRequestDetail = null;
                long endpointUserId = 0;

                if (user == null)
                {
                    WriteProvisioningLog(LogLevelType.Warning, "Provisioning failed. User object is null");
                    return false;
                }
                if (CurrentRequest.AdHocAccessRequestId.HasValue)
                {
                    sfbRequestDetail = (AdhocSkypeForBusinessRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    if (sfbRequestDetail != null)
                    {
                        endpointUserId = sfbRequestDetail.EndpointUserId;
                    }
                }
                if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding)
                {
                    isExecutedSuccessfully = ExecuteEnableCommand(fields, user,true);
                }
                else
                {
                    if (CurrentRequest.AdHocAccessRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                    {
                        var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                        var o365 = EndPoint.GetByApplicationType(ApplicationType.Office365);
                        var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, o365.Id);
                        if (adUserInfo == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AD Account does not exists/Locked for the User:{user.AccountName}");
                            return false;
                        }
                        if (o365User == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Office365 User Account does not exists for the User:{user.AccountName}");
                            return false;
                        }
                    }
                   
                    switch (CurrentRequest.AdHocAccessRequest.AdhocActionType)
                    {
                        case AdHocActionType.NewAccount:
                            isExecutedSuccessfully = ExecuteEnableCommand(fields, user, true);
                            break;
                        case AdHocActionType.AmendmentAdd:
                            isExecutedSuccessfully = AmendmentAdd(user, sfbRequestDetail, fields);
                            break;
                        case AdHocActionType.AmendmentRemove:
                            isExecutedSuccessfully = AmendmentRemove(user, sfbRequestDetail, fields);
                            break;
                        case AdHocActionType.DeleteAccount:
                            isExecutedSuccessfully = OffBoardOrDeleteAccount(user, fields);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "provisioning failed", ex);
                isExecutedSuccessfully = false;
            }
            return isExecutedSuccessfully;
        }

        private bool AmendmentAddForResponseGroup(DomainModel.User user, SkypeForBusinessUserInfo sfbUser, AdhocSkypeForBusinessRequest sfbRequestDetail,List<string> entitlements)
        {
            bool isSuccess = false;
            try
            {
                var grps = new List<string>();
                isSuccess = EndpointConnector.AddEntitlement(sfbUser, EntitlementType.ResponseGroup, entitlements);
                //TODO:Command must be updated
                if (isSuccess)
                {
                    if (EndpointConnector.CheckOnlineAccountExists(sfbUser.UserPrincipalName))
                    {
                        var sfbUserInfo = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                        if(sfbUserInfo != null)
                        {
                            sfbUserInfo.Id = sfbUser.Id;
                            #region "Fix :6134
                            if (sfbUser.CreatedDate!= DateTime.MinValue)
                                sfbUserInfo.CreatedDate = sfbUser.CreatedDate; 
                            else
                                sfbUserInfo.CreatedDate = DateTime.Now;
                            #endregion
                            sfbUserInfo.Save(this.currentUser);
                        }
                    }
                    var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbUser.Id, user.Id);
                    var currentEntitlement = EntitlementReconcilation.GetActiveByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)sfbUser.UserType);
                    if(currentEntitlement != null)
                    {
                        if(EndpointConnector.EndpointOnlineUserAccount != null)
                        {
                            var usr = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                            if(usr != null)
                            {
                                currentEntitlement.EntitlementDescription = GetCurrentStatus(usr);
                                currentEntitlement.Save(currentUser);
                            }
                        }
                    }
                    var userGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(sfbUser.Id, this.Endpoint.Id);

                    foreach (var group in sfbRequestDetail.SkypeForBusinessResponseGroups)
                    {
                        SkypeForBusinessUserInfo.InsertUserGroup(sfbUser.EndPointSystemId, sfbUser.Id, group.Id, group.Mask ? 1 : 0);
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Add Endpoint Entitlement {sfbRequestDetail.EntitlementType.ToString()}[{string.Join(",", entitlements)}] for the User:{sfbUser.UserPrincipalName} successfully executed");
                    
                    userGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(sfbUser.Id, this.Endpoint.Id);
                   
                    foreach (var group in sfbRequestDetail.SkypeForBusinessResponseGroups)
                    {
                        if (group.Mask)
                        {
                            grps.Add(group.Name);
                        }
                        AddOrUpdateAgentUri(sfbUser, group.Id, group.Mask ? 1:0);
                        var sfbEntitlement = this.CreateEntitlementReconcilationShell(accountRecon, EntitlementType.ResponseGroup, null);
                        sfbEntitlement.Entitlement = group.Name.Trim();
                        sfbEntitlement.EntitlementDescription = group.Name.Trim();
                        sfbEntitlement.EntitlementId = group.Id;
                        if (group.Mask)
                        {
                            sfbEntitlement.Functions = functionMasked;
                        }
                        else
                        {
                            sfbEntitlement.Functions = "";
                        }
                        AddOrUpdateEntitlementRecord(accountRecon, group.Name, sfbEntitlement);
                    }
                }
                if (grps.Count > 0 && isSuccess)
                {
                    ProvisioningEmailHelper.NotifyMSTeamsDelegation(user, CurrentRequest, string.Join(",", grps), sfbUser.UserPrincipalName, "Add Delegation", this.Endpoint.DisplayName, sfbUser.AnyOneLineURI,"Amendment Add");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Provisioning Add User To ResponseGroup failed for user:{user.AccountName}", ex);
            }
            return isSuccess;
        }
        
        
        private bool AddUserToResponseGroupOnNewAccountCreation(DomainModel.User user, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields)
        {
            bool isSuccess = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var sfbUser = SkypeForBusinessUserInfo.GetByEndpointUserId(sfbRequestDetail.EndpointUserId);
                var entitlements = new List<string>();
                sfbRequestDetail.SkypeForBusinessResponseGroups.ForEach(i => entitlements.Add(i.Name));
                isSuccess = AmendmentAddForResponseGroup(user, sfbUser, sfbRequestDetail, entitlements);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"NewAccount: Add User To ResponseGroup failed for the user{user.AccountName}", ex);
            }
            return isSuccess;
        }
        private bool AmendmentAdd(DomainModel.User user, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields)
        {
            bool isSuccess = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var sfbUser = SkypeForBusinessUserInfo.GetByEndpointUserId(sfbRequestDetail.EndpointUserId);
                if (sfbUser != null)
                {
                    var entitlements = new List<string>();
                    
                    if (sfbRequestDetail.EntitlementType == EntitlementType.ResponseGroup)
                    {
                        sfbRequestDetail.SkypeForBusinessResponseGroups.ForEach(i => entitlements.Add(i.Name));
                        isSuccess = AmendmentAddForResponseGroup(user, sfbUser, sfbRequestDetail, entitlements);
                        entitlements.Clear();
                        sfbRequestDetail.SkypeForBusinessResponseGroups.ForEach(i => entitlements.Add(i.Name));
                    }
                    else if (sfbUser.UserType == EntitlementType.Default && sfbRequestDetail.EntitlementType == EntitlementType.DCC)
                    {
                        isSuccess = AddDCCPolicy(user,sfbUser, sfbRequestDetail.EntitlementType, sfbRequestDetail, currentUser, entitlements, fields);
                    }
                    else if (sfbUser.UserType == EntitlementType.Default && sfbRequestDetail.EntitlementType == EntitlementType.Default)
                    {
                        isSuccess = UpdateSpecialNumber(user, sfbRequestDetail, fields, currentUser, sfbUser, entitlements);
                    }
                    else if ((sfbUser.UserType == EntitlementType.Default && sfbRequestDetail.EntitlementType == EntitlementType.IDD))
                    {
                        isSuccess = AddIDDPolicy(user, sfbRequestDetail, fields, currentUser, sfbUser, entitlements);
                        if (isSuccess)
                        {
                            ProvisioningEmailHelper.NotifyMSTeamIDDRemoval(this.CurrentRequest, user, sfbUser.UserPrincipalName, sfbRequestDetail.DisplayCurrentLineURI, this.Endpoint.DisplayName, "Amendment Add");
                        }
                    }
                    
                    if (isSuccess)
                    {
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, string.Join(",", entitlements), user.AccountName);
                    }
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AmendmentAdd {sfbRequestDetail.EntitlementType.ToString()}", ex);
            }
            return isSuccess;
        }

        
        private bool AddIDDPolicy(User user, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields, string currentUser, SkypeForBusinessUserInfo sfbUser, List<string> entitlements)
        {
            bool isSuccess = false;
            try
            {
                if (EndpointConnector.CheckOnlineAccountExists(sfbUser.UserPrincipalName))
                {
                    var sfbEndpointAccount = this.EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    if (sfbEndpointAccount != null)
                    {
                        var userType = EndpointConnector.GetUserType(sfbEndpointAccount);
                        if (userType != EntitlementType.Default && userType != EntitlementType.None)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AddIDDPolicy Unable to find User Policy Details for the User:{sfbEndpointAccount.UserPrincipalName}.");
                            throw new Exception($"AddIDDPolicy Unable to find User Policy: User Type {userType}  Details for the User:{sfbEndpointAccount.UserPrincipalName}.");
                        }
                        var poolName = string.Empty;
                        var voicePolicy = string.Empty;
                        var voiceRoutingPolicy = string.Empty;
                        var dialPlanPolicy = string.Empty;
                        var userHasGroup = false;
                        if (userType == EntitlementType.Default && sfbRequestDetail.EntitlementType == EntitlementType.IDD)
                        {
                            sfbUser.OnlineVoiceRoutingPolicy = fields.FirstOrDefault(i => i.Name.ToUpper() == $"IDDOnlineVoiceRoutingPolicy".ToUpper())?.Value;
                            dialPlanPolicy = fields.FirstOrDefault(i => i.Name.ToUpper() == $"IDD_DIALPLAN".ToUpper())?.Value;

                            sfbUser.DialPlan = dialPlanPolicy;
                            sfbUser.LineURI = sfbUser.AnyOneLineURI;

                            sfbUser.UserType = EntitlementType.IDD;
                            userHasGroup = EndpointConnector.IsUserPartOfAnyGroup(sfbUser);
                            if (userHasGroup)
                            {
                                sfbUser.TenantDialPlan = fields.FirstOrDefault(i => i.Name.ToUpper() == $"IDDDialPlanWithResponseGroupPolicy".ToUpper())?.Value; ;
                                sfbUser.OnlineVoiceRoutingPolicy = fields.FirstOrDefault(i => i.Name.ToUpper() == $"IDDOnlineVoiceRoutingWithResponseGroupPolicy".ToUpper())?.Value;
                            }
                        }

                        isSuccess = AddPolicy(user, sfbUser, sfbRequestDetail.EntitlementType, sfbRequestDetail, currentUser, entitlements, fields, false,false,userHasGroup);
                        if (isSuccess)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AddIDDPolicy completed for the User:{sfbUser.UserPrincipalName}");
                            entitlements.Add($"{sfbRequestDetail.EntitlementType.ToString()} {sfbUser.LineURI}");
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Applying IDD failed for the User:{sfbUser.AccountName}", ex);
            }

            return isSuccess;
        }
        private bool UpdateSpecialNumber(User user, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields, string currentUser, SkypeForBusinessUserInfo sfbUser, List<string> entitlements)
        {
            bool isSuccess=false;
            try
            {
                var oldLineURI = sfbRequestDetail.DisplayCurrentLineURI;
                var oldLineURIExtension = Convert.ToInt32(sfbRequestDetail.DisplayCurrentLineURI.GetLastDigit(4));
                var LineUriPrefix = fields["LineUriPrefix"].Value.Replace(" ", "");
                var lineUri = $"{LineUriPrefix}{sfbRequestDetail.Extension}";
                _idmsLogger.WriteLog(LogLevelType.Info, $"Reserved Number:Request for Special Number(LineURI):{lineUri}");
                sfbUser.LineURI = lineUri;
                sfbUser.OnPremLineURI = lineUri;
                sfbUser.LineURIExtension = Convert.ToInt32(sfbRequestDetail.Extension);
                if (EndpointConnector.IsUserPropertyExists("LineURI", lineUri, sfbUser.UserPrincipalName))
                {
                    var errormsg = $"Reserved Number:LineURI:{lineUri} already exists for other user.Hence cannot assign this number to User:{sfbUser.AccountName}";
                    _idmsLogger.WriteLog(LogLevelType.Info, errormsg);
                    throw new Exception(errormsg);
                }
                else
                {
                    isSuccess = UpdateLineUri(user, sfbUser, currentUser);
                    if (isSuccess)
                    {
                        if (oldLineURIExtension > 0)
                            SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, oldLineURIExtension, currentUser, false);

                        _idmsLogger.WriteLog(LogLevelType.Info, $"Reserved Number:LineURI:{lineUri} Updating new lineUri completed for the user {sfbUser.AccountName}");
                        entitlements.Add(lineUri);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Reserved Number:LineURI:{lineUri} Updating new lineUri failed for the user {sfbUser.AccountName}");
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Reserved Number failed for the User:{sfbUser.AccountName}.");
            }

            return isSuccess;
        }

        private bool AddDCCPolicy(DomainModel.User user, SkypeForBusinessUserInfo userInfo, EntitlementType toType, AdhocSkypeForBusinessRequest sfbRequestDetail, string currentUser, List<string> entitlements, ProvisioningParamCollection fields)
        {
            bool isPolicyAdded = false;
            try
            {
                //if (string.IsNullOrWhiteSpace(sfbRequestDetail.Extension))
                //{
                //    throw new Exception("Extension number is empty");
                //}
                if (EndpointConnector.CheckOnlineAccountExists(userInfo.AccountName))
                {
                    var sfbAccount = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    var usrType = EndpointConnector.GetUserType(sfbAccount);
                    if (EndpointConnector.IsUserPartOfAnyGroup(userInfo))
                    {
                        throw new Exception("User has responsegroup. Cannot apply for DCC.");
                    }

                    //if (usrType == EntitlementType.IDD)
                    //{
                    //    throw new Exception("User has responsegroup. Cannot apply for DCC.");
                    //}

                    var dccLineUri = string.Empty;// fields["DefaultDCC"].Value.Replace(" ", "");
                    var lineUri = string.Empty;//$"{dccLineUri}{sfbRequestDetail.Extension}";

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Amendment Add: Request for DCC(LineURI). Before Removal LineURI: {userInfo.AnyOneLineURI}");

                    userInfo.LineURI = lineUri;
                    userInfo.OnPremLineURI = lineUri;
                    userInfo.LineURIExtension = null;

                    //userInfo.VoicePolicy = fields.FirstOrDefault(i => i.Name.ToUpper() == "DCC_VOICEPOLICY")?.Value;
                    userInfo.OnlineVoiceRoutingPolicy = fields.FirstOrDefault(i => i.Name.ToUpper() == "DCC_VOICEROUTINGPOLICY")?.Value;
                    //userInfo.DialPlan = fields.FirstOrDefault(i => i.Name.ToUpper() == "DCC_DIALPLAN")?.Value;
                    //userInfo.RegistrarPool = fields.FirstOrDefault(i => i.Name.ToUpper() == "RegistrarPool2".ToUpper())?.Value;

                    entitlements.Add(userInfo.LineURI);

                    var moveActionRequired = false;
                    //if (sfbAccount != null && sfbAccount.HostingProvider.Equals(defaulthostProviderString, StringComparison.InvariantCultureIgnoreCase) && (!string.IsNullOrWhiteSpace(sfbAccount.RegistrarPool) && sfbAccount.RegistrarPool.Equals(userInfo.RegistrarPool, StringComparison.InvariantCultureIgnoreCase)))
                    //{
                    //    WriteProvisioningLog(LogLevelType.Info, $"User:{userInfo.UserPrincipalName} already in {userInfo.RegistrarPool}. Hence Move command is not required");
                    //    moveActionRequired = false;
                    //}
                    userInfo.IsRequireLineUriAssignment = false;
                    isPolicyAdded = EndpointConnector.ApplyNewPolicy(userInfo, toType, moveActionRequired);
                    if (isPolicyAdded)
                    {
                        var sfb = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                        if (sfb != null)
                        {
                            userInfo.HostingProvider = sfb.HostingProvider;
                        }
                        var userGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Add Endpoint Entitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{userInfo.UserPrincipalName}  successfully executed");
                        if (userGroups.UserType == EntitlementType.Default)
                        {
                            SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, Convert.ToInt32(userGroups.LineURIExtension), currentUser, false);
                        }
                        
                        userInfo.UserType = toType;
                        userInfo.Save(currentUser);

                        var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, userInfo.Id, user.Id);
                        var sfbEntitlement = this.CreateEntitlementReconcilationShell(accountRecon, sfbRequestDetail.EntitlementType, null);
                        sfbEntitlement.EndpointUserId = userInfo.Id;
                        sfbEntitlement.Entitlement = userInfo.LineURI;
                        sfbEntitlement.EntitlementId = userInfo.Id;
                        sfbEntitlement.EntitlementDescription = userStatusOnline;

                        var currentEntitlement = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, (int)userGroups.UserType);
                        if (currentEntitlement != null)
                        {
                            foreach (var item in currentEntitlement)
                            {
                                try
                                {
                                    item.Delete();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        var entRecon = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)sfbEntitlement.EntitlementType);
                        if (entRecon == null)
                        {
                            entRecon = sfbEntitlement;
                            entRecon.CreatedDate = DateTime.Now;
                            entRecon.CreatedBy = "System";
                            entRecon.AccessRequestId = this.CurrentRequest.Id;
                            entRecon.ValidFromDate = sfbEntitlement.ValidFromDate;
                            entRecon.ValidToDate = sfbEntitlement.ValidToDate;
                            entRecon.Save(currentUser);
                        }
                        else
                        {
                            entRecon.ValidFromDate = sfbEntitlement.ValidFromDate;
                            entRecon.ValidToDate = sfbEntitlement.ValidToDate;
                            entRecon.AccessRequestId = this.CurrentRequest.Id;
                            entRecon.LastModifiedDate = DateTime.Now;
                            entRecon.LastModifiedBy = "System";
                            entRecon.Entitlement = sfbEntitlement.Entitlement;
                            entRecon.EntitlementId = sfbEntitlement.EntitlementId;
                            entRecon.EntitlementDescription = sfbEntitlement.EntitlementDescription;
                            entRecon.IsDeleted = false;
                            entRecon.DeletedDate = null;
                            entRecon.LastAttestedOn = null;
                            entRecon.Save(currentUser);
                        }
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AmendmentADD Update DCC/DCCS/IDD LineURI in User and SAPHRMS");
                        string prefixTelephone = string.Empty;
                        if (!string.IsNullOrWhiteSpace(userInfo.AnyOneLineURI))
                        {
                            var data = userInfo.AnyOneLineURI.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            if(data != null && data.Length > 0)
                                prefixTelephone = data[0];
                        }
                        
                        UpdateSAPHRMSAttributes(user, prefixTelephone, string.Empty,true);
                        UpdateDIUserTelephoneNumber(user, prefixTelephone);
                    }
                }
            }
            catch (Exception ex)
            {
                isPolicyAdded = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "", ex);
            }
            return isPolicyAdded;
        }
        private bool AddPolicy(DomainModel.User user, SkypeForBusinessUserInfo userInfo, EntitlementType toType, AdhocSkypeForBusinessRequest sfbRequestDetail, string currentUser, List<string> entitlements, ProvisioningParamCollection fields, bool UpdateLineUri=false,bool allowMove=false,bool IsMultiplepolicy=false,bool userHasGroup = false)
        {
            bool isPolicyAdded = false;
            try
            {
                isPolicyAdded = EndpointConnector.ApplyNewPolicy(userInfo, toType, allowMove);
                if (isPolicyAdded)
                {
                    SkypeForBusinessUserInfo sfb = null;
                    if (toType== EntitlementType.IDD)
                    {
                        sfb = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    }
                    else
                    {
                        sfb = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    }
                    userInfo.HostingProvider = sfb?.HostingProvider;

                    var userGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Add Endpoint Entitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{user.UserName}  successfully executed");

                    userInfo.UserType = toType;
                    userInfo.Save(currentUser);
                    var userStatus = GetCurrentStatus(userInfo);

                    var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, userInfo.Id, user.Id);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Before Delete:{userGroups.UserType}");
                    var currentEntitlement = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)userGroups.UserType);
                    if (currentEntitlement != null)
                    {
                        currentEntitlement.Delete();
                    }

                    var sfbEntitlement = this.CreateEntitlementReconcilationShell(accountRecon, sfbRequestDetail.EntitlementType, null);
                    sfbEntitlement.Entitlement = userInfo.AnyOneLineURI;
                    sfbEntitlement.EntitlementDescription = userStatus;
                    sfbEntitlement.EntitlementId = userInfo.Id;
                        
                    var entRecon = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)sfbEntitlement.EntitlementType);
                    if (entRecon == null)
                    {
                        entRecon = sfbEntitlement;
                        entRecon.CreatedDate = DateTime.Now;
                        entRecon.CreatedBy = "System";
                        entRecon.AccessRequestId = this.CurrentRequest.Id;
                        entRecon.ValidFromDate = sfbEntitlement.ValidFromDate;
                        entRecon.ValidToDate = sfbEntitlement.ValidToDate;
                        entRecon.Save(currentUser);
                    }
                    else
                    {
                        entRecon.AccessRequestId = this.CurrentRequest.Id;
                        entRecon.LastModifiedDate = DateTime.Now;
                        entRecon.LastModifiedBy = "System";
                        entRecon.Entitlement = sfbEntitlement.Entitlement;
                        entRecon.EntitlementId = sfbEntitlement.EntitlementId;
                        entRecon.EntitlementDescription = sfbEntitlement.EntitlementDescription;
                        entRecon.IsDeleted = false;
                        entRecon.DeletedDate = null;
                        entRecon.LastAttestedOn = null;
                        entRecon.ValidFromDate = sfbEntitlement.ValidFromDate;
                        entRecon.ValidToDate = sfbEntitlement.ValidToDate;
                        entRecon.Save(currentUser);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                isPolicyAdded = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "", ex);
            }
            return isPolicyAdded;
        }

        private bool UpdateLineUri(DomainModel.User user, SkypeForBusinessUserInfo sfbUser, string currentUser)
        {
            bool isUpdated = false;
            try
            {
                if (string.IsNullOrWhiteSpace(sfbUser.LineURI) || !sfbUser.LineURIExtension.HasValue)
                {
                    throw new Exception("Extension number is empty");
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"Request for Special Number(LineURI):{sfbUser.LineURI}");
                isUpdated = EndpointConnector.UpdateEndpointUserDetails(sfbUser);
                if (isUpdated)
                {
                    var telephoneNumber = sfbUser.AnyOneLineURI.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[0];
                    UpdateSAPHRMSAttributes(user, telephoneNumber, sfbUser.LineURIExtension.Value.ToString());
                    UpdateDIUserTelephoneNumber(user, telephoneNumber);

                    SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, sfbUser.LineURIExtension.Value, currentUser, true);
                    sfbUser.Save(currentUser);
                    var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbUser.Id,user.Id);

                    var currentEntitlement = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, (int)sfbUser.UserType);
                    if (currentEntitlement != null)
                    {
                        foreach (var item in currentEntitlement)
                        {
                            try
                            {
                                if(item.IsDeleted)
                                    item.Delete();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    var entitlement = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementType(this.Endpoint.Id, accountRecon.Id, (int)sfbUser.UserType);
                
                    if(entitlement == null)
                    {
                        entitlement = CreateEntitlementReconcilationShell(accountRecon, sfbUser.UserType);
                        entitlement.CreatedBy = currentUser;
                        entitlement.AccessRequestId = this.CurrentRequest.Id;
                        entitlement.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        entitlement.IsDeleted = false;
                        entitlement.DeletedDate = null;
                        entitlement.LastModifiedDate = DateTime.Now;
                    }
                    entitlement.Entitlement = sfbUser.AnyOneLineURI;
                    entitlement.EntitlementDescription = GetCurrentStatus(sfbUser);
                    entitlement.Save(currentUser);
                    
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateLineUri failed for user:{user.UserName}");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateLineUri for User:{user.UserName} failed");
            }
            return isUpdated;
        }
        private void AddOrUpdateEntitlementRecord(AccountReconcilation accountRecon, string entitlement, EntitlementReconcilation sfbEntitlement)
        {
            var entRecon = EntitlementReconcilation.GetByEntitlement(accountRecon.EndpointSystemId, accountRecon.Id, entitlement.Trim(), (int)sfbEntitlement.EntitlementType);
            if (entRecon == null)
            {
                entRecon = sfbEntitlement;
                entRecon.CreatedDate = DateTime.Now;
                entRecon.CreatedBy = "System";
                entRecon.AccessRequestId = this.CurrentRequest.Id;
                entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                entRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                entRecon.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {
                entRecon.EndpointUserId = sfbEntitlement.EndpointUserId;
                entRecon.Functions = sfbEntitlement.Functions;
                entRecon.AccessRequestId = this.CurrentRequest.Id;
                entRecon.LastModifiedDate = DateTime.Now;
                entRecon.LastModifiedBy = "System";
                entRecon.Entitlement = sfbEntitlement.Entitlement;
                entRecon.EntitlementId = sfbEntitlement.EntitlementId;
                entRecon.EntitlementDescription = sfbEntitlement.EntitlementDescription;
                entRecon.IsDeleted = false;
                entRecon.DeletedDate = null;
                entRecon.LastAttestedOn = null;
                entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                entRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                entRecon.Save(MasterConfigHelper.ServiceUser);
            }
        }
        private bool RemoveUserFromResponseGroup(DomainModel.User user, SkypeForBusinessUserInfo sfbUser, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields, List<string> entitlements, string currentUser)
        {
            bool isSuccess = false;
            try
            {
                var onlyMaskingGroups = sfbRequestDetail.SkypeForBusinessResponseGroups.Where(i => i.Mask).Select(i => i.Name).ToList();
                
                if (entitlements.Count > 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Remove ResponseGroup:{string.Join(",", entitlements)} for the User:{user.AccountName}");
                    if (EndpointConnector.RemoveResponseGroupMembers(sfbUser, sfbRequestDetail.EntitlementType, entitlements))
                    {
                        sfbUser.Save(currentUser);
                        if (onlyMaskingGroups.Count > 0)
                        {
                            ProvisioningEmailHelper.NotifyMSTeamsDelegation(user, this.CurrentRequest, string.Join(",", onlyMaskingGroups), sfbUser.UserPrincipalName, "Remove Delegation", this.Endpoint.DisplayName, sfbUser.AnyOneLineURI,"Amendment Remove");
                        }
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Remove Endpoint Entitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{sfbUser.UserPrincipalName}  successfully executed");
                        var userGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(sfbUser.Id, this.Endpoint.Id);
                        var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbUser.Id, user.Id);
                        #region Remove User from Responsegroup
                        foreach (var group in sfbRequestDetail.SkypeForBusinessResponseGroups)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, $"Remove Member from ResponseGroup:{group.Name} for the User:{user.AccountName} started");

                            if (userGroups.SkypeForBusinessResponseGroup.Exists(i => i.Name.ToUpper() == group.Name.ToUpper()))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Removing User:{sfbUser.UserPrincipalName} from Response Group: {group.Name}");
                                var entitlement = EntitlementReconcilation.GetActiveReordByUserIdWithEntitlement(sfbUser.Id, this.Endpoint.Id, EntitlementType.ResponseGroup, group.Name);
                                if (entitlement != null)
                                {
                                    entitlement.Delete();
                                }
                                SkypeForBusinessUserInfo.DeleteSkypeForBusinessUserFroups(sfbUser.Id, group.Id, this.Endpoint.Id);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Removed User:{sfbUser.UserPrincipalName} from Response Group: {group.Name}");
                            }
                            _idmsLogger.WriteLog(LogLevelType.Exception, $"RemoveUserFromResponseGroup:Remove Member from ResponseGroup:{group} for the User:{user.AccountName} completed");
                        }
                        isSuccess = true;
                        #endregion
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
                else
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "RemoveUserFromResponseGroup", ex);
            }
            return isSuccess;
        }
        private bool RemoveUserDCCLineURI(DomainModel.User user, SkypeForBusinessUserInfo sfbUser, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields, string currentUser)
        {
            bool isSuccess = false;
            try
            {
                var currentSFBUserObj = SkypeForBusinessUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                var LineUriPrefix = fields["LineUriPrefix"].Value.Replace(" ", "");
                var telephoneNumber = string.Empty;
                AssignRandomLineURI(user, sfbUser, LineUriPrefix, currentUser, ref telephoneNumber);
                sfbUser.TenantDialPlan = fields["DefaultTeamDialPlanPolicy"].Value;
                sfbUser.OnlineVoiceRoutingPolicy = fields["DefaultOnlineVoiceRoutingPolicy"].Value;
                sfbUser.TeamsUpgradePolicy = fields["TeamsUpgradePolicy"].Value;
                sfbUser.IsRequireLineUriAssignment = true;
                if (EndpointConnector.RemoveDCCPolicy(sfbUser, sfbRequestDetail.EntitlementType))
                {
                    sfbUser.VoicePolicy = null;
                    sfbUser.VoiceRoutingPolicy = null;
                    sfbUser.DialPlan = null;
                    SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, sfbUser.LineURIExtension.Value, currentUser, true);
                    if (EndpointConnector.CheckOnlineAccountExists(sfbUser.UserPrincipalName))
                    {
                        #region Update details
                        var currentSFBInfo = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                        if(currentSFBInfo != null 
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.LineURI) && !string.IsNullOrWhiteSpace(currentSFBInfo.EnterpriseVoiceEnabled)
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.TenantDialPlan)
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.OnlineVoiceRoutingPolicy)
                            && currentSFBInfo.LineURI.Contains(sfbUser.LineURI, StringComparison.InvariantCultureIgnoreCase)
                            && currentSFBInfo.EnterpriseVoiceEnabled.Trim().ToUpper() == "True".ToUpper()
                            && currentSFBInfo.TenantDialPlan.Trim().ToUpper() == sfbUser.TenantDialPlan.ToUpper()
                            && currentSFBInfo.OnlineVoiceRoutingPolicy.Trim().ToUpper() == sfbUser.OnlineVoiceRoutingPolicy.ToUpper()
                        )
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Account Online Status After New Assignment: UserPrincipalName - {currentSFBInfo.UserPrincipalName} LineURI - {currentSFBInfo.LineURI} EnterpriseVoiceEnabled - {currentSFBInfo.EnterpriseVoiceEnabled} TenantDialPlan - {currentSFBInfo.TenantDialPlan} OnlineVoiceRoutingPolicy - {currentSFBInfo.OnlineVoiceRoutingPolicy}");

                            UpdateSAPHRMSAttributes(user, telephoneNumber, sfbUser.LineURIExtension.Value.ToString());
                            UpdateDIUserTelephoneNumber(user, telephoneNumber);
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Endpoint Entitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{currentSFBInfo.UserPrincipalName}  successfully executed");
                            sfbUser.UserType = EntitlementType.Default;
                            sfbUser.HostingProvider = currentSFBInfo.HostingProvider;
                            sfbUser.Save(currentUser);
                            string currentStatus = GetCurrentStatus(currentSFBInfo);
                            var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbUser.Id, user.Id);
                            var entitlementOld = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)EntitlementType.DCC);

                            if (entitlementOld != null && entitlementOld.IsDeleted==false)
                            {
                                entitlementOld.Delete();
                            }
                            var exisitingStandardAccount = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)EntitlementType.Default);
                            if(exisitingStandardAccount == null)
                            {
                                exisitingStandardAccount = this.CreateEntitlementReconcilationShell(accountRecon, EntitlementType.Default, null);
                            }else
                            {
                                exisitingStandardAccount.ValidFromDate = CurrentRequest.ActivationDate;
                                exisitingStandardAccount.ValidToDate = GetValidTo(CurrentRequest.UserId);
                                exisitingStandardAccount.IsDeleted = false;
                                exisitingStandardAccount.DeletedDate = null;
                            }
                            exisitingStandardAccount.EndpointUserId = sfbUser.Id;
                            exisitingStandardAccount.EntitlementId = sfbUser.Id;
                            exisitingStandardAccount.Entitlement = sfbUser.AnyOneLineURI;
                            exisitingStandardAccount.EntitlementDescription = currentStatus;
                            exisitingStandardAccount.Save(currentUser);
                            isSuccess = true;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Amendment Remove: RemoveUserDCCLineURI for the User:{sfbUser.UserPrincipalName}", ex);
            }
            return isSuccess;
        }

        private string GetCurrentStatus(SkypeForBusinessUserInfo currentSFBInfo)
        {
            //var currenStatus = userStatusOnline;

            //if(string.IsNullOrWhiteSpace(currentSFBInfo.HostingProvider) || !currentSFBInfo.HostingProvider.Equals(defaulthostProviderString, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    currenStatus = userStatusOnline;
            //}
            return userStatusOnline;
        }

        
        private bool RemoveUserIDDPolicy(DomainModel.User user, SkypeForBusinessUserInfo sfbUser, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields, List<string> entitlements, string currentUser)
        {
            bool isSuccess = false;
            try
            {

                string voicePolicy, voiceRoutingPolicy, dialPlanPolicy;
                voicePolicy = voiceRoutingPolicy = dialPlanPolicy = string.Empty;
                var entitlementDetails = EntitlementReconcilation.GetByUserEntitlementByType(sfbUser.Id, this.Endpoint.Id);
                var userType = EntitlementType.Default;
                sfbUser.TenantDialPlan = fields["DefaultTeamDialPlanPolicy"].Value;
                sfbUser.OnlineVoiceRoutingPolicy = fields["DefaultOnlineVoiceRoutingPolicy"].Value;
                sfbUser.TeamsUpgradePolicy = fields["TeamsUpgradePolicy"].Value;
                sfbUser.LineURI = sfbUser.AnyOneLineURI;
                sfbUser.OnPremLineURI = sfbUser.LineURI;
                if (EndpointConnector.RemoveIDDPolicy(sfbUser, sfbRequestDetail.EntitlementType))
                {
                    sfbUser.UserType = userType;
                    if (EndpointConnector.CheckOnlineAccountExists(sfbUser.UserPrincipalName))
                    {
                        var currentSFBInfo = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Remove Endpoint Entitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{user.AccountName}  successfully executed");
                        sfbUser.HostingProvider = currentSFBInfo.HostingProvider;
                        sfbUser.Save(currentUser);
                        var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbUser.Id, user.Id);
                        var userEntitlements = EntitlementReconcilation.GetByEntitlementTypeByExcludeGroup(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId);
                        foreach (var entitlementOld in userEntitlements)
                        {
                            if (entitlementOld != null)
                            {
                                entitlementOld.Delete();
                            }
                        }

                        var exisitingStandardAccount = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)sfbUser.UserType);
                        if (exisitingStandardAccount == null)
                        {
                            exisitingStandardAccount = this.CreateEntitlementReconcilationShell(accountRecon, sfbUser.UserType, null);
                        }
                        else
                        {
                            exisitingStandardAccount.ValidFromDate = CurrentRequest.ActivationDate;
                            exisitingStandardAccount.ValidToDate = GetValidTo(CurrentRequest.UserId);
                            exisitingStandardAccount.IsDeleted = false;
                            exisitingStandardAccount.DeletedDate = null;
                        }
                        exisitingStandardAccount.EntitlementId = sfbUser.Id;
                        exisitingStandardAccount.Entitlement = sfbUser.AnyOneLineURI;
                        exisitingStandardAccount.EntitlementDescription = GetCurrentStatus(sfbUser);
                        exisitingStandardAccount.Save(currentUser);
                        isSuccess = true;
                    }

                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "RemoveUserLineURI", ex);
            }
            return isSuccess;
        }
        private bool AmendmentRemove(DomainModel.User user, AdhocSkypeForBusinessRequest sfbRequestDetail, ProvisioningParamCollection fields)
        {
            bool isSuccess = false;
            try
            {
                string entitlementEmailstring = string.Empty;
                var currentUser = MasterConfigHelper.ServiceUser;
                var sfbUser = SkypeForBusinessUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                var entitlements = new List<string>();
                if (sfbRequestDetail.EntitlementType == EntitlementType.ResponseGroup)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{sfbUser.UserPrincipalName} started");
                    entitlements.AddRange(sfbRequestDetail.SkypeForBusinessResponseGroups.Select(i=>i.Name).ToList());
                    isSuccess = RemoveUserFromResponseGroup(user, sfbUser, sfbRequestDetail, fields, entitlements, currentUser);
                    if (isSuccess)
                    {
                        entitlements.Clear();
                        sfbRequestDetail.SkypeForBusinessResponseGroups.ForEach(i => entitlements.Add(i.Name));
                    }
                }
                else if (sfbRequestDetail.EntitlementType == EntitlementType.DCC)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{sfbUser.AccountName} started");
                    entitlements.Add($"{EntitlementType.DCC.ToString()} - {sfbUser.LineURI} policy removed");
                    isSuccess = RemoveUserDCCLineURI(user, sfbUser, sfbRequestDetail, fields, currentUser);
                }
                ////No More DCCS Support
                //else if(sfbRequestDetail.EntitlementType == EntitlementType.DCCS)
                //{
                //    _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{sfbUser.UserPrincipalName} started");
                //    isSuccess = RemoveUserDCCSPolicy(user, sfbUser, sfbRequestDetail, fields, currentUser);
                //    if (isSuccess)
                //    {
                //        entitlements.Add($"{EntitlementType.DCCS.ToString()} policy removed");
                //    }
                //}
                else if (sfbRequestDetail.EntitlementType == EntitlementType.IDD)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement{sfbRequestDetail.EntitlementType.ToString()} for the User:{sfbUser.UserPrincipalName} started");
                    isSuccess = RemoveUserIDDPolicy(user, sfbUser, sfbRequestDetail, fields, entitlements, currentUser);
                    if (isSuccess)
                    {
                        ProvisioningEmailHelper.NotifyMSTeamIDDRemoval(this.CurrentRequest, user, sfbUser.UserPrincipalName, sfbRequestDetail.DisplayCurrentLineURI, this.Endpoint.DisplayName, "Amendment Remove");
                        entitlements.Add($"{EntitlementType.IDD.ToString()} policy removed");
                    }
                }
                if (isSuccess)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Provisioning(AmendmentRemove): User:{user.AccountName} sending email started");
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, string.Join(Environment.NewLine, entitlements), user.AccountName);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Provisioning(AmendmentRemove): User:{user.AccountName} sending email completed");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "AmendmentRemove failed", ex);
            }
            return isSuccess;
        }
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return OffBoardOrDeleteAccount(user, fields, true);
            
        }
        private bool OffBoardOrDeleteAccount(User user, ProvisioningParamCollection fields,bool isOffBoardRequest=false)
        {
            bool isSuccess = false;
            try
            {
                var sfbUserValue = SkypeForBusinessUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);

                if (sfbUserValue != null)
                {
                    if (isOffBoardRequest)
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"TakeOUSnapshot before OffBoardSFBuser:{sfbUserValue.UserPrincipalName} started");
                        this.TakeOUSnapshot(user.Id, sfbUserValue, EmployeeEvents.Resignation, EndpointType.MicrosoftTeams, EntitlementType.ResponseGroup);
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"TakeOUSnapshot before OffBoardSFBuser:{sfbUserValue.UserPrincipalName} started");
                    }
                    this.OffBoardSFBuser(isOffBoardRequest,user, sfbUserValue, LockScenario.OffBoard);
                    isSuccess = true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Warning, "OffBoard User doesnt exists", null);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing offboarding request. {0}", ex.Message), ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        //private void NotifyDelegationTaskToEmailTeam(List<PresentationResponseGroupMember> maskedUserGroup, DomainModel.User user, string userPrincipalName, string actionName)
        //{
        //    try
        //    {
        //        if (maskedUserGroup != null && maskedUserGroup.Count > 0)
        //            ProvisioningEmailHelper.NotifyMSTeamsDelegation(user, this.CurrentRequest, string.Join(",", maskedUserGroup.Select(i => i.ResponseGroupName)), userPrincipalName, actionName, this.Endpoint.DisplayName);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        private bool OffBoardSFBuser(bool isOffBoardRequest, DomainModel.User user, SkypeForBusinessUserInfo sfbUserValue, LockScenario offBoard)
        {
            var eventType = isOffBoardRequest ? "OffBoard" : "DeleteAccount";
            bool isSuccess = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                _idmsLogger.WriteLog(LogLevelType.Info, $"{eventType}:{sfbUserValue.UserPrincipalName} started");
                var userInfo = SkypeForBusinessUserInfo.GetByIdIncludeGroups(sfbUserValue.Id, this.Endpoint.Id);
                
                if (userInfo != null)
                {
                    var maskedUserGroup = new List<PresentationResponseGroupMember>();
                    if (userInfo.SkypeForBusinessResponseGroup != null && userInfo.SkypeForBusinessResponseGroup.Count > 0)
                    {
                        maskedUserGroup = SkypeForBusinessResponseGroupInfo.GetMaskedUser(this.Endpoint.Id, userInfo.Id, true);
                    }
                    
                    isSuccess = EndpointConnector.DisableAccount((IEndpointUserInfo)sfbUserValue);
                    if (isSuccess)
                    {
                        if (userInfo.UserType == EntitlementType.IDD)
                        {
                            ProvisioningEmailHelper.NotifyMSTeamIDDRemoval(this.CurrentRequest, user, userInfo.UserPrincipalName, userInfo.AnyOneLineURI, this.Endpoint.DisplayName, isOffBoardRequest ? "Offboard": "Delete Account");
                        }

                        if (maskedUserGroup.Count > 0)
                        {
                            ProvisioningEmailHelper.NotifyMSTeamsDelegation(user, this.CurrentRequest, string.Join(",", maskedUserGroup.Select(i => i.ResponseGroupName)), userInfo.UserPrincipalName, "Remove Delegation", this.Endpoint.DisplayName, userInfo.AnyOneLineURI, isOffBoardRequest ? "Offboard" : "Delete Account");
                        }
                        if (userInfo.SkypeForBusinessResponseGroup.Count > 0)
                        {
                            SkypeForBusinessUserInfo.DeleteAllGroupByUserId(this.Endpoint.Id, userInfo.Id);
                        }
                        //if (userInfo.LineURIExtension.HasValue && userInfo.LineURIExtension.Value > 0)
                        {
                            if (userInfo.UserType != EntitlementType.DCC)
                            {
                                if (userInfo.LineURIExtension.HasValue)
                                    SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, userInfo.LineURIExtension.Value, currentUser, false);
                            }
                            var userId = userInfo.Id;
                            userInfo.Delete();
                            var accountRecon = AccountReconcilation.GetByEndpointIdActiveAndEndpointUserId(this.Endpoint.Id, userId, user.Id);
                            if (accountRecon != null)
                            {
                                accountRecon.IsLocked = true;
                                accountRecon.IsDeleted = true;
                                accountRecon.DeletedDate = DateTime.Now;
                                accountRecon.AccessRequestId = this.CurrentRequest.Id;
                                EntitlementReconcilation.DeleteEntitlements(accountRecon.Id, this.Endpoint.Id, currentUser, this.CurrentRequest.Id);
                                accountRecon.Save(currentUser);
                            }

                            UpdateSAPHRMSAttributes(user, string.Empty, string.Empty, true);
                            UpdateDIUserTelephoneNumber(user, string.Empty);

                            if (isOffBoardRequest == false)
                            {
                                if(ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, user.AccountName))
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"AmendmentRemove User:{user.AccountName} email sent");
                                }
                            }
                        }
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"{eventType}:{sfbUserValue.UserPrincipalName} user not exists. Hence marking request as completed");
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"{eventType}:{sfbUserValue.UserPrincipalName} completed", ex);
            }
            return isSuccess;
        }
        private void SendEmailOnOffboardAndTranfser(SkypeForBusinessUserInfo userInfo)
        {

        }
        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool ReOrg(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isSuccess = true;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var sfbAcount = SkypeForBusinessUserInfo.GetPrimaryByDIUserId(user.Id, this.Endpoint.Id);
                if (sfbAcount != null)
                {
                    var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(sfbAcount.Id, this.Endpoint.Id);
                    var maskedGroups = SkypeForBusinessResponseGroupInfo.GetMaskedUser(this.Endpoint.Id, sfbAcount.Id, true);
                    this.TakeOUSnapshot(user.Id, sfbAcount, EmployeeEvents.OUChange, EndpointType.MicrosoftTeams, EntitlementType.ResponseGroup);
                    var entitlementType = EndpointConnector.GetUserType(sfbAcount);

                    sfbAcount.OnlineVoiceRoutingPolicy = fields["DefaultOnlineVoiceRoutingPolicy"].Value;
                    sfbAcount.TenantDialPlan = fields["DefaultTeamDialPlanPolicy"].Value;
                    sfbAcount.TeamsUpgradePolicy = fields["TeamsUpgradePolicy"].Value;

                    if (entitlementType == EntitlementType.DCC || entitlementType == EntitlementType.None || string.IsNullOrWhiteSpace(userWithGroups.AnyOneLineURI))
                    {
                        sfbAcount.IsRequireLineUriAssignment = true;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Transfer user:{sfbAcount.AccountName} Old number:{sfbAcount.AnyOneLineURI}");
                        var LineUriPrefix = fields["LineUriPrefix"].Value.Replace(" ", "");
                        var telephoneNumber = string.Empty;
                        AssignRandomLineURI(user, sfbAcount, LineUriPrefix, currentUser, ref telephoneNumber);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"user:{sfbAcount.AccountName} new number assignment:{telephoneNumber}");
                    }
                    //Remove from response group, Move to online
                    isSuccess = EndpointConnector.ExecuteTransferAction(sfbAcount);
                    if (isSuccess)
                    {
                        var currentSFBInfo = EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                        if (currentSFBInfo != null
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.LineURI) && !string.IsNullOrWhiteSpace(currentSFBInfo.EnterpriseVoiceEnabled)
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.TenantDialPlan)
                            && !string.IsNullOrWhiteSpace(currentSFBInfo.OnlineVoiceRoutingPolicy)
                            && currentSFBInfo.LineURI.Contains(sfbAcount.LineURI, StringComparison.InvariantCultureIgnoreCase)
                            && currentSFBInfo.EnterpriseVoiceEnabled.Trim().ToUpper() == "True".ToUpper()
                            && currentSFBInfo.TenantDialPlan.Trim().ToUpper() == sfbAcount.TenantDialPlan.ToUpper()
                            && currentSFBInfo.OnlineVoiceRoutingPolicy.Trim().ToUpper() == sfbAcount.OnlineVoiceRoutingPolicy.ToUpper()
                        )
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Account Online Status After Transfer: UserPrincipalName - {currentSFBInfo?.UserPrincipalName} LineURI - {currentSFBInfo?.LineURI} EnterpriseVoiceEnabled - {currentSFBInfo?.EnterpriseVoiceEnabled} TenantDialPlan - {currentSFBInfo?.TenantDialPlan} OnlineVoiceRoutingPolicy - {currentSFBInfo?.OnlineVoiceRoutingPolicy}");

                            if (entitlementType == EntitlementType.IDD)
                            {
                                ProvisioningEmailHelper.NotifyMSTeamIDDRemoval(this.CurrentRequest, user, userWithGroups.UserPrincipalName, userWithGroups.AnyOneLineURI, this.Endpoint.DisplayName, "OU Change");
                            }
                            if (maskedGroups != null && maskedGroups.Count > 0)
                            {
                                ProvisioningEmailHelper.NotifyMSTeamsDelegation(user, this.CurrentRequest, string.Join(",", maskedGroups.Select(i => i.ResponseGroupName)), sfbAcount.UserPrincipalName, "Remove Delegation", this.Endpoint.DisplayName, sfbAcount.LineURI, "OU Change");
                            }
                            sfbAcount.Save(currentUser);
                            if (sfbAcount.LineURIExtension.HasValue)
                                SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, sfbAcount.LineURIExtension.Value, currentUser, true);
                            var data = sfbAcount.AnyOneLineURI.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            string telephoneNumber = string.Empty;
                            if (data != null && data.Length > 0)
                            {
                                telephoneNumber = data[0];
                            }
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Transfer user:{sfbAcount.AccountName} Update User Profile ");
                            UpdateDIUserTelephoneNumber(user, telephoneNumber);

                            _idmsLogger.WriteLog(LogLevelType.Info, $"Transfer user:{sfbAcount.AccountName} Update HRMS Attributes started");
                            UpdateSAPHRMSAttributes(user, telephoneNumber, sfbAcount.LineURIExtension.Value.ToString());

                            if (userWithGroups.SkypeForBusinessResponseGroup.Count > 0)
                            {
                                SkypeForBusinessUserInfo.DeleteAllGroupByUserId(this.Endpoint.Id, sfbAcount.Id);
                            }
                            var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, sfbAcount.Id, user.Id);
                            if (accountRecon != null)
                            {
                                EntitlementReconcilation.DeleteEntitlementsByType((int)EntitlementType.ResponseGroup, accountRecon.Id, this.Endpoint.Id, currentUser, this.CurrentRequest.Id);
                                if (userWithGroups.UserType != EntitlementType.Default)
                                {
                                    EntitlementReconcilation.DeleteEntitlementsByType((int)userWithGroups.UserType, accountRecon.Id, this.Endpoint.Id, currentUser, this.CurrentRequest.Id);
                                }
                                var entRecon = EntitlementReconcilation.GetByEntitlementType(accountRecon.EndpointSystemId, accountRecon.Id, accountRecon.EndpointUserId, (int)EntitlementType.Default);
                                if (entRecon == null)
                                {
                                    entRecon = CreateEntitlementReconcilationShell(accountRecon, EntitlementType.Default);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.AccessRequestId = this.CurrentRequest.Id;
                                }
                                else
                                {
                                    entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                                    entRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                                    entRecon.AccessRequestId = this.CurrentRequest.Id;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = sfbAcount.AnyOneLineURI;
                                    entRecon.EntitlementId = sfbAcount.Id;
                                    entRecon.EntitlementDescription = sfbAcount.AnyOneLineURI;
                                    entRecon.IsDeleted = false;
                                    entRecon.DeletedDate = null;
                                    entRecon.LastAttestedOn = null;
                                }
                                entRecon.Save(currentUser);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"User:{sfbAcount.SamAccountName} updating entitlement completed");
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Account Online Status After Transfer: UserPrincipalName - {currentSFBInfo?.UserPrincipalName} LineURI - {currentSFBInfo?.LineURI} EnterpriseVoiceEnabled - {currentSFBInfo?.EnterpriseVoiceEnabled} TenantDialPlan - {currentSFBInfo?.TenantDialPlan} OnlineVoiceRoutingPolicy - {currentSFBInfo?.OnlineVoiceRoutingPolicy}");
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Transfer user:{sfbAcount.AccountName} failed becaue it not match OnlineVoiceRoutingPolicy,TenantDialPlan,LineURI,EnterpriseVoiceEnabled");
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Transfer failed for user:{sfbAcount.AccountName}");
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Transfer for user - {user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        public override bool ExecuteEnableCommand(ProvisioningParamCollection fields, DomainModel.User user, bool fromAccessRequest = false)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                if (this.CurrentRequest.AccountType == AccountType.User)
                {
                    var domain = MasterConfigHelper.O365EmailDomainName;
                    var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                    var o365 = EndPoint.GetByApplicationType(ApplicationType.Office365);
                    var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, o365.Id);

                    if (adUserInfo == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AD Account does not exists/Locked for the User:{user.AccountName}");
                        return false;
                    }
                    if (o365User == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Office365 User Account does not exists for the User:{user.AccountName}");
                        return false;
                    }
                    var resetParam = new List<KeyValuePair<string, string>>();
                    try
                    {
                        //SFB reactivation issue - msRTCSIP-DeploymentLocator.  
                        var adEp = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        var adConnector = ConnectorFactory.GetConnector(adEp, this.CurrentRequest, _idmsLogger);
                        resetParam = new List<KeyValuePair<string, string>>();
                        resetParam.Add(new KeyValuePair<string, string>("msRTCSIP-DeploymentLocator", null));
                        var updated = adConnector.UpdateSpecificAttribute(adUserInfo, resetParam);
                        if (updated)
                        {
                            WriteProvisioningLog(LogLevelType.Info, "Updated AD Attribute(msRTCSIP-DeploymentLocator) before activate account");
                            System.Threading.Thread.Sleep(30000);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"{ex}",ex);
                        throw ex;
                    }
                    finally
                    {
                        resetParam?.Clear();
                        resetParam = null;
                    }

                    var aRecon = AccountReconcilation.GetByUserId(user.Id, AccountType.User, this.Endpoint.Id);
                    if (aRecon == null || aRecon.IsDeleted || fromAccessRequest)
                    {
                        SkypeForBusinessUserInfo sfbUser = null;
                        var userPrincipalName = $"{adUserInfo.sAMAccountName}@{domain}";
                        sfbUser = SkypeForBusinessUserInfo.GetByUserPrincipalName(userPrincipalName);
                        if(sfbUser == null)
                        {
                            sfbUser = new SkypeForBusinessUserInfo();
                            sfbUser.SamAccountName = adUserInfo.sAMAccountName;
                            sfbUser.DisplayName = adUserInfo.DisplayName;
                            sfbUser.Sid = adUserInfo.objectSid;
                            sfbUser.SipAddress = $"sip:{adUserInfo.sAMAccountName}@{domain}";
                            sfbUser.UserPrincipalName = userPrincipalName;
                            sfbUser.WindowsEmailAddress = user.EmailId;
                            sfbUser.EndPointSystemId = this.Endpoint.Id;
                            sfbUser.UserType = EntitlementType.Default;
                            sfbUser.CreatedBy = currentUser;
                            sfbUser.CreatedDate = DateTime.Now;
                            sfbUser.Enabled = false;
                            sfbUser.IsValid = false;
                        }
                        else
                        {
                            sfbUser.DisplayName = adUserInfo.DisplayName;
                            sfbUser.SipAddress = $"sip:{adUserInfo.sAMAccountName}@{domain}";
                            sfbUser.WindowsEmailAddress = user.EmailId;
                            sfbUser.EndPointSystemId = this.Endpoint.Id;
                            sfbUser.UserType = EntitlementType.Default;
                        }
                        sfbUser.WaitingAccountSync = true;
                        sfbUser.AccessRequestId = this.CurrentRequest.Id;
                        sfbUser.Save(currentUser);
                        isExecutedSuccessfully = ExecuteUserAccountPolicy(fields, user, false);
                    }
                }
            }
            catch (Exception ex)
            {
                isExecutedSuccessfully = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"ExecuteEnableUserAccount failed for User:{user.AccountName}", ex);
            }
            return isExecutedSuccessfully;
        }
        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="user"></param>
        /// <param name="currentUser"></param>
        /// <param name="domain"></param>
        /// <param name="adUserInfo"></param>
        /// <param name="sfbUser"></param>
        /// <param name="fromUndoRequest"></param>
        /// <param name="telephone"></param>
        /// <returns></returns>
        private bool ApplySkypeForBusinessAttribute(ProvisioningParamCollection fields, User user, string currentUser, string domain, ADSUserInfo adUserInfo, SkypeForBusinessUserInfo sfbUser,bool fromUndoRequest,string telephone)
        {
            bool isExecutedSuccessfully = false;
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"ExecuteNewAccountPolicy started");
                if (EndpointConnector.CreateAccount(sfbUser, fields))
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"New Account Policy executed for the User:{user.DisplayName}");
                    var IsAccountExists = this.EndpointConnector.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    //If LineURI same then only allow to create profile
                    if (IsAccountExists != null && !string.IsNullOrWhiteSpace(IsAccountExists.LineURI) && !string.IsNullOrWhiteSpace(IsAccountExists.EnterpriseVoiceEnabled)
                        && !string.IsNullOrWhiteSpace(IsAccountExists.TenantDialPlan)
                        && !string.IsNullOrWhiteSpace(IsAccountExists.OnlineVoiceRoutingPolicy)
                        && !string.IsNullOrWhiteSpace(IsAccountExists.ExternalAccessPolicy)
                        && IsAccountExists.LineURI.Contains(sfbUser.LineURI,StringComparison.InvariantCultureIgnoreCase) 
                        && IsAccountExists.EnterpriseVoiceEnabled.Trim().ToUpper() == "True".ToUpper()
                        && IsAccountExists.TenantDialPlan.ToUpper() == sfbUser.TenantDialPlan.ToUpper()
                        && IsAccountExists.OnlineVoiceRoutingPolicy.ToUpper() == sfbUser.OnlineVoiceRoutingPolicy.ToUpper()
                        && IsAccountExists.ExternalAccessPolicy.ToUpper() == sfbUser.ExternalAccessPolicy.ToUpper()
                    )
                    {
                        sfbUser.HostingProvider = IsAccountExists.HostingProvider;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Account Online Status After Account Creation: UserPrincipalName - {IsAccountExists.UserPrincipalName} LineURI - {IsAccountExists.LineURI} EnterpriseVoiceEnabled - {IsAccountExists.EnterpriseVoiceEnabled} TenantDialPlan - {IsAccountExists.TenantDialPlan} OnlineVoiceRoutingPolicy - {IsAccountExists.OnlineVoiceRoutingPolicy} ExternalAccessPolicy - {IsAccountExists.ExternalAccessPolicy}");
                        isExecutedSuccessfully = CreateUserAndAccountRecon(user, sfbUser);
                        if (isExecutedSuccessfully)
                        {
                            if (sfbUser.UserType != EntitlementType.DCC)
                            {
                                SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, sfbUser.LineURIExtension.Value, currentUser, true);
                            }
                            UpdateSAPHRMSAttributes(user, telephone, sfbUser.LineURIExtension.Value.ToString());
                            UpdateDIUserTelephoneNumber(user, telephone);
                        }
                        isExecutedSuccessfully = true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Account creation failed account not exists({IsAccountExists == null}) or LineURI:({IsAccountExists?.LineURI}) not same as {sfbUser.LineURI}");
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"ApplyPolicy failed for:{user.AccountName}", ex);
                isExecutedSuccessfully = false;
            }
            return isExecutedSuccessfully;
        }
        
        //private bool IsAccountExists(string userPrincipalName)
        //{
        //    bool IsExists = false;
        //    try
        //    {
        //        if (EndpointConnector.CheckEndPointAccountExists(userPrincipalName) && EndpointConnector.CheckOnlineAccountExists(userPrincipalName))
        //        {
        //            if (EndpointConnector.EndpointUserAccount != null && EndpointConnector.EndpointOnlineUserAccount != null) 
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        IsExists = false;
        //        throw new Exception($"IsAccountExists failed", ex);
        //    }
        //    return IsExists;
        //}
        private void UpdateDIUserTelephoneNumber(User user, string telephone)
        {
            try
            {
                
                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateDIUserTelephoneNumber:{user.AccountName}, Telephone:{telephone} started");
                if (!string.IsNullOrWhiteSpace(telephone) && telephone.IndexOf("tel:") == 0)
                {
                    user.TelephoneNumber = telephone.Replace("tel:", "").Trim();
                }
                else
                {
                    if(!string.IsNullOrWhiteSpace(telephone))
                        user.TelephoneNumber = telephone;
                    else
                        user.TelephoneNumber = string.Empty;
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateDIUserTelephoneNumber:{user.AccountName}, Telephone:{telephone}");
                  SkypeForBusinessUserInfo.ExecuteQuery($"UPDATE DI_M_User SET TelephoneNumber={{0}} WHERE ID = {user.Id}", user.TelephoneNumber);
                //var userInfo = User.GetById(user.Id);
                //userInfo.TelephoneNumber = user.TelephoneNumber;
                //userInfo.Save(currentUser);
                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateDIUserTelephoneNumber:{user.AccountName}, Telephone:{telephone} Completed");

                //Agreed by Kanchana that It will update for all user.
                //if(user.FullTimeEmployee == false)
                //{
                    try
                    {
                        if(!string.IsNullOrWhiteSpace(user.TelephoneNumber))
                        {
                            EmployeeEventTrigger trigger = new EmployeeEventTrigger()
                            {
                                CreatedBy = "System",
                                CreatedDate = DateTime.Today,
                                EffectiveDate = DateTime.Now,
                                Comments = "MSTeam TelephoneNumber update",
                                EventType = EmployeeEvents.Propagation,
                                IsRequestCreated = false,
                                UserId = user.Id,
                                IsActive = true
                            };
                            trigger.Save(currentUser);
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Propagation trigger created for user:{user.UserName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, $"Creating Propagation trigger for User:{user.AccountName} failed", ex);
                    }
                //}
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UpdateDIUserTelephoneNumber:{user.AccountName}, Telephone:{telephone} failed", ex);
            }
        }

        private void UpdateSAPHRMSAttributes(User user, string telephone, string extension, bool NeedToBeRemoved = false)
        {
            try
            {
                if (user.FullTimeEmployee == false)
                {
                    return;
                }
                var tempTelephone = string.Empty;
                if (!string.IsNullOrWhiteSpace(telephone) && telephone.IndexOf("tel:") == 0)
                {
                    tempTelephone = telephone.Replace("tel:", "").Trim();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(telephone))
                    {
                        tempTelephone = telephone;
                    }
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"Update SAPHRMSAttributes User:{user.AccountName}  started");
                var sapHRMSEndPoint = EndPoint.GetTrustedSourceEndPoint();             
                if (sapHRMSEndPoint != null)
                {
                    ConnectorBase sapConnector = ConnectorFactory.GetConnector(sapHRMSEndPoint, CurrentRequest);

                    bool telephoneUpdated = false;

                    if (NeedToBeRemoved)
                    {
                        telephoneUpdated = sapConnector.DelimitTelephoneInHR(user.EmployeeId, tempTelephone, extension, user.JoinDate.Value);
                    }
                    else
                    {
                        telephoneUpdated = sapConnector.AddTelephoneInHR(user.EmployeeId, tempTelephone, extension, user.JoinDate.Value);
                    }

                    if (telephoneUpdated)
                    {
                        user.NeedTelephoneUpdateToHR = false;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"User:{user.AccountName} SAPHRMSAttributes successfully updated");
                    }
                    else
                    {
                        user.NeedTelephoneUpdateToHR = true;
                    }
                }
                else
                {
                    user.NeedTelephoneUpdateToHR = true;
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Unable to connect to SAP to update SAPHRMSAttributes User:{user.AccountName} .");
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"User:{user.AccountName} SAPHRMSAttributes completed");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UpdateSAPHRMSAttributes failed for the User:{user.AccountName}", ex);
            }
        }

        private bool CreateUserAndAccountRecon(User user, SkypeForBusinessUserInfo sfbUser)
        {
            string currentUser = MasterConfigHelper.ServiceUser;
            bool isSuccess = false;
            try
            {
                var epUser = SkypeForBusinessUserInfo.GetByUserPrincipalName(sfbUser.UserPrincipalName);
                if (epUser == null)
                {
                    sfbUser.CreatedBy = "system";
                    sfbUser.CreatedDate = DateTime.Now;
                    sfbUser.Enabled = true;
                    sfbUser.IsValid = true;
                    sfbUser.Save(currentUser);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"User Account Created {user.AccountName}");
                }
                else
                {
                    epUser.LineURI = sfbUser.LineURI;
                    epUser.LineURIExtension = sfbUser.LineURIExtension;
                    epUser.VoicePolicy = sfbUser.VoicePolicy;
                    epUser.VoiceRoutingPolicy = sfbUser.VoiceRoutingPolicy;
                    epUser.DialPlan = sfbUser.DialPlan;
                    epUser.ExternalAccessPolicy = sfbUser.ExternalAccessPolicy;
                    epUser.ConferencingPolicy = sfbUser.ConferencingPolicy;
                    epUser.LastModifiedBy = currentUser;
                    epUser.LastModifiedDate = DateTime.Now;
                    epUser.HostingProvider = sfbUser.HostingProvider;
                    epUser.Enabled = true;
                    epUser.IsValid = true;
                    epUser.Save(currentUser);
                    sfbUser = epUser;
                }

                if (CreateUserInReconTable(sfbUser, currentUser))
                {
                    var userStatus = GetCurrentStatus(sfbUser);
                    
                    _idmsLogger.WriteLog(LogLevelType.Info, $"AccountReconcilation Created for{user.AccountName}");
                    var accountRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserAccountName(this.Endpoint.Id, sfbUser.SamAccountName, user.Id);
                    var existingEntilements = EntitlementReconcilation.GetByEndpointUserId(sfbUser.Id, this.Endpoint.Id);
                    var entRecon = existingEntilements.Where(e => e.EndpointSystemId == this.Endpoint.Id && e.EndpointUserId == sfbUser.Id && e.EntitlementType == (int)sfbUser.UserType).FirstOrDefault();
                    if (entRecon == null)
                    {
                        var entitlements = (sfbUser.Entitlements.Select(x => x.EntitlementName.ToString()));
                        entRecon = this.CreateEntitlementReconcilationShell(accountRecon, EntitlementType.Default, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                        entRecon.Entitlement = sfbUser.LineURI;
                        entRecon.EntitlementId = sfbUser.Id;
                        entRecon.EntitlementDescription = userStatus;
                        entRecon.IsDeleted = false;
                        entRecon.CreatedDate = DateTime.Now;
                        entRecon.CreatedBy = currentUser;
                    }
                    else
                    {
                        entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                        entRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        entRecon.DeletedDate = null;
                        entRecon.IsDeleted = false;
                        entRecon.LastModifiedDate = DateTime.Now;
                        entRecon.LastModifiedBy = currentUser;
                        entRecon.Entitlement = sfbUser.LineURI;
                        entRecon.EntitlementId = sfbUser.Id;
                        entRecon.EntitlementDescription = userStatus;
                    }
                    entRecon.Save(currentUser);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"EntitlementReconcilation Created for the {user.AccountName}");
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                WriteProvisioningLog(LogLevelType.Exception, $"CreateAccountReconAndEntitlementRecon failed for User{user.AccountName}", ex);
            }
            return isSuccess;
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
        public override bool CreateUserInReconTable(IEndpointUserInfo epUser, string currentUser)
        {
            EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(epUser.EndPointSystemId);
            if (ep != null)
            {
                var sfbUser = epUser as SkypeForBusinessUserInfo;
                var existingAcctRecon = AccountReconcilation.GetByAccountName(sfbUser.SamAccountName, ep.Id);
                if (existingAcctRecon != null)
                {
                    existingAcctRecon.AccessRequestId = this.CurrentRequest.Id;
                    existingAcctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                    existingAcctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    existingAcctRecon.IsLocked = false;
                    if (existingAcctRecon.IsDeleted)
                    {
                        existingAcctRecon.IsDeleted = false;
                        existingAcctRecon.AccountCreatedDate = DateTime.Now;
                        existingAcctRecon.DeletedDate = null;
                        existingAcctRecon.LastAttestedOn = null;
                        existingAcctRecon.LastLogonDate = null;
                        existingAcctRecon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;
                        existingAcctRecon.MappingType = existingAcctRecon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                        existingAcctRecon.OffboardingAccountAction = (existingAcctRecon.AccountType == AccountType.User || existingAcctRecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                        existingAcctRecon.EndpointUserId = epUser.Id;
                    }
                    // set mapping for global user
                    existingAcctRecon.FullName = epUser.DisplayName;
                    existingAcctRecon.UserId = CurrentRequest.UserId;
                    existingAcctRecon.Save(currentUser);
                }
                else
                {
                    AccountReconcilation recon = new AccountReconcilation();
                    recon.IsLocked = false;
                    recon.AccessRequestId = this.CurrentRequest.Id;
                    recon.UserId = CurrentRequest.UserId;
                    recon.AccountName = sfbUser.SamAccountName;
                    recon.FullName = epUser.DisplayName;
                    recon.EndpointSystemId = epUser.EndPointSystemId;
                    recon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;
                    recon.MappingType = recon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                    recon.OffboardingAccountAction = (recon.AccountType == AccountType.User || recon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    recon.EndpointUserId = epUser.Id;
                    recon.ValidFromDate = CurrentRequest.ActivationDate;
                    recon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    recon.Save(currentUser);

                }
                
            }
            return true;
        }
        protected EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, EntitlementType _entitlementType, string Entitlements = "")
        {
            return new EntitlementReconcilation()
            {
                IsSODConflict = false,
                IsSensitiveAccess = false,
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                DeletedDate = null,
                EntitlementType = (int)_entitlementType,
                EndpointType = (EndpointType)this.Endpoint.EndpointSystemTypeId,
                ValidFromDate = CurrentRequest.ActivationDate,
                ValidToDate = GetValidTo(CurrentRequest.UserId)
            };
        }
        /// <summary>
        /// Execute the create account policy and assign number
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="user"></param>
        /// <param name="accountExists"></param>
        /// <returns></returns>
        public override bool ExecuteUserAccountPolicy(ProvisioningParamCollection fields, DomainModel.User user,bool accountExists)
        {
            bool isExecuted = false;
            try
            {
                //if (EndpointConnector.Connect())
                {
                    var currentUser = MasterConfigHelper.ServiceUser;
                    if (this.CurrentRequest.AccountType == AccountType.User)
                    {
                        var domain = MasterConfigHelper.O365EmailDomainName;
                        var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                        var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                        var o365 = EndPoint.GetByApplicationType(ApplicationType.Office365);
                        var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, o365.Id);

                        if (adUserInfo == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AD Account does not exists/Locked for the User:{user.AccountName}");
                            return false;
                        }
                        if (o365User == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Office365 User Account does not exists for the User:{user.AccountName}");
                            return false;
                        }
                        var userPrincipalName = $"{adUserInfo.sAMAccountName}@{domain}";
                        accountExists = true;
                        var sfbUserInfo = SkypeForBusinessUserInfo.GetByUserPrincipalName(userPrincipalName);
                        if (sfbUserInfo != null)
                        {
                            sfbUserInfo.TenantDialPlan = fields["DefaultTeamDialPlanPolicy"].Value;
                            sfbUserInfo.OnlineVoiceRoutingPolicy = fields["DefaultOnlineVoiceRoutingPolicy"].Value;
                            sfbUserInfo.ExternalAccessPolicy = fields["ExternalAccessPolicy"].Value;
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Before Create Account: User LineURI:{sfbUserInfo.AnyOneLineURI}");

                            AdhocSkypeForBusinessRequest sfbRequestDetails = null;
                            var telephone = string.Empty;
                            var LineUriPrefix = fields["LineUriPrefix"].Value;
                            AssignRandomLineURI(user, sfbUserInfo, LineUriPrefix, currentUser,ref telephone); 
                            _idmsLogger.WriteLog(LogLevelType.Info, $"AssignRandomLineURI:{sfbUserInfo.UserPrincipalName} {telephone}");
                            if (CurrentRequest.AdHocAccessRequest != null)
                            {
                                sfbRequestDetails = (AdhocSkypeForBusinessRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                            }
                            
                            var isPolicyApplied = ApplySkypeForBusinessAttribute(fields, user, currentUser, domain, adUserInfo, sfbUserInfo, CurrentRequest.EventUndoRequestId.HasValue, telephone);
                            if (isPolicyApplied)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Policy ApplySkypeForBusinessAttribute applied");
                                if (CurrentRequest.AdHocAccessRequest != null)
                                {
                                    if (sfbRequestDetails != null && sfbRequestDetails.SkypeForBusinessResponseGroups.Count > 0)
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"Assign user to ResponseGroups");
                                        sfbRequestDetails.EndpointUserId = SkypeForBusinessUserInfo.GetByUserPrincipalName(sfbUserInfo.UserPrincipalName).Id;
                                        isExecuted = AddUserToResponseGroupOnNewAccountCreation(user, sfbRequestDetails, fields);
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"AddUserToResponseGroupOnNewAccountCreation completed");
                                    }
                                    else
                                    {
                                        isExecuted = true;
                                    }
                                }else
                                {
                                    isExecuted = true;
                                }
                                if(isExecuted)
                                {
                                    sfbUserInfo.Enabled =true;
                                    sfbUserInfo.IsValid = true;
                                    sfbUserInfo.WaitingAccountSync = null;
                                    sfbUserInfo.AccessRequestId = null;
                                    sfbUserInfo.Save(currentUser);
                                    isExecuted = true;
                                    ProvisioningEmailHelper.SendEmailForAccountProvisioning(this.CurrentRequest, sfbUserInfo, user, ApplicationType.MicrosoftTeams);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isExecuted = false;
                _idmsLogger.WriteLog(LogLevelType.Info, $"ExecuteUserAccountPolicy failed for the User:{user.AccountName}",ex);
            }
            return isExecuted;
        }
        private void AssignRandomLineURI(DomainModel.User user, SkypeForBusinessUserInfo userInfo,string LineUriPrefix,string currentUser,ref string telephone)
        {
            var lineUriExtension = SkypeForBusinessUnAssignedNumber.GetUnAssignedRandomNumber(this.Endpoint.Id, currentUser);
            if (lineUriExtension == 0) throw new Exception($"Random Number({LineUriPrefix + " " + lineUriExtension.ToString()}) should not be Zero");
            _idmsLogger.WriteLog(LogLevelType.Info, $"Get Random Number({LineUriPrefix + " " + lineUriExtension.ToString()}) for the User:{user.AccountName}");
            telephone = $"{LineUriPrefix.Replace(" ", "")}{lineUriExtension}";
            var lineUri = $"{telephone}";
            bool isExists = true;
            while (isExists)
            {
                isExists = EndpointConnector.IsUserPropertyExists("LineURI", lineUri, userInfo.UserPrincipalName);
                if (isExists)
                {
                    SkypeForBusinessUnAssignedNumberMatrix.UpdateNumberStatus(this.Endpoint.Id, lineUriExtension, currentUser, true);
                    lineUriExtension = SkypeForBusinessUnAssignedNumber.GetUnAssignedRandomNumber(this.Endpoint.Id, currentUser);
                    if (lineUriExtension == 0) throw new Exception($"Random Number({LineUriPrefix + " " + lineUriExtension.ToString()}) should not be Zero");
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Try Another Random Number({LineUriPrefix + " " + lineUriExtension.ToString()}) for the User:{user.AccountName}");
                    telephone = $"{LineUriPrefix.Replace(" ", "")}{lineUriExtension}";
                    lineUri = $"{telephone}";
                }
            }
            userInfo.LineURI = lineUri;
            userInfo.OnPremLineURI = lineUri;
            userInfo.LineURIExtension = lineUriExtension;
        }
        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = false;
            try
            {
                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "UndoEvent started");
                    isUpdated = UndoTransferEvent(user, fields);
                    _idmsLogger.WriteLog(LogLevelType.Info, "UndoEvent completed");
                }
                else if(this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "UndoOffBoardEvent started");
                    isUpdated = UndoOffBoardEvent(user, fields);
                    _idmsLogger.WriteLog(LogLevelType.Info, "UndoOffBoardEvent completed");
                }
                return isUpdated;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return isUpdated;
        }

        private bool UndoOffBoardEvent(User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = true;
            try
            {
                var domain = MasterConfigHelper.O365EmailDomainName;
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                var o365 = EndPoint.GetByApplicationType(ApplicationType.Office365);
                var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, o365.Id);

                if (adUserInfo == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"AD Account does not exists/Locked for the User:{user.AccountName}");
                    return false;
                }
                if (o365User == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Office365 User Account does not exists for the User:{user.AccountName}");
                    return false;
                }
                var snapshot = OUChangeSnapshot.GetByUserId(user.Id);
                if (snapshot != null)
                {
                    if (this.CurrentRequest.EventUndoRequest != null && !string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalGroups))
                    {
                        var sfbSnapShotDetails = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(this.CurrentRequest.EventUndoRequest.TechnicalGroups);
                        if(sfbSnapShotDetails == null || sfbSnapShotDetails.UserInfo == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Snapshot details is empty for the User:{user.AccountName}");
                            return true;
                        }
                        var sfbEntitlements = sfbSnapShotDetails.Entitlements;
                        var sfbUserEntitlementDetails = sfbEntitlements.Where(i=>i.EntitlementType != EntitlementType.ResponseGroup);
                        var snpaShotResponseGroups = sfbEntitlements.Where(i => i.EntitlementType == EntitlementType.ResponseGroup).ToList();
                        List<ResponseGroup> adhocRequestResponseGroup = new List<ResponseGroup>();
                        foreach (var responseGroup in snpaShotResponseGroups)
                        {
                            var sfbGroup = SkypeForBusinessResponseGroupInfo.GetGroupByName(responseGroup.Entitlement);
                            if (sfbGroup != null)
                            {
                                var matrix = SkypeForBusinessProvisioningMatrix.GetByName(sfbGroup.Name, EntitlementType.ResponseGroup);
                                var grp = new ResponseGroup()
                                {
                                    Name = sfbGroup.Name,
                                    Id = sfbGroup.Id,
                                    Identity = sfbGroup.Identity,
                                };
                                if (!string.IsNullOrWhiteSpace(responseGroup.Function) && responseGroup.Function.Equals(functionMasked))
                                {
                                    grp.Mask = true;
                                }
                                adhocRequestResponseGroup.Add(grp);
                            }
                        }

                        var delimiter = new char[] { '|'};
                        SkypeForBusinessUserInfo userInfo = new SkypeForBusinessUserInfo();
                        userInfo.UserType = EntitlementType.Default;
                        userInfo.UserPrincipalName = sfbSnapShotDetails.UserInfo.UserName;
                        //var lineUriDetails = sfbSnapShotDetails.UserInfo.Details.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        //userInfo.LineURI = lineUriDetails[0];
                        //userInfo.LineURIExtension =Convert.ToInt32(lineUriDetails[1]);
                        //Commented because user will raise the request for DCC,DCCS and IDD.System only restore response group.
                        //userInfo.UserType = (EntitlementType)Enum.Parse(typeof(EntitlementType), sfbSnapShotDetails.UserInfo.UserType);
                        //var vp = string.Empty; var vrp = string.Empty; var dp = string.Empty;
                        //GetPolicyByEntitlementType(userInfo.UserType, fields, ref vp, ref vrp, ref dp, sfbSnapShotDetails.UserInfo.HasIDD);
                        //userInfo.VoicePolicy = vp;
                        //userInfo.VoiceRoutingPolicy = vrp;
                        //userInfo.DialPlan = dp;
                        CreateSFBAccountTask(user, userInfo, adhocRequestResponseGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                _idmsLogger.WriteLog(LogLevelType.Exception,$"Skype for Business:UndoOffBoardEvent failed:{ex.Message}", ex);
            }
            return isUpdated;
        }
        private void CreateSFBAccountTask(DomainModel.User user, SkypeForBusinessUserInfo sfbUser,List<ResponseGroup> responseGroup)
        {
            Guid transactionId = Guid.NewGuid();
            AccessRequest accessrequest = new AccessRequest();

            accessrequest.ActivationDate = DateTime.Now;
            accessrequest.Comments = $"Task created for UndoEvent Offboard request:Exisiting {sfbUser.UserPrincipalName}";
            accessrequest.CreatedBy = "System";
            accessrequest.CreatedDate = DateTime.Now;
            accessrequest.EndpointSystemId = this.Endpoint.Id;
            accessrequest.ApplicationType = ApplicationType.MicrosoftTeams;
            accessrequest.RequestSource = AccessRequestSource.AdhocRequest;
            accessrequest.RequestStatus = AccessRequestStatus.Granted;
            accessrequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
            accessrequest.ExecutionTime = DateTime.Now.AddHours(2);
            //Only response group allowed
            //if(sfbUser.UserType == EntitlementType.DCC || sfbUser.UserType == EntitlementType.IDD)
            //{
            //    accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            //    accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
            //}
            accessrequest.TransactionId = transactionId;
            accessrequest.UserId = user.Id;
            accessrequest.RequesterId = user.Id;
            accessrequest.AccountType = AccountType.User;
            //Only response group allowed
            //accessrequest.EventUndoRequestId = this.CurrentRequest.EventUndoRequestId;
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            adhocaccessrequest.AccountType = AccountType.User;
            adhocaccessrequest.AdhocActionType = AdHocActionType.NewAccount;
            adhocaccessrequest.Comments = accessrequest.Comments;
            adhocaccessrequest.CreatedBy = accessrequest.CreatedBy;
            adhocaccessrequest.CreatedDate = accessrequest.CreatedDate;
            adhocaccessrequest.EndpointSystemId = this.Endpoint.Id;
            adhocaccessrequest.EndpointSystemType = EndpointType.MicrosoftTeams;
            adhocaccessrequest.ApplicationType = ApplicationType.MicrosoftTeams;
            adhocaccessrequest.IsSelfRequest = true;
            adhocaccessrequest.RequesteeUserId = user.Id;
            adhocaccessrequest.RequesterUserId = user.Id;
            adhocaccessrequest.ValidFrom = DateTime.Now;
            adhocaccessrequest.ValidTo = null;
            adhocaccessrequest.AccountType = AccountType.User;
            AdhocSkypeForBusinessRequest inRequest = new AdhocSkypeForBusinessRequest()
            {
                EndpointUserId = 0,
                UserType = EntitlementType.Default,
                //DialPlan = sfbUser.DialPlan,
                //VoicePolicy = sfbUser.VoicePolicy,
                //VoiceRoutingPolicy = sfbUser.VoiceRoutingPolicy,
                //LineUri = string.IsNullOrWhiteSpace(sfbUser.LineURI) ? string.Empty : sfbUser.LineURI,
                //Extension = sfbUser.LineURIExtension.HasValue ? sfbUser.LineURIExtension.ToString() : string.Empty,
                EntitlementType = sfbUser.UserType,
            };
            if(responseGroup.Count > 0)
            {
                inRequest.SkypeForBusinessResponseGroups = responseGroup;
            }
            adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(inRequest);

            accessrequest.AdHocAccessRequest = adhocaccessrequest;

            accessrequest.Save(MasterConfigHelper.ServiceUser);
        }
        private bool UndoTransferEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var domain = MasterConfigHelper.O365EmailDomainName;
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                var o365 = EndPoint.GetByApplicationType(ApplicationType.Office365);
                var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, o365.Id);

                if (adUserInfo == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"AD Account does not exists/Locked for the User:{user.AccountName}");
                    return false;
                }
                if (o365User == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Office365 User Account does not exists for the User:{user.AccountName}");
                    return false;
                }
                var userPrincipalName = $"{adUserInfo.sAMAccountName}@{domain}";
                var snapshot = OUChangeSnapshot.GetByUserId(user.Id);
                if (snapshot != null)
                {
                    if (this.CurrentRequest.EventUndoRequest != null && !string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalGroups))
                    {
                        var sfbUserInfo = SkypeForBusinessUserInfo.GetByUserPrincipalName(userPrincipalName);
                        if (sfbUserInfo != null)
                        {
                            var sfbSnapShotDetails = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(this.CurrentRequest.EventUndoRequest.TechnicalGroups);
                            if(sfbSnapShotDetails == null || sfbSnapShotDetails.UserInfo == null)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UndoEvent-Transfer: Transfer:No entitlements found for the User:{user.AccountName}");
                                return true;
                            }
                            var sfbEntitlements = sfbSnapShotDetails.Entitlements;
                            List<string> entitlements = new List<string>();
                           
                            var sfbRequestDetail = new AdhocSkypeForBusinessRequest();
                            sfbRequestDetail.EndpointUserId = sfbUserInfo.Id;
                            sfbRequestDetail.EntitlementType = EntitlementType.ResponseGroup;
                            
                            _idmsLogger.WriteLog(LogLevelType.Info, $"UndoEvent-Transfer:AddEntitlement{string.Join(",", entitlements)}for the user {user.AccountName} started");
                            var responseGroups = sfbEntitlements.Where(i => i.EntitlementType == EntitlementType.ResponseGroup);
                            if (responseGroups.Any())
                            {
                                sfbRequestDetail.UserType = sfbUserInfo.UserType;
                                sfbRequestDetail.SkypeForBusinessResponseGroups = new List<ResponseGroup>();
                                entitlements = new List<string>();
                                var userInfo = SkypeForBusinessUserInfo.GetByUserPrincipalName(sfbUserInfo.UserPrincipalName);
                                
                                var maskedGroup = new Dictionary<string, bool>();
                                foreach (var responseGroup in responseGroups)
                                {
                                    var sfbGroup = SkypeForBusinessResponseGroupInfo.GetGroupByName(responseGroup.Entitlement);
                                    if (sfbGroup != null)
                                    {
                                        var matrix = SkypeForBusinessProvisioningMatrix.GetByName(sfbGroup.Name, EntitlementType.ResponseGroup);
                                        var grp = new ResponseGroup()
                                        {
                                            Name = sfbGroup.Name,
                                            Id = sfbGroup.Id,
                                            Identity = sfbGroup.Identity,
                                        };
                                        if (!string.IsNullOrWhiteSpace(responseGroup.Function) && responseGroup.Function.Equals(functionMasked))
                                        {
                                            grp.Mask = true;
                                        }
                                        
                                        sfbRequestDetail.SkypeForBusinessResponseGroups.Add(grp);
                                        entitlements.Add(responseGroup.Entitlement);
                                    }
                                }
                                if(entitlements.Count > 0)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"UndoEvent-Transfer:Add responsegroup started");
                                    var isSuccessFul = AmendmentAddForResponseGroup(user, userInfo, sfbRequestDetail, entitlements);
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"UndoEvent-Transfer:Add responsegroup status:{isSuccessFul}");
                                }
                                isUpdated = true;
                            }
                            _idmsLogger.WriteLog(LogLevelType.Info, $"UndoEvent-Transfer:AddEntitlement{string.Join(",", entitlements)}for the user {user.AccountName} completed");
                        }
                    }
                }
                isUpdated = true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UndoEvent-Transfer: failed for  {ex.Message}",ex);
                isUpdated = false;
            }
            return isUpdated;
        }

        private void AddOrUpdateAgentUri(SkypeForBusinessUserInfo sfbUserInfo, long groupId, int masked)
        {
            try
            {
                SkypeForBusinessUserInfo.InsertUserGroup(sfbUserInfo.EndPointSystemId, sfbUserInfo.Id, groupId, masked);

                if (sfbUserInfo.Id > 0 && groupId > 0)
                {
                    var sfbGroup = SkypeForBusinessResponseGroupInfo.GetById(groupId);
                    if (sfbGroup != null && (string.IsNullOrWhiteSpace(sfbGroup.AgentsByUri) || !sfbGroup.AgentsByUri.Contains(sfbUserInfo.UserPrincipalName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (string.IsNullOrWhiteSpace(sfbGroup.AgentsByUri))
                        {
                            sfbGroup.AgentsByUri = sfbUserInfo.UserPrincipalName;
                        }
                        else
                        {
                            sfbGroup.AgentsByUri = $"{sfbGroup.AgentsByUri},{sfbUserInfo.UserPrincipalName}";
                        }
                        sfbGroup.Save("System");
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UndoEvent-Transfer:AddOrUpdateAgentUri failed for  {ex.Message}", ex);
            }
        }
    }
}

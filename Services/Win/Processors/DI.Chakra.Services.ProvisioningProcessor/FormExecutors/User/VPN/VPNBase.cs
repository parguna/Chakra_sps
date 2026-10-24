using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Newtonsoft.Json;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class VPNBase : FormExecutorBase
    {
        private string currentUser;
        private static char[] delimiter = new char[] { ',' };
        public VPNBase(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger):base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
            currentUser =  MasterConfigHelper.ServiceUser;
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;

            AdHocVPNRequest vpnDetail = null;
            long endpointUserId = 0;

            if (user == null)
            {
                WriteProvisioningLog(LogLevelType.Warning, "VPN Provisioning failed. User object is null");
                return false;
            }

            //if adhoc request, get prov details from adhoc request object
            string leveldetail = string.Empty;
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                vpnDetail = (AdHocVPNRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                if (vpnDetail != null)
                {
                    endpointUserId = vpnDetail.EndpointUserId;
                }
            }
            switch (CurrentRequest.AdHocAccessRequest.AdhocActionType)
            {
                case AdHocActionType.NewAccount:
                    isExecutedSuccessfully = CreateAccount(fields, user, leveldetail, vpnDetail);
                    break;
                case AdHocActionType.AmendmentAdd:
                    isExecutedSuccessfully = AmendentAdd(user, leveldetail, fields);
                    break;
                case AdHocActionType.AmendmentRemove:
                    isExecutedSuccessfully = AmendentRemove(user,endpointUserId, leveldetail, fields, vpnDetail);
                    break;
                case AdHocActionType.DeleteAccount:
                    isExecutedSuccessfully = DeleteAccount(user, endpointUserId);
                    break;
                
                default:
                    break;
            }
            return isExecutedSuccessfully;
        }

        private bool AmendentRemove(DomainModel.User user, long endpointUserId, string leveldetail, ProvisioningParamCollection fields, AdHocVPNRequest vpnAdhocRequest)
        {
            var isSuccess = false;
            try
            {
                isSuccess= RemoveUserFromGroup(user, endpointUserId,vpnAdhocRequest, true);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning(AmendentRemove) failed for the user{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private bool AmendentAdd(DomainModel.User user, string leveldetail, ProvisioningParamCollection fields)
        {
            
            var isSuccess = false;
            try
            {
                var vpnAdhocRequest = (AdHocVPNRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (vpnAdhocRequest.UpdateOneKeyToken)
                {
                    isSuccess = UpdateUserOneKeyTokenId(user, vpnAdhocRequest);
                }
                else
                {
                    isSuccess = AddUserToGroup(user, vpnAdhocRequest, true, false);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning(AmendentAdd) failed for the user:{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            var success= DeleteVPNAccount(user, endpointUserId);
            if (success)
            {
                user.OneKeyTokenId = null;
                user.Save(currentUser);
            }
            return success;
        }
        private bool DeleteVPNAccount(DomainModel.User user, long endpointUserId,bool isOffboard=false)
        {
            var isSuccess = false;
            try
            {
                var vpnAdhocRequest = (AdHocVPNRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                var adGroups = new List<string>();
                //Delete the User from ALL the group
                var entRecons = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(vpnAdhocRequest.EndpointUserId, Endpoint.Id);
                var vpnUserApplnEnvIds = entRecons.Select(i => i.EntitlementId).ToList();
                if(entRecons.Count > 0)
                {
                    var vpnUserAppln = VPNUserApplicationEnviornment.GetByIds(vpnUserApplnEnvIds, vpnAdhocRequest.EndpointUserId);
                    adGroups = GetVPNApplicationADGroups(vpnUserAppln);
                }
                if (adGroups.Count > 0)
                {
                    var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, adEndpoint.Id, AccountType.User);
                    if (adUsers != null && adUsers.Count > 0)
                    {
                        var adUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(adUsers.FirstOrDefault().Id);
                        if(adUserInfo != null)
                        {
                            adUserInfo.Groups = adGroups;
                            var ADGroups = adUserInfo.ADSGroups;
                            adUserInfo.Groups = adUserInfo.Groups.Where(i => ADGroups.Exists(j => j.name.Equals(i, StringComparison.InvariantCultureIgnoreCase))).ToList();
                            var groups = string.Join(",", adUserInfo.Groups);
                            ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, CurrentRequest);
                            WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning DeleteAccount:Remove User from ADGroups:{groups}");
                            if (adConnector.RemoveEntitlement(adUserInfo, EntitlementType.Group))
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning DeleteAccount:Remove User from ADSUserGroups:{groups}");
                                ADSUserInfo.RemoveEntitlement(adUserInfo.Id, EntitlementType.Group, groups);
                            }
                            else
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning DeleteAccount:Remove User from ADGroups:{groups} failed");
                            }
                        }
                    }
                }
                WriteProvisioningLog(LogLevelType.DebugStep, $"VPM Provisioning DeleteAccount:Remove User Entitlement started");
                entRecons.ForEach(entRecon =>
                {
                    entRecon.IsDeleted = true;
                    entRecon.DeletedDate = DateTime.Now;
                    entRecon.Save(currentUser);
                });
                WriteProvisioningLog(LogLevelType.DebugStep, $"VPM Provisioning DeleteAccount:Remove User Entitlement completed");
                WriteProvisioningLog(LogLevelType.DebugStep, $"VPM Provisioning DeleteAccount:Remove User from AccountReconcilation started");
                var accounRecon = AccountReconcilation.GetByEndpointUserId(vpnAdhocRequest.EndpointUserId, Endpoint.Id);
                if (accounRecon != null)
                {
                    accounRecon.IsLocked = true;
                    accounRecon.IsDeleted = true;
                    accounRecon.DeletedDate = DateTime.Now;
                    accounRecon.Save(currentUser);
                    WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning DeleteAccount:Remove User from AccountReconcilation completed");
                }
                VPNUserApplicationEnviornment.DeleteList(vpnUserApplnEnvIds);
                WriteProvisioningLog(LogLevelType.DebugStep, $"VPM Provisioning DeleteAccount:Remove VPNUserInfo started");
                VPNUserInfo.DeleteByEndpointUserId(this.Endpoint.Id, vpnAdhocRequest.EndpointUserId);
                WriteProvisioningLog(LogLevelType.DebugStep, $"VPM Provisioning DeleteAccount:Remove VPNUserInfo completed");
                
                if (isOffboard)
                {
                    ProvisioningEmailHelper.SendEmailAfterOffBoardingTheUser(user);
                }
                else
                {
                    ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, Endpoint, CurrentRequest, accounRecon.AccountName);
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning DeleteAccount failed for the user{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private List<string> GetVPNApplicationADGroups(List<VPNUserApplicationEnviornment> vpnUserAppln)
        {
            var adgroups = new List<string>();
            foreach (var usrAppln in vpnUserAppln)
            {
                usrAppln.ADGroups.Split(delimiter).ToList().ForEach(j => adgroups.Add(j.Trim()));
            }
            adgroups = adgroups.Distinct().ToList();
            return adgroups;
        }

        private List<string> GetADGroupFromMatrix(List<VPNMatrix> vpnMatrix)
        {
            var adGroups = new List<string>();

            foreach (var vpn in vpnMatrix)
            {
                if (!string.IsNullOrWhiteSpace(vpn.ProductionADgroups))
                {
                    vpn.ProductionADgroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i=> adGroups.Add(i));
                }
                if (!string.IsNullOrWhiteSpace(vpn.QAADGroups))
                {
                    vpn.QAADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i));
                }
                if (!string.IsNullOrWhiteSpace(vpn.DevelpomentADGroups))
                {
                    vpn.DevelpomentADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i));
                }
                if (!string.IsNullOrWhiteSpace(vpn.ADGroups))
                {
                    vpn.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i));
                }
            }
            return adGroups.Distinct().ToList();
        }

        private bool CreateAccount(ProvisioningParamCollection fields, DomainModel.User user, string leveldetail, AdHocVPNRequest vpnDetail)
        {
            var isSuccess = false;
            try
            {
                if(vpnDetail!=null && string.IsNullOrWhiteSpace(vpnDetail.OneKey))
                {
                    throw new Exception($"VPN CreateAccount:Token Number is empty for the User:{user.DisplayName}");
                }
                isSuccess = AddUserToGroup(user, vpnDetail, true, true);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning(DeleteAccount) failed for the user{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        
        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            var isSuccess = false;
            try
            {
                if (string.IsNullOrWhiteSpace(user.OneKeyTokenId))
                {
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Offboard:Token Number not found for the user {user.AccountName}. Hence making current request as completed");
                    return true;
                }
                var endpointUser = VPNUserInfo.GetByOnekeyTokenId(user.OneKeyTokenId);
                if(this.CurrentRequest.EmployeeEventTriggerId == null)
                {
                    var ouSnapShot = OUChangeSnapshot.GetByUserId(user.Id).FirstOrDefault();
                    this.TakeOUSnapshot(user.Id, endpointUser, EmployeeEvents.Resignation, EndpointType.VPN, EntitlementType.Application, ouSnapShot?.BatchId);
                    var vpnDetails = (AdHocVPNRequest)this.CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    if (vpnDetails.IsOffboardOrTransferRequest)
                    {
                        if(endpointUser != null)
                        {
                            DeleteVPNAccount(user, endpointUser.Id, true);
                        }
                        
                        user.OneKeyTokenId = null;
                        user.Save(currentUser);
                        isSuccess = true;
                    }
                    else
                    {
                        CreateVPNDeleteTaskForITSecurity(user, LockScenario.OffBoard, endpointUser);
                        isSuccess = true;
                    }
                }   
                else
                {
                    CreateVPNDeleteTaskForITSecurity(user, LockScenario.OffBoard, endpointUser);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning(Offboard) failed for the user{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
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
            var isSuccess = false;
            try
            {
                if (string.IsNullOrWhiteSpace(user.OneKeyTokenId))
                {
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning(Transfer):Token Number not found for the user {user.AccountName}. Hence making current request as completed.");
                    return true;
                }
                var endpointUser = VPNUserInfo.GetByOnekeyTokenId(user.OneKeyTokenId);
                if (this.CurrentRequest.EmployeeEventTriggerId == null)
                {
                    var vpnDetails = (AdHocVPNRequest)this.CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    if (vpnDetails.IsOffboardOrTransferRequest)
                    {
                        var ouSnapShot = OUChangeSnapshot.GetByUserId(user.Id).FirstOrDefault();
                        this.TakeOUSnapshot(user.Id, endpointUser, EmployeeEvents.OUChange,EndpointType.VPN,EntitlementType.Application, ouSnapShot?.BatchId);
                        var vpnAdhocRequest = (AdHocVPNRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                        var entRecons = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(vpnAdhocRequest.EndpointUserId, Endpoint.Id);
                        var vpnUserApplnEnvIds = entRecons.Select(i => i.EntitlementId).ToList();
                        WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning Transfer:Remove User Entitlement started");
                        entRecons.ForEach(entRecon =>
                        {
                            entRecon.IsDeleted = true;
                            entRecon.DeletedDate = DateTime.Now;
                            entRecon.Save(currentUser);
                        });
                        WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning Transfer:Remove User Entitlement completed");
                        VPNUserApplicationEnviornment.DeleteList(vpnUserApplnEnvIds);
                        var accounRecon = AccountReconcilation.GetByEndpointUserId(vpnAdhocRequest.EndpointUserId, Endpoint.Id);
                        if (accounRecon != null)
                        {
                            accounRecon.IsLocked = true;
                            accounRecon.IsDeleted = true;
                            accounRecon.DeletedDate = DateTime.Now;
                            accounRecon.Save(currentUser);
                            WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning Transfer:Remove User from AccountReconcilation completed");
                        }
                        WriteProvisioningLog(LogLevelType.Info, $"VPM Provisioning Transfer:Remove VPNUserInfo started");
                        VPNUserInfo.DeleteByEndpointUserId(this.Endpoint.Id, vpnAdhocRequest.EndpointUserId);
                        user.OneKeyTokenId = null;
                        user.Save(currentUser);
                        ProvisioningEmailHelper.SendEmailAfterExecutingOUChangeRequest(user);
                        isSuccess = true;
                    }
                    else
                    {
                        CreateVPNDeleteTaskForITSecurity(user, LockScenario.OUChange, endpointUser);
                        isSuccess = true;
                    }
                }
                else
                {
                    CreateVPNDeleteTaskForITSecurity(user, LockScenario.OUChange, endpointUser);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"VPM Provisioning(Transfer) failed for the user{user.AccountName}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        private bool UpdateUserOneKeyTokenId(DomainModel.User user,AdHocVPNRequest vpnDetail)
        {
            bool isSuccess = true;
            try
            {
                if (vpnDetail.EndpointUserId == 0) throw new Exception("Invalid EndpointUserId");

                var vpnUserInfo = VPNUserInfo.GetByEndpointUserId(vpnDetail.EndpointUserId);
                if (vpnUserInfo != null)
                {
                    var aRecon = AccountReconcilation.GetByAccountName(vpnUserInfo.OneKeyTokenId, this.Endpoint.Id);
                    if(aRecon != null)
                    {
                        aRecon.AccountName = vpnDetail.OneKey;
                        aRecon.LastModifiedDate = DateTime.Now;
                        aRecon.LastModifiedBy = currentUser;
                        aRecon.Save(currentUser);
                    }
                    vpnUserInfo.OneKeyTokenId = vpnDetail.OneKey;
                    vpnUserInfo.Save(currentUser);

                    var usr = DomainModel.User.GetById(user.Id);
                    if (usr != null)
                    {
                        usr.OneKeyTokenId = vpnDetail.OneKey;
                        usr.Save(currentUser);
                    }

                    if(vpnDetail.AdHocVPNRequestDetails.Count > 0)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Add Application/Projects started");
                        isSuccess = this.AddUserToGroup(user, vpnDetail, false, false,false);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Add Application/Projects completed");
                    }

                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} sending email started");
                    ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, $"<br/>Update Token Number:<br/><b style='color:green'>New:{vpnDetail.OneKey}</b><br/><b style='color:red'>Old:{user.OneKeyTokenId}</b><br/> ].", user.UserName);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} sending email completed");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"VPN Provisioning Update Token Number failed for the User:{user.AccountName}, Token Number {vpnDetail.OneKey}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        
        private bool AddUserToGroup(DomainModel.User user, AdHocVPNRequest vpnDetail,bool sendEmail,bool isNewAccount,bool allowedMailOption=true)
        {
            bool IsSuccess = true;
            try
            {
                if (vpnDetail == null)
                    throw new Exception("AdHocVPNRequest is null");

                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, adEndpoint.Id, AccountType.User);
                if (adUsers.Count == 0)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning: Active Directory User not found for the User:{user.AccountName} Token Number {vpnDetail.OneKey}");
                    return false;
                }

                var adUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(adUsers.FirstOrDefault().Id);
                var adGroups = new List<string>();
                var tempADGroups = new List<string>();
                adUserInfo.Groups.Clear();
                var userApplnEnviornments = new List<VPNApplicationEnvironment>();
                foreach (var adHocVPNRequest in vpnDetail.AdHocVPNRequestDetails)
                {
                    var vpnUserApplnEnv = VPNApplicationEnvironment.GetByMatrixId(adHocVPNRequest.VPNMatrixId);
                    if (vpnUserApplnEnv.Count > 0)
                    {
                        foreach (var item in vpnUserApplnEnv)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ADGroups) && item.EnviornmentType == VPNMatrixEnviornmentType.Production && adHocVPNRequest.Production)
                            {
                                item.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i.Trim()));
                                userApplnEnviornments.Add(item);
                            }
                            else if (!string.IsNullOrWhiteSpace(item.ADGroups) && item.EnviornmentType == VPNMatrixEnviornmentType.QA && adHocVPNRequest.QA)
                            {
                                item.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i.Trim()));
                                userApplnEnviornments.Add(item);
                            }
                            else if (!string.IsNullOrWhiteSpace(item.ADGroups) && item.EnviornmentType == VPNMatrixEnviornmentType.Development && adHocVPNRequest.Dev)
                            {
                                item.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i.Trim()));
                                userApplnEnviornments.Add(item);
                            }
                            else if (!string.IsNullOrWhiteSpace(item.ADGroups) && item.EnviornmentType == VPNMatrixEnviornmentType.ADGroup && adHocVPNRequest.isOthers)
                            {
                                item.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => adGroups.Add(i.Trim()));
                                userApplnEnviornments.Add(item);
                            }
                        }
                    }
                    adUserInfo.Groups.AddRange(adGroups);
                }

                userApplnEnviornments = userApplnEnviornments.Distinct().ToList();
                adUserInfo.Groups = adUserInfo.Groups.Distinct().ToList();
                bool isAdRequired = true;
                if (adUserInfo.Groups.Count > 0)
                {
                    var ADGroups = adUserInfo.ADSGroups;
                    var grps = string.Join(",", adUserInfo.Groups);
                    WriteProvisioningLog(LogLevelType.DebugStep, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} create AD Connector");
                    ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, CurrentRequest);
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Create VPN Entitlement(ADGroup: {grps})");
                    adUserInfo.Groups = adUserInfo.Groups.Where(i => !ADGroups.Exists(j => j.name.Equals(i, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    if (adConnector.AddEntitlement(adUserInfo, EntitlementType.Group))
                    {
                        ADSUserInfo.AddEntitlement(adUserInfo.Id, EntitlementType.Group, grps);
                        isAdRequired = true;
                    }
                    else
                    {
                        isAdRequired = false;
                        IsSuccess = false;
                        WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning:AddEntitlement:{grps} User:{user.AccountName} Token Number {vpnDetail.OneKey} Create VPN Entitlement(ADGroup) failed");
                    }
                }
                if (isAdRequired)
                {
                    VPNUserInfo vpnUserInfo = null;
                    #region Create VPNUser
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Create/Update VPNUserInfo started");
                    if (isNewAccount)
                    {
                        vpnUserInfo = VPNUserInfo.GetByOnekeyTokenId(vpnDetail.OneKey);
                        if (vpnUserInfo == null)
                        {
                            vpnUserInfo = new VPNUserInfo();
                            vpnUserInfo.UserId = user.Id;
                            vpnUserInfo.DisplayName = user.DisplayName;
                            vpnUserInfo.EndPointSystemId = this.Endpoint.Id;
                            vpnUserInfo.OneKeyTokenId = vpnDetail.OneKey;
                            vpnUserInfo.UserName = user.EmployeeId;
                            vpnUserInfo.Save(currentUser);
                        }
                    }
                    else
                    {
                        vpnUserInfo = VPNUserInfo.GetByEndpointUserId(vpnDetail.EndpointUserId);
                    }
                    #endregion
                    #region Update User with OneKeyToken
                    if (isNewAccount)
                    {
                        var usr = DomainModel.User.GetById(user.Id);
                        usr.OneKeyTokenId = vpnDetail.OneKey;
                        usr.Save(currentUser);
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Create/Update VPNUserInfo completed");
                    #endregion
                    #region create AccountRecon & Entitlement Recon
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Create/Update AccountRecon started");
                    var accountRecon = AccountReconcilation.GetByAccountName(vpnUserInfo.OneKeyTokenId, this.Endpoint.Id);
                    if (accountRecon == null)
                    {
                        accountRecon = new AccountReconcilation();
                        accountRecon.FullName = user.DisplayName;
                        accountRecon.AccountName = vpnUserInfo.OneKeyTokenId;
                        accountRecon.UserId = user.Id;
                        AccountReconcilation.UpdateException(accountRecon, true);
                        accountRecon.AccountType = AccountType.User;
                        accountRecon.EndpointSystemId = Endpoint.Id;
                        accountRecon.EndpointUserId = vpnUserInfo.Id;
                        accountRecon.CreatedDate = DateTime.Now;
                        accountRecon.ValidFromDate = CurrentRequest.ActivationDate;
                        accountRecon.ValidToDate = GetValidTo(user.Id);
                        accountRecon.CreatedBy = currentUser;
                        accountRecon.Save(currentUser);
                    }
                    else
                    {
                        if (accountRecon.IsDeleted)
                        {
                            accountRecon.ValidFromDate = CurrentRequest.ActivationDate;
                            accountRecon.ValidToDate = GetValidTo(user.Id);
                        }
                        accountRecon.FullName = user.DisplayName;
                        accountRecon.EndpointUserId = vpnUserInfo.Id;
                        accountRecon.IsDeleted = false;
                        accountRecon.DeletedDate =null;
                        accountRecon.AccountName = vpnUserInfo.OneKeyTokenId;
                        accountRecon.UserId = user.Id;
                        accountRecon.IsLocked = false;
                        if (accountRecon.EndpointUserId == 0)
                        {
                            accountRecon.EndpointUserId = vpnUserInfo.Id;
                        }
                        accountRecon.Save(currentUser);
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Create/Update AccountRecon completed");
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Creating entitlement started");
                    foreach (var userApplnEnviornment in userApplnEnviornments)
                    {
                        var data = VPNUserApplicationEnviornment.GetByNameAndType(accountRecon.EndpointUserId, userApplnEnviornment.Name, userApplnEnviornment.MatrixType, userApplnEnviornment.EnviornmentType, userApplnEnviornment.MatrixId);
                        if (data == null)
                        {
                            data = new VPNUserApplicationEnviornment();
                            data.Name = userApplnEnviornment.Name;
                            data.MatrixType = userApplnEnviornment.MatrixType;
                            data.EnviornmentType = userApplnEnviornment.EnviornmentType;
                            data.EndpointUserId = accountRecon.EndpointUserId;
                            data.ADGroups = userApplnEnviornment.ADGroups;
                            data.MatrixId = userApplnEnviornment.MatrixId;
                            data.CreatedBy = currentUser;
                            data.CreatedDate = DateTime.Now;
                            data.Save(currentUser);
                        }
                        else
                        {
                            data.ADGroups = userApplnEnviornment.ADGroups;
                            data.EndpointUserId = accountRecon.EndpointUserId;
                            data.MatrixId = userApplnEnviornment.MatrixId;
                            data.LastModifiedBy = currentUser;
                            data.LastModifiedDate = DateTime.Now;
                            data.Save(currentUser);
                        }
                        var entitlementType = data.MatrixType == VPNMatrixType.Application ? EntitlementType.Application : EntitlementType.Project;

                        var vpnEntitlement = this.CreateEntitlementReconcilationShell(accountRecon, entitlementType, data.Name);
                        vpnEntitlement.Entitlement = data.Name;
                        vpnEntitlement.EntitlementDescription = data.ADGroups;
                        vpnEntitlement.EntitlementId = data.Id;
                        vpnEntitlement.Functions = data.EnviornmentType.ToString();
                        var entRecon = EntitlementReconcilation.GetByEntitlement(accountRecon.EndpointSystemId, accountRecon.Id, data.Name, (int)entitlementType);
                        if (entRecon == null)
                        {
                            entRecon = vpnEntitlement;
                            entRecon.EndpointUserId = vpnUserInfo.Id;
                            EntitlementReconcilation.UpdateException(vpnEntitlement, true);
                            entRecon.CreatedDate = DateTime.Now;
                            entRecon.CreatedBy = "System";
                            entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                            entRecon.ValidToDate =GetValidTo(user.Id);
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                        else
                        {
                            entRecon.ValidFromDate = CurrentRequest.ActivationDate;
                            entRecon.ValidToDate = GetValidTo(user.Id);
                            entRecon.EndpointUserId = vpnUserInfo.Id;
                            entRecon.LastModifiedDate = DateTime.Now;
                            entRecon.LastModifiedBy = "System";
                            entRecon.Entitlement = vpnEntitlement.Entitlement;
                            entRecon.EntitlementId = vpnEntitlement.EntitlementId;
                            entRecon.EntitlementDescription = vpnEntitlement.EntitlementDescription;
                            if (entRecon.IsDeleted)
                            {
                                EntitlementReconcilation.UpdateException(entRecon, true);
                            }
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} Creating entitlement completed");
                    #endregion
                    if (sendEmail && allowedMailOption)
                    {
                        var entitlements = string.Join(",", userApplnEnviornments.Select(i => i.Name));
                        _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} sending email started");
                        if (isNewAccount)
                        {
                            ProvisioningEmailHelper.SendEmailAfterVPNCreateAccount(user, CurrentRequest, entitlements, adUserInfo.sAMAccountName);
                        }
                        else
                        {
                            ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, entitlements, adUserInfo.sAMAccountName);
                        }
                        _idmsLogger.WriteLog(LogLevelType.Info, $"VPN Provisioning: User:{user.AccountName} Token Number {vpnDetail.OneKey} sending email completed");
                    }
                }
                
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, "VPN Provisioning failed. CreateAccount", ex);
                IsSuccess = false;
            }
            return IsSuccess;
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
        protected EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, EntitlementType _entitlementType, string Entitlements = "")
        {
            return new EntitlementReconcilation()
            {
                Entitlement = Entitlements,
                IsSODConflict = false,
                IsSensitiveAccess = false,
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                EntitlementType = (int)_entitlementType,
                EndpointType = (EndpointType)this.Endpoint.EndpointSystemTypeId,
            };
        }
        private bool RemoveUserFromGroup(DomainModel.User user,long endpointUserId, AdHocVPNRequest vpnAdhocAccessRequest,bool sendEmail)
        {
            bool IsSuccess = true;
            try
            {
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUsers = ADSUserInfo.GetActiveAccounts(user.Id, adEndpoint.Id, AccountType.User);
                if (adUsers.Count == 0) return false;
                var adUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(adUsers.FirstOrDefault().Id);
                var adGroups = new List<string>();
                var removeAllVPNEnviornment = new List<VPNUserApplicationEnviornment>();
                var vpnMatrixForSpecificEnviornment = new List<VPNMatrix>();

                string ProdRemove = string.Empty;
                string QARemove = string.Empty;
                string DevRemove = string.Empty;
                var entitlementRecons = EntitlementReconcilation.GetByEndpointUser(endpointUserId, this.Endpoint.Id);
                var tempADGroups = new List<string>();
                foreach (var adHocVPNRequest in vpnAdhocAccessRequest.AdHocVPNRequestDetails)
                {
                    var vpnUserApplnEnvs = VPNUserApplicationEnviornment.GetByMatrixId(adHocVPNRequest.VPNMatrixId, endpointUserId);
                    foreach (var vpnUserApplnEnv in vpnUserApplnEnvs)
                    {
                        if (!string.IsNullOrWhiteSpace(vpnUserApplnEnv.ADGroups) && 
                            ((adHocVPNRequest.Production && vpnUserApplnEnv.EnviornmentType == VPNMatrixEnviornmentType.Production)||
                            (adHocVPNRequest.QA && vpnUserApplnEnv.EnviornmentType == VPNMatrixEnviornmentType.QA) ||
                            (adHocVPNRequest.Dev && vpnUserApplnEnv.EnviornmentType == VPNMatrixEnviornmentType.Development) ||
                            (adHocVPNRequest.isOthers && vpnUserApplnEnv.EnviornmentType == VPNMatrixEnviornmentType.ADGroup)
                            ))
                        {
                            tempADGroups.Clear();
                            vpnUserApplnEnv.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => tempADGroups.Add(i.Trim()));
                            foreach (var adGroup in tempADGroups)
                            {
                                var isOtherAppln = VPNUserApplicationEnviornment.GetUserApplicationEnviornment(vpnAdhocAccessRequest.EndpointUserId, vpnUserApplnEnv.Name);
                                if (isOtherAppln.Count > 0)
                                {
                                    var isExists = isOtherAppln.Exists(i => i.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).Contains(adGroup));
                                    if (!isExists)
                                    {
                                        adGroups.Add(adGroup);
                                        removeAllVPNEnviornment.Add(vpnUserApplnEnv);
                                    }
                                }
                                else
                                {
                                    adGroups.Add(adGroup);
                                    removeAllVPNEnviornment.Add(vpnUserApplnEnv);
                                }
                            }
                        }
                    }
                }
                adGroups = adGroups.Distinct().ToList();
                adUserInfo.Groups = adGroups;
                if (adUserInfo.Groups.Count > 0)
                {
                    var ADGroups = adUserInfo.ADSGroups;
                    adUserInfo.Groups = adUserInfo.Groups.Where(i => ADGroups.Exists(j => j.name.Equals(i, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    var groups = string.Join(",", adUserInfo.Groups);
                    ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, CurrentRequest);
                    if(adConnector.RemoveEntitlement(adUserInfo, EntitlementType.Group))
                    {
                        if(ADSUserInfo.RemoveEntitlement(adUserInfo.Id, EntitlementType.Group, groups))
                        {
                            var vpnUser = VPNUserInfo.GetById(endpointUserId);
                            foreach (var appln in removeAllVPNEnviornment)
                            {
                                var entitlementType = appln.MatrixType == VPNMatrixType.Application ? EntitlementType.Application : EntitlementType.Project;
                                var entitlementRecon = EntitlementReconcilation.GetByUser(vpnUser.Id, this.Endpoint.Id, appln.Name, entitlementType);
                                if (entitlementRecon != null)
                                {
                                    if (!entitlementRecon.IsDeleted)
                                    {
                                        entitlementRecon.IsDeleted = true;
                                        entitlementRecon.DeletedDate = DateTime.Now;
                                        entitlementRecon.ExceptionType = ExceptionTypes.Removed;
                                        entitlementRecon.ExceptionOccuredDate = DateTime.Now;
                                        // clearing resolved flag so that this item shows up in reconciliation again
                                        entitlementRecon.IsResolved = false;
                                        entitlementRecon.ResolvedBy = null;
                                        entitlementRecon.LastAttestedOn = null;
                                        entitlementRecon.Save(currentUser);
                                    }
                                }
                                VPNUserApplicationEnviornment.Delete(appln.Id);
                            }

                            if (sendEmail && adUserInfo.Groups.Count > 0)
                            {
                                ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, string.Join(",", removeAllVPNEnviornment.Select(i=>i.Name)), adUserInfo.sAMAccountName);
                            }
                        }
                    }
                }
                else
                {
                    IsSuccess = true;
                    WriteProvisioningLog(LogLevelType.Info, $"VPN Provisioning: 0 Groups found for the User:{user.AccountName} Token Number {vpnAdhocAccessRequest.OneKey}");
                }
               
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Error, "VPN Provisioning failed. CreateAccount", ex);
                IsSuccess = false;
            }
            return IsSuccess;
        }
        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = true;
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"VPN:UndoEvent started");
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adUserInfo = ADSUserInfo.GetByDIUserIdByEndPointId(user.Id, adEndpoint.Id);

                if (adUserInfo == null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN UndoEvent: AD Account does not exists/Locked for the User:{user.DisplayName}");
                    return false;
                }
                var snapshot = OUChangeSnapshot.GetByUserId(user.Id);
                if (snapshot != null && this.CurrentRequest.EventUndoRequest != null && !string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalGroups))
                {
                    var vpnUserDetail = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(this.CurrentRequest.EventUndoRequest.TechnicalGroups);
                    if (vpnUserDetail == null || vpnUserDetail.UserInfo == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"VPN UndoEvent: Snapshot not found:{user.DisplayName}");
                        return false;
                    }
                    if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                    {
                        isUpdated = UnodEventTransferAndOffboard(user, vpnUserDetail, adUserInfo);
                    }
                    else if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                    {
                        isUpdated = UnodEventTransferAndOffboard(user, vpnUserDetail, adUserInfo);
                    }
                }else
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"VPN UndoEvent:snapshot is empty or no entitlement found:{user.DisplayName}");
                }
                _idmsLogger.WriteLog(LogLevelType.Exception, $"VPN:UndoEvent completed");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"VPN: Error while executing the undo request. { ex.Message}", ex);
                isUpdated =false;
            }   
            return isUpdated;
        }

        private bool UnodEventTransferAndOffboard(DomainModel.User user, SnapshotUserDetails<SnapshotUser> vpnUserDetail,ADSUserInfo adsUserInfo)
        {
            bool isSuccess = true;
            try
            {
                var delimiter = new char[] { ',' };
                var vpnApplnEnviornments = new List<VPNApplicationEnvironment>();
                if (vpnUserDetail.Entitlements != null)
                {
                    foreach (var Entitlement in vpnUserDetail.Entitlements)
                    {
                        if (!string.IsNullOrWhiteSpace(Entitlement.Entitlement))
                        {

                            var vpnMatrix = VPNApplicationEnvironment.GetByEntitlementAndMatrixType(Entitlement.Entitlement, (int)Entitlement.EntitlementType);
                            if (vpnMatrix != null)
                            {
                                if (!string.IsNullOrWhiteSpace(vpnMatrix.ADGroups))
                                {
                                    vpnApplnEnviornments.Add(vpnMatrix);
                                }
                            }
                        }
                    }
                }
                
                CreateVPNAccountTaskForITSecurity(user, vpnUserDetail.UserInfo.UserName, vpnApplnEnviornments);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"VPN: Error while executing the UnodEventTransferAndOffboard request. { ex.Message}", ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private void CreateVPNDeleteTaskForITSecurity(DomainModel.User user,LockScenario lockScenario, VPNUserInfo vpnUser)
        {
            Guid transactionId = Guid.NewGuid();
            AccessRequest accessrequest = new AccessRequest();

            accessrequest.ActivationDate = DateTime.Now;
            accessrequest.Comments = "Task for IAM Team to check token returned";
            accessrequest.CreatedBy = "System";
            accessrequest.CreatedDate = DateTime.Now;
            accessrequest.DeactivationDate = DateTime.Now;
            accessrequest.EndpointSystemId = this.Endpoint.Id;
            accessrequest.ApplicationType = ApplicationType.VPN;
            if (lockScenario == LockScenario.OffBoard)
            {
                accessrequest.RequestSource = AccessRequestSource.OffBoarding;
            }
            else
            {
                accessrequest.RequestSource = AccessRequestSource.OUChange;
            }

            accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
            accessrequest.TransactionId = transactionId;
            accessrequest.UserId = user.Id;
            accessrequest.RequesterId = user.Id;
            accessrequest.AccountType = AccountType.User;
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            adhocaccessrequest.AccountType = AccountType.User;
            adhocaccessrequest.AdhocActionType = AdHocActionType.DeleteAccount;
            adhocaccessrequest.Comments = accessrequest.Comments;
            adhocaccessrequest.CreatedBy = accessrequest.CreatedBy;
            adhocaccessrequest.CreatedDate = accessrequest.CreatedDate;
            adhocaccessrequest.EndpointSystemId = this.Endpoint.Id;
            adhocaccessrequest.EndpointSystemType = EndpointType.VPN;
            adhocaccessrequest.ApplicationType = ApplicationType.VPN;
            adhocaccessrequest.IsSelfRequest = true;
            adhocaccessrequest.RequesteeUserId = user.Id;
            adhocaccessrequest.RequesterUserId = user.Id;
            adhocaccessrequest.ValidFrom = DateTime.Now;
            adhocaccessrequest.ValidTo = DateTime.Now;
            adhocaccessrequest.AccountType = AccountType.User;

            accessrequest.OrgCompany = user?.OrgCompany;
            accessrequest.OrgDepartment = user?.OrgDepartment;
            accessrequest.OrgDivision = user?.OrgDivision;
            accessrequest.OrgBranch = user?.OrgBranch;
            accessrequest.OrgSection = user?.OrgSection;
            AdHocVPNRequest inRequest = new AdHocVPNRequest()
            {
                EndpointUserId = vpnUser != null ? vpnUser.Id:0,
                IsOffboardOrTransferRequest = true,
                OneKey = user.OneKeyTokenId,
            };

            adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(inRequest);

            accessrequest.AdHocAccessRequest = adhocaccessrequest;

            accessrequest.Save(MasterConfigHelper.ServiceUser);
        }
        private void CreateVPNAccountTaskForITSecurity(DomainModel.User user, string OneKeyTokenId, List<VPNApplicationEnvironment> vpnApplnEnviornments)
        {
            Guid transactionId = Guid.NewGuid();
            AccessRequest accessrequest = new AccessRequest();

            accessrequest.ActivationDate = DateTime.Now;
            accessrequest.Comments = "UndoEvent: Task for IT Security Team to Register the VPN Token Number";
            accessrequest.CreatedBy = "System";
            accessrequest.CreatedDate = DateTime.Now;
            accessrequest.EndpointSystemId = this.Endpoint.Id;
            accessrequest.ApplicationType = ApplicationType.VPN;
            accessrequest.RequestSource = AccessRequestSource.AdhocRequest;
            accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
            accessrequest.TransactionId = transactionId;
            accessrequest.UserId = user.Id;
            accessrequest.RequesterId = user.Id;
            accessrequest.AccountType = AccountType.User;
            accessrequest.EventUndoRequestId = this.CurrentRequest.EventUndoRequestId;
            accessrequest.OrgCompany = CurrentRequest.OrgCompany;
            accessrequest.OrgDepartment = CurrentRequest.OrgDepartment;
            accessrequest.OrgDivision = CurrentRequest.OrgDivision;
            accessrequest.OrgBranch = CurrentRequest.OrgBranch;
            accessrequest.OrgSection = CurrentRequest.OrgSection;

            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            adhocaccessrequest.AccountType = AccountType.User;
            adhocaccessrequest.AdhocActionType = AdHocActionType.NewAccount;
            adhocaccessrequest.Comments = accessrequest.Comments;
            adhocaccessrequest.CreatedBy = accessrequest.CreatedBy;
            adhocaccessrequest.CreatedDate = accessrequest.CreatedDate;
            adhocaccessrequest.EndpointSystemId = this.Endpoint.Id;
            adhocaccessrequest.EndpointSystemType = EndpointType.VPN;
            adhocaccessrequest.ApplicationType = ApplicationType.VPN;
            adhocaccessrequest.IsSelfRequest = true;
            adhocaccessrequest.RequesteeUserId = user.Id;
            adhocaccessrequest.RequesterUserId = user.Id;
            adhocaccessrequest.ValidFrom = DateTime.Now;
            adhocaccessrequest.ValidTo = null;
            adhocaccessrequest.AccountType = AccountType.User;
            AdHocVPNRequest inRequest = new AdHocVPNRequest()
            {
                EndpointUserId = 0,
                IsOffboardOrTransferRequest = false,
                OneKey = OneKeyTokenId,
                UpdateOneKeyToken=false
            };
            if(vpnApplnEnviornments.Count > 0)
            {
                int identity = 1;
                foreach (var vpnApplnEnviornment in vpnApplnEnviornments)
                {
                    var vpnMatrix = VPNMatrix.GetById(vpnApplnEnviornment.MatrixId);
                    if (vpnMatrix == null) continue;

                    var requestDetail = new AdHocVPNRequestDetail();
                    requestDetail.MatrixType = vpnApplnEnviornment.MatrixType.ToString();
                    requestDetail.VPNMatrixId = vpnApplnEnviornment.MatrixId;
                    requestDetail.identity = identity;
                    requestDetail.ApplicationName = vpnMatrix.Name;
                    if (vpnApplnEnviornment.EnviornmentType == VPNMatrixEnviornmentType.Production && !string.IsNullOrWhiteSpace(vpnMatrix.ProductionADgroups))
                    {
                        requestDetail.VPNType = VPNType.SSLVPNOnly;
                        requestDetail.Production = true;
                        requestDetail.ProductionADGroups = vpnMatrix.ProductionADgroups;
                    }
                    else if (vpnApplnEnviornment.EnviornmentType == VPNMatrixEnviornmentType.QA && !string.IsNullOrWhiteSpace(vpnMatrix.QAADGroups))
                    {
                        requestDetail.VPNType = VPNType.SSLVPNOnly;
                        requestDetail.QA = true;
                    }
                    else if (vpnApplnEnviornment.EnviornmentType == VPNMatrixEnviornmentType.Development && !string.IsNullOrWhiteSpace(vpnMatrix.DevelpomentADGroups))
                    {
                        requestDetail.VPNType = VPNType.SSLVPNOnly;
                        requestDetail.Dev = true;
                    }else if(vpnApplnEnviornment.EnviornmentType == VPNMatrixEnviornmentType.ADGroup && !string.IsNullOrWhiteSpace(vpnMatrix.ADGroups))
                    {
                        requestDetail.VPNType = VPNType.VDI;
                        requestDetail.isOthers = true;
                    }
                    inRequest.AdHocVPNRequestDetails.Add(requestDetail);
                    identity = identity + 1 ;
                }
            }
            adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(inRequest);

            accessrequest.AdHocAccessRequest = adhocaccessrequest;

            accessrequest.Save(MasterConfigHelper.ServiceUser);
        }

    }
}

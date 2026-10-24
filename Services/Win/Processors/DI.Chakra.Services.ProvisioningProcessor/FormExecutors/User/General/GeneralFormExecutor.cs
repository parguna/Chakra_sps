using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class GeneralFormExecutor : FormExecutorBase
    {
        string ServiceUser { get; set; }

        readonly List<long> _successFactors_NC_EndpointSystemIDs = new List<long>();
        public GeneralFormExecutor(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
            ServiceUser = MasterConfigHelper.ServiceUser;
            _successFactors_NC_EndpointSystemIDs.AddRange(MasterConfigHelper.SuccessFactorEndpointSystemIDs);
        }
        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            AdHocINHRequest inhDetail = null;
            long endpointUserId = 0;

            //if adhoc request, get prov details from adhoc request object
            if (CurrentRequest.AdHocAccessRequestId.HasValue)
            {
                inhDetail = (AdHocINHRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();

                if (inhDetail != null)
                {
                    endpointUserId = inhDetail.EndpointUserId;
                }
            }

            if (CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest)
            {
                if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    isExecutedSuccessfully = CreateAccountOrAmendment(user, inhDetail);
                    endpointUserId = inhDetail.EndpointUserId;
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    isExecutedSuccessfully = DeleteAccount(user, inhDetail.EndpointUserId);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    isExecutedSuccessfully = AmendentAdd(user, inhDetail);
                }
                else if (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    isExecutedSuccessfully = AmendentRemove(user, inhDetail);
                }
                WriteProvisioningLog(LogLevelType.Info, $"ActionType:{CurrentRequest.AdHocAccessRequest.AdhocActionType} isExecutedSuccessfully:{isExecutedSuccessfully}");
            }
            else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
            {
                if (CurrentRequest.AccountType == AccountType.User)
                {
                    inhDetail = new AdHocINHRequest();

                    if (fields["Permissions"] != null && !string.IsNullOrWhiteSpace(fields["Permissions"].Value))
                    {
                        inhDetail.EntitlementType = EntitlementType.Permission;
                        inhDetail.Entitlement = fields["Permissions"].Value;
                    }

                    isExecutedSuccessfully = CreateAccountOrAmendment(user, inhDetail);
                    endpointUserId = inhDetail.EndpointUserId;
                }
            }

            if (isExecutedSuccessfully)
            {
                TriggerEmail(isExecutedSuccessfully, user, endpointUserId);
            }

            return isExecutedSuccessfully;
        }

        public bool IsSuccessFactorEndpointSystem()
        {
            return   _successFactors_NC_EndpointSystemIDs.Contains((long)this.CurrentRequest.EndpointSystemId);
        }


        private void TriggerEmail(bool isExecutedSuccessfully, DomainModel.User user, long endpointUserId)
        {
            try
            {
                bool emailSent = false;
                var epUserInfo = InhouseUserInfo.GetById(endpointUserId);
                var adhocActionType = (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire) ? AdHocActionType.NewAccount :
                                                                     CurrentRequest.AdHocAccessRequest.AdhocActionType;

                switch (adhocActionType)
                {
                    case AdHocActionType.NewAccount:
                        emailSent = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, epUserInfo, user, ApplicationType.InhouseSystems);
                        break;
                    case AdHocActionType.AmendmentAdd:
                        emailSent = ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, CurrentRequest.AdHocAccessRequest.Entitlement, epUserInfo.UserName);
                        break;
                    case AdHocActionType.AmendmentRemove:
                        emailSent = ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, CurrentRequest.AdHocAccessRequest.Entitlement, epUserInfo.UserName);
                        break;
                    case AdHocActionType.DeleteAccount:
                        emailSent = ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, this.Endpoint, CurrentRequest, epUserInfo.UserName);
                        break;
                }

                if (!emailSent)
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Error while sending email for actiontype {CurrentRequest.AdHocAccessRequest.AdhocActionType}");
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while sending an email", ex);
            }
        }

        bool AddINHAccountReconAndEntitlementRecon(AdHocINHRequest iNHRequest)
        {
            ConnectorBase inhConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);

            var inhUserInfo = InhouseUserInfo.GetById(iNHRequest.EndpointUserId);

            var accountCreated = CreateUserInReconTable(inhUserInfo, ServiceUser);
            if (accountCreated)
                return AddEntitlement(inhUserInfo, iNHRequest);
            return false;
        }

        private void MapUserPermissionFunction(AdHocINHRequest iNHRequest, InhousePermission permission, string functions)
        {
            if (!string.IsNullOrWhiteSpace(functions))
            {
                foreach (var function in functions.Split(','))
                {
                    var functionObj = InhouseFunction.GetInhouseFunctionByApplicationCode(function.Trim(), Endpoint.ApplicationCode, permission.Name);
                    if (functionObj == null)
                        continue;

                    var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionIdFunctionId(iNHRequest.EndpointUserId, permission.Id, functionObj.Id, Convert.ToInt64(CurrentRequest.EndpointSystemId));
                    if (existingUPF == null)
                    {
                        existingUPF = new InhUserPermissionFunction();
                        existingUPF.EndpointUserId = iNHRequest.EndpointUserId;
                        existingUPF.EndPointSystemId = Endpoint.Id;
                        existingUPF.InhouseSystemCode = Endpoint.ApplicationCode;
                        existingUPF.Permission = permission.Name;
                        existingUPF.PermissionId = permission.Id;
                        existingUPF.FunctionId = functionObj.Id;
                        existingUPF.Functions = functions;
                        existingUPF.CreatedBy = ServiceUser;
                        existingUPF.CreatedDate = DateTime.Now;
                        existingUPF.Save(ServiceUser);
                    }
                }
            }
            else
            {
                var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionId(iNHRequest.EndpointUserId, permission.Id);
                if (existingUPF == null)
                {
                    existingUPF = new InhUserPermissionFunction();
                    existingUPF.EndpointUserId = iNHRequest.EndpointUserId;
                    existingUPF.EndPointSystemId = Endpoint.Id;
                    existingUPF.InhouseSystemCode = Endpoint.ApplicationCode;
                    existingUPF.Permission = permission.Name;
                    existingUPF.PermissionId = permission.Id;
                    existingUPF.CreatedBy = ServiceUser;
                    existingUPF.CreatedDate = DateTime.Now;
                    existingUPF.Save(ServiceUser);
                }
            }
        }

        private void UnMapUserPermissionFunction(AdHocINHRequest iNHRequest, InhousePermission permission, string functions)
        {
            if (!string.IsNullOrWhiteSpace(functions))
            {
                foreach (var function in functions.Split(','))
                {
                    var functionObj = InhouseFunction.GetInhouseFunctionByApplicationCode(function, Endpoint.ApplicationCode, permission.Name);
                    var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionIdFunctionId(iNHRequest.EndpointUserId, permission.Id, functionObj.Id, Convert.ToInt64(CurrentRequest.EndpointSystemId));
                    if (existingUPF != null)
                        InhUserPermissionFunction.Delete(existingUPF.Id);
                }
            }
            else
            {
                var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionId(iNHRequest.EndpointUserId, permission.Id);
                if (existingUPF != null)
                    InhUserPermissionFunction.Delete(existingUPF.Id);
            }
        }

        DateTime? GetValidTo(Nullable<long> userId)
        {
            var validTo = DateTime.MaxValue;
            if ((CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

            if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if(user.FullTimeEmployee == false)
                    {
                        return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour);
                    }
                    else
                    {
                        return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour).AddMinutes(Constants.MaxMinutes).AddSeconds(Constants.MaxSeconds);
                    }
                }
                else
                {
                    return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour).AddMinutes(Constants.MaxMinutes).AddSeconds(Constants.MaxSeconds);
                }
            }
            else
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if(user != null && user.FullTimeEmployee == false && user.TerminationDate.HasValue)
                    {
                        validTo = user.TerminationDate.Value.Date.AddHours(Constants.MaxHour);
                    }
                }
            }
            return validTo;
        }

        private bool AddEntitlement(IEndpointUserInfo _userInfo, AdHocINHRequest iNHRequest)
        {
            bool executed = false;
            try
            {
                var accRec = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == accRec)
                    return false;

                if (!string.IsNullOrEmpty(iNHRequest.Entitlement))
                {
                    string[] entitlements = null;
                    if (Endpoint.IsFunctionLevel)
                    {
                        entitlements = iNHRequest.Entitlement.Split('|');
                    }
                    else
                    {
                        entitlements = iNHRequest.Entitlement.Split(',');
                    }
                    foreach (var item in entitlements)
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        var permission = ent[0].Trim();
                        var functions = ent.Length > 1 ? ent[1] : string.Empty;

                        var permissionObj = InhousePermission.GetByNameAndEndpointSystemId(permission, _userInfo.EndPointSystemId);
                        if (permissionObj == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"AddEntitlement: Id: {_userInfo?.Id}, Endpointsystem: {_userInfo?.EndPointSystemId}, AccountName: {_userInfo?.AccountName} Permission:{permission} not found.");
                            executed = true;
                            continue;
                        }
                        MapUserPermissionFunction(iNHRequest, permissionObj, functions);

                        var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(accRec.Id, permissionObj.Name);
                        if (entRecon == null)
                        {
                            entRecon = new EntitlementReconcilation();
                        }

                        entRecon.AccountReconcilationId = accRec.Id;
                        entRecon.EndpointSystemId = accRec.EndpointSystemId;
                        entRecon.EndpointUserId = accRec.EndpointUserId;
                        entRecon.AccountName = accRec.AccountName;
                        entRecon.UserId = accRec.UserId;
                        entRecon.EnteredDate = DateTime.Now;
                        entRecon.IsDeleted = false;
                        entRecon.EntitlementType = Convert.ToInt32(EntitlementType.Permission);
                        entRecon.EndpointType = EndpointType.InhouseSystems;
                        entRecon.Entitlement = permission;
                        entRecon.EntitlementId = permissionObj.Id;
                        entRecon.EntitlementDescription = permissionObj.Description;
                        entRecon.ValidFromDate = CurrentRequest.ActivationDate.HasValue ? CurrentRequest.ActivationDate.Value : DateTime.Now;
                        entRecon.ValidToDate = GetValidTo(accRec.UserId);
                        entRecon.EntitlementDescription = permissionObj.Description;

                        if (!string.IsNullOrWhiteSpace(functions))
                        {
                            if (!string.IsNullOrWhiteSpace(entRecon.Functions))
                            {
                                foreach (var function in functions.Split(','))
                                {
                                    if (!entRecon.Functions.Contains(function))
                                        entRecon.Functions = string.Join(",", new string[] { entRecon.Functions, functions });
                                }
                            }
                            else
                                entRecon.Functions = functions;
                        }
                        entRecon.Save(ServiceUser);
                        executed = true;
                    }
                }
                else
                {
                    executed = true;
                }
            }
            catch (Exception ex)
            {
                executed = false;
            }
            return executed;
        }

        private bool RemoveEntitlement(IEndpointUserInfo _userInfo, AdHocINHRequest iNHRequest)
        {
            bool executed = false;
            try
            {
                var accRec = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == accRec)
                    return false;

                if (!string.IsNullOrEmpty(iNHRequest.Entitlement))
                {
                    string[] entitlements = null;
                    if (Endpoint.IsFunctionLevel)
                    {
                        entitlements = iNHRequest.Entitlement.Split('|');
                    }
                    else
                    {
                        entitlements = iNHRequest.Entitlement.Split(',');
                    }
                    foreach (var item in entitlements)
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        var permission = ent[0].Trim();
                        var functions = ent.Length > 1 ? ent[1] : string.Empty;

                        var permissionObj = InhousePermission.GetByNameAndEndpointSystemId(permission, _userInfo.EndPointSystemId);

                        if (permissionObj == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"RemoveEntitlement: Id: {_userInfo?.Id}, Endpointsystem: {_userInfo?.EndPointSystemId}, AccountName: {_userInfo?.AccountName} Permission:{permission} not found.");
                            executed = true;
                            continue;
                        }

                        UnMapUserPermissionFunction(iNHRequest, permissionObj, functions);

                        var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(accRec.Id, permissionObj.Name);
                        if (entRecon != null)
                        {
                            entRecon.LastModifiedDate = DateTime.Now;
                            entRecon.LastModifiedBy = "System";
                            entRecon.IsSODConflict = false; // Clear SOD Conflict Flag if its set.
                            entRecon.IsSensitiveAccess = false; // Clear SA Flag if its set.

                            if (string.IsNullOrWhiteSpace(functions) || string.IsNullOrWhiteSpace(entRecon.Functions))
                            {
                                entRecon.Functions = string.Empty;
                                entRecon.IsDeleted = true;
                                entRecon.DeletedDate = DateTime.Now;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(entRecon.Functions))
                                {
                                    var existingFunctions = entRecon.Functions.Split(',').ToList();
                                    foreach (var function in functions.Split(','))
                                    {
                                        existingFunctions.Remove(function);
                                    }
                                    if (existingFunctions.Count == 0)
                                    {
                                        entRecon.Functions = string.Empty;
                                        entRecon.IsDeleted = true;
                                        entRecon.DeletedDate = DateTime.Now;
                                    }
                                    else
                                    {
                                        entRecon.Functions = string.Join(",", existingFunctions);
                                        entRecon.IsDeleted = false;
                                    }
                                }
                            }
                            entRecon.Save(ServiceUser);
                        }
                    }
                    executed = true;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"RemoveEntitlement failed:{ex.Message} StackTrace:{ex.StackTrace}", ex);
            }
            return executed;
        }

        private bool RemoveAllEntitlements(IEndpointUserInfo _userInfo, long endpointUserId, ConnectorBase inhConnector)
        {
            bool executed = false;
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"RemoveAllEntitlements: Id: {_userInfo?.Id}, Endpointsystem: {_userInfo?.EndPointSystemId}, AccountName: {_userInfo?.AccountName}");
                DeleteINHUserAndMapping(endpointUserId);
                WriteProvisioningLog(LogLevelType.DebugStep, "DeleteINHUserAndMapping");
                inhConnector.RemoveAllEntitlements(_userInfo);
                WriteProvisioningLog(LogLevelType.DebugStep, "inhConnector.RemoveAllEntitlements");
                executed = true;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"RemoveAllEntitlements failed. { ex.Message} {ex.StackTrace}");
            }
            return executed;
        }

        private void DeleteINHUserAndMapping(long endpointUserId)
        {
            var inhPermissionFunctions = InhUserPermissionFunction.GetByEndpointUserId(endpointUserId, Convert.ToInt64(CurrentRequest.EndpointSystemId));
            foreach (var item in inhPermissionFunctions)
            {
                InhUserPermissionFunction.Delete(item.Id);
            }
        }

        bool RemoveINHAccountReconAndEntitlementRecon(AdHocINHRequest iNHRequest)
        {
            var inhUserInfo = InhouseUserInfo.GetById(iNHRequest.EndpointUserId);
            if (inhUserInfo == null)
                return false;

            return RemoveEntitlement(inhUserInfo, iNHRequest);
        }

        long CreateINHUser(DomainModel.User user)
        {
            try
            {
                InhouseUserInfo inhUser = InhouseUserInfo.GetByUserName(Endpoint.Id, user.UserName);
                if (inhUser == null)
                {
                    inhUser = new InhouseUserInfo();
                }
                inhUser.EndPointSystemId = Endpoint.Id;
                inhUser.NRIC = user.EmployeeId;
                inhUser.EmployeeId = user.EmployeeId;
                inhUser.InhouseSystemCode = Endpoint.ApplicationCode;
                inhUser.Email = user.EmailId;
                inhUser.UserName = user.UserName;
                inhUser.DisplayName = user.DisplayName;
                inhUser.FirstName = user.FirstName;
                inhUser.LastName = user.LastName;
                inhUser.AccountType = CurrentRequest.AccountType;
                inhUser.CreatedBy = ServiceUser;
                inhUser.CreatedDate = DateTime.Now;
                inhUser.IsLocked = false;
                inhUser.Password = user.Password;
                inhUser.AccountCreatedDate = DateTime.Now;
                inhUser.IsDisabled = false;
                inhUser.IsDeleted = false;
                inhUser.DeletedDate = null;
                inhUser.DisabledDate = null;
                inhUser.LockedDate = null;
                inhUser.Save(ServiceUser);
                return inhUser.Id;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(Endpoint.Id, LogLevelType.Error, ex.Message);
                return 0;
            }
        }

        bool EntitlementAlreadyExists(SAPEPUserInfo sapuserinfo, EntitlementType entitlementType, long endpointsystemId)
        {
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, "EntitlementAlreadyExists");
                List<string> listToCompare = new List<string>();
                if (entitlementType == EntitlementType.Role)
                    listToCompare = sapuserinfo.Roles;
                else
                    listToCompare = sapuserinfo.Groups;

                if (listToCompare == null || listToCompare.Count == 0)
                    return true;

                var currentEntitlements = EntitlementReconcilation.GetByEndpointUserId(sapuserinfo.Id, endpointsystemId, entitlementType).Where(x => !x.IsDeleted).ToList();
                return currentEntitlements.Any(p => listToCompare.Contains(p.Entitlement));
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"EntitlementAlreadyExists {ex.Message}", ex);
                return true;
            }
        }

        private bool CreateAccountOrAmendment(DomainModel.User user, AdHocINHRequest iNHRequest)
        {
            bool success = false;

            if (user == null || iNHRequest == null)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"GeneralFormExecutor: CreateAccountOrAmendment - User is Null:[{user==null}], InhRequest is null:[{iNHRequest == null}]");
                return success;
            }
            #region NewACcount Validation
            if (Endpoint.CanAutomate && this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (CurrentRequest.AccountType != AccountType.User)
                {
                    user.UserName = iNHRequest.AdHocINHSharedAccount?.AccountName;
                }
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var epEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                var ceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);

                foreach (var permission in iNHRequest.Entitlement.Split(','))
                {
                    var inhPermission = InhousePermission.GetByPermissionName(permission.Trim(), Endpoint.Id);
                    if (inhPermission == null)
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"Other System Permission not found in Matrix: {permission}");
                        continue;
                    }
                    if (adEndpoint != null && adEndpoint.IsActive && inhPermission != null && !string.IsNullOrWhiteSpace(inhPermission.ADGroups))
                    {
                        if (IsEndpointAccountActive(adEndpoint, user) == false)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Endpoint:{adEndpoint.DisplayName} account does not exist.");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                    }
                    if (epEndpoint != null && epEndpoint.IsActive && inhPermission != null && (!string.IsNullOrWhiteSpace(inhPermission.EPGroups) || !string.IsNullOrWhiteSpace(inhPermission.EPRoles)))
                    {
                        if (IsEndpointAccountActive(epEndpoint, user) == false)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Endpoint:{epEndpoint.DisplayName} account does not exist.");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                    }
                    if (ceEndpoint != null && ceEndpoint.IsActive && inhPermission != null && (!string.IsNullOrWhiteSpace(inhPermission.CEGroups) || !string.IsNullOrWhiteSpace(inhPermission.CERoles)))
                    {
                        if (IsEndpointAccountActive(ceEndpoint, user) == false)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Endpoint:{ceEndpoint.DisplayName} account does not exist.");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                    }
                }
            }
            #endregion

            if (iNHRequest.EndpointUserId == 0)
            {
                if (CurrentRequest.AccountType != AccountType.User)
                {
                    user.UserName = iNHRequest.AdHocINHSharedAccount?.AccountName;
                    user.DisplayName = iNHRequest.AdHocINHSharedAccount?.DisplayName;
                    user.Password = iNHRequest.AdHocINHSharedAccount?.Password;
                }

                var tempId = CreateINHUser(user);
                if (tempId == 0)
                    return false;

                iNHRequest.EndpointUserId = tempId;
            }
            else
            {
                //unlock inhouse user
                var inhUser = InhouseUserInfo.GetById(iNHRequest.EndpointUserId);
                inhUser.IsDisabled = false;
                inhUser.IsDeleted = false;
                inhUser.DeletedDate = null;
                inhUser.DisabledDate = null;
                inhUser.LockedDate = null;
                inhUser.IsLocked = false;
                inhUser.Save(ServiceUser);
            }
            if (Endpoint.CanAutomate)
            {
                var inhUserInfo = DomainModel.InhouseUserInfo.GetById(iNHRequest.EndpointUserId);
                user.UserName = inhUserInfo.UserName;

                WriteProvisioningLog(LogLevelType.DebugStep, $"Executing the CreateAccountOrAmendment: {user.AccountName}");

                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var epEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                var ceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);
                ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, CurrentRequest);
                ConnectorBase epConnector = ConnectorFactory.GetConnector(epEndpoint, CurrentRequest);
                ConnectorBase ceConnector = ConnectorFactory.GetConnector(ceEndpoint, CurrentRequest);

                foreach (var permission in iNHRequest.Entitlement.Split(','))
                {
                    var inhPermission = InhousePermission.GetByPermissionName(permission.Trim(), Endpoint.ApplicationCode);
                    if (inhPermission == null)
                    {
                        //if no entitlement found. Need to create acccount recon only
                        success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(inhPermission.ADGroups))
                    {
                        if(IsEndpointAccountActive(adEndpoint, user) == false)
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        var adsUserInfo = ADSUserInfo.GetByEndPointSystemId(user.AccountName, adEndpoint.Id);
                        if (adsUserInfo == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"Could not find AD Account/Inactive for the User:{user.AccountName}");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        adsUserInfo.Groups = inhPermission.ADGroups.Split(',')?.ToList();
                        var executedSuccessfully = adConnector.AddEntitlement(adsUserInfo, EntitlementType.Group);
                        if (executedSuccessfully)
                            success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        else
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            WriteProvisioningLog(LogLevelType.Warning, $"Adding entitlement failed for AD Groups:{string.Join(",", adsUserInfo.Groups)}");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(inhPermission.EPGroups))
                    {
                        if (IsEndpointAccountActive(epEndpoint, user) == false)
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, epEndpoint.Id);
                        if (sAPEPUserInfo == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"User:{user.AccountName} SAPEP Account not found");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        sAPEPUserInfo.Groups = inhPermission.EPGroups.Split(',')?.ToList();

                        success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Group, epEndpoint.Id);
                        if (!success)
                        {
                            success = epConnector.AddEntitlement(sAPEPUserInfo, EntitlementType.Group);
                        }

                        if (success)
                            success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        else
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            WriteProvisioningLog(LogLevelType.Warning, $"Adding entitlement failed for EP Groups:{string.Join(",", sAPEPUserInfo.Roles)}");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(inhPermission.EPRoles) && epEndpoint.IsActive)
                    {
                        if (IsEndpointAccountActive(epEndpoint, user) == false)
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, epEndpoint.Id);
                        if (sAPEPUserInfo == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"User:{user.AccountName} SAPEP Account not found");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        sAPEPUserInfo.Roles = inhPermission.EPRoles.Split(',')?.ToList();

                        success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Role, epEndpoint.Id);
                        if (!success)
                        {
                            success = epConnector.AddEntitlement(sAPEPUserInfo, EntitlementType.Role);
                        }

                        if (success)
                            success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        else
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            WriteProvisioningLog(LogLevelType.Warning, $"Adding entitlement failed for EP Roles:{string.Join(",", sAPEPUserInfo.Roles)}");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(inhPermission.CEGroups) && ceEndpoint.IsActive)
                    {

                        if (IsEndpointAccountActive(ceEndpoint, user) == false)
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        var sAPCEUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, ceEndpoint.Id);
                        if (sAPCEUserInfo == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"User:{user.AccountName} SAPCE Account not found");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        sAPCEUserInfo.Groups = inhPermission.CEGroups.Split(',')?.ToList();

                        success = EntitlementAlreadyExists(sAPCEUserInfo, EntitlementType.Group, ceEndpoint.Id);
                        if (!success)
                        {
                            success = ceConnector.AddEntitlement(sAPCEUserInfo, EntitlementType.Group);
                        }

                        if (success)
                            success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        else
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            WriteProvisioningLog(LogLevelType.Warning, $"Adding entitlement failed for CE Groups:{string.Join(",", sAPCEUserInfo.Groups)}");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(inhPermission.CERoles))
                    {
                        if (IsEndpointAccountActive(ceEndpoint, user) == false)
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }

                        var sAPCEUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, ceEndpoint.Id);
                        if (sAPCEUserInfo == null)
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"User:{user.AccountName} SAPCE Account not found");
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            return false;
                        }
                        sAPCEUserInfo.Roles = inhPermission.CERoles.Split(',')?.ToList();

                        success = EntitlementAlreadyExists(sAPCEUserInfo, EntitlementType.Role, ceEndpoint.Id);
                        if (!success)
                        {
                            success = ceConnector.AddEntitlement(sAPCEUserInfo, EntitlementType.Role);
                        }

                        if (success)
                            success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
                        else
                        {
                            CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                            WriteProvisioningLog(LogLevelType.Warning, $"Adding entitlement failed for CE Roles:{string.Join(",", sAPCEUserInfo.Roles)}");
                        }
                    }
                }
            }
            else
            {
                success = AddINHAccountReconAndEntitlementRecon(iNHRequest);
            }

            return success;
        }



        private bool DeleteAccount(DomainModel.User user, long endpointUserId)
        {
            WriteProvisioningLog(LogLevelType.Info, $"Delete Account Started - {Endpoint.DisplayName}");

            ConnectorBase inhConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);
            var inhUserInfo = InhouseUserInfo.GetById(endpointUserId);
            bool executed = true;

            if (Endpoint.CanAutomate && CurrentRequest.RequestSource == AccessRequestSource.AdhocRequest && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, "Adhoc delete account started");

                AdHocINHRequest adHocINHRequest = new AdHocINHRequest();
                adHocINHRequest.EndpointUserId = inhUserInfo.Id;
                var entitlementReconcilations = EntitlementReconcilation.GetByEndpointUserId(inhUserInfo.Id, Endpoint.Id);
                if (entitlementReconcilations != null && entitlementReconcilations.Count > 0)
                {
                    adHocINHRequest.Entitlement = string.Join(",", entitlementReconcilations.Select(p => p.Entitlement));
                    WriteProvisioningLog(LogLevelType.Info, $"User entitlments: {adHocINHRequest.Entitlement} for user: {user.UserName}");
                }
                executed = AmendentRemove(user, adHocINHRequest);

                WriteProvisioningLog(LogLevelType.Info, $"Removed entitlments: {adHocINHRequest.Entitlement} for user: {user.UserName} status:{executed}");
            }

            executed = inhConnector.RemoveAllEntitlementsByEndpointUserId(endpointUserId, this.Endpoint.Id);

            var query = $"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId={endpointUserId} AND EndPointSystemId={this.Endpoint.Id}";
            InhouseUserInfo.ExecuteQuery(query);

            WriteProvisioningLog(LogLevelType.Info, "Completed removing the entitlements");

            if (executed)
            {
                DeleteAccountRecon(endpointUserId);
                if (inhUserInfo != null)
                {
                    inhUserInfo.IsLocked = inhUserInfo.IsDeleted = true;
                    inhUserInfo.DeletedDate = inhUserInfo.LockedDate = DateTime.Now;
                    inhUserInfo.Save(MasterConfigHelper.ServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, $"Delete Account completed - {Endpoint.DisplayName}");
                }
                return true;
            }

            return false;
        }

        bool DeleteAccountRecon(long endpointUserId)
        {
            var acctRecon = AccountReconcilation.GetByUser(Endpoint.Id, endpointUserId);
            if (null == acctRecon)
                return false;
            else
            {
                acctRecon.EndpointUserId = 0;
                acctRecon.IsDeleted = true;
                acctRecon.IsLocked = true;
                acctRecon.DeletedDate = DateTime.Now;
                acctRecon.Save(ServiceUser);
                return true;
            }
        }

        private bool AmendentAdd(DomainModel.User user, AdHocINHRequest iNHRequest)
        {
            return CreateAccountOrAmendment(user, iNHRequest);
        }

        private bool AmendentRemove(DomainModel.User user, AdHocINHRequest iNHRequest)
        {
            bool success = false;
            ConnectorBase inhConnector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);
            try
            {
                if (Endpoint.CanAutomate)
                {
                    var inhUserInfo = DomainModel.InhouseUserInfo.GetById(iNHRequest.EndpointUserId);
                    user.UserName = inhUserInfo.UserName;

                    WriteProvisioningLog(LogLevelType.Info, $"Remove Permission for AccountName : {user.AccountName} Permission:{iNHRequest.Entitlement}");

                    var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var epEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                    var ceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);
                    ConnectorBase adConnector = ConnectorFactory.GetConnector(adEndpoint, CurrentRequest);
                    ConnectorBase epConnector = ConnectorFactory.GetConnector(epEndpoint, CurrentRequest);
                    ConnectorBase ceConnector = ConnectorFactory.GetConnector(ceEndpoint, CurrentRequest);
                    if (!string.IsNullOrWhiteSpace(iNHRequest.Entitlement))
                    {
                        foreach (var permission in iNHRequest.Entitlement.SplitBySeparator(",", StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!string.IsNullOrWhiteSpace(permission))
                            {
                                var inhPermission = InhousePermission.GetByPermissionName(permission.Trim(), Endpoint.Id);
                                if (inhPermission == null)
                                {
                                    WriteProvisioningLog(LogLevelType.Warning, $"Other System Permission not found in Matrix: {permission}");
                                    continue;
                                }
                                if (adEndpoint != null && adEndpoint.IsActive && !string.IsNullOrWhiteSpace(inhPermission.ADGroups))
                                {

                                    var adsUserInfo = ADSUserInfo.GetByEndPointSystemId(user.AccountName, adEndpoint.Id);
                                    if (adsUserInfo == null)
                                    {
                                        WriteProvisioningLog(LogLevelType.DebugStep, $"Could not find AD Account for the User:{user.AccountName}");
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        return false;
                                    }
                                    adsUserInfo.Groups = inhPermission.ADGroups.Split(',')?.ToList();
                                    var executedSuccessfully = adConnector.RemoveEntitlement(adsUserInfo, EntitlementType.Group);
                                    if (executedSuccessfully)
                                    {
                                        success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                                        WriteProvisioningLog(LogLevelType.Info, $"Removing AD Entitlement:{inhPermission.ADGroups} completed status:{success}");
                                    }
                                    else
                                    {
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        WriteProvisioningLog(LogLevelType.Warning, $"Removing entitlement failed for AD Groups:{string.Join(",", adsUserInfo.Groups)}");
                                    }
                                }

                                if (epEndpoint != null && epEndpoint.IsActive && !string.IsNullOrWhiteSpace(inhPermission.EPGroups))
                                {
                                    var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, epEndpoint.Id);
                                    if (sAPEPUserInfo == null)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"SAPEP Groups: Account not found for User:{user.AccountName}");
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        return false;
                                    }
                                    sAPEPUserInfo.Groups = inhPermission.EPGroups.Split(',')?.ToList();
                                    success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Group, epEndpoint.Id);
                                    if (success)
                                        success = epConnector.RemoveEntitlement(sAPEPUserInfo, EntitlementType.Group);

                                    if (success)
                                        success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                                    else
                                    {
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        WriteProvisioningLog(LogLevelType.Warning, $"Removing entitlement failed for EP Groups:{string.Join(",", sAPEPUserInfo.Groups)}");
                                    }
                                }

                                if (epEndpoint != null && epEndpoint.IsActive && !string.IsNullOrWhiteSpace(inhPermission.EPRoles))
                                {
                                    var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, epEndpoint.Id);
                                    if (sAPEPUserInfo == null)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"SAPEP Roles: Account not found for User:{user.AccountName}");
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        return false;
                                    }
                                    sAPEPUserInfo.Roles = inhPermission.EPRoles.Split(',')?.ToList();
                                    success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Role, epEndpoint.Id);
                                    if (success)
                                        success = epConnector.RemoveEntitlement(sAPEPUserInfo, EntitlementType.Role);

                                    if (success)
                                        success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                                    else
                                    {
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        WriteProvisioningLog(LogLevelType.Warning, $"Removing entitlement failed for EP Roles:{string.Join(",", sAPEPUserInfo.Groups)}");
                                    }
                                }

                                if (ceEndpoint != null && ceEndpoint.IsActive && !string.IsNullOrWhiteSpace(inhPermission.CEGroups))
                                {
                                    var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, ceEndpoint.Id);
                                    if (sAPEPUserInfo == null)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"SAPCE Groups Account not found for User:{user.AccountName}");
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        return false;
                                    }
                                    sAPEPUserInfo.Groups = inhPermission.CEGroups.Split(',')?.ToList();
                                    success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Group, ceEndpoint.Id);
                                    if (success)
                                        success = ceConnector.RemoveEntitlement(sAPEPUserInfo, EntitlementType.Group);

                                    if (success)
                                        success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                                    else
                                    {
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        WriteProvisioningLog(LogLevelType.Warning, $"Removing entitlement failed for CE Groups:{string.Join(",", sAPEPUserInfo.Groups)}");
                                    }
                                }

                                if (ceEndpoint != null && ceEndpoint.IsActive && !string.IsNullOrWhiteSpace(inhPermission.CERoles))
                                {
                                    var sAPEPUserInfo = SAPEPUserInfo.GetByUserName(user.AccountName, ceEndpoint.Id);
                                    if (sAPEPUserInfo == null)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"SAPCE Roles Account not found for User:{user.AccountName}");
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        return false;
                                    }
                                    sAPEPUserInfo.Roles = inhPermission.CERoles.Split(',')?.ToList();
                                    success = EntitlementAlreadyExists(sAPEPUserInfo, EntitlementType.Role, ceEndpoint.Id);
                                    if (success)
                                        success = ceConnector.RemoveEntitlement(sAPEPUserInfo, EntitlementType.Role);

                                    if (success)
                                        success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                                    else
                                    {
                                        CurrentRequest.ProgressStatus = AccessRequestProgress.ExecutionFailed;
                                        WriteProvisioningLog(LogLevelType.Warning, $"Removing entitlement failed for CE Roles:{string.Join(",", sAPEPUserInfo.Groups)}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    success = RemoveINHAccountReconAndEntitlementRecon(iNHRequest);
                }
                WriteProvisioningLog(LogLevelType.DebugStep, $"AmendentRemove status:{success}");
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Error, $"AmendentRemove: Request:{this.CurrentRequest?.Id} Error {ex}",ex);
            }
            return success;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                var inhAdhocDetails = (AdHocINHRequest)this.CurrentRequest.AdHocAccessRequest?.GetEntitlementDetail();

                if (this.CurrentRequest.EmployeeEventTriggerId == null && inhAdhocDetails != null && inhAdhocDetails.IsOffBoardRequest && !Endpoint.CanAutomate)
                {
                    List<InhouseUserInfo> inhUsers = new List<InhouseUserInfo>();
                    //Handle old request before the fix(Else Part)
                    if (inhAdhocDetails.EndpointUserId == 0)
                    {
                        inhUsers = InhouseUserInfo.GetByUserIdAndAccountType(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                    }
                    else
                    {
                        var inhUserInfo = InhouseUserInfo.GetById(inhAdhocDetails.EndpointUserId);
                        if (inhUserInfo != null)
                        {
                            inhUsers.Add(inhUserInfo);
                        }
                    }
                    
                    foreach (var inhUser in inhUsers)
                    {
                        var ouSnapShot = OUChangeSnapshot.GetByUserId(user.Id).FirstOrDefault();
                        TakeOUSnapshot(user.Id, inhUser, EmployeeEvents.Resignation, EndpointType.InhouseSystems, EntitlementType.Permission, ouSnapShot?.BatchId);
                        DeleteAccount(user, inhUser.Id);
                    }
                }
                else
                {
                    if (!Endpoint.CanAutomate)
                    {
                        this.CreateTaskForProcessingTeam(user,fields);
                    }
                    else
                    {
                        List<InhouseUserInfo> inhUsers = InhouseUserInfo.GetByUserIdAndAccountType(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                        List<AdHocINHRequest> inhRequests = new List<AdHocINHRequest>();
                        AdHocINHRequest adHocINHRequest;
                        List<EntitlementReconcilation> entitlementReconcilations;
                        foreach (var inhUser in inhUsers)
                        {
                            var ouSnapShot = OUChangeSnapshot.GetByUserId(user.Id).FirstOrDefault();
                            TakeOUSnapshot(user.Id, inhUser, EmployeeEvents.Resignation, EndpointType.InhouseSystems, EntitlementType.Permission, ouSnapShot?.BatchId);
                            adHocINHRequest = new AdHocINHRequest { EndpointUserId = inhUser.Id };
                            entitlementReconcilations = EntitlementReconcilation.GetByEndpointUserId(inhUser.Id, Endpoint.Id);

                            if (entitlementReconcilations != null && entitlementReconcilations.Count > 0)
                            {
                                adHocINHRequest.Entitlement = string.Join(",", entitlementReconcilations.Select(p => p.Entitlement));
                            }
                            inhRequests.Add(adHocINHRequest);
                        }

                        bool amendRemoved = true;
                        foreach (var inhRequest in inhRequests)
                        {
                            amendRemoved = AmendentRemove(user, inhRequest);
                            DeleteAccount(user, inhRequest.EndpointUserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"OffBoard:{ex}", ex);
                return false;
            }
            return true;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                var inhAdhocDetails = (AdHocINHRequest)this.CurrentRequest.AdHocAccessRequest?.GetEntitlementDetail();

                if (inhAdhocDetails == null)
                    return false;

                if (!inhAdhocDetails.IsOffBoardRequest && !Endpoint.CanAutomate)
                {
                    CreateTaskForProcessingTeam(user, fields);
                }
                else if ((!inhAdhocDetails.IsOffBoardRequest && Endpoint.CanAutomate) || (inhAdhocDetails.IsOffBoardRequest && !Endpoint.CanAutomate))
                {
                    CreateAccountOrAmendment(user, inhAdhocDetails);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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
            throw new NotImplementedException();
        }

        public override bool Transfer(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                var inhAdhocDetails = (AdHocINHRequest)this.CurrentRequest.AdHocAccessRequest?.GetEntitlementDetail();

                if (this.CurrentRequest.EmployeeEventTriggerId == null && inhAdhocDetails != null && inhAdhocDetails.IsOffBoardRequest && !Endpoint.CanAutomate)
                {
                    List<InhouseUserInfo> inhUsers = new List<InhouseUserInfo>();
                    //Handle old request before the fix(Else Part)
                    if (inhAdhocDetails.EndpointUserId == 0)
                    {
                        inhUsers = InhouseUserInfo.GetByUserIdAndAccountType(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                        
                    }
                    else
                    {
                        var inhUserInfo = InhouseUserInfo.GetById(inhAdhocDetails.EndpointUserId);
                        if(inhUserInfo != null)
                        {
                            inhUsers.Add(inhUserInfo);
                        }
                    }
                    foreach (var inhUser in inhUsers)
                    {
                        TakeOUSnapshot(user.Id, inhUser, EmployeeEvents.OUChange, EndpointType.InhouseSystems, EntitlementType.Permission);

                        if (this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            RemoveEntitlement(inhUser, inhAdhocDetails);
                        }
                        else if (this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            DeleteAccount(user, inhUser.Id);
                        }
                    }
                }
                else
                {
                    if (!Endpoint.CanAutomate)
                    {
                        this.CreateTaskForProcessingTeam(user,fields);
                    }
                    else
                    {
                        List<InhouseUserInfo> inhUsers = InhouseUserInfo.GetByUserIdAndAccountType(user.Id, Endpoint.Id, CurrentRequest.AccountType);
                        List<AdHocINHRequest> inhRequests = new List<AdHocINHRequest>();
                        AdHocINHRequest adHocINHRequest;
                        List<EntitlementReconcilation> entitlementReconcilations;
                        foreach (var inhUser in inhUsers)
                        {
                            TakeOUSnapshot(user.Id, inhUser, EmployeeEvents.OUChange, EndpointType.InhouseSystems, EntitlementType.Permission);
                            adHocINHRequest = new AdHocINHRequest { EndpointUserId = inhUser.Id };
                            entitlementReconcilations = EntitlementReconcilation.GetByEndpointUserId(inhUser.Id, Endpoint.Id);

                            if (entitlementReconcilations != null && entitlementReconcilations.Count > 0)
                            {
                                if (IsSuccessFactorEndpointSystem())
                                {
                                    adHocINHRequest.Entitlement = getEntitlementToRemove(user, fields, this.CurrentRequest.RequestSource);
                                }
                                else
                                {
                                    adHocINHRequest.Entitlement = string.Join(",", entitlementReconcilations.Select(p => p.Entitlement));
                                }
                            }
                            inhRequests.Add(adHocINHRequest);
                        }

                        bool amendRemoved = true;
                        foreach (var inhRequest in inhRequests)
                        {
                            amendRemoved = AmendentRemove(user, inhRequest);
                            if(!IsSuccessFactorEndpointSystem())
                                DeleteAccount(user, inhRequest.EndpointUserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(Endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
            return true;
        }

        public override bool Extension(DomainModel.User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            try
            {
                if (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared)
                    return true;

                var accountRecons = AccountReconcilation.GetActiveAccountRecons(user.Id, Endpoint.Id, CurrentRequest.AccountType);

                if (Endpoint.CanAutomate)
                {
                    var oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;

                    if (!oldTerminationDate.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.DebugStep, $"Old termination date not available for the user");
                        return false;
                    }

                    WriteProvisioningLog(LogLevelType.Info, $"Accounts found for extension count :{accountRecons?.Count}");

                    foreach (AccountReconcilation accountRecon in accountRecons)
                    {

                        if (accountRecon.ValidToDate == null || (accountRecon.ValidToDate.HasValue &&
                            (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value)))
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Extend Account:{accountRecon.AccountName}");
                            base.Extension(user, fields, accountType);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.DebugStep, $"Skipping the account :{accountRecon.AccountName}");
                        }
                    }
                    return true;
                }

                var inhAdhocDetails = (AdHocINHRequest)this.CurrentRequest.AdHocAccessRequest?.GetEntitlementDetail();

                if (inhAdhocDetails != null && inhAdhocDetails.IsOffBoardRequest && !Endpoint.CanAutomate)
                {
                    var oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;

                    if (!oldTerminationDate.HasValue)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"Old termination date not available for the user");
                        return false;
                    }

                    WriteProvisioningLog(LogLevelType.Info, $"Accounts found for extension count :{accountRecons?.Count}");

                    foreach (AccountReconcilation accountRecon in accountRecons)
                    {
                        if (accountRecon.ValidToDate == null ||  (accountRecon.ValidToDate.HasValue &&
                            (accountRecon.ValidToDate.Value.ToShortDateString() == oldTerminationDate.Value.ToShortDateString() || accountRecon.ValidToDate.Value > user.TerminationDate.Value)))
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Extend Account:{accountRecon.AccountName}");
                            base.Extension(user, fields, accountType, accountRecon.Id);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Skipping the account :{accountRecon.AccountName}. Criteria does not match.");
                        }
                    }
                }
                else
                {
                    DateTime? oldTerminationDate = UtilityMethods.Deserialize<DomainModel.User>(EmployeeEventTrigger.GetById(CurrentRequest.EmployeeEventTriggerId.Value).OldUserDetails)?.TerminationDate;
                    CreateExtensionTaskForSecurityTeam(user, oldTerminationDate);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(Endpoint.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
            return true;
        }

        private void CreateTaskForProcessingTeam(DomainModel.User user, ProvisioningParamCollection fields)
        {
            string nonDefaultEntitlement = string.Empty;

            if ( (this.CurrentRequest.RequestSource == AccessRequestSource.OUChange || this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding   ) && IsSuccessFactorEndpointSystem())
            {
                nonDefaultEntitlement = getEntitlementToRemove(user, fields, this.CurrentRequest.RequestSource);
                if (string.IsNullOrWhiteSpace(nonDefaultEntitlement) && this.CurrentRequest.RequestSource == AccessRequestSource.OUChange)
                {
                    return;
                }
            }

            Guid transactionId = Guid.NewGuid();
            AccessRequest accessrequest = new AccessRequest();
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            long endpointSystemId = (long)this.CurrentRequest.EndpointSystemId;
            string message = string.Empty;

            if (IsSuccessFactorEndpointSystem())
            {
                if (this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding)
                {
                    message = "Delete Account";
                }
                else if (this.CurrentRequest.RequestSource == AccessRequestSource.OUChange)
                {
                    message = "Remove Non-Default Entitlement";
                }
            }
            else
            {
                message = (this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding || this.CurrentRequest.RequestSource == AccessRequestSource.OUChange) ?
                     "Delete Account" : "Create Account";
            }
            accessrequest.Comments = $"Task for Processing Team to {message} in non connected System - {this.CurrentRequest.RequestSource.ToString()} ";
            accessrequest.CreatedBy = "System";
            accessrequest.CreatedDate = DateTime.Now;
            accessrequest.EndpointSystemId = this.CurrentRequest.EndpointSystemId;
            accessrequest.ApplicationType = ApplicationType.InhouseSystems;
            accessrequest.Description = $"Task for Processing Team to {message} in non connected System - {this.CurrentRequest.RequestSource.ToString()} ";
            if (this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding)
            {
                accessrequest.RequestSource = AccessRequestSource.OffBoarding;
                adhocaccessrequest.AdhocActionType = AdHocActionType.DeleteAccount;
                accessrequest.ActivationDate = DateTime.Now;
                accessrequest.DeactivationDate = DateTime.Now;
                adhocaccessrequest.ValidFrom = DateTime.Now;
                adhocaccessrequest.ValidTo = DateTime.Now;
            }
            else if (this.CurrentRequest.RequestSource == AccessRequestSource.OUChange)
            {
                if (IsSuccessFactorEndpointSystem())
                {
                    adhocaccessrequest.AdhocActionType = AdHocActionType.AmendmentRemove;
                }
                else
                {
                    adhocaccessrequest.AdhocActionType = AdHocActionType.DeleteAccount;
                }
                accessrequest.RequestSource = AccessRequestSource.OUChange;
                accessrequest.ActivationDate = DateTime.Now;
                accessrequest.DeactivationDate = DateTime.Now;
                adhocaccessrequest.ValidFrom = DateTime.Now;
                adhocaccessrequest.ValidTo = DateTime.Now;

            }
            else if (this.CurrentRequest.RequestSource == AccessRequestSource.UndoEvent)
            {
                accessrequest.RequestSource = AccessRequestSource.UndoEvent;
                adhocaccessrequest.AdhocActionType = AdHocActionType.NewAccount;

                accessrequest.ActivationDate = DateTime.Now;
                accessrequest.DeactivationDate = user.TerminationDate;
                adhocaccessrequest.ValidFrom = DateTime.Now;
                adhocaccessrequest.ValidTo = user.TerminationDate;
            }

            accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
            accessrequest.TransactionId = transactionId;
            accessrequest.UserId = this.CurrentRequest.UserId;
            accessrequest.RequesterId = this.CurrentRequest.UserId;
            accessrequest.ApplicationType = ApplicationType.InhouseSystems;
            accessrequest.AccountType = CurrentRequest.AccountType;
            accessrequest.OrgCompany = CurrentRequest.OrgCompany;
            accessrequest.OrgDepartment = CurrentRequest.OrgDepartment;
            accessrequest.OrgDivision = CurrentRequest.OrgDivision;
            accessrequest.OrgBranch = CurrentRequest.OrgBranch;
            accessrequest.OrgSection = CurrentRequest.OrgSection;

            adhocaccessrequest.AccountType = CurrentRequest.AccountType;
            adhocaccessrequest.Comments = accessrequest.Comments;
            adhocaccessrequest.CreatedBy = accessrequest.CreatedBy;
            adhocaccessrequest.CreatedDate = accessrequest.CreatedDate;
            adhocaccessrequest.EndpointSystemId = this.CurrentRequest.EndpointSystemId;
            adhocaccessrequest.EndpointSystemType = EndpointType.InhouseSystems;
            adhocaccessrequest.ApplicationType = ApplicationType.InhouseSystems;
            adhocaccessrequest.IsSelfRequest = true;
            adhocaccessrequest.RequesteeUserId = this.CurrentRequest.UserId;
            adhocaccessrequest.RequesterUserId = this.CurrentRequest.UserId;

            AdHocINHRequest inRequest = null;

            if (this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.EntitlementDetail != null)
            {
                inRequest = UtilityMethods.Deserialize<AdHocINHRequest>(this.CurrentRequest.AdHocAccessRequest.EntitlementDetail);
            }
            if (inRequest == null)
            {
                inRequest = new AdHocINHRequest();

                if (IsSuccessFactorEndpointSystem())
                {


                    inRequest.Entitlement = nonDefaultEntitlement;
                    inRequest.EntitlementType = EntitlementType.Permission;


                    // For SF_NC, if there is no non-default entitlement, then IDMS should implicitly delete the account.
                    // Else need to send delete account to processing team
                    if (accessrequest.RequestSource == AccessRequestSource.OffBoarding
                        && string.IsNullOrWhiteSpace(nonDefaultEntitlement))
                    {
                        accessrequest.RequestStatus = AccessRequestStatus.Granted;
                        accessrequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        accessrequest.ExecutionTime = DateTime.Now;
                    }
                }
                if (!string.IsNullOrWhiteSpace(this.CurrentRequest.AdditionalInfo))
                {
                    try
                    {
                        adhocaccessrequest.EntitlementDetail = this.CurrentRequest.AdditionalInfo;
                        var entitlementDetaisl = adhocaccessrequest.GetEntitlementDetail();
                        if (entitlementDetaisl != null)
                        {
                            var data = entitlementDetaisl as AdHocINHRequest;
                            if (data != null)
                            {
                                
                                inRequest.DisplayAccountName = data.DisplayAccountName;
                                if (this.CurrentRequest.RequestSource == AccessRequestSource.OffBoarding || this.CurrentRequest.RequestSource == AccessRequestSource.OUChange)
                                {
                                    inRequest.EndpointUserId = data.EndpointUserId;
                                }
                                inRequest.ApplicationCode = data.ApplicationCode;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (adhocaccessrequest.AccountType != AccountType.User)
            {
                inRequest.AdHocINHSharedAccount = new AdHocINHSharedAccount();
                inRequest.AdHocINHSharedAccount.AccountName = inRequest.DisplayAccountName;
                inRequest.AdHocINHSharedAccount.DisplayName = inRequest.DisplayAccountName;
                inRequest.AdHocINHSharedAccount.Description = inRequest.DisplayAccountName;
            }
            inRequest.IsOffBoardRequest = true;

            accessrequest.AdHocAccessRequest = adhocaccessrequest;

            adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(inRequest);

            accessrequest.Save(ServiceUser);
        }

        private string getEntitlementToRemove(DomainModel.User user, ProvisioningParamCollection fields,AccessRequestSource  requestsource)
        {
            var inhUserInfo = InhouseUserInfo.GetByUserName(Endpoint.Id, user.UserName);
            var entitlementReconcilations = EntitlementReconcilation.GetByEndpointUserId(inhUserInfo.Id, Endpoint.Id).Where(e=> e.IsDeleted==false).ToList();

            var defaultPermissions = new List<string>();

            if (fields["Permissions"] != null && !string.IsNullOrWhiteSpace(fields["Permissions"].Value))
            {
                if (Endpoint.IsFunctionLevel)
                {
                    defaultPermissions.AddRange(fields["Permissions"].Value.Split('|').Select(p => p.Trim()));
                }
                else
                {
                    defaultPermissions.AddRange(fields["Permissions"].Value.Split(',').Select(p => p.Trim()));
                }
            }
            if (entitlementReconcilations != null && entitlementReconcilations.Count > 0)
            {
                if (requestsource == AccessRequestSource.OUChange)
                {
                    return string.Join(",", entitlementReconcilations.Where(p =>
                    {
                        return !defaultPermissions.Contains(p.Entitlement);
                    }).Select(p => p.Entitlement));
                }
                else
                {
                    return string.Join(",", entitlementReconcilations.Select(e=> e.Entitlement).ToList());
                }

            }
            else
                return string.Empty;

        }

        private void CreateExtensionTaskForSecurityTeam(DomainModel.User user, DateTime? oldTerminationDate)
        {
            LogDebugStep("CreateExtensionTaskForSecurityTeam");
            Guid transactionId = Guid.NewGuid();
            AccessRequest accessrequest = new AccessRequest();

            string strOldTermDate = oldTerminationDate.HasValue ? oldTerminationDate.Value.ToString("dd-MM-yyyy") : "";
            string strNewTermDate = user.TerminationDate.HasValue ? user.TerminationDate.Value.ToString("dd-MM-yyyy") : "";

            accessrequest.ActivationDate = DateTime.Now;
            accessrequest.Comments = $"Task for Processing Team to extend/shorten the validity of user account from {strOldTermDate} to {strNewTermDate} in non connected System";
            accessrequest.CreatedBy = "System";
            accessrequest.CreatedDate = DateTime.Now;
            accessrequest.DeactivationDate = DateTime.Now;
            accessrequest.EndpointSystemId = CurrentRequest.EndpointSystemId;
            accessrequest.RequestSource = AccessRequestSource.NonEmployeeExtension;

            accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
            accessrequest.TransactionId = transactionId;
            accessrequest.UserId = CurrentRequest.UserId;
            accessrequest.RequesterId = CurrentRequest.UserId;
            accessrequest.ApplicationType = ApplicationType.InhouseSystems;
            accessrequest.AccountType = CurrentRequest.AccountType;
            accessrequest.EmployeeEventTriggerId = CurrentRequest.EmployeeEventTriggerId;
            accessrequest.OrgCompany = CurrentRequest.OrgCompany;
            accessrequest.OrgDepartment = CurrentRequest.OrgDepartment;
            accessrequest.OrgDivision = CurrentRequest.OrgDivision;
            accessrequest.OrgBranch = CurrentRequest.OrgBranch;
            accessrequest.OrgSection = CurrentRequest.OrgSection;
            accessrequest.ActivationDate = oldTerminationDate;
            accessrequest.DeactivationDate = user.TerminationDate;

            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            adhocaccessrequest.AccountType = CurrentRequest.AccountType;
            adhocaccessrequest.AdhocActionType = AdHocActionType.AmendmentAdd;
            adhocaccessrequest.Comments = accessrequest.Comments;
            adhocaccessrequest.CreatedBy = accessrequest.CreatedBy;
            adhocaccessrequest.CreatedDate = accessrequest.CreatedDate;
            adhocaccessrequest.EndpointSystemId = CurrentRequest.EndpointSystemId;
            adhocaccessrequest.EndpointSystemType = EndpointType.InhouseSystems;
            adhocaccessrequest.ApplicationType = ApplicationType.InhouseSystems;
            adhocaccessrequest.IsSelfRequest = true;
            adhocaccessrequest.RequesteeUserId = CurrentRequest.UserId;
            adhocaccessrequest.RequesterUserId = CurrentRequest.UserId;
            adhocaccessrequest.ValidFrom = oldTerminationDate;
            adhocaccessrequest.ValidTo = user.TerminationDate;

            AdHocINHRequest inRequest = new AdHocINHRequest()
            {
                IsOffBoardRequest = true,
            };

            adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(inRequest);

            accessrequest.AdHocAccessRequest = adhocaccessrequest;
            LogDebugStep("CreateExtensionTaskForSecurityTeam 2");
            accessrequest.Save(ServiceUser);
            LogDebugStep("CreateExtensionTaskForSecurityTeam 3");
        }
        private bool IsEndpointAccountActive(EndPoint endPoint, IEndpointUserInfo user)
        {
            var accountRecon = AccountReconcilation.GetActiveAccountByAccountName(user.AccountName, endPoint.Id);
            if (accountRecon == null)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"Endpoint: {endPoint.DisplayName}- Account is Inactive for the User:{user.AccountName}");
                return false;
            }
            return true;
        }
    }
}

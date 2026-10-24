using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.UserHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class IDMS : FormExecutorBase
    {
        public IDMS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(idmsLogger)
        {
            Endpoint = endpoint; CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool isExecutedSuccessfully = false;
            try
            {

                AdHocIACMRequest idmsDetail = new AdHocIACMRequest();
                List<string> groups = new List<string>();
                List<string> roles = new List<string>();

                var existingUser = IACMUserInfo.GetByEmployeeId(user.EmployeeId);

                if (CurrentRequest.AdHocAccessRequestId.HasValue && CurrentRequest.AdHocAccessRequest != null)
                {
                    idmsDetail = (AdHocIACMRequest)CurrentRequest.AdHocAccessRequest.GetEntitlementDetail();
                    if (idmsDetail != null && idmsDetail.AdHocIACMRequestDetails != null)
                    {

                        foreach (var ent in idmsDetail.AdHocIACMRequestDetails)
                        {
                            if (ent.EntitlementType == EntitlementType.Group)
                            {
                                groups.Add(ent.Entitlement);
                            }
                            if (ent.EntitlementType == EntitlementType.Role)
                            {
                                roles.Add(ent.Entitlement);
                            }
                        }
                    }
                }
                else if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
                {

                    if (!string.IsNullOrEmpty(fields["Roles"].Value))
                    {
                        foreach (var ent in fields["Roles"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(ent))
                            {
                                roles.Add(ent);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(fields["Groups"].Value))
                    {
                        foreach (var ent in fields["Groups"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(ent))
                            {
                                groups.Add(ent);
                            }
                        }
                    }
                }

                if (existingUser == null)
                {
                    existingUser = IacmUserHelper.CreateAndSaveIacmUserInfo(user, null, this.Endpoint, _idmsLogger);
                    if (existingUser != null)
                    {
                        IACMUserInfo.SaveUsers(new List<IACMUserInfo> { existingUser }, this.Endpoint.Id, "System", user);
                    }
                }
                else
                {
                    existingUser.UserName = user.UserName;
                    existingUser.ADID = user.UserName;
                    existingUser.Save("System");
                }

                WriteProvisioningLog(LogLevelType.Info, $"IACM USer - {existingUser.DisplayName}");

                if (roles.Count() > 0)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"IDMS request Roles Assignment started for user - {user.DisplayName}");
                    if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.AddEntitlement(user, EntitlementType.Role, roles);
                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                    }
                    else if (CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.AddEntitlement(user, EntitlementType.Role, roles);
                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, string.Join(",", roles), user.AccountName);

                    }
                    else if (CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.RemoveEntitlement(user, EntitlementType.Role, roles);

                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, string.Join(",", roles), user.AccountName);
                    }
                }

                if (groups.Count() > 0)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"IDMS request Groups Assignment started for user - {user.DisplayName}");
                    if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.AddEntitlement(user, EntitlementType.Group, groups);
                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                    }
                    else if (CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.AddEntitlement(user, EntitlementType.Group, groups);
                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, CurrentRequest, string.Join(",", roles), user.AccountName);

                    }
                    else if (CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        isExecutedSuccessfully = this.EndpointConnector.RemoveEntitlement(user, EntitlementType.Group, groups);
                        if (!isExecutedSuccessfully) return isExecutedSuccessfully;
                        ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentRemove(user, CurrentRequest, string.Join(",", roles), user.AccountName);
                    }
                }

                if (CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
                {
                    if (!user.IsActive)
                    {
                        user.IsActive = true;
                        user.TerminationDate = null;
                        user.Save(MasterConfigHelper.ServiceUser);
                    }

                    AccountReconcilation acctRecon = AccountReconcilation.GetByEndpointUserId(existingUser.Id, this.Endpoint.Id);

                    if (acctRecon != null && (acctRecon.IsDeleted || acctRecon.IsLocked))
                    {
                        acctRecon.IsDeleted = false;
                        acctRecon.IsLocked = false;
                        acctRecon.DeletedDate = null;
                        acctRecon.LastModifiedBy = MasterConfigHelper.ServiceUser;
                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.ValidToDate = !user.FullTimeEmployee && user.TerminationDate.HasValue ? user.TerminationDate.Value : DateTime.MaxValue;
                        acctRecon.Save(MasterConfigHelper.ServiceUser);
                    }

                    isExecutedSuccessfully = true;
                }


                if (isExecutedSuccessfully && CurrentRequest.RequestSource == AccessRequestSource.OnBoarding || CurrentRequest.RequestSource == AccessRequestSource.Rehire)
                {
                    bool Result = ProvisioningEmailHelper.SendEmailForAccountProvisioning(CurrentRequest, existingUser, user, CurrentRequest.Application.ApplicationType);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing IDMS request. {0}", ex.Message), ex);
                return false;
            }

            return isExecutedSuccessfully;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            try
            {
                if (user == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, string.Format("IACM User Object not found  {0}", user.EmployeeId));

                }
                else
                {
                    var cUser = MasterConfigHelper.ServiceUser;
                    var existingUser = IACMUserInfo.GetByEmployeeId(user.EmployeeId);
                    var acctRecon = AccountReconcilation.GetByDIUserId(user.Id, this.Endpoint.Id);
                    //AccountReconcilation acctRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, existingUser.Id);
                    if (acctRecon == null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Account Recon for IACM User Object not found  {0}", user.EmployeeId));
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"OffBoard IDMS TakeOUSnapshot for User:{user.EmployeeId} started");
                        this.TakeOUSnapshot(user.Id, existingUser, EmployeeEvents.Resignation, EndpointType.IDMS, EntitlementType.Role);
                        WriteProvisioningLog(LogLevelType.Info, $"OffBoard IDMS  TakeOUSnapshot for User:{user.EmployeeId} completed");
                        var entRecon = EntitlementReconcilation.GetAllByAccountReconcilationId(acctRecon.Id);
                        this.EndpointConnector.RemoveAllEntitlements(existingUser);
                        foreach (var item in entRecon)
                        {
                            item.DeletedDate = DateTime.Now;
                            item.IsDeleted = true;
                            item.LastModifiedBy = cUser;
                            item.LastModifiedDate = DateTime.Now;
                            item.Save(cUser);
                        }
                        acctRecon.IsDeleted = true;
                        acctRecon.IsLocked = true;
                        acctRecon.LastModifiedBy = cUser;
                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.OffboardingAccountAction = OffboardingAccountActionType.Delete;
                        acctRecon.ValidToDate = this.CurrentRequest.DeactivationDate;
                        acctRecon.Save(cUser);
                    }
                    user.IsActive = false;
                    user.Reserved4 = Constants.EmployeeStatus.OffBoarded;
                    user.Save(cUser);
                    existingUser.IsDeleted = true;
                    existingUser.DeletedDate = DateTime.Now;
                    existingUser.Save(cUser);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Exception occured at IACM Offboard  {0}", user.EmployeeId), ex);
                return false;
            }

            return true;
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
            try
            {
                var existingUser = IACMUserInfo.GetByEmployeeId(user.EmployeeId);

                if (existingUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, string.Format("IACM User Object not found  {0}", user.EmployeeId));
                }
                else
                {
                    var acctRecon = AccountReconcilation.GetByDIUserId(user.Id, this.Endpoint.Id);
                    //AccountReconcilation acctRecon = AccountReconcilation.GetByEndpointIdAndEndpointUserId(this.Endpoint.Id, existingUser.Id);
                    if (acctRecon == null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, string.Format("Account Recon for IACM User Object not found  {0}", user.EmployeeId));
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"IDMS Transfer request- TakeOUSnapshot Started for {user.KnownAs}");
                        this.TakeOUSnapshot(user.Id, existingUser, EmployeeEvents.OUChange, EndpointType.IDMS, EntitlementType.Role);
                        WriteProvisioningLog(LogLevelType.Info, $"IDMS Transfer request- TakeOUSnapshot completed for {user.KnownAs}");

                        WriteProvisioningLog(LogLevelType.Info, string.Format("Removing All Roles and Groups for User {0}", user.EmployeeId));

                        this.EndpointConnector.RemoveAllEntitlements(existingUser);
                        var entRecon = EntitlementReconcilation.GetAllByAccountReconcilationId(acctRecon.Id);

                        foreach (var item in entRecon)
                        {
                            item.IsDeleted = true;
                            item.LastModifiedBy = "System";
                            item.LastModifiedDate = DateTime.Now;
                            item.Save("System");
                        }

                        acctRecon.LastModifiedBy = "System";
                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.Save("System");

                        WriteProvisioningLog(LogLevelType.Info, string.Format("Applying Default Roles and Groups for User {0}", user.EmployeeId));

                        List<string> groups = new List<string>();
                        List<string> roles = new List<string>();

                        if (!string.IsNullOrEmpty(fields["Roles"].Value))
                        {
                            foreach (var ent in fields["Roles"].Value.Split(','))
                            {
                                if (!string.IsNullOrEmpty(ent))
                                {
                                    roles.Add(ent);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(fields["Groups"].Value))
                        {
                            foreach (var ent in fields["Groups"].Value.Split(','))
                            {
                                if (!string.IsNullOrEmpty(ent))
                                {
                                    groups.Add(ent);
                                }
                            }
                        }

                        if (roles.Count() > 0)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"IDMS request Default Roles Assignment started for user - {user.DisplayName}");
                            this.EndpointConnector.AddEntitlement(user, EntitlementType.Role, roles);
                        }

                        if (groups.Count() > 0)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"IDMS request Default Groups Assignment started for user - {user.DisplayName}");
                            this.EndpointConnector.AddEntitlement(user, EntitlementType.Group, groups);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Exception occured at IACM Transfer  {0}", user.EmployeeId), ex);
                return false;
            }

            return true;
        }

        public override bool Retain(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isSuccess = false;
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                if (this.CurrentRequest.EventUndoRequest != null && !string.IsNullOrWhiteSpace(this.CurrentRequest.EventUndoRequest.TechnicalRoles))
                {
                    var idmsUserDetail = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(this.CurrentRequest.EventUndoRequest.TechnicalRoles);
                    if (idmsUserDetail == null || idmsUserDetail.UserInfo == null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM UndoEvent: Snapshot not found:{user.DisplayName}");
                        return false;
                    }

                    List<string> groups = new List<string>();
                    List<string> roles = new List<string>();
                    var existingUser = IACMUserInfo.GetByEmployeeId(user.EmployeeId);
                    groups = idmsUserDetail.Entitlements.Where(i => i.EntitlementType == EntitlementType.Group).Select(i => i.Entitlement).ToList();
                    roles = idmsUserDetail.Entitlements.Where(i => i.EntitlementType == EntitlementType.Role).Select(i => i.Entitlement).ToList();

                    if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.OUChange)
                    {
                        #region transfer
                        if (groups.Any() || roles.Any())
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"IACM RemoveAllEntitlements for User:{user.DisplayName}");
                            this.EndpointConnector.RemoveAllEntitlements(existingUser);
                            _idmsLogger.WriteLog(LogLevelType.Info, $"IACM RemoveAllEntitlements for User:{user.DisplayName}");
                        }
                        #endregion
                    }
                    else if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                    {
                        #region offboard
                        user.IsActive = true;
                        // Commenting the below lines - For non-employee undo after offboard we shouldnt put the terimination date back from undo request. 
                        //if (idmsUserDetail != null && idmsUserDetail.UserInfo != null && !string.IsNullOrEmpty(idmsUserDetail.UserInfo.Details))
                        //{
                        //    user.TerminationDate = Convert.ToDateTime(idmsUserDetail.UserInfo.Details);
                        //}
                        // For employee we should put it as null
                        if (user.FullTimeEmployee)
                        {
                            user.TerminationDate = null;
                        }
                        user.Save(currentUser);

                        if (existingUser == null)
                        {
                            existingUser = IacmUserHelper.CreateAndSaveIacmUserInfo(user, null, this.Endpoint, _idmsLogger);
                        }
                        existingUser.IsDeleted = false;
                        existingUser.DeletedDate = null;
                        existingUser.IsDisabled = false;
                        existingUser.Save(currentUser);

                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM User:{user.DisplayName} enabled successfully");
                        AccountReconcilation acctRecon = AccountReconcilation.GetByIDMSUserByDIUserID(this.Endpoint.Id, user.Id, false);
                        if (acctRecon != null && (acctRecon.IsDeleted || acctRecon.IsLocked))
                        {
                            acctRecon.EndpointUserId = existingUser.Id;
                            acctRecon.IsLocked = false;
                            acctRecon.IsDeleted = false;
                            acctRecon.DeletedDate = null;
                            acctRecon.AccessRequestId = this.CurrentRequest.Id;
                            acctRecon.LastModifiedDate = DateTime.Now;
                            acctRecon.Save(currentUser);
                            _idmsLogger.WriteLog(LogLevelType.Info, $"IACM User:{user.DisplayName} account successfully");
                        }
                        #endregion
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"IACM restore group and role started for User:{user.DisplayName}");
                    if (groups != null && groups.Count > 0)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM restore group: {string.Join(",", groups)} for User:{user.DisplayName}");
                        isSuccess = this.EndpointConnector.AddEntitlement(user, EntitlementType.Group, groups);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM AdddEntitlements group for Status:{isSuccess}");
                    }
                    if (roles != null && roles.Count > 0)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM AdddEntitlements role for User:{user.DisplayName}");
                        isSuccess = this.EndpointConnector.AddEntitlement(user, EntitlementType.Role, roles);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"IACM AdddEntitlements role: {string.Join(",", roles)} for Status:{isSuccess}");
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"IACM UndoEvent:snapshot is empty or no entitlement found:{user.EmployeeId}");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Exception occured at IACM UndoEvent  {0}", user.EmployeeId), ex);
            }
            return isSuccess;
        }



        public override bool RemediateAttestation(DomainModel.User user, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }
    }
}

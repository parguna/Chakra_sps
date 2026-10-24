
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using System.Linq;
using System.Text;
using DI.Chakra.Services.EmailingProcessor;
using System.Collections.Generic;

namespace DI.Chakra.Connector
{

    //Dummy Connector for Post Provisioning Activities
    public class GeneralConnector : ConnectorBase
    {
        public GeneralConnector(EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            return true;
        }

        public override bool Connect()
        {
            return true;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override void Disconnect()
        {
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public override void GetData()
        {
            try
            {
                var endpointId = this.Endpoint.Id;
                if (this.Endpoint.CanAutomate && this.Endpoint.IsActive)
                {
                    List<string> actions = new List<string>();
                    string serviceUser = "Discovery Process";
                    bool anyRecordExists = false;
                    var inhPermission = InhousePermission.GetOtherSystemPermissionByEndPointSystemId(endpointId).Where(i => !string.IsNullOrWhiteSpace(i.ADGroups)).ToList();
                    if (inhPermission != null && inhPermission.Count > 0)
                    {
                        var adEP = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);

                        try
                        {
                            //Update AccountType,MappingType,Userid and Offboard action
                            DomainModel.InhouseUserInfo.UpdateADMappingAccountType(endpointId, adEP.Id);
                        }
                        catch (Exception ex)
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Exception, $"Error while UpdateADMappingAccountType {ex}", ex);
                        }

                        var inhUsers = InhouseUserInfo.GetByEndpointSystemId(endpointId);
                        var accountRecons = AccountReconcilation.GetAllByEndpointId(endpointId);
                        var adUsers = DomainModel.InhouseUserInfo.GetPresentationOtherSystemDatas(endpointId, adEP.Id);
                        this._idmsLogger.WriteLog(LogLevelType.Info, $"Total New Account: {adUsers.Count}", null);
                        bool newUser = false;
                        string userName = string.Empty;
                        bool isAlreadyExists = false;
                        #region New User
                        if (adUsers != null && adUsers.Count > 0)
                        {
                            actions.Add("New Account Added");
                            anyRecordExists = true;
                            foreach (var adUser in adUsers.GroupBy(i => i.AccountName))
                            {
                                try
                                {
                                    userName = "";
                                    newUser = false;
                                    var adUserData = adUser.FirstOrDefault();
                                    userName = adUserData.AccountName;
                                    var permissions = adUser.Select(i => new { PermissionName = i.Permission, i.PermissionId, i.Description }).Distinct().ToList();
                                    var oINH = inhUsers.FirstOrDefault(i => i.UserName.ToUpper() == userName.ToUpper());
                                    if (oINH == null)
                                    {
                                        oINH = new InhouseUserInfo();
                                        oINH.FirstName = adUserData.FirstName;
                                        oINH.LastName = adUserData.LastName;
                                        oINH.CreatedDate = DateTime.Now;
                                        oINH.AccountCreatedDate = DateTime.Now;
                                        oINH.IsLocked = false;
                                        oINH.IsDeleted = false;
                                        oINH.IsDisabled = false;
                                        oINH.CreatedBy = serviceUser;
                                        newUser = true;
                                    }
                                    else
                                    {
                                        oINH.LastLoginDate = DateTime.Now;
                                        oINH.LastModifiedBy = serviceUser;
                                        if (oINH.IsDeleted)
                                        {
                                            newUser = true;
                                            oINH.IsLocked = false;
                                            oINH.IsDeleted = false;
                                            oINH.IsDisabled = false;
                                        }
                                    }

                                    oINH.EndPointSystemId = endpointId;
                                    oINH.InhouseSystemCode = this.Endpoint.ApplicationCode;
                                    oINH.UserName = adUserData.AccountName;
                                    oINH.DisplayName = adUserData.FullName;
                                    oINH.EmployeeId = adUserData.EmployeeId;
                                    oINH.AccountType = adUserData.AccountType;
                                    oINH.Save(serviceUser);

                                    {
                                        isAlreadyExists = false;
                                        if (newUser)
                                        {
                                            var audit = new AccountEntitlementAuditTrial();
                                            audit.EndPointSystemId = this.Endpoint.Id;
                                            audit.UserName = userName;
                                            audit.AccountType = adUserData.AccountType.GetDescription();
                                            audit.Permission = string.Join(",", adUser.Select(i => i.Permission));
                                            audit.Remarks = "Add Account";
                                            audit.CreatedDate = DateTime.Now;
                                            audit.DisplayName = adUserData.FullName;
                                            audit.CreatedBy = serviceUser;
                                            audit.Save(serviceUser);
                                        }
                                        var oARedit = accountRecons.Where(ar => ar.AccountName.Trim().ToUpper() == userName.Trim().ToUpper() && ar.EndpointSystemId == endpointId).OrderBy(i => i.CreatedDate).FirstOrDefault();
                                        if (oARedit == null)
                                        {
                                            oARedit = new AccountReconcilation();
                                            oARedit.EndpointSystemId = oINH.EndPointSystemId;
                                            oARedit.EndpointUserId = Convert.ToInt64(oINH.Id);
                                            oARedit.AccountName = oINH.AccountName;
                                            oARedit.AccountType = oINH.AccountType;
                                            oARedit.FullName = oINH.DisplayName;
                                            oARedit.CreatedDate = oINH.CreatedDate;
                                            oARedit.CreatedBy = serviceUser;
                                            oARedit.IsDeleted = oINH.IsDeleted;
                                            oARedit.IsLocked = oINH.IsLocked;
                                            oARedit.UserId = adUserData.UserId;
                                            isAlreadyExists = false;
                                        }
                                        else
                                        {
                                            isAlreadyExists = true;
                                            oARedit.EndpointSystemId = oINH.EndPointSystemId;
                                            oARedit.EndpointUserId = Convert.ToInt64(oINH.Id);
                                            oARedit.AccountName = oINH.AccountName;
                                            oARedit.FullName = oINH.DisplayName;
                                            oARedit.AccountType = oINH.AccountType;
                                            oARedit.CreatedDate = oINH.CreatedDate;
                                            oARedit.LastModifiedDate = DateTime.Now;
                                            oARedit.LastModifiedBy = serviceUser;
                                            oARedit.UserId = adUserData.UserId;
                                            if (oINH.IsDeleted && !oARedit.IsDeleted)
                                            {
                                                UpdateException(oARedit, false);
                                            }
                                            if (!oINH.IsDeleted && oARedit.IsDeleted)
                                            {
                                                UpdateException(oARedit, true);
                                            }
                                        }

                                        oARedit.OffboardingAccountAction = (oARedit.AccountType == AccountType.User || oARedit.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                                        oARedit.Save(oARedit, serviceUser);
                                        if (isAlreadyExists == false)
                                        {
                                            accountRecons.Add(oARedit);
                                        }
                                        if (oARedit != null && oARedit.IsDeleted == false)
                                        {
                                            if (permissions != null && permissions.Count() > 0)
                                            {
                                                foreach (var permission in permissions)
                                                {
                                                    try
                                                    {
                                                        var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(oARedit.Id, permission.PermissionName);

                                                        if (entRecon == null)
                                                        {
                                                            entRecon = new EntitlementReconcilation();
                                                            entRecon.EnteredDate = DateTime.Now;
                                                            entRecon.CreatedDate = DateTime.Now;
                                                            EntitlementReconcilation.UpdateException(entRecon, true);
                                                        }

                                                        entRecon.AccountReconcilationId = oARedit.Id;
                                                        entRecon.EndpointType = EndpointType.InhouseSystems;
                                                        entRecon.EndpointSystemId = oARedit.EndpointSystemId;
                                                        entRecon.EndpointUserId = oARedit.EndpointUserId;
                                                        entRecon.UserId = oARedit.UserId;
                                                        entRecon.EntitlementType = (int)EntitlementType.Permission;
                                                        entRecon.AccountName = oARedit.AccountName;
                                                        entRecon.Entitlement = permission.PermissionName;
                                                        entRecon.EntitlementDescription = permission.Description;
                                                        entRecon.EntitlementId = permission.PermissionId;
                                                        entRecon.LastModifiedDate = DateTime.Now;
                                                        if (entRecon.IsDeleted && oARedit.IsDeleted == false)//For active account only
                                                        {
                                                            entRecon.IsDeleted = false;
                                                            entRecon.DeletedDate = null;
                                                        }
                                                        entRecon.LastModifiedBy = "System";
                                                        entRecon.Save(serviceUser);

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        this._idmsLogger.WriteLog(LogLevelType.Exception, $"Failed to create permission:{permission} for New User {userName}", ex);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (oARedit != null)
                                                {
                                                    var entRecons = EntitlementReconcilation.GetByAccountReconcilationId(oARedit.Id, oARedit.EndpointSystemId);
                                                    foreach (var entRecon in entRecons)
                                                    {
                                                        EntitlementReconcilation.UpdateException(entRecon, false);
                                                        entRecon.Save(serviceUser);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this._idmsLogger.WriteLog(LogLevelType.Exception, $"Failed to construct New User {userName}", ex);
                                }
                            }
                        }
                        #endregion
                        #region Existing User with No AD Groups
                        foreach (var inhUser in inhUsers)
                        {
                            try
                            {
                                var adUserNoADGroups = DomainModel.InhouseUserInfo.GetOtherSystemDataNoADGroups(inhUser.UserName, endpointId, adEP.Id);
                                if (adUserNoADGroups != null && adUserNoADGroups.Count > 0)
                                {
                                    var gAccounts = adUserNoADGroups.GroupBy(i => i.UserName);
                                    foreach (var gAccount in gAccounts)
                                    {
                                        var oADRecon = accountRecons.FirstOrDefault(i => i.AccountName.ToUpper() == gAccount.Key.ToUpper() && i.IsDeleted == false);
                                        if (oADRecon != null)
                                        {
                                            anyRecordExists = true;

                                            var permissions = gAccount.Select(i => i.Permission).Distinct();
                                            var audit = new AccountEntitlementAuditTrial();
                                            audit.EndPointSystemId = endpointId;
                                            audit.UserName = oADRecon.AccountName;
                                            audit.AccountType = oADRecon.AccountType.GetDescription();
                                            audit.DisplayName = oADRecon.FullName;
                                            audit.Permission = string.Join(",", permissions);
                                            audit.Remarks = "Remove Role";
                                            audit.CreatedDate = DateTime.Now;
                                            audit.CreatedBy = serviceUser;
                                            audit.Save(serviceUser);

                                            foreach (var permission in permissions)
                                            {
                                                var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(oADRecon.Id, permission);
                                                if (entRecon != null)
                                                {
                                                    EntitlementReconcilation.UpdateException(entRecon, false);
                                                    entRecon.Save(serviceUser);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Failed to Existing User {inhUser?.AccountName}", ex);
                            }
                        }
                        if (anyRecordExists)
                        {
                            actions.Add("No AD Groups.Role Removed");
                        }
                        #endregion
                        #region UserwithNo AD Accounts
                        var inhUserList = DomainModel.InhouseUserInfo.GetInhouseUsersWithoutADAccount(endpointId, adEP.Id);
                        if (inhUserList != null && inhUserList.Count > 0)
                        {
                            actions.Add("No AD Account. Account Removed");
                            AccountEntitlementAuditTrial audit = null;
                            foreach (var inhUser in inhUserList)
                            {
                                audit = null;
                                try
                                {
                                    if (inhUser.AccountType == AccountType.Service || inhUser.AccountType == AccountType.Shared)
                                    {
                                        audit = new AccountEntitlementAuditTrial();
                                        audit.EndPointSystemId = endpointId;
                                        audit.UserName = inhUser.UserName;
                                        audit.Permission = null;
                                        audit.DisplayName = inhUser.DisplayName;
                                        audit.AccountType = inhUser.AccountType.GetDescription();
                                        audit.Remarks = $"Skip Delete Account";
                                        audit.CreatedDate = DateTime.Now;
                                        audit.CreatedBy = serviceUser;
                                        audit.Save(serviceUser);
                                    }
                                    else
                                    {

                                        audit = new AccountEntitlementAuditTrial();
                                        audit.EndPointSystemId = endpointId;
                                        audit.UserName = inhUser.UserName;
                                        audit.Permission = null;
                                        audit.DisplayName = inhUser.DisplayName;
                                        audit.AccountType = inhUser.AccountType.GetDescription();
                                        audit.Remarks = "Remove Account(No AD Account)";
                                        audit.CreatedDate = DateTime.Now;
                                        audit.CreatedBy = serviceUser;
                                        audit.Save(serviceUser);

                                        StringBuilder cleanupQuery = new StringBuilder();
                                        cleanupQuery.AppendLine($"Update DI_M_AccountReconcilation SET IsDeleted = 1, DeletedDate = GETDATE(), IsLocked = 1,ExceptionType=2,ExceptionOccuredDate = GETDATE(), LastAttestedOn = NULL,LastModifiedDate=GETDATE(),LastModifiedBy='{serviceUser}' WHERE Id = {inhUser.AccountReconcilationId} And EndPointSystemId = {endpointId};");
                                        cleanupQuery.AppendLine($"Update DI_INH_UserInfo SET IsDeleted = 1,DeletedDate = GETDATE(), IsDisabled=1,IsLocked=1,LastModifiedDate=GETDATE(),LastModifiedBy='{serviceUser}' WHERE EndPointSystemId = {endpointId} and UserName= '{inhUser.UserName}';");
                                        cleanupQuery.AppendLine($"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId={inhUser.EndpointUserId} AND EndPointSystemId={endpointId};");
                                        AccountReconcilation.ExecuteQueryWithCommandTimeout(cleanupQuery.ToString(), 0);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this._idmsLogger.WriteLog(LogLevelType.Exception, $"User without AD Group failed for {inhUser?.UserName}", ex);
                                }
                            }
                        }
                        #endregion

                        #region Add missing Permission
                        bool isRoleAdded = false;
                        foreach (var inhUser in inhUsers)
                        {
                            try
                            {
                                userName = inhUser.AccountName;
                                var userWithMissingPermissions = DomainModel.InhouseUserInfo.GetOtherSystemDataGetMissingPermission(inhUser.UserName, endpointId, adEP.Id);
                                if (userWithMissingPermissions != null && userWithMissingPermissions.Count > 0)
                                {
                                    isRoleAdded = true;

                                    var oARedit = accountRecons.Where(ar => ar.AccountName.Trim().ToUpper() == userName.Trim().ToUpper() && ar.EndpointSystemId == endpointId).OrderBy(i => i.CreatedDate).FirstOrDefault();
                                    foreach (var permission in userWithMissingPermissions)
                                    {

                                        var audit = new AccountEntitlementAuditTrial();
                                        audit.EndPointSystemId = this.Endpoint.Id;
                                        audit.UserName = userName;
                                        audit.DisplayName = permission.FullName;
                                        audit.AccountType = oARedit.AccountType.GetDescription();
                                        audit.Permission = permission.Permission;
                                        audit.Remarks = "Add Role";
                                        audit.CreatedDate = DateTime.Now;
                                        audit.CreatedBy = serviceUser;
                                        audit.Save(serviceUser);

                                        try
                                        {
                                            var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(oARedit.Id, permission.Entitlement);

                                            if (entRecon == null)
                                            {
                                                entRecon = new EntitlementReconcilation();
                                                entRecon.EnteredDate = DateTime.Now;
                                                entRecon.CreatedDate = DateTime.Now;
                                                EntitlementReconcilation.UpdateException(entRecon, true);
                                            }

                                            entRecon.AccountReconcilationId = oARedit.Id;
                                            entRecon.EndpointType = EndpointType.InhouseSystems;
                                            entRecon.EndpointSystemId = oARedit.EndpointSystemId;
                                            entRecon.EndpointUserId = oARedit.EndpointUserId;
                                            entRecon.UserId = oARedit.UserId;
                                            entRecon.EntitlementType = (int)EntitlementType.Permission;
                                            entRecon.AccountName = oARedit.AccountName;
                                            entRecon.Entitlement = permission.Permission;
                                            entRecon.EntitlementDescription = permission.Description;
                                            entRecon.EntitlementId = permission.PermissionId;
                                            entRecon.LastModifiedDate = DateTime.Now;
                                            if (entRecon.IsDeleted && oARedit.IsDeleted == false)//For active account only
                                            {
                                                entRecon.IsDeleted = false;
                                                entRecon.DeletedDate = null;
                                            }
                                            entRecon.LastModifiedBy = "System";
                                            entRecon.Save(serviceUser);

                                        }
                                        catch (Exception ex)
                                        {
                                            this._idmsLogger.WriteLog(LogLevelType.Exception, $"Failed to create permission:{permission} for New User {userName}", ex);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Failed to Add missing Permission for User {inhUser.AccountName}", ex);
                            }
                        }
                        if (isRoleAdded)
                            actions.Add("Role Added");
                        #endregion
                        #region User with No Permission
                        var accountReconUsers = DomainModel.InhouseUserInfo.GetUserWithNoPermission(endpointId);
                        if (accountReconUsers != null && accountReconUsers.Count > 0)
                        {
                            actions.Add("No Role. Account Removed");
                            AccountEntitlementAuditTrial audit = null;
                            foreach (var accountReconUser in accountReconUsers)
                            {
                                audit = null;
                                try
                                {
                                    if (accountReconUser.AccountType == AccountType.Service || accountReconUser.AccountType == AccountType.Shared)
                                    {
                                        audit = new AccountEntitlementAuditTrial();
                                        audit.EndPointSystemId = endpointId;
                                        audit.UserName = accountReconUser.UserName;
                                        audit.Permission = null;
                                        audit.DisplayName = accountReconUser.DisplayName;
                                        audit.AccountType = accountReconUser.AccountType.GetDescription();
                                        audit.Remarks = $"Skip Delete Account";
                                        audit.CreatedDate = DateTime.Now;
                                        audit.CreatedBy = serviceUser;
                                        audit.Save(serviceUser);
                                    }
                                    else
                                    {

                                        audit = new AccountEntitlementAuditTrial();
                                        audit.EndPointSystemId = endpointId;
                                        audit.UserName = accountReconUser.UserName;
                                        audit.DisplayName = accountReconUser.DisplayName;
                                        audit.Permission = null;
                                        audit.AccountType = accountReconUser.AccountType.GetDescription();
                                        audit.Remarks = "Remove Account(No Role)";
                                        audit.CreatedDate = DateTime.Now;
                                        audit.CreatedBy = serviceUser;
                                        audit.Save(serviceUser);

                                        StringBuilder cleanupQuery = new StringBuilder();
                                        cleanupQuery.AppendLine($"Update DI_M_AccountReconcilation SET IsDeleted = 1, DeletedDate = GETDATE(), IsLocked = 1,ExceptionType=2,ExceptionOccuredDate = GETDATE(), LastAttestedOn = NULL,LastModifiedDate=GETDATE(),LastModifiedBy='{serviceUser}' WHERE Id = {accountReconUser.AccountReconcilationId} And EndPointSystemId = {endpointId};");
                                        cleanupQuery.AppendLine($"Update DI_INH_UserInfo SET IsDeleted = 1, IsDisabled=1,IsLocked=1,LastModifiedDate=GETDATE(),LastModifiedBy='{serviceUser}' WHERE EndPointSystemId = {endpointId} and UserName= '{accountReconUser.UserName}';");
                                        cleanupQuery.AppendLine($"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId={accountReconUser.EndpointUserId} AND EndPointSystemId={endpointId};");
                                        AccountReconcilation.ExecuteQueryWithCommandTimeout(cleanupQuery.ToString(), 0);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    this._idmsLogger.WriteLog(LogLevelType.Exception, $"User dont have permission {accountReconUser?.UserName}", ex);
                                }
                            }
                        }
                        #endregion

                        if ((anyRecordExists || actions.Count > 0) && base.DiscoveryProcess != null)
                            NotifyITAdmin(actions.Distinct().ToList());

                        inhUserList?.Clear();
                        accountRecons?.Clear();
                        inhUsers?.Clear();
                        adUsers?.Clear();

                        

                    }
                    inhPermission?.Clear();
                }
                try
                {
                    //Update entitlement description of entitlement recon from INH permisson
                    if (this.Endpoint.IsActive)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, $"UpdateINHReconEntitlementDescription - Started updating description ", null);
                        DomainModel.InhouseUserInfo.UpdateINHReconEntitlementDescription(endpointId);
                    }
                }
                catch (Exception ex)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Exception, $"Error while UpdateINHReconEntitlementDescription {ex}", ex);
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Error while GetData {this.Endpoint.DisplayName}", ex);
            }
        }
        public void UpdateException(AccountReconcilation accountReconcilation, bool IsAdded)
        {
            if (!IsAdded)
            {
                accountReconcilation.IsDeleted = true;
                accountReconcilation.IsLocked = true;
                accountReconcilation.LastAttestedOn = null;
                accountReconcilation.DeletedDate = DateTime.Now;
                accountReconcilation.ExceptionType = ExceptionTypes.Removed;
                accountReconcilation.ExceptionOccuredDate = DateTime.Now;
            }
            else
            {
                accountReconcilation.LastAttestedOn = null;
                accountReconcilation.IsDeleted = false;
                accountReconcilation.DeletedDate = null;
                accountReconcilation.IsLocked = false;
                accountReconcilation.LockScenario = LockScenario.None;
                accountReconcilation.ExceptionType = ExceptionTypes.Added;
                accountReconcilation.ExceptionOccuredDate = DateTime.Now;
            }

            accountReconcilation.IsResolved = false;
            accountReconcilation.ResolvedBy = string.Empty;
            accountReconcilation.ResolvedStatus = 0;
            accountReconcilation.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.BackEnd.ToString();
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        private void NotifyITAdmin(List<string> actions)
        {

            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.ADReconcilationNotification);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", this.Endpoint.DisplayName);

                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder tableData = new StringBuilder();

                StringBuilder actionData = new StringBuilder();
                if (actions != null && actions.Count > 0)
                {
                    actionData.Append($"<table class='table'><tr><th>EndpointSystem Name</th><th>Action</th></tr><tr><td>{this.Endpoint.DisplayName}</td><td><ul>");
                    foreach (var action in actions)
                    {
                        actionData.Append($"<li>{action}</li>");
                    }
                    actionData.Append($"</ul></td></tr></table>");
                }
                body.Replace("@@ACTIONLIST@@", actionData.ToString());
                body.Replace("@@ENDPOINTNAME@@", this.Endpoint.DisplayName);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                emailInfo.From = template.MailFrom;
                if (!string.IsNullOrWhiteSpace(template.MailTo))
                    emailInfo.To = emailInfo.To + "," + template.MailTo;

                if (!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template:NotifySystemOwnerForNewAccount", ex);
            }
        }
    }
}

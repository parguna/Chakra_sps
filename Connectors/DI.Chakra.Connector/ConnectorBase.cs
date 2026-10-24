
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Connector
{
    public abstract class ConnectorBase : ConnectorUtilityBase
    {
        public ConnectorBase(IdmsLogger logger) : base(logger)
        {
        }
        //Created for SSIDS to check EmployeeType
        public long? UserId { get; set; }
        public IEndpointUserInfo EndpointUserAccount { get; set; }
        public bool IsAccountEnabled { get; set; }
        public bool IsOnlineAccountExists { get; set; }
        public IEndpointUserInfo EndpointOnlineUserAccount { get; set; }
        private EndpointConfiguration m_Config = null;
        public EndpointConfiguration EndpointConfiguration
        {
            get
            {
                return m_Config;
            }

            set
            {
                value.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                    else if (lq.KeyId == Convert.ToInt64(EndpointKey.AzureADUserPassword))
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });

                this.m_Config = value;
            }

        }
        
        public string UnLockChangePasswordAPIMessage { get; set; } = string.Empty;
        public virtual bool IsUserExists(string userName)
        {
            return true;
        }
        public virtual bool IsUserLocked(string userName)
        {
            return true;
        }
        protected string Configuration(EndpointKey Key)
        {
            var config = EndpointConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public virtual IMProcessor IMProcessor
        {
            get { return null; }
        }
        public virtual bool ConnectOnLine()
        {
            return true;
        }
        #region forSelfservice
        public bool IsThroughPasswordReset { get; set; }
        /// <summary>
        /// This is mainly for issue while reseting the password through Self Service. When user Enter same password it get failed. To avoid such issue we use to send the password to User through email
        /// </summary>
        public bool IsResetPasswordSuccessfull { get; set; }
        #endregion
        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract void GetData();
        public abstract bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation);
        public virtual bool ChangePasswordByEnforcingPasswordPolicy(string UserName, string oldPassword, string NewPassword, long DIUserId, bool isReactivation)
        {
            return false;
        }
        public virtual bool ResetPassword(string UserName, string NewPassword, string OldPassword)
        {
            return false;
        }

        public abstract bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields);

        public abstract bool UpdateAccount(IEndpointUserInfo _userInfo);

        public virtual bool LockAccount(IEndpointUserInfo _userInfo)
        {
            return LockAccountRecon(_userInfo);
        }

        public virtual bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            return UnLockAccountRecon(_userInfo);
        }

        public virtual bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            return LockAccountRecon(_userInfo);
        }

        public virtual bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            return UnLockAccountRecon(_userInfo);
        }

        public abstract bool ChangeContainer(IEndpointUserInfo _userInfo);

        public abstract bool ExtendValidity(IEndpointUserInfo _userInfo);

        public virtual bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return AddEntitlements(_userInfo, _entitlementType);
        }

        public virtual bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return RemoveEntitlements(_userInfo, _entitlementType);
        }

        public virtual bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            EntitlementReconcilation.RemoveEntitlementsByUser(_userInfo.EndPointSystemId, _userInfo.Id, DomainModel.MasterConfigHelper.ServiceUser);
            return true;
        }


        //SAP specific methods

        public virtual bool AssignPosition(IEndpointUserInfo _userInfo, string positionCode, string positionText, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            if(_userInfo is SAPHANAUserinfo)
            {
                var userInfo = (SAPHANAUserinfo)_userInfo;
                try
                {
                    LogDebugStep($"AssignPosition: {positionCode}");
                    //var roles = SAPHRP1001.GetSAPRolesByPosition(positionCode, _userInfo.EndPointSystemId);

                    //GOTO: Add position description

                    //userInfo.Roles = roles;

                    SAPHANAHRP1001 newHRP1001 = new SAPHANAHRP1001()
                    {
                        ActualObjectCode = positionCode,
                        BeginDate = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : DateTime.Today.ToString("yyyyMMdd"),
                        EndDate = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : "99991231",
                        EndpointSystemId = userInfo.EndPointSystemId,
                        RelatedObjectCode = userInfo.UserLogonName,
                        RelatedObjectType = "US",
                        ActualObjectType = "S",
                    };

                    LogDebugStep("newHRP1001");
                    var hrp1001 = SAPHANAHRP1001.GetByActualObjectCode(newHRP1001);
                    LogDebugStep($"hrp1001: {hrp1001 == null}");
                    if (hrp1001 == null)
                    {
                        newHRP1001.Save(DomainModel.MasterConfigHelper.ServiceUser);
                    }

                    userInfo.validToDate = endDate.HasValue ? endDate.Value : DateTime.MaxValue;
                    LogDebugStep("AddPositionToEntitlementsReconcilation");
                    this.AddPositionToEntitlementsReconcilation(userInfo, positionCode, positionText);
                    //LogDebugStep("AddEntitlements");
                    //this.AddEntitlements(userInfo, EntitlementType.Role);
                }
                catch (Exception ex)
                {
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while AssignPosition in Connector Base"), ex);
                }
                return true;
            }
            else
            {
                SAPUserinfo userInfo = (SAPUserinfo)_userInfo;
                try
                {
                    LogDebugStep($"AssignPosition: {positionCode}");
                    //var roles = SAPHRP1001.GetSAPRolesByPosition(positionCode, _userInfo.EndPointSystemId);

                    //GOTO: Add position description

                    //userInfo.Roles = roles;

                    SAPHRP1001 newHRP1001 = new SAPHRP1001()
                    {
                        ActualObjectCode = positionCode,
                        BeginDate = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : DateTime.Today.ToString("yyyyMMdd"),
                        EndDate = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : "99991231",
                        EndpointSystemId = userInfo.EndPointSystemId,
                        RelatedObjectCode = userInfo.UserLogonName,
                        RelatedObjectType = "US",
                        ActualObjectType = "S",
                    };

                    LogDebugStep("newHRP1001");
                    var hrp1001 = SAPHRP1001.GetByActualObjectCode(newHRP1001);
                    LogDebugStep($"hrp1001: {hrp1001 == null}");
                    if (hrp1001 == null)
                    {
                        newHRP1001.Save(DomainModel.MasterConfigHelper.ServiceUser);
                    }

                    userInfo.validToDate = endDate.HasValue ? endDate.Value : DateTime.MaxValue;
                    LogDebugStep("AddPositionToEntitlementsReconcilation");
                    this.AddPositionToEntitlementsReconcilation(userInfo, positionCode, positionText);
                    //LogDebugStep("AddEntitlements");
                    //this.AddEntitlements(userInfo, EntitlementType.Role);
                }
                catch (Exception ex)
                {
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while AssignPosition in Connector Base"), ex);
                }
                return true;
            }
        }

        public virtual bool RemovePosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            if(_userInfo is SAPHANAUserinfo)
            {
                var userInfo = (SAPHANAUserinfo)_userInfo;
                try
                {
                    var roles = SAPHANAHRP1001.GetSAPRolesByPosition(positionCode, _userInfo.EndPointSystemId);
                    userInfo.Roles = roles;

                    var existingHRP1001s = SAPHANAHRP1001.GetByRelatedObjectCode(userInfo.UserLogonName, userInfo.EndPointSystemId);

                    SAPHANAHRP1001 existingHRP1001 = new SAPHANAHRP1001()
                    {
                        ActualObjectCode = positionCode,
                        EndpointSystemId = userInfo.EndPointSystemId,
                        RelatedObjectCode = userInfo.UserLogonName,
                        RelatedObjectType = "US",
                        ActualObjectType = "S",
                    };

                    SAPHANAHRP1001.DeleteHRP1001Dump(existingHRP1001);

                    this.RemovePositionFromEntitlementsReconcilation(userInfo, positionCode);
                    this.RemoveEntitlements(_userInfo, EntitlementType.Role);
                }
                catch (Exception ex)
                {
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while RemovePosition in Connector Base"), ex);
                }
            }
            else
            {
                SAPUserinfo userInfo = (SAPUserinfo)_userInfo;
                try
                {
                    var roles = SAPHRP1001.GetSAPRolesByPosition(positionCode, _userInfo.EndPointSystemId);
                    userInfo.Roles = roles;

                    var existingHRP1001s = SAPHRP1001.GetByRelatedObjectCode(userInfo.UserLogonName, userInfo.EndPointSystemId);

                    SAPHRP1001 existingHRP1001 = new SAPHRP1001()
                    {
                        ActualObjectCode = positionCode,
                        EndpointSystemId = userInfo.EndPointSystemId,
                        RelatedObjectCode = userInfo.UserLogonName,
                        RelatedObjectType = "US",
                        ActualObjectType = "S",
                    };

                    SAPHRP1001.DeleteHRP1001Dump(existingHRP1001);

                    this.RemovePositionFromEntitlementsReconcilation(userInfo, positionCode);
                    this.RemoveEntitlements(_userInfo, EntitlementType.Role);
                }
                catch (Exception ex)
                {
                    WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while RemovePosition in Connector Base"), ex);
                }
            }
            
            return true;
        }

        public virtual bool AddEntitlement(IEndpointUserInfo _userInfo, string Validfrom, string ValidTo, EntitlementType entitlementType)
        {
            return base.AddEntitlements(_userInfo, entitlementType);
        }

        public virtual bool DeactivatePassword(string userName)
        {
            return true;
        }

        public virtual bool AssignBusinessPartner(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public virtual bool CreateSAPPhone(IEndpointUserInfo _userInfo, string taskId)
        {
            return true;
        }


        //AD specific methods

        public virtual void MoveOU(IEndpointUserInfo _userInfo, string _ouName)
        {
        }
        public virtual bool IsActiveAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public virtual bool ChangePasswordUsingOldPassword(string UserName, string Password, string oldPassword)
        {
            return true;
        }

        public virtual bool UpdateADManagerAttribute(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public virtual bool AssignRightsToHomeFolder(string samAccountName)
        {
            return true;
        }

        public virtual bool PostReactivationProcedure(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public virtual bool UpdatePOBoxAttribute(IEndpointUserInfo _userInfo, bool add)
        {
            return true;
        }

        public virtual bool UpdateExterEmailAccecss(string accountName, string access)
        {
            return true;
        }


        //NPAC Specific method
        public virtual bool UpdateUserIdentity(string oldUserId, string newUserId)
        {
            return true;
        }

        // HRMS specific methods
        public virtual bool AddEmailIdInHR(string employeeId, string emailId, DateTime beginDate)
        {
            return true;
        }

        // HRMS specific methods
        public virtual bool AddTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            return true;
        }

        public virtual bool AddFLINSLogonIdInHR(string employeeId, string sapLogonId, DateTime beginDate)
        {
            return true;
        }

        public virtual bool DelimitEmailIdInHR(string employeeId, DateTime delimitDate)
        {
            return true;
        }
        public virtual bool DelimitFLINSLogonIdInHR(string employeeId, DateTime delimitDate)
        {
            return true;
        }

        public virtual bool DelimitTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            return true;
        }
        public virtual void SyncTelephoneDirectory()
        {
        }

        public virtual int DownloadAllUsersFromHR(DateTime scanDate)
        {
            return 0;
        }
        public virtual void UpdateAllUserFromHRMS(List<string> perNos)
        {

        }
        public virtual void DownloadSpecificUserFromHR(string pernr)
        {

        }

        public virtual void DiscoverSpecificUserFromHR(string pernr)
        {

        }

        public virtual DateTime GetEffectiveDateByPernr(string pernr, EmployeeEvents eventType, DateTime effectiveStartDate, out bool isTransferWithRetain, EmployeeEventTriggerComment eventTriggerComment=null)
        {
            isTransferWithRetain = false;
            return DateTime.MinValue;
        }

        //O365 specific methods

        public virtual bool RemoveLicense(string userPrincipalName, Dictionary<string, string> licenses, List<string> licensesToRemove)
        {
            return true;
        }

        public virtual bool AssignLicense(string userPrincipalName, Dictionary<string, string> licenses)
        {
            return true;
        }

        public virtual bool UpdateMailBoxSettings(string userPrincipalName, ProvisioningParamCollection fields)
        {
            return true;
        }

        public virtual bool AssignMembers(string emailAddress, Dictionary<string, string> members)
        {
            return true;
        }

        public virtual bool RemoveMembers(string emailAddress, Dictionary<string, string> members)
        {
            return true;
        }

        public virtual bool DownloadUser(string userPrincipalName)
        {
            return true;
        }

        public virtual bool ConvertSharedToRegularMailBox(string emailAddress)
        {
            return true;
        }

        public virtual bool ConvertRegularToSharedMailBox(string emailAddress)
        {
            return true;
        }

        public virtual void ForceSyncOffice365()
        {
        }

        //Specific to GEMS
        public virtual bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            return DeleteAccountRecon(_userInfo);
        }

        public virtual bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        { return true; }

        public virtual bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        { return true; }
        public virtual bool ExecuteTransferAction(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public virtual bool AddMasking(IEndpointUserInfo _userInfo, Dictionary<string, bool> groupWithMaskFlag)
        {
            return true;
        }
        public virtual bool RemoveMasking(IEndpointUserInfo _userInfo, List<string> groupNames)
        {
            return true;
        }
        public virtual void TestSAPConnection()
        {

        }

        public virtual dynamic GetProcessorConfig()
        {
            return null;
        }
        public virtual bool UpdateSFBTelephoneNumbers(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public virtual bool CheckEndPointAccountExists(string accountName)
        {
            return true;
        }
        public virtual bool CheckOnlineAccountExists(string accountName)
        {
            return true;
        }
        public virtual bool ExecuteNewAccountPolicy(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields, AdHocActionType actionType)
        {
            return true;
        }
        public virtual bool UndoOffBoardEvent(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            return true;
        }
        public virtual bool MoveUser(IEndpointUserInfo _userInfo, string registrarPool)
        {
            return true;
        }
        public virtual bool UpdateEndpointUserDetails(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public virtual bool ApplyNewPolicy(IEndpointUserInfo _userInfo, EntitlementType currentUserType, bool moveActionRequired = false)
        {
            return true;
        }
        /// <summary>
        /// Accountname is optional but used mainly for SFB.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public virtual bool IsUserPropertyExists(string propertyName, string propertyValue,string accountName)
        {
            return true;
        }
        public virtual bool RemoveResponseGroupMembers(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {
            return true;
        }
        public virtual bool RemoveDCCPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return true;
        }
        public virtual bool RemoveDCCSPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return true;
        }
        public virtual bool RemoveIDDPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return true;
        }
        public virtual bool UpdateAccountAttribute(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public virtual void UpdateValidRoles(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            var ep = EndPoint.GetById(_userInfo.EndPointSystemId);

            if (ep == null)
                return;

            List<string> invalidRoles = new List<string>();
            List<string> invalidGroups = new List<string>();

            if ((EndpointType)ep.EndpointSystemTypeId == EndpointType.SAP)
            {
                SAPUserinfo _SAPUserInfo = (SAPUserinfo)_userInfo;

                if (_entitlementType == EntitlementType.Role)
                {
                    var entitlements = EntitlementReconcilation.GetByEndpointUserId(_SAPUserInfo.Id, _SAPUserInfo.EndPointSystemId, _entitlementType).Where(x => !x.IsDeleted).Select(x => x.Entitlement).ToList();

                    for (int i = 0; i < _SAPUserInfo.Roles.Count; i++)
                    {
                        if (!entitlements.Contains(_SAPUserInfo.Roles[i], StringComparer.InvariantCultureIgnoreCase))
                        {
                            invalidRoles.Add(_SAPUserInfo.Roles[i]);
                            _SAPUserInfo.Roles.RemoveAt(i);
                        }
                    }
                }
            }
            else if ((EndpointType)ep.EndpointSystemTypeId == EndpointType.SAPEnterprisePortal)
            {
                SAPEPUserInfo _SAPUserInfo = (SAPEPUserInfo)_userInfo;

                if (_entitlementType == EntitlementType.Role)
                {
                    var entitlements = EntitlementReconcilation.GetByEndpointUserId(_SAPUserInfo.Id, _SAPUserInfo.EndPointSystemId, _entitlementType).Where(x => !x.IsDeleted).Select(x => x.Entitlement).ToList();

                    for (int i = 0; i < _SAPUserInfo.Roles.Count; i++)
                    {
                        if (!entitlements.Contains(_SAPUserInfo.Roles[i], StringComparer.InvariantCultureIgnoreCase))
                        {
                            invalidRoles.Add(_SAPUserInfo.Roles[i]);
                            _SAPUserInfo.Roles.RemoveAt(i);
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Group)
                {
                    var entitlements = EntitlementReconcilation.GetByEndpointUserId(_SAPUserInfo.Id, _SAPUserInfo.EndPointSystemId, _entitlementType).Where(x => !x.IsDeleted).Select(x => x.Entitlement).ToList();

                    for (int i = 0; i < _SAPUserInfo.Groups.Count; i++)
                    {
                        if (!entitlements.Contains(_SAPUserInfo.Roles[i], StringComparer.InvariantCultureIgnoreCase))
                        {
                            invalidGroups.Add(_SAPUserInfo.Roles[i]);
                            _SAPUserInfo.Roles.RemoveAt(i);
                        }
                    }
                }
            }

            if (invalidRoles.Count > 0)
            {
                WriteProvisioningLog(LogLevelType.Info, $"User {_userInfo.AccountName} is not assigned with roles {string.Join(",", invalidRoles)}");
            }

            if (invalidGroups.Count > 0)
            {
                WriteProvisioningLog(LogLevelType.Info, $"User {_userInfo.AccountName} is not assigned with groups {string.Join(",", invalidGroups)}");
            }
        }

        public virtual bool IsPositionExist(long endpointUserId, string positionCode, long endpointId, string accountName)
        {
            var positions = EntitlementReconcilation.GetByEndpointUserId(endpointUserId, endpointId, EntitlementType.Position).Where(x => x.Entitlement.Trim() == positionCode.Trim() && !x.IsDeleted).ToList();

            if (positions.Count > 0)
            {
                WriteProvisioningLog(LogLevelType.Info, $"User {accountName} is assigned with positioncode {string.Join(",", positionCode)}");
                return true;
            }
            else
            {
                WriteProvisioningLog(LogLevelType.Info, $"User {accountName} is not assigned with positioncode {string.Join(",", positionCode)}");
                return false;
            }
        }
        public virtual bool RemoveAllEntitlementsByEndpointUserId(long endpointUserId,long endpointSystemId)
        {
            EntitlementReconcilation.RemoveEntitlementsByUser(endpointSystemId, endpointUserId, DomainModel.MasterConfigHelper.ServiceUser);
            return true;
        }
        public virtual string ConstructPassword(long endpointId)
        {
            var endpointPolicy = PasswordPolicyEndpoint.GetEndpointPasswordPolicy(endpointId);
            var policy = endpointPolicy.PasswordPolicy;
            string password = new PasswordResolver().ConstructGeneralUserPassword
                (
                    policy.PasswordLength.HasValue ? policy.PasswordLength.Value : 8,
                    policy.NoOfSpecialCharacters.HasValue ? policy.NoOfSpecialCharacters.Value : 0,
                    policy.NoOfLowerCaseAlphabets.HasValue ? policy.NoOfLowerCaseAlphabets.Value : 0,
                    policy.NoOfUpperCaseAlphabets.HasValue ? policy.NoOfUpperCaseAlphabets.Value : 0,
                    policy.NoOfNumericCharacters.HasValue ? policy.NoOfNumericCharacters.Value : 0
                );

            return password;
        }

        public virtual  bool IsAdminLock(string userLogonName)
        {
            return false;
        }
        public virtual bool UpdateSpecificAttribute(IEndpointUserInfo userInfo, List<KeyValuePair<string,string>> keyValuePair)
        {
            return true;
        }
        public virtual EntitlementType GetUserType(IEndpointUserInfo userInfo)
        {
            return EntitlementType.Default;
        }
        public virtual bool IsUserPartOfAnyGroup(IEndpointUserInfo userInfo)
        {
            return true;
        }
        public virtual bool MailBoxExists(string accountName)
        {
            return true;
        }
        /// <summary>
        /// Mainly used for SAP SSO implementation for user account
        /// </summary>
        /// <param name="UserLogonName">SAP AccountName</param>
        /// <param name="samAccountName">User Primary AD samaccount account(Singaporpower.Local AD</param>
        /// <param name="accountType">Its for User Account only</param>
        /// <returns></returns>
        public virtual bool UpdateSSOAttributes(IEndpointUserInfo userInfo, string samAccountName, AccountType accountType)
        {
            return true;
        }
        /// <summary>
        /// Maily used for self service account Unlock
        /// </summary>
        /// <param name="_userInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual bool UnLockAccount(IEndpointUserInfo _userInfo, long userId)
        {
            return true;
        }
        public virtual bool AssignSAPHANAPosition(IEndpointUserInfo _userInfo, string positionCode, string positionText, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            SAPHANAUserinfo userInfo = (SAPHANAUserinfo)_userInfo;
            try
            {
                LogDebugStep($"AssignPosition: {positionCode}");

                SAPHANAHRP1001 newHRP1001 = new SAPHANAHRP1001()
                {
                    ActualObjectCode = positionCode,
                    BeginDate = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : DateTime.Today.ToString("yyyyMMdd"),
                    EndDate = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : "99991231",
                    EndpointSystemId = userInfo.EndPointSystemId,
                    RelatedObjectCode = userInfo.UserLogonName,
                    RelatedObjectType = "US",
                    ActualObjectType = "S",
                };

                LogDebugStep("newHRP1001");
                var hrp1001 = SAPHANAHRP1001.GetByActualObjectCode(newHRP1001);
                LogDebugStep($"hrp1001: {hrp1001 == null}");
                if (hrp1001 == null)
                {
                    newHRP1001.Save(DomainModel.MasterConfigHelper.ServiceUser);
                }

                userInfo.validToDate = endDate.HasValue ? endDate.Value : DateTime.MaxValue;
                LogDebugStep("AddPositionToEntitlementsReconcilation");
                this.AddSAPHANAPositionToEntitlementsReconcilation(userInfo, positionCode, positionText);
                //LogDebugStep("AddEntitlements");
                //this.AddEntitlements(userInfo, EntitlementType.Role);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while AssignPosition in Connector Base"), ex);
            }
            return true;
        }

        public virtual bool RemoveSAPHANAPosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {

            SAPHANAUserinfo userInfo = (SAPHANAUserinfo)_userInfo;
            try
            {
                var roles = SAPHANAHRP1001.GetSAPRolesByPosition(positionCode, _userInfo.EndPointSystemId);
                userInfo.Roles = roles;

                var existingHRP1001s = SAPHRP1001.GetByRelatedObjectCode(userInfo.UserLogonName, userInfo.EndPointSystemId);

                SAPHANAHRP1001 existingHRP1001 = new SAPHANAHRP1001()
                {
                    ActualObjectCode = positionCode,
                    EndpointSystemId = userInfo.EndPointSystemId,
                    RelatedObjectCode = userInfo.UserLogonName,
                    RelatedObjectType = "US",
                    ActualObjectType = "S",
                };

                SAPHANAHRP1001.DeleteHRP1001Dump(existingHRP1001);

                this.RemoveSAPHANAPositionFromEntitlementsReconcilation(userInfo, positionCode);
                this.RemoveEntitlements(_userInfo, EntitlementType.Role);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while RemovePosition in Connector Base"), ex);
            }
            return true;
        }
    }
}

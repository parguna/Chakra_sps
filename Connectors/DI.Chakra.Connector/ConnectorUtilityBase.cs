using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector
{
    public abstract class ConnectorUtilityBase
    {
        protected readonly IdmsLogger _idmsLogger;
        protected EndPoint Endpoint { get; set; }
        public AccessRequest CurrentRequest { get; set; }
        public DiscoveryProcess DiscoveryProcess { get; set; }
        public Process Process { get; set; }

        public ConnectorUtilityBase(IdmsLogger logger)
        {
            if (logger != null)
            {
                _idmsLogger = logger;
                if (_idmsLogger is DiscoveryLogger)
                    DILog.DiscoveryLogger = (DiscoveryLogger)logger;
                else if (_idmsLogger is ProvisioningLogger)
                    DILog.ProvisioningLogger = (ProvisioningLogger)logger;
            }
            else
            {
                _idmsLogger = new ProvisioningLogger(null, null, null, string.Empty);
            }

        }
        protected virtual void LogDebugStep(string message)
        {
            _idmsLogger.WriteLog(LogLevelType.DebugStep, message, null);
        }
        public bool IsFirstTimeDiscovery
        {
            get
            {
                if (DiscoveryProcess != null)
                {
                    if (DiscoveryProcess.DisableDeltaDiscovery)
                    {
                        return true;
                    }
                    else
                    {
                        if (this.Process == null) return true;
                        return !this.Process.LastRunOn.HasValue;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        protected virtual void AddEntitlementCommon(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, AccountReconcilation acctRecon, string fieldName)
        {
            try
            {
                var entitlements = this.GetValueByFieldName(_userInfo, fieldName);
                if (entitlements != null)
                {
                    foreach (string entitlement in entitlements)
                    {
                        var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, entitlement.Trim(), _entitlementType);

                        if (null == entRecon)
                        {
                            CreateEntitlementReconcilationShellAndSave(acctRecon, _entitlementType, entitlement.Trim());
                        }
                        else
                        {
                            entRecon.EndpointUserId = _userInfo.Id;
                            entRecon.Entitlement = entitlement.Trim();
                            entRecon.IsDeleted = false;
                            entRecon.DeletedDate = null;
                            entRecon.ValidToDate = GetValidTo(acctRecon.UserId);
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"Exception during occured AddEntitlementCommon.Stack trace:{ex?.StackTrace}", ex);
            }
        }
        protected virtual void AddEntitlementCommonWithValidTo(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, AccountReconcilation acctRecon, string fieldName)
        {
            try
            {
                var smsUser = _userInfo as SMSUserInfo;
                var entitlements = this.GetValueByFieldName(_userInfo, fieldName);
                if (entitlements != null)
                {
                    foreach (string entitlement in entitlements)
                    {
                        var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, entitlement.Trim(), _entitlementType);

                        if (null == entRecon)
                        {
                            CreateEntitlementReconcilationShellAndSave(acctRecon, _entitlementType, entitlement.Trim());
                        }
                        else
                        {
                            entRecon.EndpointUserId = _userInfo.Id;
                            entRecon.Entitlement = entitlement.Trim();
                            entRecon.IsDeleted = false;
                            entRecon.DeletedDate = null;
                            entRecon.ValidToDate = GetValidTo(acctRecon.UserId);
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"Exception during occured AddEntitlementCommon.Stack trace:{ex?.StackTrace}", ex);
            }
        }
        protected virtual void AddEntitlementCommonWithDescription(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, AccountReconcilation acctRecon, string fieldName)
        {
            try
            {
                var entitlements = this.GetValueByFieldName(_userInfo, fieldName);
                if (entitlements != null && entitlements.Count > 0)
                {
                    if (_userInfo is GEMSUserInfo)
                    {
                        foreach (string entitlement in entitlements)
                        {
                            string description = string.Empty;
                            try
                            {
                                var gemsRoleInfo = GEMSRoleInfo.GetByRoleName(entitlement);
                                description = gemsRoleInfo.Display_Name;
                                if (gemsRoleInfo != null)
                                {
                                    var gemRole = GEMSRoleMatrix.GetMatrixFromTechnicalRole(gemsRoleInfo.Display_Name, _userInfo.EndPointSystemId)?.BusinessRole ?? "";
                                    if (!string.IsNullOrWhiteSpace(gemRole))
                                    {
                                        description = $"{description}({gemRole})";
                                    }
                                }
                            }
                            catch
                            {
                            }

                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, entitlement.Trim(), _entitlementType);

                            if (null == entRecon)
                            {
                                CreateEntitlementReconcilationShellAndSaveWithDescriptionForGems(acctRecon, _entitlementType, entitlement.Trim(), description);
                            }
                            else
                            {
                                entRecon.EntitlementDescription = description;
                                entRecon.EndpointUserId = _userInfo.Id;
                                entRecon.Entitlement = entitlement.Trim();
                                entRecon.IsDeleted = false;
                                entRecon.DeletedDate = null;
                                entRecon.ValidToDate = GetValidTo(acctRecon.UserId);
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                    else
                    {
                        foreach (string entitlement in entitlements)
                        {
                            string desc = SMSXLRoleCatalogue.GetBuildingandSectionByTechRoles(entitlement)?.Description;

                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, entitlement.Trim(), _entitlementType);

                            if (null == entRecon)
                            {
                                CreateEntitlementReconcilationShellAndSaveWithDescription(acctRecon, _entitlementType, entitlement.Trim(), desc);
                            }
                            else
                            {
                                entRecon.EntitlementDescription = desc;
                                entRecon.EndpointUserId = _userInfo.Id;
                                entRecon.Entitlement = entitlement.Trim();
                                entRecon.IsDeleted = false;
                                entRecon.DeletedDate = null;
                                entRecon.ValidToDate = GetValidTo(acctRecon.UserId);
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"Exception during occured AddEntitlementCommon. Stack trace:{ex?.StackTrace}", ex);
            }
        }
        protected bool ExtendPositionValidity(IEndpointUserInfo _userInfo, string positionCode, DateTime validTo)
        {
            try
            {
                var entitlementRecons = EntitlementReconcilation.GetByEndpointUserIdWithEntitlement(_userInfo.Id, _userInfo.EndPointSystemId, EntitlementType.Position, positionCode);
                if (entitlementRecons != null)
                {
                    foreach (var ent in entitlementRecons)
                    {
                        ent.ValidToDate = validTo;
                        ent.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("Error occured while Adding Entitlements to user - {0} in system - {1}", _userInfo.Id, this.Endpoint.DisplayName), ex);
                return false;
            }
            return true;
        }
       

        protected bool AddEntitlements(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<SMSAccessLevel> entitlementWithDescription = null)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;

                var fieldName = string.Empty;
                switch (_entitlementType)
                {
                    case EntitlementType.Role:
                    case EntitlementType.Level:
                    case EntitlementType.HandPhone:
                        fieldName = "Roles";
                        break;
                    case EntitlementType.Group:
                        fieldName = "Groups";
                        break;
                    case EntitlementType.Profile:
                        fieldName = "Profiles";
                        break;
                    case EntitlementType.Position:
                        fieldName = "Positions";
                        break;
                    case EntitlementType.Permission:
                        fieldName = "Permissions";
                        break;
                    case EntitlementType.AccessLevel:
                        fieldName = "AccessLevels";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(fieldName))
                {
                    if(EntitlementType.AccessLevel == _entitlementType || _userInfo is GEMSUserInfo)
                    {
                        AddEntitlementCommonWithDescription(_userInfo, _entitlementType, acctRecon, fieldName);
                    }
                    else if (_userInfo is SMSUserInfo)
                    {
                        AddEntitlementCommonWithValidTo(_userInfo, _entitlementType, acctRecon, fieldName);
                    }
                    else
                    {
                        AddEntitlementCommon(_userInfo, _entitlementType, acctRecon, fieldName);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("Error occured while Adding Entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        protected virtual bool RemoveEntitlements(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                if (_userInfo == null)
                {
                    DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Info, $"RemoveEntitlements User object null UserId:{this.CurrentRequest.UserId}");
                    return true;
                }

                long? diuserId = this.CurrentRequest?.UserId ?? null;
                if (_entitlementType == EntitlementType.Group)
                {
                    var groups = this.GetValueByFieldName(_userInfo, "Groups");

                    if (groups != null)
                    {
                        foreach (string group in groups)
                        {
                            if (!string.IsNullOrWhiteSpace(group))
                            {
                                var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, group.Trim(), _entitlementType);

                                if (entRecon != null)
                                {
                                    this.DeleteEntitlementReconcilation(entRecon);
                                    AccountReconcilation account = AccountReconcilation.GetById(entRecon.AccountReconcilationId);
                                    if (!EntitlementReconcilation.DoesAnyEntitlementHavingSODConflict(_userInfo.EndPointSystemId, _userInfo.Id))

                                        account.IsSODConflict = false;
                                    if (!EntitlementReconcilation.DoesAnyEntitlementHavingSensitiveAccess(_userInfo.EndPointSystemId, _userInfo.Id))
                                        account.IsSensitiveAccess = false;
                                    account.Save(MasterConfigHelper.ServiceUser);

                                }
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Role)
                {
                    var roles = this.GetValueByFieldName(_userInfo, "Roles");

                    if (roles != null)
                    {
                        foreach (string role in roles)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, role.Trim(), _entitlementType);
                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                                AccountReconcilation account = AccountReconcilation.GetById(entRecon.AccountReconcilationId);
                                if (!EntitlementReconcilation.DoesAnyEntitlementHavingSODConflict(_userInfo.EndPointSystemId, _userInfo.Id))
                                    account.IsSODConflict = false;
                                if (!EntitlementReconcilation.DoesAnyEntitlementHavingSensitiveAccess(_userInfo.EndPointSystemId, _userInfo.Id))
                                    account.IsSensitiveAccess = false;
                                account.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Profile)
                {
                    var profiles = this.GetValueByFieldName(_userInfo, "Profiles");

                    if (profiles != null)
                    {
                        foreach (string profile in profiles)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, profile.Trim(), _entitlementType);
                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Position)
                {
                    var positions = this.GetValueByFieldName(_userInfo, "Positions");

                    if (positions != null)
                    {
                        foreach (string position in positions)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, position.Trim(), _entitlementType);
                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Permission)
                {
                    var permissions = this.GetValueByFieldName(_userInfo, "Permissions");

                    if (permissions != null)
                    {
                        foreach (string permission in permissions)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, permission.Trim(), _entitlementType);

                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Level)
                {
                    var roles = this.GetValueByFieldName(_userInfo, "Roles");

                    if (roles != null)
                    {
                        foreach (string role in roles)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, role.Trim(), _entitlementType);

                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.AccessLevel)
                {
                    var roles = this.GetValueByFieldName(_userInfo, "AccessLevels");

                    if (roles != null)
                    {
                        foreach (string role in roles)
                        {
                            var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, role.Trim(), _entitlementType);

                            if (entRecon != null)
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("Error occured while removing Entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }
            return true;
        }

        protected bool AddPositionToEntitlementsReconcilation(IEndpointUserInfo _userInfo, string positionCode, string positionText)
        {
            try
            {
                if(_userInfo is SAPHANAUserinfo)
                {
                    SAPHANAUserinfo userInfo = (SAPHANAUserinfo)_userInfo;
                    LogDebugStep($"AddPositionToEntitlementsReconcilation, EndpointsystemId: {_userInfo.EndPointSystemId}, EndpointUserId: {_userInfo.Id}");
                    var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id, false);
                    LogDebugStep($"{acctRecon == null}");
                    if (acctRecon != null)
                    {
                        var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, positionCode.Trim(), EntitlementType.Position);
                        if (entRecon == null)
                        {
                            entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Position);
                            if (userInfo.validToDate.HasValue)
                            {
                                entRecon.ValidToDate = userInfo.validToDate;
                            }
                        }

                        if (entRecon != null)
                        {
                            LogDebugStep("entRecon.Save");
                            entRecon.ValidFromDate = DateTime.Now;
                            entRecon.ValidToDate = userInfo.validToDate;
                            entRecon.Entitlement = positionCode.Trim();
                            entRecon.EntitlementDescription = positionText;
                            entRecon.IsDeleted = false;
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                else
                {
                    SAPUserinfo userInfo = (SAPUserinfo)_userInfo;
                    LogDebugStep($"AddPositionToEntitlementsReconcilation, EndpointsystemId: {_userInfo.EndPointSystemId}, EndpointUserId: {_userInfo.Id}");
                    var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id, false);
                    LogDebugStep($"{acctRecon == null}");
                    if (acctRecon != null)
                    {
                        var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, positionCode.Trim(), EntitlementType.Position);
                        if (entRecon == null)
                        {
                            entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Position);
                            if (userInfo.validToDate.HasValue)
                            {
                                entRecon.ValidToDate = userInfo.validToDate;
                            }
                        }

                        if (entRecon != null)
                        {
                            LogDebugStep("entRecon.Save");
                            entRecon.ValidFromDate = DateTime.Now;
                            entRecon.ValidToDate = userInfo.validToDate;
                            entRecon.Entitlement = positionCode.Trim();
                            entRecon.EntitlementDescription = positionText;
                            entRecon.IsDeleted = false;
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("Error occured while Adding Entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }
            return true;
        }

        protected bool RemovePositionFromEntitlementsReconcilation(IEndpointUserInfo _userInfo, string positionCode)
        {
            try
            {
                var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, positionCode, EntitlementType.Position);

                if (entRecon != null)
                {
                    this.DeleteEntitlementReconcilation(entRecon);
                }
            }
            catch (Exception ex)
            {
                // DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, this.CurrentRequest,  LogLevelType.Error, string.Format("Error occured while removing position entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.EndPoint.DisplayName),ex);
            }
            return true;
        }

        protected DataTable ConvertPipesToColumns(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= dtFields.Rows.Count)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                        else
                        {
                            Array.Resize(ref SplitProfile, dtFields.Rows.Count);
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
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
            };
        }
        protected DateTime? GetValidTo(Nullable<long> userId)
        {
            var validTo = DateTime.MaxValue;
            if (CurrentRequest != null && (CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

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
        protected void CreateEntitlementReconcilationShellAndSaveWithDescription(AccountReconcilation acctRecon, EntitlementType _entitlementType, string _Entitlement, string description = null)
        {
            var entitlementReconcilation = new EntitlementReconcilation()
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
                Entitlement = _Entitlement,
                EntitlementDescription = description,
                ValidToDate = GetValidTo(acctRecon.UserId)
            };
            entitlementReconcilation.Save(MasterConfigHelper.ServiceUser);
        }
        protected void CreateEntitlementReconcilationShellAndSaveWithDescriptionForGems(AccountReconcilation acctRecon, EntitlementType _entitlementType, string _Entitlement, string description = null)
        {
            var entitlementReconcilation = new EntitlementReconcilation()
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
                Entitlement = _Entitlement,
                EntitlementDescription = description,
                ValidToDate = GetValidTo(acctRecon.UserId)
            };
            entitlementReconcilation.Save(MasterConfigHelper.ServiceUser);
        }
        protected void CreateEntitlementReconcilationShellAndSave(AccountReconcilation acctRecon, EntitlementType _entitlementType, string _Entitlement, DateTime? validTo = null)
        {
            var entitlementReconcilation = new EntitlementReconcilation()
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
                Entitlement = _Entitlement,
                ValidToDate = GetValidTo(acctRecon.UserId)
            };
            entitlementReconcilation.Save(MasterConfigHelper.ServiceUser);
        }

        protected virtual List<string> GetValueByFieldName(IEndpointUserInfo _userInfo, string fieldName)
        {
            var fld = _userInfo.GetType().GetFields().FirstOrDefault(item => item.Name == fieldName);

            if (fld != null)
            {
                return (List<string>)fld.GetValue(_userInfo);
            }
            else
            {
                return new List<string>();
            }
        }

        protected void DeleteEntitlementReconcilation(EntitlementReconcilation entRecon)
        {
            try
            {
                entRecon.LastAttestedOn = null;
                entRecon.IsDeleted = true;
                entRecon.DeletedDate = DateTime.Now;
                entRecon.IsSODConflict = false; // Clear SOD Conflict Flag if its set.
                entRecon.IsSensitiveAccess = false; // Clear SA Flag if its set.
                entRecon.LastModifiedDate = DateTime.Now;
                entRecon.LastModifiedBy = "System";
                entRecon.LastAttestedOn = null;
                entRecon.Save(MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool IsAccountMarkedAsLocked(long endpointUserId, long endpointId)
        {
            var accountRecon = AccountReconcilation.GetByEndpointUserId(endpointUserId, endpointId);

            if (accountRecon != null)
            {
                return accountRecon.IsLocked;
            }
            else
            {
                return true;
            }
        }

        protected virtual bool LockAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    acctRecon.ValidToDate = this.CurrentRequest.DeactivationDate.HasValue? this.CurrentRequest.DeactivationDate.Value : DateTime.Now;
                    acctRecon.DeletedDate = DateTime.Now;
                    acctRecon.IsLocked = true;
                    acctRecon.IsDeleted = true;
                    acctRecon.LastAttestedOn = null;
                    acctRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while locking the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        protected virtual bool UnLockAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    acctRecon.IsDeleted = false;
                    acctRecon.IsLocked = false;
                    if(CurrentRequest != null)
                    {
                        acctRecon.UserId = CurrentRequest.UserId;
                        acctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                        acctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    }
                    acctRecon.FullName = !string.IsNullOrWhiteSpace(_userInfo.DisplayName) ? _userInfo.DisplayName : _userInfo.AccountName;
                    acctRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while unlocking the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        protected bool DeleteAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    acctRecon.ValidToDate = DateTime.Now;
                    acctRecon.IsLocked = true;
                    acctRecon.IsDeleted = true;
                    acctRecon.Save(MasterConfigHelper.ServiceUser);

                    //Delete Entitlement Recon 
                    var entReconList = EntitlementReconcilation.GetByAccountReconcilationId(acctRecon.Id, _userInfo.EndPointSystemId);
                    if (entReconList != null && entReconList.Count > 0)
                    {
                        foreach (var entRecon in entReconList)
                        {
                            entRecon.ValidToDate = DateTime.Now;
                            entRecon.IsDeleted = true;
                            entRecon.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while Deleting the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        protected void WriteProvisioningLog(LogLevelType loglevel, string messageText, Exception ex = null)
        {
            _idmsLogger.WriteLog(loglevel, messageText, ex);
        }

        protected virtual void WriteDiscoveryLog(LogLevelType loglevel, string messageText, Exception ex = null)
        {
            _idmsLogger.WriteLog(loglevel, messageText, ex);
        }

        protected virtual void WriteDiscoveryLog(LogLevelType loglevel, string messageText, DiscoveryObjectType type, Exception ex = null)
        {
            DILog.DiscoveryLogger.WriteLog(loglevel, messageText, type, ex);
        }

        #region Compliance SOD Violation calculation logic

        public static IList<UserSODComplianceReport> CheckUserCompliance(Dto.UserSODComplianceRequest request)
        {
            ComplianceProcess complianceProcess = null;
            AccessRequest accessRequest = null;
            List<UserSODComplianceReport> complianceRoleDetails = new List<UserSODComplianceReport>();
            try
            {
                if (request.AccessRequestId > 0) accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(request.AccessRequestId);
                if (request.UserId <= 0)
                {
                    if (accessRequest != null)
                    {
                        request.UserId = accessRequest.UserId;
                    }
                    else
                        return complianceRoleDetails;

                }

                if (string.IsNullOrEmpty(request.EntitlementDetails) && accessRequest != null)
                {
                    if (accessRequest.AdHocAccessRequest != null)
                        request.EntitlementDetails = accessRequest.AdHocAccessRequest.AccountType != AccountType.User ? accessRequest.AdHocAccessRequest.SharedAccountName : accessRequest.AdHocAccessRequest.Entitlement;
                }

                if ((request.EndpointSystemId <= 0) && accessRequest != null)
                {
                    request.EndpointSystemId = (long)accessRequest.EndpointSystemId;
                    if (accessRequest.AdHocAccessRequest != null)
                        request.EndpointTypeValue = accessRequest.AdHocAccessRequest.EndpointSystemType;
                }

                var _processId = request.PolicyId;
                //try to get the available policy to validate.
                //understanding is that company will have only one policy.
                //if we have many policy, we are taking the first policy for the SOD.
                if (_processId <= 0)
                {
                    // var defaultPolicy = CompliancePolicy.GetById(2,true);
                    var defaultPolicy = CompliancePolicy.GetAll().Where(a => a.CompliancePolicyType == "SOD" && a.IsActive == true).FirstOrDefault();
                    if (defaultPolicy == null) return complianceRoleDetails;
                    var sODPolicyExecutionDefinition = SODPolicyExecutionDefinition.GetAll().Where(pe => pe.CompliancePolicyId.HasValue && pe.CompliancePolicyId.Value == defaultPolicy.Id).FirstOrDefault();
                    if (sODPolicyExecutionDefinition == null) return complianceRoleDetails;
                    _processId = sODPolicyExecutionDefinition.Id;
                }

                complianceProcess = ComplianceProcess.GetComplianceProcessByPolicyExecutionId(_processId);
                var policyExecutionDefinition = complianceProcess != null ? complianceProcess.PolicyExecutionDefinition : null;
                if (policyExecutionDefinition == null)
                {
                    complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                }
                var policy = policyExecutionDefinition.CompliancePolicy;
                if (policy == null)
                {
                    complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                }

                if (policy.CompliancePolicyType == "SOD")
                {
                    //get all user roles.
                    List<EntitlementReconcilation> userEntitlement = EntitlementReconcilation.GetByUser(request.UserId);
                    if (!string.IsNullOrEmpty(request.EntitlementDetails))
                    {
                        var entitlementDetails = request.EntitlementDetails.Trim();
                        if (!string.IsNullOrEmpty(entitlementDetails))
                        {
                            foreach (string requestedRole in entitlementDetails.Split(','))
                            {
                                userEntitlement.Add(new EntitlementReconcilation
                                {
                                    Entitlement = requestedRole,
                                    EndpointSystemId = request.EndpointSystemId,
                                    EndpointType = request.EndpointTypeValue,
                                });
                            }
                        }
                    }

                    //if its from Access Request get all the entitlements from cart as well.
                    if (request.IsFromAccessRequest)
                    {
                        var interimRequest = InterimStore.GetByUserId(request.RequesteeUserId, true, Infrastructure.Enums.InterimStoreItemType.AccessRequest);

                        if (interimRequest != null)
                        {
                            for (int i = 0; i < interimRequest.Count; i++)
                            {
                                var AdhocaccessRequest = !String.IsNullOrEmpty(interimRequest[i].XmlData) ?
                                   Infrastructure.UtilityMethods.Deserialize<Dto.AdHocAccessRequest>(interimRequest[i].XmlData) : null;
                                if (AdhocaccessRequest != null)
                                {
                                    if (request.UserId == AdhocaccessRequest.RequesteeUserId)
                                    {
                                        userEntitlement.Add(new EntitlementReconcilation
                                        {
                                            Entitlement = AdhocaccessRequest.Entitlement,
                                            EndpointSystemId = (long)AdhocaccessRequest.EndpointSystemId,
                                            EndpointType = AdhocaccessRequest.EndpointSystemType,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        List<SODConflict> lstConflicts = SODConflict.GetBySodConflictCodeIncludingSODRiskCode(conflictfunction.SodConflictCode);
                        if (lstConflicts.Count > 0)
                        {
                            foreach (SODConflict conflict in lstConflicts)
                            {
                                complianceRoleDetails.AddRange(CompliancePolicy.CheckUserConflictBetweenFunctions(userEntitlement, conflict));
                            }
                        }
                    }
                }
                foreach (UserSODComplianceReport userSOD in complianceRoleDetails)
                {
                    userSOD.CompliancePolicyCode = policy.CompliancePolicyCode;
                    userSOD.CompliancePolicyName = policy.CompliancePolicyName;
                    userSOD.CompliancePolicyType = policy.CompliancePolicyType;
                }
                complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));

                return complianceRoleDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        protected bool AddSAPHANAPositionToEntitlementsReconcilation(IEndpointUserInfo _userInfo, string positionCode, string positionText)
        {
            try
            {
                var userInfo = (SAPHANAUserinfo)_userInfo;
                LogDebugStep($"AddPositionToEntitlementsReconcilation, EndpointsystemId: {_userInfo.EndPointSystemId}, EndpointUserId: {_userInfo.Id}");
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id, false);
                LogDebugStep($"{acctRecon == null}");
                if (acctRecon != null)
                {
                    var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, positionCode.Trim(), EntitlementType.Position);
                    if (entRecon == null)
                    {
                        entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Position);
                        if (userInfo.validToDate.HasValue)
                        {
                            entRecon.ValidToDate = userInfo.validToDate;
                        }
                    }

                    if (entRecon != null)
                    {
                        LogDebugStep("entRecon.Save");
                        entRecon.ValidFromDate = DateTime.Now;
                        entRecon.ValidToDate = userInfo.validToDate;
                        entRecon.Entitlement = positionCode.Trim();
                        entRecon.EntitlementDescription = positionText;
                        entRecon.IsDeleted = false;
                        entRecon.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, LogLevelType.Error, string.Format("Error occured while Adding Entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }
            return true;
        }

        protected bool RemoveSAPHANAPositionFromEntitlementsReconcilation(IEndpointUserInfo _userInfo, string positionCode)
        {
            try
            {
                var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, positionCode, EntitlementType.Position);

                if (entRecon != null)
                {
                    this.DeleteEntitlementReconcilation(entRecon);
                }
            }
            catch (Exception ex)
            {
                // DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id, this.CurrentRequest,  LogLevelType.Error, string.Format("Error occured while removing position entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.EndPoint.DisplayName),ex);
            }
            return true;
        }
    }

}

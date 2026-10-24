using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Connector
{
    public class EntitlementPGrpPOrg
    {
        public string Entitlement { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
    }
    public class SRMConnector : ConnectorBase
    {
        readonly char[] delimiter = new char[] { ',' };
        readonly string ServiceUser;
        readonly IdmsLogger _logger;
        public SRMConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            ServiceUser = MasterConfigHelper.ServiceUser;
            this.Endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            _logger = logger;
        }
        public override bool ChangeContainer(Chakra.DomainModel.IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        public override bool Connect()
        {
            return true;
        }

        public override bool CreateAccount(Chakra.DomainModel.IEndpointUserInfo _userInfo, Chakra.Infrastructure.ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override bool ExtendValidity(Chakra.DomainModel.IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public override void GetData()
        {
            _logger.WriteLog(LogLevelType.DebugStep, "SRM Connector Getdata");
            try
            {
                DoReverseEngineeringForUserToRoleMapping();
                ProcessDeletedAccounts();
                UpdateEntitlementRecon();
                EntitlementReconcilation.ProcessDeletedEntitlements(Endpoint.Id, Process, ServiceUser);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.Message, ex);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void ProcessDeletedAccounts()
        {
            List<SAPSRMBusinessRoleMapping> deletedUsers = new List<SAPSRMBusinessRoleMapping>();
            List<SAPUserinfo> sapDeletedUsers = new List<SAPUserinfo>();
            try
            {
                deletedUsers = SAPSRMBusinessRoleMapping.GetSRMDeletedUsers(Process);
                foreach (var user in deletedUsers)
                {
                    user.Delete();
                    //Add log for audit
                }
                sapDeletedUsers = SAPUserinfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, Process);
                foreach (var sapUser in sapDeletedUsers)
                {
                    sapUser.IsActive = false;
                    sapUser.Save(ServiceUser);
                }
                AccountReconcilation.ProcessDeletedAccounts(Endpoint.Id, Process, ServiceUser);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"ProcessDeletedAccounts - Error while deleting account:{ex.Message} StackTrace: {ex.StackTrace}", ex);
            }
            finally
            {
                deletedUsers?.Clear();
                deletedUsers = null;

                sapDeletedUsers?.Clear();
                sapDeletedUsers = null;
            }
        }

        public void UpdateEntitlementRecon()
        {
            string UserLogonName = null;
            List<SAPSRMBusinessRoleMapping> uniqueBusinessRoles = null;
            List<SAPSRMBusinessRoleMapping> userRoles = null;
            try
            {
                
                userRoles = SAPSRMBusinessRoleMapping.GetAll();
                var acocuntNames = userRoles.Select(i => i.UserName).Distinct().ToList();
                foreach (var acocuntName in acocuntNames)
                {
                    UserLogonName = acocuntName;
                    try
                    {
                        var accountReconcilation = AccountReconcilation.GetByAccountName(acocuntName, this.Endpoint.Id);
                        if (accountReconcilation == null)
                        {
                            WriteDiscoveryLog(LogLevelType.Info, $"Account exists in SRMRoleMapping and not in AccountReconcilation not found - {acocuntName}.", DiscoveryObjectType.Role);
                        }
                        var accountWithRoles = userRoles.Where(i => i.UserName.ToUpper() == acocuntName.ToUpper()).Select(i => new { i.UserName, i.BusinessRole }).Distinct();
                        foreach (var accountWithRole in accountWithRoles)
                        {
                            uniqueBusinessRoles = new List<SAPSRMBusinessRoleMapping>();
                            try
                            {
                                uniqueBusinessRoles = userRoles.Where(i => i.UserName == accountWithRole.UserName && i.BusinessRole == accountWithRole.BusinessRole).ToList();
                                var entitlementReconcilation = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(accountReconcilation.Id, accountWithRole.BusinessRole);
                                if (entitlementReconcilation == null)
                                {
                                    entitlementReconcilation = CreateEntitlementReconcilationShell(accountReconcilation, accountWithRole.BusinessRole, 0);
                                }
                                else
                                {
                                    entitlementReconcilation.EntitlementId = 0;
                                    entitlementReconcilation.Entitlement = accountWithRole.BusinessRole;
                                    entitlementReconcilation.IsDeleted = false;
                                    entitlementReconcilation.DeletedDate = null;
                                }
                                if(accountReconcilation != null && accountReconcilation.ValidToDate.HasValue)
                                    entitlementReconcilation.ValidToDate = accountReconcilation.ValidToDate;

                                entitlementReconcilation.LastModifiedDate = DateTime.Now;
                                entitlementReconcilation.LastModifiedBy = "System";
                                var tempresult = uniqueBusinessRoles.GroupBy(j => new { j.BusinessRole, j.POrg })?.Select(k => k.Key.POrg + " - " + string.Join(" , ", k.Select(l => l.PGrp)))?.ToList();
                                entitlementReconcilation.EntitlementDescription = tempresult != null ? string.Join("<br/>", tempresult) : string.Empty;
                                entitlementReconcilation.Save(ServiceUser);
                            }
                            catch (Exception ex)
                            {
                                WriteDiscoveryLog(LogLevelType.Exception, $"Error while create/updating the Entitlement:{ accountWithRole?.BusinessRole}", DiscoveryObjectType.Role, ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, $"Error while create/updating the account:{UserLogonName} entitlement", DiscoveryObjectType.Role, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Error while create/updating the account entitlement", DiscoveryObjectType.Role, ex);
            }
            finally
            {
                userRoles?.Clear();
                userRoles = null;

                uniqueBusinessRoles?.Clear();
                uniqueBusinessRoles = null;
            }
        }

        public void DoReverseEngineeringForUserToRoleMapping()
        {
            //Get All Matrix data
            LogDebugStep("DoReverseEngineeringForUserToRoleMapping");
            SRMHangingRoleInfo.ExecuteQuery("TRUNCATE TABLE DI_M_SRMHangingRoleInfo");
            var srmMatrices = SAPSRMMainRoleMatrix.GetAllSAPSRMEMainRoleMatrix(true, true);
            LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - srmMatrices count : {srmMatrices.Count}");
            var srmEnablerRoleMatrices = SAPSRMEnablerRoleMatrix.GetAllSAPSRMEnablerMatrix(true, true);
            LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - srmEnablerRoleMatrices count : {srmEnablerRoleMatrices.Count}");
            var matrixADGroups = new List<string>();
            var matrixBIRoles = new List<string>();
            var matrixSUPRoles = new List<string>();
            var matrixSRPRoles = new List<string>();
            foreach (var srmMatrice in srmMatrices)
            {
                if (!string.IsNullOrWhiteSpace(srmMatrice.ADGroups))
                {
                    matrixADGroups.AddRange(srmMatrice.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                }
                if (!string.IsNullOrWhiteSpace(srmMatrice.BIRoles))
                {
                    matrixBIRoles.AddRange(srmMatrice.BIRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                }
                if (!string.IsNullOrWhiteSpace(srmMatrice.SUSRoles))
                {
                    matrixSUPRoles.AddRange(srmMatrice.SUSRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                }
                if (!string.IsNullOrWhiteSpace(srmMatrice.SRMRoles))
                    matrixSRPRoles.AddRange(srmMatrice.SRMRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));

                if (!string.IsNullOrWhiteSpace(srmMatrice.SRMEnablerRule))
                {
                    var enablerRoles = srmEnablerRoleMatrices.Where(i => i.SRMEnablerRule.ToUpper() == srmMatrice.SRMEnablerRule.ToUpper());
                    if (enablerRoles != null && enablerRoles.Count() > 0)
                    {
                        foreach (var enablerRole in enablerRoles)
                        {
                            if (!string.IsNullOrWhiteSpace(enablerRole.SRMTechRole))
                                matrixSRPRoles.AddRange(enablerRole.SRMTechRole.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                }
            }

            //Get All entitlement details of SRM Endpoints - SRP, SUP, AD, FLINS
            var applications = new List<ApplicationType> { ApplicationType.ADSingaporePowerLocal, ApplicationType.FLINS_BI, ApplicationType.SAP_SRP, ApplicationType.SAP_SUS };
            var endpoints = EndPoint.GetByApplicationTypes(applications);

            LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - endpoints count : {endpoints.Count}");
            bool biEndPointActive = false;
            var adEndpoint = endpoints.Where(p => p.ApplicationType == ApplicationType.ADSingaporePowerLocal).FirstOrDefault();
            var biEndpoint = endpoints.Where(p => p.ApplicationType == ApplicationType.FLINS_BI).FirstOrDefault();
            var srpEndpoint = endpoints.Where(p => p.ApplicationType == ApplicationType.SAP_SRP).FirstOrDefault();
            var susEndpoint = endpoints.Where(p => p.ApplicationType == ApplicationType.SAP_SUS).FirstOrDefault();

            var longEndpoints = new List<long>();
            foreach (var ep in endpoints)
            {
                if(ep.ApplicationType == ApplicationType.FLINS_BI)
                {
                    biEndPointActive = ep.IsActive;
                    continue;
                }
                longEndpoints.Add(ep.Id);
            }

            var accountTypes = new[] { AccountType.User, AccountType.Shared, AccountType.Service, AccountType.Secondary };

            List<EntitlementReconcilation> entitlements = new List<EntitlementReconcilation>();
            List<EntitlementReconcilation> accountsEntitlements = new List<EntitlementReconcilation>();
            List<AccountReconcilation> srmAccounts = new List<AccountReconcilation>();
            List<SAPSRMBusinessRoleMapping> lstSRMMappings = new List<SAPSRMBusinessRoleMapping>();
            SAPSRMBusinessRoleMapping srmMapping = null;
            AccountReconcilation accountReconcilation = null;
            List<EntitlementPGrpPOrg> lstEntitlementPGrpPOrgs;
            SAPUserinfo sapUserInfo = null;
            List<string> adRoles = new List<string>();
            List<string> biRoles = new List<string>();
            List<string> srpRoles = new List<string>();
            List<string> susRoles = new List<string>();

            List<string> matchedADGroups = new List<string>();
            List<string> matchedBIGroups = new List<string>();
            List<string> matchedSRPGroups = new List<string>();
            List<string> matchedSUPGroups = new List<string>();
            List<string> hangingBIRoles = null;
            foreach (var accountType in accountTypes)
            {
                entitlements = new List<EntitlementReconcilation>();
                accountsEntitlements = new List<EntitlementReconcilation>();
                srmAccounts = new List<AccountReconcilation>();
                try
                {
                    entitlements = EntitlementReconcilation.GetByMultipleEndpoints(longEndpoints, accountType);
                    srmAccounts = AccountReconcilation.GetAcountsByEndpointSystemId(this.Endpoint.Id, accountType);

                    Dictionary<string, long?> distinctUsers = AccountReconcilation.GetByMultipleEndpoints(new List<long> { srpEndpoint.Id }, accountType)
                                                                                    .Where(x => !x.IsDeleted || !x.IsLocked).ToDictionary(x => x.AccountName, x => x.UserId);

                    LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - longEndpoints count: {longEndpoints.Count}, entitlements count: {entitlements.Count}, srmAccounts count: {srmAccounts.Count}");

                    LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - distinctUsers count : {distinctUsers.Count}");
                    //Compare with matrix

                    bool adMatched, biMatched, srpMatched, susMatched = false;
                    lstEntitlementPGrpPOrgs = new List<EntitlementPGrpPOrg>();
                    lstSRMMappings = new List<SAPSRMBusinessRoleMapping>();
                    srmMapping = new SAPSRMBusinessRoleMapping();
                    accountReconcilation = null;

                    sapUserInfo = null;

                   adRoles = new List<string>();
                   biRoles = new List<string>();
                   srpRoles = new List<string>();
                   susRoles = new List<string>();

                    matchedADGroups = new List<string>();
                    matchedBIGroups = new List<string>();
                    matchedSRPGroups = new List<string>();
                    matchedSUPGroups = new List<string>();

                    foreach (var user in distinctUsers)
                    {
                        try
                        {
                            var srqAccountRecon = AccountReconcilation.GetByAccountName(user.Key, srpEndpoint.Id);
                            //User usr = User.GetById(user.Value.Value);
                            if (accountType != AccountType.User)
                            {
                                accountsEntitlements = entitlements.Where(p => p.AccountReconcilation.AccountName.Equals(user.Key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                            }
                            else
                            {
                                accountsEntitlements = entitlements.Where(p =>
                                    p.AccountReconcilation.AccountName.Equals(user.Key, StringComparison.InvariantCultureIgnoreCase)
                                    //&& p.AccountReconcilation.UserId == user.Value
                                    )
                                    .ToList();
                            }
                            #region create/update Account
                            sapUserInfo = SAPUserinfo.GetActiveByUserLogonName(user.Key, this.Endpoint.Id);
                            if (sapUserInfo == null)
                            {
                                sapUserInfo = SAPUserinfo.GetLockedAccountByName(user.Key, user.Value.HasValue ? user.Value.Value : 0, this.Endpoint.Id, accountType);
                                if (sapUserInfo == null)
                                {
                                    LogDebugStep($"SRM Account does not exists.Creating SRM account from SRP SAPUserInfo:{user.Key}");
                                    sapUserInfo = SAPUserinfo.GetActiveByUserLogonName(user.Key, srpEndpoint.Id);
                                    sapUserInfo.Id = 0;
                                    sapUserInfo.EndPointSystemId = this.Endpoint.Id;
                                    sapUserInfo.IsUserLocked = false;
                                    sapUserInfo.validFromDate = DateTime.Now;
                                    sapUserInfo.Save("System");
                                }
                                else
                                {
                                    sapUserInfo.validFromDate = DateTime.Now;
                                    sapUserInfo.LastModifiedDate = DateTime.Now;
                                    sapUserInfo.IsUserLocked = false;
                                    sapUserInfo.EndPointSystemId = this.Endpoint.Id;
                                    sapUserInfo.Save("System");
                                }
                            }
                            else
                            {
                                sapUserInfo.IsUserLocked = false;
                                sapUserInfo.LastModifiedDate = DateTime.Now;
                                sapUserInfo.Save("System");
                            }
                            accountReconcilation = srmAccounts.Where(p => p.AccountName.Equals(user.Key, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (accountReconcilation == null)
                            {
                                accountReconcilation = new AccountReconcilation
                                {
                                    UserId = user.Value,
                                    AccountType = accountType,
                                    AccountName = user.Key,
                                    EndpointSystemId = this.Endpoint.Id,
                                    AccountCreatedDate = srqAccountRecon.AccountCreatedDate,
                                    OffboardingAccountAction = srqAccountRecon.OffboardingAccountAction,
                                    IsDeleted = false,
                                    IsLocked = false,
                                    CreatedBy = ServiceUser,
                                    CreatedDate = DateTime.Now,
                                    FullName = srqAccountRecon?.FullName,
                                    EndpointUserId = sapUserInfo.Id
                                };
                                var accountId = accountReconcilation.Save(ServiceUser);
                                LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - accountId : {accountId}");
                                accountReconcilation.Id = accountId;
                                accountReconcilation.EndpointUserId = sapUserInfo.Id;
                                srmAccounts.Add(accountReconcilation);
                            }
                            else
                            {
                                accountReconcilation.AccountType = accountType;
                                accountReconcilation.OffboardingAccountAction = srqAccountRecon.OffboardingAccountAction;
                                accountReconcilation.IsDeleted = false;
                                accountReconcilation.DeletedDate = null;
                                accountReconcilation.IsLocked = false;
                                accountReconcilation.LastModifiedBy = ServiceUser;
                                accountReconcilation.LastModifiedDate = DateTime.Now;
                                accountReconcilation.EndpointUserId = sapUserInfo.Id;
                                accountReconcilation.FullName = srqAccountRecon?.FullName;
                                accountReconcilation.Save(ServiceUser);
                            }
                            #endregion
                            adRoles = accountsEntitlements.Where(p => p.EndpointSystemId == adEndpoint?.Id).Select(p => p.Entitlement).ToList();
                            if (biEndPointActive)
                            {
                                biRoles = accountsEntitlements.Where(p => p.EndpointSystemId == biEndpoint?.Id).Select(p => p.Entitlement).ToList();
                            }
                            srpRoles = accountsEntitlements.Where(p => p.EndpointSystemId == srpEndpoint?.Id).Select(p => p.Entitlement).ToList();
                            susRoles = accountsEntitlements.Where(p => p.EndpointSystemId == susEndpoint?.Id).Select(p => p.Entitlement).ToList();

                            LogDebugStep($"DoReverseEngineeringForUserToRoleMapping for the account {user.Key} - adRoles count: {adRoles.Count}, biRoles count: {biRoles.Count}, srpRoles count: {srpRoles.Count}, susRoles count: {susRoles.Count}");

                            foreach (var matrix in srmMatrices)
                            {
                                var srmENablerRoleByBusinessRole = srmEnablerRoleMatrices.Where(i => i.SRMEnablerRule.Equals(matrix.SRMEnablerRule, StringComparison.InvariantCultureIgnoreCase)).ToList();
                                adMatched = StringsMatched(matrix.ADGroups, adRoles);
                                if (biEndPointActive)
                                {
                                    biMatched = StringsMatched(matrix.BIRoles, biRoles);
                                }
                                else
                                {
                                    biMatched = true;
                                }
                                srpMatched = StringsMatchedForSRP(matrix.SRMRoles, matrix.SRMEnablerRule, srpRoles, srmENablerRoleByBusinessRole, out lstEntitlementPGrpPOrgs);
                                susMatched = StringsMatched(matrix.SUSRoles, susRoles);

                                LogDebugStep($"DoReverseEngineeringForUserToRoleMapping - adMatched && biMatched && srpMatched && susMatched: {adMatched}-{biMatched}-{srpMatched}-{susMatched}");
                                if (adMatched && biMatched && srpMatched && susMatched)
                                {
                                    if (!string.IsNullOrWhiteSpace(matrix.ADGroups))
                                    {
                                        matchedADGroups.AddRange(matrix.ADGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                                    }
                                    if (biEndPointActive && !string.IsNullOrWhiteSpace(matrix.BIRoles))
                                    {
                                        matchedBIGroups.AddRange(matrix.BIRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                                    }
                                    if (!string.IsNullOrWhiteSpace(matrix.SRMRoles))
                                    {
                                        matchedSRPGroups.AddRange(matrix.SRMRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));

                                        if (lstEntitlementPGrpPOrgs != null && lstEntitlementPGrpPOrgs.Count > 0)
                                            matchedSRPGroups.AddRange(lstEntitlementPGrpPOrgs.Select(i => i.Entitlement).Distinct());
                                    }
                                    if (!string.IsNullOrWhiteSpace(matrix.SUSRoles))
                                    {
                                        matchedSUPGroups.AddRange(matrix.SUSRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                                    }


                                    if (lstEntitlementPGrpPOrgs != null && lstEntitlementPGrpPOrgs.Count > 0)
                                    {
                                        Dictionary<string, List<string>> orgByGroups = new Dictionary<string, List<string>>();
                                        foreach (var pOrgGrp in lstEntitlementPGrpPOrgs)
                                        {
                                            var existingMapping = SAPSRMBusinessRoleMapping.GetMappingByUserNameAndRolepOrg(accountType, user.Key, matrix.BusinessRole, pOrgGrp.POrg);

                                            if (existingMapping == null)
                                            {
                                                //_idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSRMUserAndRoleMapping: Create User RoleMapping:{user.Key}");
                                                if (user.Value.HasValue && user.Value > 0)
                                                {
                                                    var mapping = this.CreateUserRoleMappingObjectShell(user.Value.Value, user.Key, matrix.BusinessRole, pOrgGrp, accountType);
                                                    mapping.Save("System");
                                                }
                                            }
                                            else
                                            {
                                                //_idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSRMUserAndRoleMapping: {pOrgGrp.POrg}");
                                                if (!string.IsNullOrEmpty(pOrgGrp.POrg))
                                                {
                                                    if (orgByGroups.ContainsKey(pOrgGrp.POrg))
                                                    {
                                                        orgByGroups[pOrgGrp.POrg].AddRange(pOrgGrp.PGrp.Split(',').ToList());
                                                    }
                                                    else
                                                    {
                                                        orgByGroups.Add(pOrgGrp.POrg, pOrgGrp.PGrp.Split(',').ToList());
                                                    }
                                                    //existingMapping.AccountType = accountType;
                                                    existingMapping.PGrp = string.Join(",", orgByGroups[pOrgGrp.POrg].Distinct());
                                                    existingMapping.IsActive = true;
                                                    existingMapping.LastModifiedBy = "System";
                                                    existingMapping.LastModifiedDate = DateTime.Now;
                                                    existingMapping.Save("System");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        srmMapping = SAPSRMBusinessRoleMapping.GetByAccountName(accountType, user.Key, matrix.BusinessRole);
                                        if (srmMapping == null)
                                        {
                                            srmMapping = new SAPSRMBusinessRoleMapping
                                            {
                                                UserId = user.Value.HasValue ? user.Value.Value : 0,
                                                AccountType = accountType,
                                                BusinessRole = matrix.BusinessRole,
                                                UserName = user.Key,
                                                IsActive = true,
                                                POrg = null,
                                                PGrp = null,
                                                CreatedBy = ServiceUser,
                                                CreatedDate = DateTime.Now,
                                            };
                                        }
                                        else
                                        {
                                            srmMapping.IsActive = true;
                                            srmMapping.LastModifiedBy = ServiceUser;
                                            srmMapping.LastModifiedDate = DateTime.Now;
                                        }
                                        srmMapping.Save(ServiceUser);
                                    }

                                }
                            }
                            hangingBIRoles = new List<string>();
                            var hangingADGroups = matrixADGroups.Intersect(adRoles).ToList().Where(i => !matchedADGroups.Contains(i, StringComparer.InvariantCultureIgnoreCase));
                            if (biEndPointActive)
                            {
                                hangingBIRoles = matrixBIRoles.Intersect(biRoles).ToList().Where(i => !matchedBIGroups.Contains(i, StringComparer.InvariantCultureIgnoreCase)).ToList();
                            }

                            var hangingSUPRoles = matrixSUPRoles.Intersect(susRoles).ToList().Where(i => !matchedSUPGroups.Contains(i, StringComparer.InvariantCultureIgnoreCase));
                            var hangingSRPRoles = matrixSRPRoles.Intersect(srpRoles).ToList().Where(i => !matchedSRPGroups.Contains(i, StringComparer.InvariantCultureIgnoreCase));
                            if (hangingADGroups.Count() > 0 || hangingBIRoles.Count > 0 || hangingSUPRoles.Count() > 0 || hangingSRPRoles.Count() > 0)
                            {
                                SRMHangingRoleInfo roleInfo = new SRMHangingRoleInfo();
                                roleInfo.AccountName = user.Key;
                                roleInfo.AccountType = accountType;
                                roleInfo.EndPointSystemId = this.Endpoint.Id;
                                roleInfo.ADGroups = string.Join(",", hangingADGroups);
                                roleInfo.BIRoles = string.Join(",", hangingBIRoles);
                                roleInfo.SUPRoles = string.Join(",", hangingSUPRoles);
                                roleInfo.SRPRoles = string.Join(",", hangingSRPRoles);
                                roleInfo.CreatedDate = DateTime.Now;
                                roleInfo.CreatedBy = "System";
                                roleInfo.Save("System");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, $"Error while do reverse engineering on account - {user.Key}. stacktrace:{ex.StackTrace}", ex);
                        }
                        finally
                        {
                            matchedADGroups.Clear();
                            matchedBIGroups.Clear();
                            matchedSRPGroups.Clear();
                            matchedSUPGroups.Clear();

                            adRoles.Clear();
                            biRoles.Clear();
                            srpRoles.Clear();
                            susRoles.Clear();

                            accountsEntitlements.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, $"Exception occured Do ReverseEngineering. StackTrace:{ex.StackTrace} ", ex);
                }
                finally
                {
                    srmAccounts.Clear();
                    entitlements.Clear();
                    accountsEntitlements.Clear();
                }
            }

            adEndpoint = null;
            biEndpoint = null;
            srpEndpoint = null;
            susEndpoint = null;

            matrixADGroups?.Clear();
            matrixADGroups = null;

            matrixBIRoles?.Clear();
            matrixBIRoles = null;

            matrixSUPRoles?.Clear();
            matrixSUPRoles = null;

            matrixSRPRoles?.Clear();
            matrixSRPRoles = null;
            srmMatrices = null;

            adRoles?.Clear();
            adRoles = null;

            biRoles?.Clear();
            biRoles = null;

            srpRoles?.Clear();
            srpRoles = null;

            susRoles?.Clear();
            susRoles = null;

            matchedADGroups?.Clear();
            matchedADGroups = null;

            matchedBIGroups?.Clear();
            matchedBIGroups = null;

            matchedSRPGroups?.Clear();
            matchedSRPGroups = null;

            matchedSUPGroups?.Clear();
            matchedSUPGroups = null;

            entitlements?.Clear();
            entitlements = null;
            accountsEntitlements?.Clear();
            accountsEntitlements = null;
            srmAccounts?.Clear();
            srmAccounts = null;
            lstSRMMappings?.Clear();
            lstSRMMappings = null;

            srmMapping = null;
            accountReconcilation = null;
            lstEntitlementPGrpPOrgs = null;

            srmEnablerRoleMatrices?.Clear();
            srmEnablerRoleMatrices = null;
        }
        private SAPSRMBusinessRoleMapping CreateUserRoleMappingObjectShell(long userId, string accountName, string businessRole, EntitlementPGrpPOrg roleDetail, AccountType accounttype)
        {
            SAPSRMBusinessRoleMapping mapping = new SAPSRMBusinessRoleMapping();
            mapping.AccountType = accounttype;
            mapping.BusinessRole = businessRole;
            mapping.PGrp = roleDetail.PGrp;
            mapping.POrg = roleDetail.POrg;
            mapping.UserId = userId;
            mapping.UserName = accountName;
            mapping.IsActive = true;
            mapping.CreatedBy = "System";
            mapping.CreatedDate = DateTime.Now;
            mapping.LastModifiedBy = "System";
            mapping.LastModifiedDate = DateTime.Now;

            return mapping;
        }
        Dictionary<string, long?> getDistinctAccounts(long SRP_endpointId, AccountType acountType)
        {
            //var distinctUsers = new Dictionary<string, long?>();

            return AccountReconcilation.GetByMultipleEndpoints(new List<long> { SRP_endpointId }, acountType).ToDictionary(x => x.AccountName, x => x.UserId);

            //distinctUsers = accounts.ToDictionary(x => x.AccountName, x => x.UserId);
            //foreach (var account in accounts)
            //{
            //    if (!string.IsNullOrWhiteSpace(account.AccountName))
            //    {
            //        if (!distinctUsers.Keys.Contains(account.AccountName))
            //            distinctUsers.Add(account.AccountName, account.UserId);
            //    }
            //    else
            //    {
            //        WriteDiscoveryLog(LogLevelType.Info, $"Account Name is empty:{account.AccountName} AccountReconId:{account.Id} epId:{account.EndpointSystemId} EpointUserId:{account.EndpointUserId}");
            //    }
            //}
            //return distinctUsers;
        }

        EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, string entitlement, long entitlementId)
        {
            return new EntitlementReconcilation()
            {
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                DeletedDate = null,
                EntitlementType = Convert.ToInt32(EntitlementType.Role),
                EndpointType = (EndpointType)this.Endpoint.EndpointSystemTypeId,
                Entitlement = entitlement,
                EntitlementId = entitlementId
            };
        }

        private bool StringsMatched(string matrixGroups, List<string> currentEntitlements)
        {
            if (string.IsNullOrWhiteSpace(matrixGroups))
                return true;

            if (currentEntitlements == null)
                currentEntitlements = new List<string>();

            var lstGroups = matrixGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (lstGroups.Length == 0)
                return true;

            if (matrixGroups.Length > 0 && currentEntitlements.Count == 0)
                return false;

            var roles = matrixGroups.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var grp in roles)
            {
                if (currentEntitlements.IndexOf(grp.Trim()) == -1)
                    return false;
            }
            return true;
        }

        private bool StringsMatchedForSRP(string matrixSRPRoles, string matrixSRPEnablerRule, List<string> currentSRPEntitlements, List<SAPSRMEnablerRoleMatrix> lstSRMEnablerRoleMatrices, out List<EntitlementPGrpPOrg> lstEntitlementPGrpPOrgs)
        {
            lstEntitlementPGrpPOrgs = new List<EntitlementPGrpPOrg>();

            if (string.IsNullOrWhiteSpace(matrixSRPRoles) && string.IsNullOrWhiteSpace(matrixSRPEnablerRule))
                return true;

            if (matrixSRPRoles == null)
                matrixSRPRoles = string.Empty;

            if (string.IsNullOrWhiteSpace(matrixSRPEnablerRule))
                matrixSRPEnablerRule = string.Empty;

            if (currentSRPEntitlements == null)
                currentSRPEntitlements = new List<string>();

            if ((matrixSRPRoles.Length > 0 || matrixSRPEnablerRule.Length > 0) && currentSRPEntitlements.Count == 0)
                return false;

            var srpRoles = matrixSRPRoles.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var grp in srpRoles)
            {
                if (currentSRPEntitlements.IndexOf(grp.Trim()) == -1)
                    return false;
            }

            foreach (var SRMEnablerRoleMatrice in lstSRMEnablerRoleMatrices)
            {
                if (!string.IsNullOrWhiteSpace(SRMEnablerRoleMatrice.SRMTechRole))
                {
                    var roles = SRMEnablerRoleMatrice.SRMTechRole.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (roles.All(currentSRPEntitlements.Contains))
                    {
                        foreach (var role in roles)
                        {
                            lstEntitlementPGrpPOrgs.Add(new EntitlementPGrpPOrg { Entitlement = role.Trim(), PGrp = SRMEnablerRoleMatrice.PGrp, POrg = SRMEnablerRoleMatrice.POrg });
                        }
                    }
                }
            }
            lstEntitlementPGrpPOrgs = lstEntitlementPGrpPOrgs.Distinct().ToList();
            return true;
        }

        public override bool UpdateAccount(Chakra.DomainModel.IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            var _SAPUserInfo = (SAPUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo?.UserLogonName;

            try
            {
                WriteProvisioningLog(LogLevelType.Info, $"{UserLogonName} - Locking account Started.");
                base.LockAccount(_userInfo);
                WriteProvisioningLog(LogLevelType.Info, $"{UserLogonName} - Locking account Completed.");
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while locking the account - {UserLogonName}.", ex);
                return false;
            }

            return true;
        }
    }
}

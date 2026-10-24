
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;

namespace DI.Chakra.Connector
{
    public class LenelConnector : ConnectorBase
    {
        readonly IdmsLogger _logger;
        readonly string MasterConfigHelperServiceUser;
        #region Constants and Variable
        private const string MSG_Connected = "{0} Host {1} Connected";
        private const string MSG_ConnectionFailed = "{0} Host {1} Connection Failed";
        private const string PROCESSNAME = "DiscoveryProcess";

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private string IPAddress = string.Empty;
        #endregion
        #region Constructors
        public LenelConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.ServerName));
            _logger = logger;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        #endregion
        #region baseMethods
        public override bool Connect()
        {
            bool isConnected = false;
            try
            {
                var path = this.Configuration(EndpointKey.RootConnectionString);
                ManagementScope scope = new ManagementScope(path);
                scope.Connect();
                isConnected = scope.IsConnected;
                WriteDiscoveryLog(LogLevelType.Info, string.Format("L {0} Connected", this.IPAddress));
                WriteDiscoveryLog(LogLevelType.DebugStep, $"Path:{path}, isConnected: {isConnected}");
            }
            catch (System.Management.ManagementException exception)
            {
                // Output expected ManagementExceptions.
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception in connecting OneCard {0}", this.IPAddress), exception);
                return false;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception in connecting OneCard {0}", this.IPAddress), ex);
                return false;
            }
            return isConnected;
        }
        public override void Disconnect()
        {
            this.dicUsers = null;
            this.dicRoles = null;
        }
        #endregion
        #region Discovery
        public override void GetData()
        {
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Lenel_User) != null;
            bool IsAccessLevelSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Lenel_AccessLevel) != null;

            try
            {
                
                if (IsUserSelected)
                {
                    DiscoverBadges();
                    DiscoverCompanies();
                    DiscoverUsers();
                }

                if (IsAccessLevelSelected)
                {
                    DiscoverAccessLevels();
                }
                if (IsUserSelected && IsAccessLevelSelected)
                {
                    DiscoverUserAccessLevel();
                }

                this.UpdateEntitlementRecon();

                SMSUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                SMSUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for OneCard system", ex);
                throw;
            }
            finally
            {
                dicUsers?.Clear();
                dicUsers = null;

                dicRoles?.Clear();
                dicRoles = null;

                GC.Collect();
            }
        }
        private void DiscoverUsers()
        {
            IList<SMSUserInfo> list = null;
            IList<SMSUserInfo> delSMSUsrs = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OneCard Discovering Users"));

                list = this.GetUsers();
                if (list.Count == 0)
                {
                    throw new Exception("OneCard server returned 0 users. Please contact OneCard admin.");
                }
                SMSUserInfo.SaveUsers(list, this.Endpoint.Id, MasterConfigHelperServiceUser);

                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                delSMSUsrs = SMSUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                SMSUserInfo.RemoveSMSUser(delSMSUsrs.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed OneCard Discovering Users"));
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery OneCard Users.", ex);
                throw ex;
            }
            finally
            {
                list?.Clear();
                list = null;

                delSMSUsrs?.Clear();
                delSMSUsrs = null;
            }
        }
        private void DiscoverCompanies()
        {
            IList<SMSCompany> companies = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OneCard Discovering Companies"));
                companies = this.GetCompanies();
                SMSCompany.SaveCompanies(companies, this.Endpoint.Id, MasterConfigHelperServiceUser);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed OneCard Discovering Companies"));
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery OneCard Companies.", ex);
                throw ex;
            }
            finally
            {
                companies?.Clear();
                companies = null;
            }
        }
        private void DiscoverBadges()
        {
            IList<SMSBadgeInfo> badges = null;
            IList<SMSBadgeInfo> delSMSBadges = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OneCard Discovering Badges"),DiscoveryObjectType.Badge);

                badges = this.GetBadges();
                SMSBadgeInfo.SaveBadges(badges, this.Endpoint.Id, MasterConfigHelperServiceUser);
                delSMSBadges = SMSBadgeInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.Process);
                SMSBadgeInfo.RemoveSMSBadges(delSMSBadges.ToList());

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed OneCard Discovering Badges"), DiscoveryObjectType.Badge);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery OneCard Badges.", DiscoveryObjectType.Badge, ex);
                throw ex;
            }
            finally
            {
                badges?.Clear();
                badges = null;
                delSMSBadges?.Clear();
                delSMSBadges = null;
            }
        }
        private void DiscoverAccessLevels()
        {
            IList<SMSAccessLevel> list = new List<SMSAccessLevel>();
            IList<SMSAccessLevel> delaccessLevels = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OneCard Discovering Access Levels"), DiscoveryObjectType.AccessLevel);

                list = this.GetAccessLevels();
                SMSAccessLevel.SaveAccessLevels(list, this.Endpoint.Id, MasterConfigHelperServiceUser);

                delaccessLevels = SMSAccessLevel.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.Process);
                SMSAccessLevel.RemoveSMSAccessLevels(delaccessLevels.ToList());

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed OneCard Discovering Access Levels"), DiscoveryObjectType.AccessLevel);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery OneCard Access Levels.", DiscoveryObjectType.AccessLevel, ex);
                throw ex;
            }
            finally
            {
                list?.Clear();
                list = null;

                delaccessLevels?.Clear();
                delaccessLevels = null;
            }
        }
        private void DiscoverUserAccessLevel()
        {
            List<SMSBadgeInfo> existingbadges = new List<SMSBadgeInfo>();
            List<SMSUserInfo> existingusers = new List<SMSUserInfo>();
            List<SMSAccessLevel> existingAccessLevels = new List<SMSAccessLevel>();
            IList<SMSBadgeAccessLevel> list = new List<SMSBadgeAccessLevel>();
            List<SMSUserAccessLevel> userAccessLevel = new List<SMSUserAccessLevel>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OneCard Discovering User Access Levels"), DiscoveryObjectType.UserAccessLevel);
                list = this.GetUserAccessLevels();
                existingbadges = SMSBadgeInfo.GetAllByEndpointSystemId(this.Endpoint.Id);
                existingusers = SMSUserInfo.GetAllByEndpointSystemId(this.Endpoint.Id);
                existingAccessLevels = SMSAccessLevel.GetAllByEndpointSystemId(this.Endpoint.Id);
                var currentDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                SMSBadgeInfo tempBadge = null; SMSAccessLevel tempAccess = null;
                SMSUserInfo tempuser = null;

                
                foreach (var item in list)
                {
                    //Check only for active badge
                    tempBadge = existingbadges.Where(b => b.BadgeKey == item.BadgeKey && IsValidBadgeDate(b, currentDate)).FirstOrDefault();

                    if (tempBadge != null)
                    {
                        tempuser = existingusers.Where(u => u.UserId == tempBadge.UserId).FirstOrDefault();
                        tempAccess = existingAccessLevels.Where(a => a.AccessLevelId == item.AccessLevelId).FirstOrDefault();

                        if (tempuser != null && tempAccess != null)
                        {
                            userAccessLevel.Add(new SMSUserAccessLevel()
                            {
                                SMSAccessLevel_Id = tempAccess.Id,
                                SMSUserInfo_Id = tempuser.Id
                            });
                        }
                    }
                }

                if (userAccessLevel.Count > 0)
                {
                    SMSUserInfo.SaveUserAccessLevels(userAccessLevel, this.Endpoint.Id);
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed OneCard Discovering User Access Levels"), DiscoveryObjectType.UserAccessLevel);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery OneCard User Access Levels.", DiscoveryObjectType.UserAccessLevel, ex);
                throw ex;
            }
            finally
            {
                existingbadges?.Clear();
                existingbadges = null;

                existingusers?.Clear();
                existingusers = null;

                existingAccessLevels?.Clear();
                existingAccessLevels = null;

                userAccessLevel?.Clear();
                userAccessLevel = null;
                
                list?.Clear();
                list = null;
            }
        }
        private bool IsValidBadgeDate(SMSBadgeInfo item, int currentDate)
        {
            if (!string.IsNullOrWhiteSpace(item.DEACTIVATE) && item.DEACTIVATE.Length > 8)
            {
                var date = Convert.ToInt32(item.DEACTIVATE.GetSubString(8));
                if (date > currentDate && item.STATUS == 1)
                {
                    return true;
                }
            }
            return false;
        }
        private IList<SMSCompany> GetCompanies()
        {
            try
            {
                IList<SMSCompany> list = new List<SMSCompany>();

                SMSCompany company = null;

                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    using (ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath("Lnl_Department"), null))
                    {
                        foreach (var inst in wmiClass.GetInstances())
                        {
                            company = new SMSCompany();
                            company.EndPointSystemId = this.Endpoint.Id;
                            company.CompanyKey = CheckNull.AgainstInt(inst["ID"]);
                            company.SegmentID = CheckNull.AgainstInt(inst["SEGMENTID"]);
                            company.Name = CheckNull.AgainstString(inst["NAME"]);

                            list.Add(company);
                        }
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, $"Found {list.Count.ToString()} OneCard companies(s)", DiscoveryObjectType.Company);

                return list;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Error occured at OneCard Discovery of OneCard companies, Error : {ex.Message}",DiscoveryObjectType.Company);
                throw;
            }
        }
        private IList<SMSUserInfo> GetUsers()
        {
            ManagementScope scope = null;
            //ManagementClass wmiClass = null;
            try
            {
                IList<SMSUserInfo> list = new List<SMSUserInfo>();

                SMSUserInfo SMSUserInfo = null;

                scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    using (ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath("Lnl_Cardholder"), null))
                    {
                        foreach (var inst in wmiClass.GetInstances())
                        {
                            SMSUserInfo = new SMSUserInfo();

                            SMSUserInfo.EndPointSystemId = this.Endpoint.Id;

                            SMSUserInfo.UserId = CheckNull.AgainstInt(inst["ID"]);
                            SMSUserInfo.FIRSTNAME = CheckNull.AgainstString(inst["FIRSTNAME"]);
                            SMSUserInfo.LASTNAME = CheckNull.AgainstString(inst["LASTNAME"]);
                            SMSUserInfo.MIDNAME = CheckNull.AgainstString(inst["MIDNAME"]);
                            SMSUserInfo.SSNO = CheckNull.AgainstString(inst["SSNO"]);

                            SMSUserInfo.Addr1 = CheckNull.AgainstString(inst["ADDR1"]);
                            SMSUserInfo.City = CheckNull.AgainstString(inst["CITY"]);
                            SMSUserInfo.Zip = CheckNull.AgainstString(inst["ZIP"]);
                            SMSUserInfo.EMAIL = CheckNull.AgainstString(inst["EMAIL"]);
                            SMSUserInfo.OPHONE = CheckNull.AgainstString(inst["OPHONE"]);
                            SMSUserInfo.DEPT = Convert.ToInt32(inst["DEPT"]);
                            SMSUserInfo.Extension = CheckNull.AgainstString(inst["EXT"]);

                            list.Add(SMSUserInfo);
                        }
                    }
                }

                _logger.WriteLog(LogLevelType.Info, $"Found {list.Count.ToString()} OneCard User(s)");

                return list;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Error occured at OneCard Discovery of OneCard Users, Error : {ex.Message}",DiscoveryObjectType.User, ex);
                throw;
            }
            finally
            {
                scope = null;
            }
        }
        private IList<SMSBadgeInfo> GetBadges()
        {
            ManagementScope scope = null;
            //ManagementClass wmiClass = null;
            try
            {

                IList<SMSBadgeInfo> list = new List<SMSBadgeInfo>();

                SMSBadgeInfo badgeInfo = null;

                scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                // scope.IsConnected checks if there is a connection. E.g:
                if (scope.IsConnected)
                {
                    using (ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath("Lnl_Badge"), null))
                    {
                        foreach (var inst in wmiClass.GetInstances())
                        {
                            badgeInfo = new SMSBadgeInfo();
                            badgeInfo.EndPointSystemId = this.Endpoint.Id;
                            badgeInfo.BadgeKey = CheckNull.AgainstInt(inst["BADGEKEY"]);
                            badgeInfo.UserId = CheckNull.AgainstInt(inst["PERSONID"]);
                            badgeInfo.BadgeNumber = CheckNull.AgainstLong(inst["ID"]);
                            badgeInfo.BadgeType = CheckNull.AgainstInt(inst["TYPE"]);
                            badgeInfo.DEST_EXEMPT = CheckNull.AgainstInt(inst["DEST_EXEMPT"]);
                            badgeInfo.STATUS = CheckNull.AgainstInt(inst["STATUS"]);
                            badgeInfo.ACTIVATE = CheckNull.AgainstString(inst["ACTIVATE"]);
                            badgeInfo.DEACTIVATE = CheckNull.AgainstString(inst["DEACTIVATE"]);

                            list.Add(badgeInfo);
                        }
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, $"Found {list.Count.ToString()} OneCard Badges(s)", DiscoveryObjectType.Badge);

                return list;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Error occured at OneCard Discovery of OneCard badges, Error : {ex.Message}", DiscoveryObjectType.Badge, ex);
                throw;
            }
            finally
            {
                scope = null;
            }
        }
        private IList<SMSAccessLevel> GetAccessLevels()
        {
            ManagementScope scope = null;
            //ManagementClass wmiClass = null;
            try
            {

                IList<SMSAccessLevel> list = new List<SMSAccessLevel>();
                SMSAccessLevel accessLevel = null;

                scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    using (ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath("Lnl_AccessLevel"), null))
                    {
                        foreach (var inst in wmiClass.GetInstances())
                        {
                            accessLevel = new SMSAccessLevel();

                            accessLevel.EndPointSystemId = this.Endpoint.Id;
                            accessLevel.AccessLevelId = CheckNull.AgainstInt(inst["ID"]);
                            accessLevel.SegmentID = CheckNull.AgainstInt(inst["SEGMENTID"]);
                            accessLevel.Name = CheckNull.AgainstString(inst["NAME"]).Trim();

                            list.Add(accessLevel);
                        }
                    }
                }

                _logger.WriteLog(LogLevelType.Info, $"Found {list.Count.ToString()} OneCard access levels(s)");

                return list;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, $"Error occured at OneCard Discovery of OneCard access levels, Error : {ex.Message}", ex);
                throw;
            }
            finally
            {
                scope = null;
            }
        }
        private IList<SMSBadgeAccessLevel> GetUserAccessLevels()
        {
            ManagementScope scope = null;
            //ManagementClass wmiClass = null;
            try
            {

                IList<SMSBadgeAccessLevel> badgeALList = new List<SMSBadgeAccessLevel>();

                scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    using (ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath("Lnl_AccessLevelAssignment"), null))
                    {
                        foreach (var inst in wmiClass.GetInstances())
                        {
                            SMSBadgeAccessLevel badgeAccessLevel = new SMSBadgeAccessLevel();
                            badgeAccessLevel.BadgeKey = CheckNull.AgainstInt(inst["BADGEKEY"]);
                            badgeAccessLevel.AccessLevelId = CheckNull.AgainstInt(inst["ACCESSLEVELID"]);
                            badgeALList.Add(badgeAccessLevel);
                        }
                    }
                }

                _logger.WriteLog(LogLevelType.Info, $"Found {badgeALList.Count.ToString()} OneCard user access levels(s)");

                return badgeALList;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, $"Error occured at OneCard Discovery of OneCard user access levels, Error : {ex.Message}", ex);
                throw;
            }
            finally
            {
                scope = null;
            }
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);

                if (exsitngAccountrecon == null) exsitngAccountrecon = new List<AccountReconcilation>();
                if (existingEntilements == null) existingEntilements = new List<EntitlementReconcilation>();

                exsitngAccountrecon = exsitngAccountrecon.Where(i => i.IsDeleted == false).ToList();

                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                var smsRolsMatrix = SMSXLRoleCatalogue.GetAll();
                dicUsers = SMSUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                SMSUserInfo smsUserInfo = null;
                var description = string.Empty;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    smsUserInfo = null;
                    try
                    {
                        smsUserInfo = SMSUserInfo.GetByIdIncludeAccesslevels(item.Key);

                        if (smsUserInfo != null && smsUserInfo.SMSAccessLevels != null)
                        {
                            foreach (SMSAccessLevel entitlement in smsUserInfo.SMSAccessLevels)
                            {

                                var roleMatrix = smsRolsMatrix.FirstOrDefault(i=> i.TechnicalRole.Equals(entitlement.EntitlementName,StringComparison.InvariantCultureIgnoreCase));
                                if(roleMatrix != null)
                                {
                                    description = roleMatrix.Description;
                                }
                                else
                                {
                                    description = entitlement.EntitlementDesc;
                                }

                                //[20/01/23] added the checking to retrieve only the active record
                                acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == smsUserInfo.Id && !a.IsDeleted).FirstOrDefault();  
                                if (acctRecon == null) continue;

                                entRecon = existingEntilements.Where(e => e.EndpointUserId == smsUserInfo.Id &&
                                                                            e.Entitlement.ToLower() == Convert.ToString(entitlement.Name).Trim().ToLower() &&
                                                                            e.EntitlementType == (int)EntitlementType.AccessLevel).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (smsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.AccessLevel, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = entitlement.Name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.AccountName = acctRecon.AccountName;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = entitlement.Name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = description;
                                    entRecon.AccountName = acctRecon.AccountName;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. SMS User -{0}", smsUserInfo.MIDNAME), ex);
                    }
                }

                try
                {
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", ex);
                }
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Process Entitlements Recon.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                existingEntilements?.Clear();
            }
        }
        #endregion
        #region Provisioning
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            return true;
        }
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            SMSUserInfo smsUserInfo = null;
            ManagementObject cardholder = null;
            try
            {
                smsUserInfo = (SMSUserInfo)_userInfo;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"One Card Create Account Started for User {smsUserInfo.NewPernr}", null);
                    // Check if cardholder already exist
                    cardholder = GetCardholderOnSsNo(smsUserInfo.SSNO, scope);
                    // Create and refetch cardholder if not already exist
                    if (cardholder == null)
                    {
                        CreateCardHolder(scope, smsUserInfo);
                        cardholder = GetCardholderOnSsNo(smsUserInfo.SSNO, scope);
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, $"Cardholder with PERNR '{smsUserInfo.SSNO}' already exist. Please manually remove first.", null);
                        return false;
                    }
                    // Ensure there is a valid cardholder object in order to add badge
                    if (cardholder != null)
                    {
                        int? cardholderID = cardholder["ID"] as int?;

                        _idmsLogger.WriteLog(LogLevelType.Info, $"Cardholder ID '{cardholderID}'", null);
                        if (cardholderID.HasValue)
                        {
                            smsUserInfo.UserId = cardholderID.Value;
                            smsUserInfo.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).UserId = cardholderID.Value;

                            var isActiveBadgeExists = GetValidBadgeCountForUser(smsUserInfo.UserId, scope);

                            if (isActiveBadgeExists == false)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Badge Count 0 ", null);
                                using (ManagementBaseObject addedBadge = AddBadge(smsUserInfo, scope))
                                {
                                    if (addedBadge == null)
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Error, $"addedBadge == null , Error while adding dummy badge to cardholder with PERNR {smsUserInfo.SSNO}.", null);
                                        return false;
                                    }
                                    smsUserInfo.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).BadgeKey = (int)addedBadge["BADGEKEY"];
                                }
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, $"Badge with type {fields["DummyBadgeTypeId"]} already exist. Remove it manually first.", null);
                                return false;
                            }
                        }
                    }
                    _idmsLogger.WriteLog(LogLevelType.Info, $"User {smsUserInfo.MIDNAME} Created Successfully in OneCard.", null);

                    return true;
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }

            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while creating user {smsUserInfo.MIDNAME} in OneCard.", ex);
                return false;
            }
            finally
            {
                try
                {
                    cardholder?.Dispose();
                    cardholder = null;
                }
                catch
                {
                }
            }
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            SMSUserInfo smsUserInfo = null;

            try
            {
                smsUserInfo = (SMSUserInfo)_userInfo;

                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    // Check if cardholder already exist
                    using (ManagementObject cardholder = GetCardholderOnSsNo(smsUserInfo.SSNO, scope))
                    {
                        if (cardholder != null)
                        {
                            cardholder["FIRSTNAME"] = smsUserInfo.FIRSTNAME;
                            cardholder["LASTNAME"] = smsUserInfo.LASTNAME;
                            cardholder["MIDNAME"] = smsUserInfo.MIDNAME;
                            cardholder["SSNO"] = smsUserInfo.SSNO;

                            cardholder["ADDR1"] = smsUserInfo.Addr1;
                            cardholder["CITY"] = smsUserInfo.City;
                            cardholder["ZIP"] = smsUserInfo.Zip;
                            cardholder["EMAIL"] = smsUserInfo.EMAIL;
                            cardholder["OPHONE"] = smsUserInfo.OPHONE;
                            cardholder["DEPT"] = smsUserInfo.DEPT;
                            cardholder["EXT"] = smsUserInfo.Extension;
                            cardholder.Put();

                            int? cardholderID = cardholder["ID"] as int?;

                            if (cardholderID.HasValue)
                            {
                                int badgeCount = GetBadgeCountForUser(smsUserInfo.UserId, scope);

                                if (badgeCount != 0)
                                {
                                    using (ManagementObject badge = GetBadgeByUserAndBadgeType(smsUserInfo.UserId, scope))
                                    {
                                        if (badge != null)
                                        {
                                            var tempSMSuserBadge = SMSUserInfo.GetByIdIncludeAccesslevels(smsUserInfo.Id);
                                            if (tempSMSuserBadge.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1) != null)
                                            {
                                                badge["ACTIVATE"] = tempSMSuserBadge.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).ACTIVATE;// DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                                                badge["DEACTIVATE"] = tempSMSuserBadge.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).DEACTIVATE;// DateTime.Today.AddYears(5).ToString("yyyyMMddHHmmss.ffffff+480");
                                                badge.Put();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        _idmsLogger.WriteLog(LogLevelType.Info, $"User {smsUserInfo.MIDNAME} updated Successfully in OneCard.", null);
                        return true;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while updating user {smsUserInfo.MIDNAME} in OneCard. {ex?.ToString()}", ex);
                return false;
            }
        }
        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            SMSUserInfo smsUserInfo = null;

            try
            {
                smsUserInfo = (SMSUserInfo)_userInfo;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Lock User SSNO:{smsUserInfo?.SSNO} started", null);
                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUserInfo.SSNO, scope);

                    if (cardholder != null)
                    {
                        int count = DeactivateBadgesForUser((int)(cardholder["ID"]), scope);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Locked {0} badges in total", count), null);

                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} locked Successfully in OneCard.", smsUserInfo.MIDNAME), null);

                        base.LockAccount(smsUserInfo);
                        return true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, $"User {smsUserInfo.SSNO} does not exists in OneCard.", null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, $"Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while locking user {0} in OneCard.", smsUserInfo.MIDNAME), ex);
                return false;
            }
        }
        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            SMSUserInfo smsUser = null;

            try
            {
                smsUser = (SMSUserInfo)_userInfo;
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Trying to UnLock Cardholder with PERNR '{0}'  in OneCard.", smsUser.SSNO), null);
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUser.SSNO, scope);

                    if (cardholder != null)
                    {
                        smsUser.SSNO = smsUser.NewPernr;

                        cardholder["FIRSTNAME"] = smsUser.FIRSTNAME;
                        cardholder["LASTNAME"] = smsUser.LASTNAME;
                        cardholder["MIDNAME"] = smsUser.MIDNAME;
                        cardholder["SSNO"] = smsUser.SSNO;

                        cardholder["ADDR1"] = smsUser.Addr1;
                        cardholder["CITY"] = smsUser.City;
                        cardholder["ZIP"] = smsUser.Zip;
                        cardholder["EMAIL"] = smsUser.EMAIL;
                        cardholder["OPHONE"] = smsUser.OPHONE;
                        cardholder["DEPT"] = smsUser.DEPT;
                        cardholder["EXT"] = smsUser.Extension;
                        cardholder.Put();

                        var badge = smsUser.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1);
                        if(badge != null && badge.BadgeKey > 0)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Existing BadgeInfo found for user: ModifyBadgeDetails. Badge STATUS:{badge.STATUS}");
                            ModifyBadgeDetails(smsUser, badge);
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, "New BadgeInfo for user: AddBadge");
                            var addedBadge = AddBadge(smsUser, scope);

                            smsUser.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).BadgeKey = (int)addedBadge["BADGEKEY"];
                        }
                        

                        base.UnLockAccount(smsUser);
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Cardholder with PERNR '{0}' successfully unlocked in OneCard.", smsUser.SSNO), null);

                        return true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} does not exists in OneCard.", smsUser.SSNO), null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ManagementException(OneCard) ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while Unlocking user {0} in OneCard.", smsUser.MIDNAME), ex);
                return false;
            }
        }
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            SMSUserInfo smsUser = null;

            try
            {

                smsUser = (SMSUserInfo)_userInfo;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {

                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUser.SSNO, scope);

                    if (cardholder != null)
                    {
                        cardholder["FIRSTNAME"] = smsUser.FIRSTNAME;
                        cardholder["LASTNAME"] = smsUser.LASTNAME;
                        cardholder["MIDNAME"] = smsUser.MIDNAME;
                        cardholder["SSNO"] = smsUser.SSNO;

                        cardholder["ADDR1"] = smsUser.Addr1;
                        cardholder["CITY"] = smsUser.City;
                        cardholder["ZIP"] = smsUser.Zip;
                        cardholder["EMAIL"] = smsUser.EMAIL;
                        cardholder["OPHONE"] = smsUser.OPHONE;
                        cardholder["DEPT"] = smsUser.DEPT;
                        cardholder["EXT"] = smsUser.Extension;

                        cardholder.Put();
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} updated Successfully in OneCard.", smsUser.MIDNAME), null);

                    return true;
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating user {0} in OneCard.", smsUser.MIDNAME), ex);
                return false;
            }
        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            LogDebugStep("ExtendValidity 1");
            SMSUserInfo smsUser = null;

            try
            {

                smsUser = (SMSUserInfo)_userInfo;
                LogDebugStep($"ExtendValidity 2 smsUser == null: {smsUser == null}");
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    LogDebugStep("ExtendValidity 3");
                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUser.SSNO, scope);

                    if (cardholder != null)
                    {
                        int? cardholderID = cardholder["ID"] as int?;

                        if (cardholderID.HasValue)
                        {
                            int badgeCount = GetBadgeCountForUser(smsUser.UserId, scope);
                            LogDebugStep("ExtendValidity 4");
                            if (badgeCount != 0)
                            {
                                var badge = GetBadgeByUserAndBadgeType(smsUser.UserId, scope);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"User {smsUser.MIDNAME} Badge exists {badge != null}");

                                if (badge != null)
                                {
                                    LogDebugStep($"ExtendValidity 6 : {smsUser.UserValidTo}");
                                    //badge["DEACTIVATE"] = smsUser.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).DEACTIVATE;// DateTime.Today.AddYears(5).ToString("yyyyMMddHHmmss.ffffff+480");
                                    badge["DEACTIVATE"] = smsUser.UserValidTo;// DateTime.Today.AddYears(5).ToString("yyyyMMddHHmmss.ffffff+480");
                                    badge.Put();
                                    _idmsLogger.WriteLog(LogLevelType.Info,$"User {smsUser.MIDNAME} extended successfully in OneCard.Validity{smsUser.UserValidTo}", null);
                                }
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"User {smsUser.MIDNAME} Cardholder empty");
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"User {smsUser.MIDNAME} Cardholder Null");
                    }

                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("User {0} extended successfully in OneCard.", smsUser.MIDNAME), null);

                    return true;
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while extending user {0} in OneCard.", smsUser.MIDNAME), ex);
                return false;
            }
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SMSUserInfo smsUser = null;

            try
            {
                smsUser = (SMSUserInfo)_userInfo;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUser.SSNO, scope);

                    if (cardholder != null)
                    {
                        int? cardholderID = cardholder["ID"] as int?;

                        if (cardholderID.HasValue)
                        {
                            // Get the badge(s) for the user. There should really
                            // only be one dummy badge
                            var badge = GetBadgeByUserAndBadgeType(cardholderID.Value, scope);

                            if (badge != null)
                            {
                                List<int> accessLevels = new List<int>();

                                foreach (string accessLevel in smsUser.AccessLevels)
                                {
                                    var accesslevel = SMSAccessLevel.GetAccessLevelByName(accessLevel.Trim());
                                    if (accesslevel != null)
                                    {
                                        accessLevels.Add(accesslevel.AccessLevelId);
                                    }
                                }
                                var temp = string.Join(",", accessLevels);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"AccessLevel: {temp}");
                                AssignAccessLevels((ManagementObject)badge, accessLevels.ToArray(), scope);
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("No badge of the correct type ({0}) found.", smsUser.SSNO), null);
                                return false;
                            }
                        }
                        base.AddEntitlement(_userInfo, _entitlementType);
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Added access levels {0} to Cardholder with PERNR '{1}' successfully.", string.Join(",", smsUser.AccessLevels), smsUser.SSNO), null);

                        return true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} does not exists in OneCard.", smsUser.SSNO), null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while assigning access levels to user {0} in OneCard.", smsUser.MIDNAME), ex);
                return false;
            }
        }
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SMSUserInfo smsUser = null;

            try
            {

                smsUser = (SMSUserInfo)_userInfo;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    // Check if cardholder already exist
                    var cardholder = GetCardholderOnSsNo(smsUser.SSNO, scope);

                    if (cardholder != null)
                    {
                        int? cardholderID = cardholder["ID"] as int?;

                        if (cardholderID.HasValue)
                        {
                            // Get the badge(s) for the user. There should really
                            // only be one dummy badge
                            var badge = GetBadgeByUserAndBadgeType(cardholderID.Value, scope);

                            if (badge != null)
                            {
                                List<int> accessLevels = new List<int>();

                                foreach (string accessLevel in smsUser.AccessLevels)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AccessLevel :{accessLevel}");
                                   var accesslevl = SMSAccessLevel.GetAccessLevelByName(accessLevel.Trim());
                                    if (accesslevl != null)
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"Remove AccessLevel : {accesslevl.EntitlementName}");
                                        accessLevels.Add(accesslevl.AccessLevelId);
                                    }
                                }
                                RemoveAccessLevelAssignments((ManagementObject)badge, accessLevels.ToArray(), scope);
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("No badge of the correct type ({0}) found.", smsUser.SSNO), null);
                                return false;
                            }
                        }
                        base.RemoveEntitlement(_userInfo, _entitlementType);
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removed access levels {0} from Cardholder with PERNR '{1}' successfully.", string.Join(",", smsUser.AccessLevels), smsUser.SSNO), null);

                        return true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} does not exists in OneCard.", smsUser.SSNO), null);
                        return false;
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, "Error while connecting to Lenel server.", null);
                    return false;
                }
            }
            catch (ManagementException mex)
            {
                string message = "ERROR: " + mex.Message;

                if (mex.ErrorInformation != null)
                {
                    var mgmtErrorDescription = mex.ErrorInformation["Description"];

                    if (mgmtErrorDescription != null)
                        message += ". " + mgmtErrorDescription.ToString();
                }

                _idmsLogger.WriteLog(LogLevelType.Error, message, null);
                return false;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while assigning access levels to user {0} in OneCard.", smsUser.MIDNAME), ex);
                return false;
            }
        }
        #endregion
        #region Connector Private Methods
        /// <summary>
        /// Helper method to search for and retrieve the 
        /// cardholder based on his/her social security 
        /// number
        /// </summary>
        /// <param name="ssNo"></param>
        /// <returns></returns>
        private ManagementObject GetCardholderOnSsNo(string ssNo, ManagementScope scope)
        {
            try
            {
                var query = new ObjectQuery(string.Format("SELECT * FROM Lnl_Cardholder WHERE SSNO = '{0}'", ssNo));

                using (ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher(scope, query))
                {
                    foreach (var inst in searcher.Get())
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"SSNO:{ssNo} found in OneCard");
                        return inst as ManagementObject; // Should only be one
                    }
                    return null; // Not found
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"GetCardholderOnSsNo failed for SSNo:{ssNo}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Creates a new card holder with a randomly generated social security number
        /// </summary>
        /// <returns>True if successful</returns>
        private void CreateCardHolder(ManagementScope scope, SMSUserInfo smsUser)
        {
            try
            {
                using (ManagementObject cardholder = CreateWmiInstance("Lnl_Cardholder", scope))
                {
                    cardholder["FIRSTNAME"] = smsUser.FIRSTNAME;
                    cardholder["LASTNAME"] = smsUser.LASTNAME;
                    cardholder["MIDNAME"] = smsUser.MIDNAME;
                    cardholder["SSNO"] = smsUser.SSNO;

                    cardholder["ADDR1"] = smsUser.Addr1;
                    cardholder["CITY"] = smsUser.City;
                    cardholder["ZIP"] = smsUser.Zip;
                    cardholder["EMAIL"] = smsUser.EMAIL;
                    cardholder["OPHONE"] = smsUser.OPHONE;
                    cardholder["DEPT"] = smsUser.DEPT;
                    cardholder["EXT"] = smsUser.Extension;

                    cardholder.Put();
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Created cardholder with PERNR '{0}'", smsUser.SSNO), null);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while creating cardholder with PERNR '{0}'", smsUser.SSNO), ex);
                throw;
            }
        }

        /// <summary>
        /// Adds a badge to the person. If badgeID is not provided, it is assumed thLnl_Cardholderat 
        /// the badge ID (card number) is automatically created by the badge type 
        /// configuration. This should be the case for the dummy badge as it is best that 
        /// this is generated based on configuration.
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="badgeType"></param>
        /// <param name="badgeID"></param>
        /// <returns>The created badge object instance if successful</returns>
        private ManagementBaseObject AddBadge(SMSUserInfo smsUser, ManagementScope scope)
        {
            try
            {
                //Get the badge class object
                using (ManagementClass badgeClass = new ManagementClass(scope, new ManagementPath("Lnl_Badge"), null))
                {


                    //Create a new instance of the Badge
                    using (ManagementObject newBadgeInstance = badgeClass.CreateInstance())
                    {
                        var badge = smsUser.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1);

                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[ID] - {badge.BadgeNumber}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[PIN]  -{smsUser.Password}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $" newBadgeInstance[PERSONID] - {smsUser.UserId}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[STATUS] -{badge.STATUS}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[TYPE] -{badge.BadgeType}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[DEST_EXEMPT] -{badge.DEST_EXEMPT}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[ACTIVATE] -{badge.ACTIVATE}", null);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"newBadgeInstance[DEACTIVATE] -{badge.DEACTIVATE}", null);

                        newBadgeInstance["ID"] = badge.BadgeNumber;
                        newBadgeInstance["PIN"] = smsUser.Password;
                        newBadgeInstance["PERSONID"] = smsUser.UserId;
                        newBadgeInstance["STATUS"] = badge.STATUS; //Active
                        newBadgeInstance["TYPE"] = badge.BadgeType;
                        newBadgeInstance["DEST_EXEMPT"] = badge.DEST_EXEMPT;
                        newBadgeInstance["ACTIVATE"] = badge.ACTIVATE;// DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                        newBadgeInstance["DEACTIVATE"] = badge.DEACTIVATE;// DateTime.Today.AddYears(5).ToString("yyyyMMddHHmmss.ffffff+480");

                        //Get an input parameters object for AddBadge method
                        using (ManagementBaseObject inParams = badgeClass.GetMethodParameters("AddBadge"))
                        {
                            inParams.Properties["BadgeIn"].Value = newBadgeInstance;

                            //Execute the method
                            using (ManagementBaseObject outParamObject = badgeClass.InvokeMethod("AddBadge", inParams, null))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"badgeClass.InvokeMethod() Completed", null);
                                ManagementBaseObject addedBadge = null;

                                if (outParamObject != null)
                                {
                                    //Display results
                                    object outObj = outParamObject["BadgeOut"];
                                    addedBadge = (ManagementBaseObject)outObj;
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Added dummy badge {0} to cardholder with PERNR '{1}' successfully.", badge.BadgeNumber, smsUser.SSNO), null);
                                    return addedBadge;
                                }
                                else
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, "Failed to add the badge. Check the DataConduIT.log for details", null);
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AddBadge()-  Error while assigning dummy badge to cardholder with PERNR '{smsUser.SSNO}' stacktrace:{ex.StackTrace} {ex?.ToString()}", ex);
                throw ex;
            }
        }
        public bool ModifyBadgeDetails(SMSUserInfo smsUser, SMSBadgeInfo badgeInfo)
        {
            bool isSuccessfull = false;
            try
            {
                ManagementObject BADGEKEY = null;
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                var sQuery = $"SELECT * FROM Lnl_Badge WHERE PERSONID = {badgeInfo.UserId} AND BadgeKey={badgeInfo.BadgeKey}";
                _idmsLogger.WriteLog(LogLevelType.DebugStep, sQuery);
                var query = new ObjectQuery(sQuery);
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    using (ManagementObjectCollection badgeModifyDetails = searcher.Get())
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"badgeModifyDetails is Not Null?: {badgeModifyDetails != null} && BadgeKey: { badgeInfo.BadgeKey} Count:{badgeModifyDetails?.Count}");
                        if (badgeModifyDetails != null && badgeModifyDetails.Count > 0 && badgeInfo.BadgeKey > 0)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Existing Badge found");
                            foreach (ManagementObject modifyObject in badgeModifyDetails)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"BADGEKEY : {modifyObject["BADGEKEY"]} STATUS: {modifyObject["STATUS"]} ");
                                BADGEKEY = modifyObject;
                                BADGEKEY["ID"] = badgeInfo.BadgeNumber;
                                BADGEKEY["PIN"] = smsUser.Password;
                                BADGEKEY["PERSONID"] = smsUser.UserId;
                                BADGEKEY["STATUS"] = badgeInfo.STATUS; //Active
                                BADGEKEY["TYPE"] = badgeInfo.BadgeType;
                                BADGEKEY["DEST_EXEMPT"] = badgeInfo.DEST_EXEMPT;
                                BADGEKEY["ACTIVATE"] = badgeInfo.ACTIVATE;// DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                                BADGEKEY["DEACTIVATE"] = badgeInfo.DEACTIVATE;// DateTime.Today.AddYears(5).ToString("yyyyMMddHHmmss.ffffff+480");
                                BADGEKEY.Put();
                                isSuccessfull = true;
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Badge {BADGEKEY["ID"]} Updated successfully");
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Badge doesn't exists.Create new Badge");
                            var addedBadge = AddBadge(smsUser, scope);
                            smsUser.SMSBadgeInfos.FirstOrDefault(item => item.STATUS == 1).BadgeKey = (int)addedBadge["BADGEKEY"];
                        }
                    }
                }
            }
            catch (ManagementException ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AddBadge() ManagementException-  Error while assigning dummy badge to cardholder. {ex?.ToString()}", ex);
                throw ex;
            }
            catch(Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AddBadge() Exception-  Error while assigning dummy badge to cardholder. {ex?.ToString()}", ex);
                throw ex;
            }
            return isSuccessfull;
        }
        /// <summary>
        /// Helper method to create a new instance of a class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private ManagementObject CreateWmiInstance(string className, ManagementScope scope)
        {
            try
            {
                ManagementClass wmiClass = new ManagementClass(scope, new ManagementPath(className), null);

                return wmiClass.CreateInstance();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the number of badges of the specified cardholder and badge type
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="badgeType"></param>
        /// <returns>The badge count</returns>
        private int GetBadgeCountForUser(int personID, ManagementScope scope)
        {
            try
            {
                var query = new ObjectQuery(string.Format("SELECT * FROM Lnl_Badge WHERE PersonID = {0} AND STATUS = {1}", personID, 1));

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    var coll = searcher.Get();
                    return coll == null ? 0 : coll.Count;
                }
            }
            catch
            {
                throw;
            }
        }
        private bool GetValidBadgeCountForUser(int personID, ManagementScope scope)
        {
            var activaeDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            bool IsAccountExists = false;
            try
            {
                var query = new ObjectQuery(string.Format("SELECT * FROM Lnl_Badge WHERE PersonID = {0}", personID));

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    var coll = searcher.Get();
                    foreach (ManagementObject item in coll)
                    {
                        var obj = item["DEACTIVATE"];
                        if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()) && obj.ToString().Length > 8)
                        {
                            var date = Convert.ToInt32(obj.ToString().GetSubString(8));
                            if (date > activaeDate)
                            {
                                IsAccountExists = true;
                                break;
                            }
                        }
                    }
                    return IsAccountExists;
                }
            }
            catch(Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"GetValidBadgeCountForUser() Exception occured while checking Badge DEACTIVATE. {ex?.ToString()}", ex);
                throw ex;
            }
        }
        private ManagementObject GetBadgeByUserAndBadgeType(int personID, ManagementScope scope)
        {
            try
            {
                var query = new ObjectQuery(string.Format("SELECT * FROM Lnl_Badge WHERE PersonID = {0} AND STATUS = {1}", personID, 1));

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (var inst in searcher.Get())
                {
                    return inst as ManagementObject; // Should only be one
                }

                return null; // Not found
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Removes (deletes) all the badges belonging to the cardholder. 
        /// Effectively "locking" the cardholder  without deleting the profile.
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>The number of badges deleted</returns>
        private int DeactivateBadgesForUser(int personID, ManagementScope scope)
        {
            try
            {
                var query = new ObjectQuery(string.Format("SELECT * FROM Lnl_Badge WHERE PersonID = {0} AND STATUS = {1}", personID, 1));

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    int deletedCount = 0;
                    foreach (var inst in searcher.Get())
                    {
                        var instO = inst as ManagementObject;

                        if (instO != null)
                        {
                            //string validFrom = CheckNull.AgainstString(inst["ACTIVATE"]);

                            //DateTime validFromDate = UtilityMethods.GetDateFromString(validFrom);

                            instO["DEACTIVATE"] = DateTime.Now.ToString("yyyyMMddHHmmss.ffffff+480");
                            //if (validFromDate < DateTime.Today)
                            //{
                            //    instO["DEACTIVATE"] = DateTime.Today.ToString("yyyyMMddHHmmss.ffffff+480");
                            //}
                            //else
                            //{
                            //    instO["DEACTIVATE"] = DateTime.Today.AddDays(1).ToString("yyyyMMddHHmmss.ffffff+480");
                            //}

                            //instO["STATUS"] = 3;
                            instO.Put();

                            deletedCount++;
                        }
                    }

                    return deletedCount;
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"DeactivateBadgesForUser failed. {ex.Message} {ex?.ToString()}", ex);
            }
        }

        private void AssignAccessLevels(ManagementObject badge, int[] accessLevels, ManagementScope scope)
        {
            try
            {
                using (ManagementClass badgeClass = new ManagementClass(scope, new ManagementPath("Lnl_Badge"), null))
                {
                    //Get an input parameters object for AssignAccessLevel method
                    using (ManagementBaseObject inParams = badgeClass.GetMethodParameters("AssignAccessLevel"))
                    {
                        inParams.Properties["LevelIn"].Value = accessLevels;
                        // There is no output for this one
                        badge.InvokeMethod("AssignAccessLevel", inParams, null);
                    }
                }
            }
            catch(Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"Error in AssignAccessLevels {ex?.ToString()}", ex);
                throw ex;
            }
        }

        private void RemoveAccessLevelAssignments(ManagementObject badge, int[] accessLevels, ManagementScope scope)
        {
            try
            {
                int badgeKey = (int)badge["BadgeKey"];

                string pathTemplate = scope.Path + ":Lnl_AccessLevelAssignment.AccessLevelID={0},BadgeKey={1}";

                foreach (var al in accessLevels)
                {
                    try
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AccessLevelID : {al} BadgeKey:{badgeKey}");
                        using (ManagementObject obj = new ManagementObject())
                        {
                            ManagementPath path = new ManagementPath(string.Format(pathTemplate, al, badgeKey));
                            obj.Path = path;
                            obj.Delete();
                            _idmsLogger.WriteLog(LogLevelType.Info, $"Deleted AccessLevelID : {al} BadgeKey:{badgeKey}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AccessLevelID : {al} BadgeKey:{badgeKey} Not found {ex.Message} ",ex);
                    }
                }
            }
            catch(Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"Error in RemoveAccessLevelAssignments.{ex?.ToString()} ", ex);
                throw ex;
            }
        }

        #endregion

        #region For Tool
        public List<SMSUserInfo> GetUsers(string filter)
        {
            var lstSMSUserInfo = new List<SMSUserInfo>();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var smsUserInfo = new SMSUserInfo();
                ManagementScope scope = new ManagementScope(this.Configuration(EndpointKey.RootConnectionString));
                scope.Connect();

                if (scope.IsConnected)
                {
                    using (ManagementObject inst = GetCardholderOnSsNo(filter, scope))
                    {
                        if (inst != null)
                        {
                            smsUserInfo = new SMSUserInfo();

                            smsUserInfo.EndPointSystemId = this.Endpoint.Id;

                            smsUserInfo.UserId = CheckNull.AgainstInt(inst["ID"]);
                            smsUserInfo.FIRSTNAME = CheckNull.AgainstString(inst["FIRSTNAME"]);
                            smsUserInfo.LASTNAME = CheckNull.AgainstString(inst["LASTNAME"]);
                            smsUserInfo.MIDNAME = CheckNull.AgainstString(inst["MIDNAME"]);
                            smsUserInfo.SSNO = CheckNull.AgainstString(inst["SSNO"]);

                            smsUserInfo.Addr1 = CheckNull.AgainstString(inst["ADDR1"]);
                            smsUserInfo.City = CheckNull.AgainstString(inst["CITY"]);
                            smsUserInfo.Zip = CheckNull.AgainstString(inst["ZIP"]);
                            smsUserInfo.EMAIL = CheckNull.AgainstString(inst["EMAIL"]);
                            smsUserInfo.OPHONE = CheckNull.AgainstString(inst["OPHONE"]);
                            smsUserInfo.DEPT = Convert.ToInt32(inst["DEPT"]);
                            smsUserInfo.Extension = CheckNull.AgainstString(inst["EXT"]);
                        }
                    }
                }
            }
            else
            {
                var lstUsers = GetUsers();
                if (lstUsers != null)
                    lstSMSUserInfo = lstUsers.ToList();
            }
            return lstSMSUserInfo;
        }
        #endregion
    }
}

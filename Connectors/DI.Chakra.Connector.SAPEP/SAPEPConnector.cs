using DI.Chakra.Connector.SAPEP;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SapCEWebservice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;

namespace DI.Chakra.Connector
{
    public enum objectClass
    {
        user, group, role
    }
    public enum returnType
    {
        UniqueName //, ObjectGUID
    }

    public class SAPEPConnector : ConnectorBase
    {
        #region Members
        private ZCustomUserManagementSrv_In m_serviceProxy = null;
        #endregion
        List<AccountReconcilation> accountReconcilations;

        public class SAPEPUserGroups
        {
            public int SAPEPUserInfo_Id { get; set; }
            public int SAPEPGroupInfo_Id { get; set; }
            public int EndpointId { get; set; }
        }

        public class SAPEPUserRoles
        {
            public int SAPEPUserInfo_Id { get; set; }
            public int SAPEPRoleInfo_Id { get; set; }
            public int EndpointId { get; set; }
        }

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicRoleUsers = new Dictionary<string, ArrayList>();
        private long discoveredGroupsCount = 0;
        private long discoveredRolesCount = 0;

        enum SAPEPUserControl : uint
        {
            SCRIPT = 0x0001,
            ACCOUNTDISABLE = 0x0002,
            HOMEDIR_REQUIRED = 0x0008,
            LOCKOUT = 0x0010,
            PASSWD_NOTREQD = 0x0020,
            PASSWD_CANT_CHANGE = 0x0040,
            ENCRYPTED_TEXT_PWD_ALLOWED = 0x0080,
            TEMP_DUPLICATE_ACCOUNT = 0x0100,
            NORMAL_ACCOUNT = 0x0200,
            INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
            WORKSTATION_TRUST_ACCOUNT = 0x1000,
            SERVER_TRUST_ACCOUNT = 0x2000,
            DONT_EXPIRE_PASSWORD = 0x10000,
            MNS_LOGON_ACCOUNT = 0x20000,
            SMARTCARD_REQUIRED = 0x40000,
            TRUSTED_FOR_DELEGATION = 0x80000,
            NOT_DELEGATED = 0x100000,
            USE_DES_KEY_ONLY = 0x200000,
            DONT_REQ_PREAUTH = 0x400000,
            PASSWORD_EXPIRED = 0x800000,
            TRUSTED_TO_AUTH_FOR_DELEGATION = 0x1000000
        }

        private DataTable dtWhole = new DataTable();
        private static object consoleGate = new Object();
        private static Queue<DirectoryEntry> workQueue = new Queue<DirectoryEntry>();
        private static bool ProcessItems = true;

        public List<SAPEPUserInfoHistory> Historylist { get; set; }
        private DirectoryEntry m_root = null;
        private DirectorySearcher m_sapepSearch = null;
        // public SAPEPFacade sapepFacade { get; set; }
        //public UserFacade userFacade { get; set; }

        int currentTry = 0;
        private int retry = 0;
        private EndpointConfiguration m_sapepConfig = null;
        List<SearchResultCollection> srList = new List<SearchResultCollection>();
        readonly string MasterConfigHelperServiceUser;
        public int RetryCount
        {
            get
            {
                return 2;
            }
        }

        #region Constructors

        public SAPEPConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.SAPEPConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }

        #endregion

        public EndpointConfiguration SAPEPConfiguration
        {
            get
            {
                return m_sapepConfig;
            }

            set
            {
                this.m_sapepConfig = value;
            }
        }
        private string Configuration(EndpointKey Key)
        {
            var config = SAPEPConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override bool Connect()
        {
            if (!(this.Endpoint != null && SAPEPConfiguration != null && !string.IsNullOrEmpty(Configuration(EndpointKey.WSDLURL))))
                return false;

            try
            {
                m_serviceProxy = m_serviceProxy == null ? new ZCustomUserManagementSrv_In() : m_serviceProxy;

                string pwd = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                m_serviceProxy.Credentials = new System.Net.NetworkCredential(Configuration(EndpointKey.UserName), pwd);
                m_serviceProxy.Url = Configuration(EndpointKey.WSDLURL);

                if (isConnectionOK())
                {
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public override void Disconnect()
        {
            m_serviceProxy.Url = string.Empty;
            m_serviceProxy.Credentials = null;
        }
        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverUsers Started");
            int reTryCount = 0;
            int maxReTry = 3;
            bool canTry = true;
            List<string> startChars = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,(,.".Split(',').ToList();
            List<SAPEPUserInfo> userlist = null;
            bool isAnyError = false;
            foreach (string chr in startChars)
            {
                canTry = true;
                reTryCount = 0;
                while (canTry)
                {
                    reTryCount++;
                    try
                    {
                        userlist = this.getUsers(chr + "*");
                        canTry = false;
                    }
                    catch (Exception ee)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, $"Exception while calling SAPEPConnector.getUsers for the character {chr}.Retry- {reTryCount}. The Exception is : {ee.Message}", ee);
                        isAnyError = true;
                    }

                    if (userlist == null && reTryCount < maxReTry)
                    {
                        canTry = true;
                    }
                    else
                    {
                        canTry = false;
                    }
                }

                if (userlist != null)
                    SAPEPUserInfo.SaveUsers(userlist, this.Endpoint.Id, MasterConfigHelperServiceUser, (long)this.Endpoint.EndpointSystemTypeId);
                userlist = null;
            }
            //2781 issue fix
            if (isAnyError == false)
            {
                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);

                var deletedUserlist = SAPEPUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);

                WriteDiscoveryLog(LogLevelType.Info, $"SAPEP User Delete count:{deletedUserlist?.Count} Process start at:{this.Process?.ProcessStartAt}");

                foreach (var item in deletedUserlist)
                {
                    try
                    {
                        var usrInfo = SAPEPUserInfo.GetById(item.Id, true);
                        if (usrInfo != null)
                        {
                            if (usrInfo.SAPEPGroups != null && usrInfo.SAPEPGroups.Count > 0)
                            {
                                SAPEPUserInfo.ExecuteQueryWithCommandTimeout(string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0} and SAPEPUserInfo_Id={1} and SAPEPGroupInfo_Id IN({2})", this.Endpoint.Id, usrInfo.Id, string.Join(",", usrInfo.SAPEPGroups.Select(i => i.Id))), 0);
                            }
                            if (usrInfo.SAPEPRoles != null && usrInfo.SAPEPRoles.Count > 0)
                            {
                                SAPEPUserInfo.ExecuteQueryWithCommandTimeout(string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0} and SAPEPUserInfo_Id={1} and SAPEPRoleInfo_Id IN({2})", this.Endpoint.Id, usrInfo.Id, string.Join(",", usrInfo.SAPEPRoles.Select(i => i.Id))), 0);
                            }
                        }
                        usrInfo = null;
                        //SAPEPUserInfo.DeleteSAPEPUserGroupMap(item);
                        //SAPEPUserInfo.DeleteSAPEPUserRoleMap(item);
                        item.Delete();
                        WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: User {0} has been REMOVED without any request made in the system", item.DisplayName), DiscoveryObjectType.User);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception occured while deleting SAP EP User {0} without any request made in the system", item.DisplayName), DiscoveryObjectType.User, ex);
                    }
                }
            }
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverUsers Ended", DiscoveryObjectType.User);
        }
        public override void GetData()
        {

            try
            {

                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_Group) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_Role) != null;


                if (IsUserSelected)
                {
                    DiscoverUsers();
                }
                dicUsers = SAPEPUserInfo.GetByEndPointSystemId(this.Endpoint.Id).ToDictionary(c => c.Id, c => c.UserName);

                if (IsGroupSelected)
                {
                    this.DiscoverGroups();
                }
                dicGroups = SAPEPGroupInfo.GetByEndPointId(this.Endpoint.Id).ToDictionary(c => c.Id, c => c.GroupName);

                accountReconcilations = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.DebugStep, string.Format("Total Account Reconcilations {0} ", accountReconcilations.Count));

                dicUsers = SAPEPUserInfo.GetByEndPointSystemId(this.Endpoint.Id).ToDictionary(c => c.Id, c => c.UserName);
                if (IsUserSelected || IsGroupSelected)
                {
                    SaveUserGroups();
                    //Update recon table
                    this.UpdateGroupToEntitlementRecon();
                }

                if (IsRoleSelected)
                {
                    this.DiscoverRoles();
                }


                dicRoles = SAPEPRoleInfo.GetByEndPointId(this.Endpoint.Id).ToDictionary(c => c.Id, c => c.RoleName);

                if (IsUserSelected || IsRoleSelected)
                {
                    SaveUserRoles();

                    //Update recon table
                    this.UpdateRoleToEntitlementRecon();
                }

                if (discoveredGroupsCount != 0 && discoveredRolesCount != 0)
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);

                //SAPEPUserInfo.MapWithMasterUser(EndPoint, SAPEPConfiguration.MapAll != null ? SAPEPConfiguration.MapAll : false);
                SAPEPUserInfo.MapWithMasterUserForReconTable(Endpoint, SAPEPConfiguration.MapAll != null ? SAPEPConfiguration.MapAll : false);
                //ADSUserInfo.PopulateMasterUserFromAD(EndPoint);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "SAP EP Discovery GetData failed", ex);
                throw;
            }
            finally
            {
                discoveredGroupsCount = 0;
                discoveredRolesCount = 0;

                dicUsers?.Clear();
                dicUsers = null;

                dicGroups?.Clear();
                dicGroups = null;

                dicGroupUsers?.Clear();
                dicGroupUsers = null;

                dicRoles?.Clear();
                dicRoles = null;

                dicRoleUsers?.Clear();
                dicRoleUsers = null;

                GC.Collect();
            }
        }
        private void DiscoverRoles()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverRoles Started", DiscoveryObjectType.Role);
            try
            {

                List<ZZPrincipalInformation> listWholePrinciple = new List<ZZPrincipalInformation>();

                List<SAPEPUserRole> listWholeUserRole = new List<SAPEPUserRole>();

                RoleInfoOrchestrator roleOrch = new RoleInfoOrchestrator(_idmsLogger);

                roleOrch.EndpointSystemId = (int)this.Endpoint.Id;

                List<SAPEPUserInfo> sapepUsers = SAPEPUserInfo.GetByEndPointSystemId(Endpoint.Id);


                foreach (SAPEPUserInfo user in sapepUsers)
                {
                    try
                    {
                        zGetAllRolesAssigned zGetAllRolesAssigned1 = new zGetAllRolesAssigned();
                        zGetAllRolesAssigned1.strUserID = user.UserName;
                        zGetAllRolesAssignedResponse zRolesAssignedResponse = m_serviceProxy.zGetAllRolesAssigned(zGetAllRolesAssigned1);

                        ZZPrincipalInformation[] zprincipalList = zRolesAssignedResponse.response.userInfo;
                        if (zprincipalList != null)
                        {
                            if (listWholePrinciple.Count <= 0)
                                listWholePrinciple = zprincipalList.ToList();
                            else
                                listWholePrinciple = listWholePrinciple.Union(zprincipalList.ToList()).ToList();

                            ArrayList strRoles = new ArrayList();
                            foreach (ZZPrincipalInformation zzgrps in zprincipalList)
                            {
                                strRoles.Add(zzgrps.strPrincipalName);
                            }

                            dicRoleUsers.Add(user.Id.ToString(), strRoles);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, "Exception occured while SAPEP GetRoles() ", DiscoveryObjectType.Role, ex);
                    }
                }

                List<SAPEPRoleInfo> listWholeRoles = roleOrch.GetRoles(listWholePrinciple.ToArray());

                if (listWholeRoles == null || listWholeRoles.Count == 0)
                {
                    WriteDiscoveryLog(LogLevelType.Warning, "Issue encountered when discovering roles");
                    return;
                }

                discoveredRolesCount = listWholeRoles.Count;

                WriteDiscoveryLog(LogLevelType.Info, $"Discovered roles count: {discoveredRolesCount}");

                foreach (var item in listWholeRoles)
                {
                    var existingRole = SAPEPRoleInfo.GetRoleByRoleId(this.Endpoint.Id, item.RoleId);
                    if (existingRole != null)
                    {
                        item.Id = existingRole.Id;
                        item.CreatedBy = existingRole.CreatedBy;
                        item.CreatedDate = existingRole.CreatedDate;
                        item.IsXLUploaded = existingRole.IsXLUploaded;
                        item.LastModifiedBy = "System";
                        item.LastModifiedDate = DateTime.Now;
                        item.Save("System");
                    }
                    else
                    {
                        item.CanRequest = true;
                        item.CreatedBy = "System";
                        item.CreatedDate = DateTime.Now;
                        item.Save("System");
                    }
                }

                List<SAPEPRoleInfo> listRoles = SAPEPRoleInfo.GetDeletedRoles(this.Endpoint.Id, this.Process);

                foreach (var item in listRoles)
                {
                    try
                    {
                        SAPEPUserInfo.DeleteSAPEPUserRoleMapByRoleId(this.Endpoint.Id, item.Id);
                        if (item.IsXLUploaded == false)
                            item.Delete();
                        WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: Role {0} has been REMOVED without any request made in the system", DiscoveryObjectType.Role, item.UniqueRoleName));
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception occured while deleting SAP EP Role {0} without any request made in the system", DiscoveryObjectType.Role, item.UniqueRoleName), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception occured while SAPEP DiscoveringRoles() ", DiscoveryObjectType.Role, ex);
                throw;
            }
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverRoles Ended", DiscoveryObjectType.Role);
        }
        private void DiscoverGroups()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverGroups Started", DiscoveryObjectType.Group);
            List<ZZPrincipalInformation> listWholePrinciple = new List<ZZPrincipalInformation>();
            List<SAPEPUserGroups> listWholeUserGroup = new List<SAPEPUserGroups>();
            List<SAPEPUserInfo> sapepUsers = null;
            List<SAPEPGroupInfo> listWholeGroups = null;
            List<SAPEPGroupInfo> listGroups = null;
            try
            {
                GroupInfoOrchestrator groupOrch = new GroupInfoOrchestrator(_idmsLogger);
                groupOrch.EndpointSystemId = (int)this.Endpoint.Id;

                sapepUsers = SAPEPUserInfo.GetByEndPointSystemId(Endpoint.Id);

                foreach (SAPEPUserInfo user in sapepUsers)
                {
                    try
                    {
                        zGetAllGroupsAssigned zGetAllGroupsAssigned1 = new zGetAllGroupsAssigned();
                        zGetAllGroupsAssigned1.strUserID = user.UserName;
                        zGetAllGroupsAssignedResponse zGetAllGroupsAssignedResponse1 = m_serviceProxy.zGetAllGroupsAssigned(zGetAllGroupsAssigned1);
                        ZZPrincipalInformation[] zprincipalList = zGetAllGroupsAssignedResponse1.response.userInfo;
                        if (zprincipalList != null)
                        {
                            if (listWholePrinciple.Count <= 0)
                                listWholePrinciple = zprincipalList.ToList();
                            else
                                listWholePrinciple = listWholePrinciple.Union(zprincipalList.ToList()).ToList();

                            ArrayList strGroups = new ArrayList();
                            foreach (ZZPrincipalInformation zzgrps in zprincipalList)
                            {
                                strGroups.Add(zzgrps.strPrincipalName);
                            }

                            dicGroupUsers.Add(user.Id.ToString(), strGroups);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, "Exception occured while SAPEP GetGroups() ", DiscoveryObjectType.Group, ex);
                    }
                }

                listWholeGroups = groupOrch.GetGroups(listWholePrinciple.ToArray());

                if (listWholeGroups == null || listWholeGroups.Count == 0)
                {
                    WriteDiscoveryLog(LogLevelType.Warning, "Issue encountered when discovering groups");
                    return;
                }

                discoveredGroupsCount = listWholeGroups.Count;

                WriteDiscoveryLog(LogLevelType.Info, $"Discovered groups count: {discoveredGroupsCount}");

                foreach (var item in listWholeGroups)
                {
                    var existingGroup = SAPEPGroupInfo.GetGroupByGroupUniqueId(this.Endpoint.Id, item.GroupId);
                    if (existingGroup != null)
                    {
                        item.Id = existingGroup.Id;
                        item.CreatedBy = existingGroup.CreatedBy;
                        item.CreatedDate = existingGroup.CreatedDate;
                        item.IsXLUploaded = existingGroup.IsXLUploaded;
                        item.LastModifiedBy = "System";
                        item.LastModifiedDate = DateTime.Now;
                        item.Save("System");
                    }
                    else
                    {
                        item.CreatedBy = "System";
                        item.CreatedDate = DateTime.Now;
                        item.Save("System");
                    }
                }

                listGroups = SAPEPGroupInfo.GetDeletedGroups((int)this.Endpoint.Id, this.Process);
                foreach (var item in listGroups)
                {
                    try
                    {
                        SAPEPUserInfo.DeleteSAPEPUserGroupMapByGroupId(this.Endpoint.Id, item.Id);
                        if(item.IsXLUploaded == false)
                            item.Delete();

                        WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: Group {0} has been REMOVED without any request made in the system", DiscoveryObjectType.Group, item.GroupUniqueName));
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception occured while deleting SAP EP Group {0} without any request made in the system", DiscoveryObjectType.Group, item.GroupUniqueName), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception Occured in SAPEP DiscoverGroups", DiscoveryObjectType.Group, ex);
                throw;
            }
            finally
            {
                listGroups?.Clear();
                listGroups = null;

                listWholeGroups?.Clear();
                listWholeGroups = null;

                sapepUsers?.Clear();
                sapepUsers = null;

                listWholeUserGroup?.Clear();
                listWholeUserGroup = null;

                listWholePrinciple?.Clear();
                listWholePrinciple = null;

            }
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP DiscoverGroups Ended", DiscoveryObjectType.Group);
        }
        private List<SAPEPUserInfo> getUsers(string searchString)
        {
            try
            {
                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
                m_serviceProxy = m_serviceProxy == null ? new ZCustomUserManagementSrv_In() : m_serviceProxy;

                string pwd = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                m_serviceProxy.Credentials = new System.Net.NetworkCredential(Configuration(EndpointKey.UserName), pwd);

                zGetAllUsers zGetAllUsers1 = new zGetAllUsers();
                zGetAllUsers1.strSearchLName = searchString;
                m_serviceProxy.Timeout = Int32.MaxValue;

                zGetAllUsersResponse zGetAllUsersReponse = m_serviceProxy.zGetAllUsers(zGetAllUsers1);
                ZZUserInformation[] zuserList = zGetAllUsersReponse.response.userInfo;

                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                orchestrator.EndPoint = this.Endpoint;
                return orchestrator.GetUserInfo(zuserList);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, ex.ToString() + "Error occured at SAPEP getusers(), Inner Exception:" + ex.InnerException?.ToString(), DiscoveryObjectType.User, ex);
                throw ex;
            }
        }
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            if (m_serviceProxy == null)
                this.InitializedProxy((int)this.Endpoint.Id);

            bool result = false;
            zResetUserPassword zResetUserPassword1 = new zResetUserPassword();
            zResetUserPassword1.strUSerID = userName;
            zResetUserPassword1.strNewPassword = newPassword;

            zResetUserPasswordResponse zResponse = m_serviceProxy.zResetUserPassword(zResetUserPassword1);
            result = zResponse.response.isPasswordReset;
            if (result)
            {

                SAPEPUserInfo.UpdatePasswordresetcount(this.Endpoint.Id, userName, MasterConfigHelperServiceUser);
            }
            else
            {
                return result;
            }
            return result;
        }
        private bool isConnectionOK()
        {
            bool isConnOK = false;
            if (this.m_serviceProxy != null && !string.IsNullOrEmpty(this.m_serviceProxy.Url) && this.m_serviceProxy.Credentials != null)
            {
                try
                {
                    isConnOK = true;
                }
                catch
                {
                }
            }
            else
            {
                isConnOK = false;
            }
            return isConnOK;
        }
        private void InitializedProxy(int endPointID)
        {
            try
            {
                if (!(Endpoint != null && Endpoint.Id == endPointID))
                {
                    this.Endpoint = DomainModel.EndPoint.GetById(endPointID);
                }
                zZUpdateUser updateuser = new zZUpdateUser();

                if (this.Endpoint != null && SAPEPConfiguration != null && !string.IsNullOrEmpty(Configuration(EndpointKey.WSDLURL)))
                {
                    m_serviceProxy = m_serviceProxy == null ? new ZCustomUserManagementSrv_In() : m_serviceProxy;
                    m_serviceProxy.Url = Configuration(EndpointKey.WSDLURL);
                    m_serviceProxy.Credentials = new System.Net.NetworkCredential(Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                    m_serviceProxy.Timeout = Int32.MaxValue;
                    m_serviceProxy.Proxy = null;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, "Exception at SAPEP InitilizeProxy ", ex);
                WriteDiscoveryLog(LogLevelType.Exception, "Exception at SAPEP InitilizeProxy ", ex);
                throw;
            }
        }
        private void SaveUserGroups()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Save UserGroups Started");
            SAPEPUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.Endpoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Save UserGroups Ended");
        }
        private void SaveUserRoles()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Save UserRoles Started", DiscoveryObjectType.UserRole);

            SAPEPUserInfo.SaveUserRolesMap(dicUsers, dicRoles, dicRoleUsers, this.Endpoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Save UserRoles Ended", DiscoveryObjectType.UserRole);
        }
        private void UpdateGroupToEntitlementRecon()
        {
            Dictionary<long, List<string>> allUserGroups = null;
            List <SAPEPGroupInfo> sapEPGroups = new List<SAPEPGroupInfo>();
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Update Group Entiltment Recon Started");
            try
            {
                AccountReconcilation acctRecon;
                List<string> userGroups;
                EntitlementReconcilation entRecon;
                //long epUserInfoId = 0;
                sapEPGroups = SAPEPGroupInfo.GetByEndPointId(this.Endpoint.Id);
                allUserGroups = SAPEPUserInfo.GetUserGroupsByEndpointSystemId(Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.DebugStep, $"Reading UserGroups completed. {allUserGroups.Count}");
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    allUserGroups.TryGetValue(item.Key, out userGroups);

                    if (userGroups != null && userGroups.Count > 0)
                    {
                        acctRecon = accountReconcilations.FirstOrDefault(p => p.AccountName.Equals(item.Value, StringComparison.InvariantCultureIgnoreCase) && p.EndpointSystemId == Endpoint.Id);
                        if (acctRecon != null)
                        {
                            foreach (string entitlement in userGroups)
                            {
                                var grp = sapEPGroups?.FirstOrDefault(i => (!string.IsNullOrWhiteSpace(i.GroupName) && i.GroupName.Equals(entitlement, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrWhiteSpace(i.GroupUniqueName) && i.GroupUniqueName.Equals(entitlement, StringComparison.InvariantCultureIgnoreCase))
                                );
                                entRecon = EntitlementReconcilation.GetByUser(acctRecon.EndpointUserId, Endpoint.Id, entitlement, EntitlementType.Group);

                                if (entRecon == null)
                                {
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group);
                                    entRecon.Entitlement = entitlement;
                                    entRecon.ExceptionType = ExceptionTypes.Added;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                                else
                                {
                                    if (entRecon.IsDeleted)
                                    {
                                        entRecon.LastAttestedOn = null;
                                        entRecon.IsDeleted = false;
                                        entRecon.DeletedDate = null;
                                        entRecon.ExceptionType = ExceptionTypes.Added;
                                    }
                                }
                                entRecon.EntitlementDescription = grp?.GroupDescription;
                                entRecon.AccountName = acctRecon.AccountName;
                                entRecon.UserId = acctRecon.UserId;
                                entRecon.Save(MasterConfigHelperServiceUser);
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating SAPEP Group Entitlement reconciliation.", ex);
            }
            finally
            {
                allUserGroups = null;
                sapEPGroups = null;
            }
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Update Group Entiltment Recon Ended");
        }
        private void UpdateRoleToEntitlementRecon()
        {
            WriteDiscoveryLog(LogLevelType.Info, "SAP EP Update Role Entiltment Recon Started");
            var sapRoles = new List<SAPEPRoleInfo>();
            try
            {
                AccountReconcilation acctRecon;
                List<string> userRoles;
                EntitlementReconcilation entRecon;
                sapRoles = SAPEPRoleInfo.GetByEndPointId(this.Endpoint.Id);
                // long epUserInfoId = 0;
                Dictionary<long, List<string>> allUserRoles = SAPEPUserInfo.GetUserRolesByEndpointSystemId(Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, $"UpdateRoleToEntitlementRecon started for the user. Count: {allUserRoles.Count}");
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    allUserRoles.TryGetValue(item.Key, out userRoles);
                    if (userRoles != null && userRoles.Count > 0)
                    {
                        acctRecon = accountReconcilations.FirstOrDefault(p => p.AccountName.Equals(item.Value, StringComparison.InvariantCultureIgnoreCase)
                                     && p.EndpointSystemId == Endpoint.Id);
                        if (acctRecon != null)
                        {
                            foreach (string entitlement in userRoles)
                            {
                                var role = sapRoles?.FirstOrDefault(i => (!string.IsNullOrWhiteSpace(i.RoleName) && i.RoleName.Equals(entitlement, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrWhiteSpace(i.UniqueRoleName) && i.UniqueRoleName.Equals(entitlement, StringComparison.InvariantCultureIgnoreCase))
                                );
                                entRecon = EntitlementReconcilation.GetByUser(acctRecon.EndpointUserId, Endpoint.Id, entitlement, EntitlementType.Role);

                                if (entRecon == null)
                                {
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role);
                                    entRecon.Entitlement = entitlement;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.ExceptionType = ExceptionTypes.Added;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                                else
                                {
                                    if (entRecon.IsDeleted)
                                    {
                                        entRecon.LastAttestedOn = null;
                                        entRecon.IsDeleted = false;
                                        entRecon.DeletedDate = null;
                                        entRecon.ExceptionType = ExceptionTypes.Added;
                                    }
                                }
                                entRecon.ValidToDate = acctRecon.ValidToDate;
                                entRecon.EntitlementDescription = role?.RoleDescription;
                                entRecon.AccountName = acctRecon.AccountName;
                                entRecon.UserId = acctRecon.UserId;
                                entRecon.Save(MasterConfigHelperServiceUser);
                            }
                        }
                    }
                }
                allUserRoles = null;
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating SAPEP Roles Entitlement reconciliation.", ex);
            }
            finally
            {
                sapRoles?.Clear();
                sapRoles = null;
            }
            WriteDiscoveryLog(LogLevelType.Info, "UpdateRoleToEntitlementRecon Ended");
        }

        #region provisioning
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            try
            {


                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

                if (sapEPUsr == null)
                    return false;

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP CreateAccount Started for user - {sapEPUsr.UserName}");

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

                ZZUserInformation cnt_Usr = new ZZUserInformation();

                cnt_Usr.Cellphone = sapEPUsr.Cellphone;
                cnt_Usr.City = sapEPUsr.City;
                cnt_Usr.Company = sapEPUsr.Company;
                cnt_Usr.Country = sapEPUsr.Country;
                cnt_Usr.Department = sapEPUsr.Department;
                cnt_Usr.Email = sapEPUsr.Email;
                cnt_Usr.Fax = sapEPUsr.Fax;
                cnt_Usr.FirstName = sapEPUsr.FirstName;
                cnt_Usr.JobTitle = sapEPUsr.JobTitle;
                cnt_Usr.LastName = sapEPUsr.LastName;
                cnt_Usr.Salutation = sapEPUsr.Salutation;
                cnt_Usr.State = sapEPUsr.State;
                cnt_Usr.Street = sapEPUsr.Street;
                cnt_Usr.Telephone = sapEPUsr.Telephone;
                cnt_Usr.Title = sapEPUsr.Title;
                cnt_Usr.UniqueName = sapEPUsr.UserName;
                cnt_Usr.Zip = sapEPUsr.Zip;
                cnt_Usr.Language = sapEPUsr.Language;
                cnt_Usr.Password = sapEPUsr.Password;

                if (sapEPUsr.StartDate != null)
                {
                    cnt_Usr.StartDate = string.Format("{0}/{1}/{2}", sapEPUsr.StartDate.Value.Year.ToString(), sapEPUsr.StartDate.Value.Month.ToString(), sapEPUsr.StartDate.Value.Day.ToString());
                }

                if (sapEPUsr.EndDate != null)
                {
                    cnt_Usr.EndDate = string.Format("{0}/{1}/{2}", sapEPUsr.EndDate.Value.Year.ToString(), sapEPUsr.EndDate.Value.Month.ToString(), sapEPUsr.EndDate.Value.Day.ToString());
                }

                zZCreateNewUser zZCreateNewUser1 = new zZCreateNewUser();
                zZCreateNewUser1.userInfo = cnt_Usr;
                zZCreateNewUserResponse zCreateUserReponse = m_serviceProxy.zZCreateNewUser(zZCreateNewUser1);
                return zCreateUserReponse.response.isUSerCreated;

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP CreateAccount Ended for user - {sapEPUsr.UserName}");
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, "Exception at SAPEP CreateAccount", ex);
                return false;
            }
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

                if (sapEPUsr == null)
                    return false;

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP UpdateAccount Started for user - {sapEPUsr.UserName}");

                this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

                ZZUserInformation cnt_Usr = new ZZUserInformation();

                cnt_Usr.UserName = sapEPUsr.UserName;
                cnt_Usr.FirstName = sapEPUsr.FirstName;
                cnt_Usr.LastName = sapEPUsr.LastName;
                cnt_Usr.JobTitle = sapEPUsr.JobTitle;
                cnt_Usr.Department = sapEPUsr.Department;
                cnt_Usr.Email = sapEPUsr.Email;

                zZUpdateUser zZUpdateUser1 = new zZUpdateUser();
                zZUpdateUser1.strUserID = cnt_Usr.UserName;
                zZUpdateUser1.strDept = cnt_Usr.Department;
                zZUpdateUser1.strFname = cnt_Usr.FirstName;
                zZUpdateUser1.strJobTitle = cnt_Usr.JobTitle;
                zZUpdateUser1.strEmail = cnt_Usr.Email;
                zZUpdateUser1.strLname = cnt_Usr.LastName;

                zZUpdateUserResponse zUpdateResponse = m_serviceProxy.zZUpdateUser(zZUpdateUser1);

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP UpdateAccount Ended for user - {sapEPUsr.UserName}");

                return zUpdateResponse.response.isUSerUpdatedSpecified;


            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, "Exception at SAPEP UpdateAccount", ex);
                return false;
            }
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

                if (sapEPUsr == null)
                    return false;

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP DeleteAccount Started for user - {sapEPUsr.UserName}");

                this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

                zDeleteUser zDeleteUser1 = new zDeleteUser();
                zDeleteUser1.strUserID = sapEPUsr.UserName;
                zDeleteUserResponse zDeleteResponse = m_serviceProxy.zDeleteUser(zDeleteUser1);
                bool isDeleted = zDeleteResponse.response.isUSerDeleted;

                if (isDeleted)
                {
                    base.DeleteAccount(_userInfo);
                }

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP DeleteAccount Ended for user - {sapEPUsr.UserName}");
                return isDeleted;

            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, "Exception at SAPEP DeleteAccount", ex);
                return false;
            }
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

                bool isAdded = false;

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP AddEntitlement Started for user - {sapEPUsr.UserName}");

                var recon = AccountReconcilation.GetByEndpointUserId(_userInfo.Id, this.Endpoint.Id);
                if (recon == null || (recon.IsLocked || recon.IsDeleted))
                {
                    if (this.CurrentRequest != null && this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", sapEPUsr.AccountName), null);
                        return false;
                    }
                }

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"Before assigning {_entitlementType} isAdded {isAdded}");

                if (_entitlementType == EntitlementType.Role)
                {
                    sapEPUsr.Roles = sapEPUsr.Roles?.Distinct().ToList();
                    if (sapEPUsr.Roles != null && sapEPUsr.Roles.Count() > 0)
                        isAdded = this.AddUserToRole(sapEPUsr);
                }
                else if (_entitlementType == EntitlementType.Group)
                {
                    sapEPUsr.Groups = sapEPUsr.Groups?.Distinct().ToList();
                    if (sapEPUsr.Groups != null && sapEPUsr.Groups.Count() > 0)
                        isAdded = this.AssignGroupsToUser(sapEPUsr);
                }

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"After assigning {_entitlementType} isAdded {isAdded}");

                if (isAdded)
                    base.AddEntitlement(_userInfo, _entitlementType);

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP AddEntitlement Ended for user - {sapEPUsr.UserName}");

                return isAdded;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, "Exception at SAPEP AddEntitlement", ex);
                return false;
            }
        }
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;
                bool isRemoved = true;

                this._idmsLogger.WriteLog(LogLevelType.Info, $"SAP EP RemoveEntitlement Started for user - {sapEPUsr.UserName}");
                this._idmsLogger.WriteLog(LogLevelType.Info, $"SAP EP RemoveEntitlement Roles - {string.Join(",", sapEPUsr.Roles)}");
                this._idmsLogger.WriteLog(LogLevelType.Info, $"SAP EP RemoveEntitlement Groups - {string.Join(",", sapEPUsr.Groups)}");

                var recon = AccountReconcilation.GetByEndpointUserId(_userInfo.Id, this.Endpoint.Id);
                if (recon == null || (recon.IsLocked || recon.IsDeleted))
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", sapEPUsr.AccountName), null);
                        return false;
                    }
                }

                if (_entitlementType == EntitlementType.Role)
                {
                    if (sapEPUsr.Roles != null && sapEPUsr.Roles.Count() > 0)
                        isRemoved = this.RemoveUserFromRole(sapEPUsr);
                }
                else if (_entitlementType == EntitlementType.Group)
                {
                    if (sapEPUsr.Groups != null && sapEPUsr.Groups.Count() > 0)
                        isRemoved = this.RemoveGroupAssignmentOfUser(sapEPUsr);
                }

                if (isRemoved)
                    base.RemoveEntitlement(_userInfo, _entitlementType);

                this._idmsLogger.WriteLog(LogLevelType.Info, $"SAP EP RemoveEntitlement Ended for user - {sapEPUsr.UserName}, isRemoved: {isRemoved}");

                return isRemoved;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, "Exception at SAPEP RemoveEntitlement", ex);
                return false;
            }
        }
        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            try
            {   //Commented below code because EP/CE locking happend after HRMS Flins locked
                this._idmsLogger.WriteLog(LogLevelType.Info, $"Lock {this.Endpoint.DisplayName} User {_userInfo.AccountName}");
                return base.LockAccount(_userInfo);
                //SAPEPUserInfo sapepUserinfo = (SAPEPUserInfo)_userInfo;
                //this.InitializedProxy((int)sapepUserinfo.EndPointSystemId);
                //zLockUser LockUser = new zLockUser();
                //LockUser.uniqueId = sapepUserinfo.UniqueId;
                //zLockUserResponse LockUserResponse = m_serviceProxy.zLockUser(LockUser);

                //this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP RemoveEntitlement Started for user - {sapepUserinfo.UserName}");
                //if (LockUserResponse != null && LockUserResponse.response != null && LockUserResponse.response.isSuccess)
                //{
                //    this._idmsLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, string.Format("Successfully locked the  SAPEP User {0} ", _userInfo.AccountName));
                //    base.LockAccount(_userInfo);
                //    //return true;
                //}
                //else
                //{
                //    string resMsg = string.Empty;

                //    resMsg = resMsg + LockUserResponse == null ? "LockUserResponse is NULL" : string.Empty;
                //    resMsg = resMsg + ((LockUserResponse != null && LockUserResponse.response == null) ? "LockUserResponse.response is NULL" : string.Empty);

                //    if (LockUserResponse != null && LockUserResponse.response != null && !LockUserResponse.response.isSuccess)
                //    {
                //        resMsg = LockUserResponse.response.responseLog.statusText + " - " + LockUserResponse.response.responseLog.reasonText;
                //    }
                //    this._idmsLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, string.Format("Failed to Lock SAPEP User {0}, {1} ", _userInfo.AccountName, resMsg));

                //    // return false;
                //}

                //this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP RemoveEntitlement Ended for user - {sapepUserinfo.UserName}");
                //return LockUserResponse.response.isSuccess;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP LockAccount {0} ", _userInfo.AccountName), ex);
                return false;
            }
        }
        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                this._idmsLogger.WriteLog(LogLevelType.Info, $"SAP {this.Endpoint.DisplayName} UnLockAccount started for user - {_userInfo.AccountName}");
                return base.UnLockAccount(_userInfo);
                //Below code commented because HRMS Lins will take care of 
                //SAPEPUserInfo sapepUserinfo = (SAPEPUserInfo)_userInfo;
                //this.InitializedProxy((int)sapepUserinfo.EndPointSystemId);
                //zUnlockuser unlockUser = new zUnlockuser();
                //unlockUser.uniqueId = sapepUserinfo.UniqueId;

                //this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP UnLockAccount Started for user - {sapepUserinfo.UserName}");

                //zUnlockuserResponse UnLockUserResponse = m_serviceProxy.zUnlockuser(unlockUser);

                //if (UnLockUserResponse != null && UnLockUserResponse.response != null && UnLockUserResponse.response.isSuccess)
                //{
                //    this._idmsLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, string.Format("Successfully Unlocked SAPEP User {0} ", _userInfo.AccountName));

                //    //  return true;
                //}
                //else
                //{
                //    string resMsg = string.Empty;

                //    resMsg = resMsg + UnLockUserResponse == null ? "UnLockUserResponse is NULL" : string.Empty;
                //    resMsg = resMsg + ((UnLockUserResponse != null && UnLockUserResponse.response == null) ? "UnLockUserResponse.response is NULL" : string.Empty);

                //    if (UnLockUserResponse != null && UnLockUserResponse.response != null && !UnLockUserResponse.response.isSuccess)
                //    {
                //        resMsg = UnLockUserResponse.response.responseLog.statusText + " - " + UnLockUserResponse.response.responseLog.reasonText;
                //    }
                //    this._idmsLogger.WriteLog(this.CurrentRequest.Id, this.Endpoint.Id, LogLevelType.Info, string.Format("Failed to Unlock SAPEP User {0}, {1} ", _userInfo.AccountName, resMsg));

                //    // return false;
                //}

                //this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP UnLockAccount Ended for user - {sapepUserinfo.UserName}");
                //return UnLockUserResponse.response.isSuccess;

            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP UnLockAccount {0} ", _userInfo.AccountName), ex);
                return false;
            }
        }
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            try
            {
                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("Started SAPEP ChangeContainer {0} ", _userInfo.AccountName));


                SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

                if (sapEPUsr == null)
                    return false;

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP ChangeContainer Started for user - {sapEPUsr.UserName}");

                this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

                ZZUserInformation cnt_Usr = new ZZUserInformation();

                cnt_Usr.UserName = sapEPUsr.UserName;
                cnt_Usr.FirstName = sapEPUsr.FirstName;
                cnt_Usr.LastName = sapEPUsr.LastName;
                cnt_Usr.JobTitle = sapEPUsr.JobTitle;
                cnt_Usr.Department = sapEPUsr.Department;
                cnt_Usr.Email = sapEPUsr.Email;

                zZUpdateUser zZUpdateUser1 = new zZUpdateUser();
                zZUpdateUser1.strUserID = cnt_Usr.UserName;
                zZUpdateUser1.strDept = cnt_Usr.Department;
                zZUpdateUser1.strFname = cnt_Usr.FirstName;
                zZUpdateUser1.strJobTitle = cnt_Usr.JobTitle;
                zZUpdateUser1.strEmail = cnt_Usr.Email;
                zZUpdateUser1.strLname = cnt_Usr.LastName;

                zZUpdateUserResponse zUpdateResponse = m_serviceProxy.zZUpdateUser(zZUpdateUser1);

                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"SAP EP ChangeContainer Ended for user - {sapEPUsr.UserName}");

                return zUpdateResponse.response.isUSerUpdatedSpecified;

            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP ChangeContainer {0} ", _userInfo.AccountName), ex);
                return false;
            }
        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        private bool AddUserToRole(SAPEPUserInfo sapEPUsr)
        {
            try
            {
                if (sapEPUsr.Roles.Count == 0)
                    return true;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)this.Endpoint.Id);

                string[] roles = sapEPUsr.Roles.ToArray();

                zAddUserToRole zAddUserToRole1 = new zAddUserToRole();
                zAddUserToRole1.strUserID = sapEPUsr.UserName;
                zAddUserToRole1.roleList = roles;

                zAddUserToRoleResponse zResponse = m_serviceProxy.zAddUserToRole(zAddUserToRole1);

                return zResponse.response.isuserAssignedToRoles;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP AddUserToRole {0} ", sapEPUsr.AccountName), ex);
                return false;
            }
        }
        private bool RemoveUserFromRole(SAPEPUserInfo sapEPUsr)
        {
            try
            {
                if (sapEPUsr.Roles == null || sapEPUsr.Roles.Count == 0)
                    return true;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)this.Endpoint.Id);

                string[] roles = sapEPUsr.Roles.ToArray();

                zRemoveUserFromRole zRemoveUserFromRole1 = new zRemoveUserFromRole();
                zRemoveUserFromRole1.strUserID = sapEPUsr.UserName;
                zRemoveUserFromRole1.roleList = roles;
                zRemoveUserFromRoleResponse zResponse = m_serviceProxy.zRemoveUserFromRole(zRemoveUserFromRole1);

                _idmsLogger.WriteLog(LogLevelType.Info, $"AccountName: {sapEPUsr.AccountName} RemoveUserFromRole Status:{zResponse.response?.responseLog?.statusText} Reason:{zResponse.response?.responseLog?.reasonText} isuserremovedFromGrps:{zResponse?.response?.isuserremovedFromRole}", null);

                return zResponse.response.isuserremovedFromRole;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP RemoveUserFromRole {0} ", sapEPUsr.AccountName), ex);
                return false;
            }
        }
        private bool AssignGroupsToUser(SAPEPUserInfo sapEPUsr)
        {
            try
            {
                if (sapEPUsr.Groups == null || sapEPUsr.Groups.Count == 0)
                    return true;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)this.Endpoint.Id);

                //in Group, GroupName is used to pass in method so can't use GroupID as we are using in Role.
                string[] groups = sapEPUsr.Groups.ToArray();

                zAddUserToGroup zAddUserToGroup1 = new zAddUserToGroup();
                zAddUserToGroup1.strUserID = sapEPUsr.UserName;
                zAddUserToGroup1.groupList = groups;

                zAddUserToGroupResponse zResponse = m_serviceProxy.zAddUserToGroup(zAddUserToGroup1);

                if (!zResponse.response.isuserAssignedToGrps)
                {
                    this._idmsLogger.WriteLog(LogLevelType.Error,
                        string.Format("Error while assigning EP group to the Account - {0}. {1}", sapEPUsr.AccountName,
                        zResponse.response.responseLog != null ? zResponse.response.responseLog.reasonText : string.Empty),
                        null);
                }

                return zResponse.response.isuserAssignedToGrps;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP AssignGroupsTOUser {0} ", sapEPUsr.AccountName), ex);
                return false;
            }
        }
        private bool RemoveGroupAssignmentOfUser(SAPEPUserInfo sapEPUsr)
        {
            try
            {
                if (sapEPUsr.Groups.Count == 0)
                    return true;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)this.Endpoint.Id);

                //in Group GroupName is used to pass in method so can't user GroupID as we are using in Role.
                string[] groups = sapEPUsr.Groups.ToArray();

                zRemoveUserFromGroup zRemoveUserFromGroup1 = new zRemoveUserFromGroup();
                zRemoveUserFromGroup1.strUserID = sapEPUsr.UserName;
                zRemoveUserFromGroup1.groupList = groups;

                zRemoveUserFromGroupResponse zResponse = m_serviceProxy.zRemoveUserFromGroup(zRemoveUserFromGroup1);

                this._idmsLogger.WriteLog(LogLevelType.Info, $"AccountName: {sapEPUsr.AccountName} RemoveGroupAssignmentOfUser Status:{zResponse.response?.responseLog?.statusText} Reason:{zResponse.response?.responseLog?.reasonText} isuserremovedFromGrps:{zResponse?.response?.isuserremovedFromGrps}", null);

                return zResponse.response.isuserremovedFromGrps;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Exception at SAPEP RemoveGroupAssignmentOfUser {0} ", sapEPUsr.AccountName), ex);
                return false;
            }
        }
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            this.RemoveEntitlement(_userInfo, EntitlementType.Role);
            this.RemoveEntitlement(_userInfo, EntitlementType.Group);
            return true;
        }
        #endregion
    }
}

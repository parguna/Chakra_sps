using DI.Chakra.Connector.SAPIP;
using DI.Chakra.DataAccess;
using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;

using SapEpOldWebservice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Net;


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

    public class SAPEP701Connector : ConnectorBase
    {
        #region Members
        private ZZCustUsrMgmtWService m_serviceProxy = null;
        private SAPEPFacade SAPEPFacade = null;
        private UserFacade UserFacade = null;
        private EndPointFacade EndPointFacade = null;
        private SAPEPDA sapep_DA;
        #endregion

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

        public IList<SAPEPUserInfoHistory> Historylist { get; set; }
        private SAPEPDA m_sapepDA = null;
        private DirectoryEntry m_root = null;
        private DirectorySearcher m_sapepSearch = null;
        public SAPEPFacade sapepFacade { get; set; }
        public UserFacade userFacade { get; set; }

        int currentTry = 0;
        private SAPEPConfiguration m_sapepConfig = null;
        IList<SearchResultCollection> srList = new List<SearchResultCollection>();

        public int RetryCount
        {
            get
            {
                return 2;
            }
        }

        #region Constructors

        public SAPEP701Connector(DI.Chakra.Model.EndPoint _endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;

            SAPEPFacade = new SAPEPFacade();
            UserFacade = new UserFacade();
            EndPointFacade = new EndPointFacade();

            sapepFacade = new SAPEPFacade();
            userFacade = new UserFacade();
            m_sapepDA = new SAPEPDA();

            this.SAPEPConfiguration = (SAPEPConfiguration)new EndPointFacade().GetConfiguration(_endpoint);
        }

        #endregion

        public SAPEPConfiguration SAPEPConfiguration
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

        public override bool Connect()
        {
            if (!(this.EndPoint != null && SAPEPConfiguration != null && !string.IsNullOrEmpty(SAPEPConfiguration.WSDLUrl)))
                return false;

            try
            {
                m_serviceProxy = m_serviceProxy == null ? new ZZCustUsrMgmtWService() : m_serviceProxy;
                m_serviceProxy.Url = SAPEPConfiguration.WSDLUrl;
                string pwd = RSAHelper.DecryptData(SAPEPConfiguration.Password);
                m_serviceProxy.Credentials = new System.Net.NetworkCredential(this.SAPEPConfiguration.UserName, pwd);
                m_serviceProxy.getConnection();

                return true;
            }
            catch
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
            List<string> startChars = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,(,.".Split(',').ToList();

            foreach (string chr in startChars)
            {
                var userlist = this.getUsers(chr + "*");
                sapepFacade.SaveUsers(userlist, this.EndPoint);
            }

            var deletedUserlist = sapepFacade.GetDeletedUsers(this.EndPoint.Id, this.Process);

            foreach (var item in deletedUserlist)
            {
                item.Version = 2;
                item.LastModifiedDate = DateTime.Now;
                sapepFacade.UpdateUser(item);
                WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: User {0} has been REMOVED without any request made in the system", item.DisplayName));
            }

            sapepFacade.RemoveSAPEPUser(deletedUserlist);
        }

        public override void GetData()
        {
            IList<SAPEPUserInfo> listSAPEPUsers = null;

            try
            {
                //if (EndPoint.SAPEPConfiguration.ImportUsers)
                //{
                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = EndPointFacade.GetEndpointObjectsByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_Group) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAPEP_Role) != null;


                if (IsUserSelected)
                {
                    DiscoverUsers();
                }
                dicUsers = sapepFacade.getUserIds(this.EndPoint.Id);

                if (IsGroupSelected)
                {
                    this.DiscoverGroups();
                }
                dicGroups = sapepFacade.GetSAPEPGroupInfo(this.EndPoint.Id);
                if (IsUserSelected || IsGroupSelected)
                {
                    SaveUserGroups();
                }

                if (IsRoleSelected)
                {
                    this.DiscoverRoles();
                }
                dicRoles = sapepFacade.GetSAPEPRoleInfo();

                if (IsUserSelected || IsRoleSelected)
                {
                    SaveUserRoles();
                }
                listSAPEPUsers = SAPEPFacade.GetAllSAPEPUsersByEndPoint(this.EndPoint.Id);
                bool MapAll = false;
                if (SAPEPConfiguration.MapAll.HasValue)
                    MapAll = SAPEPConfiguration.MapAll.Value;
                new SAPEPFacade().GetAllMasterUsers(listSAPEPUsers, MapAll);
                SAPEPUserMap userMap = new SAPEPUserMap();
                //userMap.MapAll = SAPEPConfiguration.MapAll.Value;
                DataTable dtSAPEPUser = userMap.MapWithMasterUser(EndPoint);
            }
            catch (Exception ex)
            {
                //NeedToConfirm
                //Log.Discovery.WriteMessage(LogLevel.ERROR, string.Format("GetData is failed"), ex);
                throw;
            }
        }

        private void DiscoverRoles()
        {
            try
            {
                ZZUserInformation[] zuserList = m_serviceProxy.zGetAllUsers(string.Empty);

                IList<ZZPrincipalInformation> listWholePrinciple = new List<ZZPrincipalInformation>();

                IList<SAPEPUserRole> listWholeUserRole = new List<SAPEPUserRole>(); // IList<SAPEPUserRoleHistory> 

                RoleInfoOrchestrator roleOrch = new RoleInfoOrchestrator();

                roleOrch.TenantId = this.EndPoint.TenantId;

                roleOrch.EndpointSystemId = this.EndPoint.Id;

                //roleOrch.HistoryId = this.HistoryId;

                IList<SAPEPUserInfo> sapepUsers = sapepFacade.GetAllSAPEPUsersByEndPoint(this.EndPoint.Id);

                foreach (SAPEPUserInfo user in sapepUsers) // foreach (ZZUserInformation user in zuserList)
                {
                    ZZPrincipalInformation[] zprincipalList = m_serviceProxy.zGetAllRolesAssigned(user.UserName);

                    if (listWholePrinciple.Count <= 0)
                        listWholePrinciple = zprincipalList.ToList();
                    else
                        listWholePrinciple = listWholePrinciple.Union(zprincipalList.ToList()).ToList();

                    //IList<SAPEPUserRole> userRole = roleOrch.GetUserRoles(user.UserName, zprincipalList); // IList<SAPEPUserRoleHistory> 

                    //if (listWholeUserRole.Count <= 0)
                    //    listWholeUserRole = userRole;
                    //else
                    //    listWholeUserRole = listWholeUserRole.Union(userRole).ToList();

                    ArrayList strRoles = new ArrayList();
                    foreach (ZZPrincipalInformation zzgrps in zprincipalList)
                    {
                        strRoles.Add(zzgrps.strPrincipalName);
                    }

                    dicRoleUsers.Add(user.Id.ToString(), strRoles);
                }

                //SAPEPFacade.AddSAPEPUserRole(listWholeUserRole);

                IList<SAPEPRoleInfo> listWholeRoles = roleOrch.GetRoles(listWholePrinciple.ToArray()); // IList<SAPEPRoleInfoHistory> 
                //SAPEPFacade.AddSAPEPRoleInfo(listWholeRoles);
                sapepFacade.SaveSAPEPRoleInfo(listWholeRoles, this.EndPoint);

                IList<SAPEPRoleInfo> listRoles = sapepFacade.GetDeletedRoles(this.EndPoint.Id);
                sapepFacade.RemoveSAPEPRole(listRoles);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void DiscoverGroups()
        {
            try
            {
                ZZUserInformation[] zuserList = m_serviceProxy.zGetAllUsers(string.Empty);

                IList<ZZPrincipalInformation> listWholePrinciple = new List<ZZPrincipalInformation>();

                IList<SAPEPUserGroup> listWholeUserGroup = new List<SAPEPUserGroup>(); // IList<SAPEPUserGroupHistory>

                GroupInfoOrchestrator groupOrch = new GroupInfoOrchestrator();

                groupOrch.TenantId = this.EndPoint.TenantId;

                groupOrch.EndpointSystemId = (int)this.EndPoint.Id;

                //groupOrch.HistoryId = this.HistoryId;

                IList<SAPEPUserInfo> sapepUsers = sapepFacade.GetAllSAPEPUsersByEndPoint(this.EndPoint.Id);

                foreach (SAPEPUserInfo user in sapepUsers) // foreach (ZZUserInformation user in zuserList)
                {
                    ZZPrincipalInformation[] zprincipalList = m_serviceProxy.zGetAllGroupsAssigned(user.UserName);

                    if (listWholePrinciple.Count <= 0)
                        listWholePrinciple = zprincipalList.ToList();
                    else
                        listWholePrinciple = listWholePrinciple.Union(zprincipalList.ToList()).ToList();

                    //IList<SAPEPUserGroup> userGroup = groupOrch.GetUserGroups(user.UserName, zprincipalList); // IList<SAPEPUserGroupHistory> 

                    //if (listWholeUserGroup.Count <= 0)
                    //    listWholeUserGroup = userGroup;
                    //else
                    //    listWholeUserGroup = listWholeUserGroup.Union(userGroup).ToList();

                    ArrayList strGroups = new ArrayList();
                    foreach (ZZPrincipalInformation zzgrps in zprincipalList)
                    {
                        strGroups.Add(zzgrps.strPrincipalName);
                    }

                    dicGroupUsers.Add(user.Id.ToString(), strGroups);
                }

                //SAPEPFacade.AddSAPEPUserGroup(listWholeUserGroup);

                IList<SAPEPGroupInfo> listWholeGroups = groupOrch.GetGroups(listWholePrinciple.ToArray()); // IList<SAPEPGroupInfoHistory> 
                //SAPEPFacade.AddSAPEPGroupInfo(listWholeGroups);
                sapepFacade.SaveSAPEPGroupInfo(listWholeGroups, this.EndPoint);

                IList<SAPEPGroupInfo> listGroups = sapepFacade.GetDeletedGroups((int)this.EndPoint.Id);
                sapepFacade.RemoveSAPEPGroup(listGroups);
            }
            catch (Exception)
            {

                throw;
            }


        }

        private IList<SAPEPUserInfo> getUsers(string searchString)
        {
            try
            {
                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
                ZZUserInformation[] zuserList = m_serviceProxy.zGetAllUsers(searchString);
                orchestrator.TenantId = this.EndPoint.TenantId;
                orchestrator.EndpointSystemId = (int)this.EndPoint.Id;
                orchestrator.EndPoint = this.EndPoint;
                return orchestrator.GetUserInfo(zuserList);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, ex.ToString() + "Inner Exception:" + ex.InnerException.ToString());
                throw;
            }
        }

        #region IM Methods

        public override bool CreateUser(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

            if (sapEPUsr == null)
                return false;

            if (m_serviceProxy == null)
                this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

            bool isUsrCreated = false;
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

            isUsrCreated = m_serviceProxy.zZCreateNewUser(cnt_Usr.Cellphone, cnt_Usr.City, cnt_Usr.Company, cnt_Usr.Country, cnt_Usr.Department, cnt_Usr.Email, cnt_Usr.Fax, cnt_Usr.FirstName, cnt_Usr.IsCompanyUser, cnt_Usr.JobTitle, cnt_Usr.LastName, cnt_Usr.Salutation, cnt_Usr.State, cnt_Usr.Street, cnt_Usr.Telephone, cnt_Usr.Title, cnt_Usr.UniqueName, cnt_Usr.Zip, cnt_Usr.Language, cnt_Usr.Password, cnt_Usr.EndDate);

            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest, LogLevelType.Info, string.Format("User {0} Created Successfully in SAP EP.", sapEPUsr.UserName));

            return isUsrCreated;
        }

        public override bool UpdateUser(IEndpointUserInfo _userInfo)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

            if (sapEPUsr == null)
                return false;

            this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

            bool isUsrUpdated = false;
            ZZUserInformation cnt_Usr = new ZZUserInformation();

            cnt_Usr.UserName = sapEPUsr.UserName;
            cnt_Usr.FirstName = sapEPUsr.FirstName;
            cnt_Usr.LastName = sapEPUsr.LastName;
            cnt_Usr.JobTitle = sapEPUsr.JobTitle;
            cnt_Usr.Department = sapEPUsr.Department;
            cnt_Usr.Email = sapEPUsr.Email;

            isUsrUpdated = m_serviceProxy.zZUpdateUser(cnt_Usr.UserName, cnt_Usr.FirstName, cnt_Usr.LastName, cnt_Usr.JobTitle, cnt_Usr.Department, cnt_Usr.Email);

            return isUsrUpdated;
        }

        public override bool DeleteUser(IEndpointUserInfo _userInfo)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

            if (sapEPUsr == null)
                return false;

            this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

            return m_serviceProxy.zDeleteUser(sapEPUsr.UserName);
        }

        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

            bool isAdded = false;

            if (_entitlementType == EntitlementType.Role)
            {
                isAdded = this.AddUserToRole(sapEPUsr);
            }
            else if (_entitlementType == EntitlementType.Group)
            {
                isAdded = this.AssignGroupsToUser(sapEPUsr);
            }

            if (isAdded) base.AddEntitlement(_userInfo, _entitlementType);

            return isAdded;
        }

        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;
            bool isRemoved = false;

            if (_entitlementType == EntitlementType.Role)
            {
                isRemoved = this.RemoveUserFromRole(sapEPUsr);
            }
            else if (_entitlementType == EntitlementType.Group)
            {
                isRemoved = this.RemoveGroupAssignmentOfUser(sapEPUsr);
            }

            if (isRemoved) base.RemoveEntitlement(_userInfo, _entitlementType);

            return isRemoved;
        }

        private bool AddUserToRole(SAPEPUserInfo sapEPUsr)
        {
            bool isRoleAssigned;

            if (sapEPUsr.Roles.Count == 0)
                return true;


            if (m_serviceProxy == null)
                this.InitializedProxy((int)this.EndPoint.Id);

            isRoleAssigned = m_serviceProxy.zAddUserToRole(sapEPUsr.UserName, (object[])sapEPUsr.Roles.ToArray());

            return isRoleAssigned;
        }

        private bool RemoveUserFromRole(SAPEPUserInfo sapEPUsr)
        {
            bool isRoleAssignmentRemoved;

            if (sapEPUsr.Roles.Count == 0)
                return true;

            if (m_serviceProxy == null)
                this.InitializedProxy((int)this.EndPoint.Id);


            isRoleAssignmentRemoved = m_serviceProxy.zRemoveUserFromRole(sapEPUsr.UserName, (object[])sapEPUsr.Roles.ToArray());

            return isRoleAssignmentRemoved;
        }

        private bool AssignGroupsToUser(SAPEPUserInfo sapEPUsr)
        {
            bool isGroupAssigned;

            if (sapEPUsr.Groups.Count == 0)
                return true;

            if (m_serviceProxy == null)
                this.InitializedProxy((int)this.EndPoint.Id);


            //in Group, GroupName is used to pass in method so can't use GroupID as we are using in Role.
            isGroupAssigned = m_serviceProxy.zAddUserToGroup(sapEPUsr.UserName, (object[])sapEPUsr.Groups.ToArray());

            return isGroupAssigned;
        }

        private bool RemoveGroupAssignmentOfUser(SAPEPUserInfo sapEPUsr)
        {
            bool isGroupAssignmentRemoved;

            if (sapEPUsr.Groups.Count == 0)
                return true;

            if (m_serviceProxy == null)
                this.InitializedProxy((int)this.EndPoint.Id);

            //in Group GroupName is used to pass in method so can't user GroupID as we are using in Role.
            isGroupAssignmentRemoved = m_serviceProxy.zRemoveUserFromGroup(sapEPUsr.UserName, (object[])sapEPUsr.Groups.ToArray());

            return isGroupAssignmentRemoved;
        }

        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            if (m_serviceProxy == null)
                this.InitializedProxy(this.EndPoint.Id);

            bool result = false;
            result = m_serviceProxy.zResetUserPassword(userName, newPassword);
            if (result)
            {
                EndPointFacade EPF = new EndPointFacade();
                EPF.UpdatePasswordresetcount(this.EndPoint.Id, userName);
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
                catch { }
            }
            else
            {
                isConnOK = false;
            }
            return isConnOK;
        }

        private void InitializedProxy(long endPointID)
        {
            if (!(EndPoint != null && EndPoint.Id == endPointID))
            {
                EndPointFacade endPtFacade = new EndPointFacade();
                this.EndPoint = endPtFacade.GetEndPointSystemByKey(endPointID);
            }

            if (this.EndPoint != null && SAPEPConfiguration != null && !string.IsNullOrEmpty(SAPEPConfiguration.WSDLUrl))
            {
                m_serviceProxy = m_serviceProxy == null ? new ZZCustUsrMgmtWService() : m_serviceProxy;
                m_serviceProxy.Url = SAPEPConfiguration.WSDLUrl;
                string pwd = RSAHelper.DecryptData(SAPEPConfiguration.Password);
                m_serviceProxy.Credentials = new System.Net.NetworkCredential(SAPEPConfiguration.UserName, pwd);
            }
        }

        #endregion

        private void SaveUserGroups()
        {
            sapepFacade.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, (int)this.EndPoint.Id);
        }

        private void SaveUserRoles()
        {
            sapepFacade.SaveUserRoleMap(dicUsers, dicRoles, dicRoleUsers, (int)this.EndPoint.Id);
        }

        public override bool LockUser(IEndpointUserInfo _userInfo)
        {
            try
            {
                SAPEPUserInfo sapepUserinfo = (SAPEPUserInfo)_userInfo;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)sapepUserinfo.EndPointSystemId);

                return m_serviceProxy.zLockUnlockUser(true, sapepUserinfo.UserName);
            }
            catch
            {
                throw;
            }

        }

        public override bool UnLockUser(IEndpointUserInfo _userInfo)
        {
            try
            {
                SAPEPUserInfo sapepUserinfo = (SAPEPUserInfo)_userInfo;

                if (m_serviceProxy == null)
                    this.InitializedProxy((int)sapepUserinfo.EndPointSystemId);

                return m_serviceProxy.zLockUnlockUser(false, sapepUserinfo.UserName);
            }
            catch
            {
                throw;
            }
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            SAPEPUserInfo sapEPUsr = (SAPEPUserInfo)_userInfo;

            if (sapEPUsr == null)
                return false;

            this.InitializedProxy((int)sapEPUsr.EndPointSystemId);

            return m_serviceProxy.zZUpdateUser(sapEPUsr.UserName, sapEPUsr.FirstName, sapEPUsr.LastName, sapEPUsr.JobTitle, sapEPUsr.Department, sapEPUsr.Email);
        }

        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            this.RemoveEntitlement(_userInfo, EntitlementType.Role);
            this.RemoveEntitlement(_userInfo, EntitlementType.Group);
            return true;
        }


        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
    }
}

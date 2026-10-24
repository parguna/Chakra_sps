#region NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.SqlClient;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;

using DI.Chakra.DomainModel.UserHelper;
#endregion

namespace DI.Chakra.Connector

{
    public class IACMConnector : ConnectorBase
    {
        #region Members

        public EndpointConfiguration iacmConfiguration { get; set; }
        #endregion

        #region Core
        readonly string MasterConfigHelperServiceUser;
        public IACMConnector(EndPoint _endpoint, IdmsLogger logger, AccessRequest currentRequest = null) : base(logger)
        {
            this.iacmConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                IACM.IMProcessor processor = new IACM.IMProcessor();
                processor.EndPoint = this.Endpoint;
                processor.IacmConfiguration = this.iacmConfiguration;
                return processor;
            }
        }



        #endregion

        #region Discovery


        public override bool Connect()
        {
            //SqlConnection sqlCon = null;
            //try
            //{

            //    string IADsConStr = "";
            //    string UserName = "";
            //    string InstanceName = "";
            //    string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
            //    string DataSource = Configuration(EndpointKey.IPAddress);

            //    if (Convert.ToInt32(Configuration(EndpointKey.UserName)) != 0)
            //    {
            //        UserName = Configuration(EndpointKey.UserName);
            //    }
            //    else
            //    {
            //        UserName = "sa";
            //    }

            //    if (!String.IsNullOrEmpty(Configuration(EndpointKey.InstanceName)))
            //    {
            //        InstanceName = Configuration(EndpointKey.InstanceName);
            //    }

            //    IADsConStr = string.Format("Data Source ={0}; Initial Catalog = {1}; Persist Security Info = True; User Id = {2}; Password = {3};" +
            //        "Connect Timeout = 1000; providerName =System.Data.SqlClient", DataSource, InstanceName, Configuration(EndpointKey.UserName), Password);

            //    sqlCon = new SqlConnection(IADsConStr);

            //    WriteDiscoveryLog(LogLevelType.Exception, string.Format("IDAS connection opened"));
            //}
            //catch (Exception ex)
            //{
            //    WriteDiscoveryLog(LogLevelType.Exception, string.Format("IDAS connection failed to open"), ex);
            //    return false;
            //}
            //finally
            //{
            //    sqlCon.Close();
            //    sqlCon.Dispose();
            //}
            return true;
        }


        private string Configuration(EndpointKey Key)
        {
            var config = iacmConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public override void Disconnect()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Disconnected to IACM Server - {0}",
                                                              Configuration(EndpointKey.ServerName)));
        }

        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_IACM_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_IACM_Group) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_IACM_Role) != null;


                if (IsGroupSelected)
                {
                    DiscoverGroups();
                }
                if (IsRoleSelected)
                {
                    DiscoverRoles();
                }
                if (IsUserSelected)
                {
                    DiscoverUsers();
                }

                IacmUserHelper.MapUserIacmUser(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
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
                WriteDiscoveryLog(LogLevelType.Exception, "IACM GetData Failed.", ex);
                throw;
            }
        }

        #region Discover Objects
        void DiscoverUsers()
        {
            List<IACMUserInfo> userList = new List<IACMUserInfo>();
            IList<IACMUserInfo> userlist = new List<IACMUserInfo>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering IACM Users"), DiscoveryObjectType.User);
                userList = this.GetUsers();
                IACMUserInfo.SaveUsers(userList, Endpoint.Id, MasterConfigHelperServiceUser);

                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                userlist = IACMUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                IACMUserInfo.RemoveUser(userlist.ToList());
                try
                {
                    var query = $@"DELETE FROM DI_M_AccountReconcilation WHERE EndpointSystemId= {Endpoint.Id} AND IsDeleted = 1 AND EndpointUserId IN(SELECT EndpointUserId from DI_M_AccountReconcilation WHERE EndpointSystemId= {Endpoint.Id} AND IsDeleted = 0);
                DELETE FROM DI_M_AccountReconcilation WHERE EndpointSystemId= {Endpoint.Id} AND IsDeleted = 1 AND EndpointUserId NOT IN(SELECT Id from DI_IACM_UserInfo WHERE EndpointSystemId= {Endpoint.Id});";
                    IACMUserInfo.ExecuteQueryWithCommandTimeout(query, 0);
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Warning, $"{ex}", DiscoveryObjectType.User,ex);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering IACM Users"), DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                userList?.Clear();
                userList = null;

                userlist?.Clear();
                userlist = null;
            }
        }
        List<IACMUserInfo> GetUsers()
        {
            AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists();
            List<IACMUserInfo> userList = new List<IACMUserInfo>();
            var diUsers = User.GetAllIncludeRolesAndGroups();
            IList<IACMUserInfo> listExistingADusers = new List<IACMUserInfo>();
            listExistingADusers = IACMUserInfo.GetAll(false);
            foreach (var diUserItem in diUsers)
            {

                var iacmUser = IacmUserHelper.CreateAndSaveIacmUserInfo(diUserItem, listExistingADusers, this.Endpoint, _idmsLogger);

                if (listExistingADusers != null)
                    listExistingADusers.Add(iacmUser);

                if (iacmUser != null)
                    userList.Add(iacmUser);
            }
            return userList;
        }

        void AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists()
        {
            try
            {
                User.AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, $"Error while ReCreating User manager Role:{ex}", DiscoveryObjectType.Role,ex);
                throw ex;
            }
        }


        void DiscoverRoles()
        {
            List<IACMRoleInfo> rolelist = null;
            IList<IACMRoleInfo> listDeletedRole = new List<IACMRoleInfo>(); ;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering IACM Roles"), DiscoveryObjectType.Role);
                rolelist = this.GetRoles().ToList();
                IACMRoleInfo.SaveRoles(rolelist, MasterConfigHelperServiceUser);
                listDeletedRole = IACMRoleInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.DiscoveryProcess.Id);
                IACMRoleInfo.RemoveRoles(listDeletedRole.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering IACM Roles"), DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rolelist?.Clear();
                rolelist = null;

                listDeletedRole?.Clear();
                listDeletedRole = null;
            }
        }

        List<IACMRoleInfo> GetRoles()
        {
            List<IACMRoleInfo> roleList = new List<IACMRoleInfo>();
            IACMRoleInfo iacmRoleInfo = null;
            var iacmRoles = Role.GetAll();

            foreach (var iacmRole in iacmRoles)
            {
                iacmRoleInfo = IACMRoleInfo.GetByRoleName(iacmRole.RoleName);
                if (iacmRoleInfo == null)
                {
                    iacmRoleInfo = new IACMRoleInfo();
                    iacmRoleInfo.EndPoint = this.Endpoint;
                    iacmRoleInfo.CreatedBy = MasterConfigHelperServiceUser;
                    iacmRoleInfo.CreatedDate = DateTime.Now;
                    iacmRoleInfo.CanRequest = true;
                }

                if (iacmRole.RoleOwner.HasValue && iacmRoleInfo.Level1Approver == null)
                {
                    iacmRoleInfo.Level1Approver = iacmRole.RoleOwner.Value;
                    iacmRoleInfo.IsLevel1Group = false;
                    iacmRoleInfo.LastModifiedBy = MasterConfigHelperServiceUser;
                    iacmRoleInfo.LastModifiedDate = DateTime.Now;
                }

                iacmRoleInfo.Name = iacmRole.RoleName;
                iacmRoleInfo.Description = iacmRole.RoleDescription;
                iacmRoleInfo.IsActive = true;

                if (null != iacmRoleInfo)
                    roleList.Add(iacmRoleInfo);
            }

            return roleList;
        }
        void DiscoverGroups()
        {
            List<IACMGroupInfo> grplist = null;
            IList<IACMGroupInfo> listDeletedGroup = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering IACM Groups"), DiscoveryObjectType.Group);
                grplist = this.GetGroups().ToList();
                IACMGroupInfo.SaveGroups(grplist, MasterConfigHelperServiceUser);
                listDeletedGroup = new List<IACMGroupInfo>();
                listDeletedGroup = IACMGroupInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.DiscoveryProcess.Id);
                IACMGroupInfo.RemoveRoles(listDeletedGroup.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering IACM Groups"), DiscoveryObjectType.Group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                grplist?.Clear();
                grplist = null;

                listDeletedGroup?.Clear();
                listDeletedGroup = null;
            }
        }

        List<IACMGroupInfo> GetGroups()
        {
            List<IACMGroupInfo> grpList = new List<IACMGroupInfo>();
            IACMGroupInfo iacmGroupInfo = null;
            var iacmGroups = Group.GetAll();

            foreach (var iacmGroup in iacmGroups)
            {
                iacmGroupInfo = IACMGroupInfo.GetByGroupName(iacmGroup.GroupName);
                if (iacmGroupInfo == null)
                {
                    iacmGroupInfo = new IACMGroupInfo();
                    iacmGroupInfo.EndPoint = this.Endpoint;
                    iacmGroupInfo.CreatedBy = MasterConfigHelperServiceUser;
                    iacmGroupInfo.CreatedDate = DateTime.Now;
                    iacmGroupInfo.CanRequest = true;
                }

                if (iacmGroup.GroupOwner.HasValue && iacmGroupInfo.Level1Approver == null)
                {
                    iacmGroupInfo.Level1Approver = iacmGroup.GroupOwner.Value;
                    iacmGroupInfo.IsLevel1Group = false;
                    iacmGroupInfo.LastModifiedBy = MasterConfigHelperServiceUser;
                    iacmGroupInfo.LastModifiedDate = DateTime.Now;
                }

                iacmGroupInfo.Name = iacmGroup.GroupName;
                iacmGroupInfo.Description = iacmGroup.GroupDescription;
                iacmGroupInfo.IsActive = true;

                if (null != iacmGroupInfo)
                    grpList.Add(iacmGroupInfo);
            }

            return grpList;
        }
        #endregion

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            try
            {
                var user = User.GetById(DIUserId);
                if (user == null)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Not able find the IACM User username - {0}",
                                                              UserName));
                    return false;
                }


                user.Password = RSAHelper.EncryptData(Password);
                user.Save(UserName);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("IACM Change password completed for username - {0}",
                                                            UserName));
                return true;

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("IACM Change password failed for username - {0}",
                                                          UserName), ex);
            }
            return false;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
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


        #endregion

        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {
            _idmsLogger.WriteLog(LogLevelType.Error, $"AddEntitlement Started.", null);
            string UserExist = string.Empty;
            string iacm_roleInsert = "INSERT INTO IACMRoleInfoIACMUserInfo ( IACMRoleInfo_Id, IACMUserInfo_Id) VALUES( {0} , {1} )";
            string iacm_groupInsert = "INSERT INTO IACMUserInfoIACMGroupInfo ( IACMGroupInfo_Id,IACMUserInfo_Id ) VALUES({0} , {1})";

            string userRoleInsert = "INSERT INTO UserRole (Role_RoleId, User_UserId ) VALUES({0},{1})";
            string userGroupInsert = "INSERT INTO UserGroup (Group_GroupId , User_UserId ) VALUES({0},{1})";

            List<string> queryList = new List<string>();

            User usr = DomainModel.User.GetByName(_userInfo.AccountName);
            DomainModel.IACMUserInfo iacmUserInfo = DomainModel.IACMUserInfo.GetByUserName(_userInfo.AccountName, true);
            ((DomainModel.User)_userInfo).EndPointSystemId = iacmUserInfo.EndPointSystemId;
            try
            {
                if (entitlement == null || entitlement.Count() == 0)
                {
                    throw new Exception("No Entitlement is given AmendAdd");
                }

                if (usr != null)
                {
                    string type = string.Empty;
                    if (_entitlementType == EntitlementType.Role)
                    {
                        type = "Roles";
                        foreach (string role in entitlement.Distinct())
                        {
                            if (!usr.Roles.Any(r => r.RoleName == role))
                            {
                                var newRole = DomainModel.Role.GetByRoleName(role);
                                if (newRole != null)
                                {
                                    ((DomainModel.User)_userInfo).Roles.Add(newRole);
                                    queryList.Add(string.Format(userRoleInsert, newRole.Id, usr.Id));
                                }
                            }

                            if (!iacmUserInfo.Roles.Any(r => r.Name == role))
                            {
                                var newIacmRole = IACMRoleInfo.GetByRoleName(role);
                                if (newIacmRole != null)
                                {
                                    iacmUserInfo.Roles.Add(newIacmRole);
                                    queryList.Add(string.Format(iacm_roleInsert, newIacmRole.Id, iacmUserInfo.Id));
                                }
                            }

                        }
                    }

                    if (_entitlementType == EntitlementType.Group)
                    {
                        type = "Groups";
                        foreach (string group in entitlement.Distinct())
                        {
                            if (!usr.Groups.Any(r => r.GroupName == group))
                            {
                                var newGroup = DomainModel.Group.GetByGroupName(group);
                                if (newGroup != null)
                                {
                                    ((DomainModel.User)_userInfo).Groups.Add(newGroup);
                                    queryList.Add(string.Format(userGroupInsert, newGroup.Id, usr.Id));
                                }
                            }
                            if (!iacmUserInfo.Groups.Any(r => r.Name == group))
                            {
                                var newIacmGroup = IACMGroupInfo.GetByGroupName(group);
                                if (newIacmGroup != null)
                                {
                                    iacmUserInfo.Groups.Add(newIacmGroup);
                                    queryList.Add(string.Format(iacm_groupInsert, newIacmGroup.Id, iacmUserInfo.Id));
                                }
                            }
                        }
                    }

                    var success = queryList.Count > 0 ? DomainModel.User.ExecuteQuery(queryList) : true;
                    if (!success) throw new Exception($"IACMConnector AddEntitlementQuery Failed : {string.Join(";", queryList)}");

                    _idmsLogger.WriteLog(LogLevelType.Info, $"{type} : {string.Join(",", entitlement)} : Added to the user {iacmUserInfo.AccountName}.", null);

                    base.AddEntitlement(iacmUserInfo, _entitlementType);
                    return true;

                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, $"User {iacmUserInfo.AccountName} Not exists to Add Entitlement in IACM Connector.", null);
                    return false;
                }

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"Exception in Add Entitlement in IACM Connector.", null);

                _idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
        }

        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {

            string iacm_roleDelete = "Delete from IACMRoleInfoIACMUserInfo where IACMRoleInfo_Id = {0} and IACMUserInfo_Id = {1}";
            string iacm_groupDelete = "Delete from IACMUserInfoIACMGroupInfo where IACMGroupInfo_Id = {0} and IACMUserInfo_Id = {1} ";


            string userRoleDelete = "Delete from UserRole where Role_RoleId ={0} and User_UserId = {1} ";
            string UserGroupDelete = "Delete from UserGroup where Group_GroupId ={0} and User_UserId = {1} ";
            string entitlementReconQuery = "Update DI_M_EntitlementReconcilation Set IsDeleted=1, DeletedDate=GETDATE(),IsSODConflict=0,IsSensitiveAccess=0,LastAttestedOn=null,LastModifiedDate=GETDATE(),LastModifiedBy='System' where EndpointSystemId ={0} and EndpointUserId={1} and Entitlement='{2}' AND EntitlementType={3}";


            List<string> queryList = new List<string>();

            User usr = DomainModel.User.GetByName(_userInfo.AccountName);
            DomainModel.IACMUserInfo iacmUserInfo = DomainModel.IACMUserInfo.GetByUserName(_userInfo.AccountName, true);
            ((DomainModel.User)_userInfo).EndPointSystemId = iacmUserInfo.EndPointSystemId;
            try
            {
                if (entitlement == null || entitlement.Count() == 0)
                {
                    throw new Exception("No Entitlement is given AmendRemove");
                }
                usr = DomainModel.User.GetByUserNameFromIMProcessor(_userInfo.AccountName);
                if (usr != null)
                {
                    string type = string.Empty;

                    if (_entitlementType == EntitlementType.Role)
                    {
                        type = "Roles";
                        foreach (string role in entitlement.Distinct())
                        {
                            if (usr.Roles.Any(r => r.RoleName == role))
                            {
                                var newRole = DomainModel.Role.GetByRoleName(role);
                                ((DomainModel.User)_userInfo).Roles.Add(newRole);
                                queryList.Add(string.Format(userRoleDelete, newRole.Id, usr.Id));
                            }

                            if (iacmUserInfo.Roles.Any(r => r.Name == role))
                            {
                                var newIacmRole = IACMRoleInfo.GetByRoleName(role);
                                queryList.Add(string.Format(iacm_roleDelete, newIacmRole.Id, iacmUserInfo.Id));
                                queryList.Add(string.Format(entitlementReconQuery, newIacmRole.EndPointSystemId, iacmUserInfo.Id, role.Replace("'", "''"), 1));
                                iacmUserInfo.Roles.Remove(newIacmRole);
                            }
                        }
                    }

                    if (_entitlementType == EntitlementType.Group)
                    {
                        type = "Groups";
                        foreach (string group in entitlement.Distinct())
                        {
                            if (usr.Groups.Any(r => r.GroupName == group))
                            {
                                var newGroup = DomainModel.Group.GetByGroupName(group);
                                ((DomainModel.User)_userInfo).Groups.Add(newGroup);
                                queryList.Add(string.Format(UserGroupDelete, newGroup.Id, usr.Id));
                            }
                            if (iacmUserInfo.Groups.Any(r => r.Name == group))
                            {
                                var newIacmGroup = IACMGroupInfo.GetByGroupName(group);
                                queryList.Add(string.Format(iacm_groupDelete, newIacmGroup.Id, iacmUserInfo.Id));
                                queryList.Add(string.Format(entitlementReconQuery, newIacmGroup.EndPointSystemId, iacmUserInfo.Id, group.Replace("'", "''"), 2));
                                iacmUserInfo.Groups.Remove(newIacmGroup);
                            }
                        }

                    }
                    var success = queryList.Count > 0 ? DomainModel.User.ExecuteQuery(queryList) : true;
                    if (!success) throw new Exception($"IACMConnector RemoveEntitlement Query Failed : {string.Join(";", queryList)}");

                    _idmsLogger.WriteLog(LogLevelType.Info, String.Format("{0} : {1} : Removed from the user {2}.", type, string.Join(",", entitlement), iacmUserInfo.AccountName), null);

                    iacmUserInfo = DomainModel.IACMUserInfo.GetByUserName(_userInfo.AccountName, true);
                    RemoveEntitlement(iacmUserInfo, _entitlementType);
                    return true;


                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists in RemoveEntitlement in IACM Connector.", iacmUserInfo.AccountName), null);
                    return false;
                }

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("Exception in RemoveEntitlement in IACM Connector.", iacmUserInfo.AccountName), null);

                _idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
        }

        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            string iacm_roleDelete = "Delete from IACMRoleInfoIACMUserInfo where  IACMUserInfo_Id = {0}";
            string iacm_groupDelete = "Delete from IACMUserInfoIACMGroupInfo where  IACMUserInfo_Id = {0} ";

            string userRoleDelete = "Delete from UserRole where  User_UserId = {0} ";
            string UserGroupDelete = "Delete from UserGroup where  User_UserId = {0} ";

            List<string> queryList = new List<string>();

            User usr = DomainModel.User.GetByName(_userInfo.AccountName);
            DomainModel.IACMUserInfo iacmUserInfo = DomainModel.IACMUserInfo.GetByUserName(_userInfo.AccountName, true);

            try
            {

                usr = DomainModel.User.GetByUserNameFromIMProcessor(_userInfo.AccountName);
                if (usr != null)
                {
                    string type = string.Empty;

                    queryList.Add(string.Format(iacm_groupDelete, iacmUserInfo.Id));
                    queryList.Add(string.Format(iacm_roleDelete, iacmUserInfo.Id));
                    queryList.Add(string.Format(userRoleDelete, usr.Id));
                    queryList.Add(string.Format(UserGroupDelete, usr.Id));

                    var success = queryList.Count > 0 ? DomainModel.User.ExecuteQuery(queryList) : true;

                    return success;
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, String.Format("User {0} Not exists in RemoveEntitlement in IACM Connector.", iacmUserInfo.AccountName), null);
                    return false;
                }

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, String.Format("Exception in RemoveAllEntitlement in IACM Connector.", iacmUserInfo.AccountName), null);

                _idmsLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
        }
        protected override List<string> GetValueByFieldName(IEndpointUserInfo _userInfo, string fieldName)
        {
            DomainModel.IACMUserInfo user = (DomainModel.IACMUserInfo)_userInfo;

            if (user != null)
            {
                if (fieldName == "Roles")
                    return user.Roles.Select(r => r.Name).ToList();
                if (fieldName == "Groups")
                    return user.Groups.Select(r => r.Name).ToList();
                return new List<string>();
            }
            else
            {
                return new List<string>();
            }
        }

        protected override bool RemoveEntitlements(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                long? diuserId = this.CurrentRequest?.UserId ?? null;

                if (_entitlementType == EntitlementType.Group)
                {
                    var groups = this.GetValueByFieldName(_userInfo, "Groups");

                    if (groups != null && groups.Count > 0)
                    {
                        var entRecons = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, _entitlementType);

                        foreach (var entRecon in entRecons)
                        {
                            if (!groups.Contains(entRecon.Entitlement))
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else if (_entitlementType == EntitlementType.Role)
                {
                    var roles = this.GetValueByFieldName(_userInfo, "Roles");

                    if (roles != null && roles.Count > 0)
                    {
                        var entRecons = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, _entitlementType);

                        foreach (var entRecon in entRecons)
                        {
                            if (!roles.Contains(entRecon.Entitlement))
                            {
                                this.DeleteEntitlementReconcilation(entRecon);
                            }
                        }
                    }
                }
                else
                    base.RemoveEntitlement(_userInfo, _entitlementType);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while removing Entitlements to user - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }
            return true;
        }
    }
}


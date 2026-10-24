using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

using System.Data;
using DeepIdentity.DataAccess;
using DeepIdentity.Logger;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model.ReportData;

namespace DeepIdentity.Facade
{
    public class SAPEPFacade
    {
        public SAPEPDA SAPEPDA { get; set; }

        public UserDA UserDA { get; set; }

        private bool m_populate = false;
        private EndpointObjectDA EndpointObjectDA = null;



        public SAPEPFacade()
        {


            SAPEPDA = new SAPEPDA();

            UserDA = new UserDA();
            EndpointObjectDA = new EndpointObjectDA();
        }


        public bool PopulateUserIntoDB
        {
            get
            {
                return m_populate;
            }
            set
            {
                m_populate = value;
            }
        }

     
        public SAPEPUserRole getUserRole(SAPEPUserRoleHistory history)
        {
            SAPEPUserRole role = new SAPEPUserRole();

            try
            {
                role.Id = history.Id;
                role.TenantId = history.TenantId;
                role.EndPointSystemId = history.EndPointSystemId;
                role.RoleId = history.RoleId;
                role.UserName = history.UserName;
                role.CreatedBy = history.CreatedBy;
                role.CreatedDate = history.CreatedDate;
                role.LastModifiedBy = history.LastModifiedBy;
                role.LastModifiedDate = history.LastModifiedDate;
            }
            catch (Exception)
            {

                throw;
            }

            return role;
        }
        public SAPEPUserGroup getUserGroup(SAPEPUserGroupHistory history)
        {
            SAPEPUserGroup group = new SAPEPUserGroup();

            try
            {
                group.Id = history.Id;
                group.TenantId = history.TenantId;
                group.EndPointSystemId = history.EndPointSystemId;
                group.GroupId = history.GroupId;
                group.UserName = history.UserName;
                group.CreatedBy = history.CreatedBy;
                group.CreatedDate = history.CreatedDate;
                group.LastModifiedBy = history.LastModifiedBy;
                group.LastModifiedDate = history.LastModifiedDate;
            }
            catch (Exception)
            {

                throw;
            }

            return group;
        }
        public SAPEPGroupInfo getGroupInfo(SAPEPGroupInfoHistory history)
        {

            SAPEPGroupInfo group = new SAPEPGroupInfo();

            try
            {
                group.Id = history.Id;
                group.TenantId = history.TenantId;
                group.EndPointSystemId = history.EndPointSystemId;
                group.GroupId = history.GroupId;
                group.GroupDescription = history.GroupDescription;
                group.GroupName = history.GroupName;
                group.GroupUniqueName = history.Extar1;
                group.CreatedBy = history.CreatedBy;
                group.CreatedDate = history.CreatedDate;
                group.LastModifiedBy = history.LastModifiedBy;
                group.LastModifiedDate = history.LastModifiedDate;
            }
            catch (Exception)
            {

                throw;
            }

            return group;
        }

        public SAPEPRoleInfo getRoleInfo(SAPEPRoleInfoHistory history)
        {
            SAPEPRoleInfo role = new SAPEPRoleInfo();

            try
            {
                role.Id = history.Id;
                role.TenantId = history.TenantId;
                role.EndPointSystemId = history.EndPointSystemId;
                role.RoleId = history.RoleId;
                role.RoleDescription = history.RoleDescription;
                role.RoleName = history.RoleName;
                role.UniqueRoleName = history.Extar1;
                role.CreatedBy = history.CreatedBy;
                role.CreatedDate = history.CreatedDate;
                role.LastModifiedBy = history.LastModifiedBy;
                role.LastModifiedDate = history.LastModifiedDate;
            }


            catch (Exception)
            {

                throw;
            }

            return role;
        }

        #region SAPEP CONFIGURATION



        public IList<SAPEPUserInfo> GetSAPEPUserInfoByDIUserId(int UserId)
        {
            return SAPEPDA.GetSAPEPUserInfoByDIUserId(UserId);
        }

        public List<SAPEPUserInfo> GetPrimarySAPEPUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            return SAPEPDA.GetPrimarySAPEPUserByDIUserId(DIUserId, endpointSystemId);
        }

        public SAPEPUserInfo GetSAPEPUserByUserId(int userId, int endpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPUserByUserId(userId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public void GetSAPEPUserGroup(int UserId, out IList<SAPEPGroupInfo> GrpList, out IList<SAPEPRoleInfo> RoleList)
        {
            SAPEPDA.GetSAPEPUserGroup(UserId, out GrpList, out RoleList);
        }

        public void GetSAPEPUserGroupByUserId(int UserId, out IList<SAPEPGroupInfo> GrpList, out IList<SAPEPRoleInfo> RoleList)
        {
            SAPEPDA.GetSAPEPUserGroupByUserId(UserId, out GrpList, out RoleList);
        }

        

        #endregion

 
        //Newly Added

        public void ExecuteQry(string cmd)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();
            EndpointObjectDA.ExecuteQry(cmd);

        }

        public IList<SAPEPUserInfo> GetAllSAPEPUsers()
        {
            try
            {
                return SAPEPDA.GetAllSAPEPUsers();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPUserInfo> GetAllSAPEPUsersByEndPoint(int EndPoinId)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPUsersByEndPoint(EndPoinId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPUserInfoReportData> GetAllSAPEPUsersReportData(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPUsersReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPEPGroupInfo> GetAllSAPEPGroups(int EndpointSystemId, bool IsXLUploaded)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPGroups(EndpointSystemId, IsXLUploaded);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPRoleInfo> GetAllSAPEPRoles(int EndpointSystemId, bool IsXLUploaded)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPRoles(EndpointSystemId, IsXLUploaded);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPUserToGroupsReportData> GetSAPEPUserToGroupsReportData(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPUserToGroupsReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPGroupToUsersReportData> GetSAPEPGroupToUsersReportData(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPGroupToUsersReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPUserToRolesReportData> GetSAPEPUserToRolesReportData(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPUserToRolesReportData(EndpointSystemId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<SAPEPRoleToUsersReportData> GetSAPEPRoleToUsersReportData(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPRoleToUsersReportData(EndpointSystemId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateUser(SAPEPUserInfo _user)
        {
            try
            {
                SAPEPDA.UpdateUser(_user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<EndpointObjects> GetSAPEPObjects()
        {
            try
            {
                return SAPEPDA.GetSAPEPObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUsers(IList<SAPEPUserInfo> list, EndPoint paramendpoint)
        {
            try
            {
                SAPEPUserInfo sapepexistingUser = null;
                foreach (SAPEPUserInfo usr in list)
                {
                    sapepexistingUser = SAPEPDA.GetUserByUserId(usr.UserName, paramendpoint.EndpointSystemId);
                    if (sapepexistingUser == null)
                    {
                        usr.Version = 2;
                        SAPEPDA.AddSAPEPUser(usr);
                        DILog.DiscoveryLogger.WriteLog(paramendpoint, LogLevelType.Warning, string.Format("Reconciliation Exception: SAPEP User {0} has been ADDED without any request made in the system", usr.DisplayName), DiscoveryObjectType.User, list.Count);
                    }
                    else
                    {
                        usr.IsLocked = sapepexistingUser.IsLocked;
                        usr.LockScenario = sapepexistingUser.LockScenario;
                        usr.TenantId = sapepexistingUser.TenantId;
                        usr.UserId = sapepexistingUser.UserId;
                        usr.DIUserId = sapepexistingUser.DIUserId;
                        usr.MappingType = sapepexistingUser.MappingType;
                        usr.AccountType = sapepexistingUser.AccountType;
                        usr.IfUnmappedMapTo = sapepexistingUser.IfUnmappedMapTo;
                        usr.CreatedDate = sapepexistingUser.CreatedDate;
                        usr.CreatedBy = sapepexistingUser.CreatedBy;
                        usr.Version = sapepexistingUser.Version;
                        SAPEPDA.UpdateUser(usr);
                    }
                    new ReconcilationFacade().AddOrUpdateAccountInReconTable(usr, sapepexistingUser == null ? true : false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPUserInfo> GetDeletedUsers(int EndpointId, Process discoveryProcess)
        {
            try
            {
                return SAPEPDA.GetDeletedUsers(EndpointId, discoveryProcess);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSAPEPUser(IList<SAPEPUserInfo> listDelUser)
        {
            try
            {
                foreach (SAPEPUserInfo user in listDelUser)
                {
                    SAPEPDA.RemoveSAPEPUser(user);
                    new ReconcilationFacade().DeleteAccount(user.EndPointSystemId, user.UserId);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> getUserIds(int EndpointID)
        {
            try
            {
                return SAPEPDA.getUserIds(EndpointID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPGroupInfo> GetDeletedGroups(int EndpointId)
        {
            try
            {
                return SAPEPDA.GetDeletedGroups(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPRoleInfo> GetDeletedRoles(int EndpointId)
        {
            try
            {
                return SAPEPDA.GetDeletedRoles(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPEPUserInfo GetLockedSAPEPUserByDIUserId(int UserId, int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetLockedSAPEPUserByDIUserId(UserId, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSAPEPGroup(IList<SAPEPGroupInfo> listDelGroups)
        {
            try
            {
                foreach (SAPEPGroupInfo group in listDelGroups)
                {
                    SAPEPDA.RemoveSAPEPGroup(group);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSAPEPRole(IList<SAPEPRoleInfo> listDelRoles)
        {
            try
            {
                foreach (SAPEPRoleInfo role in listDelRoles)
                {
                    SAPEPDA.RemoveSAPEPRole(role);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> GetSAPEPGroupInfo(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPGroupInfo(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> GetSAPEPRoleInfo()
        {
            try
            {
                return SAPEPDA.GetSAPEPRoleInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUserGroupMap(Dictionary<int, string> dicUsers, Dictionary<int, string> dicGroups, Dictionary<string, ArrayList> dicArray, int endpointSystemid)
        {
            try
            {
                SAPEPDA.SaveUserGroupMap(dicUsers, dicGroups, dicArray, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUserRoleMap(Dictionary<int, string> dicUsers, Dictionary<int, string> dicRoles, Dictionary<string, ArrayList> dicArray, int endpointSystemid)
        {
            try
            {
                SAPEPDA.SaveUserRoleMap(dicUsers, dicRoles, dicArray, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public void SaveSAPEPGroupInfo(IList<SAPEPGroupInfo> paramList, EndPoint paramendpoint)
        {
            try
            {
                SAPEPGroupInfo sapepExistingGroup = null;
                foreach (SAPEPGroupInfo grp in paramList)
                {
                    sapepExistingGroup = SAPEPDA.getGroupByUniqueId(grp.GroupId, paramendpoint.EndpointSystemId);
                    if (sapepExistingGroup == null)
                    {
                        SAPEPDA.AddSAPEPGroup(grp);
                    }
                    else
                    {
                        grp.Id = sapepExistingGroup.Id;
                        grp.GroupUniqueName = sapepExistingGroup.GroupUniqueName;
                        grp.IsXLUploaded = sapepExistingGroup.IsXLUploaded;
                        grp.CreatedDate = sapepExistingGroup.CreatedDate;
                        grp.CreatedBy = sapepExistingGroup.CreatedBy;
                        SAPEPDA.UpdateSAPEPGroup(grp);
                    }
                }

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Windows Native Group objects", paramList.Count.ToString()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPEPRoleInfo(IList<SAPEPRoleInfo> paramList, EndPoint paramendpoint)
        {
            try
            {
                SAPEPRoleInfo sapepExistingRole = null;
                foreach (SAPEPRoleInfo rle in paramList)
                {
                    sapepExistingRole = SAPEPDA.getRoleByUniqueId(rle.RoleId, paramendpoint.EndpointSystemId);
                    if (sapepExistingRole == null)
                    {
                        SAPEPDA.AddSAPEPRole(rle);
                    }
                    else
                    {
                        rle.Id = sapepExistingRole.Id;
                        rle.UniqueRoleName = sapepExistingRole.UniqueRoleName;
                        rle.IsXLUploaded = sapepExistingRole.IsXLUploaded;
                        rle.CreatedDate = sapepExistingRole.CreatedDate;
                        rle.CreatedBy = sapepExistingRole.CreatedBy;
                        SAPEPDA.UpdateSAPEPRole(rle);
                    }
                }

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Windows Native Group objects", paramList.Count.ToString()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPEPGroupInfo> GetSAPEPGroupsByUserId(int UserId, int? endpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPGroupsByUserId(UserId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }


        public List<SAPEPRoleInfo> GetSAPEPRolesByUserId(int UserId, int? endpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPRolesByUserId(UserId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public IList<SAPEPUserInfoHistory> GetAllSAPEPHistoryUsersbyendpointsystemID(int endpointsystemid)
        {
            try
            {
                return SAPEPDA.GetAllSapepHistorybyendpointid(endpointsystemid);
            }
            catch(Exception e)
            {
                throw;
            }
        }
        public IList<SAPEPUserInfoHistory> GetAllSAPEPHistoryUsers()
        {
            try
            {
                return SAPEPDA.GetAllSAPEPHistoryUsers();
            }
            catch
            {
                throw;
            }
        }


        public Dictionary<string, string> GetGroupSAPEP(int? EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetGroupSAPEP(EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetGroupsByEndpointUser(int EndpointUserId, int? EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetGroupsByEndpointUser(EndpointUserId, EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetRolesByEndpointUser(int EndpointUserId, int? EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetRolesByEndpointUser(EndpointUserId, EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<string, string> GetRoleSAPEP(int? EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetRoleSAPEP(EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetSAPEPUsersByDIUserId(int userId, int endpointSystemId, AccountType accountType)
        {
            try
            {
                return SAPEPDA.GetSAPEPUsersByDIUserId(userId, endpointSystemId, accountType);
            }
            catch
            {
                throw;
            }
        }

        public SAPEPUserInfo GetSAPEPUserByUserName(string userName, int endpointSystemId)
        {
            try
            {
                return SAPEPDA.GetSAPEPUserByUserName(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateSAPEPUser(SAPEPUserInfo sapepUser)
        {
            try
            {
                SAPEPDA.UpdateSAPEPUser(sapepUser);
            }
            catch
            {
                throw;
            }
        }

        public bool CheckSAPEPUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                return SAPEPDA.CheckSAPEPUserNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetGroupSAPEPForRemove(int EndpointUserID)
        {
            try
            {
                return SAPEPDA.GetGroupSAPEPForRemove(EndpointUserID);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetRolesSAPEPForRemove(int EndpointUserID)
        {
            try
            {
                return SAPEPDA.GetRolesSAPEPForRemove(EndpointUserID);
            }
            catch
            {
                throw;
            }
        }

        public IList<SAPEPGroupInfo> GetAllSAPEPGroupsByEndpointsystemid(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPGroupsByEndpointsystemid(EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public IList<SAPEPRoleInfo> GetAllSAPEPRolesByEndpointSystemID(int EndpointSystemId)
        {
            try
            {
                return SAPEPDA.GetAllSAPEPRolesByEndpointSystemID(EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetPortalGroups(int? EndpointSystemId, string SearchString)
        {
            try
            {
                return SAPEPDA.GetPortalGroups(EndpointSystemId, SearchString);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetPortalRoles(int? EndpointSystemId, string SearchString)
        {
            try
            {
                return SAPEPDA.GetPortalRoles(EndpointSystemId, SearchString);
            }
            catch
            {
                throw;
            }
        }

        public SAPEPUserInfo GetSAPEPUserInfo(int EndPointUserID)
        {
            return SAPEPDA.GetSAPEPUserInfo(EndPointUserID);
        }

        public SAPEPGroupInfo getGroupByName(string grpName, int EndpointSystemId)
        {
            return SAPEPDA.getGroupByName(grpName, EndpointSystemId);
        }

        public SAPEPRoleInfo getRoleByName(string rleName, int EndpointSystemId)
        {
            return SAPEPDA.getRoleByName(rleName, EndpointSystemId);
        }
        public void GetAllMasterUsers(IList<SAPEPUserInfo> listSAPEPUsers, bool mapAll)
        {
            int count = -1;
            int newRowCount = -1;
            DataRow[] dRowsMaster = null;

            string clauseType = "AND";
            if (!mapAll)
                clauseType = "OR";


            StringBuilder bldr = new StringBuilder();

            IList<User> users = new List<User>();

            object obj = null;

            IList<ConfigGeneralTrustedSource> listTrustedConfig = null;
            IList<ConfigGeneralTrustedSource> list = new DiConfigurationDA().GetAllConfigTrustedSource();

            listTrustedConfig = list.Where(li => li.TrustedSource == EndpointType.SAPEnterprisePortal).ToList();

            IList<AttributeMappingColumns> listAttributes = new DiConfigurationDA().GetAttributeMappingColumns(listSAPEPUsers.FirstOrDefault().EndPointSystemId);

            if (listTrustedConfig.Count <= 0)
            {
                Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Populate User: No attribute is configured from SAP"));
                return;
            }

            if (listAttributes.Count <= 0)
            {
                Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Populate User: Attribute Mapping is NOT configured for SAP"));
                return;
            }

            Type usertype = typeof(User);

            IList<PropertyInfo> userProperties = usertype.GetProperties().ToList();



            IList<User> listUsers = UserDA.GetAllUsers();

            DataTable dtMaster = CollectionHelper.ConvertToNormalTable<User>(listUsers);

            DataTable dtExUser = CollectionHelper.ConvertToNormalTable<SAPEPUserInfo>(listSAPEPUsers);


            GeneralDA generalDA = new GeneralDA();

            newRowCount = generalDA.GetMaxId("DI_M_User", "UserId");



            foreach (DataRow adsRow in dtExUser.Rows)
            {
                count = 0;
                bldr = new StringBuilder();

                foreach (AttributeMappingColumns att in listAttributes)
                {

                    bldr.AppendFormat(string.Format("{0}='{1}'", att.DiColumn, adsRow[att.Tgcolumn].ToString().Replace("'", "''")));

                    count++;

                    if (count < listAttributes.Count)
                        bldr.AppendFormat(" {0} ", clauseType);

                }

                dRowsMaster = dtMaster.Select(bldr.ToString());

                if (dRowsMaster.Count() > 0) //modify existing user
                {

                    foreach (ConfigGeneralTrustedSource trustedSource in listTrustedConfig)
                    {

                        if (string.IsNullOrEmpty(dRowsMaster[0][trustedSource.AttributeName].ToString()))
                            dRowsMaster[0][trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];

                        else if (!trustedSource.SelfUpdate)
                            dRowsMaster[0][trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];
                    }

                    dRowsMaster[0]["LastModifiedBy"] = "Scheduler";
                    dRowsMaster[0]["LastModifiedDate"] = DateTime.Now;

                }

                
            }



            UserDA.UpdateMasterLatest(dtMaster, null);

            Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Global users populated"));

        }
    }
}

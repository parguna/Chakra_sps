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



namespace DeepIdentity.Facade
{
    public class ActiveDirectoryFacade : IEndpointFacade
    {
        public ActiveDirectoryDA ActiveDirectoryDA { get; set; }
        private ReconcilationFacade reconcilationFacade = new ReconcilationFacade();

        private bool m_populate = false;
        private EndpointObjectDA EndpointObjectDA = null;


        public ActiveDirectoryFacade()
        {


            ActiveDirectoryDA = new ActiveDirectoryDA();


            EndpointObjectDA = new EndpointObjectDA();

        }
        #region IActiveDirectoryObjectFacade Members

        public IEndpointUserInfo GeEndpointUserInfo(int EndpointUserId)
        {
            return GetADSUserInfo(EndpointUserId);
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

        public IList<ADSUserInfo> GetAllADSUsers()
        {
            try
            {
                return ActiveDirectoryDA.GetAllADSUserInfo();
            }
            catch
            {
                throw;
            }
        }

        public bool CheckADUserNameExists(string samaAccountName, int endpointSystemId)
        {
            try
            {
                return ActiveDirectoryDA.CheckADUserNameExists(samaAccountName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

       

        public IList<ADSUserInfo> GetAllADSUsersInfo()
        {
            try
            {

                IList<ADSUserInfo> adUsers = ActiveDirectoryDA.GetAllADSUserInfo();

                return adUsers;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSUserInfo> GetAllADSUserInfoBasedOnLastLogon()
        {
            try
            {

                IList<ADSUserInfo> adUsers = ActiveDirectoryDA.GetAllADSUserInfoBasedOnLastLogon();
                return adUsers;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddADSUserInfo(IList<ADSUserInfo> list)
        {
            try
            {
                ActiveDirectoryDA.AddADSUserInfoBulk(list);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private Object GetPropValue(String name, Object obj)
        {

            Type type = obj.GetType();

            PropertyInfo info = type.GetProperty(name);

            if (info == null)
                return null;


            if (info.ToString().Equals(string.Empty))
                return null;

            obj = info.GetValue(obj, null);

            if (info.PropertyType == typeof(DateTime))
            {
                if (obj == null || obj.ToString() == string.Empty)
                    return DBNull.Value;
            }

            return obj;
        }

        private User getUser(ADSUserInfo adsUser)
        {
            User user = User.CreateUser();

            IList<AttributeMappingColumns> attributes = new DiConfigurationFacade().GetAttributeMappingColumns(adsUser.EndPointSystemId);

            Type type = typeof(User);

            type.GetProperties();


            foreach (PropertyInfo info in type.GetProperties())
            {
                IList<AttributeMappingColumns> attrs = attributes.Where(att => att.DiColumn == info.Name).ToList();

                if (attrs.Count > 0)
                {
                    AttributeMappingColumns DIAttribute = attrs.First();


                    object obj = GetPropValue(DIAttribute.Tgcolumn, adsUser);

                    info.SetValue(user, obj, null);
                }
            }


            return user;
        }


        public void AddADSComputerGroup(IList<ADSComputerGroup> _list)
        {
            try
            {
                this.DeleteAll<ADSComputerGroup>(_list.First().TenantId, _list.First().EndpointSystemId);

                ActiveDirectoryDA.AddADSComputerGroupBulk(_list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD ComputerGroup objects", _list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateAdsUserInfoOnAttributeMap(IList<ADSUserInfo> _adsUserInfoList)
        {
            try
            {
                ActiveDirectoryDA.UpdateUserInfoBatchOnAttributeMap(_adsUserInfoList);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateAdsUserInfoOnAttributeMap(DataTable dtAdsUser, EndPoint EndPoint)
        {
            try
            {
                ActiveDirectoryDA.UpdateUserInfoBatchOnAttributeMap(dtAdsUser, EndPoint);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void AddADSGroupInfo(IList<ADSGroupInfoHistory> list)
        {
            try
            {
                ActiveDirectoryDA.AddADSGroupInfoHistoryBulk(list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD GroupHistory objects", list.Count.ToString()));

                var listUser = list.IterateConvert<ADSGroupInfoHistory, ADSGroupInfo>(
                   delegate(ADSGroupInfoHistory _group)
                   {
                       return this.GetGroupInfo(_group);
                   });

                ActiveDirectoryDA.DeleteAll<ADSGroupInfo>(list.First().TenantId, list.First().EndPointSystemId);

                ActiveDirectoryDA.AddADSGroupInfoBulk(listUser);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD Group objects", list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void AddADSOuInfo(IList<ADSOuInfo> list)
        {
            try
            {

                ActiveDirectoryDA.DeleteAll<ADSOuInfo>(list.First().TenantId, list.First().EndPointSystemId);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD OU objects", list.Count.ToString()));

                ActiveDirectoryDA.AddADSOuInfoBulk(list);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void AddADSComputerInfo(IList<ADSComputerInfo> list)
        {
            try
            {
                ActiveDirectoryDA.DeleteAll<ADSComputerInfo>(list.First().TenantId, list.First().EndPointSystemId);

                ActiveDirectoryDA.AddADSComputerInfoBulk(list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD Computer objects", list.Count.ToString()));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddADSPrinterInfo(IList<ADSPrinterInfo> list)
        {
            try
            {

                ActiveDirectoryDA.DeleteAll<ADSPrinterInfo>(list.First().TenantId, list.First().EndPointSystemId);

                ActiveDirectoryDA.AddADSPrinterInfoBulk(list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD Printer objects", list.Count.ToString()));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddADSSharedFolderInfo(IList<ADSSharedFolderInfo> list)
        {
            try
            {

                ActiveDirectoryDA.DeleteAll<ADSSharedFolderInfo>(list.First().TenantId, list.First().EndPointSystemId);

                ActiveDirectoryDA.AddADSSharedFolderInfoBulk(list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} AD SharedFolder objects", list.Count.ToString()));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ADSOuInfo GetOuInfo(ADSOuInfoHistory _history)
        {
            ADSOuInfo adsOuInfo = new ADSOuInfo();

            adsOuInfo.OuId = _history.OuId;
            adsOuInfo.TenantId = _history.TenantId;
            adsOuInfo.EndPointSystemId = _history.EndPointSystemId;
            adsOuInfo.ou = _history.ou;
            adsOuInfo.description = _history.description;
            adsOuInfo.distinguishedName = _history.distinguishedName;
            adsOuInfo.instanceType = _history.instanceType;
            adsOuInfo.whenCreated = _history.whenCreated;
            adsOuInfo.whenChanged = _history.whenChanged;
            adsOuInfo.uSNCreated = _history.uSNCreated;
            adsOuInfo.uSNChanged = _history.uSNChanged;
            adsOuInfo.showInAdvancedViewOnly = _history.showInAdvancedViewOnly;
            adsOuInfo.name = _history.name;
            adsOuInfo.objectGUID = _history.objectGUID;
            adsOuInfo.systemFlags = _history.systemFlags;
            adsOuInfo.objectCategory = _history.objectCategory;
            adsOuInfo.isCriticalSystemObject = _history.isCriticalSystemObject;
            adsOuInfo.gPLink = _history.gPLink;
            adsOuInfo.nTSecurityDescriptor = _history.nTSecurityDescriptor;
            adsOuInfo.CreatedBy = _history.CreatedBy;
            adsOuInfo.CreatedDate = _history.CreatedDate;
            adsOuInfo.LastModifiedBy = _history.LastModifiedBy;
            adsOuInfo.LastModifiedDate = _history.LastModifiedDate;

            return adsOuInfo;
        }
        public ADSGroupInfo GetGroupInfo(ADSGroupInfoHistory _history)
        {
            ADSGroupInfo adsGroupInfo = new ADSGroupInfo();

            adsGroupInfo.GroupId = _history.GroupId;
            adsGroupInfo.TenantId = _history.TenantId;
            adsGroupInfo.EndPointSystemId = _history.EndPointSystemId;
            adsGroupInfo.cn = _history.cn;
            adsGroupInfo.description = _history.description;
            adsGroupInfo.member = _history.member;
            adsGroupInfo.distinguishedName = _history.distinguishedName;
            adsGroupInfo.instanceType = _history.instanceType;
            adsGroupInfo.whenCreated = _history.whenCreated;
            adsGroupInfo.whenChanged = _history.whenChanged;
            adsGroupInfo.uSNCreated = _history.uSNCreated;
            adsGroupInfo.uSNChanged = _history.uSNChanged;
            adsGroupInfo.name = _history.name;
            adsGroupInfo.objectGUID = _history.objectGUID;
            adsGroupInfo.objectSid = _history.objectSid;
            adsGroupInfo.sAMAccountName = _history.sAMAccountName;
            adsGroupInfo.sAMAccountType = _history.sAMAccountType;
            adsGroupInfo.groupType = _history.groupType;
            adsGroupInfo.objectCategory = _history.objectCategory;
            adsGroupInfo.nTSecurityDescriptor = _history.nTSecurityDescriptor;
            adsGroupInfo.CreatedBy = _history.CreatedBy;
            adsGroupInfo.CreatedDate = _history.CreatedDate;
            adsGroupInfo.LastModifiedBy = _history.LastModifiedBy;
            adsGroupInfo.LastModifiedDate = _history.LastModifiedDate;

            return adsGroupInfo;
        }

        public bool AddADSConfiguration(ADSConfiguration _adsconfigdto)
        {
            try
            {

                //  return ActiveDirectoryDA.AddADSConfiguration(_adsconfigdto);

                return false;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<ADSUserInfo> GetAdsUsers(int DIUserId)
        {
            return ActiveDirectoryDA.GetAdsUserInfo(DIUserId);
        }

        public ADSUserInfo GetADUserByUserId(int userId)
        {
            return ActiveDirectoryDA.GetADUserByUserId(userId);
        }

        public ADSUserInfo GetADUserByCN(string cn, int endpointSystemId)
        {
            return ActiveDirectoryDA.GetADUserByCN(cn, endpointSystemId);
        }

        public List<ADSGroupInfo> GetADGroupsByUserId(int userId, int? endpointSystemId)
        {
            return ActiveDirectoryDA.GetADGroupsByUserId(userId, endpointSystemId);
        }

        public List<ADSUserInfo> GetPrimaryADUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                return ActiveDirectoryDA.GetPrimaryADUserByDIUserId(DIUserId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetPrimaryADAccountByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                return ActiveDirectoryDA.GetPrimaryADAccountByDIUserId(DIUserId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public List<ADSUserInfo> GetADUserByDIUserId(int DIUserId)
        {
            try
            {
                return ActiveDirectoryDA.GetADUserByDIUserId(DIUserId);
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetLockedADUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                var adPriUser = ActiveDirectoryDA.GetLockedADUserByDIUserId(DIUserId, endpointSystemId, true);
                if (adPriUser == null)
                {
                    return ActiveDirectoryDA.GetLockedADUserByDIUserId(DIUserId, endpointSystemId, false); ;
                }
                return adPriUser;
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetADSUserInfoByName(string userName)
        {
            return ActiveDirectoryDA.GetADSUserInfoByName(userName);
        }

        public ADSUserInfo GetADSPrimaryUserInfoByName(string userName)
        {
            return ActiveDirectoryDA.GetADSPrimaryUserInfoByName(userName);
        }

        public ADSUserInfo GetADSUserInfo(int EndpointUserId)
        {
            return ActiveDirectoryDA.GetADSUserInfo(EndpointUserId);
        }

        public IList GetColumnsList(string TableName)
        {
            try
            {
                return ActiveDirectoryDA.GetColumnsList(TableName);
            }
            catch (Exception)
            {
                throw;
            }

        }






        public void UpdateDiUserId(int UserId, int DiUserId, int EndPointSystemId, EndpointType EndPointSystype)
        {
            try
            {
                ActiveDirectoryDA.UpdateDiUserId(UserId, DiUserId, EndPointSystemId, EndPointSystype);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateDiUserIdWithMappingInfo(int UserId, int DiUserId, string MapTo, int EndPointSystemId, EndpointType EndPointSystype, string UserName, AccountType accountType)
        {
            try
            {
                ActiveDirectoryDA.UpdateDiUserIdWithMappingInfo(UserId, DiUserId, MapTo, EndPointSystemId, EndPointSystype, UserName, accountType);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UpdateUser(ADSUserInfo _user)
        {
            try
            {
                ActiveDirectoryDA.UpdateUser(_user);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public void DeleteAll<T>(string _tenantId, int _endpointSystemId)
        {
            try
            {
                ActiveDirectoryDA.DeleteAll<T>(_tenantId, _endpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSUserToGroupsReportData> GetADSUserToGroupsReportData(int EndPointSystemID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSUserToGroupsReportData(EndPointSystemID);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region New Logic

        public void ExecuteQry(string cmd)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();
            EndpointObjectDA.ExecuteQry(cmd);

        }
        public IList<ADSGroupInfo> GetAllADSGroupInfo()
        {
            try
            {
                return ActiveDirectoryDA.GetAllADSGroupInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSGroupInfo> GetAllADSGroupsByEndpointSystem(int EndpointSystemId)
        {
            try
            {
                return ActiveDirectoryDA.GetAllADSGroupsByEndpointSystem(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSOuInfo> GetAllADSOUInfo()
        {
            try
            {
                return ActiveDirectoryDA.GetAllADSOUInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<int, string> GetADSGroupInfo(string searchString)
        {
            try
            {
                return ActiveDirectoryDA.GetADSGroupInfo(searchString);

            }
            catch
            {
                throw;
            }
        }
        public IList<ADSGroupInfo> GetAllADSGroupInfo(string paramgroupType)
        {
            try
            {
                return ActiveDirectoryDA.GetAllADSGroupInfo(paramgroupType);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSGroupInfo> GetADSGroupInfoByEndpointSystemID(string paramgroupType, int EndpointSystemID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSGroupInfoByEndpointSystemID(paramgroupType, EndpointSystemID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSUserInfo> GetAllADUsers(int endpointId)
        {
            try
            {
                return ActiveDirectoryDA.GetAllADUsers(endpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ADSUserInfo getADUserByObjectSID(string paramObjectSID)
        {
            try
            {
                return ActiveDirectoryDA.getADUserByObjectSID(paramObjectSID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ADSGroupInfo getADGroupByObjectSID(string paramObjectSID)
        {
            try
            {
                return ActiveDirectoryDA.getADGroupByObjectSID(paramObjectSID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUsers(IList<ADSUserInfo> list)
        {
            try
            {

                ADSUserInfo adexistingUser = null;
                foreach (ADSUserInfo usr in list)
                {

                    adexistingUser = ActiveDirectoryDA.GetUserBysAMAccountNameAndEndpoint(usr.sAMAccountName, usr.EndPointSystemId);
                    if (adexistingUser == null)
                    {
                        usr.Version = Convert.ToInt16(VersionNo.Available);
                        ActiveDirectoryDA.AddADsUser(usr);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Warning, string.Format("Reconciliation Exception: AD User {0} has been ADDED without any request made in the system", usr.displayName), DiscoveryObjectType.User, list.Count);
                    }

                    else
                    {
                        usr.IsLocked = adexistingUser.IsLocked;
                        usr.LockScenario = adexistingUser.LockScenario;
                        usr.IsAccountDisabled = adexistingUser.IsAccountDisabled;
                        usr.TenantId = adexistingUser.TenantId;
                        usr.ExUserId = adexistingUser.ExUserId;
                        usr.UserId = adexistingUser.UserId;
                        usr.DIUserId = adexistingUser.DIUserId;
                        usr.MappingType = adexistingUser.MappingType;
                        usr.AccountType = adexistingUser.AccountType;
                        usr.IfUnmappedMapTo = adexistingUser.IfUnmappedMapTo;
                        usr.IsHomeFolderPermissionSetFailed = adexistingUser.IsHomeFolderPermissionSetFailed;
                        usr.CreatedDate = adexistingUser.CreatedDate;
                        usr.CreatedBy = adexistingUser.CreatedBy;
                        usr.Version = adexistingUser.Version;
                        ActiveDirectoryDA.UpdateADsUser(usr);
                    }
                    reconcilationFacade.AddOrUpdateAccountInReconTable(usr, adexistingUser == null ? true : false);
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveGroups_OLDEF(IList<ADSGroupInfo> list)
        {
            try
            {
                ActiveDirectoryDA.SaveGroups(list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveGroups(IList<ADSGroupInfo> list)
        {
            try
            {

                ADSGroupInfo adexistingGroup = null;
                foreach (ADSGroupInfo group in list)
                {

                    adexistingGroup = ActiveDirectoryDA.GetGroupBycnAndEndpoint(group.cn, group.EndPointSystemId);
                    if (adexistingGroup == null)
                    {
                        group.CanRequest = true;
                        ActiveDirectoryDA.AddADsGroup(group);

                    }

                    else
                    {
                        group.GroupId = adexistingGroup.GroupId;
                        group.CanRequest = adexistingGroup.CanRequest;
                        group.CreatedDate = adexistingGroup.CreatedDate;
                        group.CreatedBy = adexistingGroup.CreatedBy;
                        ActiveDirectoryDA.UpdateADsGroup(group);
                    }
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> GetADSGroupInfo(int EndPointID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSGroupInfo(EndPointID);
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
                ActiveDirectoryDA.SaveUserGroupMap(dicUsers, dicGroups, dicArray, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> getUserIds(int EndpointId)
        {
            try
            {
                return ActiveDirectoryDA.getUserIds(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveADsUser(IList<ADSUserInfo> listDelUser)
        {
            try
            {
                foreach (ADSUserInfo user in listDelUser)
                {
                    ActiveDirectoryDA.RemoveADsUser(user);

                    reconcilationFacade.DeleteAccount(user.EndPointSystemId, user.UserId);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<ADSUserInfo> GetDeletedUsers(int EndpointId, Process discoveryProcess)
        {
            try
            {
                return ActiveDirectoryDA.GetDeletedUsers(EndpointId, discoveryProcess);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveADsGroup(IList<ADSGroupInfo> listDelGroups)
        {
            try
            {
                foreach (ADSGroupInfo group in listDelGroups)
                {
                    ActiveDirectoryDA.RemoveADsGroup(group);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<ADSGroupInfo> GetDeletedGroups(int EndpointId)
        {
            try
            {
                return ActiveDirectoryDA.GetDeletedGroups(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        public IList<ADSUserInfoHistory> GetADSHistoryUsersEndpointSystemID(int EndPointSystemID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSHistoryUsersEndpointSystemID(EndPointSystemID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<EndpointObjects> GetADObjects()
        {
            try
            {
                return ActiveDirectoryDA.GetADObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ADSUsersReportData> GetADSUsersReportDataByEndpointSystem(int EndpointsystemID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSUsersReportDataByEndpointSystem(EndpointsystemID);
            }
            catch (Exception)
            {
                throw;
            }
        }



        public IList<ADSOUsReportData> GetADSOUInfoReportDataByEndPointSystemID(int EndPointSystemID)
        {
            try
            {
                return ActiveDirectoryDA.GetADSOUInfoReportDataByEndPointSystemID(EndPointSystemID);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ADSUserInfo> GetUsersInternetAccess()
        {

            try
            {
                return ActiveDirectoryDA.GetUsersInternetAccess();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ADSUserInfo> GetHomeFolderAccessAssignmentFailedUsers()
        {
            try
            {
                return ActiveDirectoryDA.GetHomeFolderAccessAssignmentFailedUsers();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CheckADUserNameExist(string samaAccountName)
        {
            try
            {
                return ActiveDirectoryDA.CheckADUserNameExist(samaAccountName);
            }
            catch
            {
                throw;
            }
        }

        public IList<TableProperties> GetColumnList(string TableName)
        {
            try
            {
                return ActiveDirectoryDA.GetColumnList(TableName);
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetPrimaryADUserByDIUserId(int diuserId)
        {
            return ActiveDirectoryDA.GetPrimaryADUserByDIUserId(diuserId);
        }

        public List<ADSUserInfo> GetAllUsersByGroup(string GroupCN, int EndpointSystemId)
        {
            try
            {
                return ActiveDirectoryDA.GetAllUsersByGroup(GroupCN, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ADSGroupInfo GetGroupBycn(string cn)
        {
            try
            {
                return ActiveDirectoryDA.GetGroupBycn(cn);
            }
            catch (Exception)
            {
                throw;
            }
        }


       
    }
}

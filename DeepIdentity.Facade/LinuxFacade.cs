using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;

using System.Data;
using System.Reflection;
using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.Facade
{
    public class LinuxFacade
    {

        public LinuxDA LinuxDA { get; set; }
        EndpointObjectDA EndpointObjectDA = null;


        public LinuxFacade()
        {
            LinuxDA = new LinuxDA();
            EndpointObjectDA = new EndpointObjectDA();

        }


        public IList<EndpointObjects> GetLinuxObjects()
        {
            try
            {
                return LinuxDA.GetLinuxObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LinuxUserInfo> GetAllUnMappedAdsUsers(EndPoint EndPoint)
        {
            try
            {
                return LinuxDA.GetAllUnMappedLinuxUsers(EndPoint);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UpdateAdsUserInfoOnAttributeMap(DataTable dtLinuxUser, EndPoint EndPoint)
        {
            try
            {
                LinuxDA.UpdateUserInfoBatchOnAttributeMap(dtLinuxUser, EndPoint);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public LinuxUserInfo GetLinuxUserByUserId(int userId)
        {
            return LinuxDA.GetLinuxUserByUserId(userId);
        }


        public IList<AttributeMappingColumns> GetAttributeMappingColumns(int EndPointSystemID)
        {
            try
            {

                IList<AttributeMappingColumns> attributemapping = LinuxDA.GetAttributeMappingColumns(EndPointSystemID);

                return attributemapping;
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



        public IList<LinuxUserInfo> GetAllLinuxUsers(IList<LinuxUserInfoHistory> list)
        {
            var listUser = list.IterateConvert<LinuxUserInfoHistory, LinuxUserInfo>(
                   delegate(LinuxUserInfoHistory _user)
                   {
                       return this.GetUserInfo(_user);
                   });

            return listUser.ToList();

        }
        private LinuxUserInfo GetUserInfo(LinuxUserInfoHistory _history)
        {
            LinuxUserInfo linuxUserInfo = new LinuxUserInfo();

            linuxUserInfo.UserId = _history.UserId;
            linuxUserInfo.TenantId = _history.TenantId;
            linuxUserInfo.EndPointSystemId = _history.EndPointSystemId;
            linuxUserInfo.LinuxUserId = _history.LinuxUserId;
            linuxUserInfo.UserName = _history.UserName;
            linuxUserInfo.Password = _history.Password;
            linuxUserInfo.GroupId = _history.GroupId;
            linuxUserInfo.UserIdInfo = _history.UserIdInfo;
            linuxUserInfo.HomeDirectory = _history.HomeDirectory;
            linuxUserInfo.CommandShell = _history.CommandShell;
            linuxUserInfo.DIUserId = _history.DIUserId;
            linuxUserInfo.CreatedBy = _history.CreatedBy;
            linuxUserInfo.CreatedDate = _history.CreatedDate;
            linuxUserInfo.LastModifiedBy = _history.LastModifiedBy;
            linuxUserInfo.LastModifiedDate = _history.LastModifiedDate;
            linuxUserInfo.IsLocked = false;
            linuxUserInfo.AccountType = AccountType.User;



            return linuxUserInfo;
        }
        private LinuxGroupInfo GetGroupInfo(LinuxGroupInfoHistory _history)
        {
            LinuxGroupInfo linuxGroupInfo = new LinuxGroupInfo();

            linuxGroupInfo.Id = _history.Id;
            linuxGroupInfo.TenantId = _history.TenantId;
            linuxGroupInfo.EndPointSystemId = _history.EndPointSystemId;
            linuxGroupInfo.GroupName = _history.GroupName;
            linuxGroupInfo.GroupPassword = _history.GroupPassword;
            linuxGroupInfo.GroupNumber = _history.GroupNumber;
            linuxGroupInfo.CreatedBy = _history.CreatedBy;
            linuxGroupInfo.CreatedDate = _history.CreatedDate;
            linuxGroupInfo.LastModifiedBy = _history.LastModifiedBy;
            linuxGroupInfo.LastModifiedDate = _history.LastModifiedDate;

            return linuxGroupInfo;
        }
        private LinuxResourceInfo GetResourceInfo(LinuxResourceInfoHistory _history)
        {
            LinuxResourceInfo linuxResourceInfo = new LinuxResourceInfo();


            linuxResourceInfo.Resource_Id = 1;
            linuxResourceInfo.TenantId = _history.TenantId;
            linuxResourceInfo.EndPointSystemId = _history.EndPointSystemId;
            linuxResourceInfo.Resource_Name = _history.Resource_Name;
            linuxResourceInfo.Resource_Type = _history.Resource_Type;
            linuxResourceInfo.Resource_User = _history.Resource_User;
            linuxResourceInfo.Resource_Group = _history.Resource_Group;
            linuxResourceInfo.Resource_Parent = _history.Resource_Parent;
            linuxResourceInfo.Resource_Level = _history.Resource_Level;
            linuxResourceInfo.Resouce_AccessRights = _history.Resouce_AccessRights;
            linuxResourceInfo.Resouce_Size = _history.Resouce_Size;
            linuxResourceInfo.Resouce_LastModified = _history.Resouce_LastModified;
            linuxResourceInfo.MountDrive = _history.MountDrive;
            linuxResourceInfo.CreatedBy = _history.CreatedBy;
            linuxResourceInfo.CreatedDate = _history.CreatedDate;
            linuxResourceInfo.LastModifiedBy = _history.LastModifiedBy;
            linuxResourceInfo.LastModifiedDate = _history.LastModifiedDate;

            return linuxResourceInfo;
        }
        public void AddLinuxUserGroup(IList<LinuxUserGroup> _list)
        {
            try
            {
               
                this.DeleteAll<LinuxUserGroup>(_list.First().TenantId, _list.First().EndpointSystemId);

                LinuxDA.AddLinuxUserGroupBulk(_list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux UserGroup objects", _list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddLinuxUserGroup(LinuxUserInfo linuxUserValue)
        {
            try
            {

                //this.DeleteAll<LinuxUserGroup>(_list.First().TenantId, _list.First().EndpointSystemId);

                LinuxDA.AddLinuxUserGroupBulk(linuxUserValue);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux UserGroup objects", linuxUserValue.Groups.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void AddLinuxGroupInfo(IList<LinuxGroupInfoHistory> list, EndPoint endPoint)
        {
            try
            {
                LinuxDA.AddLinuxGroupInfoHistoryBulk(list,endPoint);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux GroupHistory objects", list.Count.ToString()));

                var listUser = list.IterateConvert<LinuxGroupInfoHistory, LinuxGroupInfo>(
                   delegate(LinuxGroupInfoHistory _group)
                   {
                       return this.GetGroupInfo(_group);
                   });

              //  this.DeleteAll<LinuxGroupInfo>(list.First().TenantId, list.First().EndPointSystemId);

                LinuxDA.AddLinuxGroupInfoBulk(listUser, endPoint);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux Group objects", list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }

        }
        public void AddLinuxUserInfo(IList<LinuxUserInfoHistory> list, EndPoint _endpoint)
        {
            try
            {
                LinuxDA.AddLinuxUserInfoHistoryBulk(list);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux UserHistory objects", list.Count.ToString()));

                var listUser = list.IterateConvert<LinuxUserInfoHistory, LinuxUserInfo>(
                   delegate(LinuxUserInfoHistory _user)
                   {
                       return this.GetUserInfo(_user);
                   });
                LinuxDA.AddLinuxUserInfoBulk(listUser,_endpoint);
              


                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux User objects", list.Count.ToString()));


            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteResourceInfo(string tenantId, int endpointId)
        {
            ActiveDirectoryDA acDA = new ActiveDirectoryDA();

            acDA.DeleteAll<LinuxResourceInfo>(tenantId, endpointId);
            
            Log.General.WriteMessage(LogLevel.INFO, string.Format("Delete All Resources in Linux"));

        }

        public void AddLinuxResourceInfo(IList<LinuxResourceInfoHistory> list, EndPoint _endpoint)
        {
            try
            {
     

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux ResourceHistory(s) objects", list.Count.ToString()));

                var listResource = list.IterateConvert<LinuxResourceInfoHistory, LinuxResourceInfo>(
                   delegate(LinuxResourceInfoHistory _user)
                   {
                       return this.GetResourceInfo(_user);
                   });


                LinuxDA.AddLinuxResourceInfoBulk(listResource);

                Log.General.WriteMessage(LogLevel.INFO, string.Format("Added {0} Linux Resource(s) objects", list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool AddLinuxConfiguration(LinuxConfiguration _linuxsconfiguration)
        {
            try
            {
                return LinuxDA.AddLinuxConfiguration(_linuxsconfiguration);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void UpdateLinuxConfiguration(LinuxConfiguration _linuxsconfiguration)
        {
            try
            {
                LinuxDA.UpdateLinuxConfiguration(_linuxsconfiguration);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public void AddAllUnmapperUsers(int _endpointSystemId, int _processId)
        //{
        //    try
        //    {
        //        EndpointObjectDA.AddAllUnmappedUsers(Constants.LINUX, _endpointSystemId, _processId);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
      
        #region UserListing methods
        public IList<LinuxUserInfo> GetLinuxUsersByDIUserID(int DIUserID, int EndpointId=0)
        {
            return LinuxDA.GetLinuxUserInfoByDIUserId(DIUserID ,EndpointId);
        }

        public IList<LinuxUserInfo> GetLinuxUsersUserID(int DIUserID)
        {
            return LinuxDA.GetLinuxUsersByDIUserID(DIUserID);
        }

        public IList<string> GetLinuxUserGroupsByDIUserID(int DIUserID, int endPointSystemId)
        {
            return LinuxDA.GetLinuxUserGroupsByDIUserID(DIUserID, endPointSystemId);
        }
        #endregion


        public void DeleteAll<T>(string _tenantId, int _endpointSystemId)
        {
            try
            {
                ActiveDirectoryDA acDA = new ActiveDirectoryDA();

                acDA.DeleteAll<T>(_tenantId, _endpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Newly Added

        public void ExecuteQry(string cmd)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();
            EndpointObjectDA.ExecuteQry(cmd);

        }



        public LinuxUserInfo GetPrimaryLINUXUserByIdorDIID(int UserId, int EndpointId = 0)
        {
            try
            {
                return LinuxDA.GetPrimaryLINUXUserByIdorDIID(UserId, EndpointId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void UpdateUser(LinuxUserInfo endpointUser)
        {
            try
            {
                LinuxDA.UpdateUser(endpointUser);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ChecklinuxUserNameExists(string userName, int EndPointId)
        {
            try
            {
              return  LinuxDA.ChecklinuxUserNameExists(userName, EndPointId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<LinuxGroupInfo> GetAllGroups()
        {
            try
            {
                return LinuxDA.GetAllGroups();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<LinuxUserInfo> GetLinuxUsers(int userId)
        {
            try
            {
                return LinuxDA.GetAllusers(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<LinuxUserInfo> GetAllLinuxUsersReport()
        {
            try
            {
                return LinuxDA.GetAllLinuxUsersReport();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public object GetLinuxUserToGroupsReportData()
        {
            try
            {
                return LinuxDA.GetLinuxUserToGroupsReportData();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public object GetLinuxGrouptoUserReportData()
        {
            try
            {
                return LinuxDA.GetLinuxGrouptoUserReportData();
            }
            catch (Exception)
            {

                throw;
            }
        }



        public List<LinuxUserInfo> GetLINUXGroupsByUserId(int endpointUserId, int endpointSystemId)
        {
            return LinuxDA.GetLINUXGroupsByUserId(endpointUserId, endpointSystemId);
        }

        public void DeleteLinuxUserGroup(LinuxUserInfo linuxUserValue)
        {
            try
            {
                LinuxDA.DeleteLinuxUserGroup(linuxUserValue);
            }
            catch (Exception)
            { }
        }

        public  IList<LinuxGroupInfo> GetLINUXGroups(int userId, int EndPointSystemId)
        {
            try
            {
              return  LinuxDA.GetLINUXGroups(userId, EndPointSystemId);
            }
            catch (Exception)
            {
              //  throw;            
                return new List<LinuxGroupInfo>();
            }
        }

        public void GetAllMasterUsers(IList<LinuxUserInfo> listLinuxUser, bool mapAll) //public void GetAllMasterUsers(IList<IAggregateRoot> listLinuxUser, bool mapAll)
        {
            int EndpointID = listLinuxUser.FirstOrDefault().EndPointSystemId;
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

            listTrustedConfig = list.Where(li => li.TrustedSource == EndpointType.Linux).ToList();

            IList<AttributeMappingColumns> listAttributes = new LinuxDA().GetAttributeMappingColumns(EndpointID);

            if (listTrustedConfig.Count <= 0)
            {
                Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Populate User: No attribute is configured from LINUX"));
                return;
            }

            if (listAttributes.Count <= 0)
            {
                Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Populate User: Attribute Mapping is NOT configured for LINUX"));
                return;
            }

            Type usertype = typeof(User);

            IList<PropertyInfo> userProperties = usertype.GetProperties().ToList();

            string tenantId = string.Empty;

            if (listLinuxUser.Count > 0)
            {
                tenantId = listLinuxUser.First().TenantId;
            }
            IList<User> listUsers = new List<User>();

            if (tenantId != string.Empty)
                listUsers = new UserDA().GetAllUsers(tenantId);
            else
                listUsers = new UserDA().GetAllUsers();

            DataTable dtMaster = CollectionHelper.ConvertToNormalTable<User>(listUsers);

            DataTable dtAdsUser = CollectionHelper.ConvertToNormalTable<LinuxUserInfo>(listLinuxUser);


            GeneralDA genDA = new GeneralDA();

            newRowCount = genDA.GetMaxId("DI_M_User", "UserId");

            foreach (DataRow adsRow in dtAdsUser.Rows)
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



            new UserDA().UpdateMasterLatest(dtMaster, tenantId);

            Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Global users populated"));

        }

        public  int AddLinuxUser(LinuxUserInfo LinuxUser)
        {
           return LinuxDA.AddLinuxUser(LinuxUser);
        }

        public LinuxUserInfo GetLockedLinuxUserByDIUserId(int UserId, int EndpointSystemId)
        {
            return LinuxDA.GetLockedLinuxUserByDIUserId(UserId, EndpointSystemId);
        }
    }
}

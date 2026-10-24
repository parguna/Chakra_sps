using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;
using DI.Chakra.Infrastructure;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Logger;



namespace DI.Chakra.DataAccess
{
    public class SAPEPDA
    {


        private QueryExecutor m_executor = null;

        private IACMDbOperator m_iacmOperator = null;
        private IACMDBContext _context;

        public SAPEPDA()
        {
            _context = new IACMDBContext();
        }

        public SAPEPConfiguration GetConfiguration(int _systemid)
        {
            SAPEPConfiguration config = null;
            config = _context.SapEpConfiguration.FirstOrDefault(s => s.EndPointSystemId == _systemid);
            config.Password = RSAHelper.DecryptData(config.Password);
            return config;
        }

        public class SAPEPUserGroups : IAggregateRoot
        {
            public long SAPEPGroupInfo_Id { get; set; }
            public int SAPEPUserInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int Version { get; set; }
        }

        public class SAPEPUserRoles : IAggregateRoot
        {
            public long SAPEPRoleInfo_Id { get; set; }
            public int SAPEPUserInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int Version { get; set; }

        }



        public IList<SAPEPUserInfo> GetSAPEPUserInfoByDIUserId(long UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from user in _context.SAPEPUserInfos
                                 .Include("EndPointSystem")
                                 .Include("EndPointSystem.EndPointSystemType")
                            where user.DIUserId == UserId && user.IsLocked == false
                            orderby user.CreatedDate
                            select user).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPEPUser By DIUserId ", ex);
                throw ex;
            }
        }

        public List<SAPEPUserInfo> GetPrimarySAPEPUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.Where(item => item.DIUserId == DIUserId && (item.AccountType == AccountType.User ||
                        item.AccountType == AccountType.Secondary) && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All sapep DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public SAPEPUserInfo GetSAPEPUserByUserId(long userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.FirstOrDefault(item => item.Id == userId && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void GetSAPEPUserGroup(long UserId, out IList<SAPEPGroupInfo> GrpList, out IList<SAPEPRoleInfo> RoleList)
        {

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPEPUserInfo aSAPEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(usr => usr.Id == UserId);
                    GrpList = new List<SAPEPGroupInfo>();
                    RoleList = new List<SAPEPRoleInfo>();
                    if (aSAPEPUser != null)
                    {
                        GrpList = aSAPEPUser.SAPEPGroups.Where(item => item.IsXLUploaded).ToList();
                        RoleList = aSAPEPUser.SAPEPRoles.Where(item => item.IsXLUploaded).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPEPUser By DIUserId ", ex);
                throw ex;
            }
        }

        public void GetSAPEPUserGroupByUserId(long UserId, out IList<SAPEPGroupInfo> GrpList, out IList<SAPEPRoleInfo> RoleList)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPEPUserInfo aSAPEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(usr => usr.Id == UserId);
                    GrpList = new List<SAPEPGroupInfo>();
                    RoleList = new List<SAPEPRoleInfo>();
                    if (aSAPEPUser != null)
                    {
                        GrpList = aSAPEPUser.SAPEPGroups.Where(item => item.IsXLUploaded).ToList();
                        RoleList = aSAPEPUser.SAPEPRoles.Where(item => item.IsXLUploaded).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPEP User By UserId ", ex);
                throw ex;
            }
        }



        #region  Provisioning Methods
        public bool UpdateSAPEPUserInfoFromProvisioning(SAPEPUserInfo sapEPUserInfo)
        {
            bool isDataSaved = false;
            if (sapEPUserInfo == null)
                return isDataSaved;

            //string query = "DI_spUpdateSAPEPUserInfoFromProvisioning";
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<SAPEPUserInfo> userList = _context.SAPEPUserInfos.Where(usr => usr.Id == sapEPUserInfo.Id).ToList();

                    foreach (SAPEPUserInfo userItem in userList)
                    {
                        userItem.FirstName = sapEPUserInfo.FirstName;
                        userItem.LastName = sapEPUserInfo.LastName;
                        userItem.JobTitle = sapEPUserInfo.JobTitle;
                        userItem.Email = sapEPUserInfo.Email;
                        userItem.Department = sapEPUserInfo.Department;
                        userItem.LastModifiedBy = string.IsNullOrEmpty(sapEPUserInfo.LastModifiedBy) ? "System" : sapEPUserInfo.LastModifiedBy;
                        userItem.LastModifiedDate = sapEPUserInfo.LastModifiedDate;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while updating  SAPEPUserInfo From Provisioning.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public Dictionary<string, string> GetSAPEPRoleUniqueNamesDictionary()
        {
            Dictionary<string, string> roleDict = new Dictionary<string, string>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //string query = "DI_spGetGetSAPEPRoleUniqueNames";
                    string tmpRoleId;
                    string tmpRoleFullName;

                    IList<SAPEPRoleInfoTemp> SAPEPRoleInfoTempList = new List<SAPEPRoleInfoTemp>();
                    SAPEPRoleInfoTempList = _context.SAPEPRoleInfoTemps.ToList();

                    foreach (SAPEPRoleInfoTemp listItem in SAPEPRoleInfoTempList)
                    {
                        tmpRoleId = CheckNull.AgainstString(listItem.RoleUniqueName);
                        tmpRoleFullName = CheckNull.AgainstString(listItem.RoleFullName);
                        if (!string.IsNullOrEmpty(tmpRoleId) && !roleDict.ContainsKey(tmpRoleId))
                        {
                            roleDict.Add(tmpRoleId, tmpRoleFullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting RoleID Name Dictionary", ex);
                throw ex;
            }
            return roleDict;
        }
        #endregion

        public IList<SAPEPUserInfo> GetAllSAPEPUsers()
        {
            IList<SAPEPUserInfo> _list = new List<SAPEPUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    _list = _context.SAPEPUserInfos.Include("EndPointSystem").ToList();
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting  SAPEP Users ", ex);
                throw ex;
            }
        }

        public IList<SAPEPUserInfo> GetAllSAPEPUsersByEndPoint(long EndpointID)
        {
            IList<SAPEPUserInfo> _list = new List<SAPEPUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    _list = _context.SAPEPUserInfos.Where(e => e.EndPointSystemId == EndpointID).Include("EndPointSystem").ToList();
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting  SAPEP Users ", ex);
                throw ex;
            }
        }

        public IList<SAPEPUserInfoReportData> GetAllSAPEPUsersReportData(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from users in _context.SAPEPUserInfos
                            where users.EndPointSystemId == EndpointSystemId
                            select new SAPEPUserInfoReportData
                            {
                                UserId = users.Id,
                                UserName = users.UserName,
                                FirstName = users.FirstName,
                                LastName = users.LastName,
                                Salutation = users.Salutation,
                                DisplayName = users.DisplayName,
                                StartDate = users.StartDate,
                                Title = users.Title,
                                Company = users.Company,
                                IsLocked = users.IsLocked ? "YES" : "NO",
                                LockScenario = users.LockScenario,
                                EndDate = users.EndDate,
                                Email = users.Email,
                                Department = users.Department,
                                DataSource = users.UniqueId.Contains("USER.R3_DATASOURCE") ? "ABAP" : "UME Database",
                                IsMapped = (users.DIUserId != null) ? "Yes" : "No",
                                Owner = (users.DIUserId != null) ? _context.Users.FirstOrDefault(usrs => usrs.Id == users.DIUserId).DisplayName : ""
                            }).ToList();
                }

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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from groups in _context.SAPEPGroupInfos
                            where groups.EndPointSystemId == EndpointSystemId && groups.IsXLUploaded == IsXLUploaded
                            select groups).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from roles in _context.SAPEPRoleInfos
                            where roles.EndPointSystemId == EndpointSystemId && roles.IsXLUploaded == IsXLUploaded
                            select roles).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapepUser in _context.SAPEPUserInfos
                            from sapepGrp in sapepUser.SAPEPGroups
                            where (!string.IsNullOrEmpty(sapepGrp.GroupName) && sapepUser.EndPointSystemId == EndpointSystemId)
                            select new SAPEPUserToGroupsReportData
                            {
                                UserId = sapepUser.Id,
                                UserName = sapepUser.UserName,
                                DisplayName = sapepUser.DisplayName,
                                InternetAddress = sapepUser.Email,
                                Department = sapepUser.Department,
                                GroupId = sapepGrp.Id,
                                GroupName = sapepGrp.GroupName,
                                GroupDescription = sapepGrp.GroupDescription
                            }
                            ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<SAPEPGroupToUsersReportData> GetSAPEPGroupToUsersReportData(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapepGrp in _context.SAPEPGroupInfos
                            from sapepUser in sapepGrp.SAPEPUserInfos
                            where (!string.IsNullOrEmpty(sapepUser.UserName) && sapepGrp.EndPointSystemId == EndpointSystemId)
                            select new SAPEPGroupToUsersReportData
                            {
                                GroupId = sapepGrp.Id,
                                GroupName = sapepGrp.GroupName,
                                GroupDescription = sapepGrp.GroupDescription,

                                UserId = sapepUser.Id,
                                UserName = sapepUser.UserName,
                                DisplayName = sapepUser.DisplayName,
                                InternetAddress = sapepUser.Email,
                                Department = sapepUser.Department,
                                IACMPasswordChangeDate = sapepUser.IACMLastPasswordChange
                            }
                            ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<SAPEPUserToRolesReportData> GetSAPEPUserToRolesReportData(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapepUser in _context.SAPEPUserInfos
                            from sapepRle in sapepUser.SAPEPRoles
                            where (!string.IsNullOrEmpty(sapepRle.RoleName) && sapepUser.EndPointSystemId == EndpointSystemId)
                            select new SAPEPUserToRolesReportData
                            {
                                UserId = sapepUser.Id,
                                UserName = sapepUser.UserName,
                                DisplayName = sapepUser.DisplayName,
                                InternetAddress = sapepUser.Email,
                                Department = sapepUser.Department,

                                RoleId = sapepRle.Id,
                                RoleName = sapepRle.RoleName,
                                RoleDescription = sapepRle.RoleDescription
                            }
                            ).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapepRle in _context.SAPEPRoleInfos
                            from sapepUser in sapepRle.SAPEPUserInfos
                            where (!string.IsNullOrEmpty(sapepUser.UserName) && sapepRle.EndPointSystemId == EndpointSystemId)
                            select new SAPEPRoleToUsersReportData
                            {
                                RoleId = sapepRle.Id,
                                RoleName = sapepRle.RoleName,
                                RoleDescription = sapepRle.RoleDescription,

                                UserId = sapepUser.Id,
                                UserName = sapepUser.UserName,
                                DisplayName = sapepUser.DisplayName,
                                InternetAddress = sapepUser.Email,
                                Department = sapepUser.Department,
                                IACMPasswordChangeDate = sapepUser.IACMLastPasswordChange
                                //IsCompanyUser = sapepUser.IsCompanyUser ? "YES" : "NO",
                            }
                            ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateUser(SAPEPUserInfo _sapepUser)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.SAPEPUserInfos.Attach(_sapepUser);
                    _context.Entry(_sapepUser).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }



        public IList<EndpointObjects> GetSAPEPObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => e.EndpointSystemTypeName == EndpointType.SAPEnterprisePortal).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All SAPEP EndpointObjects ", ex);
                throw ex;
            }
        }
        public SAPEPUserInfo GetUserByUserName(string userName, int paramEndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPUserInfos.FirstOrDefault(t => t.UserName == userName && t.EndPointSystemId == paramEndpointId);
            }
        }

        public SAPEPUserInfo GetUserByUserId(string userName, long paramEndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPUserInfos.FirstOrDefault(t => t.UserName == userName && t.EndPointSystemId == paramEndpointId);
            }
        }


        public void AddSAPEPUser(SAPEPUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPEPUserInfos.Add(user);
                _context.SaveChanges();
            }
        }

        public IList<SAPEPUserInfo> GetDeletedUsers(long EndpointId, Process discoveryProcess)
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPEPUserInfo> listDeletedUsers = new List<SAPEPUserInfo>();
                if (discoveryProcess != null)
                {
                    listDeletedUsers = _context.SAPEPUserInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < discoveryProcess.ProcessStartAt).ToList();
                }
                return listDeletedUsers;
            }
        }

        public IList<SAPEPUserInfoHistory> GetAllSapepHistorybyendpointid(int endpointsystemid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //IList<SAPEPUserInfo> listDeletedUsers = new List<SAPEPUserInfo>();
                    var item = _context.SAPEPUserInfoHistorys.Where(e => e.EndPointSystemId == endpointsystemid);
                    return item.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<SAPEPUserInfoHistory> GetAllSAPEPHistoryUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfoHistorys.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void RemoveSAPEPUser(SAPEPUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                SAPEPUserInfo sapepsuser = new SAPEPUserInfo();
                sapepsuser = _context.SAPEPUserInfos.First(u => u.Id == user.Id);
                _context.SAPEPUserInfos.Remove(sapepsuser);
                _context.SaveChanges();
            }
        }

        public Dictionary<long, string> getUserIds(long EndpointID)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPUserInfos.Where(e => e.EndPointSystemId == EndpointID).ToDictionary(item => (long)item.Id, item => item.UniqueName);
            }
        }

        public IList<SAPEPGroupInfo> GetDeletedGroups(int EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPEPGroupInfo> listDeletedGroups = new List<SAPEPGroupInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedGroups = _context.SAPEPGroupInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < process.ProcessStartAt && d.IsXLUploaded == false).ToList();
                    }
                }

                return listDeletedGroups;

            }
        }

        public IList<SAPEPRoleInfo> GetDeletedRoles(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPEPRoleInfo> listDeletedRoles = new List<SAPEPRoleInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedRoles = _context.SAPEPRoleInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < process.ProcessStartAt && d.IsXLUploaded == false).ToList();
                    }
                }

                return listDeletedRoles;

            }
        }

        public SAPEPUserInfo GetLockedSAPEPUserByDIUserId(int DIUserId, int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == EndpointSystemId
                            && item.AccountType == AccountType.User && item.IsLocked);
                }
            }
            catch
            {
                throw;
            }
        }

        public void RemoveSAPEPGroup(SAPEPGroupInfo sapepGroup)
        {
            using (_context = new IACMDBContext())
            {
                SAPEPGroupInfo _sapepGroup = new SAPEPGroupInfo();
                _sapepGroup = _context.SAPEPGroupInfos.First(u => u.Id == sapepGroup.Id);
                _context.SAPEPGroupInfos.Remove(_sapepGroup);
                _context.SaveChanges();
            }
        }

        public void RemoveSAPEPRole(SAPEPRoleInfo sapepRole)
        {
            using (_context = new IACMDBContext())
            {
                SAPEPRoleInfo _sapepRole = new SAPEPRoleInfo();
                _sapepRole = _context.SAPEPRoleInfos.First(u => u.Id == sapepRole.Id);
                _context.SAPEPRoleInfos.Remove(_sapepRole);
                _context.SaveChanges();
            }
        }

        public Dictionary<long, string> GetSAPEPGroupInfo(long EndpointID)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPEPGroupInfos.Where(e => e.EndPointSystemId == EndpointID).ToDictionary(item => (long)item.Id, item => item.GroupName);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetSAPEPRoleInfo()
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPEPRoleInfos.ToDictionary(item => (long)item.Id, item => item.RoleName);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, ArrayList> dicGroupUsers, long endpointSystemid)
        {
            IList<SAPEPUserGroups> listUG = new List<SAPEPUserGroups>();

            SAPEPUserGroups userGroup = null;
            SAPEPGroupInfo sapepGroup = null;

            using (_context = new IACMDBContext())
            {
                //_context.Database.ExecuteSqlCommand("Truncate table SAPEPUserInfoSAPEPGroupInfo");
                string query = string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0}", endpointSystemid);
                _context.Database.ExecuteSqlCommand(query);

                foreach (var user in dicGroupUsers)
                {
                    string UserName = user.ToString();

                    foreach (var group in user.Value)
                    {
                        userGroup = new SAPEPUserGroups();
                        string GroupName = group.ToString();
                        sapepGroup = _context.SAPEPGroupInfos.Where(ep => ep.EndPointSystemId == endpointSystemid).FirstOrDefault(G => G.GroupName == GroupName);
                        if (sapepGroup != null)
                        {
                            userGroup.SAPEPGroupInfo_Id = sapepGroup.Id;
                            userGroup.SAPEPUserInfo_Id = Convert.ToInt32(user.Key);
                            userGroup.EndpointSystemId = (int)endpointSystemid;
                            userGroup.Version = Convert.ToInt16(VersionNo.Available);
                            SAPEPUserGroups existingUserGroup = listUG.FirstOrDefault(usrgrp => usrgrp.SAPEPGroupInfo_Id == userGroup.SAPEPGroupInfo_Id
                                                            && usrgrp.SAPEPUserInfo_Id == userGroup.SAPEPUserInfo_Id);

                            if (existingUserGroup == null)
                                listUG.Add(userGroup);
                        }
                    }

                }
            }
            SQLBulkUploader<SAPEPUserGroups> sqlUserGroupMap = null;
            try
            {
                string tableName = "SAPEPUserInfoSAPEPGroupInfo";

                sqlUserGroupMap = new SQLBulkUploader<SAPEPUserGroups>(tableName);

                sqlUserGroupMap.Flush(listUG);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }

        }

        public void SaveUserRoleMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicRoles, Dictionary<string, ArrayList> dicRoleUsers, long endpointSystemid)
        {
            IList<SAPEPUserRoles> listUR = new List<SAPEPUserRoles>();
            SAPEPUserRoles userRole = null;
            SAPEPRoleInfo sapepRole = null;

            using (_context = new IACMDBContext())
            {
                //_context.Database.ExecuteSqlCommand("Truncate table SAPEPRoleInfoSAPEPUserInfo");
                string query = string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0}", endpointSystemid);
                _context.Database.ExecuteSqlCommand(query);

                foreach (var user in dicRoleUsers)
                {
                    string UserName = user.ToString();
                    foreach (var role in user.Value)
                    {
                        userRole = new SAPEPUserRoles();
                        string RoleName = role.ToString();
                        sapepRole = _context.SAPEPRoleInfos.Where(ep => ep.EndPointSystemId == endpointSystemid).FirstOrDefault(R => R.RoleName == RoleName);
                        if (sapepRole != null)
                        {
                            userRole.SAPEPRoleInfo_Id = sapepRole.Id;
                            userRole.SAPEPUserInfo_Id = Convert.ToInt32(user.Key);
                            userRole.EndpointSystemId = (int)endpointSystemid;
                            userRole.Version = Convert.ToInt16(VersionNo.Available);
                            SAPEPUserRoles existingUserRoles = listUR.FirstOrDefault(usrrole => usrrole.SAPEPRoleInfo_Id == userRole.SAPEPRoleInfo_Id
                                && usrrole.SAPEPUserInfo_Id == userRole.SAPEPUserInfo_Id);
                            if (existingUserRoles == null)
                                listUR.Add(userRole);
                        }
                    }

                }
            }

            SQLBulkUploader<SAPEPUserRoles> sqlUserRoleMap = null;
            try
            {
                string tableName = "SAPEPRoleInfoSAPEPUserInfo";

                sqlUserRoleMap = new SQLBulkUploader<SAPEPUserRoles>(tableName);

                sqlUserRoleMap.Flush(listUR);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserRoleMap.Close();
            }

        }

        public void AddSAPEPGroup(SAPEPGroupInfo group)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                oIACMDBContext.SAPEPGroupInfos.Add(group);
                oIACMDBContext.SaveChanges();
            }
        }

        public void UpdateSAPEPGroup(SAPEPGroupInfo group)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                oIACMDBContext.SAPEPGroupInfos.Attach(group);
                oIACMDBContext.Entry(group).State = EntityState.Modified;
                oIACMDBContext.SaveChanges();
            }
        }

        public void AddSAPEPRole(SAPEPRoleInfo role)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                oIACMDBContext.SAPEPRoleInfos.Add(role);
                oIACMDBContext.SaveChanges();
            }
        }

        public void UpdateSAPEPRole(SAPEPRoleInfo role)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                oIACMDBContext.SAPEPRoleInfos.Attach(role);
                oIACMDBContext.Entry(role).State = EntityState.Modified;
                oIACMDBContext.SaveChanges();
            }
        }

        public SAPEPGroupInfo getGroupByUniqueId(string groupId, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPGroupInfos.FirstOrDefault(t => t.GroupId == groupId && t.EndPointSystemId == EndpointSystemId);
            }
        }

        public SAPEPRoleInfo getRoleByUniqueId(string roleId, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPRoleInfos.FirstOrDefault(t => t.RoleId == roleId && t.EndPointSystemId == EndpointSystemId);
            }
        }

        public List<SAPEPGroupInfo> GetSAPEPGroupsByUserId(long UserId, int? endpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    if(endpointSystemId == null)
                    return _context.SAPEPUserInfos.FirstOrDefault(t => t.Id == UserId).SAPEPGroups.Where(item => item.IsXLUploaded).ToList();
                    else
                    {
                        return _context.SAPEPUserInfos.FirstOrDefault(t => t.Id == UserId && t.EndPointSystemId == endpointSystemId.Value).SAPEPGroups.Where(item => item.IsXLUploaded).ToList();
                        
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        public List<SAPEPRoleInfo> GetSAPEPRolesByUserId(long UserId, int? endpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    if(endpointSystemId == null)
                    return _context.SAPEPUserInfos.FirstOrDefault(t => t.Id == UserId).SAPEPRoles.Where(item => item.IsXLUploaded).ToList();
                    else
                        return _context.SAPEPUserInfos.FirstOrDefault(t => t.Id == UserId && t.EndPointSystemId == endpointSystemId.Value).SAPEPRoles.Where(item => item.IsXLUploaded).ToList();

                }
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
                using (_context = new IACMDBContext())
                {
                    var result = (from source in _context.SAPEPGroupInfos where source.EndPointSystemId == EndpointSystemId && source.IsXLUploaded select source);

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.GroupName))
                        {
                            data.Add(items.GroupUniqueName, items.GroupName);
                        }
                    }
                    return data;
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetGroupsByEndpointUser(long EndpointUserId, int? EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return (from user in _context.SAPEPUserInfos
                            from groups in user.SAPEPGroups
                            where user.Id == EndpointUserId && user.EndPointSystemId == EndpointSystemId
                            select groups)
                               .Select(group => new { key = group.Id, value = group.GroupName })
                               .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetRolesByEndpointUser(long EndpointUserId, int? EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return (from user in _context.SAPEPUserInfos
                            from roles in user.SAPEPRoles
                            where user.Id == EndpointUserId && user.EndPointSystemId == EndpointSystemId
                            select roles)
                            .Select(role => new { key = role.UniqueRoleName, value = role.RoleName })
                            .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
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
                using (_context = new IACMDBContext())
                {
                    var result = (from source in _context.SAPEPRoleInfos where source.EndPointSystemId == EndpointSystemId && source.IsXLUploaded select source);

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.RoleName))
                        {
                            data.Add(items.UniqueRoleName, items.RoleName);
                        }
                    }
                    return data;
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetSAPEPUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.SAPEPUserInfos.Where(item => item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType && item.DIUserId == userid))
                                                .Select(x => new { Key = x.Id, Value = x.UserName })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.SAPEPUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.SAPEPUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SAPEPUserInfo GetSAPEPUserByUserName(string userName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.FirstOrDefault(item => item.UserName == userName && item.EndPointSystemId == endpointSystemId);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var accReqEntry = _context.Entry(sapepUser);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<SAPEPUserInfo>();
                        SAPEPUserInfo attachedAccReq = set.Find(sapepUser.Id);

                        if (attachedAccReq != null)
                        {
                            var attachedEntry = _context.Entry(attachedAccReq);
                            attachedEntry.CurrentValues.SetValues(sapepUser);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.Any(item => item.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public static Dictionary<string, string> GetGroupSAPEPForRemove(long EndpointUserID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var result = _context.SAPEPUserInfos.FirstOrDefault(item => item.Id == EndpointUserID).SAPEPGroups.
                        Select(role => new { key = role.GroupUniqueName, value = role.GroupName }).OrderBy(item => item.value).Distinct().ToDictionary(di => di.key, di => di.value);

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.Value))
                        {
                            data.Add(items.Key, items.Value);
                        }
                    }

                    return data;
                }

            }
            catch
            {
                throw;
            }
        }

        public static Dictionary<string, string> GetRolesSAPEPForRemove(long EndpointUserID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                List<string> SapRoles = (from user in _context.SAPEPUserInfos
                                         where user.Id == EndpointUserID
                                         from roles in user.SAPEPRoles
                                         select roles.RoleName).ToList();

                var result = (from businessRole in _context.SAPEPRoleInfos
                              join tRoles in SapRoles on businessRole.RoleName equals tRoles
                              select businessRole)
                         .Select(role => new { key = role.UniqueRoleName, value = role.RoleName }).OrderBy(item => item.value)
                         .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);


                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (var items in result)
                {
                    if (!data.ContainsValue(items.Value))
                    {
                        data.Add(items.Key, items.Value);
                    }
                }

                return data;


            }
        }

        public IList<SAPEPGroupInfo> GetAllSAPEPGroupsByEndpointsystemid(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from groups in _context.SAPEPGroupInfos
                            where groups.EndPointSystemId == EndpointSystemId
                            select groups).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPEPRoleInfo> GetAllSAPEPRolesByEndpointSystemID(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from roles in _context.SAPEPRoleInfos
                            where roles.EndPointSystemId == EndpointSystemId
                            select roles).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<string, string> GetPortalGroups(int? EndpointSystemId, string SearchString)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    var result = (from source in _context.SAPEPGroupInfos where source.GroupName.Contains(SearchString) && source.EndPointSystemId == EndpointSystemId && source.IsXLUploaded select source);

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.GroupName))
                        {
                            data.Add(items.GroupUniqueName, items.GroupName);
                        }
                    }
                    return data;
                }
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
                using (_context = new IACMDBContext())
                {
                    var result = (from source in _context.SAPEPRoleInfos where source.RoleName.Contains(SearchString) && source.EndPointSystemId == EndpointSystemId && source.IsXLUploaded select source);

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.RoleName))
                        {
                            data.Add(items.UniqueRoleName, items.RoleName);
                        }
                    }
                    return data;
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<UserAccount> GetAccountDetail(long UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.DIUserId == UserID && sapepusr.IsLocked == false).ToList();
                foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                {
                    if (sapepusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = sapepusr.Id,
                            UserName = sapepusr.AccountName,
                            FullName = sapepusr.DisplayName,
                            AccountType = sapepusr.AccountType,
                            LastLogonDate = sapepusr.LastModifiedDate,
                            EndpointSystemId = sapepusr.EndPointSystemId,
                            EndPointSystemType = sapepusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapepusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(sapepusr => sapepusr.EndPointSystemId == ApplicationID && sapepusr.IsLocked == false).ToList();
                foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                {
                    if (sapepusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = sapepusr.Id,
                            UserName = sapepusr.AccountName,
                            FullName = sapepusr.DisplayName,
                            AccountType = sapepusr.AccountType,
                            LastLogonDate = sapepusr.LastModifiedDate,
                            EndpointSystemId = sapepusr.EndPointSystemId,
                            EndPointSystemType = sapepusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapepusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            SAPEPUserInfo SAPEPUserInfo = new SAPEPUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.SAPEPGroups).Include(lq => lq.SAPEPRoles).Where(sapepusr => sapepusr.Id == EndpointUserId).FirstOrDefault();
                if (SAPEPUserInfo == null)
                    return Listtobereturn;

                foreach (SAPEPGroupInfo grp in SAPEPUserInfo.SAPEPGroups)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = SAPEPUserInfo.Id;
                    UserEntitlement.UserName = SAPEPUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = SAPEPUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPEPUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.GroupName;
                        UserEntitlement.Description = grp.GroupDescription;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
                foreach (SAPEPRoleInfo role in SAPEPUserInfo.SAPEPRoles)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = SAPEPUserInfo.Id;
                    UserEntitlement.UserName = SAPEPUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = SAPEPUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPEPUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    if (role != null)
                    {
                        UserEntitlement.Name = role.RoleName;
                        UserEntitlement.Description = role.RoleDescription;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
            }
            return Listtobereturn;
        }

        public AccountDetail GetAccountDetailByEndPointUserID(long EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            SAPEPUserInfo SAPEPUserInfo = new SAPEPUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.Id == EndpointUserId).FirstOrDefault();
                if (SAPEPUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = SAPEPUserInfo.AccountName;
                AccountDetail.DisplayName = SAPEPUserInfo.DisplayName;
                AccountDetail.PhoneNo = SAPEPUserInfo.Cellphone;
                AccountDetail.Email = SAPEPUserInfo.Email;
                AccountDetail.AccountType = SAPEPUserInfo.AccountType;
                AccountDetail.IsMappedAccount = SAPEPUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = SAPEPUserInfo.LastModifiedDate;
                AccountDetail.Version = Convert.ToString(SAPEPUserInfo.Version);
                AccountDetail.DIUserID = SAPEPUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = SAPEPUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }

        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                SAPEPUserInfo SAPEPUInfo = Context.SAPEPUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (SAPEPUInfo != null)
                {
                    SAPEPUInfo.DIUserId = AccountDetail.DIUserID; ;
                    SAPEPUInfo.LastModifiedDate = DateTime.Today;
                    SAPEPUInfo.MappingType = "Manual";
                    SAPEPUInfo.AccountType = AccountDetail.AccountType;
                    SAPEPUInfo.IfUnmappedMapTo = AccountDetail.UnMappedMapTo;
                    Context.SaveChanges();
                }
            }
        }

        public SAPEPUserInfo GetSAPEPUserInfo(long EndPointUserID)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return Context.SAPEPUserInfos.Where(sapepusr => sapepusr.Id == EndPointUserID).FirstOrDefault();
            }
        }

        public SAPEPGroupInfo getGroupByName(string grpName, int EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPGroupInfos.FirstOrDefault(t => t.GroupName == grpName && t.EndPointSystemId == EndpointSystemId);
            }
        }

        public SAPEPRoleInfo getRoleByName(string rleName, int EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPEPRoleInfos.FirstOrDefault(t => t.RoleName == rleName && t.EndPointSystemId == EndpointSystemId);
            }
        }

    }
}

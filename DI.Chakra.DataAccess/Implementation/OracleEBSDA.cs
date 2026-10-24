#region Namespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

using System.Configuration;
using System.ComponentModel;

using DI.Chakra.Infrastructure;
using DI.Chakra.Model;

using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;


#endregion

namespace DI.Chakra.DataAccess
{

    public class OracleEBSDA
    {

        #region Private Variables

        private QueryExecutor executor = null;
        private IACMDbOperator iacmOperator = null;
        private IACMDBContext _context;

        #endregion

        #region Constructor

        public OracleEBSDA()
        {
            _context = new IACMDBContext();
        }

        #endregion

        //*UserGroupMap 
        public class OEBSUserRole : IAggregateRoot
        {
            public long OracleEBSRoleInfo_RoleId { get; set; }
            public long OracleEBSUserInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }

        public class OEBSUserResponsibilities : IAggregateRoot
        {
            public long OracleEBSUserInfo_Id { get; set; }
            public long OracleEBSResponsibilitiesInfo_ResponsibilityId { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }

        #region Add to Oracle EBS History Table

        public void AddOracleEBSHistoryBulk(IList<OracleEBSUserInfoHistory> users)
        {

            //SQLBulkUploader<OUMLoginInfo> sqlUserHistory = null;
            //try
            //{
            //    string tableName = "DI_M_OUM_LoginInfo";

            //    sqlUserHistory = new SQLBulkUploader<OUMLoginInfo>(tableName);

            //    sqlUserHistory.Flush(users);

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //finally
            //{
            //}
        }

        #endregion

        #region Add to Oracle EBS Live Table

        public void AddOracleEBSInfo(IList<OracleEBSUserInfo> users)
        {
            //SQLBulkUploader<OUMLoginInfo> sql = null;
            //try
            //{
            //    string tableName = "DI_M_OUM_LoginInfo";

            //    sql = new SQLBulkUploader<OUMLoginInfo>(tableName);
            //    sql.Flush(users);

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            //finally
            //{

            //}
        }

        #endregion

        public OracleEBSConfiguration GetConfiguration(long _systemid)
        {
            return _context.OracleEBSConfiguration.FirstOrDefault(s => s.EndpointSystem.Id == _systemid);
        }


        public IList<EndpointObjects> GetORACLEObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => e.EndpointSystemTypeName == EndpointType.OracleEBS).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting ORACLE EBS EndpointObjects ", ex);
                throw ex;
            }
        }

        public IList<OracleEBSUserInfo> GetAllUsersInfo()
        {
            IList<OracleEBSUserInfo> OracleEBSUserInfo = new List<OracleEBSUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //OracleEBSUserInfo = (from usrinfo in _context.OracleEBSUserInfo where (!usrinfo.IsLocked) select usrinfo).ToList();
                    OracleEBSUserInfo = _context.OracleEBSUserInfo.Where(usrinfo => !usrinfo.IsLocked).ToList();
                }

                return OracleEBSUserInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle EBS User Information", ex);
                throw ex;
            }
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            OracleEBSUserInfo OEBSUserInfo = new OracleEBSUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                OEBSUserInfo = oIACMDBContext.OracleEBSUserInfo.Where(Oebsusr => Oebsusr.Id == EndpointUserId).FirstOrDefault();
                if (OEBSUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = OEBSUserInfo.AccountName;
                AccountDetail.AccountType = OEBSUserInfo.AccountType;
                AccountDetail.IsMappedAccount = OEBSUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = OEBSUserInfo.LastModifiedDate;
                AccountDetail.Version = Convert.ToString(OEBSUserInfo.Version);
                AccountDetail.DIUserID = OEBSUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = OEBSUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }


        public IList<OracleEBSHRMSUserInfo> GetAllHRMSUsersInfo()
        {
            IList<OracleEBSHRMSUserInfo> OracleEBSHRMSUserInfo = new List<OracleEBSHRMSUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSHRMSUserInfo = (from usrinfo in _context.OracleEBSHRMSUserInfo select usrinfo).ToList();
                }

                return OracleEBSHRMSUserInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle EBS HRMS User Information", ex);
                throw ex;
            }
        }

        public IList<OracleEBSRoleInfo> GetOEBSRoles()
        {
            IList<OracleEBSRoleInfo> OracleEBSRoleInfo = new List<OracleEBSRoleInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSRoleInfo = (from roleinfo in _context.OracleEBSRoleInfo select roleinfo).ToList();
                }

                return OracleEBSRoleInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetOEBSResponsibilities()
        {
            IList<OracleEBSResponsibilitiesInfo> OracleEBSResponsibilitiesInfo = new List<OracleEBSResponsibilitiesInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSResponsibilitiesInfo = (from resinfo in _context.OracleEBSResponsibilitiesInfo select resinfo).ToList();
                }

                return OracleEBSResponsibilitiesInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }


        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            OracleEBSUserInfo OEBSUserInfo = new OracleEBSUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                OEBSUserInfo = oIACMDBContext.OracleEBSUserInfo.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.RolesInfo).Include(lq => lq.ResponsibilitiesInfo).Where(OEBSusr => OEBSusr.Id == EndpointUserId).FirstOrDefault();
                if (OEBSUserInfo == null)
                    return Listtobereturn;

                foreach (OracleEBSRoleInfo grp in OEBSUserInfo.RolesInfo)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = OEBSUserInfo.Id;
                    UserEntitlement.UserName = OEBSUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = OEBSUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = OEBSUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.NAME;
                        UserEntitlement.Description = grp.DESCRIPTION;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }

                foreach(OracleEBSResponsibilitiesInfo grp in OEBSUserInfo.ResponsibilitiesInfo)
                {

                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = OEBSUserInfo.Id;
                    UserEntitlement.UserName = OEBSUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = OEBSUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = OEBSUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Responsibility);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.RESPONSIBILITY_NAME;
                        UserEntitlement.Description = grp.DESCRIPTION;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }

            }
            return Listtobereturn;
        }


        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                OracleEBSUserInfo OEBSInfo = Context.OracleEBSUserInfo.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (OEBSInfo != null)
                {
                    OEBSInfo.DIUserId = AccountDetail.DIUserID; ;
                    OEBSInfo.LastModifiedDate = DateTime.Today;
                    OEBSInfo.MappingType = "Manual";
                    OEBSInfo.AccountType = AccountDetail.AccountType;

                    Context.SaveChanges();
                }
            }
        }











        public IList<OracleEBSUserInfo> GetUsers(int id)
        {
            IList<OracleEBSUserInfo> OracleEBSUserInfo = new List<OracleEBSUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSUserInfo = _context.OracleEBSUserInfo.Include("ResponsibilitiesInfo").Include("RolesInfo").Where(usrinfo => usrinfo.DIUserId == id && !usrinfo.IsLocked).ToList();
                    //   OracleEBSUserInfo = (from usrinfo in _context.OracleEBSUserInfo where usrinfo.DIUserId == id select usrinfo).ToList();
                }

                return OracleEBSUserInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }

        public IList<OracleEBSRoleInfo> GetOEBSRole(int id)
        {
            IList<OracleEBSRoleInfo> OracleEBSRoleInfo = new List<OracleEBSRoleInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSRoleInfo = (from roleinfo in _context.OracleEBSRoleInfo where roleinfo.Id == id select roleinfo).ToList();
                }

                return OracleEBSRoleInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetOEBSResponsibility(int id)
        {
            IList<OracleEBSResponsibilitiesInfo> OracleEBSResponsibilitiesInfo = new List<OracleEBSResponsibilitiesInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleEBSResponsibilitiesInfo = (from resinfo in _context.OracleEBSResponsibilitiesInfo where resinfo.Id == id select resinfo).ToList();
                }

                return OracleEBSResponsibilitiesInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }

        public void AddOracleEBSRoleInfo(OracleEBSRoleInfo RoleInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleEBSRoleInfo.Add(RoleInfo);
                _context.SaveChanges();
            }
        }

        public void AddOracleEBSResponsibilitiesInfo(OracleEBSResponsibilitiesInfo ResponsibilitiesInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleEBSResponsibilitiesInfo.Add(ResponsibilitiesInfo);
                _context.SaveChanges();
            }
        }

        public void AddOracleEBSUserInfo(OracleEBSUserInfo UserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleEBSUserInfo.Add(UserInfo);
                _context.SaveChanges();
            }
        }


        public void AddOracleEBSMenus(IList<OracleEBSMenusInfo> MenuInfos)
        {
            using (_context = new IACMDBContext())
            {
                List<OracleEBSMenusInfo> ExisingMenus = _context.OracleEBSMenusInfos.ToList();
                List<OracleEBSMenusInfo> DeletedMenus = ExisingMenus.Where(c => MenuInfos.Count(d => d.MENU_NAME == c.MENU_NAME) == 0).ToList();
                List<OracleEBSMenusInfo> NewMenus = MenuInfos.Where(c => ExisingMenus.Count(d => d.MENU_NAME == c.MENU_NAME) == 0).ToList();
                _context.OracleEBSMenusInfos.AddRange(NewMenus);

                foreach (OracleEBSMenusInfo item in DeletedMenus)
                {
                    OracleEBSMenusInfo attachedMenu = _context.OracleEBSMenusInfos.Find(item.Id);
                    if (attachedMenu != null)
                    {
                        _context.OracleEBSMenusInfos.Remove(attachedMenu);
                    }

                }
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("update DI_OEBS_MenusInfo set LastModifiedBy='System', LastModifiedDate = '" + DateTime.Now + "'");
            }
        }


        public void AddOracleEBSSubmenus(IList<OracleEBSSubmenusInfo> subMenuInfos)
        {
            using (_context = new IACMDBContext())
            {
                List<OracleEBSSubmenusInfo> ExisingSubmenus = _context.OracleEBSSubmenusInfos.ToList();
                List<OracleEBSSubmenusInfo> Deletedsubmenus = ExisingSubmenus.Where(c => subMenuInfos.Count(d => d.PROMPT == c.PROMPT) == 0).ToList();
                List<OracleEBSSubmenusInfo> NewSubmenus = subMenuInfos.Where(c => ExisingSubmenus.Count(d => d.PROMPT == c.PROMPT) == 0).ToList();
                _context.OracleEBSSubmenusInfos.AddRange(NewSubmenus);

                foreach (OracleEBSSubmenusInfo item in Deletedsubmenus)
                {
                    OracleEBSSubmenusInfo attachedMenu = _context.OracleEBSSubmenusInfos.Find(item.Id);
                    if (attachedMenu != null)
                    {
                        _context.OracleEBSSubmenusInfos.Remove(attachedMenu);
                    }

                }
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("update DI_OEBS_SubmenusInfo set LastModifiedBy='System', LastModifiedDate = '" + DateTime.Now + "'");
            }
        }


        public void ChangeOracleEBSUserInfo(OracleEBSUserInfo UserInfo)
        {
            using (_context = new IACMDBContext())
            {
                //try
                //{


                var userEntry = _context.Entry(UserInfo);
                if (userEntry.State == EntityState.Detached)
                {
                    var set = _context.Set<User>();
                    User attachedUser = set.Find(UserInfo.Id);

                    if (attachedUser != null)
                    {
                        var attachedEntry = _context.Entry(attachedUser);
                        attachedEntry.CurrentValues.SetValues(attachedEntry);
                    }
                }
                else
                {
                    _context.OracleEBSUserInfo.Attach(UserInfo);
                    _context.Entry(UserInfo).State = EntityState.Modified;
                }
                _context.SaveChanges();

            }
        }


        public void DeleteOracleEBSUserInfo(IList<OracleEBSUserInfo> UserInfos)
        {
            using (_context = new IACMDBContext())
            {
                foreach (OracleEBSUserInfo delUsers in UserInfos)
                {
                    OracleEBSUserInfo User = _context.OracleEBSUserInfo.Find(delUsers.Id);
                    if (User != null)
                    {
                        _context.OracleEBSUserInfo.Remove(User);
                        _context.SaveChanges();
                    }
                }

            }
        }

        public void AddOracleEBSHRMSUserInfo(OracleEBSHRMSUserInfo HRMSUserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleEBSHRMSUserInfo.Add(HRMSUserInfo);
                _context.SaveChanges();
            }
        }

        public void ChangeOracleEBSHRMSUserInfo(OracleEBSHRMSUserInfo HRMSUserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleEBSHRMSUserInfo.Attach(HRMSUserInfo);
                _context.Entry(HRMSUserInfo).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<OracleEBSUserInfo> GetOracleEBSLoginInfoByDIUserId(int DIUserId)
        {
            //string query = "DI_spGetSQLUMLoginInfoByDIUserId";
            IList<OracleEBSUserInfo> list = new List<OracleEBSUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    list = _context.OracleEBSUserInfo
                        .Include("EndPointSystem")
                        .Include("EndPointSystem.EndPointSystemType")
                        .Where(oracleUI => oracleUI.DIUserId == DIUserId && !oracleUI.IsLocked)
                        .ToList();
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting Oracle EBS Login Info By DIUserId", ex);
                throw;
            }
        }

        public IList<OracleEBSUserInfo> GetOracleEBSUserRoleByUserId(int UserId)
        {
            //string query = "DI_spGetSQLUMLoginInfoByDIUserId";
            IList<OracleEBSUserInfo> list = new List<OracleEBSUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    list = _context.OracleEBSUserInfo
                         .Include("OracleEBSUserRole")
                       .Where(oracleUI => oracleUI.Id == UserId && !oracleUI.IsLocked)
                        .ToList();
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting SQLUM Login Info By DIUserId", ex);
                throw;
            }
        }

        public OracleEBSUserInfo GetOracleEBSUserByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.FirstOrDefault(item => item.Id == userId && !item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OracleEBSUserInfo GetOracleEBSUserByDIUserId(long diUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.FirstOrDefault(item => item.DIUserId == diUserId && !item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OracleEBSUserInfo GetLockedOracleEBSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OracleEBSUserInfo> GetPrimaryOracleEBSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Where(item => item.DIUserId == DIUserId && item.AccountType == AccountType.User
                                && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All LotusUser DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public void AddOracleEBSUserInfoBulk(IList<OracleEBSUserInfo> lotusUserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand("Truncate table DI_OEBS_UserInfo");
            }

            SQLBulkUploader<OracleEBSUserInfo> oraUser = null;
            try
            {
                string tablename = "DI_OEBS_UserInfo";
                oraUser = new SQLBulkUploader<OracleEBSUserInfo>(tablename);
                oraUser.Flush(lotusUserInfo);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                oraUser.Close();
            }
        }

        public long AddOracleEBSUser(OracleEBSUserInfo OraEBSUserInfo)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.OracleEBSUserInfo.Add(OraEBSUserInfo);
                    _context.SaveChanges();
                    return OraEBSUserInfo.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<OracleEBSRoleInfo> GetOracleEBSRoleByUserId(int userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Include("RolesInfo").FirstOrDefault(item => item.DIUserId == userId && item.EndPointSystemId == endpointSystemId && !item.IsLocked).RolesInfo.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<OracleEBSRoleInfo> GetOracleEBSRolesByEndpointUserId(long userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Include("RolesInfo").Include("ResponsibilitiesInfo").FirstOrDefault(item => item.Id == userId && item.EndPointSystemId == endpointSystemId && !item.IsLocked).RolesInfo.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<OracleEBSResponsibilitiesInfo> GetOracleEBSResponsilbilitiesByUserId(long userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Include("ResponsibilitiesInfo").FirstOrDefault(item => item.Id == userId && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ResponsibilitiesInfo.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckOracleEBSUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Any(item => item.USER_NAME.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId && !item.IsLocked);
                }
            }
            catch
            {
                throw;
            }
        }

        #region Provisioning Methods

        public bool UpdateOracleEBSUserInfoFromProvisioning(OracleEBSUserInfo oraUserInfo)
        {
            bool isDataSaved = false;
            if (oraUserInfo == null)
                return isDataSaved;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry(oraUserInfo);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<OracleEBSUserInfo>();
                        OracleEBSUserInfo attachedreviewEP = set.Find(oraUserInfo.Id);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(oraUserInfo);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while updating  OracleEBSUserInfo From Provisioning.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        #endregion

        public IList<OracleEBSRoleInfo> GetOracleEBSRoles()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSRoleInfo.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetOracleEBSResponsibilitiesInfo()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //return _context.OracleEBSResponsibilitiesInfo.Include("OracleEBSUsers").ToList();
                    return _context.OracleEBSResponsibilitiesInfo.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<OracleEBSUserInfo> OracleEBSUserToResponsibilities()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSUserInfo.Include("ResponsibilitiesInfo").Where(c => !c.IsLocked).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        public IList<OracleEBSUserRoleReport> GetOracleEBSResponsibilitiesToUser()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from Responsibilities in _context.OracleEBSResponsibilitiesInfo
                            from Users in Responsibilities.OracleEBSUsers
                            select new OracleEBSUserRoleReport
                            {
                                UserName = Users.USER_NAME,
                                RoleName = Responsibilities.RESPONSIBILITY_KEY,
                                CreatedAt = Users.CreatedDate
                            }).ToList();

                }

                //using (IACMDBContext _context = new IACMDBContext())
                //{
                //    return _context.OracleEBSResponsibilitiesInfo.Include("OracleEBSUsers").ToList();
                //}
            }
            catch (Exception)
            {
                throw;
            }


        }

        public List<OracleEBSUserRoleReport> GetOracleEBSUserToRoleReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from Users in _context.OracleEBSUserInfo.Where(c => !c.IsLocked)
                            from Roles in Users.RolesInfo
                            select new OracleEBSUserRoleReport
                            {
                                UserName = Users.USER_NAME,
                                RoleName = Roles.NAME,
                                CreatedAt = Users.CreatedDate

                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSUserRoleReport> GetOracleEBSRoleToUserReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from Users in _context.OracleEBSUserInfo
                            from Roles in Users.RolesInfo
                            select new OracleEBSUserRoleReport
                            {
                                UserName = Users.USER_NAME,
                                RoleName = Roles.NAME,
                                CreatedAt = Users.CreatedDate

                            }).ToList();

                    //return (from Roles in _context.OracleEBSRoleInfo
                    //        from Users in Roles.OracleEBSUsers
                    //        select new OracleEBSUserRoleReport
                    //        {
                    //            UserName = Users.USER_NAME,
                    //            RoleName = Roles.NAME,
                    //            CreatedAt = Users.CreatedDate
                    //        }).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //public IList<OracleEBSResponsibilitiesToUser> GetOracleEBSResponsibilitiesToUser()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return (from Roles in _context.OracleEBSResponsibilitiesInfo
        //                    from Users in Roles.OracleEBSUsers
        //                    select new OracleEBSResponsibilitiesToUser
        //                    {
        //                        UserName = Users.USER_NAME,
        //                        RoleName = Roles.DESCRIPTION,
        //                        CreatedAt = Users.CreatedDate
        //                    }).ToList();

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        public IList<OracleEBSUserInfo> GetOracleUMLoginInfoByDIUserId(int DIUserId)
        {
            IList<OracleEBSUserInfo> list = new List<OracleEBSUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    list = _context.OracleEBSUserInfo
                        .Include("EndPointSystem")
                        .Include("EndPointSystem.EndPointSystemType")
                        .Where(oracleUI => oracleUI.DIUserId == DIUserId && !oracleUI.IsLocked)
                        .ToList();
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting SQLUM Login Info By DIUserId", ex);
                throw;
            }
        }


        public IList<OracleEBSUserRoleReport> GetOracleEBSUserToResponsibilitiesReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from Responsibilities in _context.OracleEBSResponsibilitiesInfo
                            from Users in Responsibilities.OracleEBSUsers
                            select new OracleEBSUserRoleReport
                            {
                                UserName = Users.USER_NAME,
                                RoleName = Responsibilities.RESPONSIBILITY_KEY,
                                CreatedAt = Users.CreatedDate
                            }).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /*
         
        public bool AddOracleEBSConfiguration(OracleEBSConfiguration _OEBSConfiguration)
        {
            try
            {
                string query = "DI_spAddORAUMConfiguration";
                int noAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OEBSConfiguration.EndpointSystemId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@HostName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.HostName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.IPAddress;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DomainName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.DomainName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.UserName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.Password;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ServerName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.ServerName;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@InstanceName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50; ;
                sqlParam.Value = _OEBSConfiguration.InstanceName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@PortNo";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OEBSConfiguration.PortNo;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportUsers";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportUsers;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportRoles";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportRoles;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportTables";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportTables;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.MapAll;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@CreatedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.CreatedBy;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);

                IACMDbOperator iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(iacmOperator);
                noAffected = executor.ExecuteQuery();

                if (noAffected <= 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the ORACLE EBS Configuration Details", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }
 
         
        public void UpdateOEBSConfiguration(OracleEBSConfiguration _OEBSConfiguration)
        {

            try
            {
                string query = "DI_spUpdateORAUMConfiguration";
                int noAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OEBSConfiguration.EndpointSystemId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@HostName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.HostName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.IPAddress;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ServerName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.ServerName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DomainName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.DomainName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.UserName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = _OEBSConfiguration.Password;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@InstanceName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.InstanceName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@PortNo";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OEBSConfiguration.PortNo;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportUsers";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportUsers;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportRoles";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportRoles;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportTables";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.ImportTables;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OEBSConfiguration.MapAll;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OEBSConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);

                IACMDbOperator iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(iacmOperator);
                noAffected = executor.ExecuteQuery();

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the ORA UM Configuration Details", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }

         */

        /*
        public IList<OracleUMUserInfo> GetOracleUserInfo(int DIUserId)
        {
            string query = "DI_spGetORAUserInfo";

            OracleUMUserInfo _orclUser = null;

            IList<OracleUMUserInfo> _list = new List<OracleUMUserInfo>();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam = null;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserId;
                parameters.Add(sqlParam);

                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                executor = new QueryExecutor(iacmOperator);

                IDataReader reader = executor.ExecuteQueryDR();


                while (reader.Read())
                {
                    _orclUser = new OracleUMUserInfo();

                    _orclUser.UserId = CheckNull.AgainstInt(reader["UserId"]);
                    _orclUser.TenantId = CheckNull.AgainstString(reader["TenantId"]);
                    _orclUser.EndPointSystemId = CheckNull.AgainstInt(reader["EndPointSystemId"]);
                    _orclUser.UserName = CheckNull.AgainstString(reader["UserName"]);
                    _orclUser.OraUserId = CheckNull.AgainstInt(reader["OraUserId"]);
                    _orclUser.AccountStatus = CheckNull.AgainstString(reader["AccountStatus"]);
                    _orclUser.CreatedAt = CheckNull.AgainstDatetime(reader["CreatedAt"]);
                    _orclUser.LockAt = CheckNull.AgainstDatetime(reader["LockAt"]);
                    _orclUser.ExpiryAt = CheckNull.AgainstDatetime(reader["ExpiryAt"]);
                    _orclUser.DefaultTableSpace = CheckNull.AgainstString(reader["DefaultTableSpace"]);
                    _orclUser.Profile = CheckNull.AgainstString(reader["Profile"]);
                    _orclUser.InitialRSRCConsumerGroup = CheckNull.AgainstString(reader["InitialRSRCConsumerGroup"]);
                    _orclUser.AuthenticationType = CheckNull.AgainstString(reader["AuthenticationType"]);
                    _orclUser.DIUserId = CheckNull.AgainstInt(reader["DIUserId"]);
                    _orclUser.SpecialAccount = CheckNull.AgainstBit(reader["SpecialAccount"]);
                    _orclUser.CreatedBy = CheckNull.AgainstString(reader["CreatedBy"]);
                    _orclUser.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    _orclUser.LastModifiedBy = CheckNull.AgainstString(reader["LastModifiedBy"]);
                    _orclUser.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);
                    _list.Add(_orclUser);

                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }

        public IList<string> GetOracleRoles(int DIUserId, int endPointSystemId)
        {
            string query = "DI_spGetORAUserRolesByDIUserId";
            string tmpValue;
            IList<string> roleList = new List<string>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            try
            {


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DIUserId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSystemId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = endPointSystemId;
                parameters.Add(sqlParam);

                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(iacmOperator);
                IDataReader reader = executor.ExecuteQueryDR();

                while (reader.Read())
                {
                    tmpValue = CheckNull.AgainstString(reader["RoleName"]);
                    if (!string.IsNullOrEmpty(tmpValue) && !roleList.Contains(tmpValue))
                    {
                        roleList.Add(tmpValue);
                    }



                }

                return roleList;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
                throw ex;
            }
            finally
            {
                if (executor != null)
                    executor.IACMDbOperator.Close();
            }
        }

        public void AddOracleTableInfo(OracleUMTableInfo TableInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleUMTableInfo.Add(TableInfo);
                _context.SaveChanges();
            }
        }
        */

        /*public void SaveUserRoleMap(IDictionary<string,int> _oebsUser, IDictionary<string,int> _roleInfo, DataSet oebsUserRole, int endpointSystemid)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM OracleEBSRoleInfoOracleEBSUserInfo WHERE EndpointSystemId = {0}", endpointSystemid));
            }

            IList<OEBSUserRole> listUR = new List<OEBSUserRole>();
            OEBSUserRole userRole = null;

            foreach (DataRow item in oebsUserRole.Tables[0].Rows)
            {
                userRole = new OEBSUserRole();
                try
                {

                    //userRole.OracleEBSRoleInfo_RoleId = _roleInfo.FirstOrDefault(x => x.NAME.ToString().ToUpper() == CheckNull.AgainstString(item["RoleName"]).ToString().ToUpper()).RoleId;
                    int role = _roleInfo[CheckNull.AgainstString(item["RoleName"])];

                    if (role != null)
                        userRole.OracleEBSRoleInfo_RoleId = role;
                    //userRole.OracleEBSUserInfo_Id = _oebsUser.FirstOrDefault(x => x.USER_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["UserName"]).ToString().ToUpper()).UserId;
                    int usr = _oebsUser[CheckNull.AgainstString(item["UserName"])];
                    if (usr != null)
                        userRole.OracleEBSUserInfo_Id = usr;
                }
                catch (Exception ex)
                {

                }

                userRole.EndpointSystemId = endpointSystemid;
                userRole.Version = Convert.ToInt16(VersionNo.Available);

                if (userRole.OracleEBSUserInfo_Id != 0)
                {
                    if (listUR.Count(c => c.OracleEBSRoleInfo_RoleId == userRole.OracleEBSRoleInfo_RoleId && c.OracleEBSUserInfo_Id == userRole.OracleEBSUserInfo_Id) > 0)
                    {
                        listUR.Add(userRole);
                    }
                }
            }
            SQLBulkUploader<OEBSUserRole> sqlUserGroupMap = null;
            try
            {
                string tableName = "OracleEBSRoleInfoOracleEBSUserInfo";
                sqlUserGroupMap = new SQLBulkUploader<OEBSUserRole>(tableName);
                sqlUserGroupMap.Flush(listUR);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }

        }*/

        public void SaveUserRoleMap(IDictionary<string, long> _oebsUser, IDictionary<string, long> _roleInfo, DataSet oebsUserRole, long endpointSystemid)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM OracleEBSRoleInfoOracleEBSUserInfo WHERE EndpointSystemId = {0}", endpointSystemid));
            }

            IList<OEBSUserRole> listUR = new List<OEBSUserRole>();
            OEBSUserRole userRole = null;

            foreach (DataRow item in oebsUserRole.Tables[0].Rows)
            {
                userRole = new OEBSUserRole();
                try
                {
                    //userRole.OracleEBSRoleInfo_RoleId = _roleInfo.FirstOrDefault(x => x.NAME.ToString().ToUpper() == CheckNull.AgainstString(item["RoleName"]).ToString().ToUpper()).RoleId;
                    if (_roleInfo.ContainsKey(CheckNull.AgainstString(item["RoleName"])))
                    {
                        long role = _roleInfo[CheckNull.AgainstString(item["RoleName"])];

                        if (role != 0)
                            userRole.OracleEBSRoleInfo_RoleId = role;
                    }
                    //userRole.OracleEBSUserInfo_Id = _oebsUser.FirstOrDefault(x => x.USER_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["UserName"]).ToString().ToUpper()).UserId;
                    if (_oebsUser.ContainsKey(CheckNull.AgainstString(item["UserName"])))
                    {
                        long usr = _oebsUser[CheckNull.AgainstString(item["UserName"])];
                        if (usr != 0)
                            userRole.OracleEBSUserInfo_Id = usr;
                    }
                }
                catch (Exception ex)
                {

                }

                userRole.EndpointSystemId = endpointSystemid;
                userRole.Version = Convert.ToInt16(VersionNo.Available);

                if (userRole.OracleEBSUserInfo_Id != 0)
                {
                    if (listUR.Count(c => c.OracleEBSRoleInfo_RoleId == userRole.OracleEBSRoleInfo_RoleId && c.OracleEBSUserInfo_Id == userRole.OracleEBSUserInfo_Id) <= 0)
                    {
                        listUR.Add(userRole);
                    }
                }
            }
            SQLBulkUploader<OEBSUserRole> sqlUserGroupMap = null;
            try
            {
                string tableName = "OracleEBSRoleInfoOracleEBSUserInfo";
                sqlUserGroupMap = new SQLBulkUploader<OEBSUserRole>(tableName);
                sqlUserGroupMap.Flush(listUR);

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

        public void SaveUserResponsibilitiesMap(IList<OracleEBSUserInfo> _oebsUser, IList<OracleEBSResponsibilitiesInfo> _responsibilitiesInfo, DataSet oebsUserResponsibilities, int endpointSystemid)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM OracleEBSUserInfoOracleEBSResponsibilitiesInfo WHERE EndpointSystemId = {0}", endpointSystemid));
            }

            IList<OEBSUserResponsibilities> listUR = new List<OEBSUserResponsibilities>();
            OEBSUserResponsibilities userRes = null;

            SQLBulkUploader<OEBSUserResponsibilities> sqlUserGroupMap = null;

            try
            {

                foreach (DataRow item in oebsUserResponsibilities.Tables[0].Rows)
                {
                    userRes = new OEBSUserResponsibilities();

                    //userRes.OracleEBSResponsibilitiesInfo_ResponsibilityId = _responsibilitiesInfo.FirstOrDefault(x => x.RESPONSIBILITY_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["ResName"]).ToString().ToUpper()).ResponsibilityId;
                    OracleEBSResponsibilitiesInfo responsibility = _responsibilitiesInfo.FirstOrDefault(x => x.RESPONSIBILITY_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["ResName"]).ToString().ToUpper());
                    if (responsibility != null)
                        userRes.OracleEBSResponsibilitiesInfo_ResponsibilityId = responsibility.Id;
                    //userRes.OracleEBSUserInfo_Id = _oebsUser.FirstOrDefault(x => x.USER_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["UserName"]).ToString().ToUpper()).UserId;
                    OracleEBSUserInfo usr = _oebsUser.FirstOrDefault(x => x.USER_NAME.ToString().ToUpper() == CheckNull.AgainstString(item["UserName"]).ToString().ToUpper());
                    if (usr != null)
                        userRes.OracleEBSUserInfo_Id = usr.Id;
                    userRes.EndpointSystemId = endpointSystemid;
                    userRes.Version = Convert.ToInt16(VersionNo.Available);

                    if (userRes.OracleEBSUserInfo_Id != 0)
                    {
                        listUR.Add(userRes);
                    }
                }
                sqlUserGroupMap = null; // SQLBulkUploader<OEBSUserResponsibilities> 

                string tableName = "OracleEBSUserInfoOracleEBSResponsibilitiesInfo";
                sqlUserGroupMap = new SQLBulkUploader<OEBSUserResponsibilities>(tableName);
                sqlUserGroupMap.Flush(listUR);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }

        }


        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<OracleEBSUserInfo> listOEBSUserInfo = new List<OracleEBSUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listOEBSUserInfo = oIACMDBContext.OracleEBSUserInfo.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(Oebsusr => Oebsusr.EndPointSystemId == ApplicationID && Oebsusr.IsLocked == false).ToList();
                foreach (OracleEBSUserInfo OEBSusr in listOEBSUserInfo)
                {
                    if (OEBSusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = OEBSusr.Id,
                            UserName = OEBSusr.AccountName,
                            FullName = OEBSusr.USER_NAME,
                            AccountType = OEBSusr.AccountType,
                            LastLogonDate = OEBSusr.LastModifiedDate,
                            EndpointSystemId = OEBSusr.EndPointSystemId,
                            EndPointSystemType = OEBSusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = OEBSusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }




        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<OracleEBSUserInfo> listOEBSUserInfo = new List<OracleEBSUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listOEBSUserInfo = oIACMDBContext.OracleEBSUserInfo.Include("EndPointSystem").Where(usr => usr.DIUserId == UserID && usr.IsLocked == false).ToList();
                foreach (OracleEBSUserInfo user in listOEBSUserInfo)
                {
                    if (user != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = user.Id,
                            UserName = user.AccountName,
                            AccountType = user.AccountType,
                            LastLogonDate = user.LastModifiedDate,
                            EndpointSystemId = user.EndPointSystemId,
                            EndPointSystemType = user.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = user.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        //AccessRequest

        public List<AccessRequestItem> GetAccessRequestItem(string searchString)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                //TODO: Union with Responsibilities
                return (from roles in Context.OracleEBSRoleInfo
                        where roles.NAME.Contains(searchString)
                        orderby roles.NAME
                        select new AccessRequestItem
                        {
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            EntitlementType = Infrastructure.Enums.EntitlementType.Role,
                            EndpointType = EndpointType.OracleEBS,
                            KeyId = roles.Id,
                            EndpointSystemId = (int)EndpointType.OracleEBS,
                            EndpointSystemTypeId = (int)EndpointType.OracleEBS,
                            ItemName = roles.NAME,
                            ItemDescription=roles.DESCRIPTION,
                            ItemDetail=roles.DESCRIPTION
                        }).ToList();
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Infrastructure.Enums;
using System.Data.Entity;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class RoleDA //: IRoleDA
    {
        public IACMDbOperator m_dbOperator = null;

        #region IRoleDA Members

        public IList<Role> GetAllRole()
        {
            //string query = "DI_spGetRole";
            IList<Role> list = new List<Role>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.Roles.Include(x => x.Users).OrderBy(x => x.RoleName).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting all roles.", ex);
                throw;
            }
        }

        public Role GetRole(int _id)
        {
            Role role = new Role();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    role = oIACMDBContext.Roles.Include(lq => lq.Users).Where(lq => lq.Id == _id).FirstOrDefault();
                }
                return role;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting all roles.", ex);
                throw;
            }
        }

        public bool IsRoleExist(string RoleName, int RoleID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                bool flag = true;
                if (RoleID > 0)
                {
                    var ExistRoleName = GetRole(RoleID).RoleName;
                    flag = (ExistRoleName != RoleName) ? true : false;
                }
                if (flag)
                {
                    var dbRole = oIACMDBContext.Roles.Where(c => c.RoleName == RoleName).FirstOrDefault();
                    return dbRole != null ? true : false;
                }
                return false;
            }
        }

        //public int AddRole(Role _role)
        //{
        //    //string query = "DI_spAddRole";
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            oIACMDBContext.Roles.Add(_role);
        //            oIACMDBContext.SaveChanges();
        //            return _role.RoleId;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //       DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while adding new role.", ex);
        //        throw;
        //    }
        //}

        //public int updateRole(Role role)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            var taskEntry = oIACMDBContext.Entry<Role>(role);
        //            if (taskEntry.State == EntityState.Detached)
        //            {
        //                var set = oIACMDBContext.Set<Role>();
        //                Role attachedTask = set.Find(role.RoleId);

        //                if (attachedTask != null)
        //                {
        //                    var attachedEntry = oIACMDBContext.Entry(attachedTask);
        //                    attachedEntry.CurrentValues.SetValues(role);
        //                }
        //            }
        //            oIACMDBContext.SaveChanges();
        //        }
        //        return role.RoleId;
        //    }
        //    catch (Exception ex)
        //    {
        //       DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while updating role.", ex);
        //        throw;
        //    }
        //}

        public long SaveRole(Role _role)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_role.Id == 0)
                    {
                        oIACMDBContext.Roles.Attach(_role);
                        oIACMDBContext.Entry(_role).State = EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        oIACMDBContext.Database.ExecuteSqlCommand(string.Format("DELETE FROM UserRole where Role_RoleId={0}", _role.Id));
                        var dbRole = oIACMDBContext.Roles
                                             .Include(x => x.Users)
                                             .Single(c => c.Id == _role.Id);

                        oIACMDBContext.Entry(dbRole).CurrentValues.SetValues(_role);
                        foreach (var newUser in _role.Users)
                        {
                            var dbUser = oIACMDBContext.Users.Where(t => t.Id == newUser.Id).FirstOrDefault();
                            if (dbUser != null)
                                 dbRole.Users.Add(dbUser);
                        }
                    }
                    oIACMDBContext.SaveChanges();
                }
                return _role.Id;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Role", ex);
                throw ex;
            }
        }

        public long CloneRole(Role role, bool cloneMenu, bool cloneQL, bool cloneDB, int cloneRoleID)
        {
            try
            {
                long roleID = SaveRole(role);

                if (cloneMenu)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        List<AccessRights> list = (from ar in oIACMDBContext.AccessRights
                                    where ar.RoleId ==cloneRoleID select ar).ToList();

                        foreach (AccessRights t in list)
                        {
                            if (t.PagePermissionId > 0)
                            {
                                t.RoleId = roleID;
                                oIACMDBContext.AccessRights.Attach(t);
                                oIACMDBContext.Entry(t).State = EntityState.Added;
                                oIACMDBContext.SaveChanges();
                            }
                        }
                    }
                }

                if (cloneQL)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        List<QuickLinkAccessRights> list = (from ar in oIACMDBContext.QuickLinkAccessRights
                                                   where ar.RoleId == cloneRoleID
                                                   select ar).ToList();

                        foreach (QuickLinkAccessRights t in list)
                        {
                            if (t.QuickLinkId > 0)
                            {
                                t.RoleId = roleID;
                                oIACMDBContext.QuickLinkAccessRights.Attach(t);
                                oIACMDBContext.Entry(t).State = EntityState.Added;
                                oIACMDBContext.SaveChanges();
                            }
                        }
                    }
                }

                if (cloneDB)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        List<DashboardAccessRights> list = (from ar in oIACMDBContext.DashboardAccessRights
                                                   where ar.RoleId == cloneRoleID
                                                   select ar).ToList();

                        foreach (DashboardAccessRights t in list)
                        {
                            if (t.DashboardId > 0)
                            {
                                t.RoleId = roleID;
                                oIACMDBContext.DashboardAccessRights.Attach(t);
                                oIACMDBContext.Entry(t).State = EntityState.Added;
                                oIACMDBContext.SaveChanges();
                            }
                        }
                    }
                }

                return roleID;

            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while cloning the Role", ex);
                throw ex;
            }
        }

        public bool DeleteRole(int RoleID)
        {
            bool isDelete = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    Role delRole = oIACMDBContext.Roles.Where(ra => ra.Id == RoleID).FirstOrDefault();
                    var v = (from x in delRole.Users
                             select x).ToList();

                    foreach (var item in v)
                    {
                        delRole.Users.Remove(item);
                    }
                    oIACMDBContext.Roles.Remove(delRole);
                    oIACMDBContext.SaveChanges();
                    isDelete = true;
                }
                return isDelete;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting role.", ex);
                isDelete = false;
                throw;
            }
        }


        public bool DeleteUserForSelectedRole(int RoleID, User User)
        {
            bool isDelete = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    Role SelRole = oIACMDBContext.Roles.Include("Users").Where(ra => ra.Id == RoleID).FirstOrDefault();

                    SelRole.Users.Remove(User);
                    //SelRole.Users.RemoveRange(SelRole.Users);

                    //  oIACMDBContext.Entry(SelRole.Users).State = EntityState.Modified;

                    oIACMDBContext.Entry(SelRole).State = EntityState.Modified;

                    oIACMDBContext.SaveChanges();
                    isDelete = true;
                }
                return isDelete;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting user for selected role.", ex);
                isDelete = false;
                throw;
            }
        }
        #endregion


        public IList<Role> GetAllRolesByEndpointID(int EndpointSystemId, int EndPointID)
        {
            Role role;
            IList<Role> list = new List<Role>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    switch (EndpointSystemId)
                    {
                        case (int)EndpointType.SAP:
                            var tmpList2 = (from ri in oIACMDBContext.SAPRoleInfos
                                            where ri.EndPointSystemId == EndPointID
                                            orderby ri.ROLE_CODE
                                            select new { RoleId = ri.Id, RoleName = ri.ROLE_CODE, Description = ri.ROLE_DESC }).Distinct();

                            if (tmpList2.Count() > 0)
                            {
                                foreach (var tLst in tmpList2)
                                {
                                    role = new Role();
                                    role.Id = CheckNull.AgainstInt(tLst.RoleId);
                                    role.RoleName = CheckNull.AgainstString(tLst.RoleName);
                                    role.RoleDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(role);
                                }
                            }
                            break;
                        case (int)EndpointType.SAPEnterprisePortal:
                            var tmpList3 = (from ri in oIACMDBContext.SAPEPRoleInfos
                                            where ri.EndPointSystemId == EndPointID
                                            orderby ri.RoleName
                                            select new { RoleId = ri.Id, ri.RoleName, Description = ri.RoleDescription }).Distinct();

                            if (tmpList3.Count() > 0)
                            {
                                foreach (var tLst in tmpList3)
                                {
                                    role = new Role();
                                    role.Id = CheckNull.AgainstInt(tLst.RoleId);
                                    role.RoleName = CheckNull.AgainstString(tLst.RoleName);
                                    role.RoleDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(role);
                                }
                            }
                            break;
                    }
                }
                return list;
            }
            catch
            {
                throw;
            }
        }
    }
}

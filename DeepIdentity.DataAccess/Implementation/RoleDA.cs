using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model.ReportData;
using DeepIdentity.Infrastructure.Enums;
using System.Data.Entity;

namespace DeepIdentity.DataAccess
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
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting all roles.", ex);
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
                    role = oIACMDBContext.Roles.Include(lq => lq.Users).Where(lq => lq.RoleId == _id).FirstOrDefault();
                }
                return role;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting all roles.", ex);
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
        //        Log.General.WriteMessage(LogLevel.ERROR, "Error occur while adding new role.", ex);
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
        //        Log.General.WriteMessage(LogLevel.ERROR, "Error occur while updating role.", ex);
        //        throw;
        //    }
        //}

        public int SaveRole(Role _role)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_role.RoleId == 0)
                    {
                        oIACMDBContext.Roles.Attach(_role);
                        oIACMDBContext.Entry(_role).State = EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        var dbRole = oIACMDBContext.Roles
                                              .Include(x => x.Users)
                                              .Single(c => c.RoleId == _role.RoleId);

                        oIACMDBContext.Entry(dbRole).CurrentValues.SetValues(_role);

                        var v = (from x in dbRole.Users
                                 where !_role.Users.Contains(x)
                                 select x).ToList();

                        foreach (var item in v)
                        {
                            dbRole.Users.Remove(item);
                        }


                        foreach (var newUser in _role.Users)
                        {
                            var item = dbRole.Users.SingleOrDefault(s => s.UserId == newUser.UserId);
                            if (item != null)
                                oIACMDBContext.Entry(item).CurrentValues.SetValues(newUser);
                            else
                            {
                                var dbUser = oIACMDBContext.Users.Where(t => t.UserId == newUser.UserId).FirstOrDefault();

                                if (dbUser != null)
                                    dbRole.Users.Add(dbUser);
                            }
                        }
                    }
                    oIACMDBContext.SaveChanges();
                }
                return _role.RoleId;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Saving the Group", ex);
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
                    Role delRole = oIACMDBContext.Roles.Where(ra => ra.RoleId == RoleID).FirstOrDefault();
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
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting role.", ex);
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
                                            select new { RoleId = ri.SAP_ROLEID, RoleName = ri.ROLE_CODE, Description = ri.ROLE_DESC }).Distinct();

                            if (tmpList2.Count() > 0)
                            {
                                foreach (var tLst in tmpList2)
                                {
                                    role = new Role();
                                    role.RoleId = CheckNull.AgainstInt(tLst.RoleId);
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
                                    role.RoleId = CheckNull.AgainstInt(tLst.RoleId);
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

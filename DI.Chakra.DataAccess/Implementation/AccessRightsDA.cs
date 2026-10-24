using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;
using DI.Chakra.Infrastructure;
using System.Data.Entity;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class AccessRightsDA
    {
        public IList<AccessRights> GetAccessRightsByRoleId(int roleID)
        {
            IList<AccessRights> list = new List<AccessRights>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.AccessRights.Include("PagePermission").Where(arights => arights.RoleId == roleID).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while getting AccessRights By RoleID.", ex);
                throw;
            }
            finally
            {

            }
        }

        public int Add(AccessRights newAccessRights)
        {
            try
            {
                //using (IACMDBContext c = new IACMDBContext())
                //{
                //    IList<AccessRights> chkAccessRights = c.AccessRights.Where(ar => ar.RoleId == newAccessRights.RoleId && ar.TabMenuId == newAccessRights.TabMenuId).ToList();
                //    if (chkAccessRights.Count == 0)
                //    {
                //        c.AccessRights.Add(newAccessRights);
                //        c.SaveChanges();
                //    }
                //    int tmpCnt = c.AccessRights.Count();
                //    return tmpCnt;
                //}
              
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while adding new AccessRights.", ex);
                throw;
            }
            return -1;
        }

        public bool DeleteAccessRights(AccessRights AccessRight)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    bool isDelete = false;
                    AccessRights delAccessRight = c.AccessRights.Where(ar => ar.RoleId == AccessRight.RoleId && (ar.PagePermissionId == AccessRight.PagePermissionId)).FirstOrDefault();
                    if (delAccessRight != null)
                    {
                        c.Database.ExecuteSqlCommand(string.Format("UPDATE DI_M_AccessRights SET PagePermissionId= NULL WHERE Id = {0}", AccessRight.Id));
                        c.SaveChanges();
                        c.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_M_AccessRights where Id={0}", AccessRight.Id));
                        c.SaveChanges();
                        isDelete = true;
                    }
                    return isDelete;
                }
             
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while deleting AccessRight.", ex);
                throw;
            }
            finally
            {

            }

            return false;
        }

        public void SaveAccessRights(AccessRights AccessRightDetails)
        {
            using (IACMDBContext c = new IACMDBContext())
            {
                if (AccessRightDetails.Id == 0)
                {
                    if (AccessRightDetails.PagePermission != null && AccessRightDetails.PagePermissionId > 0)
                    {
                        c.AccessRights.Attach(AccessRightDetails);
                        c.Entry(AccessRightDetails).State = EntityState.Added;
                        c.SaveChanges();
                    }
                }
                else
                {
                    if (AccessRightDetails.PagePermission != null && AccessRightDetails.PagePermissionId < 0)
                    {
                        DeleteAccessRights(AccessRightDetails);
                    }
                }
                c.SaveChanges();
            }
        }

        public AccessRights GetAccessRights(User userInfo, string absolutePath)
        {
            try
            {
                //using (IACMDBContext _context = new IACMDBContext())
                //{
                //    if (userInfo.Roles != null && userInfo.Roles.Count > 0)
                //    {
                //        foreach (Role role in userInfo.Roles)
                //        {
                //            return (from TM in _context.TabMenu
                //                    join AR in _context.AccessRights
                //                    on TM.Id equals AR.TabMenuId
                //                    where TM.TargetUrl.ToUpper() == absolutePath.ToUpper()
                //                    && AR.RoleId == role.RoleId
                //                    select AR).FirstOrDefault();

                //        }
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public AccessRights GetAccessRight(int RoleId, int TabMenuId)
        {
            //using (IACMDBContext oIACMDBContext = new IACMDBContext())
            //{
            //    return oIACMDBContext.AccessRights.Where(c => c.RoleId == RoleId && c.TabMenuId == TabMenuId).FirstOrDefault();
            //}
            return null; 
        }

        public List<PagePermission> GetPagePermission(int PageId)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                return oIACMDBContext.PagePermissions.Include(lq => lq.Page).Include(lq => lq.Permission).Where(c => c.PageId == PageId).ToList();
            }
        }

        public IList<AccessRights> GetAccessRights(int TabMenuId)
        {
            //using (IACMDBContext oIACMDBContext = new IACMDBContext())
            //{
            //    return oIACMDBContext.AccessRights.Include("TabMenuPermission").Include("TabMenuPermission.Permission").Where(c => c.TabMenuId == TabMenuId).Distinct().ToList();
            //}
            return null;
        }
       
        public IList<PagePermission> GetPagePermissions(int PageId, User _user)
        {
            List<PagePermission> permissions = new List<PagePermission>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (_user.Roles != null && _user.Roles.Count > 0)
                    {
                        foreach (Role role in _user.Roles)
                        {
                            var list = (from perms in _context.PagePermissions
                                        join AR in _context.AccessRights
                                        on (int)perms.Id equals AR.PagePermissionId
                                        join R in _context.Roles
                                        on AR.RoleId equals R.Id
                                        where perms.PageId == PageId && perms.Permission.Name != "View" && R.Id == role.Id
                                        select perms).Include("Url").Include("Permission");
                            var list1 = list.Distinct().ToList();

                            permissions.AddRange(list);

                        }
                    }
                }

                return permissions.Distinct(new PermissionComparer()).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool HasAccess(int PageId, User _user)
        {
            List<PagePermission> permissions = new List<PagePermission>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (_user.Roles != null && _user.Roles.Count > 0)
                    {
                        foreach (Role role in _user.Roles)
                        {
                            var list = (from perms in _context.PagePermissions
                                        join AR in _context.AccessRights
                                        on (int)perms.Id equals AR.PagePermissionId
                                        join R in _context.Roles
                                        on AR.RoleId equals R.Id
                                        where perms.PageId == PageId && R.Id == role.Id
                                        select perms).Distinct().ToList();

                            permissions.AddRange(list);

                        }
                    }
                }

                if (permissions.Count > 0)
                    return true;

            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public PagePermission GetPermission(int PageId, int PermissionID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                return oIACMDBContext.PagePermissions.Where(c => c.PageId == PageId && c.PermissionId == PermissionID).FirstOrDefault();
            }
        }
    }
}

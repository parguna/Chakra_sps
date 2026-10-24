using DI.Chakra.Model;
using DI.Chakra.Model.GridData;
using DI.Chakra.Model.ReportData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DataAccess
{
    public class AuditDA
    {
        public List<AccessRightAuditData> GetAccessRightAuditDetails()
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    return (from acc in _iacmdbcontext.AccessRightHistories
                            join role in _iacmdbcontext.Roles
                                on acc.RoleId equals role.Id
                            join tab in _iacmdbcontext.TabMenu
                                on (int)acc.TabMenuId equals tab.Id
                            select new AccessRightAuditData
                            {
                                Action = acc.UserAction,
                                ChangedBy = acc.LastModifiedBy,
                                ChangedDate = acc.LastModifiedDate,
                                RoleName = role.RoleName,
                                MenuName = tab.MenuName
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public List<MasterConfigAuditData> GetMasterConfigAuditDetails()
        //{
        //    try
        //    {
        //        using (IACMDBContext _iacmdbcontext = new IACMDBContext())
        //        {
        //            return (from acc in _iacmdbcontext.ConfigurationMasterHistories
        //                    select new MasterConfigAuditData
        //                    {
        //                        ConfigKey = acc.ConfigKey,
        //                        ConfigValue = acc.ConfigValue,
        //                        ChangedBy = acc.LastModifiedBy,
        //                        ChangedDate = acc.LastModifiedDate
        //                    }).ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<GroupAuditData> GetGroupAuditDetails()
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    return (from acc in _iacmdbcontext.GroupHistories
                            select new GroupAuditData
                            {
                                GroupName = acc.GroupName,
                                GroupOwner = acc.GroupOwner,
                                Description = acc.GroupDescription,
                                IsDefaultGroup = acc.IsDefaultGroup,
                                ChangedBy = acc.LastModifiedBy,
                                ChangedDate = acc.LastModifiedDate
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RoleAuditData> GetRoleAuditDetails()
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    return (from acc in _iacmdbcontext.RoleHistories
                            select new RoleAuditData
                            {
                                RoleName = acc.RoleName,
                                Description = acc.RoleName,
                                IsDefaultRole = acc.IsDefaultRole,
                                ChangedBy = acc.LastModifiedBy,
                                ChangedDate = acc.LastModifiedDate
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

       

    }
}

using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;
using System.Text;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Collections;
namespace DeepIdentity.DataAccess
{
    public class InhouseSystemDA
    {
        public void AddInhouseSystem(InhouseSystemConfiguration inhouseSystem)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.InhouseSystems.Add(inhouseSystem);
                    _context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
        }

        public InhouseSystemConfiguration GetInhouseSystemById(int EndpointsystemID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseSystems.FirstOrDefault(item => item.EndpointSystemId == EndpointsystemID);
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInhouseSystem(InhouseSystemConfiguration inhouseSystem)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var objEntry = _context.Entry(inhouseSystem);
                    if (objEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<InhouseSystemConfiguration>();
                        InhouseSystemConfiguration attachedObj = set.Find(inhouseSystem.InhouseSystemId);

                        if (attachedObj != null)
                        {
                            var attachedEntry = _context.Entry(attachedObj);
                            attachedEntry.CurrentValues.SetValues(inhouseSystem);
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



        public IList<InhouseSystemsGridData> GetInhouseSystemsGridData()
        {
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        var List = (from inhouseSystems in _context.InhouseSystems
            //                    join usr in _context.Users
            //                    on inhouseSystems.ApplicationOwnerId equals usr.UserId into list
            //                    from diusr in list.DefaultIfEmpty()
            //                    select new InhouseSystemsGridData
            //                    {
            //                        InhouseSystemId = inhouseSystems.InhouseSystemId,
            //                        EndpointSystemId = inhouseSystems.EndpointSystemId,
            //                        SystemName = inhouseSystems.SystemName,
            //                        IsActive = inhouseSystems.IsActive,
            //                        ApplicationOwner = diusr.DisplayName,
            //                        CreatedBy = inhouseSystems.CreatedBy,
            //                        CreatedDate = inhouseSystems.CreatedDate,
            //                        LastModifiedBy = inhouseSystems.LastModifiedBy,
            //                        LastModifiedDate = inhouseSystems.LastModifiedDate
            //                    }).ToList();

            //        return List;
            //    }


            //}
            //catch
            //{
            //    throw;
            //}
            return null;
        }
        public IList<InhouseSystemEntitlementsGridData> GetInhouseSystemEntitlementsGridData(int inhouseSystemId)
        {
            return null;
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        var List = (from inhouseSystemEntitlements in _context.InhouseSystemEntitlements
            //                    join inhouseSystems in _context.InhouseSystems
            //                    on inhouseSystemEntitlements.EndPointSystemId equals inhouseSystems.EndpointSystemId
            //                    select new InhouseSystemEntitlementsGridData
            //                    {
            //                        InhouseSystemEntitlementId = inhouseSystemEntitlements.InhouseSystemEntitlementId,
            //                        InhouseSystemsId = inhouseSystems.InhouseSystemId,
            //                        SystemName = inhouseSystems.SystemName,
            //                        EntitlementName = inhouseSystemEntitlements.EntitlementName,
            //                        CreatedBy = inhouseSystems.CreatedBy,
            //                        CreatedDate = inhouseSystems.CreatedDate,
            //                        LastModifiedBy = inhouseSystems.LastModifiedBy,
            //                        LastModifiedDate = inhouseSystems.LastModifiedDate
            //                    }).Where(t => t.InhouseSystemsId == inhouseSystemId || inhouseSystemId == -1).ToList();

            //        return List;
            //    }


            //}
            //catch
            //{
            //    throw;
            //}
        }

        public IList<InhouseUserInfo> GetInhouseUsers(int DIUserId)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from user in _context.InhouseUserInfos
                                  .Include("EndPoint")
                                  .Include("EndPoint.EndPointSystemType")
                            where user.DIUserId == DIUserId
                            orderby user.CreatedDate
                            select user).ToList();
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        public InhouseUserInfo GetInhouseUserInfoByUserId(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.FirstOrDefault(user => user.Id == UserId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public InhouseUserInfo GetInhouseUserInfoByKey(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.FirstOrDefault(user => user.UserId == UserId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<InhousePermission> GetAllPermissionsByUserid(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<InhousePermission> InhousePermissions = null;
                    InhouseUserInfo InhouseUserInfo = _context.InhouseUserInfos.FirstOrDefault(user => user.Id == UserId);
                    if (InhouseUserInfo != null)
                    {
                        string Permission = InhouseUserInfo.Permission;
                        string[] Permissions = Permission.Split(',');
                        InhousePermissions = (from inHousePermission in _context.InhousePermissions
                                              join PermissionVaue in Permissions on inHousePermission.Name equals PermissionVaue
                                              select inHousePermission).ToList();

                    }
                    return InhousePermissions;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<InhouseSystemConfiguration> GetInhouseSystemsByEndpointType(EndpointType endpointSystemType)
        {
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {

            //        var systems = (from inHousePermission in _context.InhousePermissions
            //                       join PermissionVaue in Permissions on inHousePermission.Name equals PermissionVaue
            //                       select inHousePermission).ToList();

            //        return _context.InhouseSystems.Where(item => item.EndpointSystemType == endpointSystemType).OrderBy(s=>s.SystemName).ToList();
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
            return null;
        }

        public Dictionary<int, string> GetPermissionsByEndpointSystemId(int EndpointSystemid, string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    return oIACMDBContext.InhousePermissions

                        .Where(inh => inh.IsActive == true && inh.Description.Contains(searchString) && inh.EndPointSystemID == EndpointSystemid).OrderBy(inh => inh.Description)
                        .Select(inh => new { Key = inh.Id, Value = inh.Description }).ToDictionary(o => o.Key, o => o.Value);

                }

            }
            catch
            {
                throw;
            }

        }



        public InhouseSystemConfiguration GetInhouseSystemBySystemCode(string SystemCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    InhouseSystemConfiguration obj = (from inhouseSystems in _context.InhouseSystems
                                                      join epSystems in _context.EndpointSystems
                                                      on inhouseSystems.EndpointSystemId equals epSystems.EndpointSystemId
                                                      where epSystems.ApplicationCode == SystemCode
                                                      select inhouseSystems).FirstOrDefault();

                    return obj;
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddInhouseSystemUserInfo(InhouseUserInfo inhouseSystem)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.InhouseUserInfos.Add(inhouseSystem);
                    _context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
        }

        public InhousePermission GetInhouseSystemPermissionById(int ID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhousePermissions.FirstOrDefault(item => item.Id == ID);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddInhouseSystemPermission(InhousePermission inhousepermission)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.InhousePermissions.Add(inhousepermission);
                    _context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInhousePermission(InhousePermission inhousepermission)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var objEntry = _context.Entry(inhousepermission);
                    if (objEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<InhousePermission>();
                        InhousePermission attachedObj = set.Find(inhousepermission.Id);

                        if (attachedObj != null)
                        {
                            var attachedEntry = _context.Entry(attachedObj);
                            attachedEntry.CurrentValues.SetValues(inhousepermission);
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

        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionGridData(string inhouseSystemCode)
        {
            return null;
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        var List = (from inhousepermission in _context.InhousePermissions
            //                    join inhouseSystems in _context.InhouseSystems
            //                    on inhousepermission.EndPointSystemID equals inhouseSystems.EndpointSystemId
            //                    join ep in _context.EndpointSystems
            //                    on inhouseSystems.EndpointSystemId equals ep.EndpointSystemId
            //                    select new InhouseSystemEntitlementsGridData
            //                    {
            //                        InhouseSystemEntitlementId = inhousepermission.Id,
            //                        InhouseSystemsCode = inhousepermission.InhouseSystemCode,
            //                        SystemName = inhouseSystems.SystemName,
            //                        EntitlementName = inhousepermission.Name,
            //                        IsActive = inhousepermission.IsActive,
            //                        CreatedBy = inhouseSystems.CreatedBy,
            //                        CreatedDate = inhousepermission.CreatedDate,
            //                        LastModifiedBy = inhouseSystems.LastModifiedBy,
            //                        LastModifiedDate = inhouseSystems.LastModifiedDate
            //                    }).Where(t => t.InhouseSystemsCode == inhouseSystemCode).ToList();

            //        return List;
            //    }


            //}
            //catch
            //{
            //    throw;
            //}
        }

        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionByInhouseID(int EndpointSystemID)
        {
            return null;
            //TODO: CHECK WASIM, FOUND SAME METHOD IN OTHER PLACE
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        var List = (from inhousepermission in _context.InhousePermissions
            //                    join inhouseSystems in _context.InhouseSystems
            //                    on inhousepermission.InhouseSystemCode equals inhouseSystems.SystemCode
            //                    select new InhouseSystemEntitlementsGridData
            //                    {
            //                        InhouseSystemEntitlementId = inhousepermission.Id,
            //                        InhouseSystemsId = inhouseSystems.InhouseSystemId,
            //                        EndPointSystemID = inhouseSystems.EndpointSystemId,
            //                        EntitlementName = inhousepermission.Name,
            //                        CanRequestMultiple = inhouseSystems.CanRequestMultipleRoles
            //                    }).Where(t => t.EndPointSystemID == EndpointSystemID).ToList();

            //        return List.OrderBy(item=>item.EntitlementName).ToList();
            //    }


            //}
            //catch
            //{
            //    throw;
            //}
        }

        public List<InhouseUserInfo> GetInhouseUserInfoByUserName(string UserName, string inhousesystemcode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.Where(item => item.UserName == UserName && item.InhouseSystemCode == inhousesystemcode).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetInhouseUserInfoByID(int ID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.Where(item => item.UserId == ID).ToList();
                }
            }
            catch
            {
                throw;
            }
        }
        public List<InhouseUserInfo> GetPrimaryInhouseUserInfoByuserId(int UserID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.Where(item => item.DIUserId == UserID && item.IsLocked == false).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetPrimaryInhouseUserInfoByuserId(int UserID, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.Where(item => item.DIUserId == UserID && item.EndPointSystemId == endpointSystemId && item.IsLocked == false).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseSystemConfiguration> GetInhouseSystemBySystemName(string SystemName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseSystems.Where(item => item.EndpointSystem.DisplayName == SystemName).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhousePermission> GetInhousePermissionByName(string PermissionName, string SystemCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhousePermissions.Where(item => item.Name == PermissionName && item.InhouseSystemCode == SystemCode).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseSystemsGridData> GetInhouseUserInfoBySystemCode(string inhousesystemcode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var List = (from INHUsr in _context.InhouseUserInfos
                                join diusr in _context.Users
                                on INHUsr.DIUserId equals diusr.UserId into details
                                from INHUSR in details.DefaultIfEmpty()
                                where INHUsr.IsDeleted == false && INHUsr.IsLocked == false
                                select new InhouseSystemsGridData
                                {
                                    UserId = INHUsr.UserId,
                                    DIUserID = INHUSR.UserId,
                                    UserName = INHUsr.UserName,
                                    DisplayName = INHUsr.DisplayName,
                                    NRIC = INHUsr.NRIC,
                                    Email = INHUsr.Email,
                                    EmployeeId = INHUsr.EmployeeId,
                                    AccountCreatedDate = INHUsr.AccountCreatedDate,
                                    ApplicationOwner = INHUSR.DisplayName,
                                    SystemCode = INHUsr.InhouseSystemCode,
                                    CreatedBy = INHUsr.CreatedBy,
                                    CreatedDate = INHUsr.CreatedDate,
                                    LastLoginDate = INHUsr.LastLoginDate,
                                    LastModifiedBy = INHUsr.LastModifiedBy,
                                    IsDisabled = INHUsr.IsDisabled,
                                    IsDeleted = INHUsr.IsDeleted,
                                    IsLocked = INHUsr.IsLocked,
                                    Permission = INHUsr.Permission,
                                    AccountType = INHUsr.AccountType,
                                    LastModifiedDate = INHUsr.LastModifiedDate,
                                    Lockscenario = INHUsr.LockScenario,
                                    IsMapped = (INHUsr.DIUserId != null) ? "Yes" : "No",
                                    Owner = (INHUsr.DIUserId.HasValue) ? _context.Users.FirstOrDefault(usrs => usrs.UserId == INHUsr.DIUserId).DisplayName : ""
                                }).Where(t => t.SystemCode == inhousesystemcode).ToList();

                    return List;
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInhouseUserInfo(InhouseUserInfo inhouseuser)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var objEntry = _context.Entry(inhouseuser);
                    if (objEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<InhouseUserInfo>();
                        InhouseUserInfo attachedObj = set.Find(inhouseuser.UserId);

                        if (attachedObj != null)
                        {
                            var attachedEntry = _context.Entry(attachedObj);
                            attachedEntry.CurrentValues.SetValues(inhouseuser);
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


        public List<AttributeMappingColumns> GetAttributeMappingColumns(int SystemId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    return oIACMDBContext.AttributeMappingColumnss.Where(item => item.EndPoint.EndpointSystemId == SystemId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

       



        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionBySystemCode(string SystemCode)
        {

            return null;
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        var List = (from inhousepermission in _context.InhousePermissions
            //                    join inhouseSystems in _context.InhouseSystems
            //                    on inhousepermission.InhouseSystemCode equals inhouseSystems.SystemCode
            //                    select new InhouseSystemEntitlementsGridData
            //                    {
            //                        InhouseSystemEntitlementId = inhousepermission.Id,
            //                        InhouseSystemsId = inhouseSystems.InhouseSystemId,
            //                        EntitlementName = inhousepermission.Name,
            //                        CanRequestMultiple = inhouseSystems.CanRequestMultipleRoles,
            //                        InhouseSystemsCode = inhouseSystems.SystemCode
            //                    }).Where(t => t.InhouseSystemsCode == SystemCode).ToList();
            //        var Sortlist = List.OrderBy(s => s.EntitlementName).ToList();

            //        return Sortlist;
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
        }

        public string GetJoinQry(int EndPointSystemID)
        {
            bool MapAll = true;

            string clauseType = "AND";

            if (!MapAll)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();



            IList<AttributeMappingColumns> listAttributes = GetAttributeMappingColumns(EndPointSystemID);


            foreach (AttributeMappingColumns attDto in listAttributes)
            {

                bldr.AppendFormat("DI_INH_UserInfo.{0}=DI_M_User.{1} {2} ", attDto.Tgcolumn, attDto.DiColumn, clauseType);

            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));

            return string.Empty;
        }
        public DataTable MapWithMasterUser(int EndPointSystemID)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();

            string strQry = string.Format("Update DI_INH_UserInfo Set DIUserId=NULL, MappingType=NULL where EndpointSystemId={0} AND MappingType='System'", EndPointSystemID);
            EndpointObjectDA.ExecuteQry(strQry);
            StringBuilder bldR = new StringBuilder();

            string strJoin = GetJoinQry(EndPointSystemID);

            if (strJoin == string.Empty)
                return null;

            bldR.Append("Update DI_INH_UserInfo set DI_INH_UserInfo.DIUserId = DI_M_User.UserId, DI_INH_UserInfo.MappingType='System',DI_INH_UserInfo.AccountType=1 FROM DI_INH_UserInfo JOIN DI_M_User on");
            bldR.Append(" ");
            bldR.Append(strJoin);
            bldR.Append(" ");
            bldR.AppendFormat("WHERE DI_INH_UserInfo.EndpointSystemId = {0} AND DI_M_User.UserId <> 1", EndPointSystemID);
            EndpointObjectDA.ExecuteQry(bldR.ToString());
            return null;
        }

        public Dictionary<int, string> GetINHUsersByDIUserId(int DIUserID, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //return _context.InhouseUserInfos.Where(item => item.EndPointSystemId == endpointsystemid && item.IsDeleted == false && item.IsLocked == false
                    //    && accountType == AccountType.User ? (item.IsUserAccount == true && item.IsSharedAccount == false &&
                    //    item.IsServiceAccount == false && item.DIUserId == DIUserID) : accountType == AccountType.Shared ?
                    //    item.IsSharedAccount == true : item.IsServiceAccount == true && item.DIUserId == DIUserID)
                    //    .Select(x => new { Key = x.UserId, Value = x.UserName })
                    //    .Distinct().ToDictionary(o => o.Key, o => o.Value);


                    if (accountType == AccountType.User)
                    {
                        return _context.InhouseUserInfos.Where(item => item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType && item.DIUserId == DIUserID))
                                                .Select(x => new { Key = x.UserId, Value = x.UserName })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.UserId == DIUserID).Select(lq => lq.SectionCode).FirstOrDefault();
                        List<int> listofUser = _context.Users.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.UserId).ToList();
                        var listofitems = _context.InhouseUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.UserId, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.UserId == DIUserID).Select(lq => lq.SectionCode).FirstOrDefault();
                        List<int> listofUser = _context.Users.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.UserId).ToList();
                        var listofitems = _context.InhouseUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.UserId, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<InhouseSystemsGridData> GetInhouseInfoByDIUserIDSystemCode(int DIUserID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var List = (from users in _context.InhouseUserInfos
                                join inhouseSystems in _context.InhouseSystems
                                on users.EndPointSystemId equals inhouseSystems.EndpointSystemId
                                join endpoints in _context.EndpointSystems
                                on users.EndPointSystemId equals endpoints.EndpointSystemId
                                where users.IsDeleted == false && users.IsLocked == false
                                select new InhouseSystemsGridData
                                {
                                    DIUserID = users.DIUserId,
                                    UserName = users.UserName,
                                    SystemCode = users.InhouseSystemCode,
                                    SystemName = endpoints.DisplayName,
                                    Permission = users.Permission
                                }).Where(item => item.DIUserID == DIUserID).ToList();

                    return List;
                }


            }
            catch
            {
                throw;
            }
        }

        public IList<TableProperties> GetColumnsList(string TableName)
        {
            IACMDBContext dbcontext = new IACMDBContext();

            ObjectContext ctx = ((IObjectContextAdapter)dbcontext).ObjectContext;
            var cols = from meta in ctx.MetadataWorkspace.GetItems(DataSpace.CSpace)
                       .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                       from p in (meta as EntityType).Properties
                          .Where(p => p.DeclaringType.Name == TableName)
                       select new TableProperties
                       {
                           ColumName = p.Name,
                           DataType = p.TypeUsage.EdmType.Name
                       };


            return cols.ToList();
        }

        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<InhouseUserInfo> listINHUserInfo = new List<InhouseUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listINHUserInfo = oIACMDBContext.InhouseUserInfos.Where(INHusr => INHusr.DIUserId == UserID && INHusr.IsLocked == false && INHusr.IsDeleted == false).ToList();
                foreach (InhouseUserInfo inhusr in listINHUserInfo)
                {
                    if (inhusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = inhusr.UserId,
                            UserName = inhusr.AccountName,
                            FullName = inhusr.DisplayName,
                            AccountType = inhusr.AccountType,
                            LastLogonDate = inhusr.LastLoginDate,
                            EndpointSystemId = inhusr.EndPointSystemId,
                            EndPointSystemType = inhusr.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = inhusr.EndPoint.DisplayName
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
            IList<InhouseUserInfo> listINHUserInfo = new List<InhouseUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listINHUserInfo = oIACMDBContext.InhouseUserInfos.Where(INHusr => INHusr.EndPointSystemId == ApplicationID && INHusr.IsLocked == false && INHusr.IsDeleted == false).ToList();
                foreach (InhouseUserInfo inhusr in listINHUserInfo)
                {
                    if (inhusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = inhusr.UserId,
                            UserName = inhusr.AccountName,
                            FullName = inhusr.DisplayName,
                            AccountType = inhusr.AccountType,
                            LastLogonDate = inhusr.LastLoginDate,
                            EndpointSystemId = inhusr.EndPointSystemId,
                            EndPointSystemType = inhusr.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = inhusr.EndPoint.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            InhouseUserInfo InhouseUserInfo = new InhouseUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                InhouseUserInfo = oIACMDBContext.InhouseUserInfos.Where(inhusr => inhusr.UserId == EndpointUserId).FirstOrDefault();
                if (InhouseUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = InhouseUserInfo.AccountName;
                AccountDetail.DisplayName = InhouseUserInfo.DisplayName;
                AccountDetail.Email = InhouseUserInfo.Email;
                AccountDetail.AccountType = InhouseUserInfo.AccountType;
                AccountDetail.IsMappedAccount = InhouseUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = InhouseUserInfo.LastLoginDate;
                AccountDetail.Version = Convert.ToString(InhouseUserInfo.Version);
                AccountDetail.DIUserID = InhouseUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = InhouseUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }

        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                InhouseUserInfo InhouseUserInfo = Context.InhouseUserInfos.FirstOrDefault(ENDPT => ENDPT.UserId == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (InhouseUserInfo != null)
                {
                    InhouseUserInfo.DIUserId = AccountDetail.DIUserID; ;
                    InhouseUserInfo.LastModifiedDate = DateTime.Today;
                    InhouseUserInfo.MappingType = "Manual";
                    InhouseUserInfo.AccountType = AccountDetail.AccountType;
                    Context.SaveChanges();
                }
            }
        }

        public InhouseUserInfo GetInhouseInfoByUserNameAndSystemCode(string userName, string systemCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.FirstOrDefault(item => item.UserName == userName && item.InhouseSystemCode == systemCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetInhouseUserInfoByEndpointSystemId(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InhouseUserInfos.Where(item => item.EndPointSystemId == EndpointSystemId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteInhouseUserInfo(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    InhouseUserInfo userInfo = _context.InhouseUserInfos.Where(item => item.UserId == UserId).FirstOrDefault();
                    if (userInfo != null)
                    {
                        _context.InhouseUserInfos.Remove(userInfo);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveInhouseUserInfo(InhouseUserInfo inhouseuser)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (inhouseuser.UserId == 0)
                    {
                        _context.InhouseUserInfos.Attach(inhouseuser);
                        _context.Entry(inhouseuser).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var objEntry = _context.Entry(inhouseuser);
                        if (objEntry.State == EntityState.Detached)
                        {
                            var set = _context.Set<InhouseUserInfo>();
                            InhouseUserInfo attachedObj = set.Find(inhouseuser.UserId);

                            if (attachedObj != null)
                            {
                                var attachedEntry = _context.Entry(attachedObj);
                                attachedEntry.CurrentValues.SetValues(inhouseuser);
                            }
                        }
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

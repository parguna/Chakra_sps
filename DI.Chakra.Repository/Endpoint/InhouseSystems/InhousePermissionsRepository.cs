using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DI.Chakra.Repository
{
    public class InhousePermissionsRepository : RepositoryBase<InhousePermission>
    {
        public InhousePermissionsRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<InhouserEntitlementCustom> GetPermissionAndFunctionWithDescription(long endPointSystemId, string permission, string[] functions)
        {
            var q = (from perm in CurrentContext.InhousePermissions
                     join function in CurrentContext.InhouseFunctions
                     on perm.Name equals function.PermissionName
                     where perm.Name.Equals(permission, StringComparison.InvariantCultureIgnoreCase) && functions.Contains(function.FunctionName)
                     && perm.EndPointSystemId == endPointSystemId
                     select new InhouserEntitlementCustom
                     { Permission = perm.Name, PermissionDescription = perm.Description, Function = function.FunctionName, FunctionDescription = function.Description });
            return q.ToList();
        }

        public List<InhouserEntitlementCustom> GetPermissionWithDescription(long endPointSystemId, string[] permissions)
        {
            var q = (from perm in CurrentContext.InhousePermissions
                     where permissions.Contains(perm.Name)
                     && perm.EndPointSystemId == endPointSystemId
                     select new InhouserEntitlementCustom
                     { Permission = perm.Name, PermissionDescription = perm.Description }).ToList();
            return q;
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return CurrentContext.InhousePermissions
                       .Where(inh => inh.EndPointSystemId == endPointSystemId && inh.IsActive == true).
                       OrderBy(inh => inh.Name)
                       .Select(inh => new { Key = inh.Id, Value = inh.Name }).ToDictionary(o => o.Key, o => o.Value);
            }
            else
            {
                return CurrentContext.InhousePermissions
                       .Where(inh => inh.EndPointSystemId == endPointSystemId && inh.IsActive == true
                       && inh.Name.Contains(searchString)).
                       OrderBy(inh => inh.Name)
                       .Select(inh => new { Key = inh.Id, Value = inh.Name }).ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public Dictionary<long, string> GetByEndPointSystemIdExisting(long endPointSystemId, string searchString, long endpointUserId)
        {
            var mainResult = new Dictionary<long, string>();
            var query = $"select P.Id,P.Name from DI_INH_Permission P Inner join ( Select Entitlement,endpointUserId,EndPointSystemId FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId = {endPointSystemId}  AND EndpointUserId = {endpointUserId} AND IsDeleted = 0) E on P.EndPointSystemId = E.EndpointSystemId where E.EndpointSystemId = {endPointSystemId}  AND E.EndpointUserId = {endpointUserId}  AND P.Name = E.Entitlement AND P.IsActive = 1";
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = $"{query} AND P.Name like '%{searchString}%'";
            }
            var result = GetDataTableByQuery(query);
            foreach (DataRow row in result.Rows)
            {
                mainResult.Add(Convert.ToInt64(row["Id"]), Convert.ToString(row["Name"]));
            }
            return mainResult;
        }

        public Dictionary<long, string> GetAllPermissionsExceptExisting(long endPointSystemId, string searchString, long endpointUserId, bool isFunctionLevel, List<string> permissions)
        {
            var mainResult = new Dictionary<long, string>();
            var query = string.Empty;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                if (!isFunctionLevel)
                {
                    query = $"SELECT distinct A.Id, A.Name from DI_INH_Permission A where A.IsActive = 1 AND A.EndPointSystemId = {endPointSystemId} AND A.Name NOT IN(Select Entitlement FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId = {endPointSystemId}   AND EndpointUserId = {endpointUserId} AND IsDeleted = 0) order by A.Name";
                    //query = $"select distinct A.Id, A.Name from DI_INH_Permission A where A.IsActive = 1 AND A.EndPointSystemId = {endPointSystemId}  and A.Id not in (select PermissionId from DI_INH_UserPermissionFunction where EndPointSystemId = {endPointSystemId} and EndpointUserId = {endpointUserId}) order by A.Name";
                    if(permissions != null && permissions.Count > 0)
                    {
                        query = $"select distinct A.Id, A.Name from DI_INH_Permission A where A.IsActive = 1 AND A.EndPointSystemId = {endPointSystemId}  order by A.Name";
                    }
                }
                else
                {
                    query = $"select distinct A.Id, A.Name from DI_INH_Permission A inner join DI_INH_Functions B on A.Name = B.PermissionName" +
                        $" where A.IsActive = 1 AND A.EndpointSystemId = {endPointSystemId} and B.Id not in (select FunctionId from DI_INH_UserPermissionFunction C where C.EndpointUserId = {endpointUserId}) order by A.Name";
                }
            }
            else
            {
                if (!isFunctionLevel)
                {
                    //query = $"select distinct A.Id, A.Name from DI_INH_Permission A left join DI_INH_UserPermissionFunction B on A.Id = B.PermissionId and B.EndPointSystemId={endPointSystemId} and B.EndpointUserId={endpointUserId} where A.IsActive = 1 AND B.PermissionId is null and A.Name like '%{searchString}%' and A.EndPointSystemId = {endPointSystemId}";
                    query = $" select distinct A.Id, A.Name from DI_INH_Permission A left join (select Entitlement from DI_M_EntitlementReconcilation WHERE EndpointSystemId={endPointSystemId} and EndpointUserId={endpointUserId} and IsDeleted=0)AS B ON A.Name = B.Entitlement where A.EndPointSystemId = {endPointSystemId} AND A.IsActive = 1 AND B.Entitlement IS NULL AND A.Name like '%{searchString}%'";
                    if (permissions != null && permissions.Count > 0)
                    {
                        query = $"select distinct A.Id, A.Name from DI_INH_Permission A WHERE A.IsActive = 1 AND A.EndPointSystemId={endPointSystemId} AND A.Name like '%{searchString.Replace("'","''")}%'";
                    }
                }
                else
                {
                    query = $"select distinct A.Id, A.Name from DI_INH_Permission A inner join DI_INH_Functions B on A.Name = B.PermissionName" +
                        $" where A.IsActive = 1 AND A.EndpointSystemId = {endPointSystemId} and A.Name like '%{searchString.Replace("'", "''")}%' and B.Id not in " +
                        $"(select FunctionId from DI_INH_UserPermissionFunction C where C.EndpointUserId = {endpointUserId}) order by A.Name";
                }
            }
            var result = GetDataTableByQuery(query);
            foreach (DataRow row in result.Rows)
            {
                mainResult.Add(Convert.ToInt64(row["Id"]), Convert.ToString(row["Name"]));
            }
            return mainResult;
        }

        public List<InhousePermission> GetByRoleIdIncludeFunctions(long id)
        {
            return (from inhousePermission in CurrentContext.InhousePermissions.Include("Functions")
                    where inhousePermission.Id == id && inhousePermission.IsActive == true
                    select inhousePermission).ToList();
        }

        public void TruncateDI_INH_Permission()
        {
            string cmd = "Truncate table DI_INH_Permission";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
            CurrentContext.SaveChanges();
        }

        public InhousePermission GetByEndPoint(InhousePermission obj)
        {
            InhousePermission inhPerm = (from inhPer in this.CurrentContext.InhousePermissions.Include("Functions")
                                         join ePoint in this.CurrentContext.EndPoint
                                         on inhPer.EndPointSystemId equals ePoint.Id
                                         where inhPer.Name == obj.Name && inhPer.EndPointSystemId == obj.EndPointSystemId
                                         select inhPer).FirstOrDefault();

            return inhPerm;
        }
        public List<PresentationAccessRequestItem> GetAccessRequestItem(string searchString)
        {
            return (from inhouseper in CurrentContext.InhousePermissions
                    where inhouseper.Description.Contains(searchString)
                    orderby inhouseper.Description
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EntitlementType = Infrastructure.Enums.EntitlementType.Group,
                        EndpointType = EndpointType.InhouseSystems,
                        KeyId = inhouseper.Id,
                        EndpointSystemId = inhouseper.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.InhouseSystems,
                        ItemName = inhouseper.Name,
                        ItemDescription = inhouseper.Description,
                        ItemDetail = inhouseper.UserDefinedDesc
                    }).ToList();
        }


        public override long Insert(InhousePermission permission)
        {
            return base.Insert(permission);

        }

        public override void Update(InhousePermission permission)
        {
            base.Update(permission);
        }

        public List<InhousePermission> GetActiveRolesWithADGroups()
        {
            List<InhousePermission> results = new List<InhousePermission>();
            var data = (from perm in CurrentContext.InhousePermissions
                   join ep in CurrentContext.EndPoint on perm.EndPointSystemId equals ep.Id
                     where perm.IsActive == true && perm.ADGroups != null && perm.ADGroups.Length > 0 && ep.IsActive == true
                     select new
                     {
                         ADGroups = perm.ADGroups,
                         //Temporarly for Endpointname
                         Description = ep.DisplayName
                     }
                     ).ToList();

            if(data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    results.Add(new InhousePermission() {ADGroups = item.ADGroups, Description = item.Description });
                }
            }
            return results;
        }
    }
}

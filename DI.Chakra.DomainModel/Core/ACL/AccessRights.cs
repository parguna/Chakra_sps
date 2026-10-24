using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AccessRights : DomainModelBase<AccessRights, EntityModel.AccessRights>
    {
        public long RoleId { get; set; }
        public long? PagePermissionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Role role { get; set; }
        public PagePermission PagePermission { get; set; }

        
        public static long CloneRole(Role role, bool cloneMenu, bool cloneQL, bool cloneDB, long cloneRoleID, string currentUser)
        {
            List<AccessRights> list;
            role.Save(currentUser);
            long roleID = role.Id;

            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                try
                {
                    if (cloneMenu)
                    {
                        list = ORMapList(new AccessRightsRepository(transaction).GetByCloneRoleId(cloneRoleID));

                        foreach (AccessRights t in list)
                        {
                            AccessRights cloneRight = new AccessRights();
                            if (t.PagePermissionId > 0)
                            {
                                cloneRight.RoleId = roleID;
                                cloneRight.PagePermissionId = t.PagePermissionId;
                                cloneRight.Save(transaction);
                            }
                        }
                        transaction.Commit();
                    }

                    if (cloneDB)
                    {
                        string cloneARQLQuery = string.Format(@"INSERT INTO DI_M_DashboardAccessRights
SELECT {0}, DashBoardId, '{1}', GETDATE(), '{1}', GETDATE() FROM DI_M_DashboardAccessRights where RoleId = {2} ", roleID, currentUser, cloneRoleID); ;
                        AccessRights.ExecuteQuery(cloneARQLQuery);

                        string cloneQLQuery = string.Format(@"INSERT INTO DI_M_DefaultRoleDashboard
SELECT {0}, DashBoardId, Position, '{1}', GETDATE(), '{1}', GETDATE() FROM DI_M_DefaultRoleDashboard where RoleId = {2} ", roleID, currentUser, cloneRoleID); ;
                        AccessRights.ExecuteQuery(cloneQLQuery);

                    }

                    if (cloneQL)
                    {
                        string cloneARQLQuery = string.Format(@"INSERT INTO DI_M_QuickLinkAccessRights
SELECT {0}, QuickLinkId, '{1}', GETDATE(), '{1}', GETDATE() FROM DI_M_QuickLinkAccessRights where RoleId = {2} ", roleID, currentUser, cloneRoleID);
                        AccessRights.ExecuteQuery(cloneARQLQuery);

                        string cloneDBQuery = string.Format(@"INSERT INTO DI_M_DefaultRoleQuickLink
SELECT {0}, QuickLinkId, Position, '{1}', GETDATE(), '{1}', GETDATE() FROM DI_M_DefaultRoleQuickLink where RoleId = {2} ", roleID, currentUser, cloneRoleID);
                        AccessRights.ExecuteQuery(cloneDBQuery);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return roleID;
        }

      public static AccessRights GetByPagePermissionId(long PagePermissionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccessRights>(dataAccessTransaction)
                    .Get(a => a.PagePermissionId == PagePermissionId).FirstOrDefault());
            }
        }

        public static List<AccessRights> GetByPagePermission(long PagePermissionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRights>(dataAccessTransaction)
                    .Get(a => a.PagePermissionId == PagePermissionId).ToList());
            }
        }

        public static AccessRights GetByRoleId(long roleId,long PagePermissionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccessRights>(dataAccessTransaction)
                    .Get(a => a.PagePermissionId == PagePermissionId && a.RoleId == roleId).FirstOrDefault());
            }
        }
        public static IList<AccessRights> GetAccessRightsByRoleId(long roleID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRights>(dataAccessTransaction)
                  .Get(rights => rights.RoleId == roleID,rights=>rights.PagePermission).ToList());
            }
        }

        public static  bool DeleteById(long  rightsId,DataAccessTransaction dataAccessTransaction)
        {
            return new AccessRightsRepository(dataAccessTransaction).DeleteById(rightsId, dataAccessTransaction);
        }
        public static bool DeleteAccessRights(AccessRights accessRight)
        {
            
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    bool isDelete = false;
                    AccessRights delAccessRight = AccessRights.GetByRoleId(accessRight.RoleId, (long)accessRight.PagePermissionId);
                       
                    if (delAccessRight != null)
                    {
                        DeleteById(delAccessRight.Id, dataAccessTransaction);
                        isDelete = true;
                    }
                    dataAccessTransaction.Commit();
                    return isDelete;
                }

            
           

            
        }

        public static bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            return new AccessRightsRepository(dataAccessTransaction).DeleteByRoleId(roleId, dataAccessTransaction);
        }
    }
}

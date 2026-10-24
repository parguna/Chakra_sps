using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class Role : DomainModelBase<Role, EntityModel.Role>
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public int? RoleOwner { get; set; }
        public bool IsDefaultRole { get; set; }
        public List<User> Users { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }        
        public User Owner { get; set; }      

        public override string ToString()
        {
            return RoleName.ToString();
        }

        public static bool DoesExists(string roleName, long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbRole = GetByRoleName(roleName);
                return dbRole != null ? true : false;
            }
        }

        public static Role GetByIdIncludingUser(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Role>(tranaction).GetById(id, role => role.Owner, role => role.Users));
            }
        }

        public static Role GetByRoleName(string roleName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Role>(tranaction).Get(role => role.RoleName == roleName).FirstOrDefault());
            }
        }



        //public override void Save()  
        //{
        //    //using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    //{
        //    //    if (this.Id == 0)
        //    //    {
        //    //        this.Id = new RoleRepository(transaction).Insert(ORUnMap());
        //    //    }
        //    //    else
        //    //    {
        //    //        new RoleRepository(transaction).Update(ORUnMap());
        //    //    }

        //    //    transaction.Commit();
        //    //}
        //}
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new RoleRepository(transaction).Insert(ORUnMap());
                    //base.Save(transaction);
                    var query = string.Format(@"Declare @UrlId bigint  
DECLARE @RIDRole bigint  
DECLARE @PagePermissionId bigint  
DECLARE @PageId bigint  
SELECT @UrlId = Id FROM[DI_M_PageUrl] WHERE[Url] = '/Admin/Attestation/NReview.aspx'  

insert into DI_M_AccessRights   
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId  

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%PREFERENCES_USERPREFERENCE%'  
insert into DI_M_AccessRights  
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId   

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%HOME_MYGROUPTASK%'  
insert into DI_M_AccessRights  
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%REPORT_CustomReportViewer%'  
insert into DI_M_AccessRights  
SELECT {0}, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId
", this.Id);
                    new RoleRepository(transaction).ExecuteQuery(query);
                }
                else
                {
                    new RoleRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
        public static IList<Role> GetAllRolesByEndpointID(long endpointSystemId, long EndPointID)
        {
            Role role;
            IList<Role> list = new List<Role>();

            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                switch (endpointSystemId)
                {
                    case (int)EndpointType.SAP:
                        var tmpList2 = SAPRoleInfo.GetByEndpointSystemId(EndPointID).OrderBy(x => x.ROLE_CODE).Distinct();
                        //(from ri in oIACMDBContext.SAPRoleInfos
                        //            where ri.EndPointSystemId == EndPointID
                        //            orderby ri.ROLE_CODE
                        //            select new { RoleId = ri.Id, RoleName = ri.ROLE_CODE, Description = ri.ROLE_DESC }).Distinct();

                        if (tmpList2.Count() > 0)
                        {
                            foreach (var tLst in tmpList2)
                            {
                                role = new Role();
                                role.Id = CheckNull.AgainstInt(tLst.Id);
                                role.RoleName = CheckNull.AgainstString(tLst.ROLE_CODE);
                                role.RoleDescription = CheckNull.AgainstString(tLst.ROLE_DESC);
                                list.Add(role);
                            }
                        }
                        break;
                    case (int)EndpointType.SAPEnterprisePortal:
                        var tmpList3 = SAPEPRoleInfo.GetByEndPointId(EndPointID).OrderBy(x => x.RoleName).Distinct();

                        //(from ri in oIACMDBContext.SAPEPRoleInfos
                        //            where ri.EndPointSystemId == EndPointID
                        //            orderby ri.RoleName
                        //            select new { RoleId = ri.Id, ri.RoleName, Description = ri.RoleDescription }).Distinct();

                        if (tmpList3.Count() > 0)
                        {
                            foreach (var tLst in tmpList3)
                            {
                                role = new Role();
                                role.Id = CheckNull.AgainstInt(tLst.RoleId);
                                role.RoleName = CheckNull.AgainstString(tLst.RoleName);
                                role.RoleDescription = CheckNull.AgainstString(tLst.RoleDescription);
                                list.Add(role);
                            }
                        }
                        break;
                }
            }
            return list;

        }

        public static bool DeleteRole(long roleId, string currentUser)
        {
            Role role = Role.GetById(roleId, true);
            if (role != null)
            {
                try
                {
                    if (role.Users != null && role.Users.Count > 0)
                    {
                        return false;
                    }

                    using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
                    {
                        // delete access rights
                        AccessRights.DeleteByRoleId(roleId, transaction);
                        // delete dashboard
                        DashboardAccessRights.DeleteByRoleId(roleId, transaction);
                        // delete dashboard
                        QuickLinkAccessRights.DeleteByRoleId(roleId, transaction);
                        // delete role
                        Delete(transaction, roleId);

                        transaction.Commit();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }
    }
}

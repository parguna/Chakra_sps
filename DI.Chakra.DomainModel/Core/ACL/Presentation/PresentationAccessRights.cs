using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PresentationAccessRights : DomainModelBase<PresentationAccessRights, EntityModel.PresentationAccessRights>
    {
        public string RoleName { get; set; }
        public string MenuPath { get; set; }
        public string Permissions { get; set; }

        public static IList<PresentationAccessRights> GetExportData(long roleId)
        {
            List<PresentationAccessRights> result = new List<PresentationAccessRights>();

            string query = string.Format(@";WITH children AS
                                (
	                                SELECT id, ParentTabMenuId,PageId,CAST(ISNULL(menuname + '->' ,'')AS VARCHAR(4000)) AS Path
	                                FROM DI_M_TabMenu
	                                WHERE ParentTabMenuId IS NULL
	                                UNION ALL
	                                SELECT t.id,t.ParentTabMenuId, t.PageId,list= CAST(d.Path + ISNULL(t.menuname  + '->' ,'') AS VARCHAR(4000))
	                                FROM DI_M_TabMenu t
	                                INNER JOIN children  AS d
	                                ON t.ParentTabMenuId = d.id
                                )
                                SELECT
                                DISTINCT 
                                    RoleName,MenuPath,
                                    STUFF((
                                        SELECT ', ' + t.Name
                                        FROM (SELECT r.RoleName, SUBSTRING(path,0,LEN(path)-1) MenuPath,p.Name
                                FROM DI_M_PagePermission pp 
                                JOIN DI_M_AccessRights ar ON pp.Id=ar.PagePermissionId 
                                JOIN children c ON pp.PageId=c.PageId
                                JOIN DI_M_Permission p ON pp.PermissionId=p.Id
                                JOIN DI_M_Role r ON ar.RoleId =r.Id
                                WHERE r.Id={0}) t
                                        WHERE t.MenuPath = A.MenuPath
                                        FOR XML PATH('')
                                    ),1,1,'') AS [Permissions]
                                FROM (SELECT r.RoleName, SUBSTRING(path,0,LEN(path)-1) MenuPath,p.Name
                                FROM DI_M_PagePermission pp 
                                JOIN DI_M_AccessRights ar ON pp.Id=ar.PagePermissionId 
                                JOIN children c ON pp.PageId=c.PageId
                                JOIN DI_M_Permission p ON pp.PermissionId=p.Id
                                JOIN DI_M_Role r ON ar.RoleId =r.Id
                                WHERE r.Id={0}) A
                                GROUP BY MenuPath,RoleName,Name; ", roleId);

            using (DataAccessTransaction txn = new DataAccessTransaction())
            {
                var dataTable = new AccessRightsRepository(txn).GetDataTableByQuery(query);
                if (dataTable == null) return result;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    result.Add(new PresentationAccessRights() { RoleName = Convert.ToString(dataRow[0]), MenuPath = Convert.ToString(dataRow[1]), Permissions = Convert.ToString(dataRow[2]) });
                }
            }

            return result;
        }
    }
}

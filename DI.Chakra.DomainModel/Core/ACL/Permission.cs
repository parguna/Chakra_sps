using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Permission : DomainModelBase<Permission, EntityModel.Permission>
    {
        public string Name { get; set; }
        public string IconURL { get; set; }
        public string ClientScript { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string name)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.Permission>(dataAccessTransaction).Any(permission => permission.Name == name);
            }
        }

        public static bool HasAnyPagePermission(long permissionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Any(pagePermission => pagePermission.PermissionId == permissionId);
            }
        }

        public static List<Permission> GetBypageId(long pageId)
        {
            List<Permission> returnList = new List<Permission>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var List = PagePermission.GetByPageId(pageId, true);

                List.ForEach(lq =>
                {
                    returnList.Add(lq.Permission);
                });
            }
            return returnList;
        }
    }
}

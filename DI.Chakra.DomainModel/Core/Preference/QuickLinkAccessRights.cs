using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class QuickLinkAccessRights : DomainModelBase<QuickLinkAccessRights, EntityModel.QuickLinkAccessRights>
    {
       
        public long RoleId { get; set; }

        public long QuickLinkId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  QuickLink QuickLink { get; set; }

        public  Role Role { get; set; }

        public static bool DoesExists(long roleId, int quickLinkId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.QuickLinkAccessRights>(dataAccessTransaction).Any(d => (d.QuickLinkId == quickLinkId) && (d.RoleId == roleId));

            }
        }

        public new static void Delete(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                QuickLinkAccessRights delItem = QuickLinkAccessRights.GetById(id);

                DefaultRoleQuickLink drql = Mapper.Map<EntityModel.DefaultRoleQuickLink, DefaultRoleQuickLink>(new RepositoryBase<EntityModel.DefaultRoleQuickLink>(dataAccessTransaction).Get(Per => (Per.RoleId == delItem.RoleId) && (Per.QuickLinkId == delItem.QuickLinkId)).FirstOrDefault());

                delItem.Delete(dataAccessTransaction);

                drql.Delete(dataAccessTransaction);

                dataAccessTransaction.Commit();
            }
        }

        public static bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            return new QuickLinkRepository(dataAccessTransaction).DeleteByRoleId(roleId, dataAccessTransaction);
        }
    }
}

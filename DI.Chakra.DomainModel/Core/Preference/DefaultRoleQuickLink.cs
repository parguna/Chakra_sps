using System;
using System.Collections.Generic;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DI.Chakra.DomainModel
{
    public class DefaultRoleQuickLink : DomainModelBase<DefaultRoleQuickLink, EntityModel.DefaultRoleQuickLink>
    {
     
        public long RoleId { get; set; }

        public long QuickLinkId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public  QuickLink QuickLink { get; set; }

        public  Role Role { get; set; }

        public static bool DoesExists(long roleId, long quickLinkId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.DefaultRoleQuickLink>(dataAccessTransaction).Any(q => (q.QuickLinkId == quickLinkId) && (q.RoleId == roleId));
  
            }
        }



        public static bool DoesExistsByPosition(long dbPosition, long roleID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbItem = new RepositoryBase<EntityModel.DefaultRoleQuickLink>(dataAccessTransaction).Get(c => (c.Position == dbPosition) && (c.RoleId == roleID)).FirstOrDefault();
                return dbItem != null ? true : false;     

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



        public static IList<string> GetQLCatagoriesByRoleID(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new QuickLinkRepository(dataAccessTransaction).GetQLCatagoriesByRoleID(roleId);
            }
        }



    }
}

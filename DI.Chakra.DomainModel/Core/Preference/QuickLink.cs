using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class QuickLink : DomainModelBase<QuickLink, EntityModel.QuickLink>
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public long QuickLinkId { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public static IList<QuickLink> GetByRoleId(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new QuickLinkRepository(dataAccessTransaction).GetByRoleId(roleId));
            }
        }

        public static bool DoesExists(string qlName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbItem = new RepositoryBase<EntityModel.QuickLink>(dataAccessTransaction).Any(c => c.Name == qlName);
                return dbItem;
            }
        }
        public static List<QuickLink> GetByCategory(string category)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.QuickLink>(dataAccessTransaction).Get( q => q.Category == category).ToList());
            }
        }

        public static QuickLink GetByName(string id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.QuickLink>(dataAccessTransaction).Get(q => q.Name == id).FirstOrDefault());
            }
        }
    }
}

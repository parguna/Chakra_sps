using System;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class UserQuickLink : DomainModelBase<UserQuickLink, EntityModel.UserQuickLink>
    {        
        public long UserId { get; set; }

        public string Name { get; set; }

        public long QuickLinkId { get; set; }

        public int Position { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
      
        public  User User { get; set; }

        public  QuickLink QuickLink { get; set; }

        public static void Delete(long quickLinkId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                UserQuickLink link = ORMap(new RepositoryBase<EntityModel.UserQuickLink>(dataAccessTransaction).
                    Get(UQ => UQ.QuickLinkId == quickLinkId && UQ.UserId == userId).FirstOrDefault());
                if (link != null)
                {
                    UserQuickLink.Delete(link.Id);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static List<UserQuickLink> GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserQuickLink>(dataAccessTransaction).
                    Get(q => q.UserId == userId).ToList());
            }
        }
    }
}

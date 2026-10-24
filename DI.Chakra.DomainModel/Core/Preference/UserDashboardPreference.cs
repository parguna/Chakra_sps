using System;
using System.Collections.Generic;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class UserDashboardPreference : DomainModelBase<UserDashboardPreference, EntityModel.UserDashboardPreference>
    {   
        public long UserId { get; set; }

        public string Name { get; set; }

        public long DashboardId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public  User User { get; set; }
        
        public  Dashboard Dashboard { get; set; }
        
        public static void Delete(long dashBoardId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                UserDashboardPreference link = ORMap(new RepositoryBase<EntityModel.UserDashboardPreference>(dataAccessTransaction).Get(UQ => UQ.DashboardId == dashBoardId && UQ.UserId == userId).FirstOrDefault());
                if (link != null)
                {
                    UserDashboardPreference.Delete(link.Id);
                }
                dataAccessTransaction.Commit();
            }
        }
    }
}

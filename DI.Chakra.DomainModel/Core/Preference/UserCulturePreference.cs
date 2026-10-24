using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class UserCulturePreference:DomainModelBase<UserCulturePreference,EntityModel.UserCulturePreference>
    {
        public long UserId { get; set; }

        public string CultureName { get; set; }

        public string DateFormat { get; set; }

        public string TimeFormat { get; set; }

        public string LongDateFormat { get; set; }

        public string LongTimeFormat { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string LandingPageMenuName { get; set; }
        public static List<UserCulturePreference> GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.UserCulturePreference>(dataAccessTransaction).Get(culturePreference => culturePreference.UserId == userId).ToList());
            }
        }
    }
}

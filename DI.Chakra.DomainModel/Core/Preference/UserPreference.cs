using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
  public  class UserPreference: DomainModelBase<UserPreference, EntityModel.UserPreference>
    {
        public long TabMenuId { get; set; }

        public string MenuName { get; set; }

        public int Position { get; set; }

        public string TargetUrl { get; set; }


        public static IList<UserPreference> GetDefaultMenus(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new UserPreferenceRepository(dataAccessTransaction).GetDefaultMenus(roleId).ToList());
            }
        }

        public static IList<UserPreference> GetUserPreferenceMenus(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new UserPreferenceRepository(dataAccessTransaction).GetUserPreferenceMenus(userId).ToList());
            }
        }
    }
}

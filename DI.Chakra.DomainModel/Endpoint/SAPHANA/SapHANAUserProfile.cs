using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SapHANAUserProfile : DomainModelBase<SapHANAUserProfile, EntityModel.SapHANAUserProfile>
    {
        public string UserName { get; set; }

        public string ProfileName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }

        public static IList<SapHANAUserProfile> GetByProfileCode(string profileCode)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapHANAUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode).ToList());
            }
        }

        public static SapHANAUserProfile GetByProfileCode(string profileCode,string userName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SapHANAUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode && profileInfo.UserName==userName).FirstOrDefault());
            }
        }

        public static SapHANAUserProfile GetByProfileCode(string profileCode, string userName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SapHANAUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode && profileInfo.UserName == userName && profileInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static IList<SapHANAUserProfile> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapHANAUserProfile>(transaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId && profileInfo.LastModifiedDate<modifiedDate).ToList());
            }
        }
    }
}

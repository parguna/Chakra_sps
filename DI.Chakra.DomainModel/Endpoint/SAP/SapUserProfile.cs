using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SapUserProfile : DomainModelBase<SapUserProfile, EntityModel.SapUserProfile>
    {
        public string UserName { get; set; }

        public string ProfileName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }

        public static IList<SapUserProfile> GetByProfileCode(string profileCode)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode).ToList());
            }
        }

        public static SapUserProfile GetByProfileCode(string profileCode,string userName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SapUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode && profileInfo.UserName==userName).FirstOrDefault());
            }
        }

        public static SapUserProfile GetByProfileCode(string profileCode, string userName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SapUserProfile>(transaction).Get(profileInfo => profileInfo.ProfileName == profileCode && profileInfo.UserName == userName && profileInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static IList<SapUserProfile> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapUserProfile>(transaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId && profileInfo.LastModifiedDate<modifiedDate).ToList());
            }
        }

        public IList<SapUserProfile> GetDeletedSAPUserProfile(long endPointSystemId)
        {
            DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
            IList<SapUserProfile> listDeletedSAPUserProfile = new List<SapUserProfile>();
            objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
            if (objDiscoveryProcess != null)
            {
                Process process = Process.GetByProcessId(objDiscoveryProcess.Id);

                if (process != null)
                {
                    listDeletedSAPUserProfile = SapUserProfile.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                }
            }

            return listDeletedSAPUserProfile;
        }

        public static void SaveProfiles(IList<SapUserProfile> list, long EndpointSystemId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SapUserProfile> listExistingSapUserProfiles = new List<SapUserProfile>();
                SapUserProfile sapexistingSapUserProfile = null;
                listExistingSapUserProfiles = SapUserProfile.GetAll();
                foreach (SapUserProfile profile in list)
                {
                    sapexistingSapUserProfile = listExistingSapUserProfiles.Where(t => t.ProfileName == profile.ProfileName && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                    if (sapexistingSapUserProfile == null)
                    {
                        profile.Save(dataAccessTransaction);
                    }
                    else
                    {
                        profile.Id = sapexistingSapUserProfile.Id;
                        profile.ProfileName = sapexistingSapUserProfile.ProfileName;
                        profile.Save(dataAccessTransaction);
                    }
                }

                dataAccessTransaction.Commit();
            }
        }
    }
}

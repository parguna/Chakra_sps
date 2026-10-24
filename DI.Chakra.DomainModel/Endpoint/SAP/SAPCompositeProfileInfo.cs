using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPCompositeProfileInfo : DomainModelBase<SAPCompositeProfileInfo, EntityModel.SAPCompositeProfileInfo>
    {
        

        public long EndPointSystemId { get; set; }

        public string PROFILE_CODE { get; set; }

        public string PROFILE_DESC { get; set; }

        public string SUBProfile_Code { get; set; }

        public string TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }


        public static SAPCompositeProfileInfo GetByProfileCode(string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPCompositeProfileInfo>(dataAccessTransaction).Get(compositeProfile => compositeProfile.SUBProfile_Code == profileCode).FirstOrDefault());
            }
        }

        public static SAPCompositeProfileInfo GetByProfileCode(string profileCode, string subProfileCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPCompositeProfileInfo>(dataAccessTransaction).Get(compositeProfile => compositeProfile.SUBProfile_Code == subProfileCode
                    && compositeProfile.PROFILE_CODE == profileCode && compositeProfile.EndPointSystemId == endPointSystemId, true).FirstOrDefault());
            }
        }

        public static IList<SAPCompositeProfileInfo> GetByModifiedDate(long endPointSystemId,DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPCompositeProfileInfo>(dataAccessTransaction).Get(compositeProfile => compositeProfile.EndPointSystemId == endPointSystemId && compositeProfile.LastModifiedDate < modifiedDate, true).ToList());
            }
        }


        public static IList<SAPCompositeProfileInfo> GetDeletedCompositeProfiles(long endpointSystemid)
        {
            
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPCompositeProfileInfo> listDeletedProfiles = new List<SAPCompositeProfileInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointSystemid).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process =Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                   
                    if (process != null)
                    {
                        listDeletedProfiles = SAPCompositeProfileInfo.GetByModifiedDate(endpointSystemid, process.ProcessStartAt);
                    }
                }

                return listDeletedProfiles;

        }

        public static void SaveCompositeProfiles(IList<SAPCompositeProfileInfo> list, long endpointSystemId, string currentUser)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPCompositeProfileInfo> listExistingSAPProfileInfos = new List<SAPCompositeProfileInfo>();
                SAPCompositeProfileInfo sapexistingSAPProfileInfo = null;
                listExistingSAPProfileInfos = SAPCompositeProfileInfo.GetAll();
                foreach (SAPCompositeProfileInfo profile in list)
                {
                    sapexistingSAPProfileInfo = listExistingSAPProfileInfos.Where(t => t.PROFILE_CODE == profile.PROFILE_CODE && t.SUBProfile_Code == profile.SUBProfile_Code && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapexistingSAPProfileInfo == null)
                    {
                        profile.Save(dataAccessTransaction);
                    }
                    else
                    {
                        profile.Id = sapexistingSAPProfileInfo.Id;
                        profile.PROFILE_CODE = sapexistingSAPProfileInfo.PROFILE_CODE;
                        profile.SUBProfile_Code = sapexistingSAPProfileInfo.SUBProfile_Code;
                        profile.Save(dataAccessTransaction);
                    }
                }

                dataAccessTransaction.Commit();
            }

            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            //{
            //    SAPCompositeProfileInfo sapexistingProfile = null;

            //    foreach (SAPCompositeProfileInfo profile in list)
            //    {

            //        sapexistingProfile = GetByProfileCode(profile.PROFILE_CODE, profile.SUBProfile_Code, endpointSystemId);

            //        sapexistingProfile.Save(dataAccessTransaction);
            //    }
            //    dataAccessTransaction.Commit();
            //}

          
        }

    }
}

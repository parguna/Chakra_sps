using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPHANAProfileInfoRepository : RepositoryBase<SAPHANAProfileInfo>
    {
        public SAPHANAProfileInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationSAPHANAProfileReport> GetSAPUserToProfilesReportData(long endPointSystemId)
        {
            return (from sapProfile in CurrentContext.SAPHANAProfileInfo
                    where sapProfile.EndPointSystemId == endPointSystemId
                    select new PresentationSAPHANAProfileReport

                    {
                        SAP_PROFILEID = (int)sapProfile.Id,
                        PROFILE_CODE = sapProfile.PROFILE_CODE,
                        PROFILE_DESC = sapProfile.PROFILE_DESC

                    }).ToList();
        }
        public Dictionary<long, string> GetSAPProfileBySearchString(long endpointId, string searchString)
        {
            Dictionary < long, string> sapProfiles=  (from sapProfile in CurrentContext.SAPHANAProfileInfo
                    where sapProfile.PROFILE_CODE.Contains(searchString) && sapProfile.EndPointSystemId == endpointId
                    select new { sapProfile.Id, sapProfile.PROFILE_CODE }).Take(100)
                    .Distinct().OrderBy(s => s.PROFILE_CODE)
                    .ToDictionary(c => c.Id, c => c.PROFILE_CODE);
            return sapProfiles;
        }
    }
}

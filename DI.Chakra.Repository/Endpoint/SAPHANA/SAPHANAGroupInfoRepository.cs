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
    public class SAPHANAGroupInfoRepository : RepositoryBase<SAPHANAGroupInfo>
    {
        public SAPHANAGroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void UpdateSAPUserinfoSAPGroupInfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string cmd = "UPDATE [SAPHANAUserinfoSAPHANAGroupInfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPUserinfo_Id = " + id + "";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string query = "UPDATE [SAPHANARoleInfoSAPHANAUserinfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPUserinfo_Id = " + id;
            this.CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<PresentationSAPHANAUsersToProfileReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            return (from sapUserprofile in CurrentContext.SapUserProfile
                    where sapUserprofile.EndPointSystemId == endpointSystemId
                    select new PresentationSAPHANAUsersToProfileReport
                    {
                        UserName = sapUserprofile.UserName,
                        ProfileCode = sapUserprofile.ProfileName
                    }).ToList();
        }
    }
}

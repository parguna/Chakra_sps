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
    public class SAPGroupInfoRepository : RepositoryBase<SAPGroupInfo>
    {
        public SAPGroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void UpdateSAPUserinfoSAPGroupInfoByEndPointSystemId(long endPointSystemId,long id)   
        {
            string cmd = "UPDATE [SAPUserinfoSAPGroupInfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPUserinfo_Id = " + id+""; 
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string query = "UPDATE [SAPRoleInfoSAPUserinfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPUserinfo_Id = " + id;
            this.CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<PresentationSAPUsersToProfileReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            return (from sapUserprofile in CurrentContext.SapUserProfile
                    where sapUserprofile.EndPointSystemId == endpointSystemId
                    select new PresentationSAPUsersToProfileReport
                    {
                        UserName = sapUserprofile.UserName,
                        ProfileCode = sapUserprofile.ProfileName
                    }).ToList();
        }

        
    }
}


using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class O365GroupInfoRepository : RepositoryBase<Office365GroupInfo>
    {

        public O365GroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        
        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.Office365GroupInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.distinguishedName);

        }
        public Office365GroupInfo GetByEndPointSystemIdAndName(long endPointSystemId,string groupName)
        {
            return CurrentContext.Office365GroupInfo.FirstOrDefault(ep => ep.EndPointSystemId == endPointSystemId && ep.name == groupName);

        }
        public void DeleteGroupUser(long groupId, long endpointsystemid)
        {
            string cmd = "DELETE FROM [Office365UserInfoOffice365GroupInfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  Office365GroupInfo_Id = " + groupId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
    }
}

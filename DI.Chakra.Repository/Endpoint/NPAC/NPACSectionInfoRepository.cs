using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class NPACSectionInfoRepository : RepositoryBase<NPACSectionInfo>
    {
        public NPACSectionInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public NPACSectionInfo GetByOrganizationCode(string orgCode)
        {
            NPACSectionInfo npacSectionInfo = null;
            npacSectionInfo = (from sectionInfo in CurrentContext.NPACSectionInfo
                    join orgInfo in CurrentContext.NOrganisations on sectionInfo.HRSectionName equals orgInfo.Name
                    where orgInfo.Code == orgCode
                    select sectionInfo).FirstOrDefault();

            return npacSectionInfo;
        }
    }
}

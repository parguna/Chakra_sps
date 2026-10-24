using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class AttestationCampaignRepository : RepositoryBase<AttestationCampaign>
    {
        public AttestationCampaignRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<AttestationCampaign> GetAttestationCampaignsForReport()
        {
            return (from attcmp in CurrentContext.AttestationCampaigns
                    join attcmpReport in CurrentContext.AttestationCampaignReports on attcmp.Id equals attcmpReport.AttestationCampaignId into attcmpRpt
                    from report in attcmpRpt.DefaultIfEmpty()
                    where attcmp.ParentCampaign == null && attcmp.ChildSplitCount > 0 && attcmp.EndDate != null
                    && report == null
                    select attcmp).ToList();
        }
        public List<AttestationCampaign> GetChildCampaignsIncludingStatus(long parentCampaignId)
        {
            return (from attpro in CurrentContext.AttestationCampaigns
                    where attpro.ParentCampaign.HasValue &&
                    attpro.ParentCampaign.Value == parentCampaignId &&
                    attpro.Status != Infrastructure.Enums.AttestationCampaignStatus.IgnoreNoUserFound
                    select new 
                    {
                        Id = attpro.Id,
                        Comments = attpro.Comments,
                        CampaignName = attpro.CampaignName,
                        Status = attpro.Status
                    }).ToList().Select(i=> new AttestationCampaign()
                    {
                        Id = i.Id,
                        Comments = i.Comments,
                        CampaignName = i.CampaignName,
                        Status = i.Status
                    }).ToList();
        }
    }
}

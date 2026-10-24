using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Linq;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Workflow.Processor
{
    public class SetAttestationCampaignEndDate
    {
        public void Execute(long campaignId)
        {
            try
            {
                AttestationCampaign campaign = AttestationCampaign.GetByCampaignId(campaignId);
                campaign.EndDate = DateTime.Now;
                campaign.Save(MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, "Error while executing the SetAttestationCampaignEndDate activity", ex);
            }
        }
    }
}
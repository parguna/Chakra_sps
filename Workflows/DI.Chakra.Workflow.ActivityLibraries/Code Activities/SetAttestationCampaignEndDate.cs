using DI.Chakra.DomainModel;
using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class SetAttestationCampaignEndDate : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> CampaignId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            long campId = this.CampaignId.Get(context);

            Processor.SetAttestationCampaignEndDate processor = new Processor.SetAttestationCampaignEndDate();
            processor.Execute(campId);
        }

    }
}
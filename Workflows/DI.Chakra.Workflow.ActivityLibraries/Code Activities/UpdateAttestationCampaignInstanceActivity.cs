using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class UpdateAttestationCampaignInstanceActivity : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        [RequiredArgument]
        public InArgument<long> CampaignId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid workflowInstanceId = WorkflowInstanceId.Get(context);
            long campaignId = this.CampaignId.Get(context);

            Processor.UpdateAttestationCampaignInstance processor = new Processor.UpdateAttestationCampaignInstance();
            processor.Execute(campaignId, workflowInstanceId);
        }

    }
}
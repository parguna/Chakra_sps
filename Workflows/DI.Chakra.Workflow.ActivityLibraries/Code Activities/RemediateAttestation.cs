using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class RemediateAttestation : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<long> CampaignId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);
            var campaignId = this.CampaignId.Get(context);

            Processor.RemediateAttestation processor = new Processor.RemediateAttestation();
            processor.Execute(task, campaignId);
        }

    }
}

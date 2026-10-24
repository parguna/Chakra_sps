using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class UpdateAttestationDateForDeltaActivity : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<long> CampaignId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            var campid = this.CampaignId.Get(context);

            Processor.UpdateAttestationDateForDelta processor = new Processor.UpdateAttestationDateForDelta();
            processor.Execute(task, campid);
        }

    }
}
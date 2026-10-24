using DI.Chakra.DomainModel;
using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class SaveAttestationSnapshot : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }
        
        [RequiredArgument]
        public InArgument<long> CampaignId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);
            long campId = this.CampaignId.Get(context);

            Processor.SaveAttestationSnapshot processor = new Processor.SaveAttestationSnapshot();
            processor.Execute(task, campId);
        }

    }
}
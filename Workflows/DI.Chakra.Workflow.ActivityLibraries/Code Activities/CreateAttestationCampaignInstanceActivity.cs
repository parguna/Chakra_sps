using DI.Chakra.DomainModel;
using System;
using System.Activities;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class CreateAttestationCampaignInstanceActivity : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        public OutArgument<long> CampaignId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid workflowInstanceId = WorkflowInstanceId.Get(context);
            long campId = 0;
            var task = this.Task.Get(context);

            if (task != null)
            {
                Processor.CreateAttestationCampaignInstance processor = new Processor.CreateAttestationCampaignInstance();
                campId = processor.Execute(task, workflowInstanceId);
            }
            this.CampaignId.Set(context, campId);
        }

    }
}
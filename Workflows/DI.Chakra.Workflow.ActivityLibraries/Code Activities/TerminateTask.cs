using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class TerminateTask : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.TerminateTask processor = new Processor.TerminateTask();
            processor.Execute(workflowInstanceId);
        }

    }
}

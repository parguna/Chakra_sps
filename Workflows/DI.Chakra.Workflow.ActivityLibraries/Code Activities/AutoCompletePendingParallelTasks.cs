using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class AutoCompletePendingParallelTasks : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.AutoCompletePendingParallelTasks processor = new Processor.AutoCompletePendingParallelTasks();
            processor.Execute(workflowInstanceId);
        }

    }
}

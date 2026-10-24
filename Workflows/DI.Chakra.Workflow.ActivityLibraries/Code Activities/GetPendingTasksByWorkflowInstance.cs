using DI.Chakra.DomainModel;
using System;
using System.Activities;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class GetPendingTasksByWorkflowInstance : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        public OutArgument<List<Task>> PendingTasks { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.GetPendingTasksByWorkflowInstance processor = new Processor.GetPendingTasksByWorkflowInstance();
            var response = processor.Execute(workflowInstanceId);

            this.PendingTasks.Set(context, response.PendingTasks);
        }
    }
}

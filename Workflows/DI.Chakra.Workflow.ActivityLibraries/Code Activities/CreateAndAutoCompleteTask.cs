using DI.Chakra.DomainModel;
using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class CreateAndAutoCompleteTask : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

            var task = this.Task.Get(context);
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.CreateAndAutoCompleteTask processor = new Processor.CreateAndAutoCompleteTask();
            processor.Execute(task, workflowInstanceId);
        }

    }
}

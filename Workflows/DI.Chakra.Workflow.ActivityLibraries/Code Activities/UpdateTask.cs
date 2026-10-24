using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class UpdateTask : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);
            var workflowConfiguration = this.WorkflowConfiguration.Get(context);

            Processor.UpdateTask processor = new Processor.UpdateTask();
            processor.Execute(task, workflowConfiguration);
        }

    }
}

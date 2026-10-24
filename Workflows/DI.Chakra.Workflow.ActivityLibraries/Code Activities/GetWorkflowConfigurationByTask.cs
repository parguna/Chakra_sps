using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class GetWorkflowConfigurationByTask : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        public OutArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.GetWorkflowConfigurationByTask processor = new Processor.GetWorkflowConfigurationByTask();
            var response = processor.Execute(task);

            this.WorkflowConfiguration.Set(context, response.WorkflowConfiguration);
        }
    }
}

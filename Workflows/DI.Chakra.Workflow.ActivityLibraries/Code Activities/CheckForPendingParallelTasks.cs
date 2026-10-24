using System.Activities;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class CheckForPendingParallelTasks : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        public OutArgument<bool> CanContinueApproval { get; set; }
        public OutArgument<bool> IsPendingTasksExists { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);
            var workflowConfiguration = this.WorkflowConfiguration.Get(context);

            Processor.CheckForPendingParallelTasks processor = new Processor.CheckForPendingParallelTasks();
            var response = processor.Execute(task, workflowConfiguration);

            this.IsPendingTasksExists.Set(context, response.IsPendingTasksExists);
            this.CanContinueApproval.Set(context, response.CanContinueApproval);
        }

    }
}

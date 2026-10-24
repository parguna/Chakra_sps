using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class GetNextLevelApprovalTasks : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        [RequiredArgument]
        public InArgument<byte> ApprovalLevel { get; set; }

        public OutArgument<Task> NextLevelTask { get; set; }

        public OutArgument<bool> IsApproved { get; set; }

        public OutArgument<bool> SkipNextLevel { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Task task = this.Task.Get(context);
            var workflowConfiguration = this.WorkflowConfiguration.Get(context);
            byte currentState = this.ApprovalLevel.Get(context);

            Processor.GetNextLevelApprovalTasks processor = new Processor.GetNextLevelApprovalTasks();
            var response = processor.Execute(task, workflowConfiguration, currentState);

            this.NextLevelTask.Set(context, response.NextLevelTask);
            this.IsApproved.Set(context, response.IsApproved);
            this.SkipNextLevel.Set(context, response.SkipNextLevel);
        }
    }
}

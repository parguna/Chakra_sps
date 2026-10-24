using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class SendNotificationEmail : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        [RequiredArgument]
        public InArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        public OutArgument<bool> IsMailSent { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            WorkflowConfiguration currentWorkflowConfiguration = WorkflowConfiguration.Get(context);
            Task currentTask = Task.Get(context);

            Processor.SendNotificationEmail processor = new Processor.SendNotificationEmail();
            var response = processor.Execute(currentTask, currentWorkflowConfiguration);

            IsMailSent.Set(context, response.IsMailSent);
        }

    }
}

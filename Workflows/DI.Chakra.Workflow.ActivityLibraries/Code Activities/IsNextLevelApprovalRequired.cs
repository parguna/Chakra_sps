using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class IsNextLevelApprovalRequired : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        public OutArgument<bool> IsRequired { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.IsNextLevelApprovalRequired processor = new Processor.IsNextLevelApprovalRequired();
            var response = processor.Execute(task);

            this.IsRequired.Set(context, response.IsRequired);
        }
    }
}

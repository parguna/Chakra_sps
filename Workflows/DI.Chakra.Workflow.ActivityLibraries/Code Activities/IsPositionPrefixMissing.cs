using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class IsPositionPrefixMissing: CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        public OutArgument<bool> PositionPrefixMissing { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.IsPositionPrefixMissing processor = new Processor.IsPositionPrefixMissing();
            var response = processor.Execute(task);
            if (MasterConfigHelper.MockSkipApproval && task.IsActionOnlyRequiredTask == false)
            {
                task.Comments = $"Auto approved because of MockSkipApproval.";
                task.ActionDate = System.DateTime.Now;
                response.PositionPrefixMissing = false;
            }
            this.PositionPrefixMissing.Set(context, response.PositionPrefixMissing);
        }
    }
}

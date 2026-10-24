using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class ResetReviewCompleteFlagForNextApprovalLevel : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.ResetReviewCompleteFlagForNextApprovalLevel processor = new Processor.ResetReviewCompleteFlagForNextApprovalLevel();
            processor.Execute(workflowInstanceId);
        }

    }
}

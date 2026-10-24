using DI.Chakra.DomainModel;
using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class WithdrawRequest : CodeActivity
    {
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowInstanceId = this.WorkflowInstanceId.Get(context);

            Processor.WithdrawRequest processor = new Processor.WithdrawRequest();
            processor.Execute(workflowInstanceId);
        }

    }
}

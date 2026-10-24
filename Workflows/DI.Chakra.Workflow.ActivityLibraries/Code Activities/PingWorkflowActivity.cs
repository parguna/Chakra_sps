using System.Activities;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class PingWorkflowActivity : CodeActivity
    {

        protected override void Execute(CodeActivityContext context)
        {
            Processor.PingWorkflowActivity processor = new Processor.PingWorkflowActivity();
            processor.Execute();
        }

    }
}
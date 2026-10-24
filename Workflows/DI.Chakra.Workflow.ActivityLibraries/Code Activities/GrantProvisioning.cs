using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class GrantProvisioning : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.GrantProvisioning processor = new Processor.GrantProvisioning();
            processor.Execute(task);
        }
    }
}

using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class UpdateEscalationFlag : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.UpdateEscalationFlag processor = new Processor.UpdateEscalationFlag();
            processor.Execute(task);
        }

    }
}

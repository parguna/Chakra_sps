using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class IsWorkflowStateConflict : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Task> IncomingTask { get; set; }

        [RequiredArgument]
        public InArgument<byte> CurrentState { get; set; }

        public OutArgument<bool> StateConflict { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.IncomingTask.Get(context);
            var currentState = this.CurrentState.Get(context);

            Processor.IsWorkflowStateConflict processor = new Processor.IsWorkflowStateConflict();
            var response = processor.Execute(task, currentState);

            this.StateConflict.Set(context, response.StateConflict);
        }
    }
}

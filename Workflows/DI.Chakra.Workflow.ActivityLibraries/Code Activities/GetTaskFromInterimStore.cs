using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class GetTaskFromInterimStore : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> TaskId { get; set; }

        public OutArgument<Task> Task { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var taskId = this.TaskId.Get(context);

            Processor.GetTaskFromInterimStore processor = new Processor.GetTaskFromInterimStore();
            var response = processor.Execute(taskId);

            this.Task.Set(context, response.Task);
        }
    }
}

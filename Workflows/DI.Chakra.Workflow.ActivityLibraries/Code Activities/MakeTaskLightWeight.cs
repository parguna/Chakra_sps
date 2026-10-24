using DI.Chakra.DomainModel;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class MakeTaskLightWeight : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Task> Task { get; set; }

        public OutArgument<Task> LightWeightTask { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var task = this.Task.Get(context);

            Processor.MakeTaskLightWeight processor = new Processor.MakeTaskLightWeight();
            var response = processor.Execute(task);

            this.LightWeightTask.Set(context, response.LightWeightTask);
        }

    }
}

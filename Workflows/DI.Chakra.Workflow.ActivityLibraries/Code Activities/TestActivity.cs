using DI.Chakra.DomainModel;
using System;
using System.Activities;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class TestActivity : CodeActivity
    {
        public InArgument<string> Info { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var info = this.Info.Get(context);

            Processor.TestActivity processor = new Processor.TestActivity();
            processor.Execute(info);
        }

    }
}

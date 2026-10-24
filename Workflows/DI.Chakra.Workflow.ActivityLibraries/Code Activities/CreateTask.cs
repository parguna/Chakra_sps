using DI.Chakra.DomainModel;
using System;
using System.Activities;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.ActivityLibraries
{
    public class CreateTask : CodeActivity
    {

        [RequiredArgument]
        public InArgument<List<Task>> TaskList { get; set; }

        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        [RequiredArgument]
        public InArgument<WorkflowConfiguration> WorkflowConfiguration { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<Task> taskList = TaskList.Get(context);
            WorkflowConfiguration currentWorkflowConfiguration = WorkflowConfiguration.Get(context);
            Guid workflowInstanceId = WorkflowInstanceId.Get(context);

            Processor.CreateTask processor = new Processor.CreateTask();
            processor.Execute(taskList, currentWorkflowConfiguration, workflowInstanceId);
        }

    }
}

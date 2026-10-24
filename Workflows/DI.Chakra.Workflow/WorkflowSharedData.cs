using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepIdentity.Workflow
{
    public class WorkflowSharedData
    {
       public IList<WorkflowModule> listmodules;

        private static WorkflowSharedData crntInst;

        private WorkflowSharedData()
        {
        }

        public static WorkflowSharedData GetWorkflowSharedData()
        {
            if (crntInst == null)
                crntInst = new WorkflowSharedData();

            return crntInst;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class WorkflowReminderService 
    {
        public WorkflowReminderProcess workflowReminderProcess { get; private set; }
        public WorkflowReminderService()
        {
            workflowReminderProcess = new WorkflowReminderProcess();
        }

        public void Start()
        {
            workflowReminderProcess.Start();
        }

        public void Stop()
        {
            workflowReminderProcess.Stop();
        }
    }
}

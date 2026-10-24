using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
namespace DI.Chakra.Workflow.Processor
{
    public class PingWorkflowActivity
    {
        public void Execute()
        {
            DILog.AttestationLogger.WriteLog(LogLevelType.Info, "Workflow pinged for Task ");
        }
    }
}
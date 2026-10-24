using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{
    public class GetPendingTasksByWorkflowInstance
    {
        public GetPendingTasksByWorkflowInstanceResponse Execute(Guid workflowInstanceId)
        {
            RequestType requestType = RequestType.None;
            List<Task> pendingTasks = new List<Task>();
            try
            {
                pendingTasks = Task.GetPendingTaskByworkflowInstanceId(workflowInstanceId);
                if (pendingTasks.Count > 0)
                {
                    requestType = (RequestType)pendingTasks[0].RequestTypeId;
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the GetPendingTasksByWorkflowInstance activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null,LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }

            return new GetPendingTasksByWorkflowInstanceResponse()
            {
                PendingTasks = pendingTasks
            };
        }
    }
}

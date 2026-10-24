using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class GetWorkflowConfigurationByTask
    {
        public GetWorkflowConfigurationByTaskResponse Execute(Task task)
        {
            RequestType requestType = RequestType.None;
            WorkflowConfiguration workflowConfiguration = null;
            try
            {
                requestType = (RequestType)task.RequestTypeId;

                workflowConfiguration = WorkflowConfiguration.GetWorkflowConfigurationDetailById(task.WorkflowConfigurationId);
            }
            catch (Exception ex)
            {
                string message = "Error while executing the GetWorkflowConfigurationByTask activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Compliance)
                {
                    //DILog.Compliance.WriteLog(LogType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }

            return new GetWorkflowConfigurationByTaskResponse()
            {
                WorkflowConfiguration = workflowConfiguration
            };
        }
    }
}

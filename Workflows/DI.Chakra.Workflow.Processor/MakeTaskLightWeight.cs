using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
namespace DI.Chakra.Workflow.Processor
{
    public class MakeTaskLightWeight
    {
        public MakeTaskLightWeightResponse Execute(Task task)
        {
            RequestType requestType = RequestType.None;
            try
            {
                if(task != null)
                {
                    requestType = (RequestType)task.RequestTypeId;

                    if (requestType == RequestType.Provisioning)
                    {
                        //clear the provisioning task details
                        task.AccessRequest = null;
                    }

                    if (requestType == RequestType.BulkRequest)
                    {
                        task.BulkRequest = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the MakeTaskLightWeight activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,   LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }

            return new MakeTaskLightWeightResponse()
            {
                LightWeightTask = task
            };
        }
    }
}

using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class GetTaskFromInterimStore
    {
        public GetTaskFromInterimStoreResponse Execute(long taskId)
        {
            RequestType requestType = RequestType.None;
            Task task = new Task();
            try
            {
                var originalTask = Task.GetById(taskId);

                // if attestation, take task from database
                if ((RequestType)originalTask.RequestTypeId == RequestType.Attestation)
                {
                    task = originalTask;
                }
                else
                {
                    // get from interim store
                    var interimStore = InterimStore.GetByTaskId(taskId, true, InterimStoreItemType.Task);

                    task = UtilityMethods.Deserialize<Task>(interimStore.XmlData);
                }

                requestType = (RequestType)task.RequestTypeId;
                //PresentationAttestationSnapshotActionsForTask
            }
            catch (Exception ex)
            {
                string message = "Error while executing the GetTaskFromInterimStore activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);      
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

            return new GetTaskFromInterimStoreResponse()
            {
               Task = task 
            };
        }
    }
}

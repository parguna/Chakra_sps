using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using DI.Chakra.DomainModel; 

namespace DI.Chakra.Workflow.Processor
{
    public class AutoCompletePendingParallelTasks
    {
        public void Execute(Guid workflowInstanceId)
        {
            RequestType requestType = RequestType.None;
            try
            {
                var pendingTasks =Task.GetPendingTaskByworkflowInstanceId(workflowInstanceId);

                if (pendingTasks.Count > 0)
                {
                    requestType = (RequestType)pendingTasks[0].RequestTypeId;
                }

                foreach (Task task in pendingTasks)
                {
                    task.IsActive = false;
                    task.Action = StatusType.AutoCompleted;
                    //task.LastModifiedBy = "System";
                    //task.LastModifiedDate = DateTime.Now;
                    task.Save(MasterConfigHelper.ServiceUser);
                    InterimStore.DeleteInterimStoreByTaskId(task.Id);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the AutoCompletePendingParallelTasks activity.";
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
        }
    }
}

using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Workflow.Processor
{
    public class CheckForPendingParallelTasks
    {
        public CheckForPendingParallelTasksResponse Execute(Task task, WorkflowConfiguration workflowConfiguration)
        {
            RequestType requestType = RequestType.None;
            bool isPendingTasksExists = true;
            bool canContinueApproval = true;

            try
            {
                var currentApprover = workflowConfiguration.WorkflowConfigurationDetails.Find(item => item.SequenceNumber == task.WorkflowStateId);

                requestType = (RequestType)task.RequestTypeId;

                var pendingTasks = Task.GetPendingTaskByworkflowInstanceId(task.WorkflowInstanceId);

                if (pendingTasks.Count > 0)
                {
                    if (requestType == RequestType.Attestation)
                    {
                        isPendingTasksExists = true;
                    }
                    else if (requestType == RequestType.Provisioning)
                    {
                        isPendingTasksExists = currentApprover.RequiresApprovalFromAll;
                    }
                }
                else
                {
                    isPendingTasksExists = false;
                }

                if (requestType == RequestType.Provisioning)
                {
                    if (task.AccessRequest.RequestStatus == AccessRequestStatus.Approved)
                    {
                        canContinueApproval = true;
                    }
                    else if (task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                    {
                        canContinueApproval = false;
                    }
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    if (task.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                    {
                        canContinueApproval = true;
                    }
                    else if (task.BulkRequest.RequestStatus == AccessRequestStatus.Rejected)
                    {
                        canContinueApproval = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the CheckForPendingParallelTasks activity.";
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

            CheckForPendingParallelTasksResponse response = new CheckForPendingParallelTasksResponse()
            {
                CanContinueApproval = canContinueApproval,
                IsPendingTasksExists = isPendingTasksExists
            };

            return response;
        }
    }
}

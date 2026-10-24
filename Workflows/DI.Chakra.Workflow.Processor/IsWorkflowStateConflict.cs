using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class IsWorkflowStateConflict
    {
        public IsWorkflowStateConflictResponse Execute(Task task, byte currentState)
        {
            RequestType requestType = RequestType.None;
            bool stateConflict = false;

            try
            {
                requestType = (RequestType)task.RequestTypeId;

                stateConflict = task.WorkflowStateId != currentState;

                if (stateConflict)
                {
                    var completedTask = Task.GetCompletedTaskByWorkflowInstanceIdAndState(task.WorkflowInstanceId, task.WorkflowStateId);
                    User user = null;

                    if (!task.IsGroupTask)
                    {
                        user = User.GetById(task.ActionUserId.Value);
                    }
                    else
                    {
                        user = User.GetById(task.AssignedTo.Value);
                    }

                    string message = string.Format("This task is already Approved/Rejected by {0} on {1}.", user.DisplayName, completedTask != null ? completedTask.LastModifiedDate.ToString() : string.Empty);

                    if (requestType == RequestType.Attestation)
                    {
                        DILog.AttestationLogger.WriteLog(null, LogLevelType.Warning, message);
                    }
                    else if (requestType == RequestType.Provisioning)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Warning, message);
                    }
                    else
                    {
                        DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the IsWorkflowStateConflict activity.";
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

            return new IsWorkflowStateConflictResponse()
            {
                StateConflict = stateConflict
            };
        }
    }
}

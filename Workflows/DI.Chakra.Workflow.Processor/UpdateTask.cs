using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Workflow.Processor.Common;
using System; 

namespace DI.Chakra.Workflow.Processor
{
    public class UpdateTask
    {
        public void Execute(Task task, WorkflowConfiguration workflowConfiguration)
        {
            RequestType requestType = RequestType.None;
            UtilityWFFunctions utility = new UtilityWFFunctions();

            try
            {
                var currentApprover = workflowConfiguration.WorkflowConfigurationDetails.Find(item => item.SequenceNumber == (task.WorkflowStateId));

                requestType = (RequestType)task.RequestTypeId;
                string approverType = utility.GetApprover(currentApprover);
                var currentUser = MasterConfigHelper.ServiceUser;

                if (requestType == RequestType.Attestation && task.AttestationCampaignId.HasValue)
                {
                    task.Action = StatusType.Approved;
                    task.ActionDate = DateTime.Now;

                    if (task.ActionUserId.HasValue)
                    {
                        if (task.ActionUser != null)
                            currentUser = task.ActionUser.UserName;
                        else
                        {
                            var actionUsr = User.GetById((long)task.ActionUserId);
                            if (actionUsr != null)
                                currentUser = actionUsr.UserName;
                        }
                    }

                    if (task.IsGroupTask && !task.IsDelegated && task.AssignedTo.HasValue && task.AssignedTo.Value > 0)
                    {
                        var actionUsr = User.GetById((long)task.AssignedTo);
                        if (actionUsr != null)
                            currentUser = actionUsr.UserName;
                    }

                    task.Save(currentUser);

                    task.SaveAttestationTask(currentUser, task.Id, task.AttestationCampaignId.Value, false, true);
                }
                else if (requestType == RequestType.Provisioning)
                {

                    task.AccessRequest.IsCurrentLevelApproved = true;
                    task.AccessRequest.ApproverType = approverType;
                    task.AccessRequest.Remarks = string.Empty;

                    if (task.AccessRequest.RequestStatus == AccessRequestStatus.Approved)
                    {
                        task.Action = StatusType.Approved;
                    }
                    else if (task.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                    {
                        task.Action = StatusType.Rejected;
                    }

                    task.ActionDate = DateTime.Now;
                    task.Comments = task.AccessRequest.Comments;

                    task.Save(currentUser);
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    task.BulkRequest.IsCurrentLevelApproved = true;
                    task.BulkRequest.ApproverType = approverType;
                    task.BulkRequest.Remarks = string.Empty;

                    if (task.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                    {
                        task.Action = StatusType.Approved;
                    }
                    else if (task.BulkRequest.RequestStatus == AccessRequestStatus.Rejected)
                    {
                        task.Action = StatusType.Rejected;
                    }
                    else
                    {
                        task.Comments = task.BulkRequest.Comments;
                    }

                    task.Save(currentUser);
                }
                else if (requestType == RequestType.EmployeeEvent)
                {
                    if (task.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Approved)
                    {
                        task.Action = StatusType.Approved;
                    }
                    else
                    {
                        task.Action = StatusType.Rejected;
                    }

                    task.Comments = task.EmployeeEventTrigger.Comments;

                    task.Save(currentUser);
                }

                //clear if any data available in interim store
                InterimStore.DeleteInterimStoreByTaskId(task.Id);
            }
            catch (Exception ex)
            {
                string message = "Error while executing the UpdateTask activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }
        }
    }
}

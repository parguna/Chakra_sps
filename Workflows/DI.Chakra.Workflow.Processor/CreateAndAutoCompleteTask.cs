
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class CreateAndAutoCompleteTask
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        public void Execute(DomainModel.Task task, Guid workflowInstanceId)
        {
            try
            {
                task.WorkflowInstanceId = workflowInstanceId;

                if (task.WorkflowStateId == (byte)WorkflowState.None)
                {
                    task.WorkflowStateId = (byte)WorkflowState.Level1;
                }

                task.IsActive = false;
                task.Save(MasterConfigHelper.ServiceUser);

                task = DomainModel.Task.GetProvisioningTask(task.Id);
                task.IsNotResponded = true;
                task.IsActive = false;


                task.Comments = "Does not require approval. Auto-created by System.";
                if (MasterConfigHelper.MockSkipApproval)
                {
                    task.Comments = "Auto approved because of MockSkipApproval.";
                    task.ActionDate = DateTime.Now;
                    //This(IsActive = true) for UAT should show MockSkip Approval Task
                    task.IsActive = true;
                }
                if (task.AttestationCampaignId.HasValue)
                {
                    task.Comments = "Does not require approval for Attestation Remediation request. Auto-created by System.";
                }
                //task.Approver = "System";
                task.Action = StatusType.Approved;
                utility.LogDebugStep($"task accessrequestId:{task.AccessRequestId}, bulkrequestid:{task.BulkRequestId}");
                var currentUser = MasterConfigHelper.ServiceUser;
                if (task.AccessRequestId.HasValue)
                {
                    task.AccessRequest.RequestStatus = AccessRequestStatus.Approved;
                    task.AccessRequest.IsCurrentLevelApproved = true;
                    task.Save(currentUser);
                }
                else if (task.BulkRequestId.HasValue)
                {
                    task.BulkRequest = DomainModel.BulkRequest.GetById(task.BulkRequestId.Value);
                    task.BulkRequest.RequestStatus = AccessRequestStatus.Approved;
                    task.BulkRequest.IsCurrentLevelApproved = true;
                    task.Save(currentUser);
                }
                else if (task.EmployeeEventTriggerId.HasValue && MasterConfigHelper.MockSkipApproval)
                {
                    //This(IsActive = true) for UAT should show MockSkip Approval Task
                    task.IsActive = true;
                    task.EmployeeEventTrigger = DomainModel.EmployeeEventTrigger.GetById(task.EmployeeEventTriggerId.Value);
                    task.EmployeeEventTrigger.RequestStatus = AccessRequestStatus.Approved;
                    task.EmployeeEventTrigger.Save(currentUser);
                    task.Save(currentUser);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the CreateAndAutoCompleteTask activity.";
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
            }
        }
    }
}

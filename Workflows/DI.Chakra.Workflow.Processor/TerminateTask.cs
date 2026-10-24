using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
namespace DI.Chakra.Workflow.Processor
{
    public class TerminateTask
    {
        public void Execute(Guid workflowInstanceId)
        {
            RequestType requestType = RequestType.None;
            try
            {
                var serviceUser = MasterConfigHelper.ServiceUser;
                var tasks = Task.GetPendingTaskByworkflowInstanceId(workflowInstanceId);

                if (tasks.Count > 0)
                {
                    requestType = (RequestType)tasks[0].RequestTypeId;
                }

                foreach (Task currentTask in tasks)
                {
                    if (requestType == RequestType.Attestation)
                    {
                        var task = Task.GetByIdAttestationTaskIncluded(currentTask.Id);
                        
                        if (task == null) continue;
                        task.IsNotResponded = true;
                        task.IsActive = false;
                        task.IsCompleted = true;
                        task.Comments = "Auto Approved";
                        //task.Approver = "System";
                        task.Action = StatusType.AutoCompleted;
                        task.ActionDate = DateTime.Now;
                        task.Save(serviceUser);
                        //if first level approval is not responded then do auto approve, else retain the response of last approval
                        if (task.WorkflowStateId == (byte)WorkflowState.Level1)
                        {
                            //var actions = AttestationSnapshotAction.GetByTask((long)task.AttestationCampaignId, task.Id);
                            //foreach (var action in actions)
                            //{
                            //    //foreach (ReviewEndpointSystem reviewEndpointSystem in taskReview.AttestationReview.ReviewEndpointSystems)
                            //    //{
                            //    //reviewEndpointSystem.AttestationReviewEndpointSystem.IsCurrentLevelApproved = true;
                            //    action.Comment += "Auto Approved";
                            //    action.Approve = true;
                            //    action.Revoke = false;
                            //    action.LastModifiedBy = "System";
                            //    //}
                            //}
                            task.SaveAttestationTask(MasterConfigHelper.ServiceUser, task.Id, task.AttestationCampaignId.HasValue ? task.AttestationCampaignId.Value : 0, true, true);
                        }
                        
                    }
                    else if (requestType == RequestType.Provisioning)
                    {
                        var task = Task.GetProvisioningTask(currentTask.Id);

                        task.IsNotResponded = true;
                        task.IsActive = false;
                        task.Comments = "Auto reject by system after response timeout.";
                        //task.Approver = "System";
                        task.Action = StatusType.Rejected;
                        task.ActionDate = DateTime.Now;
                        task.AccessRequest.Remarks = "Auto reject by system after response timeout.";
                        task.AccessRequest.RequestStatus = AccessRequestStatus.Rejected;
                        task.AccessRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                        task.AccessRequest.IsCurrentLevelApproved = true;
                        task.Save(MasterConfigHelper.ServiceUser);
                    }

                    else if (requestType == RequestType.BulkRequest)
                    {
                        var task = Task.GetBulkRequestTask(currentTask.Id);

                        task.IsNotResponded = true;
                        task.IsActive = false;
                        task.LastModifiedBy = "System";
                        task.LastModifiedDate = DateTime.Now;

                        task.Comments = "Auto reject by system after response timeout.";
                        task.Approver = "System";
                        task.Action = StatusType.Rejected;

                        task.BulkRequest.Remarks = "Auto reject by system after response timeout.";
                        task.BulkRequest.RequestStatus = AccessRequestStatus.Rejected;
                        task.BulkRequest.LastModifiedBy = "System";
                        task.BulkRequest.LastModifiedDate = DateTime.Now;

                        task.Save(MasterConfigHelper.ServiceUser);
                    }
                    else if (requestType == RequestType.EmployeeEvent)
                    {
                        var task = Task.GetEmployeeEventTask(currentTask.Id);

                        task.IsNotResponded = true;
                        task.IsActive = false;
                        task.LastModifiedBy = "System";
                        task.LastModifiedDate = DateTime.Now;

                        task.Comments = "Auto reject by system after response timeout.";
                        task.Approver = "System";
                        task.Action = StatusType.Rejected;

                        task.EmployeeEventTrigger.RequestStatus = AccessRequestStatus.Rejected;
                        task.EmployeeEventTrigger.LastModifiedBy = "System";
                        task.EmployeeEventTrigger.LastModifiedDate = DateTime.Now;

                        task.Save(MasterConfigHelper.ServiceUser);
                    }

                    //clear if any data available in interim store
                    InterimStore.DeleteInterimStoreByTaskId(currentTask.Id);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the TerminateTask activity.";
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
        }
    }
}

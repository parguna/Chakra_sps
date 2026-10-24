using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System; 

namespace DI.Chakra.Workflow.Processor
{
    public class ResetReviewCompleteFlagForNextApprovalLevel
    {
        public void Execute(Guid workflowInstanceId)
        {
            RequestType requestType = RequestType.None;
            try
            {
                var tasks = Task.GetByWorkflowInstanceId(workflowInstanceId);

                if (tasks.Count > 0)
                {
                    requestType = (RequestType)tasks[0].RequestTypeId;
                }

                if (requestType == RequestType.Attestation)
                {
                    foreach (Task currentTask in tasks)
                    {
                        //var task = Task.GetAttestationTask(currentTask.Id, true);

                        //foreach (TaskReview taskReview in task.TaskReviews)
                        //{
                        //    foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                        //    {
                        //        reviewEP.AttestationReviewEndpointSystem.IsCurrentLevelApproved = false;
                        //        (reviewEP.AttestationReviewEndpointSystem).Save(MasterConfigHelper.ServiceUser);
                        //    }
                        //}
                    }
                }
                else if (requestType == RequestType.Provisioning)
                {
                    foreach (Task currentTask in tasks)
                    {
                        var task = Task.GetProvisioningTask(currentTask.Id);

                        (task.AccessRequest).Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the ResetReviewCompleteFlagForNextApprovalLevel activity.";
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

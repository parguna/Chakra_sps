using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class IsNextLevelApprovalRequired
    {
        public IsNextLevelApprovalRequiredResponse Execute(Task task)
        {
            bool isRequired = true;

            try
            {
                var requestType = (RequestType)task.RequestTypeId;

                ApplicationType appType = ApplicationType.None;
                AdHocActionType actionType = AdHocActionType.None;

                IEntitlementDetail detail = null;
                long endPointSystemId = 0;
                if (requestType == RequestType.Provisioning)
                {
                    
                    task = Task.GetProvisioningTask(task.Id);
                    appType = task.AccessRequest.ApplicationType;
                    actionType = task.AccessRequest.AdHocAccessRequest.AdhocActionType;
                    if(task.AccessRequest.EndpointSystemId.HasValue && task.AccessRequest.EndpointSystemId.Value > 0)
                    {
                        endPointSystemId = task.AccessRequest.EndpointSystemId.Value;
                    }
                    if (endPointSystemId == 0 && task.AccessRequest.AdHocAccessRequest.EndpointSystemId.HasValue)
                        endPointSystemId = task.AccessRequest.AdHocAccessRequest.EndpointSystemId.Value;

                    detail = task.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    task = Task.GetBulkRequestTask(task.Id);
                    appType = task.BulkRequest.ApplicationType;
                    actionType = task.BulkRequest.AdhocActionType;
                    endPointSystemId = task.BulkRequest.EndpointSystemId;
                    detail = task.BulkRequest.GetEntitlementDetail();
                }
                else if(requestType == RequestType.Attestation)
                {
                    task = Task.GetAttestationTask(task.Id);
                    if(task != null && (task.Action == StatusType.Withdrawn || task.Action == StatusType.Rejected))
                    {
                        isRequired = false;
                    }
                }
                //if (appType == ApplicationType.NPAC)
                //{
                //    var npacDetail = (AdHocNPACRequest)detail;

                //    if(npacDetail.Entitlement != "1" && npacDetail.Entitlement != MasterConfigHelper.NPACLevel99)
                //    {
                //        isRequired = false;
                //    }
                //}

                //if (appType == ApplicationType.InhouseSystems)
                //{
                //    var inhDetail = (AdHocINHRequest)detail;
                //    var endpoint = EndPoint.GetById(endPointSystemId);
                //    if (endpoint != null && endpoint.CanAutomate)
                //    {
                //        isRequired = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                string message = "Error while executing the IsWorkflowStateConflict activity.";
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
            }

            return new IsNextLevelApprovalRequiredResponse()
            {
                IsRequired = isRequired
            };
        }
    }
}

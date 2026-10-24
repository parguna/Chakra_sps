//using DI.Chakra.Model;
using DI.Chakra.Workflow.ServiceHelpers.AttestationApprovalWFServiceReference;
using System.Collections.Generic;
using DI.Chakra.DomainModel;
using System;

namespace DI.Chakra.Workflow.ServiceHelpers.Attestation
{
    class AttestationApprovalWFServiceHelper : IAttestattionServiceHelper
    {

        public void InitiateAttestationRequest(List<DomainModel.Task> taskList, long campaignId)
        {
            WFServiceClient client = new WFServiceClient();
            client.InitiateAttestationRequest(taskList, campaignId);
        }

        public bool ApprovalRequest(long taskId, string workflowInstanceId)
        {
            WFServiceClient client = new WFServiceClient();
            client.ApprovalRequestAsync(taskId, workflowInstanceId);
            return true;
        }


        public void WithdrawRequest(string workflowInstanceId)
        {
            WFServiceClient client = new WFServiceClient();
            client.WithdrawRequestAsync(workflowInstanceId);
        }
    }
}

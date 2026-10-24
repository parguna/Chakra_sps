//using DI.Chakra.Model;
using DI.Chakra.Workflow.ServiceHelpers.ProvisioningApprovalWFServiceReference;
//using System.Collections.Generic;
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.ServiceHelpers.Provisioning
{
    class ProvisioningApprovalWFServiceHelper : IProvisioningServiceHelper
    {

        public void InitiateProvisioningRequest(List<DomainModel.Task> taskList)
        {
            if (taskList != null && taskList.Count > 0)
            {
                WFServiceClient client = new WFServiceClient();
                client.InitiateProvisioningRequest(taskList);
            }
        }

        public bool ApprovalRequest(long taskId, string workflowInstanceId)
        {
            WFServiceClient client = new WFServiceClient();
            return client.ApprovalRequest(taskId, workflowInstanceId);
        }

        public void WithdrawRequest(string workflowInstanceId)
        {
            WFServiceClient client = new WFServiceClient();
            client.WithdrawRequest(workflowInstanceId);
        }

    }
}

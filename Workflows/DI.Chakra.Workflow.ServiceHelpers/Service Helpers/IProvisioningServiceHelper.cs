//using DI.Chakra.Model;
using System.Collections.Generic;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Workflow.ServiceHelpers
{
    public interface IProvisioningServiceHelper
    {
        void InitiateProvisioningRequest(List<DI.Chakra.DomainModel.Task> taskList);

        bool ApprovalRequest(long taskId, string workflowInstanceId);

        void WithdrawRequest(string workflowInstanceId);
    }
}

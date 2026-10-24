//using DI.Chakra.Model;
using System.Collections.Generic;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Workflow.ServiceHelpers
{
    public interface IAttestattionServiceHelper
    {
        void InitiateAttestationRequest(List<DI.Chakra.DomainModel.Task> taskList, long campaignId);

        bool ApprovalRequest(long taskId, string workflowInstanceId);

        void WithdrawRequest(string workflowInstanceId);

    }
}

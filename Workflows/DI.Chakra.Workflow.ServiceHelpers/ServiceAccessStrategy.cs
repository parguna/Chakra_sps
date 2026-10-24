using DI.Chakra.Infrastructure.Enums;
//using DI.Chakra.Model.CustomEntities;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.Workflow.ServiceHelpers
{
    public class ServiceAccessStrategy
    {

        PresentationWorkflowCriteriaBase criteria = null;

        public ServiceAccessStrategy(PresentationWorkflowCriteriaBase _criteria)
        {
            criteria = _criteria;
        }


        public bool AccessWorkflowService()
        {
            bool status = true;

            if (criteria.RequestType == RequestType.Attestation)
            {
                status = this.AccessAttestationService();
            }
            else if (criteria.RequestType == RequestType.Provisioning)
            {
                status = this.AccessProvisioningService();
            }
            else if (criteria.RequestType == RequestType.BulkRequest)
            {
                status = this.AccessProvisioningService();
            }
            else if (criteria.RequestType == RequestType.EmployeeEvent)
            {
                status = this.AccessProvisioningService();
            }

            return status;
        }

        //Attestation

        private bool AccessAttestationService()
        {
            bool status = true;
            var helper = ServiceHelperFactory.GetAttestationServiceHelper();

            if (criteria.GetType() == typeof(SubmitActionCriteria))
            {
                var actualCriteria = (SubmitActionCriteria)criteria;
                helper.InitiateAttestationRequest(actualCriteria.TaskList, actualCriteria.CampaignId);
            }
            else if (criteria.GetType() == typeof(ApprovalActionCriteria))
            {
                var actualCriteria = (ApprovalActionCriteria)criteria;
                status = helper.ApprovalRequest(actualCriteria.TaskId, actualCriteria.WorkflowInstanceId.ToString());
            }
            else if (criteria.GetType() == typeof(WithdrawActionCriteria))
            {
                var actualCriteria = (WithdrawActionCriteria)criteria;
                helper.WithdrawRequest(actualCriteria.WorkflowInstanceId.ToString());
            }

            return status;
        }

        //Provisioning

        private bool AccessProvisioningService()
        {

            bool status = true;
            var helper = ServiceHelperFactory.GetProvisioningServiceHelper();

            if (criteria.GetType() == typeof(SubmitActionCriteria))
            {
                var actualCriteria = (SubmitActionCriteria)criteria;
                helper.InitiateProvisioningRequest(actualCriteria.TaskList);
            }
            else if (criteria.GetType() == typeof(ApprovalActionCriteria))
            {
                var actualCriteria = (ApprovalActionCriteria)criteria;
                status = helper.ApprovalRequest(actualCriteria.TaskId, actualCriteria.WorkflowInstanceId.ToString());
            }
            else if (criteria.GetType() == typeof(WithdrawActionCriteria))
            {
                var actualCriteria = (WithdrawActionCriteria)criteria;
                helper.WithdrawRequest(actualCriteria.WorkflowInstanceId.ToString());
            }

            return status;
        }

    }
}

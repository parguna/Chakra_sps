 
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Workflow.Processor
{
    public class UpdateAttestationCampaignInstance
    {
        public void Execute(long campaignId, Guid workflowInstanceId)
        {
            try
            {
                AttestationCampaign campaign = AttestationCampaign.GetById(campaignId);

                if (campaign != null)
                {
                    //campaign.Status = Infrastructure.Enums.AttestationCampaignStatus.WorkInProgress;
                    //campaign.WorkflowInstanceID = workflowInstanceId;
                    AttestationCampaign.UpdateCampaignWorkflowInstanceWithStatus(campaignId, workflowInstanceId, Infrastructure.Enums.AttestationCampaignStatus.WorkInProgress);
                        //ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [Status] = {(int)},WorkflowInstanceID='{workflowInstanceId}',LastModifiedBy='System'  WHERE ID ={campaignId}");
                    //campaign.Save(MasterConfigHelper.ServiceUser);
                }
                else
                {
                    throw new Exception("Attestation campaign instance is null");
                }
            }
            catch (Exception ex)
            {
                string message = "Error while updating AttestationCampaignInstance.";

                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
            }
        }

    }
}
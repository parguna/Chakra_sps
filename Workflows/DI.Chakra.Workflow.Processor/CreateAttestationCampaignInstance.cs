 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Workflow.Processor
{
    public class CreateAttestationCampaignInstance
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();

        public long Execute(Task task, Guid workflowInstanceId)
        {
            try
            {
                DateTime time = DateTime.Now;

                string suffix = String.Format("{0:0000}-{1:00}-{2:00}-{3:00}-{4:00}-{5:00}", 
                    time.Year,time.Month, time.Day, time.Hour, time.Minute, time.Second);

                Attestation att = Attestation.GetById(Convert.ToInt64(task.AttestationId));

                if (att != null)
                {
                    AttestationCampaign campaign = new AttestationCampaign
                    {
                        CampaignName = String.Format("{0}-{1}", att.AttestationName, suffix),
                        Description = att.AttestationDescription,
                        Comments = String.Empty,
                        AttestationID = att.Id,
                        DefaultReviewStatus = att.DefaultReviewStatus,
                        WorkflowInstanceID = workflowInstanceId,
                        RequiresAttestationRemediation = att.RequiresAttestationRemediation
                    };

                    campaign.Save(MasterConfigHelper.ServiceUser);

                    return campaign.Id;
                }
                else
                {
                    throw new Exception("Attestation instance is null");
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the CreateAttestationCampaignInstance activity.";
                
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
            }
            return 0;
        }
        
    }
}
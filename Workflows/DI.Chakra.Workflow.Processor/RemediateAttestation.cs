using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Linq; 

namespace DI.Chakra.Workflow.Processor
{
    public class RemediateAttestation
    {
        public void Execute(Task task, long campaignId)
        {
            try
            {

                //var attestation = Attestation.GetById(task.AttestationId.Value);
                if (task == null)
                {
                    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Attestation Campaign task empty");
                    return;
                }

                var tTask = Task.GetById(task.Id);
                if (tTask == null)
                {
                    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Attestation Campaign task empty {task?.Id} campaignId:{campaignId}");
                    return;
                }
                AttestationCampaign campaign = AttestationCampaign.GetById(campaignId);
                if (campaign == null)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Error, $"Error while executing the RemediateAttestation activity - Cannot get campaign! Details: TaskID: {task?.Id}, CampaignID: {campaignId}");
                    return;
                }
                if (tTask.Action == StatusType.Rejected || tTask.Action == StatusType.Withdrawn)
                {
                    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Info, $"Attestation Campaign task  {task?.Id} withdrawn for Campaign Id:{campaignId}");
                    //campaign.Status = AttestationCampaignStatus.Completed;
                    //campaign.EndDate = DateTime.Now;
                    //campaign.Save(MasterConfigHelper.ServiceUser);
                    AttestationCampaign.UpdateCampaignEndDateWithStatus(campaign.Id, AttestationCampaignStatus.Completed);
                    
                    if (campaign.ParentCampaign.HasValue)
                    {
                        UpdateParentCampaign(campaign);
                    }
                    
                    return;
                }
                //if (campaign != null)
                {
                    if (campaign.RequiresAttestationRemediation)
                    {
                        bool remediationNeeded = false;
                        //var tasks = Task.GetByWorkflowInstanceIdAndState(task.WorkflowInstanceId, task.WorkflowStateId);

                        //foreach (Task tsk in tasks)
                        {
                            var actions = AttestationSnapshotAction.GetByCampaignIdForRevocation(campaignId);

                            foreach (var action in actions)
                            {
                                remediationNeeded = true;
                                // set remediation status as Pending
                                action.RemediationStatus = AttestationRemediationStatus.Pending;
                                action.Save(MasterConfigHelper.ServiceUser);
                            }
                            // TODO: Remediation
                            //var deepLoadedTask = Task.GetAttestationTask(tsk.Id, true);
                            //foreach (TaskReview taskReview in deepLoadedTask.TaskReviews)
                            //{
                            //    taskReview.AttestationReview.RemediationStatus = AttestationRemediationStatus.Pending;
                            //    (taskReview.AttestationReview).Save(MasterConfigHelper.ServiceUser);
                            //}
                            actions?.Clear();
                            actions = null;
                        }

                        if (remediationNeeded)
                        {
                            campaign.Status = AttestationCampaignStatus.Remediation;
                        }
                        else
                        {
                            campaign.Status = AttestationCampaignStatus.Completed;
                        }

                        //campaign.EndDate = DateTime.Now;
                        //campaign.Save(MasterConfigHelper.ServiceUser);
                        AttestationCampaign.UpdateCampaignEndDateWithStatus(campaign.Id, campaign.Status);
                        if (campaign.ParentCampaign.HasValue)
                        {
                            UpdateParentCampaign(campaign);
                        }
                        else
                        {
                            SendEmailForCampaignCompletion(campaign);
                        }
                    }
                    else
                    {
                        //campaign.Status = AttestationCampaignStatus.Completed;
                        //campaign.EndDate = DateTime.Now;
                        //campaign.Save(MasterConfigHelper.ServiceUser);
                        AttestationCampaign.UpdateCampaignEndDateWithStatus(campaign.Id, AttestationCampaignStatus.Completed);
                        if (campaign.ParentCampaign.HasValue)
                        {
                            UpdateParentCampaign(campaign);
                        }
                        else
                        {
                            SendEmailForCampaignCompletion(campaign);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, "Error while executing the RemediateAttestation activity", ex);
            }
        }

        private void SendEmailForCampaignCompletion(AttestationCampaign campaign)
        {
            var sendEmail = new SendNotificationEmail();
            sendEmail.SendEmailForAttestationCompletion(campaign);
            sendEmail = null;
        }

        private void UpdateParentCampaign(AttestationCampaign campaign)
        {
            try
            {
                var pendingAttestationCampaign = AttestationCampaign.GetChildCampaigns(campaign.ParentCampaign.Value).Where(a => a.Status != AttestationCampaignStatus.Completed).ToList();
                pendingAttestationCampaign = pendingAttestationCampaign.Where(a => a.Status != AttestationCampaignStatus.Remediation && a.Status != AttestationCampaignStatus.IgnoreNoUserFound).ToList();
                if (pendingAttestationCampaign == null || pendingAttestationCampaign.Count() == 0)
                {
                    var parentCampaign = AttestationCampaign.GetById(campaign.ParentCampaign.Value);
                    if (parentCampaign != null)
                    {
                        //parentCampaign.Status = AttestationCampaignStatus.Completed;
                        //parentCampaign.LastModifiedBy = "System";
                        //parentCampaign.LastModifiedDate = DateTime.Now;
                        //parentCampaign.Save(MasterConfigHelper.ServiceUser);
                        AttestationCampaign.UpdateCampaignEndDateWithStatus(parentCampaign.Id, AttestationCampaignStatus.Completed);
                        SendEmailForCampaignCompletion(parentCampaign);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, "Error while UpdateParentCampaign in the RemediateAttestation activity", ex);
            }
        }

    }
}

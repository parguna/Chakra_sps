using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Linq; 
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{
    public class UpdateAttestationDateForDelta
    {
        public void Execute(Task task, long campaignId)
        {
            try
            {
                var tTask = Task.GetById(task.Id);

                if (tTask == null)
                {
                    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Attestation Campaign task empty {task?.Id} campaignId:{campaignId}");
                    return;
                }

                if (tTask.Action == StatusType.Rejected || tTask.Action == StatusType.Withdrawn)
                {
                    //DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Info, $"Attestation Campaign task  {task?.Id} withdrawn for Campaign Id:{campaignId}");
                    return;
                }

                var campaign = AttestationCampaign.GetById(campaignId);
                if (campaign == null)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Error, $"Error while executing the UpdateAttestationDateForDelta activity - Cannot get campaign! Details: TaskID: {task?.Id}, CampaignID: {campaignId}");
                    return;
                }

                if (campaign != null && task != null)
                {
                    DateTime campaignStartDate = campaign.CreatedDate;
                    //AccountReconcilation user = null;
                    //EntitlementReconcilation entitlement = null;
                    var tasks = Task.GetByWorkflowInstanceIdAndState(task.WorkflowInstanceId, task.WorkflowStateId);
                    var attestation = Attestation.GetById(campaign.AttestationID);
                    AccountReconcilation.ExecuteQueryWithCommandTimeout($@"UPDATE DI_M_AccountReconcilation SET LastAttestedOn=(SELECT CreatedDate FROM DI_M_AttestationCampaign WHERE ID = {campaignId}),LastModifiedDate = GETDATE(),LastModifiedBy = 'System' WHERE ID IN(SELECT Distinct S.KeyId from DI_M_AttestationSnapshotAction SA inner Join DI_M_AttestationSnapshot S on SA.AttestationSnapshotId = S.Id
                                             WHERE S.AttestationCampaignId = {campaignId} AND(S.Level = 1) AND S.KeyId > 0)",180);
                    #region Delta update
                    /*
        foreach (Task tsk in tasks)
        {
            try
            {
                var actions = AttestationSnapshotAction.GetByCampaign(campaignId, 0);

                if (attestation != null && attestation.AttestationType == AttestationType.User && actions != null && actions.Count > 0)
                {


                    // loop task reviews
                    foreach (var snpShot in actions)
                    {
                        try
                        {
                            if (snpShot == null || snpShot.AttestationSnapshot == null) continue;
                            switch (snpShot.AttestationSnapshot.Level)
                            {
                                case 1:
                                    //Account Level:
                                    AccountReconcilation.ExecuteQuery($"UPDATE DI_M_AccountReconcilation SET LastAttestedOn=(SELECT CreatedDate FROM DI_M_AttestationCampaign WHERE ID={campaignId}),LastModifiedDate=GETDATE(),LastModifiedBy='System' WHERE ID={snpShot.AttestationSnapshot.KeyId}");
                                    break;
                                case 2:
                                    #region Level2
                                    switch (snpShot.AttestationSnapshot.EndpointType)
                                    {
                                        case EndpointType.SRM:
                                            #region SRM
                                            if (!string.IsNullOrWhiteSpace(snpShot.AttestationSnapshot.Description))
                                            {
                                                EntitlementReconcilation.ExecuteQuery($"UPDATE DI_M_EntitlementReconcilation SET LastAttestedOn=(SELECT CreatedDate FROM DI_M_AttestationCampaign WHERE ID={campaignId}),LastModifiedDate=GETDATE(),LastModifiedBy='System' WHERE Entitlement='{snpShot.AttestationSnapshot.Description.Replace("'", "''")}' AND ACCOUNTNAME='{snpShot.AttestationSnapshot.AccountName.Replace("'", "''")}'");
                                            }
                                            break;
                                        #endregion
                                        case EndpointType.GEMS:
                                            #region Gems
                                            try
                                            {
                                                EntitlementReconcilation.ExecuteQuery($@"UPDATE E 
                                    SET E.LastAttestedOn =(SELECT CreatedDate FROM DI_M_AttestationCampaign WHERE ID={campaignId}),LastModifiedDate=GETDATE(),LastModifiedBy='System' FROM DI_M_EntitlementReconcilation  E INNER JOIN 
                                    (SELECT R.Name as TechnicalRole,M.BusinessRoleName from DI_GEMS_RoleMatrix M inner join DI_GEMS_RoleInfo R on M.TechnicalRole = R.Display_Name )AS M ON E.Entitlement = M.TechnicalRole WHERE E.EndpointSystemId={snpShot.AttestationSnapshot.EndpointId} AND E.UserId = {snpShot.AttestationSnapshot.UserId} AND E.IsDeleted=0 AND M.BusinessRoleName='{snpShot.AttestationSnapshot.Name.Replace("'", "''")}'");
                                            }
                                            catch (Exception ex)
                                            {
                                                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Error while updating LastAttestedOn for user:{snpShot.AttestationSnapshot.DIUserName} entitlement:{snpShot.AttestationSnapshot.Name}", ex);
                                            }
                                            #endregion
                                            break;
                                        case EndpointType.EAM_NOVA:
                                            break;
                                        default:
                                            {
                                                EntitlementReconcilation.ExecuteQuery($"UPDATE DI_M_EntitlementReconcilation SET LastAttestedOn=(SELECT CreatedDate FROM DI_M_AttestationCampaign WHERE ID={campaignId}),LastModifiedDate=GETDATE(),LastModifiedBy='System' WHERE ID={snpShot.AttestationSnapshot.KeyId}");

                                            }
                                            break;
                                    }
                                    break;
                                #endregion
                                //case 3:
                                default:
                                    break;
                            }

                            //if (snpShot.AttestationSnapshot.Level == 1)
                            //{
                            //    //user = AccountReconcilation.GetById(snpShot.AttestationSnapshot.KeyId);
                            //    //if (user != null)
                            //    //{
                            //    //    user.LastAttestedOn = campaignStartDate;
                            //    //    user.Save(MasterConfigHelper.ServiceUser);
                            //    //}
                            //}
                            //else if (snpShot.AttestationSnapshot.Level == 2)
                            //{
                                //entitlement = EntitlementReconcilation.GetById(snpShot.AttestationSnapshot.KeyId);
                                //if (entitlement != null)
                                //{
                                //    entitlement.LastAttestedOn = campaignStartDate;
                                //    entitlement.Save(MasterConfigHelper.ServiceUser);
                                //}
                            //}
                            //else
                            //{
                            //    //to fill as per the need.
                            //}
                        }
                        catch (Exception ex)
                        {
                            DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Error while executing the UpdateAttestationDateForDelta activity. snpShotId={snpShot.Id}", ex);
                        }
                    }
                }
                        }
                        catch (Exception ex)
                        {
                            DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Error while executing the UpdateAttestationDateForDelta activity. Taskid={tsk.Id}", ex);
                        }
                    }
                }*/
                    #endregion
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while executing the UpdateAttestationDateForDelta activity", ex);
            }
        }


    }
}
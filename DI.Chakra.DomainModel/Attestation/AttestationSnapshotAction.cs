using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AttestationSnapshotAction : DomainModelBase<AttestationSnapshotAction, EntityModel.AttestationSnapshotAction>
    {
        public long AttestationSnapshotId { get; set; }
        public long TaskId { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public Nullable<long> ReviewerId { get; set; }
        public bool IsReviewerGroup { get; set; }
        public bool Expired { get; set; }
        public bool IsChanged { get; set; }
        public bool IsChangedInPreviousLevel { get; set; }
        public bool IsDelta { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActionable { get; set; }
        public bool IsActionExtendable { get; set; }
        public bool IsException { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public Nullable<long> RootSnapshotId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }        
        public AttestationSnapshot AttestationSnapshot { get; set; }
        public AttestationRemediationStatus RemediationStatus { get; set; }
        public string ParentObjects { get; set; }
        /// <summary>
        /// Get the list of Snapshot Actions for a specific user in a given campaign
        /// </summary>
        /// <param name="diUserId">DI User id of specific user</param>
        /// <param name="campaignId">Attestation Campaign ID</param>
        /// <returns>List of all Attestation Snapshots for specific user 
        /// i.e. Accounts the user is mapped to in Account Reconciliation table.</returns>
        public static IList<AttestationSnapshotAction> GetByDIUserId(long diUserId, long campaignId, long level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId &&
                    attAct.AttestationSnapshot.UserId == diUserId
                    && attAct.AttestationSnapshot.Level == level, true).ToList());
            }
        }

        public static IList<AttestationSnapshotAction> GetByRoleId(long entitlementId, EntitlementType entitlementType, long campaignId, long level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId &&
                    attAct.AttestationSnapshot.KeyId == entitlementId
                    && attAct.AttestationSnapshot.EntitlementType == entitlementType
                    && attAct.AttestationSnapshot.Level == level, true).ToList());
            }

        }

        /// <summary>
        /// Get the list of all Snapshot Actions for a specific campaign
        /// </summary>
        /// <param name="campaignId">Attestation Campaign ID</param>
        /// <returns>List of all Attestation Snapshots for specific campaign </returns>
        public static IList<AttestationSnapshotAction> GetByCampaign(long campaignId, int level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    // either level is less than 0, no level check done, all levels retrieved
                    // or level is greater than 0, specified level retrieved
                    && (level < 1 || attAct.AttestationSnapshot.Level == level), true).ToList());
            }

        }
        public static bool IsSnapshotActionExistsForCampaign(long campaignId, int level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotActionRepository(tran).IsSnapshotActionExistsForCampaign(campaignId, level);
            }
        }
        public static IList<AttestationSnapshotAction> GetUnmappedAccountsByCampaign(long campaignId, int level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    && attAct.AttestationSnapshot.UserId == 0
                    // either level is less than 0, no level check done, all levels retrieved
                    // or level is greater than 0, specified level retrieved
                    && (level < 1 || attAct.AttestationSnapshot.Level == level), true).ToList());
            }

        }

        /// <summary>
        /// Get the list of all Snapshot Actions for a specific task
        /// </summary>
        /// <param name="taskId">Attestation task ID</param>
        /// <returns>List of all Attestation Snapshots for specific task </returns>
        public static List<AttestationSnapshotAction> GetByTask(long campaignId, long taskId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var userLevelAttestationSnapshot = PresentationAttestationSnapshotActionsForTask.ConvertToSnapshotActionDomainModel(new AttestationSnapshotActionRepository(tran).GetAttestationSnapshotActionDetails(taskId, campaignId)).ToList();

                return userLevelAttestationSnapshot;
            }
        }
        public static IList<AttestationSnapshotAction> GetByCampaignIdForRevocation(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(dataAccessTransaction)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    // Taking User level only
                    && attAct.Revoke).ToList());
            }
        }

        public static List<AttestationSnapshotAction> GetByTask(long campaignId, long taskId, long level)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var userLevelAttestationSnapshot = ORMapList(new AttestationSnapshotActionRepository(tran).GetAttestationSnapshotActionDetails(taskId, campaignId, level)).ToList();

                return userLevelAttestationSnapshot;
            }
        }

        /// <summary>
        /// Get the list of all reviwers set for a specific campaign
        /// </summary>
        /// <param name="campaignId">Attestation Campaign ID</param>
        /// <returns>List of all reviewers for specific campaign </returns>
        public static IList<AttestationSnapshotAction> GetReviewerByCampaign(long campaignId, long level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var allreviewerActions = ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    // Taking User level only
                    && attAct.AttestationSnapshot.Level == level
                    && attAct.ReviewerId.HasValue,
                    attAct => attAct.AttestationSnapshot).ToList());

                return allreviewerActions;
            }
        }

        /// <summary>
        /// Returns all Snapshot Actions for a specific Attestation campaign for the given endpoint
        /// </summary>
        /// <param name="endpointSystemID">ID of Endpoint System</param>
        /// <param name="campaignId">Attestation Campaign</param>
        /// <returns></returns>
        public static IList<AttestationSnapshotAction> GetByEndpoint(long endpointSystemID, long campaignId, long level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId &&
                    attAct.AttestationSnapshot.EndpointId == endpointSystemID
                    // Taking User level only
                    && attAct.AttestationSnapshot.Level == level, true).ToList());
            }

        }

        public static IList<AttestationSnapshotAction> GetByEndpointAndTask(long endpointSystemID, long campaignId, long taskId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId &&
                    attAct.AttestationSnapshot.EndpointId == endpointSystemID &&
                    attAct.TaskId == taskId, at => at.AttestationSnapshot).ToList());
            }

        }

        /// <summary>
        /// Returns all Snapshot Actions for a specific Attestation campaign for the given endpoint
        /// </summary>
        /// <param name="endpointSystemID">ID of Endpoint System</param>
        /// <param name="campaignId">Attestation Campaign</param>
        /// <returns></returns>
        public static IList<AttestationSnapshotAction> GetByCampaignId(long campaignId, long level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    // Taking User level only
                    && attAct.AttestationSnapshot.Level == level, true).ToList());
            }

        }

        /// <summary>
        /// Returns all Snapshot Actions that are pending remediation
        /// </summary>
        /// <returns></returns>
        public static IList<AttestationSnapshotAction> GetPendingRemediations()
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.RemediationStatus == AttestationRemediationStatus.Pending, true).ToList());
            }
        }

        /// <summary>
        /// Returns all Snapshot Actions assigned to a specific reviewer for a specific Attestation campaign
        /// </summary>
        /// <param name="reviewerId">ID of Reviewer</param>
        /// <param name="campaignId">Attestation Campaign</param>
        /// <returns></returns>
        public static IList<AttestationSnapshotAction> GetByReviewerId(long reviewerId, bool isGroupTask, long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationSnapshotActionRepository(tran).GetByReviewerId(reviewerId, isGroupTask, campaignId));
            }
        }

        public static void UpdateReviewerDetail(long attestationId, long attestationCampaignId, long taskId, long assignedTo, string currentUser)
        {
                //update action.
                string updateAction = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET ReviewerId = {0} WHERE TaskId = {1}",
                    assignedTo, taskId);
                AttestationSnapshotAction.ExecuteQuery(updateAction);

                //update the interim as well.
                string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET ReviewerId = {0} WHERE TaskId = {1}",
                    assignedTo, taskId);               
                AttestationSnapshotAction.ExecuteQuery(updateInterim);
        }

        public static bool CheckAllPositionSelected(long snapShotActionId, string accountName,long endpointSystemId, long campaignId)
        {
            var snapShotAction = GetById(snapShotActionId,true);
            if(snapShotAction != null)
            {
                EndPoint dependentEP = null;
                Infrastructure.Enums.ApplicationType applicationType = Infrastructure.Enums.ApplicationType.None;
                var ep = EndPoint.GetById(endpointSystemId);
                if (ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_CRM || ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_ISU)
                {
                    applicationType = ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_CRM ? Infrastructure.Enums.ApplicationType.MSSL_ISU : Infrastructure.Enums.ApplicationType.MSSL_CRM;
                }
                else if (ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_CRM || ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_ISU)
                {
                    applicationType = ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_CRM ? Infrastructure.Enums.ApplicationType.EBS_ISU : Infrastructure.Enums.ApplicationType.EBS_CRM;
                }
                dependentEP = EndPoint.GetByApplicationType(applicationType);
                var dependentPosition = snapShotAction.AttestationSnapshot.Reserved8;

                using (var tran = new DataAccessTransaction(false))
                {

                    var snapShot = new AttestationSnapshotRepository(tran).GetAttestationSnapshot(accountName, dependentEP.Id, campaignId);
                    if (!string.IsNullOrWhiteSpace(dependentPosition) && snapShot != null)
                    {
                        //using (var tran = new DataAccessTransaction(false))
                        {
                            var dpSnapShotActions = new AttestationSnapshotActionRepository(tran).GetAttestationSnapshotActionDetails(snapShot.Id, accountName, snapShot.EndpointId, campaignId).ToList();
                            if (dpSnapShotActions.Count > 1)
                            {
                                dpSnapShotActions.ForEach(dpSnapShotAction =>
                                {
                                    if (dpSnapShotAction.Reserved3 == dependentPosition)
                                    {
                                        dpSnapShotAction.Revoke = true;
                                    }
                                });
                                if (dpSnapShotActions.All(i => i.Revoke))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                using (var tran = new DataAccessTransaction(false))
                {
                    var attestationSnapShotActions = new AttestationSnapshotActionRepository(tran).GetAttestationSnapshotActionDetails(snapShotAction.RootSnapshotId.Value, accountName, endpointSystemId, campaignId).ToList();
                    if (attestationSnapShotActions.Count > 1)
                    {
                        attestationSnapShotActions.ForEach(i =>
                        {
                            if (i.Id == snapShotActionId)
                            {
                                i.Revoke = true;
                            }
                        });
                        return attestationSnapShotActions.All(i => i.Revoke);
                    }
                    return false;
                }
            }
            return false;
        }
        public static bool IsUnMappedAccountWithoutReviewer(long campaignId, int level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.AttestationSnapshotAction>(tran)
                    .Get(attAct => attAct.AttestationSnapshot.AttestationCampaignId == campaignId
                    && ( attAct.AttestationSnapshot.Level == 1) && attAct.ReviewerId.HasValue == false && attAct.AttestationSnapshot.UserId == 0, true).Any();
            }

        }
    }
}

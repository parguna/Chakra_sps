using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestationSnapshotActionsForTask
    {
        public List<AttestationSnapshotAction> AttestationSnapshotActions { get; set; }
        public long? TaskID { get; set; }
        public bool IsUnSavedChangesAvailable { get; set; }

        public static AttestationSnapshotAction ORMap(EntityModel.AttestationSnapshotAction attestationSnapshotAction)
        {
            if (attestationSnapshotAction == null)
                return null;

            return Mapper.Map<EntityModel.AttestationSnapshotAction, AttestationSnapshotAction>(attestationSnapshotAction);
        }

        public static List<AttestationSnapshotAction> ORMapList(IList<EntityModel.AttestationSnapshotAction> attestationSnapshotAction)
        {
            return attestationSnapshotAction.Select(ORMap).ToList();
        }

        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskIdByActionId(long taskId, long campaignId, long actionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionFullDetails(taskId, campaignId, actionId)).ToList();
                //var snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskId,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }

        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskId(long taskId, long campaignId, long levelToLoad, string entity, string accountName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetails(taskId, campaignId, levelToLoad, entity, accountName));
                //var snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskId,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }

        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskId(long taskId, long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetails(taskId, campaignId)).ToList();
                //var snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskId,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }

        public static PresentationAttestationSnapshotActionsForTask GetSavedAttestationSnapshotByTaskId(long taskId, long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetSavedAttestationSnapshotActionDetails(taskId, campaignId)).ToList();
                var snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskIdToStore,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }

        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskIdForReport(long taskId, long campaignId, bool forReport, bool reportFromWorkflow = false)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                List<AttestationSnapshotAction> snapshotForTaskId = new List<AttestationSnapshotAction>();
                if(!reportFromWorkflow)
                    snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetails(taskId, campaignId)).ToList();
                else
                    snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetailsForExport(campaignId)).ToList();
                List<AttestationSnapshotAction> snapshotForTaskIdToStore = new List<AttestationSnapshotAction>();
                if (!forReport)
                    snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                else
                    snapshotForTaskIdToStore = snapshotForTaskId.ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskIdToStore,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }

        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskIdFromHistory(long taskId, long campaignId, bool forReport)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetailsFromHistory(taskId, campaignId)).ToList();
                List<AttestationSnapshotAction> snapshotForTaskIdToStore = new List<AttestationSnapshotAction>();
                if (!forReport)
                    snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible).ToList();
                else
                    snapshotForTaskIdToStore = snapshotForTaskId.ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskIdToStore,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }
        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskIdFromHistory(long taskId, long campaignId, bool forReport, string entity)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionDetailsFromHistory(taskId, campaignId, entity)).ToList();
                List<AttestationSnapshotAction> snapshotForTaskIdToStore = new List<AttestationSnapshotAction>();
                if (!forReport)
                    snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible).ToList();
                else
                    snapshotForTaskIdToStore = snapshotForTaskId.ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskIdToStore,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }
        public static PresentationAttestationSnapshotActionsForTask GetAttestationSnapshotByTaskIdByActionId(long taskId, long campaignId, string entity)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                AttestationSnapshotActionRepository rep = new AttestationSnapshotActionRepository(dataAccessTransaction);
                var snapshotForTaskId = ConvertToSnapshotActionDomainModel(rep.GetAttestationSnapshotActionFullDetails(taskId, campaignId, entity)).ToList();
                //var snapshotForTaskIdToStore = snapshotForTaskId.Where(a => a.IsVisible && a.IsActionable).ToList();
                var unsavedChanges = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                return new PresentationAttestationSnapshotActionsForTask
                {
                    AttestationSnapshotActions = snapshotForTaskId,
                    TaskID = taskId,
                    IsUnSavedChangesAvailable = unsavedChanges != null ? unsavedChanges.UnSavedChangesAvailable : false,
                };
            }
        }
        public static List<AttestationSnapshotAction> ConvertToSnapshotActionDomainModel(List<EntityModel.AttestationSnapshotAction> eAttestationSnapshotActions)
        {
            List<AttestationSnapshotAction> attestationSnapshotActions = new List<AttestationSnapshotAction>();
            foreach (var eAttestationSnapshotAction in eAttestationSnapshotActions)
            {
                attestationSnapshotActions.Add(new AttestationSnapshotAction()
                {
                    Approve = eAttestationSnapshotAction.Approve,
                    AttestationSnapshot = ConvertToSnapshotDomainModel(eAttestationSnapshotAction.AttestationSnapshot),
                    AttestationSnapshotId = eAttestationSnapshotAction.AttestationSnapshotId,
                    Comment = eAttestationSnapshotAction.Comment,
                    CreatedBy = eAttestationSnapshotAction.CreatedBy,
                    CreatedDate = eAttestationSnapshotAction.CreatedDate,
                    EffectiveDate = eAttestationSnapshotAction.EffectiveDate,
                    Expired = eAttestationSnapshotAction.Expired,
                    Id = eAttestationSnapshotAction.Id,
                    IsActionable = eAttestationSnapshotAction.IsActionable,
                    IsActionExtendable = eAttestationSnapshotAction.IsActionExtendable,
                    IsChanged = eAttestationSnapshotAction.IsChanged,
                    IsChangedInPreviousLevel = eAttestationSnapshotAction.IsChangedInPreviousLevel,
                    IsDelta = eAttestationSnapshotAction.IsDelta,
                    IsException = eAttestationSnapshotAction.IsException,
                    IsReviewerGroup = eAttestationSnapshotAction.IsReviewerGroup,
                    IsVisible = eAttestationSnapshotAction.IsVisible,
                    LastModifiedBy = eAttestationSnapshotAction.LastModifiedBy,
                    LastModifiedDate = eAttestationSnapshotAction.LastModifiedDate,
                    RemediationStatus = eAttestationSnapshotAction.RemediationStatus,
                    Reserved1 = eAttestationSnapshotAction.Reserved1,
                    Reserved2 = eAttestationSnapshotAction.Reserved2,
                    Reserved3 = eAttestationSnapshotAction.Reserved3,
                    ReviewerId = eAttestationSnapshotAction.ReviewerId,
                    Revoke = eAttestationSnapshotAction.Revoke,
                    RootSnapshotId = eAttestationSnapshotAction.RootSnapshotId,
                    TaskId = eAttestationSnapshotAction.TaskId
                });
            }
            return attestationSnapshotActions;
        }
        public static AttestationSnapshot ConvertToSnapshotDomainModel(EntityModel.AttestationSnapshot eAttestationSnapshot)
        {
            if (eAttestationSnapshot == null) return null;

            AttestationSnapshot attestationSnapshot = new AttestationSnapshot()
            {
                AccountName = eAttestationSnapshot.AccountName,
                AccountStatus = eAttestationSnapshot.AccountStatus,
                AccountType = eAttestationSnapshot.AccountType,
                AttestationCampaign = AutoMapper.Mapper.Map<EntityModel.AttestationCampaign, AttestationCampaign>(eAttestationSnapshot.AttestationCampaign),
                AttestationCampaignId = eAttestationSnapshot.AttestationCampaignId,
                CreatedBy = eAttestationSnapshot.CreatedBy,
                CreatedDate = eAttestationSnapshot.CreatedDate,
                Description = eAttestationSnapshot.Description,
                DIUserDisplayName = eAttestationSnapshot.DIUserDisplayName,
                DIUserName = eAttestationSnapshot.DIUserName,
                EndpointId = eAttestationSnapshot.EndpointId,
                EndpointName = eAttestationSnapshot.EndpointName,
                EndpointType = eAttestationSnapshot.EndpointType,
                EntitlementType = eAttestationSnapshot.EntitlementType,
                Entity = eAttestationSnapshot.Entity,
                HasRisk = eAttestationSnapshot.HasRisk,
                Id = eAttestationSnapshot.Id,
                KeyId = eAttestationSnapshot.KeyId,
                LastLogon = eAttestationSnapshot.LastLogon,
                LastModifiedBy = eAttestationSnapshot.LastModifiedBy,
                LastModifiedDate = eAttestationSnapshot.LastModifiedDate,
                Level = eAttestationSnapshot.Level,
                Name = eAttestationSnapshot.Name,
                ParentEntity = eAttestationSnapshot.ParentEntity,
                Reserved1 = eAttestationSnapshot.Reserved1,
                Reserved10 = eAttestationSnapshot.Reserved10,
                Reserved2 = eAttestationSnapshot.Reserved2,
                Reserved3 = eAttestationSnapshot.Reserved3,
                Reserved4 = eAttestationSnapshot.Reserved4,
                Reserved5 = eAttestationSnapshot.Reserved5,
                Reserved6 = eAttestationSnapshot.Reserved6,
                Reserved7 = eAttestationSnapshot.Reserved7,
                Reserved8 = eAttestationSnapshot.Reserved8,
                Reserved9 = eAttestationSnapshot.Reserved9,
                RootSnapshotId = eAttestationSnapshot.RootSnapshotId,
                UserId = eAttestationSnapshot.UserId
            };
            return attestationSnapshot;
        }
    }
}

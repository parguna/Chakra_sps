using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class AttestationSnapshotActionRepository : RepositoryBase<AttestationSnapshotAction>
    {
        List<AttestationSnapshotAction> needToOrder = new List<AttestationSnapshotAction>();
        public AttestationSnapshotActionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetails(long taskId, long campaignId, long levelToLoad, string entity, string accountName)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking().Include("AttestationSnapshot")
                                   join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                   on action.Id equals interim.AttestationSnapshotActionId
                                   where action.AttestationSnapshot.AttestationCampaignId == campaignId &&
                                   action.AttestationSnapshot.Level == levelToLoad &&
                                   action.AttestationSnapshot.ParentEntity == entity
                                   select new
                                   {
                                       Id = action.Id,
                                       AttestationSnapshotId = action.AttestationSnapshotId,
                                       TaskId = interim.TaskId,
                                       Approve = interim.UnSavedApprove,
                                       Revoke = interim.UnSavedRevoke,
                                       Comment = interim.UnSavedComment,
                                       EffectiveDate = interim.UnSavedEffectiveDate,
                                       ReviewerId = interim.ReviewerId,
                                       Expired = action.Expired,
                                       IsChanged = interim.UnSavedIsChanged,
                                       IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                       IsDelta = action.IsDelta,
                                       IsVisible = action.IsVisible,
                                       IsActionable = action.IsActionable,
                                       IsActionExtendable = action.IsActionExtendable,
                                       IsException = action.IsException,
                                       Reserved1 = interim.UnSavedReserved1,
                                       Reserved2 = interim.UnSavedReserved2,
                                       Reserved3 = interim.UnSavedReserved3,
                                       CreatedBy = action.CreatedBy,
                                       CreatedDate = action.CreatedDate,
                                       LastModifiedBy = interim.LastModifiedBy,
                                       LastModifiedDate = interim.LastModifiedDate,
                                       AttestationSnapshot = action.AttestationSnapshot,
                                       RemediationStatus = action.RemediationStatus,
                                   }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                   {
                                       Id = a.Id,
                                       AttestationSnapshotId = a.AttestationSnapshotId,
                                       TaskId = a.TaskId,
                                       Approve = a.Approve,
                                       Revoke = a.Revoke,
                                       Comment = a.Comment,
                                       EffectiveDate = a.EffectiveDate,
                                       ReviewerId = a.ReviewerId,
                                       Expired = a.Expired,
                                       IsChanged = a.IsChanged,
                                       IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                       IsDelta = a.IsDelta,
                                       IsVisible = a.IsVisible,
                                       IsActionable = a.IsActionable,
                                       IsActionExtendable = a.IsActionExtendable,
                                       IsException = a.IsException,
                                       Reserved1 = a.Reserved1,
                                       Reserved2 = a.Reserved2,
                                       Reserved3 = a.Reserved3,
                                       CreatedBy = a.CreatedBy,
                                       CreatedDate = a.CreatedDate,
                                       LastModifiedBy = a.LastModifiedBy,
                                       LastModifiedDate = a.LastModifiedDate,
                                       AttestationSnapshot = a.AttestationSnapshot,
                                       RemediationStatus = a.RemediationStatus,
                                   }).ToList();

            if (snapshotForCampaign == null || snapshotForCampaign.Count == 0)
                return new List<AttestationSnapshotAction>();

            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign;

            if (reviewerSnapshot != null && reviewerSnapshot.Count > 0 && reviewerSnapshot.Any(a => a.IsVisible))
                return reviewerSnapshot;

            List<string> entities = new List<string>();
            entities = reviewerSnapshot.Select(a => a.AttestationSnapshot.Entity).ToList();
            long levelLoaded = levelToLoad;

            var maxLevel = (from snapshot in CurrentContext.AttestationSnapshots
                            where snapshot.AttestationCampaignId == campaignId
                            select snapshot.Level).Max();

            for (long level = levelLoaded + 1; level <= maxLevel; level++)
            {
                var levelSnapshot = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking().Include("AttestationSnapshot")
                                     join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                     on action.Id equals interim.AttestationSnapshotActionId
                                     where action.AttestationSnapshot.AttestationCampaignId == campaignId &&
                                     action.AttestationSnapshot.Level == level &&
                                     entities.Contains(action.AttestationSnapshot.ParentEntity)
                                     select new
                                     {
                                         Id = action.Id,
                                         AttestationSnapshotId = action.AttestationSnapshotId,
                                         TaskId = interim.TaskId,
                                         Approve = interim.UnSavedApprove,
                                         Revoke = interim.UnSavedRevoke,
                                         Comment = interim.UnSavedComment,
                                         EffectiveDate = interim.UnSavedEffectiveDate,
                                         ReviewerId = interim.ReviewerId,
                                         Expired = action.Expired,
                                         IsChanged = interim.UnSavedIsChanged,
                                         IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                         IsDelta = action.IsDelta,
                                         IsVisible = action.IsVisible,
                                         IsActionable = action.IsActionable,
                                         IsActionExtendable = action.IsActionExtendable,
                                         IsException = action.IsException,
                                         Reserved1 = interim.UnSavedReserved1,
                                         Reserved2 = interim.UnSavedReserved2,
                                         Reserved3 = interim.UnSavedReserved3,
                                         CreatedBy = action.CreatedBy,
                                         CreatedDate = action.CreatedDate,
                                         LastModifiedBy = interim.LastModifiedBy,
                                         LastModifiedDate = interim.LastModifiedDate,
                                         AttestationSnapshot = action.AttestationSnapshot,
                                         RemediationStatus = action.RemediationStatus,
                                     }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                     {
                                         Id = a.Id,
                                         AttestationSnapshotId = a.AttestationSnapshotId,
                                         TaskId = a.TaskId,
                                         Approve = a.Approve,
                                         Revoke = a.Revoke,
                                         Comment = a.Comment,
                                         EffectiveDate = a.EffectiveDate,
                                         ReviewerId = a.ReviewerId,
                                         Expired = a.Expired,
                                         IsChanged = a.IsChanged,
                                         IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                         IsDelta = a.IsDelta,
                                         IsVisible = a.IsVisible,
                                         IsActionable = a.IsActionable,
                                         IsActionExtendable = a.IsActionExtendable,
                                         IsException = a.IsException,
                                         Reserved1 = a.Reserved1,
                                         Reserved2 = a.Reserved2,
                                         Reserved3 = a.Reserved3,
                                         CreatedBy = a.CreatedBy,
                                         CreatedDate = a.CreatedDate,
                                         LastModifiedBy = a.LastModifiedBy,
                                         LastModifiedDate = a.LastModifiedDate,
                                         AttestationSnapshot = a.AttestationSnapshot,
                                         RemediationStatus = a.RemediationStatus,
                                     }).ToList();

                reviewerSnapshot = levelSnapshot.ToList();
                if (reviewerSnapshot != null && reviewerSnapshot.Count > 0 && reviewerSnapshot.Any(a => a.IsVisible))
                    return reviewerSnapshot;

                entities = reviewerSnapshot.Select(a => a.AttestationSnapshot.Entity).ToList();
            }

            return reviewerSnapshot;
        }

        public bool UpdateAttestationTaskActionByUser(long taskId, long endpointId, long userId, string entitlement, string userName, AccountType accountType, string accessType, long actionUserId, int level, EntitlementType entitlementType)
        {
            try
            {
                var sqlParamtaskId = new SqlParameter();
                sqlParamtaskId.ParameterName = "@taskId";
                sqlParamtaskId.SqlDbType = SqlDbType.BigInt;
                sqlParamtaskId.Value = taskId;

                var sqlParamendpointId = new SqlParameter();
                sqlParamendpointId.ParameterName = "@endpointId";
                sqlParamendpointId.SqlDbType = SqlDbType.BigInt;
                sqlParamendpointId.Value = endpointId;

                var sqlParamuserid = new SqlParameter();
                sqlParamuserid.ParameterName = "@userid";
                sqlParamuserid.SqlDbType = SqlDbType.BigInt;
                sqlParamuserid.Value = userId;

                var sqlParamactionUserId = new SqlParameter();
                sqlParamactionUserId.ParameterName = "@actionUserId";
                sqlParamactionUserId.SqlDbType = SqlDbType.BigInt;
                sqlParamactionUserId.Value = actionUserId;


                var sqlParamentitlement = new SqlParameter();
                sqlParamentitlement.ParameterName = "@entitlement";
                sqlParamentitlement.SqlDbType = SqlDbType.NVarChar;
                sqlParamentitlement.Size = 1024;
                sqlParamentitlement.Value = entitlement;

                var sqlParamusername = new SqlParameter();
                sqlParamusername.ParameterName = "@username";
                sqlParamusername.SqlDbType = SqlDbType.NVarChar;
                sqlParamusername.Size = 1024;
                sqlParamusername.Value = userName;

                var sqlParamaccountType = new SqlParameter();
                sqlParamaccountType.ParameterName = "@accountType";
                sqlParamaccountType.SqlDbType = SqlDbType.TinyInt;
                sqlParamaccountType.Value = accountType;

                var sqlParamaccessType = new SqlParameter();
                sqlParamaccessType.ParameterName = "@accessType";
                sqlParamaccessType.SqlDbType = SqlDbType.NVarChar;
                sqlParamaccessType.Size = 32;
                sqlParamaccessType.Value = accessType;

                var sqlParamlevel = new SqlParameter();
                sqlParamlevel.ParameterName = "@level";
                sqlParamlevel.SqlDbType = SqlDbType.Int;
                sqlParamlevel.Size = 32;
                sqlParamlevel.Value = level;

                var sqlParamEntitlementType = new SqlParameter();
                sqlParamEntitlementType.ParameterName = "@entitlementType";
                sqlParamEntitlementType.SqlDbType = SqlDbType.Int;
                sqlParamEntitlementType.Value = (int)entitlementType;

                CurrentContext.Database.CommandTimeout = 300;
                object[] parameters = { sqlParamtaskId, sqlParamendpointId, sqlParamuserid, sqlParamactionUserId, sqlParamentitlement, sqlParamusername, sqlParamaccountType, sqlParamaccessType, sqlParamlevel, sqlParamEntitlementType };
                CurrentContext.Database.ExecuteSqlCommand($"EXEC USP_UpdateAttestationReviewAction @endpointId,@taskId,@userid,@entitlement,@username,@accountType,@accessType,@actionUserId,@level,@entitlementType", parameters);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetails(long taskId, long campaignId)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                                   join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on action.AttestationSnapshotId equals snapShot.Id
                                   join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                   on action.Id equals interim.AttestationSnapshotActionId
                                   where snapShot.AttestationCampaignId == campaignId
                                   select new
                                   {
                                       Id = action.Id,
                                       AttestationSnapshotId = action.AttestationSnapshotId,
                                       TaskId = interim.TaskId,
                                       Approve = interim.UnSavedApprove,
                                       Revoke = interim.UnSavedRevoke,
                                       Comment = interim.UnSavedComment,
                                       EffectiveDate = interim.UnSavedEffectiveDate,
                                       ReviewerId = interim.ReviewerId,
                                       Expired = action.Expired,
                                       IsChanged = interim.UnSavedIsChanged,
                                       IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                       IsDelta = action.IsDelta,
                                       IsVisible = action.IsVisible,
                                       IsActionable = action.IsActionable,
                                       IsActionExtendable = action.IsActionExtendable,
                                       IsException = action.IsException,
                                       Reserved1 = interim.UnSavedReserved1,
                                       Reserved2 = interim.UnSavedReserved2,
                                       Reserved3 = interim.UnSavedReserved3,
                                       CreatedBy = action.CreatedBy,
                                       CreatedDate = action.CreatedDate,
                                       LastModifiedBy = interim.LastModifiedBy,
                                       LastModifiedDate = interim.LastModifiedDate,
                                       AttestationSnapshot = snapShot,
                                       RemediationStatus = action.RemediationStatus,
                                   }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                   {
                                       Id = a.Id,
                                       AttestationSnapshotId = a.AttestationSnapshotId,
                                       TaskId = a.TaskId,
                                       Approve = a.Approve,
                                       Revoke = a.Revoke,
                                       Comment = a.Comment,
                                       EffectiveDate = a.EffectiveDate,
                                       ReviewerId = a.ReviewerId,
                                       Expired = a.Expired,
                                       IsChanged = a.IsChanged,
                                       IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                       IsDelta = a.IsDelta,
                                       IsVisible = a.IsVisible,
                                       IsActionable = a.IsActionable,
                                       IsActionExtendable = a.IsActionExtendable,
                                       IsException = a.IsException,
                                       Reserved1 = a.Reserved1,
                                       Reserved2 = a.Reserved2,
                                       Reserved3 = a.Reserved3,
                                       CreatedBy = a.CreatedBy,
                                       CreatedDate = a.CreatedDate,
                                       LastModifiedBy = a.LastModifiedBy,
                                       LastModifiedDate = a.LastModifiedDate,
                                       AttestationSnapshot = a.AttestationSnapshot,
                                       RemediationStatus = a.RemediationStatus,
                                   }).ToList();

            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign.Where(a =>
                                // Taking User level only
                                a.TaskId == taskId).ToList();

            List<long> actionIds = new List<long>();
            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            long levelLoaded = 1;
            if (reviewerSnapshot != null && reviewerSnapshot.Count > 0)
            {
                levelLoaded = reviewerSnapshot.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
            }

            var maxLevel = snapshotForCampaign.Select(a => a.AttestationSnapshot.Level).Distinct().Max();

            for (long level = levelLoaded + 1; level <= maxLevel; level++)
            {
                var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                if (previousEntity.Count > 500)
                {
                    for (int i = 0; i < previousEntity.Count; i = i + 500)
                    {
                        var levelSnapshot = snapshotForCampaign.Where(a => !actionIds.Contains(a.Id)
                                     && previousEntity.Skip(i).Take(500).Contains(a.AttestationSnapshot.ParentEntity)).ToList();
                        reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                        actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
                    }
                }
                else
                {
                    var levelSnapshot = snapshotForCampaign.Where(a => !actionIds.Contains(a.Id)
                                     && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)).ToList();
                    reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                    actionIds = reviewerSnapshot.Select(a => a.Id).ToList();

                }
                //var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                //var levelSnapshot = snapshotForCampaign.Where(a => !actionIds.Contains(a.Id)
                //                     && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)).ToList();                                     

                //reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                //actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            }

            return reviewerSnapshot;
        }
        List<AttestationSnapshotAction> GetAttestationSnapshotsForCampaign(long campaignId)
        {
            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking().Include("AttestationSnapshot")
                                join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                on action.Id equals interim.AttestationSnapshotActionId
                                where action.AttestationSnapshot.AttestationCampaignId == campaignId
                                select new
                                {
                                    Id = action.Id,
                                    AttestationSnapshotId = action.AttestationSnapshotId,
                                    TaskId = interim.TaskId,
                                    Approve = interim.UnSavedApprove,
                                    Revoke = interim.UnSavedRevoke,
                                    Comment = interim.UnSavedComment,
                                    EffectiveDate = interim.UnSavedEffectiveDate,
                                    ReviewerId = interim.ReviewerId,
                                    Expired = action.Expired,
                                    IsCurrentChangeSaved = interim.SavedIsChanged,
                                    IsChanged = interim.UnSavedIsChanged,
                                    IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                    IsVisible = action.IsVisible,
                                    IsActionable = action.IsActionable,
                                    IsActionExtendable = action.IsActionExtendable,
                                    IsException = action.IsException,
                                    Reserved1 = action.Reserved1,
                                    Reserved2 = action.Reserved2,
                                    Reserved3 = action.Reserved3,
                                    CreatedBy = action.CreatedBy,
                                    CreatedDate = action.CreatedDate,
                                    LastModifiedBy = interim.LastModifiedBy,
                                    LastModifiedDate = interim.LastModifiedDate,
                                    AttestationSnapshot = action.AttestationSnapshot,
                                    RemediationStatus = action.RemediationStatus,
                                }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                {
                                    Id = a.Id,
                                    AttestationSnapshotId = a.AttestationSnapshotId,
                                    TaskId = a.TaskId,
                                    Approve = a.Approve,
                                    Revoke = a.Revoke,
                                    Comment = a.Comment,
                                    EffectiveDate = a.EffectiveDate,
                                    ReviewerId = a.ReviewerId,
                                    Expired = a.Expired,
                                    IsChanged = a.IsChanged,
                                    IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                    IsVisible = a.IsVisible,
                                    IsActionable = a.IsActionable,
                                    IsActionExtendable = a.IsActionExtendable,
                                    IsException = a.IsException,
                                    Reserved1 = a.Reserved1,
                                    Reserved2 = a.Reserved2,
                                    Reserved3 = a.Reserved3,
                                    CreatedBy = a.CreatedBy,
                                    CreatedDate = a.CreatedDate,
                                    LastModifiedBy = a.LastModifiedBy,
                                    LastModifiedDate = a.LastModifiedDate,
                                    AttestationSnapshot = a.AttestationSnapshot,
                                    RemediationStatus = a.RemediationStatus,
                                }).ToList();
            return reviewerSnapshot;
        }
        public List<AttestationSnapshotAction> GetAttestationSnapshotActionFullDetails(long taskId, long campaignId, string entity)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = GetAttestationSnapshotsForCampaign(campaignId);

            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign.Where(a => a.TaskId == taskId && a.AttestationSnapshot.Entity == entity).ToList();

            return reviewerSnapshot;
        }
        public List<AttestationSnapshotAction> GetAttestationSnapshotActionFullDetails(long taskId, long campaignId, long snapshotActionId)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = GetAttestationSnapshotsForCampaign(campaignId);

            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign.Where(a => a.Id == snapshotActionId).ToList();

            if (reviewerSnapshot != null && reviewerSnapshot.FirstOrDefault().AttestationSnapshot.RootSnapshotId.HasValue)
            {
                var firstEntry = reviewerSnapshot.FirstOrDefault().AttestationSnapshot;
                reviewerSnapshot.AddRange(snapshotForCampaign.Where(a => a.AttestationSnapshot.Id == firstEntry.RootSnapshotId.Value));
                reviewerSnapshot.AddRange(snapshotForCampaign.Where(a => a.AttestationSnapshot.ParentEntity == firstEntry.Entity
                                          && a.AttestationSnapshot.RootSnapshotId == firstEntry.RootSnapshotId.Value));
            }
            else if (reviewerSnapshot != null && !reviewerSnapshot.FirstOrDefault().AttestationSnapshot.RootSnapshotId.HasValue)
            {
                reviewerSnapshot.AddRange(snapshotForCampaign.Where(a => a.AttestationSnapshot.RootSnapshotId == reviewerSnapshot.FirstOrDefault().AttestationSnapshot.Id));
            }

            List<long> actionIds = new List<long>();
            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            long levelLoaded = 1;
            levelLoaded = reviewerSnapshot.Select(a => a.AttestationSnapshot.Level).Min();
            long maxLevel = 1; long minLevel = 1; var endpoint = (reviewerSnapshot != null && reviewerSnapshot.Count > 0) ? reviewerSnapshot.First().AttestationSnapshot.EndpointId : 0;
            if (reviewerSnapshot != null && reviewerSnapshot.Count > 0)
            {
                var levels = snapshotForCampaign.Where(a => a.AttestationSnapshot.EndpointId == endpoint).Select(a => a.AttestationSnapshot.Level).Distinct().ToList();
                minLevel = levels.Min();
                maxLevel = levels.Max();
            }

            for (long level = levelLoaded + 1; level <= maxLevel; level++)
            {
                var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                var levelSnapshot = snapshotForCampaign.Where(a => a.AttestationSnapshot.EndpointId == endpoint
                                     && !actionIds.Contains(a.Id)
                                     && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)
                                     && a.IsVisible && a.IsActionable).ToList();

                reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            }

            if (levelLoaded > 1)
            {
                for (long level = levelLoaded - 1; level <= minLevel; level--)
                {
                    if (level == 0) break;
                    var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level + 1).Select(a => a.AttestationSnapshot.ParentEntity).Distinct().ToList();
                    var levelSnapshot = snapshotForCampaign.Where(a => a.AttestationSnapshot.EndpointId == endpoint
                                         && !actionIds.Contains(a.Id)
                                         // Taking User level only
                                         && previousEntity.Contains(a.AttestationSnapshot.Entity)
                                         && a.IsVisible && a.IsActionable).ToList();

                    reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                    actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
                }
            }

            needToOrder = reviewerSnapshot.OrderBy(a => a.AttestationSnapshot.EndpointId).ThenBy(a => a.AttestationSnapshot.AccountName).ThenBy(a => a.AttestationSnapshot.Level).ThenBy(a => a.AttestationSnapshot.Entity)
                .OrderByDescending(a => a.IsChangedInPreviousLevel).ThenByDescending(a => a.IsChanged).ToList();
            reviewerSnapshot = GetList().ToList();

            return reviewerSnapshot;
        }
        IEnumerable<AttestationSnapshotAction> GetChildList(AttestationSnapshotAction parent)
        {
            List<AttestationSnapshotAction> ReportingOrder = new List<AttestationSnapshotAction>();
            if (parent != null)
            {
                var parentEntity = parent.AttestationSnapshot.Entity;
                var accountName = parent.AttestationSnapshot.AccountName;

                var childs = needToOrder.Where(x => x.AttestationSnapshot.ParentEntity == parentEntity &&
                x.AttestationSnapshot.Level == (parent.AttestationSnapshot.Level + 1) &&
               (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(x.AttestationSnapshot.AccountName) || x.AttestationSnapshot.AccountName == accountName))
               .OrderBy(x => x.AttestationSnapshot.Level).ToList();

                if (childs != null && childs.Count > 0)
                {
                    foreach (var child in childs)
                    {
                        ReportingOrder.Add(child);
                        var childRows = GetChildList(child);
                        if (childRows != null && childRows.Count() > 0)
                            ReportingOrder.AddRange(childRows);
                    }
                }
            }

            return ReportingOrder;
        }
        IEnumerable<AttestationSnapshotAction> GetList()
        {
            if (needToOrder.Any(a => string.IsNullOrEmpty(a.AttestationSnapshot.AccountName)))
            {
                var lvlToStart = needToOrder.Where(a => string.IsNullOrEmpty(a.AttestationSnapshot.AccountName)).Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                var lvlToFill = needToOrder.Where(a => a.AttestationSnapshot.Level == lvlToStart - 1).ToList();
                if (lvlToFill != null && lvlToFill.Count > 0)
                {
                    foreach (var level in lvlToFill)
                    {
                        needToOrder.InsertRange(needToOrder.FindIndex(a => a.AttestationSnapshotId == level.AttestationSnapshotId), GetChildList(level));
                    }
                }
            }

            return needToOrder;
        }
        public List<AttestationSnapshotAction> GetSavedAttestationSnapshotActionDetails(long taskId, long campaignId)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = (from action in CurrentContext.AttestationSnapshotActions.Include("AttestationSnapshot")
                                   join interim in CurrentContext.AttestationSnapshotActionInterims
                                   on action.Id equals interim.AttestationSnapshotActionId
                                   where action.AttestationSnapshot.AttestationCampaignId == campaignId
                                   select new
                                   {
                                       Id = action.Id,
                                       AttestationSnapshotId = action.AttestationSnapshotId,
                                       TaskId = interim.TaskId,
                                       Approve = interim.UnSavedApprove,
                                       Revoke = interim.UnSavedRevoke,
                                       Comment = interim.UnSavedComment,
                                       EffectiveDate = interim.UnSavedEffectiveDate,
                                       ReviewerId = interim.ReviewerId,
                                       Expired = action.Expired,
                                       IsChanged = interim.UnSavedIsChanged,
                                       IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                       IsDelta = action.IsDelta,
                                       IsVisible = action.IsVisible,
                                       IsActionable = action.IsActionable,
                                       IsActionExtendable = action.IsActionExtendable,
                                       IsException = action.IsException,
                                       Reserved1 = interim.UnSavedReserved1,
                                       Reserved2 = interim.UnSavedReserved2,
                                       Reserved3 = interim.UnSavedReserved3,
                                       CreatedBy = action.CreatedBy,
                                       CreatedDate = action.CreatedDate,
                                       LastModifiedBy = interim.LastModifiedBy,
                                       LastModifiedDate = interim.LastModifiedDate,
                                       AttestationSnapshot = action.AttestationSnapshot,
                                       RemediationStatus = action.RemediationStatus,
                                   }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                   {
                                       Id = a.Id,
                                       AttestationSnapshotId = a.AttestationSnapshotId,
                                       TaskId = a.TaskId,
                                       Approve = a.Approve,
                                       Revoke = a.Revoke,
                                       Comment = a.Comment,
                                       EffectiveDate = a.EffectiveDate,
                                       ReviewerId = a.ReviewerId,
                                       Expired = a.Expired,
                                       IsChanged = a.IsChanged,
                                       IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                       IsDelta = a.IsDelta,
                                       IsVisible = a.IsVisible,
                                       IsActionable = a.IsActionable,
                                       IsActionExtendable = a.IsActionExtendable,
                                       IsException = a.IsException,
                                       Reserved1 = a.Reserved1,
                                       Reserved2 = a.Reserved2,
                                       Reserved3 = a.Reserved3,
                                       CreatedBy = a.CreatedBy,
                                       CreatedDate = a.CreatedDate,
                                       LastModifiedBy = a.LastModifiedBy,
                                       LastModifiedDate = a.LastModifiedDate,
                                       AttestationSnapshot = a.AttestationSnapshot,
                                       RemediationStatus = a.RemediationStatus,
                                   }).ToList();
            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign.Where(a =>
                                // Taking User level only
                                a.TaskId == taskId).ToList();

            List<long> actionIds = new List<long>();
            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            long levelLoaded = 1;
            if (reviewerSnapshot != null && reviewerSnapshot.Count > 0)
            {
                levelLoaded = reviewerSnapshot.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
            }

            var maxLevel = snapshotForCampaign.Select(a => a.AttestationSnapshot.Level).Distinct().Max();

            for (long level = levelLoaded + 1; level <= maxLevel; level++)
            {
                var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                var levelSnapshot = snapshotForCampaign.Where(a =>
                                     !actionIds.Contains(a.Id)
                                     // Taking User level only
                                     && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)).ToList();

                reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            }

            return reviewerSnapshot;
        }

        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetailsForExport(long campaignId)
        {
            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                                join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on action.AttestationSnapshotId equals snapShot.Id
                                join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                on action.Id equals interim.AttestationSnapshotActionId
                                where snapShot.AttestationCampaignId == campaignId
                                select new
                                {
                                    Id = action.Id,
                                    AttestationSnapshotId = action.AttestationSnapshotId,
                                    TaskId = interim.TaskId,
                                    Approve = action.Approve,
                                    Revoke = action.Revoke,
                                    Comment = action.Comment,
                                    EffectiveDate = action.EffectiveDate,
                                    ReviewerId = interim.ReviewerId,
                                    Expired = action.Expired,
                                    IsChanged = action.IsChanged,
                                    IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                    IsDelta = action.IsDelta,
                                    IsVisible = action.IsVisible,
                                    IsActionable = action.IsActionable,
                                    IsActionExtendable = action.IsActionExtendable,
                                    IsException = action.IsException,
                                    Reserved1 = action.Reserved1,
                                    Reserved2 = action.Reserved2,
                                    Reserved3 = action.Reserved3,
                                    CreatedBy = action.CreatedBy == "System" ? action.CreatedBy : (CurrentContext.User.Where(i => i.UserName == action.CreatedBy).Select(i => new { displayName = i.DisplayName + "(" + i.UserName + ")" }).FirstOrDefault().displayName),
                                    CreatedDate = action.CreatedDate,
                                    LastModifiedBy = interim.LastModifiedBy == "System" ? interim.LastModifiedBy : (CurrentContext.User.Where(i => i.UserName == interim.LastModifiedBy).Select(i => new { displayName = i.DisplayName + "(" + i.UserName + ")" }).FirstOrDefault().displayName),
                                    LastModifiedDate = interim.LastModifiedDate,
                                    AttestationSnapshot = snapShot,
                                    RemediationStatus = action.RemediationStatus,
                                }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                {
                                    Id = a.Id,
                                    AttestationSnapshotId = a.AttestationSnapshotId,
                                    TaskId = a.TaskId,
                                    Approve = a.Approve,
                                    Revoke = a.Revoke,
                                    Comment = a.Comment,
                                    EffectiveDate = a.EffectiveDate,
                                    ReviewerId = a.ReviewerId,
                                    Expired = a.Expired,
                                    IsChanged = a.IsChanged,
                                    IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                    IsDelta = a.IsDelta,
                                    IsVisible = a.IsVisible,
                                    IsActionable = a.IsActionable,
                                    IsActionExtendable = a.IsActionExtendable,
                                    IsException = a.IsException,
                                    Reserved1 = a.Reserved1,
                                    Reserved2 = a.Reserved2,
                                    Reserved3 = a.Reserved3,
                                    CreatedBy = a.CreatedBy,
                                    CreatedDate = a.CreatedDate,
                                    LastModifiedBy = a.LastModifiedBy,
                                    LastModifiedDate = a.LastModifiedDate,
                                    AttestationSnapshot = a.AttestationSnapshot,
                                    RemediationStatus = a.RemediationStatus,
                                }).ToList();
            return reviewerSnapshot;
        }

        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetailsFromHistory(long taskId, long campaignId, string entity = null)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();

            var query = CurrentContext.AttestationSnapshotActionHistories.AsNoTracking().Where(a => a.TaskId == taskId);
            var historyActionIds = query.Select(a => a.AttestationSnapshotActionId).Distinct();

            if (historyActionIds.Count() == 0)
                return GetAttestationSnapshotActionDetails(taskId, campaignId);

            snapshotForCampaign = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                                   join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on action.AttestationSnapshotId equals snapShot.Id
                                   join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                   on action.Id equals interim.AttestationSnapshotActionId
                                   where snapShot.AttestationCampaignId == campaignId && (entity == null ? true : snapShot.ParentEntity == entity)
                                   select new
                                   {
                                       Id = action.Id,
                                       AttestationSnapshotId = action.AttestationSnapshotId,
                                       TaskId = interim.TaskId,
                                       Approve = interim.UnSavedApprove,
                                       Revoke = interim.UnSavedRevoke,
                                       Comment = interim.UnSavedComment,
                                       EffectiveDate = interim.UnSavedEffectiveDate,
                                       ReviewerId = interim.ReviewerId,
                                       Expired = action.Expired,
                                       IsChanged = interim.UnSavedIsChanged,
                                       IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                       IsDelta = action.IsDelta,
                                       IsVisible = action.IsVisible,
                                       IsActionable = action.IsActionable,
                                       IsActionExtendable = action.IsActionExtendable,
                                       IsException = action.IsException,
                                       Reserved1 = interim.UnSavedReserved1,
                                       Reserved2 = interim.UnSavedReserved2,
                                       Reserved3 = interim.UnSavedReserved3,
                                       CreatedBy = action.CreatedBy,
                                       CreatedDate = action.CreatedDate,
                                       LastModifiedBy = interim.LastModifiedBy,
                                       LastModifiedDate = interim.LastModifiedDate,
                                       AttestationSnapshot = snapShot,
                                       RemediationStatus = action.RemediationStatus,
                                   }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                   {
                                       Id = a.Id,
                                       AttestationSnapshotId = a.AttestationSnapshotId,
                                       TaskId = a.TaskId,
                                       Approve = a.Approve,
                                       Revoke = a.Revoke,
                                       Comment = a.Comment,
                                       EffectiveDate = a.EffectiveDate,
                                       ReviewerId = a.ReviewerId,
                                       Expired = a.Expired,
                                       IsChanged = a.IsChanged,
                                       IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                       IsDelta = a.IsDelta,
                                       IsVisible = a.IsVisible,
                                       IsActionable = a.IsActionable,
                                       IsActionExtendable = a.IsActionExtendable,
                                       IsException = a.IsException,
                                       Reserved1 = a.Reserved1,
                                       Reserved2 = a.Reserved2,
                                       Reserved3 = a.Reserved3,
                                       CreatedBy = a.CreatedBy,
                                       CreatedDate = a.CreatedDate,
                                       LastModifiedBy = a.LastModifiedBy,
                                       LastModifiedDate = a.LastModifiedDate,
                                       AttestationSnapshot = a.AttestationSnapshot,
                                       RemediationStatus = a.RemediationStatus,
                                   }).ToList();

            reviewerSnapshot = (from actionDetail in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                                join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on actionDetail.AttestationSnapshotId equals snapShot.Id
                                join hist in query on actionDetail.Id equals hist.AttestationSnapshotActionId
                                select new
                                {
                                    AttestationSnapshot = snapShot,
                                    Id = actionDetail.Id,
                                    AttestationSnapshotId = actionDetail.AttestationSnapshotId,
                                    TaskId = actionDetail.TaskId,
                                    Approve = actionDetail.Approve,
                                    Revoke = actionDetail.Revoke,
                                    Comment = actionDetail.Comment,
                                    EffectiveDate = actionDetail.EffectiveDate,
                                    ReviewerId = actionDetail.ReviewerId,
                                    Expired = actionDetail.Expired,
                                    IsChanged = actionDetail.IsChanged,
                                    IsChangedInPreviousLevel = actionDetail.IsChangedInPreviousLevel,
                                    IsDelta = actionDetail.IsDelta,
                                    IsVisible = actionDetail.IsVisible,
                                    IsActionable = actionDetail.IsActionable,
                                    IsActionExtendable = actionDetail.IsActionExtendable,
                                    IsException = actionDetail.IsException,
                                    Reserved1 = actionDetail.Reserved1,
                                    Reserved2 = actionDetail.Reserved2,
                                    Reserved3 = actionDetail.Reserved3,
                                    CreatedBy = actionDetail.CreatedBy,
                                    CreatedDate = actionDetail.CreatedDate,
                                    LastModifiedBy = actionDetail.LastModifiedBy,
                                    LastModifiedDate = actionDetail.LastModifiedDate,
                                    RemediationStatus = actionDetail.RemediationStatus,
                                    IsReviewerGroup = actionDetail.IsReviewerGroup,
                                    RootSnapshotId = actionDetail.RootSnapshotId,
                                }).AsEnumerable().Select(actionDetail => new AttestationSnapshotAction()
                                {
                                    AttestationSnapshot = actionDetail.AttestationSnapshot,
                                    Id = actionDetail.Id,
                                    AttestationSnapshotId = actionDetail.AttestationSnapshotId,
                                    TaskId = actionDetail.TaskId,
                                    Approve = actionDetail.Approve,
                                    Revoke = actionDetail.Revoke,
                                    Comment = actionDetail.Comment,
                                    EffectiveDate = actionDetail.EffectiveDate,
                                    ReviewerId = actionDetail.ReviewerId,
                                    Expired = actionDetail.Expired,
                                    IsChanged = actionDetail.IsChanged,
                                    IsChangedInPreviousLevel = actionDetail.IsChangedInPreviousLevel,
                                    IsDelta = actionDetail.IsDelta,
                                    IsVisible = actionDetail.IsVisible,
                                    IsActionable = actionDetail.IsActionable,
                                    IsActionExtendable = actionDetail.IsActionExtendable,
                                    IsException = actionDetail.IsException,
                                    Reserved1 = actionDetail.Reserved1,
                                    Reserved2 = actionDetail.Reserved2,
                                    Reserved3 = actionDetail.Reserved3,
                                    CreatedBy = actionDetail.CreatedBy,
                                    CreatedDate = actionDetail.CreatedDate,
                                    LastModifiedBy = actionDetail.LastModifiedBy,
                                    LastModifiedDate = actionDetail.LastModifiedDate,
                                    RemediationStatus = actionDetail.RemediationStatus,
                                    IsReviewerGroup = actionDetail.IsReviewerGroup,
                                    RootSnapshotId = actionDetail.RootSnapshotId,
                                }).ToList();
            if (snapshotForCampaign.Count == reviewerSnapshot.Count)
            {
                return reviewerSnapshot;
            }
            long levelLoaded = 1;
            List<long> actionIds = new List<long>();
            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();

            var maxLevel = (from snapshot in CurrentContext.AttestationSnapshots.AsNoTracking()
                            where snapshot.AttestationCampaignId == campaignId
                            select snapshot.Level).Max();

            var availableEndpoints = reviewerSnapshot.Select(a => a.AttestationSnapshot.EndpointId).Distinct().ToList();
            var levelSnapshot = new List<AttestationSnapshotAction>();

            for (long level = levelLoaded; level <= maxLevel; level++)
            {
                var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                if (previousEntity.Count > 500)
                {
                    for (int i = 0; i < previousEntity.Count; i = i + 500)
                    {
                        levelSnapshot = snapshotForCampaign.Where(a => !actionIds.Contains(a.Id)
                                 && previousEntity.Skip(i).Take(500).Contains(a.AttestationSnapshot.ParentEntity)
                                 && availableEndpoints.Contains(a.AttestationSnapshot.EndpointId)
                                 ).ToList();
                        if (levelSnapshot.Count > 0)
                        {
                            reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
                        }
                    }
                }
                else
                {
                    levelSnapshot = snapshotForCampaign.Where(a => !actionIds.Contains(a.Id)
                                 && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)
                                 && availableEndpoints.Contains(a.AttestationSnapshot.EndpointId)
                                 ).ToList();
                    if (levelSnapshot.Count > 0)
                    {
                        reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                        actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
                    }
                }
                //levelSnapshot = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking().Include("AttestationSnapshot")
                //                     where action.AttestationSnapshot.AttestationCampaignId == campaignId
                //                     && !actionIds.Contains(action.Id)
                //                     // Taking User level only
                //                     && previousEntity.Contains(action.AttestationSnapshot.ParentEntity)
                //                     && availableEndpoints.Contains(action.AttestationSnapshot.EndpointId)
                //                     select action).ToList();
                //if(levelSnapshot.Count > 0)
                //{
                //    reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                //    actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
                //}
            }

            return reviewerSnapshot;
        }

        private List<AttestationSnapshotAction> ConvertFromPresentation(List<PresentationAttestationSnapshotActionDetails> actionDetails)
        {
            List<AttestationSnapshotAction> attestationSnapshotActions = new List<AttestationSnapshotAction>();
            foreach (var actionDetail in actionDetails)
            {
                attestationSnapshotActions.Add(new AttestationSnapshotAction()
                {
                    AttestationSnapshot = actionDetail.AttestationSnapshot,
                    Id = actionDetail.Id,
                    AttestationSnapshotId = actionDetail.AttestationSnapshotId,
                    TaskId = actionDetail.TaskId,
                    Approve = actionDetail.Approve,
                    Revoke = actionDetail.Revoke,
                    Comment = actionDetail.Comment,
                    EffectiveDate = actionDetail.EffectiveDate,
                    ReviewerId = actionDetail.ReviewerId,
                    Expired = actionDetail.Expired,
                    IsChanged = actionDetail.IsChanged,
                    IsChangedInPreviousLevel = actionDetail.IsChangedInPreviousLevel,
                    IsDelta = actionDetail.IsDelta,
                    IsVisible = actionDetail.IsVisible,
                    IsActionable = actionDetail.IsActionable,
                    IsActionExtendable = actionDetail.IsActionExtendable,
                    IsException = actionDetail.IsException,
                    Reserved1 = actionDetail.Reserved1,
                    Reserved2 = actionDetail.Reserved2,
                    Reserved3 = actionDetail.Reserved3,
                    CreatedBy = actionDetail.CreatedBy,
                    CreatedDate = actionDetail.CreatedDate,
                    LastModifiedBy = actionDetail.LastModifiedBy,
                    LastModifiedDate = actionDetail.LastModifiedDate,
                    RemediationStatus = actionDetail.RemediationStatus,
                    IsReviewerGroup = actionDetail.IsReviewerGroup,
                    RootSnapshotId = actionDetail.RootSnapshotId,

                });
            }
            return attestationSnapshotActions;
        }

        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetails(long taskId, long campaignId, long level)
        {
            List<AttestationSnapshotAction> snapshotForCampaign = new List<AttestationSnapshotAction>();
            snapshotForCampaign = (from action in CurrentContext.AttestationSnapshotActions.AsNoTracking().Include("AttestationSnapshot")
                                   join interim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking()
                                   on action.Id equals interim.AttestationSnapshotActionId
                                   where action.AttestationSnapshot.AttestationCampaignId == campaignId
                                   select new
                                   {
                                       Id = action.Id,
                                       AttestationSnapshotId = action.AttestationSnapshotId,
                                       TaskId = interim.TaskId,
                                       Approve = interim.UnSavedApprove,
                                       Revoke = interim.UnSavedRevoke,
                                       Comment = interim.UnSavedComment,
                                       EffectiveDate = interim.UnSavedEffectiveDate,
                                       ReviewerId = interim.ReviewerId,
                                       Expired = action.Expired,
                                       IsChanged = interim.UnSavedIsChanged,
                                       IsChangedInPreviousLevel = action.IsChangedInPreviousLevel,
                                       IsDelta = action.IsDelta,
                                       IsVisible = action.IsVisible,
                                       IsActionable = action.IsActionable,
                                       IsActionExtendable = action.IsActionExtendable,
                                       IsException = action.IsException,
                                       Reserved1 = interim.UnSavedReserved1,
                                       Reserved2 = interim.UnSavedReserved2,
                                       Reserved3 = interim.UnSavedReserved3,
                                       CreatedBy = action.CreatedBy,
                                       CreatedDate = action.CreatedDate,
                                       LastModifiedBy = interim.LastModifiedBy,
                                       LastModifiedDate = interim.LastModifiedDate,
                                       AttestationSnapshot = action.AttestationSnapshot,
                                       RemediationStatus = action.RemediationStatus,
                                   }).AsEnumerable().Select(a => new AttestationSnapshotAction
                                   {
                                       Id = a.Id,
                                       AttestationSnapshotId = a.AttestationSnapshotId,
                                       TaskId = a.TaskId,
                                       Approve = a.Approve,
                                       Revoke = a.Revoke,
                                       Comment = a.Comment,
                                       EffectiveDate = a.EffectiveDate,
                                       ReviewerId = a.ReviewerId,
                                       Expired = a.Expired,
                                       IsChanged = a.IsChanged,
                                       IsChangedInPreviousLevel = a.IsChangedInPreviousLevel,
                                       IsDelta = a.IsDelta,
                                       IsVisible = a.IsVisible,
                                       IsActionable = a.IsActionable,
                                       IsActionExtendable = a.IsActionExtendable,
                                       IsException = a.IsException,
                                       Reserved1 = a.Reserved1,
                                       Reserved2 = a.Reserved2,
                                       Reserved3 = a.Reserved3,
                                       CreatedBy = a.CreatedBy,
                                       CreatedDate = a.CreatedDate,
                                       LastModifiedBy = a.LastModifiedBy,
                                       LastModifiedDate = a.LastModifiedDate,
                                       AttestationSnapshot = a.AttestationSnapshot,
                                       RemediationStatus = a.RemediationStatus,
                                   }).ToList();

            List<AttestationSnapshotAction> reviewerSnapshot = new List<AttestationSnapshotAction>();
            reviewerSnapshot = snapshotForCampaign.Where(a =>
                                // Taking User level only
                                a.TaskId == taskId).ToList();

            List<long> actionIds = new List<long>();
            actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            long levelLoaded = 1;
            if (reviewerSnapshot != null && reviewerSnapshot.Count > 0)
            {
                levelLoaded = reviewerSnapshot.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
            }

            var maxLevel = snapshotForCampaign.Select(a => a.AttestationSnapshot.Level).Distinct().Max();

            maxLevel = Math.Min(level, maxLevel);

            for (long lvl = levelLoaded + 1; lvl <= maxLevel; lvl++)
            {
                var previousEntity = reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == lvl - 1).Select(a => a.AttestationSnapshot.Entity).Distinct().ToList();
                var levelSnapshot = snapshotForCampaign.Where(a =>
                                     // Taking User level only
                                     !actionIds.Contains(a.Id)
                                     && previousEntity.Contains(a.AttestationSnapshot.ParentEntity)).ToList();

                reviewerSnapshot = reviewerSnapshot.Union(levelSnapshot).ToList();
                actionIds = reviewerSnapshot.Select(a => a.Id).ToList();
            }

            return reviewerSnapshot.Where(a => a.AttestationSnapshot.Level == level).ToList();
        }
        public List<AttestationSnapshotAction> GetAttestationSnapshotActionDetails(long rootSnapId, string accountName, long endpointSystemId, long campaignId)
        {
            //EndPoint dependentEP = null;
            //Infrastructure.Enums.ApplicationType applicationType = Infrastructure.Enums.ApplicationType.None;
            //var ep = CurrentContext.EndPoint.FirstOrDefault(i=> i.Id == endpointSystemId);
            //if(ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_CRM || ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_ISU)
            //{
            //    applicationType = ep.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_CRM ? Infrastructure.Enums.ApplicationType.MSSL_ISU : Infrastructure.Enums.ApplicationType.MSSL_CRM;
            //}
            //else if (ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_CRM || ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_ISU)
            //{
            //    applicationType = ep.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_CRM ? Infrastructure.Enums.ApplicationType.EBS_ISU : Infrastructure.Enums.ApplicationType.EBS_CRM;
            //}

            //dependentEP = CurrentContext.EndPoint.FirstOrDefault(i => i.ApplicationType == applicationType);

            return (from snp in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                    join attestSnap in CurrentContext.AttestationSnapshots.AsNoTracking() on snp.AttestationSnapshotId equals attestSnap.Id
                    join intrim in CurrentContext.AttestationSnapshotActionInterims.AsNoTracking() on snp.Id equals intrim.AttestationSnapshotActionId
                    where attestSnap.AttestationCampaignId == campaignId && snp.RootSnapshotId == rootSnapId && attestSnap.EntitlementType == Infrastructure.Enums.EntitlementType.Position
                    select new
                    {
                        Id = snp.Id,
                        Revoke = intrim.UnSavedRevoke,
                        Approve = intrim.UnSavedApprove,
                        RootSnapshotId = snp.RootSnapshotId,
                        IsActionable = snp.IsActionable,
                        IsActionExtendable = snp.IsActionExtendable,
                        AttestationSnapshotId = snp.AttestationSnapshotId,
                        IsDelta = snp.IsDelta,
                        attestSnap.Entity
                    }).ToList().Select(i => new AttestationSnapshotAction
                    {
                        Id = i.Id,
                        Revoke = i.Revoke,
                        Approve = i.Approve,
                        RootSnapshotId = i.RootSnapshotId,
                        IsActionable = i.IsActionable,
                        IsActionExtendable = i.IsActionExtendable,
                        AttestationSnapshotId = i.AttestationSnapshotId,
                        IsDelta = i.IsDelta,
                        Reserved3 = i.Entity
                    }).ToList();
        }

        public AttestationSnapshotAction GetDependentAttestationSnapshotAction(string accountName, long endpointSystemId, long campaignId, string position, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Account)
            {
                return (from snapAction in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                        join snap in CurrentContext.AttestationSnapshots.AsNoTracking() on snapAction.AttestationSnapshotId equals snap.Id
                        where snap.AttestationCampaignId == campaignId && snap.AccountName == accountName && snap.EndpointId == endpointSystemId
                        select snapAction).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Position)
            {
                return (from snapAction in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                        join snap in CurrentContext.AttestationSnapshots.AsNoTracking() on snapAction.AttestationSnapshotId equals snap.Id
                        where snap.AttestationCampaignId == campaignId && snap.AccountName == accountName && snap.Entity == position && snap.EndpointId == endpointSystemId
                        select snapAction).FirstOrDefault();
            }
            return null;
        }

        public IList<AttestationSnapshotAction> GetByReviewerId(long reviewerId, bool isGroupTask, long campaignId)
        {
            return (from snapAction in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                    join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on snapAction.AttestationSnapshotId equals snapShot.Id
                    where snapShot.AttestationCampaignId == campaignId && snapAction.ReviewerId == reviewerId && snapAction.IsReviewerGroup == isGroupTask
                    select snapAction
                    ).ToList();
        }

        public bool IsSnapshotActionExistsForCampaign(long campaignId, int level = 1)
        {
            return (from snapAction in CurrentContext.AttestationSnapshotActions.AsNoTracking()
                    join snapShot in CurrentContext.AttestationSnapshots.AsNoTracking() on snapAction.AttestationSnapshotId equals snapShot.Id
                    where snapShot.AttestationCampaignId == campaignId && (level < 1 || snapAction.AttestationSnapshot.Level == level)
                    select snapAction).Any();
        }

        public PresentationGridPagedData<EntityModel.ViewUserEntitlementData> GetAttestationUserComparisionData(long taskId, int draw, int start, int length, List<long> endpointIds, AccountType accountType, string search = "")
        {
            CurrentContext.Database.CommandTimeout = 300;
            var isPending = CurrentContext.Tasks.FirstOrDefault(i => i.Id == taskId).Action == StatusType.Pending;
            var searchData = search != null && !string.IsNullOrWhiteSpace(search) ? search.Split(new char[] { ',' }).ToList() : new List<string>();
            PresentationGridPagedData<EntityModel.ViewUserEntitlementData> dataResult = new PresentationGridPagedData<EntityModel.ViewUserEntitlementData>();
            var employeeQueryParameters = new object[]
            {
                new System.Data.SqlClient.SqlParameter("@TaskId", taskId),
                new System.Data.SqlClient.SqlParameter("@AccountType", (int)accountType)
            };
            var distinctEmployees = CurrentContext.Database.SqlQuery<FilterData>("EXEC [DI_USP_GetAttestationEmployeeByTaskid] @TaskId,NULL,@AccountType", employeeQueryParameters).ToList();
            var onlyEmployee = distinctEmployees.GroupBy(d => $"{d.AdditionalInfo}_{d.Name.ToUpper()}_{d.UserName.ToUpper()}").ToDictionary(i => i.Key, v => v.ToList()).ToList();
            var totalCount = 0;
            var result = new List<ViewUserEntitlementData>();
            long rIndex = start;

            if (isPending)
            {
                var totalCountQ = (from Ent in CurrentContext.AttestationTaskEntitlementUserComparisions
                                   where Ent.TaskId == taskId && (searchData.Count > 0 ? searchData.Contains(Ent.Entitlement) : true)
                                   && Ent.AccountType == accountType
                                   select new { Ent.Entitlement, Ent.EndpointSystemId, Ent.EntitlementType, Ent.EntitlementDescription }
                    );
                if (endpointIds != null && endpointIds.Count > 0)
                {
                    totalCount = totalCountQ.Where(k => endpointIds.Contains(k.EndpointSystemId)).Distinct().Count();
                }
                else
                {
                    totalCount = totalCountQ.Distinct().Count();
                }
                if (length == 0)
                {
                    length = totalCount;
                }
                var query = (from Ent in CurrentContext.AttestationTaskEntitlementUserComparisions
                             where Ent.TaskId == taskId && (searchData.Count > 0 ? searchData.Contains(Ent.Entitlement) : true)
                             && Ent.AccountType == accountType
                             select Ent);
                if (endpointIds != null && endpointIds.Count > 0)
                {
                    query = query.Where(ep => endpointIds.Contains(ep.EndpointSystemId));
                }
                var data = (from Ent in query
                            select new EntityModel.PresentationUserEntitlementData
                            {
                                EmployeeId = Ent.EmployeeNo,
                                EmployeeName = Ent.Name,
                                EndpointId = Ent.EndpointSystemId,
                                Entitlement = Ent.Entitlement,
                                EntitlementDescription = Ent.EntitlementDescription,
                                EntitlementType = Ent.EntitlementType,
                                SystemName = Ent.EndpointSystemName,
                                UserId = Ent.UserId,
                                Approved = Ent.Approve,
                                Revoked = Ent.Revoke,
                                UserName = Ent.UserName,
                                IsActionable = Ent.IsActionable
                            }).GroupBy(item => new { item.Entitlement, item.EntitlementType, item.EntitlementDescription, item.SystemName, item.EndpointId }, (key, group) => new
                            {
                                Key = key,
                                Group = group.AsEnumerable()
                            })
                        .OrderBy(col => col.Key.SystemName).ThenBy(col => col.Key.EntitlementType).ThenBy(col => col.Key.Entitlement)
                        .AsEnumerable()
                        .Skip(start).Take(length)
                        .ToDictionary(item => item.Key, item => item.Group);

                foreach (var item in data)
                {
                    rIndex = rIndex + 1;
                    var dRecord = new ViewUserEntitlementData();
                    dRecord.rowID = rIndex;
                    dRecord.Entitlement = item.Key.Entitlement;
                    dRecord.EntitlementDescription = item.Key.EntitlementDescription;
                    dRecord.EntitlementType = item.Key.EntitlementType.ToString();
                    dRecord.SystemName = item.Key.SystemName;
                    dRecord.EndpointId = item.Key.EndpointId;
                    dRecord.EmployeeNames = new Dictionary<string, KeyValueData>();
                    foreach (var emp in item.Value.OrderBy(ep => $"{ep.EmployeeName.ToUpper()}_{ep.EmployeeId}_{ep.UserName?.ToUpper()}"))
                    {
                        var empV = new KeyValueData()
                        {
                            AccountId = emp.UserId != null ? emp.UserId.Value : 0,
                            Approve = emp.Approved,
                            Revoke = emp.Revoked,
                            EmployeeName = emp.EmployeeName,
                            EmployeeNo = emp.EmployeeId,
                            UserName = emp.UserName.ToUpper(),
                            Exists = true,
                            IsActionable = emp.IsActionable
                        };
                        if (dRecord.EmployeeNames.ContainsKey($"{emp.EmployeeName.ToUpper()}_{emp.EmployeeId}_{emp.UserName.ToUpper()}") == false)
                            dRecord.EmployeeNames.Add($"{emp.EmployeeName.ToUpper()}_{emp.EmployeeId}_{emp.UserName.ToUpper()}", empV);
                    }
                    result.Add(dRecord);
                }
            }
            else
            {
                var totalCountQ = (from Ent in CurrentContext.AttestationTaskHistoryEntitlementUserComparisions
                                   where Ent.TaskId == taskId && (searchData.Count > 0 ? searchData.Contains(Ent.Entitlement) : true)
                                   && Ent.AccountType == accountType
                                   select new { Ent.Entitlement, Ent.EndpointSystemId, Ent.EntitlementType, Ent.EntitlementDescription }
                    );
                if (endpointIds != null && endpointIds.Count > 0)
                {
                    totalCount = totalCountQ.Where(k => endpointIds.Contains(k.EndpointSystemId)).Distinct().Count();
                }
                else
                {
                    totalCount = totalCountQ.Distinct().Count();
                }
                if (length == 0)
                {
                    length = totalCount;
                }
                var query = (from Ent in CurrentContext.AttestationTaskHistoryEntitlementUserComparisions
                             where Ent.TaskId == taskId && (searchData.Count > 0 ? searchData.Contains(Ent.Entitlement) : true)
                             && Ent.AccountType == accountType
                             select Ent);
                if (endpointIds != null && endpointIds.Count > 0)
                {
                    query = query.Where(ep => endpointIds.Contains(ep.EndpointSystemId));
                }

                var data = (from Ent in query
                            select new EntityModel.PresentationUserEntitlementData
                            {
                                EmployeeId = Ent.EmployeeNo,
                                EmployeeName = Ent.Name,
                                EndpointId = Ent.EndpointSystemId,
                                Entitlement = Ent.Entitlement,
                                EntitlementDescription = Ent.EntitlementDescription,
                                EntitlementType = Ent.EntitlementType,
                                SystemName = Ent.EndpointSystemName,
                                UserId = Ent.UserId,
                                Approved = Ent.Approve,
                                Revoked = Ent.Revoke,
                                UserName = Ent.UserName,
                                IsActionable = Ent.IsActionable,
                            }).GroupBy(item => new { item.Entitlement, item.EntitlementType, item.EntitlementDescription, item.SystemName, item.EndpointId }, (key, group) => new
                            {
                                Key = key,
                                Group = group.AsEnumerable()
                            })
                        .OrderBy(col => col.Key.EndpointId).ThenBy(col => col.Key.EntitlementType).ThenBy(col => col.Key.Entitlement)
                        .AsEnumerable()
                        .Skip(start).Take(length)
                        .ToDictionary(item => item.Key, item => item.Group);
                foreach (var item in data)
                {
                    rIndex = rIndex + 1;
                    var dRecord = new ViewUserEntitlementData();
                    dRecord.rowID = rIndex;
                    dRecord.Entitlement = item.Key.Entitlement;
                    dRecord.EntitlementDescription = item.Key.EntitlementDescription;
                    dRecord.EntitlementType = item.Key.EntitlementType.ToString();
                    dRecord.SystemName = item.Key.SystemName;
                    dRecord.EndpointId = item.Key.EndpointId;
                    dRecord.EmployeeNames = new Dictionary<string, KeyValueData>();
                    foreach (var emp in item.Value.OrderBy(ep => $"{ep.SystemName.ToUpper()}_{ep.EmployeeId}_{ep.UserName.ToUpper()}"))
                    {
                        var exist = distinctEmployees.FirstOrDefault(eI => eI.EndpointId == emp.EndpointId && eI.AdditionalInfo == emp.EmployeeId && eI.UserName == emp.UserName);
                        var empV = new KeyValueData()
                        {
                            AccountId = emp.UserId != null ? emp.UserId.Value : 0,
                            Approve = emp.Approved,
                            Revoke = emp.Revoked,
                            EmployeeName = emp.EmployeeName,
                            EmployeeNo = emp.EmployeeId,
                            UserName = emp.UserName.ToUpper(),
                            Exists = exist != null,
                            IsActionable = emp.IsActionable
                        };
                        if (dRecord.EmployeeNames.ContainsKey($"{emp.EmployeeName.ToUpper()}_{emp.EmployeeId}_{emp.UserName.ToUpper()}") == false)
                            dRecord.EmployeeNames.Add($"{emp.EmployeeName.ToUpper()}_{emp.EmployeeId}_{emp.UserName.ToUpper()}", empV);
                    }
                    result.Add(dRecord);
                }
            }
            dataResult.Data = result;
            dataResult.TotalCount = totalCount;
            return dataResult;
        }
        public List<FilterData> GetFilterDataByType(long taskId, int filterType = 1, List<long> epIds = null, List<long> userIds = null, AccountType accountType = AccountType.User)
        {
            var epExists = epIds != null && epIds.Count > 0;
            var userIdsExists = userIds != null && userIds.Count > 0;
            if (userIdsExists == false) userIds = new List<long>();
            if (epExists == false) epIds = new List<long>();
            var isPending = CurrentContext.Tasks.FirstOrDefault(i => i.Id == taskId).Action == StatusType.Pending;
            //if (filterType == 1)//Employee -- Not Required
            //{

            //    return (from sH in CurrentContext.AttestationTaskEntitlementUserComparisions
            //            where sH.TaskId == taskId && (epExists ? epIds.Contains(sH.EndpointSystemId) : true)
            //            select new FilterData
            //            {
            //                Name = sH.Name,
            //                Id = sH.UserId
            //            }).Distinct().OrderBy(i => i.Name).ToList();
            //}
            //else if (filterType == 2)//Entitlement
            if (filterType == 2)//Entitlement
            {
                List<FilterData> filterDatas = new List<FilterData>();
                var query = (from sH in CurrentContext.AttestationTaskEntitlementUserComparisions
                             where sH.TaskId == taskId
                               && sH.AccountType == accountType
                         && (epExists ? epIds.Contains(sH.EndpointSystemId) : true)
                         && (userIdsExists ? userIds.Contains(sH.UserId) : true)
                             select new
                             {
                                 sH.Entitlement,
                                 sH.EntitlementType,
                                 sH.EndpointSystemName,
                                 sH.EndpointSystemId
                             });
                if (isPending == false)
                {
                    query = (from sH in CurrentContext.AttestationTaskHistoryEntitlementUserComparisions
                             where sH.TaskId == taskId
                               && sH.AccountType == accountType
                         && (epExists ? epIds.Contains(sH.EndpointSystemId) : true)
                         && (userIdsExists ? userIds.Contains(sH.UserId) : true)
                             select new
                             {
                                 sH.Entitlement,
                                 sH.EntitlementType,
                                 sH.EndpointSystemName,
                                 sH.EndpointSystemId
                             });
                }
                var entitlements = query.Distinct().OrderBy(i => i.EndpointSystemName).ThenBy(i => i.EntitlementType).ThenBy(i => i.Entitlement);
                foreach (var entitlement in entitlements)
                {
                    filterDatas.Add(new FilterData()
                    {
                        Name = entitlement.Entitlement,
                        AdditionalInfo = entitlement.Entitlement,
                        EndpointName = entitlement.EndpointSystemName,
                        EndpointId = entitlement.EndpointSystemId,
                        EntitlementType = entitlement.EntitlementType.ToString()
                    });
                }
                return filterDatas;
            }
            else
            {
                //Endpoint
                return (from sH in CurrentContext.AttestationTaskEntitlementUserComparisions
                        where sH.TaskId == taskId
                        && sH.AccountType == accountType
                        && (epExists ? epIds.Contains(sH.EndpointSystemId) : true)
                        && (userIdsExists == true ? userIds.Contains(sH.UserId) : true)
                        select new FilterData
                        {
                            Name = sH.EndpointSystemName,
                            Id = sH.EndpointSystemId
                        }).Distinct().OrderBy(i => i.Name).ToList();
            }
        }

        public Dictionary<string, List<FilterData>> GetEmployeeInformation(long taskId, AccountType accountType, List<long> epIds)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                var sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@taskId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = taskId;

                var sqlParamEp = new SqlParameter();
                sqlParamEp.ParameterName = "@endpointSystemIds";
                sqlParamEp.SqlDbType = SqlDbType.Xml;

                if (epIds != null && epIds.Count > 0)
                {
                    var xmlData = UtilityMethods.ObjectToXMLGeneric(epIds);
                    sqlParamEp.Value = xmlData;
                }
                else
                {
                    sqlParamEp.Value = DBNull.Value;
                }

                var sqlParamAcType = new SqlParameter();
                sqlParamAcType.ParameterName = "@accountType";
                sqlParamAcType.SqlDbType = SqlDbType.TinyInt;
                sqlParamAcType.Value = (int)accountType;
                object[] param = { sqlParam, sqlParamEp, sqlParamAcType };
                var distinctEmployees = CurrentContext.Database.SqlQuery<FilterData>($"EXEC [DI_USP_GetAttestationEmployeeByTaskid] @taskId,@endpointSystemIds,@accountType", param).ToList();
                var re = new Dictionary<string, List<FilterData>>();

                if (distinctEmployees != null)
                    return distinctEmployees.GroupBy(d => $"{d.Name.ToUpper()}_{d.AdditionalInfo}").ToDictionary(i => i.Key, v => v.ToList());
                return re;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<AccountType> GetAccountTypes(long taskId)
        {
            List<AccountType> accountTypes = new List<AccountType>();
            try
            {
                var isPending = CurrentContext.Tasks.FirstOrDefault(i => i.Id == taskId).Action == StatusType.Pending;
                if (isPending)
                {

                    accountTypes = (from sH in CurrentContext.AttestationSnapshotActions
                                    join snap in CurrentContext.AttestationSnapshots on sH.AttestationSnapshotId equals snap.Id
                                    where sH.TaskId == taskId
                                    select snap.AccountType).Distinct().ToList();
                }
                else
                {
                    accountTypes = (from sH in CurrentContext.AttestationSnapshotActionHistories
                                    join snapA in CurrentContext.AttestationSnapshotActions on sH.AttestationSnapshotActionId equals snapA.Id
                                    join snap in CurrentContext.AttestationSnapshots on snapA.AttestationSnapshotId equals snap.Id
                                    where sH.TaskId == taskId
                                    select snap.AccountType).Distinct().ToList();
                }
            }
            catch (Exception)
            {
                return accountTypes;
            }
            return accountTypes;
        }

        public List<FilterData> GetAccountStatusInformation(long taskId, AccountType accountType, long userId, string accountName)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                var sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@taskId";
                sqlParam.SqlDbType = SqlDbType.BigInt;
                sqlParam.Value = taskId;

                var sqlParamEp = new SqlParameter();
                sqlParamEp.ParameterName = "@endpointSystemIds";
                sqlParamEp.SqlDbType = SqlDbType.Xml;


                var sqlParamAcType = new SqlParameter();
                sqlParamAcType.ParameterName = "@accountType";
                sqlParamAcType.SqlDbType = SqlDbType.TinyInt;
                sqlParamAcType.Value = (int)accountType;

                var sqlParamAccName = new SqlParameter();
                sqlParamAccName.ParameterName = "@accountname";
                sqlParamAccName.SqlDbType = SqlDbType.NVarChar;
                sqlParamAccName.Size = 1024;
                sqlParamAccName.Value = DBNull.Value;

                if (!string.IsNullOrWhiteSpace(accountName))
                    sqlParamAccName.Value = accountName;


                var sqlParamUserId = new SqlParameter();
                sqlParamUserId.ParameterName = "@userId";
                sqlParamUserId.SqlDbType = SqlDbType.BigInt;
                sqlParamUserId.Value = userId;


                object[] param = { sqlParam, sqlParamAcType, sqlParamUserId, sqlParamAccName };
                var distinctEmployees = CurrentContext.Database.SqlQuery<FilterData>($"EXEC USP_GETACCOUNTSTATUSBYTASKID @taskId,@accountType,@userId,@accountname", param).ToList();
                return distinctEmployees.OrderBy(i => i.EndpointName).ThenBy(i => i.UserName).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttestationTaskEntitlementUserComparision> GetAttestationTaskEntitlementUserComparision(long taskId)
        {
            var list = (from attEntUserComp in CurrentContext.AttestationTaskEntitlementUserComparisions
                         where attEntUserComp.TaskId == taskId
                         select attEntUserComp).ToList();

            return list;
        }
        public List<AttestationCampaignEntitlementUserComparision> GetAttestationTaskEntitlementUserComparisionByCampaignId(int pageNumber, int pageSize, long campaignId)
        {
            var list = (from snap in CurrentContext.AttestationSnapshots 
                        join snapAction in CurrentContext.AttestationSnapshotActions on snap.Id equals snapAction.AttestationSnapshotId
                        where snap.AttestationCampaignId == campaignId && snap.Level == 2
                        select new AttestationCampaignEntitlementUserComparision
                        {
                            SnapshotActionId = snapAction.Id,
                            EndpointSystemName = snap.EndpointName,
                            EndpointType = snap.EndpointType,
                            AccountType = snap.AccountType,
                            Entitlement = snap.Name,
                            EntitlementDescription = snap.Description,
                            UserName = snap.AccountName,
                            Name = snap.DIUserDisplayName,
                            Approve = snapAction.Approve,
                            Revoke = snapAction.Revoke
                        })
                        .OrderBy(i=>i.EndpointSystemName).ThenBy(i=>i.Name).ThenBy(i=>i.AccountType).ThenBy(i=>i.Entitlement)
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
    }
}

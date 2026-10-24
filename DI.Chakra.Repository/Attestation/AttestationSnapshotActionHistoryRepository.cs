using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class AttestationSnapshotActionHistoryRepository : RepositoryBase<AttestationSnapshotActionHistory>
    {
        public AttestationSnapshotActionHistoryRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<AttestationSnapshotActionHistory> GetAttestationActionHistory(long attestationActionId, bool loadAllHistroy, long versionTillToLoad)
        {
            List<AttestationSnapshotActionHistory> reviewerHistory = new List<AttestationSnapshotActionHistory>();
            try
            {
                reviewerHistory = (from actionHistory in CurrentContext.AttestationSnapshotActionHistories
                                   join user in CurrentContext.User on actionHistory.ReviewerId equals user.Id
                                   where actionHistory.AttestationSnapshotActionId == attestationActionId
                                   orderby actionHistory.Version descending
                                   select new
                                   {
                                       Version = actionHistory.Version,
                                       TaskId = actionHistory.TaskId,
                                       Approve = actionHistory.Approve,
                                       Revoke = actionHistory.Revoke,
                                       Comment = actionHistory.Comment,
                                       EffectiveDate = actionHistory.EffectiveDate,
                                       LastModifiedBy = user.DisplayName,
                                       LastModifiedDate = actionHistory.LastModifiedDate,
                                   }
                                   ).AsEnumerable().Select(x => new AttestationSnapshotActionHistory
                                   {
                                       Version = x.Version,
                                       TaskId = x.TaskId,
                                       Approve = x.Approve,
                                       Revoke = x.Revoke,
                                       Comment = x.Comment,
                                       EffectiveDate = x.EffectiveDate,
                                       LastModifiedBy = x.LastModifiedBy,
                                       LastModifiedDate = x.LastModifiedDate,
                                   }).ToList();

                if (!loadAllHistroy)
                {
                    reviewerHistory = reviewerHistory.Where(a => a.Version < versionTillToLoad).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return reviewerHistory;
        }

        public List<long> GetUserIdForTask(long taskId)
        {
            List<long> reviewerHistory = new List<long>();
            try
            {
                reviewerHistory = (from actionHistory in CurrentContext.AttestationSnapshotActionHistories
                                   where actionHistory.TaskId == taskId
                                   select new
                                   {
                                       actionHistory.UserId
                                   }
                                    ).AsEnumerable().Select(x => x.UserId).ToList();

            }
            catch (Exception ex)
            {

            }
            return reviewerHistory;
        }
        public string GetAttestationActionReviewComments(long attestationActionId)
        {
            var result = (from ah in CurrentContext.AttestationSnapshotActionHistories
                     join usr in CurrentContext.User on ah.LastModifiedBy equals usr.UserName
                     where ah.AttestationSnapshotActionId == attestationActionId
                     select new
                     {
                         ah.Version,
                         ah.LastModifiedDate,
                         ah.Comment,
                         usr.DisplayName,
                         usr.UserName
                     }).Distinct().OrderBy(i => i.Version).ToList();
            var retComments = string.Empty;
            if(result != null && result.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                result.ForEach(i => {
                    if(!string.IsNullOrWhiteSpace(i.Comment))
                    sb.AppendFormat("Reviewed Date:{0} Reviewed By:{1}({2}) Comments:{3}{4}", (i.LastModifiedDate != null ? i.LastModifiedDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : ""),
                        i.DisplayName, i.UserName, i.Comment??"",Environment.NewLine);
                });
                retComments = sb.ToString();
                if(!string.IsNullOrWhiteSpace(retComments) && retComments.Length > 1)
                {
                    retComments = retComments.Substring(0, retComments.Length - 1);
                }
            }
            return retComments;
        }

        public List<PresentationSnapshotActionHistory> GetHistoryByCampaignId(long campaignId)
        {
            var query = $@"select SA.TaskId,H.AttestationSnapshotActionId,H.LastModifiedDate as ReviewedDate,U.UserName,U.DisplayName,H.Version, T.DelegatedBy, T.DelegationComments FROM DI_M_AttestationCampaign C INNER JOIN  DI_M_AttestationSnapshot S on c.Id = S.AttestationCampaignId INNER JOIN  DI_M_AttestationSnapshotAction SA on S.Id = SA.AttestationSnapshotId  INNER JOIN  DI_M_AttestationSnapshotAction_History H on SA.Id = H.AttestationSnapshotActionId Left JOIN DI_M_User U on H.ReviewerId = U.Id
                INNER JOIN DI_M_Task T on H.TaskId = T.Id WHERE C.Id = {campaignId} order by H.AttestationSnapshotActionId,H.Version";
            return this.CurrentContext.Database.SqlQuery<PresentationSnapshotActionHistory>(query).ToList();
        }
    }
}

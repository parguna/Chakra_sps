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
    public class AttestationSnapshotActionHistory : DomainModelBase<AttestationSnapshotActionHistory, EntityModel.AttestationSnapshotActionHistory>
    {
        public long Version { get; set; }
        public long AttestationSnapshotActionId { get; set; }
        public long UserId { get; set; }
        public long TaskId { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public Nullable<long> ReviewerId { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static IList<AttestationSnapshotActionHistory> GetAttestationActionHistory(long attestationActionId, bool loadAllHistroy, long versionTillToLoad)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationSnapshotActionHistoryRepository(dataAccessTransaction)
                    .GetAttestationActionHistory(attestationActionId, loadAllHistroy, versionTillToLoad).ToList());

            }
        }

		public static List<AttestationSnapshotActionHistory> GetAttestationActionReviewHistory(long attestationActionId, bool loadAllHistroy, long versionTillToLoad)
        {
            List<AttestationSnapshotActionHistory> reviewComments = new List<AttestationSnapshotActionHistory>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var history = ORMapList(new AttestationSnapshotActionHistoryRepository(dataAccessTransaction)
                    .GetAttestationActionHistory(attestationActionId, true, 0).ToList());

                reviewComments = history.Where(a => !string.IsNullOrEmpty(a.Comment)).ToList();

                reviewComments.ForEach(a => { var fullMessage = a.Comment;
                    if(fullMessage.Contains("Message:"))
                    {
                        a.Comment = fullMessage.Split(new[] { "Message:" }, StringSplitOptions.None)[1].Replace(@"\r\n", "");
                    }
                });

                if (!loadAllHistroy)
                {
                    reviewComments = reviewComments.Where(a => a.Version < versionTillToLoad).ToList();
                }
            }
            return reviewComments;
        }

        public static IList<long> GetUserIdForTask(long taskId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return (new AttestationSnapshotActionHistoryRepository(dataAccessTransaction)
                    .GetUserIdForTask(taskId).ToList());

            }
        }
        public static string GetAttestationActionReviewComments(long attestationActionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotActionHistoryRepository(dataAccessTransaction)
                    .GetAttestationActionReviewComments(attestationActionId);
            }
        }

        
    }
}

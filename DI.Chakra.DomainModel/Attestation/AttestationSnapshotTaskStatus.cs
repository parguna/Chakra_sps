using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AttestationSnapshotTaskStatus : DomainModelBase<AttestationSnapshotTaskStatus, EntityModel.AttestationSnapshotTaskStatus>
    {
        public long AttestationId { get; set; }
        public long CampaignId { get; set; }
        public long TaskId { get; set; }
        public bool UnSavedChangesAvailable { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static AttestationSnapshotTaskStatus GetByTaskId(long TaskId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationSnapshotTaskStatus>(dataAccessTransaction).Get(a => a.TaskId == TaskId).FirstOrDefault());
            }
        }

    }
}

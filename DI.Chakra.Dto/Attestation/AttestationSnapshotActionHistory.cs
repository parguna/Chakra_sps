using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationSnapshotActionHistory : DtoBase
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

    }
}

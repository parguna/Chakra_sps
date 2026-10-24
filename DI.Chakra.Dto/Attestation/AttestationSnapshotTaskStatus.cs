using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationSnapshotTaskStatus : DtoBase
    {
        public long AttestationId { get; set; }
        public long CampaignId { get; set; }
        public long TaskId { get; set; }
        public bool UnSavedChangesAvailable { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}

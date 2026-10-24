using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationSnapshotActionInterim : DtoBase
    {
        public long AttestationSnapshotActionId { get; set; }
        public long TaskId { get; set; }
        public Nullable<long> ReviewerId { get; set; }
        public bool UnSavedApprove { get; set; }
        public bool UnSavedRevoke { get; set; }
        public string UnSavedComment { get; set; }
        public Nullable<DateTime> UnSavedEffectiveDate { get; set; }
        public bool UnSavedIsChanged { get; set; }
        public string UnSavedReserved1 { get; set; }
        public string UnSavedReserved2 { get; set; }
        public string UnSavedReserved3 { get; set; }
        public bool SavedApprove { get; set; }
        public bool SavedRevoke { get; set; }
        public string SavedComment { get; set; }
        public Nullable<DateTime> SavedEffectiveDate { get; set; }
        public bool SavedIsChanged { get; set; }
        public string SavedReserved1 { get; set; }
        public string SavedReserved2 { get; set; }
        public string SavedReserved3 { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public AttestationSnapshotAction AttestationSnapshotAction { get; set; }

    }
}

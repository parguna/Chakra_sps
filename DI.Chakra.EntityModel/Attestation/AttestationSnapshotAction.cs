using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationSnapshotAction")]
    public class AttestationSnapshotAction : EntityBase, ICreatable, IUpdatable
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

        [ForeignKey("AttestationSnapshotId")]
        public AttestationSnapshot AttestationSnapshot { get; set; }

        public AttestationRemediationStatus RemediationStatus { get; set; }

    }
}

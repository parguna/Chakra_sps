using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_RequestAttachment")]
    public class RequestAttachment : EntityBase,ICreatable,IUpdatable
    {
        public RequestType RequestType { get; set; }

        public string AttachmentNote { get; set; }

        public Nullable<long> AccessRequestId { get; set; }

        public Nullable<int> AttestationId { get; set; }

        public byte WorkflowStateId { get; set; }

        public byte[] Attachment { get; set; }

        public bool IsActive { get; set; }

        public long UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual User UploadedUser { get; set; }

        public virtual AccessRequest AccessRequest { get; set; }

        public virtual Attestation Attestation { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}

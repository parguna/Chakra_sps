using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_M_RequestAttachment")]
    [Serializable]
    public class RequestAttachment : IAggregateRoot
    {

        [Key]
        public long Id { get; set; }
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
    }

}

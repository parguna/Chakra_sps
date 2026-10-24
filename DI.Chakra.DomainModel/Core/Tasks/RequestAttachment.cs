using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class RequestAttachment : DomainModelBase<RequestAttachment, EntityModel.RequestAttachment>
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

        public User UploadedUser { get; set; }

        public AccessRequest AccessRequest { get; set; }

        public Attestation Attestation { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}

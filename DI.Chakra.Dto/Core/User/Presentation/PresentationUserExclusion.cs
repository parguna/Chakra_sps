using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationUserExclusion : DtoBase
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string EventType { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string FileName { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string ContentType { get; set; }
        public string Remarks { get; set; }
        public bool HasAttachments { get; set; }
        public string DisplayName { get; set; }
        public bool IsDeleted { get; set; }
    }
}

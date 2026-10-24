using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationInterimStore
    {
        public long Id { get; set; }
        public InterimStoreItemType InterimStoreItemType { get; set; }

        public long? TaskId { get; set; }

        public long? UserId { get; set; }

        public string XmlData { get; set; }

        public bool IsSaved { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public bool HasAttachment { get; set; }
        public string AttachmentFileName { get; set; }
    }
}

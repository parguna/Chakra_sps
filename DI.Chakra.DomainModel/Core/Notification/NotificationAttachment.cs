using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
   public class NotificationAttachment : DomainModelBase<NotificationAttachment, EntityModel.NotificationAttachment>
    {
        public long NotificationId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Attachment { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
    }
}

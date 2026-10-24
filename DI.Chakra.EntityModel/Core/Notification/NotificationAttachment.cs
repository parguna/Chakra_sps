using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NotificationAttachment")]
    public class NotificationAttachment :EntityBase, ICreatable, IUpdatable
    {
        public long NotificationId { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] Attachment { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }

    }
}

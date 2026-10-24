using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Notification")]
    public class Notification : EntityBase,ICreatable,IUpdatable
    {

        public Nullable<long> ParentId { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public long SenderId { get; set; }

        public long ReceiverId { get; set; }

        public long? SenderTaskId { get; set; }

        public long? ReceiverTaskId { get; set; }

        public long RequestId { get; set; }

        public bool IsRead { get; set; }

        public bool IsReplied { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        [ForeignKey("ReceiverId ")]
        public User Receiver { get; set; }

        public List<NotificationAttachment> NotificationAttachment { get; set; }
    }
}

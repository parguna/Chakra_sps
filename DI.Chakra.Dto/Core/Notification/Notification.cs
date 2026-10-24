using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class Notification : DtoBase
    {
        public long TaskId { get; set; }

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

        public List<NotificationAttachment> NotificationAttachment { get; set; }

        public User Sender { get; set; }

        public User Receiver { get; set; }

        public AccessRequest Request { get; set; }

        public bool HasAttachments { get; set; }
    }
}

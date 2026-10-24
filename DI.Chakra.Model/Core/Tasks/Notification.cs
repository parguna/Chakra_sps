using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_M_Notification")]
    [Serializable]
    public class Notification : IAggregateRoot
    {

        [Key]
        public long Id { get; set; }
        public Nullable<long> TaskId { get; set; }
        public Nullable<long> ParentId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public long SenderId { get; set; }
        public Nullable<long> ReceiverUserId { get; set; }
        public Nullable<long> ReceiverGroupId { get; set; }
        public bool IsRead{ get; set; }
        public bool IsReplied { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverUserId")]
        public virtual User ReceiveUser { get; set; }

        [ForeignKey("ReceiverGroupId")]
        public virtual Group ReceiveGroup { get; set; }

        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
    }

}

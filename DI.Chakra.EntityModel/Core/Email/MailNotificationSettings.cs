using System;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_MailNotificationSettings")]
    public class MailNotificationSettings : EntityBase, ICreatable, IUpdatable
    {
        public string TriggerName { get; set; }

        public bool MailTriggerFromJob { get; set; }

        public bool MailTriggerOnEffectiveDate { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        public string Cc { get; set; }

        public int PreConfigureNotificationDays { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}

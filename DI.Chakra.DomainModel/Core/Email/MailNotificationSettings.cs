using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.DomainModel
{
    public class MailNotificationSettings : DomainModelBase<MailNotificationSettings, EntityModel.MailNotificationSettings>
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

        public static MailNotificationSettings GetMailNotificationSettingsByTrigger(string TriggerName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.MailNotificationSettings>(dataAccessTransaction).Get(mail => mail.TriggerName == TriggerName).FirstOrDefault());
            }
        }
    }
}

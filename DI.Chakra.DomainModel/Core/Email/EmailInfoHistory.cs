using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class EmailInfoHistory : DomainModelBase<EmailInfoHistory, EntityModel.EmailInfoHistory>
    {
        public string ActivityType { get; set; }

        public string Priority { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public string Attachment { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? DispatchDateTime { get; set; }

        public DateTime? SentDateTime { get; set; }

        public int NoOfAttempts { get; set; }

        public string RecordStatus { get; set; }

        public static List<EmailInfoHistory> GetByActivityType(string activityType, string status)    
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmailInfoHistory>(dataAccessTransaction)
                      .Get(ei => (ei.Status == status || status == "") && (ei.ActivityType == activityType || activityType == "")).
                       OrderByDescending(item => item.SentDateTime).ToList()); 
            }
        }
    }
}

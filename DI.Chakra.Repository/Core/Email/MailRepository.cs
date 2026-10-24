using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class MailRepository : RepositoryBase<EmailInfo>
    {
        public MailRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<PresentationMail> GetAllEmailInfos()
        {
            return (from mail in CurrentContext.EmailInfos
                    select new PresentationMail
                    {
                        Id = mail.Id,
                        Subject = mail.Subject,
                        To = mail.To,
                        ActivityType = mail.ActivityType,
                        Status = mail.Status,
                        SentDateTime = mail.SentDateTime,
                    }).ToList();
        }

        public IList<PresentationMail> GetAllMailHistory()
        {
            return (from mailHistory in CurrentContext.EmailHistoryInfos
                        select new PresentationMail
                        {
                            Id = mailHistory.Id,
                            Subject = mailHistory.Subject,
                            To = mailHistory.To,
                            ActivityType = mailHistory.ActivityType,
                            Status = mailHistory.Status,
                            SentDateTime = mailHistory.SentDateTime,
                        }).ToList();
        }

        public IList<PresentationMail> GetByActivityType(string activityType, string status) 
        {
            List<PresentationMail> list = new List<PresentationMail>();

            var email = (from mail in CurrentContext.EmailInfos
                         select new PresentationMail()
                         {
                             Id = mail.Id,
                             Cc = mail.Cc,
                             Bcc = mail.Bcc,
                             Subject = mail.Subject,
                             To = mail.To,
                             ActivityType = mail.ActivityType,
                             Status = mail.Status,
                             CreatedDate = mail.CreatedDate,
                             SentDateTime = mail.SentDateTime,
                         });
            var email_history = (from mailHistory in CurrentContext.EmailHistoryInfos
                                 select new PresentationMail()
                                 {
                                     Id = mailHistory.Id,
                                     Cc = mailHistory.Cc,
                                     Bcc = mailHistory.Bcc,
                                     Subject = mailHistory.Subject,
                                     To = mailHistory.To,
                                     ActivityType = mailHistory.ActivityType,
                                     Status = mailHistory.Status,
                                     CreatedDate = mailHistory.CreatedDate,
                                     SentDateTime = mailHistory.SentDateTime,
                                 });

            // if activitytype = ALL
            if (activityType == "All")
            {
                // if status is ALL
                if (status == "All")
                {
                    list = email.Union(email_history).ToList();
                }
                // if status is SENT
                else if (status.ToUpper() == "SENT")
                {
                    list = email_history.Where(eh => (eh.Status == status)).ToList();
                }
                else if (status.ToUpper() == "EXCLUDED")
                {
                    list = email_history.Where(eh => (eh.Status == status)).ToList();
                }
                // if status is other than SENT or ALL
                else
                {
                    list = email.Where(eh => (eh.Status == status)).ToList();
                }
            }
            // if activity type != ALL
            else
            {
                // if status is ALL
                if (status == "All")
                {
                    list = email.Union(email_history).Where(eh => eh.ActivityType == activityType).ToList();
                }
                // if status is SENT
                else if (status.ToUpper() == "SENT")
                {
                    list = email_history.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType)).ToList();
                }
                else if (status.ToUpper() == "EXCLUDED")
                {                    
                    list = email_history.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType)).ToList();
                }
                // if status is other than SENT or ALL
                else
                {
                    list = email.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType)).ToList();
                }
            }
            
            return list.OrderByDescending(p => p.Id).ToList();
        }
        public PresentationGridPagedData<PresentationMail> GetPagedDataByActivityType(string activityType, string status, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, DateTime? fromDate, DateTime? ToDate)
        {
            PresentationGridPagedData<PresentationMail> dbResult = new PresentationGridPagedData<PresentationMail>();
            IQueryable<PresentationMail> query = null;
            CurrentContext.Database.CommandTimeout = 300;
            var email = (from mail in CurrentContext.EmailInfos
                         select new PresentationMail()
                         {
                             Id = mail.Id,
                             Cc = mail.Cc,
                             Bcc = mail.Bcc,
                             Subject = mail.Subject,
                             To = mail.To,
                             ActivityType = mail.ActivityType,
                             Status = mail.Status,
                             CreatedDate = mail.CreatedDate,
                             SentDateTime = mail.SentDateTime,
                         });
            if (fromDate.HasValue)
            {
                email = email.Where(i=> i.CreatedDate >= fromDate);
            }
            if (ToDate.HasValue)
            {
                email = email.Where(i => i.CreatedDate <= ToDate);
            }
            var email_history = (from mailHistory in CurrentContext.EmailHistoryInfos
                                 select new PresentationMail()
                                 {
                                     Id = mailHistory.Id,
                                     Cc = mailHistory.Cc,
                                     Bcc = mailHistory.Bcc,
                                     Subject = mailHistory.Subject,
                                     To = mailHistory.To,
                                     ActivityType = mailHistory.ActivityType,
                                     Status = mailHistory.Status,
                                     CreatedDate = mailHistory.CreatedDate,
                                     SentDateTime = mailHistory.SentDateTime,
                                 });
            if (fromDate.HasValue)
            {
                email_history = email_history.Where(i => i.CreatedDate >= fromDate);
            }
            if (ToDate.HasValue)
            {
                email_history = email_history.Where(i => i.CreatedDate <= ToDate);
            }
            // if activitytype = ALL
            if (activityType == "All")
            {
                // if status is ALL
                if (status == "All")
                {
                    query = email.Union(email_history);
                }
                // if status is SENT
                else if (status.ToUpper() == "SENT")
                {
                    query = email_history.Where(eh => (eh.Status == status));
                }
                else if (status.ToUpper() == "EXCLUDED")
                {
                    query = email_history.Where(eh => (eh.Status == status));
                }
                // if status is other than SENT or ALL
                else
                {
                    query = email.Where(eh => (eh.Status == status));
                }
            }
            // if activity type != ALL
            else
            {
                // if status is ALL
                if (status == "All")
                {
                    query =  email.Union(email_history).Where(eh => eh.ActivityType == activityType);
                }
                // if status is SENT
                else if (!string.IsNullOrWhiteSpace(status) && status.ToUpper() == "SENT")
                {
                    query = email_history.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType));
                }
                else if (!string.IsNullOrWhiteSpace(status) && status.ToUpper() == "EXCLUDED")
                {
                    query = email_history.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType));
                }
                // if status is other than SENT or ALL
                else
                {
                    query = email.Where(eh => (eh.Status == status) && (eh.ActivityType == activityType));
                }
            }
            if (!string.IsNullOrWhiteSpace(FilterExpression))
            {
                query = query.Where(FilterExpression);
            }
            dbResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i=>i.Id);
            }
            if (maximumRows == -1)
                dbResult.Data = query.ToList();
            else
                dbResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();

            return dbResult;
        }
    }
}

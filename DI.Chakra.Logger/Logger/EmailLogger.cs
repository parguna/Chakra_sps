using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class EmailLogger
    {
        public void WriteLog(long? emailProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                DILog.WriteEmailLogToDB(DILog.GetEmailLog(emailProcessId, loglevel, messageText, ex));
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? emailProcessId, LogLevelType loglevel, string messageText, Exception ex, long? emailId, string subject)
        {
            try
            {
                DILog.WriteEmailLogToDB(DILog.GetEmailLog(emailProcessId, loglevel, messageText, ex, emailId, subject));
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? emailProcessId, string reviewer, string emailSubject, LogLevelType loglevel, string messageText, Exception ex, long? emailId, string subject)
        {
            try
            {
                EmailLog log = DILog.GetEmailLog(emailProcessId, loglevel, messageText, ex, emailId, subject);
                log.Reviewer = reviewer;
                log.EmailSubject = emailSubject;
                DILog.WriteEmailLogToDB(log);
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? emailProcessId, LogLevelType loglevel, string messageText, long? emailId, string subject)
        {
            this.WriteLog(emailProcessId, loglevel, messageText, null, emailId, subject);
        }

        public void WriteLog(LogLevelType loglevel, string messageText, long? emailId, string subject)
        {
            this.WriteLog(null, loglevel, messageText, null, emailId, subject);
        }

        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex, long? emailId, string subject)
        {
            this.WriteLog(null, loglevel, messageText, ex, emailId, subject);
        }

        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex)
        {
            this.WriteLog(null, loglevel, messageText, ex, null, string.Empty);
        }

        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            this.WriteLog(loglevel, messageText, null);
        }

    }
}

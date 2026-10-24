using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class SchedulerLogger
    {
       public void WriteLog(long? schedulerProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                DILog.WriteServiceLogToDB(DILog.GetServiceLog(schedulerProcessId, LogReferenceType.Scheduler, loglevel, messageText, ex));
            }
            catch (Exception e)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? schedulerProcessId, LogLevelType loglevel, LogReferenceType refType,  string messageText, Exception ex)
        {
            try
            {
                DILog.WriteServiceLogToDB(DILog.GetServiceLog(schedulerProcessId, refType, loglevel, messageText, ex));
            }
            catch (Exception e)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? schedulerProcessId, LogLevelType loglevel, string messageText)
        {
            this.WriteLog(schedulerProcessId, loglevel, messageText, null);
        }

        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            this.WriteLog(null, loglevel, messageText, null);
        }

        public void WriteLog(LogLevelType loglevel, string messageText,Exception ex)
        {
            this.WriteLog(null, loglevel, messageText, ex);
        }
    }
}

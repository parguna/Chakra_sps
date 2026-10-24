using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class ReportingLogger
    {
        public  void WriteLog(long? reportingScheduleId,LogLevelType loglevel,string messageText,Exception ex)
        {
            try
            {
                DILog.WriteServiceLogToDB(DILog.GetServiceLog(reportingScheduleId, LogReferenceType.Reporting, loglevel, messageText, ex));
            }
            catch(Exception)
            {

            }
        }
        public void WriteLog(long? reportingScheduleId, LogLevelType loglevel, string messageText)
        {
            this.WriteLog(reportingScheduleId, loglevel, messageText, null);
        }

        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            this.WriteLog(null, loglevel, messageText, null);
        }
        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex)
        {
            this.WriteLog(null, loglevel, messageText, ex);
        }

    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class ComplianceLogger
    {
        public void WriteLog(long? complianceProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                DILog.WriteServiceLogToDB(DILog.GetServiceLog(complianceProcessId, LogReferenceType.Compliance, loglevel, messageText, ex));
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? complianceProcessId, LogLevelType loglevel, string messageText)
        {
            this.WriteLog(complianceProcessId, loglevel, messageText, null);
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

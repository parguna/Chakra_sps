using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class AuthenticationLogger
    {
     

        public void WriteLog(long? provisioningProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                DILog.WriteServiceLogToDB(DILog.GetServiceLog(provisioningProcessId, LogReferenceType.Authentication, loglevel, messageText, ex));
            }
            catch (Exception e)
            {
                string exMessage = e.Message;
                //Swallow Exception
            }
        }
        public void WriteLog(long? provisioningProcessId, LogLevelType loglevel, string messageText)
        {
            this.WriteLog(provisioningProcessId, loglevel, messageText, null);
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

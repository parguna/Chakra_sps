using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class AttestationLogger
    {      
        public void WriteLog(long? attestationProcessId,string attestationName, string reviewer, string reviewerType, string reviewee, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                var log = DILog.GetAttestationLog(attestationProcessId, attestationName, reviewer, reviewerType, reviewee, loglevel, messageText, ex);
                
                DILog.WriteAttestationLogToDB(log);
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? attestationProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            this.WriteLog(attestationProcessId, null, null, null, null, loglevel, messageText, ex);
        }

        public void WriteLog(long? attestationProcessId, LogLevelType loglevel, string messageText)
        {
            this.WriteLog(attestationProcessId,null,null,null,null, loglevel, messageText, null);
        }

        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            this.WriteLog(null, null, null, null, null, loglevel, messageText, null);
        }

        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex)
        {
            this.WriteLog(null, null, null, null, null, loglevel, messageText, ex);
        }
    }
}

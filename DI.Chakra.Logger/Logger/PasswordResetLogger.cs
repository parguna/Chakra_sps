using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class PasswordResetLogger:IdmsLogger
    {

        readonly long? _endpointId;

        readonly string _endpointSystemTypeName;
        readonly string _username;
        readonly long? _endpointSystemTypeId;
        public PasswordResetLogger(long? endpointId, long? endpointSystemTypeId, string endpointName, string username = null)
        {
            _endpointId = endpointId;
            _endpointSystemTypeId = endpointSystemTypeId;
            _endpointSystemTypeName = endpointName;
            _username = username;
        }
        public void WriteLog(long? provisioningProcessId, LogLevelType loglevel, string messageText, Exception ex)
        {
            try
            {
                var log = DILog.GetServiceLog(provisioningProcessId, LogReferenceType.PasswordReset, loglevel, $"{(!string.IsNullOrWhiteSpace(this._endpointSystemTypeName) ? $"Endpoint:{this._endpointSystemTypeName}" : "")}" +
                    $"{(!string.IsNullOrWhiteSpace(_username) ? $" UserName:{_username}" : "")}" +
                    $" { messageText}", ex);
                DILog.WriteServiceLogToDB(log);
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

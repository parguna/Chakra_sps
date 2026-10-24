using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure; 

namespace DI.Chakra.Logger
{
    public class ProvisioningLogger : IdmsLogger
    {
        readonly long? _requestId;
        readonly long? _endpointId;
        
        readonly string _endpointSystemTypeName;
        readonly long? _endpointSystemTypeId;
        public ProvisioningLogger(long? requestId, long? endpointId, long? endpointSystemTypeId, string endpointName)
        {
            _requestId = requestId;
            
                _endpointId = endpointId;
                _endpointSystemTypeId = endpointSystemTypeId;
            //if(endpointSystemTypeId.HasValue)
            _endpointSystemTypeName = endpointName;// ((EndpointType)endpointSystemTypeId).GetDescription(); 
        }
        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            WriteLog(loglevel, messageText, null);
        }
        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex = null)
        {
            try
            {
                ProvisioningLog provisioningLog = DILog.GetProvisioningLog(_requestId, _endpointId, loglevel, messageText, ex);
                provisioningLog.EndpointSystemTypeId = _endpointSystemTypeId;
                provisioningLog.EndpointSystemTypeName = _endpointSystemTypeName;
                DILog.WriteProvisioningLogToDB(provisioningLog);
            }
            catch (Exception e)
            {
                string exMessage = e.Message;
                //Swallow Exception
            }
        }

        public void WriteLog(long? requestId, long? endpointId, LogLevelType loglevel, string messageText, Exception ex = null)
        {
            try
            {
                ProvisioningLog provisioningLog = DILog.GetProvisioningLog(requestId, endpointId, loglevel, messageText, ex);
                provisioningLog.AccessRequestId = requestId;
                if (_endpointId.HasValue && endpointId.HasValue && _endpointId.Value == endpointId.Value)
                {
                    provisioningLog.EndpointSystemTypeName = _endpointSystemTypeName;
                    provisioningLog.EndpointSystemTypeId = _endpointSystemTypeId;
                    provisioningLog.EndpointSystemId = endpointId.Value;
                }
                else {
                    provisioningLog.EndpointSystemId = endpointId;
                }
                DILog.WriteProvisioningLogToDB(provisioningLog);
            }
            catch (Exception e)
            {
                string exMessage = e.Message;
                //Swallow Exception
            }
        }

    }
}

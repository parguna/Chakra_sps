using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Logger
{
    public class DiscoveryLogger : IdmsLogger
    {
        readonly long _discoveryProcessId;
        readonly long _endSystemId;
        readonly EndpointType _endpointSystemType;
        readonly string _endpointSystemTypeName;

        public DiscoveryLogger(long discoveryProcessId,
          long endSystemId,
          EndpointType endpointSystemType, string endpointSystemName)
        {
            _discoveryProcessId = discoveryProcessId;
            _endSystemId = endSystemId;
            _endpointSystemType = endpointSystemType;

            _endpointSystemTypeName = endpointSystemName;// endpointSystemType.GetDescription();
        }

        public void WriteLog(LogLevelType loglevel, string messageText)
        {
            var log = DILog.GetDiscoveryLog(_endSystemId, _endpointSystemType, DiscoveryObjectType.None, loglevel, messageText, null);
            log.DiscoveryProcessId = _discoveryProcessId;
            log.EndpointSystemName = _endpointSystemTypeName;

            DILog.WriteDiscoveryLogToDB(log);
        }
        public void WriteLog(LogLevelType loglevel, string messageText,DiscoveryObjectType discoveryObjectName)
        {
            var log = DILog.GetDiscoveryLog(_endSystemId, _endpointSystemType, discoveryObjectName, loglevel, messageText, null);
            log.DiscoveryProcessId = _discoveryProcessId;
            log.EndpointSystemName = _endpointSystemTypeName;

            DILog.WriteDiscoveryLogToDB(log);
        }
        public void WriteLog(LogLevelType loglevel, string messageText, Exception ex=null)
        {
            var log = DILog.GetDiscoveryLog(_endSystemId, _endpointSystemType, DiscoveryObjectType.None, loglevel, messageText, ex);
            log.DiscoveryProcessId = _discoveryProcessId;
            log.EndpointSystemName = _endpointSystemTypeName;
            
            DILog.WriteDiscoveryLogToDB(log);
        }

        public void WriteLog(LogLevelType loglevel, string messageText, DiscoveryObjectType type, Exception ex = null)
        {
            var log = DILog.GetDiscoveryLog(_endSystemId, _endpointSystemType, type, loglevel, messageText, ex);
            log.DiscoveryProcessId = _discoveryProcessId;
            log.EndpointSystemName = _endpointSystemTypeName;

            DILog.WriteDiscoveryLogToDB(log);
        }
        public void WriteLog(long? discoveryProcessId, LogLevelType loglevel, string messageText,Exception ex=null)
        {
            this.WriteLog(loglevel, messageText, null);
        }

        //public void WriteLog(long? endPointSystemId, LogLevelType loglevel, string messageText, Exception ex)
        //{
        //    this.WriteLog(null, endPointSystemId, EndpointType.None, DiscoveryObjectType.None, loglevel, messageText, ex);
        //}
        //public void WriteLog( LogLevelType loglevel, string messageText)
        //{
        //    this.WriteLog(null, loglevel, messageText);
        //}
        //public void WriteLog(LogLevelType loglevel, string messageText, Exception ex)
        //{
        //    this.WriteLog(null, loglevel, messageText, ex);
        //}
    }
}

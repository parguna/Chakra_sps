using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    public class DiscoveryLogExportData
    {
        public int DiscoveryLogId { get; set; }
        public long EndpointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public EndpointType EndpointSystemType { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; }
        public DateTime LogDate { get; set; }
    }
}

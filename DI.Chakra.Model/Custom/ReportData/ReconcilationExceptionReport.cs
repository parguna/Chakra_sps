using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model.ReportData
{
    public class ReconcilationExceptionReport
    {
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

    }
}

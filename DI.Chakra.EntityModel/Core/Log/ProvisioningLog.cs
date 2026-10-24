using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Log_Provisioning")]
    public class ProvisioningLog : EntityBase
    {
        public long? AccessRequestId { get; set; }
        public string LogLevel { get; set; }
        public long? EndpointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public long? EndpointSystemId { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string InnerExceptionMessage { get; set; }
        public DateTime LogDate { get; set; }
        public string CreatedBy { get; set; }

    }
}

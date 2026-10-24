using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Log_Discovery")]
    public class DiscoveryLog : EntityBase, ICreatable, IUpdatable
    {
        public string LogLevel { get; set; }
        public long? DiscoveryProcessId { get; set; }
        public EndpointType EndpointSystemType { get; set; }
        public string EndpointSystemName { get; set; }
        public long EndpointSystemId { get; set; }
        public DiscoveryObjectType DiscoveryObjectType { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string InnerExceptionMessage { get; set; }
        public DateTime LogDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}

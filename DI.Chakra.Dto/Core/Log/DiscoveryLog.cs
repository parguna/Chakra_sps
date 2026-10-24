using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class DiscoveryLog : DtoBase, ILogger
    {
        public int? DiscoveryProcessId { get; set; }

        public string LogLevel { get; set; }

        public long EndpointSystemId { get; set; }
        //public long? EndpointSystemTypeId { get; set; }
        public EndpointType EndpointSystemType { get; set; }
        //public string EndpointSystemTypeName { get; set; }

        public string EndpointSystemName { get; set; }

        public string Message { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionSource { get; set; }

        public string InnerExceptionMessage { get; set; }

        public DateTime LogDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }
        public DiscoveryObjectType DiscoveryObjectType { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }


        public string ReferenceId
        {
            get
            {
                return DI.Chakra.Infrastructure.UtilityMethods.GetEnumAttribute<Description>(DiscoveryObjectType);
            }
        }

        public string EndpointSystemTypeFullName
        {
            get
            {
                return DI.Chakra.Infrastructure.UtilityMethods.GetEnumAttribute<Description>(this.EndpointSystemType);
            }
        }

    }


    public class LogParameter
    {
        public string logReferenceType { get; set; }
        public DateTime? dtFrom { get; set; }
        public DateTime? dtTo { get; set; }
        public LogLevelType logLevelType { get; set; }
        public int? pageSize { get; set; }
        public int? pageNumber { get; set; }
        public string FilterExpression { get; set; }
        public string sortExpression { get; set; }
        public long? endpointSystemId { get; set; }
        public DiscoveryObjectType objectType { get; set; }
        public long? RequestId { get; set; }

    }

 
}

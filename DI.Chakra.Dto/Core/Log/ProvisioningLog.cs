using System;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Dto
{
    public class ProvisioningLog : DtoBase, ILogger
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

        public string ReferenceId
        {
            get
            {
                return this.AccessRequestId.HasValue? this.AccessRequestId.Value.ToString() : string.Empty;
            }
        }



        public string EndpointSystemName
        {
            get
            {
                return EndpointSystemTypeName;
            }
        }

        public string EndpointSystemTypeFullName
        {
            get
            {
                if (EndpointSystemTypeId == null) return string.Empty;
                return UtilityMethods.GetEnumAttribute<Description>((DI.Chakra.Infrastructure.Enums.EndpointType)EndpointSystemTypeId);
            }
        }
    }
}

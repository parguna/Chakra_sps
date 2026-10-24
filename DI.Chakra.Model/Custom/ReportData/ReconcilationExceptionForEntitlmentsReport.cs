using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model.ReportData
{
    public class ReconcilationExceptionForEntitlmentsReport
    {
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public int EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public string EntitlementTypeValue
        {
            get
            {
                return ((EntitlementType)this.EntitlementType).ToString();
            }
        }

    }
}

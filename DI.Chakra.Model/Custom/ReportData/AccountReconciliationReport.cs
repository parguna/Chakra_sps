using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model.ReportData
{
    public class AccountReconciliationReport
    {
        public string EndpointSystemType { get; set; }
        public string EndpointSystemName { get; set; }
        public string AccountName { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsManuallyAdded { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
    }

    public class EntitlementReconciliationReport
    {
        public string EndpointSystemType { get; set; }
        public string EndpointSystemName { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsManuallyAdded { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
    }

}

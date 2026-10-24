using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationEntitlementReconcilation
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

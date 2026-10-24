using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAccountReconcilationReport
    {
        public long ReconciliationId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }

        public string EndPointName { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }
        public ReconcilationType ReconcilationType { get; set; }

        public string ResolvedBy { get; set; }

        public string Comments { get; set; }
        public string EntitlementType { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public string IsDeleted { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsLocked { get; set; } 
        public bool IsDeletedBoolean { get; set; }
        public AccountType AccountType { get; set; }
        public string OffboardingAction { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAccountReconcilationReport : IIDMSReportPropertyExtractor
    {
        public long ReconciliationId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }

        public string EndPointName { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }
        public ReconcilationType ReconcilationType { get; set; }
        public string EntitlementType { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }

        public string ResolvedBy { get; set; }

        public string Comments { get; set; }

        public string IsDeleted { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsLocked { get; set; } 
        public bool IsDeletedBoolean { get; set; }
        public AccountType AccountType { get; set; }

        public string OffboardingAction { get; set; }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "AccountName":
                    returnValue = AccountName ?? "";
                    break;
                case "EndPointName":
                    returnValue = EndPointName ?? "";
                    break;
                case "DisplayName":
                    returnValue = DisplayName ?? "";
                    break;
                case "UserDisplayName":
                    returnValue = UserDisplayName ?? "";
                    break;
                case "IsLocked":
                    returnValue = IsLocked.ToString();
                    break;
                case "IsDeletedBoolean":
                    returnValue = IsDeletedBoolean.ToString();
                    break;
                case "AccountType":
                    returnValue = AccountType.ToString();
                    break;
                case "OffboardingAction":
                    returnValue = OffboardingAction ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}

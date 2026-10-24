using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class InhouseUserInfo : DtoBase, IEndpointUserInfo, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndPointSystemId { get; set; }

        public string InhouseSystemCode { get; set; }

        public string NRIC { get; set; }

        public string Email { get; set; }
        public string IsMapped { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? AccountCreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedDate { get; set; }
        public string LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsDisabled { get; set; }

        public DateTime? DisabledDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Remarks1 { get; set; }

        public string Remarks2 { get; set; }

        public string Remarks3 { get; set; }

        public string Remarks4 { get; set; }

        public string Remarks5 { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual List<InhousePermission> Permissions { get; set; }

        public EndPoint EndPoint { get; set; }

        public string AccountName { get; set; }
        

        public string AccountPassword { get; set; }
       

        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
        

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public string FullName { get; set; }
        public string EmployeeId { get; set; }
        public string Permission { get; set; }
        public long ReconciliationId { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "UserName":
                    returnValue = UserName ?? "";
                    break;
                case "InhouseSystemCode":
                    returnValue = InhouseSystemCode ?? "";
                    break;
                case "EmployeeId":
                    returnValue = EmployeeId ?? "";
                    break;
                case "Email":
                    returnValue = Email ?? "";
                    break;
                case "DisplayName":
                    returnValue = DisplayName ?? "";
                    break;

                case "AccountType":
                    returnValue = AccountType.ToString();
                    break;
                case "AccountCreatedDate":
                    returnValue = AccountCreatedDate.HasValue ? AccountCreatedDate.Value.ToString("dd-MM-yyyy") : "";
                    break;
                case "LastLoginDate":
                    returnValue = LastLoginDate.HasValue ? LastLoginDate.Value.ToString("dd-MM-yyyy") : "";
                    break;
                case "IsLocked":
                    returnValue = IsLocked.ToString();
                    break;
                case "IsDisabled":
                    returnValue = IsDisabled.ToString();
                    break;
                case "IsDeleted":
                    returnValue = IsDeleted.ToString();
                    break;
                case "Remarks1":
                    returnValue = Remarks1 ?? "";
                    break;
                case "Remarks2":
                    returnValue = Remarks2 ?? "";
                    break;
                case "Remarks3":
                    returnValue = Remarks3 ?? "";
                    break;
                case "Remarks4":
                    returnValue = Remarks4 ?? "";
                    break;
                case "Remarks5":
                    returnValue = Remarks5 ?? "";
                    break;
                case "Permission":
                    returnValue = Permission ?? "";
                    break;
                case "FirstName":
                    returnValue = FirstName ?? "";
                    break;
                case "LastName":
                    returnValue = LastName ?? "";
                    break;
                case "ValidationMessage":
                    returnValue = ValidationMessage ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
        public Nullable<bool> AddedByDiscovery { get; set; }
    }
}

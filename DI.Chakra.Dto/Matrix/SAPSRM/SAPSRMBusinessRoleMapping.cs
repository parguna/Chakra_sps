using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPSRMBusinessRoleMapping:DtoBase, IXLData, IIDMSReportPropertyExtractor
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public AccountType AccountType { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public String DisplayName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string EmployeeId { get; set; }
        public bool IsValid { get; set; }
        public bool IsEditMode { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "EmployeeId":
                    returnValue = EmployeeId ?? "";
                    break;
                case "UserName":
                    returnValue = UserName ?? "";
                    break;
                case "AccountType":
                    returnValue = Convert.ToString(AccountType);
                    break;
                case "BusinessRole":
                    returnValue = BusinessRole ?? "";
                    break;
                case "POrg":
                    returnValue = POrg ?? "";
                    break;
                case "PGrp":
                    returnValue = PGrp ?? "";
                    break;
                
                case "ValidationMessage":
                    returnValue = ValidationMessage ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}

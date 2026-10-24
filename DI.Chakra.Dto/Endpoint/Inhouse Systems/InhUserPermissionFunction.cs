using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class InhUserPermissionFunction: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndpointUserId { get; set; }
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string EmployeeId { get; set; }
        //public string NRIC { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string Functions { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "UserName":
                    returnValue = UserName ?? "";
                    break;
                case "EmployeeId":
                    returnValue = EmployeeId ?? "";
                    break;
                case "InhouseSystemCode":
                    returnValue = InhouseSystemCode ?? "";
                    break;
                case "Permission":
                    returnValue = Permission ?? "";
                    break;
                case "Functions":
                    returnValue = Functions ?? "";
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

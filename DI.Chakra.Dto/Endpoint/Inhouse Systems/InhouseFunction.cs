using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class InhouseFunction: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndpointSystemId { get; set; }
        public string FunctionName { get; set; }
        public string PermissionName { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "FunctionName":
                    returnValue = FunctionName ?? "";
                    break;
                case "PermissionName":
                    returnValue = PermissionName ?? "";
                    break;
                case "InhouseSystemCode":
                    returnValue = InhouseSystemCode ?? "";
                    break;
                case "Description":
                    returnValue = Description ?? "";
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

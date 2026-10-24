using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NOVACompositeRoleMatrix: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndPointSystemId { get; set; }
        public string Job { get; set; }
        public string CompositeRoleName { get; set; }
        public string ModuleWiseRoles { get; set; }
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
                case "Job":
                    returnValue = Job ?? "";
                    break;
                case "CompositeRoleName":
                    returnValue = CompositeRoleName ?? "";
                    break;
                case "ModuleWiseRoles":
                    returnValue = ModuleWiseRoles ?? "";
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

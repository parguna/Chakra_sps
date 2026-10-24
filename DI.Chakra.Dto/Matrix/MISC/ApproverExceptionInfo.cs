using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class ApproverExceptionInfo: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string Person { get; set; }
        public string Approver { get; set; }
        public int ApprovalLevel { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Person":
                    returnValue = Person ?? "";
                    break;
                case "Approver":
                    returnValue = Approver ?? "";
                    break;
                case "ApprovalLevel":
                    returnValue = ApprovalLevel.ToString();
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

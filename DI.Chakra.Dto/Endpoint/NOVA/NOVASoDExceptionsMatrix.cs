using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NOVASoDExceptionsMatrix : DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndPointSystemId { get; set; }
        public string Exceptions { get; set; }
        public string AffectedUsers { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Exceptions":
                    returnValue = Exceptions ?? "";
                    break;
                case "AffectedUsers":
                    returnValue = AffectedUsers ?? "";
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

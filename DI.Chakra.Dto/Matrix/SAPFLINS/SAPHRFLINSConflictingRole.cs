
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPHRFLINSConflictingRole  : DtoBase , IXLData,IIDMSReportPropertyExtractor
    {        
        public string BusinessRole { get; set; }
        public string ConflictingBusinessRole { get; set; }
        public string TechnicalRole { get; set; }
        public string ConflictingTechnicalRole { get; set; }

        public Nullable<bool> IsModified { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public bool IsValid { get; set; } 
        public string ValidationMessage { get; set; }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "BusinessRole":
                    returnValue = BusinessRole ?? "";
                    break;
                case "ConflictingBusinessRole":
                    returnValue = ConflictingBusinessRole ?? "";
                    break;
                case "TechnicalRole":
                    returnValue = TechnicalRole ?? "";
                    break;
                case "ConflictingTechnicalRole":
                    returnValue = ConflictingTechnicalRole ?? "";
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


using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPRoleMandatoryField  : DtoBase, IXLData,IIDMSReportPropertyExtractor
    { 
       
        public string BusinessRole { get; set; }
        public string MandatoryField1 { get; set; }
        public string MandatoryField2 { get; set; }
        public Nullable<bool> IsModified { get; set; } 
        public Nullable<bool> IsDeleted { get; set; } 
        public Nullable<DateTime> ValidFrom { get; set; } 
        public Nullable<DateTime> ValidTo { get; set; } 
        public Nullable<bool> IsModifiedbyExcelUpload{get;set;} 
        public Nullable<int> ReferenceId { get; set; }         
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
                case "MandatoryField1":
                    returnValue = MandatoryField1 ?? "";
                    break;
                case "MandatoryField2":
                    returnValue = MandatoryField2 ?? "";
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


using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class ProvisioningConfigMatrixSAPPosition : DtoBase , IXLData,IIDMSReportPropertyExtractor
    {       
        public string PositionText { get; set; }
        public string DefaultRole { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
      
        public bool IsValid { get; set; }
     
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "PositionText":
                    returnValue = PositionText ?? "";
                    break;
                case "DefaultRole":
                    returnValue = DefaultRole ?? "";
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

 
using System;
 

namespace DI.Chakra.Dto
{
    [Serializable]
    public class ProvisioningConfigMatrixSapEEGroup : DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string EmployeeGroupNo { get; set; }
        public string EmployeeGroup { get; set; }
        public string PArea { get; set; }
        public string DefaultRole { get; set; }
        public string Remark { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "EmployeeGroupNo":
                    returnValue = EmployeeGroupNo ?? "";
                    break;
                case "EmployeeGroup":
                    returnValue = EmployeeGroup ?? "";
                    break;
                case "PArea":
                    returnValue = PArea ?? "";
                    break;
                case "DefaultRole":
                    returnValue = DefaultRole ?? "";
                    break;
                case "Remark":
                    returnValue = Remark ?? "";
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

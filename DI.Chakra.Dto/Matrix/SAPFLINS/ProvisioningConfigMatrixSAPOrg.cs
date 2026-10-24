
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class ProvisioningConfigMatrixSAPOrg : DtoBase , IXLData,IIDMSReportPropertyExtractor
    {        
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string DefaultHRMSGeneralRoles { get; set; }
        public string DefaultEPGroup { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
       
        public bool IsValid { get; set; }
       
        public string ValidationMessage { get; set; }

        public string personnelArea { get; set; }
        public string PersonnelSubArea { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "CompanyCode":
                    returnValue = CompanyCode ?? "";
                    break;
                case "CompanyName":
                    returnValue = CompanyName ?? "";
                    break;
                case "DepartmentCode":
                    returnValue = DepartmentCode ?? "";
                    break;
                case "personnelArea":
                    returnValue = personnelArea ?? "";
                    break;
                case "DivisionCode":
                    returnValue = DivisionCode ?? "";
                    break;
                case "PersonnelSubArea":
                    returnValue = PersonnelSubArea ?? "";
                    break;
                case "CostCenterCode":
                    returnValue = CostCenterCode ?? "";
                    break;
                case "CostCenterName":
                    returnValue = CostCenterName ?? "";
                    break;
                case "DefaultHRMSGeneralRoles":
                    returnValue = DefaultHRMSGeneralRoles ?? "";
                    break;
                case "DefaultEPGroup":
                    returnValue = DefaultEPGroup ?? "";
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

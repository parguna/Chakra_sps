
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPHRFLINSSuppRoleMatrix  : DtoBase , IXLData, IIDMSReportPropertyExtractor
    { 
        
        public string Rule { get; set; }
        
        public string Company { get; set; }
        
        public string CostCenter { get; set; }
        
        public string PurchasingOrg { get; set; }
        
        public string PurchasingGrp { get; set; }
        
        public string Plant { get; set; }
        
        public string MRPControl { get; set; }
        
        public string StorageLocation { get; set; }
        
        public string Role { get; set; }
        
        public string RoleName { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsModifiedbyExcelUpdate { get; set; }
        public Nullable<int> ReferenceId { get; set; }
      
        public bool IsValid { get; set; }
      
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Company":
                    returnValue = Company ?? "";
                    break;
                case "Rule":
                    returnValue = Rule ?? "";
                    break;
                case "CostCenter":
                    returnValue = CostCenter ?? "";
                    break;
                case "PurchasingOrg":
                    returnValue = PurchasingOrg ?? "";
                    break;
                case "PurchasingGrp":
                    returnValue = PurchasingGrp ?? "";
                    break;
                case "MRPControl":
                    returnValue = MRPControl ?? "";
                    break;
                case "Plant":
                    returnValue = Plant ?? "";
                    break;
                case "StorageLocation":
                    returnValue = StorageLocation ?? "";
                    break;
                case "Role":
                    returnValue = Role ?? "";
                    break;
                case "RoleName":
                    returnValue = RoleName ?? "";
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


using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPRoleMatrix : DtoBase , IXLData ,IIDMSReportPropertyExtractor
    { 
        public string Application { get; set; }
        
        public string EligibilityRule { get; set; }
        
        public string Company { get; set; }
        
        public string PersonnelArea { get; set; }
        
        public string PersonnelSubArea { get; set; }
        
        public string Position { get; set; }
        
        public string OrgUnit { get; set; }
        
        public string CostCenter { get; set; }
        
        public string Remarks { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsUpdatedbyExcel { get; set; }
        public Nullable<int> ReferenceId { get; set; } 
        public bool IsValid { get; set; }
         
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Application":
                    returnValue = Application ?? "";
                    break;
                case "EligibilityRule":
                    returnValue = EligibilityRule ?? "";
                    break;
                case "Company":
                    returnValue = Company ?? "";
                    break;
                case "PersonnelArea":
                    returnValue = PersonnelArea ?? "";
                    break;
                case "PersonnelSubArea":
                    returnValue = PersonnelSubArea ?? "";
                    break;
                case "Position":
                    returnValue = Position ?? "";
                    break;
                case "OrgUnit":
                    returnValue = OrgUnit ?? "";
                    break;
                case "CostCenter":
                    returnValue = CostCenter ?? "";
                    break;
                case "Remarks":
                    returnValue = Remarks ?? "";
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

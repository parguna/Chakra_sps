using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPBIRoleCategory  : DtoBase , IXLData, IApprover,IIDMSReportPropertyExtractor
    {               
        public string Remarks { get; set; }
        
        public string Application { get; set; }
        
        public string Module { get; set; }
        
        public string SubModule { get; set; }
        
        public string BusinessRole { get; set; }
        
        public string TechnicalRole { get; set; }
        
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
      
        public Nullable<DateTime> ValidTo { get; set; }

        //public Nullable<bool> IsModifiedbyExcelUpload{get;set;}

        //public Nullable<int> ReferenceId { get; set; }

        
        public bool IsValid { get; set; }
        
        public string ValidationMessage { get; set; }

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.FLINS_BI);
            }
        }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Remarks":
                    returnValue = Remarks??"";
                    break;
                case "Application":
                    returnValue = Application ?? "";
                    break;
                case "Module":
                    returnValue = Module ?? "";
                    break;
                case "SubModule":
                    returnValue = SubModule ?? "";
                    break;
                case "BusinessRole":
                    returnValue = BusinessRole ?? "";
                    break;
                case "TechnicalRole":
                    returnValue = TechnicalRole ?? "";
                    break;
                case "Level1Approver":
                    returnValue = Level1Approver ?? "";
                    break;
                case "Level1ApproverType":
                    returnValue = ((int)Level1ApproverType).ToString();
                    break;
                case "Level2Approver":
                    returnValue = Level2Approver ?? ""; ;
                    break;
                case "Level2ApproverType":
                    returnValue = ((int)Level2ApproverType).ToString();
                    break;
                case "Level3Approver":
                    returnValue = Level3Approver ?? "";
                    break;
                case "Level3ApproverType":
                    returnValue = ((int)Level3ApproverType).ToString();
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

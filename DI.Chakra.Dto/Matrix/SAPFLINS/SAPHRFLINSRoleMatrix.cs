
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPHRFLINSRoleMatrix  : DtoBase , IXLData, IApprover,IIDMSReportPropertyExtractor
    {         
        public string Remarks { get; set; }
        
        public string Application { get; set; }
        
        public string Company { get; set; }
        
        public string CostCenter { get; set; }
        
        public string PurchasingOrg { get; set; }
        
        public string PurchasingGrp { get; set; }
        
        public string Plant { get; set; }
        
        public string MRPControl { get; set; }
        
        public string StorageLocation { get; set; }
        
        public string Module { get; set; }
        
        public string SubModule { get; set; }
        
        public string BusinessRole { get; set; }
        
        public string TechnicalRule { get; set; }
        
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }

        
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        
        public string MainSupp { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public Nullable<bool> IsUpdatedbyExcelUpload { get; set; }
        public Nullable<int> ReferenceId { get; set; }

         
        public bool IsValid { get; set; }
        
        public string ValidationMessage { get; set; }

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.HRMS_FLINS);
            }
        }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Remarks":
                    returnValue = Remarks ?? "";
                    break;
                case "Application":
                    returnValue = Application ?? "";
                    break;
                case "Company":
                    returnValue = Company ?? "";
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
                case "StorageLocation":
                    returnValue = StorageLocation ?? "";
                    break;
                case "Plant":
                    returnValue = Plant ?? "";
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
                case "TechnicalRule":
                    returnValue = TechnicalRule ?? "";
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
                case "MainSupp":
                    returnValue = MainSupp??"";
                    break;
                case "ValidationMessage":
                    returnValue = ValidationMessage ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
        public string Level1ApproverTypeString
        {
            get
            {
                return ((int)Level1ApproverType).ToString();
            }
        }
        public string Level2ApproverTypeString
        {
            get
            {
                return ((int)Level2ApproverType).ToString();
            }
        }
        public string Level3ApproverTypeString
        {
            get
            {
                return ((int)Level3ApproverType).ToString();
            }
        }
    }

    [Serializable]
    public class SAPFlinsMatrixBusinessRules
    {
        public bool IsCompanyMandatory {get;set;}

        public bool HasSupplimentaryRole { get; set; }

        public bool CanClearAllPurchaseGrp { get; set; }

        public bool CanClearAllCostCenter { get; set; }

        public bool ShouldClearAllPurchaseGroup { get; set; }
        public bool ShouldClearAllCostcenter { get; set; }

        public bool CanClearAllPurchaseOrg { get; set; }

        public bool ShouldClearAllPurchaseOrg { get; set; }

        public bool CanClearAllPlant { get; set; }

        public bool ShouldClearAllPlant { get; set; }

        public bool CanClearAllMRP { get; set; }

        public bool ShouldClearAllMRP { get; set; }

        public bool CanClearAllStorageLocation { get; set; }

        public bool ShouldClearAllStorageLocation { get; set; }

    }
}

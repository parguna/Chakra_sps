using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class GEMSRoleMatrix : DtoBase, IEndpointEntitlementInfo, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndPointSystemId { get; set; }
     
        public string BusinessRoleName { get; set; }
       
        public string TechnicalRole { get; set; }
        public string ADGroups { get; set; }    
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }       
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }       
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }       
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string UserDefinedDesc { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "BusinessRoleName":
                    returnValue = BusinessRoleName ?? "";
                    break;
                case "TechnicalRole":
                    returnValue = TechnicalRole ?? "";
                    break;
                case "ADGroups":
                    returnValue = ADGroups ?? "";
                    break;
                case "Level1ApproverType":
                    returnValue = ((int)Level1ApproverType).ToString();
                    break;
                case "Level1Approver":
                    returnValue =Level1Approver??"";
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
}

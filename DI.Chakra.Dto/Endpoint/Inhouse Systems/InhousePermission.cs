using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class InhousePermission : DtoBase, IEndpointEntitlementInfo, IXLData,IIDMSReportPropertyExtractor
    {
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public virtual EndPoint EndPoint { get; set; }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Permission; }
        }
        public string EntitlementName
        {
            get { return Name; }
        }

        public string EntitlementDesc
        {
            get { return Description; }
        }

        public string UserDefinedDesc { get; set; }
        public string ADGroups { get; set; }
        public string EPGroups { get; set; }
        public string EPRoles { get; set; }
        public string CEGroups { get; set; }
        public string CERoles { get; set; }
        public bool IsDeleted { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "EntitlementName":
                    returnValue = EntitlementName ?? "";
                    break;
                case "InhouseSystemCode":
                    returnValue = InhouseSystemCode ?? "";
                    break;
                case "Description":
                    returnValue = Description ?? "";
                    break;
                case "ADGroups":
                    returnValue = ADGroups ?? "";
                    break;
                case "EPGroups":
                    returnValue = EPGroups ?? "";
                    break;
                case "EPRoles":
                    returnValue = EPRoles ?? "";
                    break;
                case "CEGroups":
                    returnValue = CEGroups ?? "";
                    break;
                case "CERoles":
                    returnValue = CERoles ?? "";
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
                case "IsActive":
                    returnValue = IsActive ? "Yes":"No";
                    break;
                case "IsDeleted":
                    returnValue = "No";
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

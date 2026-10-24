using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPSRMMainRoleMatrix: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string RoleType { get; set; }
        public string BusinessRole { get; set; }
        public string SRMRoles { get; set; }
        public string SUSRoles { get; set; }
        public string SRMEnablerRule { get; set; }
        public string BIRoles { get; set; }
        public string BIEnablerRule { get; set; }
        public string InternalPortalRoles { get; set; }
        public string InternalPortalGroups { get; set; }
        public string ExternalPortalRoles { get; set; }
        public string ExternalPortalGroups { get; set; }
        public string ADGroups { get; set; }
        public string DepartmentCode { get; set; }
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
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SRM);
            }
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "RoleType":
                    returnValue = RoleType ?? "";
                    break;
                case "BusinessRole":
                    returnValue = BusinessRole ?? "";
                    break;
                case "InternalPortalRoles":
                    returnValue = InternalPortalRoles ?? "";
                    break;
                case "InternalPortalGroups":
                    returnValue = InternalPortalGroups ?? "";
                    break;
                case "ExternalPortalRoles":
                    returnValue = ExternalPortalRoles ?? "";
                    break;
                case "ExternalPortalGroups":
                    returnValue = ExternalPortalGroups ?? "";
                    break;
                case "SRMRoles":
                    returnValue = SRMRoles ?? "";
                    break;
                case "SUSRoles":
                    returnValue = SUSRoles ?? "";
                    break;
                case "SRMEnablerRule":
                    returnValue = SRMEnablerRule ?? "";
                    break;
                case "BIRoles":
                    returnValue = BIRoles ?? "";
                    break;
                case "BIEnablerRule":
                    returnValue = BIEnablerRule ?? "";
                    break;
                case "ADGroups":
                    returnValue = ADGroups ?? "";
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
                case "DepartmentCode":
                    returnValue = DepartmentCode ?? "";
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

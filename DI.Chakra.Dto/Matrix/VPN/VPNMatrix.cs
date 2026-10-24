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
    public class VPNMatrix : DtoBase, IXLData, IIDMSReportPropertyExtractor
    {
        public string Name { get; set; }
        public string ProductionADgroups { get; set; }
        public string QAADGroups { get; set; }
        public string DevelpomentADGroups { get; set; }
        public string ADGroups { get; set; }
        public VPNMatrixType VPNMatrixType { get; set; }
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
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public string BusinessRole
        {
            get { return this.Name; }
        }

        public string SystemName
        {
            get { return ApplicationType.VPN.GetDescription(); }
        }
        public string EntitlementName
        {
            get
            {
                return this.Name;
            }
            set
            {
                Name = value;
            }
        }

        public string EntitlementDesc
        {
            get
            {
                return this.Name;
            }
            set
            {
                Name = value;
            }
        }

        public EntitlementType EntitlementType
        {
            get;
            set;
        }
        public string MatrixType
        {
            get
            {
                if (VPNMatrixType == VPNMatrixType.Project)
                {
                    return VPNType.VDI.ToString();
                }
                return VPNType.SSLVPNOnly.ToString();

            }
        }
        public VPNType Type
        {
            get
            {
                if(VPNMatrixType == VPNMatrixType.Project)
                {
                    return VPNType.VDI;
                }
                return VPNType.SSLVPNOnly;
            }
            set
            {
                if(value == VPNType.SSLVPNOnly)
                {
                    VPNMatrixType = VPNMatrixType.Application;
                }else if(value == VPNType.VDI)
                {
                    VPNMatrixType = VPNMatrixType.Project;
                }
            }
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Id":
                    returnValue = Convert.ToString(Id);break;
                case "Type":
                    returnValue =Convert.ToString(Type);
                    break;
                case "Name":
                    returnValue = Name ?? "";
                    break;
                case "ProductionADgroups":
                    returnValue = ProductionADgroups ?? "";
                    break;
                case "QAADGroups":
                    returnValue = QAADGroups ?? "";
                    break;
                case "DevelpomentADGroups":
                    returnValue = DevelpomentADGroups ?? "";
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

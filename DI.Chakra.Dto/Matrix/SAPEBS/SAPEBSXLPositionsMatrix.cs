using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    public class SAPEBSXLPositionsMatrix: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string Position_ISP { get; set; }
        public string Position_CRP { get; set; }
        public string Prefix_Employees { get; set; }
        public string Prefix_TempStaff { get; set; }
        public string User_Group { get; set; }
        public string Department_Field { get; set; }
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

        public string BusinessRole
        {
            get { return this.PositionText; }
        }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Remarks":
                    returnValue = Remarks ?? "";
                    break;
                case "PositionText":
                    returnValue = PositionText ?? "";
                    break;
                case "Position_ISP":
                    returnValue = Position_ISP ?? "";
                    break;
                case "Position_CRP":
                    returnValue = Position_CRP ?? "";
                    break;
                case "Prefix_Employees":
                    returnValue = Prefix_Employees ?? "";
                    break;
                case "Prefix_TempStaff":
                    returnValue = Prefix_TempStaff ?? "";
                    break;
                case "User_Group":
                    returnValue = User_Group ?? "";
                    break;
                case "Department_Field":
                    returnValue = Department_Field ?? "";
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

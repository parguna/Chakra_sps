using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPSRMEnablerRoleMatrix: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string SRMEnablerRule { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public string SRMTechRole { get; set; }
        public string BIPOrgRole { get; set; }
        public string BIPGrpRole { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "SRMEnablerRule":
                    returnValue = SRMEnablerRule ?? "";
                    break;
                case "POrg":
                    returnValue = POrg ?? "";
                    break;
                case "PGrp":
                    returnValue = PGrp ?? "";
                    break;
                case "SRMTechRole":
                    returnValue = SRMTechRole ?? "";
                    break;
                case "BIPOrgRole":
                    returnValue = BIPOrgRole ?? "";
                    break;
                case "BIPGrpRole":
                    returnValue = BIPGrpRole ?? "";
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

using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPEPRoleInfo : DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        
        public Nullable<long> EndPointSystemId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string UniqueRoleName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }

        public List<SAPEPUserInfo> SAPEPUserInfos { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public bool? CanRequest { get; set; }
        public string UserDefinedDesc { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "RoleId":
                    returnValue = RoleId ?? "";
                    break;
                case "RoleName":
                    returnValue = RoleName ?? "";
                    break;
                case "RoleDescription":
                    returnValue = RoleDescription ?? "";
                    break;
                case "UniqueRoleName":
                    returnValue = UniqueRoleName ?? "";
                    break;
                case "Version":
                    returnValue = Version.HasValue ? Version.ToString():"";
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

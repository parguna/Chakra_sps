using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPEPGroupInfo : DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        
        public Nullable<long> EndPointSystemId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupUniqueName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }


        public IList<SAPEPUserInfo> SAPEPUserInfos { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "GroupId":
                    returnValue = GroupId ?? "";
                    break;
                case "GroupName":
                    returnValue = GroupName ?? "";
                    break;
                case "GroupDescription":
                    returnValue = GroupDescription ?? "";
                    break;
                case "GroupUniqueName":
                    returnValue = GroupUniqueName ?? "";
                    break;
                case "Version":
                    returnValue = Version.HasValue ? Version.Value.ToString():"";
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

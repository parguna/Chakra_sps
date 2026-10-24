using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationAccountEntitlementAuditTrial : DtoBase, IIDMSReportPropertyExtractor
    {
        public string DisplayName { get; set; }
        public string EndpointName { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public string Permission { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "AccountType":
                    returnValue = AccountType ?? "";
                    break;
                case "DisplayName":
                    returnValue = DisplayName ?? "";
                    break;
                case "EndpointName":
                    returnValue = EndpointName ?? "";
                    break;
                case "UserName":
                    returnValue = UserName ?? "";
                    break;
                case "Permission":
                    returnValue = Permission ?? "";
                    break;
                case "Remarks":
                    returnValue = Remarks ?? "";
                    break;

                case "CreatedDate":
                    returnValue = CreatedDate.ToString("dd-MM-yyyy hh:mm:ss") ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}

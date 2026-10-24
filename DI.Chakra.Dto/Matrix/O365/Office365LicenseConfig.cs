using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class Office365LicenseConfig: DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string AccountSkuId { get; set; }
        public long EndPointSystemId { get; set; }
        public string LicenseDescription { get; set; }
        public string ServicePlan { get; set; }
        public string ServicePlanDescription { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "AccountSkuId":
                    returnValue = AccountSkuId ?? "";
                    break;
                case "LicenseDescription":
                    returnValue = LicenseDescription ?? "";
                    break;
                case "ServicePlan":
                    returnValue = ServicePlan ?? "";
                    break;
                case "ServicePlanDescription":
                    returnValue = ServicePlanDescription ?? "";
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

    [Serializable]
    public class Office365LicenseConfigDto
    {
        public List<ServicePlanDetail> ExistingServicePlanDetail { get; set; }
        public List<AdHocLicenseInfo> AdHocLicenseInfo { get; set; }
    }

    [Serializable]
    public class ProvMassLicenseInfoDetail
    {
        public string License { get; set; }
        public string ServiceName { get; set; }
        public bool IsRemoveLicense { get; set; }
    }

    [Serializable]
    public class ProvMassLicenseInfo
    {
        public List<ProvMassLicenseInfoDetail> LicenseDetails { get; set; }
    }
}

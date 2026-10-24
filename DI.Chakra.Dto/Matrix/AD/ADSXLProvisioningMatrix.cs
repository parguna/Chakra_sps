using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class ADSXLProvisioningMatrix : DtoBase, IXLData, IIDMSReportPropertyExtractor
    {
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DivisionCode { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostDistributionCode { get; set; }
        public string OU { get; set; }
        public string DefaultGroups { get; set; }
        public string ValidationMessage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "CompanyCode": return CompanyCode ?? "";
                case "DepartmentCode": return DepartmentCode ?? "";
                case "DivisionCode": return DivisionCode ?? "";
                case "OrganisationId": return OrganisationId ?? "";
                case "OrganisationName": return OrganisationName ?? "";
                case "OU": return OU ?? "";
                case "DefaultGroups": return DefaultGroups ?? "";
                case "ValidationMessage": return ValidationMessage ?? "";
                default:
                    return "";
            }
        }
    }
    public interface IIDMSReportPropertyExtractor
    {
        string GetReportPropertyValue(string propertyName, string format);
    }
}

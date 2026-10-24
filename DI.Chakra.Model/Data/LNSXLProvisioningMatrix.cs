using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_XLProvMatrix")]
    public class LNSXLProvisioningMatrix : IXLData
    {
        [Key]
        public long Id
        {
            get;
            set;
        }
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DivisionCode { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostDistributionCode { get; set; }
        public string MailServer { get; set; }
        public string CertifierIdFile { get; set; }
        public string CertifierOU { get; set; }
        public string DefaultGroups { get; set; }
        public string PolicyName { get; set; }
        public string ReplicatSever { get; set; }
        public DateTime? ValidFrom
        {
            get;
            set;
        }

        public DateTime? ValidTo
        {
            get;
            set;
        }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
    }
}

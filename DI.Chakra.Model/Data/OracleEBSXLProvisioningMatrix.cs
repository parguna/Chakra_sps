using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_OEBS_XLProvMatrix")]
    public class OracleEBSXLProvisioningMatrix : IXLData
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
        public string Roles { get; set; }
        public string Responsibilities { get; set; }
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

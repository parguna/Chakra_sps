using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_ADS_XLProvMatrix")]
    public class ADSXLProvisioningMatrix : EntityBase, ICreatable, IUpdatable
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
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

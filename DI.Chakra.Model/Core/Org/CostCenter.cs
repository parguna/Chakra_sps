using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_CostCenter")]
    public class CostCenter 
    {

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long CompanyId { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterText { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
      
    }
}

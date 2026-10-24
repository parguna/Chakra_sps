using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Company")]
    public class Company 
    {

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
      
    }
}

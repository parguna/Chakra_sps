using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Division")]
    public class Division 
    {

        [Key]
        public long Id { get; set; }
        public long DepartmentId { get; set; }
        public string TenantId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionText { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
      
    }
}

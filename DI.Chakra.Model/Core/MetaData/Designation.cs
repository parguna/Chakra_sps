using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Designation")]
    public class Designation 
    {
        [Key]
        public long Id { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationDesc { get; set; }
        public bool IsActive { get; set; }
    }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_M_SubTask")]
    [Serializable]
    public class SubTask
    {
        [Key]
        public long Id { get; set; }
        public string RoleName { get; set; }
        public string TCode { get; set; }
        public string TCodeDescription { get; set; }
        public string Remarks { get; set; }

        public bool IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }

        public long OwnerId;

    }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_RoleMatrixDetail")]
    public class RoleMatrixDetail
    {

        [Key]
        public long Id { get; set; }
        public long RoleMatrixId { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public bool IsFromDefault { get; set; }
        public bool IsActive { get; set; }

        public virtual ProvisioningForm Form { get; set; }

    }
}

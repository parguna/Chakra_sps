using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_VPN_Master")]
    public class VPNMaster : IXLData
    {
        [Key]
        public long Id { get; set; }
        public string ApplicationName { get; set; }
        public string ProductionADGroups { get; set; }
        public string QAADGroups { get; set; }
        public string DevADGroups { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

    }
}

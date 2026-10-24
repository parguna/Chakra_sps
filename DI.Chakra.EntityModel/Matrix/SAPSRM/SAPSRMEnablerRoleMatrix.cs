using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SRMEnablerRoleMatrix")]
    public class SAPSRMEnablerRoleMatrix: EntityBase, ICreatable, IUpdatable
    {
        public string SRMEnablerRule { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public string SRMTechRole { get; set; }
        public string BIPOrgRole { get; set; }
        public string BIPGrpRole { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SODConflictingRole")]
    public class SAPSODConflictingRole : EntityBase, ICreatable, IUpdatable
    {
        public string BusinessRole { get; set; }
        public string ConflictingBusinessRole { get; set; }
        public string TechnicalRole { get; set; }
        public string ConflictingTechnicalRole { get; set; }
        public string ConflictingRemarks { get; set; }
        public string SODOwner { get; set; }
        public string EndpointSystemName { get; set; }
        public Nullable<bool> IsModified { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public string FileName { get; set; }
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

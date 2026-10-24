using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Organisation")]
    public class Organisation : IAggregateRoot
    {

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public string OrgUnitPrefix { get; set; }
        public string OrganisationName { get; set; }
        public string ParentOrganisation { get; set; }
        public string Scope { get; set; }
        public long? Head { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}

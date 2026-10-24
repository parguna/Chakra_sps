using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_RoleTypes")]
    public class RoleTypes : EntityBase,ICreatable,IUpdatable
    {
        public string DisplayName { get; set; }
        public string RoleTypeDesc { get; set; }

        public string ApplicableApprovalUserTypes { get; set; }

        public string ApplicableRequestTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}

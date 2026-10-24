using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ApprovalUserTypes")]
    public class ApprovalUserTypes : EntityBase,ICreatable,IUpdatable
    {
        public string ApprovalUserTypeDesc { get; set; }

        public string ApplicableWorkFlowTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}

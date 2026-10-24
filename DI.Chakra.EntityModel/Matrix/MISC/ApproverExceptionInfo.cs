using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_XLApproverExceptionInfo")]
    public class ApproverExceptionInfo: EntityBase, ICreatable, IUpdatable
    {
        public string Person { get; set; }
        public string Approver { get; set; }
        public int ApprovalLevel { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}

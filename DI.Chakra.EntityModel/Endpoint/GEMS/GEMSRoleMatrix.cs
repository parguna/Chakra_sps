using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.EntityModel
{
    [Table("DI_GEMS_RoleMatrix")]
    public class GEMSRoleMatrix : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        [MaxLength(250)]
        public string BusinessRoleName { get; set; }
        [MaxLength(250)]
        public string TechnicalRole { get; set; }
        public string ADGroups { get; set; }
        [MaxLength(250)]
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        [MaxLength(250)]
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        [MaxLength(250)]
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}

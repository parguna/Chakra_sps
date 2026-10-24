using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_XLPositionsMatrix")]
    public class SAPHANAXLPositionsMatrix : EntityBase, ICreatable, IUpdatable
    {
        public long EndpointSystemId { get; set; }
        public string Application { get; set; }
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string Position { get; set; }
        
        public string Prefix_Employees { get; set; }
        public string Prefix_TempStaff { get; set; }
        public string User_Group { get; set; }
        public string Department_Field { get; set; }
        [MaxLength(250)]
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        [MaxLength(250)]
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        [MaxLength(250)]
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public string BusinessRole
        {
            get { return this.PositionText; }
        }
        [NotMapped]
        public string SystemName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

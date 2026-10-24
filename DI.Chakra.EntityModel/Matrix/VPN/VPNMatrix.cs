using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.EntityModel
{
    [Table("DI_VPN_Matrix")]
    public class VPNMatrix : EntityBase, ICreatable, IUpdatable
    {
        [Required]
        public string Name { get; set; }
        public string ProductionADgroups { get; set; }
        public string QAADGroups { get; set; }
        public string DevelpomentADGroups { get; set; }
        public string ADGroups { get; set; }
        public VPNMatrixType VPNMatrixType { get; set; }
        public bool IsActive { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
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

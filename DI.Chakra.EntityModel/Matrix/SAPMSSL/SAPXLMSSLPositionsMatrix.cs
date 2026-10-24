using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_MSSL_XL_PositionsMatrix")]
    public class SAPXLMSSLPositionsMatrix : EntityBase, ICreatable, IUpdatable
    {
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string Position_MSP { get; set; }
        public string Position_MCP { get; set; }
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

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.None);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

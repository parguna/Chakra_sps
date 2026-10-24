using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_SAP_XLPositionsMatrix")]
    public class SAPPositionsMatrix : IXLData, IApprover
    {
        [Key]
        public long Id { get; set; }
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string PositionCode { get; set; }

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
                return "SAP";
            }
        }
    }
}

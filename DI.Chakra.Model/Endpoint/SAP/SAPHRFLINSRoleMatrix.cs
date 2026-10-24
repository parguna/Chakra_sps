using DeepIdentity.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeepIdentity.Model
{
    [Table("DI_SAP_HRFLINSRoleMatrix")]
    public class SAPHRFLINSRoleMatrix
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Remarks { get; set; }
        [MaxLength(250)]
        public string Application { get; set; }
        [MaxLength(250)]
        public string Company { get; set; }
        [MaxLength(250)]
        public string CostCenter { get; set; }
        [MaxLength(250)]
        public string PurchasingOrg { get; set; }
        [MaxLength(250)]
        public string PurchasingGrp { get; set; }
        [MaxLength(250)]
        public string Plant { get; set; }
        [MaxLength(250)]
        public string MRPControl { get; set; }
        [MaxLength(250)]
        public string StorageLocation { get; set; }
        [MaxLength(250)]
        public string Module { get; set; }
        [MaxLength(250)]
        public string SubModule { get; set; }
        [MaxLength(250)]
        public string BusinessRole { get; set; }
        [MaxLength(250)]
        public string TechnicalRule { get; set; }
        [MaxLength(250)]
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }

        [MaxLength(250)]
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        [MaxLength(250)]
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        [MaxLength(250)]
        public string MainSupp { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public Nullable<bool> IsUpdatedbyExcelUpload { get; set; }
        public Nullable<int> ReferenceId { get; set; }
    }
}

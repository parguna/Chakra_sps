using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{


    [Table("DI_SAP_BIRoleCategories")]
    public class SAPBIRoleCategory : EntityBase, ICreatable, IUpdatable
    { 
        [MaxLength(250)]
        public string Remarks { get; set; }
        [MaxLength(250)]
        public string Application { get; set; }
        [MaxLength(250)]
        public string Module { get; set; }
        [MaxLength(250)]
        public string SubModule { get; set; }
        [MaxLength(250)]
        public string BusinessRole { get; set; }
        [MaxLength(250)]
        public string TechnicalRole { get; set; }
        [MaxLength(250)]
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        [MaxLength(250)]
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        [MaxLength(250)]
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        //public Nullable<bool> IsModifiedbyExcelUpload{get;set;}

        //public Nullable<int> ReferenceId { get; set; }

        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.FLINS_BI);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

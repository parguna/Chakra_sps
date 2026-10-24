using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel 
{
    [Table("DI_SAP_GRCRole")]
    public class SAPGRCRole : EntityBase, ICreatable, IUpdatable
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
        public string BusinessRoleName { get; set; }
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
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        
        public string BusinessRole
        {
            get { return this.BusinessRoleName; }
        }
       
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SAP_GRC);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

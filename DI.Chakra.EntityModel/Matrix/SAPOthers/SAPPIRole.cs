using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_PIRole")]
    public class SAPPIRole : EntityBase, ICreatable, IUpdatable
    {
        public string Remarks { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string BusinessRoleName { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public bool? IsModified { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
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
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SAP_PI);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

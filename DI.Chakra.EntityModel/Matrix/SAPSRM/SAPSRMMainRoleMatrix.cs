using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SRMMainRoleMatrix")]
    public class SAPSRMMainRoleMatrix : EntityBase, ICreatable, IUpdatable
    {
        public string RoleType { get; set; }
        public string BusinessRole { get; set; }
        public string SRMRoles { get; set; }
        public string SUSRoles { get; set; }
        public string SRMEnablerRule { get; set; }
        public string BIRoles { get; set; }
        public string BIEnablerRule { get; set; }
        public string InternalPortalRoles { get; set; }
        public string InternalPortalGroups { get; set; }
        public string ExternalPortalRoles { get; set; }
        public string ExternalPortalGroups { get; set; }
        public string ADGroups { get; set; }
        public string DepartmentCode { get; set; }
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
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SRM);
            }
        }
    }
}

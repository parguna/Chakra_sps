using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_EBS_XL_RolesMatrix")]
    public class SAPEBSXLRoleMatrix : EntityBase, ICreatable, IUpdatable
    {
        public string Remarks { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string RoleName { get; set; }
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

        public DateTime? ValidFrom
        {
            get;
            set;
        }

        public DateTime? ValidTo
        {
            get;
            set;
        }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public string BusinessRole
        {
            get { return this.RoleName; }
        }

        public string SystemName
        {
            get
            {
                ApplicationType applicationType;
                if (this.Application == Constants.EBSISP)
                {
                    applicationType = ApplicationType.EBS_ISU;
                }
                else if (this.Application == Constants.EBSCRP)
                {
                    applicationType = ApplicationType.EBS_CRM;
                }
                else
                {
                    applicationType = ApplicationType.EBS_CRM;
                    //applicationType = ApplicationType.EBS_BI;
                }
                return UtilityMethods.GetEnumAttribute<Description>(applicationType);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

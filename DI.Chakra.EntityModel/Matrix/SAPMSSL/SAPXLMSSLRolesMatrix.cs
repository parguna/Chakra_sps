using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_MSSL_XL_RolesMatrix")]
    public class SAPXLMSSLRolesMatrix: EntityBase, ICreatable, IUpdatable
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
                if (this.Application == Constants.MSSLMSP)
                {
                    applicationType = ApplicationType.MSSL_ISU;
                }
                else if (this.Application == Constants.MSSLMCP)
                {
                    applicationType = ApplicationType.MSSL_CRM;
                }
                else
                {
                    //applicationType = ApplicationType.MSSL_BI;
                    applicationType = ApplicationType.MSSL_CRM; // to be changed
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

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    public class SAPPIRole: DtoBase, IXLData, IApprover
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
        public bool IsValid { get; set; }
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
    }
}

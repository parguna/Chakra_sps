using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationInhouseSystemEntitlements : EntityBase
    {
        public long InhouseSystemEntitlementId { get; set; }
        public int InhouseSystemsId { get; set; }
        public int EndPointSystemID { get; set; }
        public string InhouseSystemsCode { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public string ApplicationOwner { get; set; }
        public string EntitlementName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool CanRequestMultiple { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
    }
}

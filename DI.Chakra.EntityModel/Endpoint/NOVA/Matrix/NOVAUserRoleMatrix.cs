using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_NOVA_UserRoleMatrix")]
    public class NOVAUserRoleMatrix : EntityBase, ICreatable, IUpdatable, INovaMatrix
    {
        public string JobDescription { get; set; }
        public long EndPointSystemId { get; set; }
        public string CompanyCode { get; set; }
        public string SectionCode { get; set; }
        public string Position { get; set; }
        public string FLINSRoles { get; set; }
        public string BWAccess { get; set; }
        public string ADGroups { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [NotMapped]
        public bool IsExclusiveForUserAccount { get; set; }
        [NotMapped]
        public string Department { get; set; }
    }

    public interface INovaMatrix
    {
        string Department { get; set; }
        string CompanyCode { get; set; }
        string SectionCode { get; set; }
        string Position { get; set; }
        string FLINSRoles { get; set; }
        string BWAccess { get; set; }
        string ADGroups { get; set; }

        string JobDescription { get; set; }
        DateTime? ValidTo { get; set; }
        bool IsExclusiveForUserAccount { get; set; }
    }
}

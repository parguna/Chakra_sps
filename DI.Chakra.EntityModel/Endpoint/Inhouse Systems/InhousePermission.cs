using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_INH_Permission")]
    public class InhousePermission : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [MaxLength(250)]
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        [MaxLength(250)]
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        [MaxLength(250)]
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }


        [NotMapped]
        public DateTime? ValidFrom { get; set; }
        [NotMapped]
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public virtual EndPoint EndPoint { get; set; }

        public string BusinessRole
        {
            get { return this.Name; }
        }

        public string SystemName
        {
            get { return this.InhouseSystemCode; }
        }

        public string UserDefinedDesc { get; set; }
        public string ADGroups { get; set; }
        public string EPGroups { get; set; }
        public string EPRoles { get; set; }
        public string CEGroups { get; set; }
        public string CERoles { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_SAPSRMBusinessRoleMapping")]
    public class SAPSRMBusinessRoleMapping: EntityBase, ICreatable, IUpdatable
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public AccountType AccountType { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [NotMapped]
        public String DisplayName { get; set; }
        [NotMapped]
        public DateTime? ValidFrom { get; set; }
        [NotMapped]
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public string EmployeeId { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public bool IsEditMode { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_INH_Functions")]
    public class InhouseFunction: EntityBase, ICreatable, IUpdatable
    {
        public long EndpointSystemId { get; set; }
        public string FunctionName { get; set; }
        public string PermissionName { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

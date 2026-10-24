using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_INH_UserPermissionFunction")]
    public class InhUserPermissionFunction : EntityBase, ICreatable, IUpdatable
    {
        public long EndpointUserId { get; set; }
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        [NotMapped]
        public string EmployeeId { get; set; }
        //[NotMapped]
        //public string NRIC { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        public long PermissionId { get; set; }
        public long FunctionId { get; set; }
        [NotMapped]
        public string Permission { get; set; }
        public string Functions { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_INH_Configuration")]
    public class InhouseSystemConfiguration : IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public bool CanRequestMultipleRoles { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual EndPoint EndpointSystem { get; set; }
    }
}

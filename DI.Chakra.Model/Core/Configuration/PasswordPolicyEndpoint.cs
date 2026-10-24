using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_PasswordPolicyEndpoint")]
    public class PasswordPolicyEndpoint : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long PolicyId { get; set; }
        public long EndpointSystemId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint Endpoint { get; set; }
        [ForeignKey("PolicyId")]
        public virtual PasswordPolicy PasswordPolicy { get; set; }
    }
}

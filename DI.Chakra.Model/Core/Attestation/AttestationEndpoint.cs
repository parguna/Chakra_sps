using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Attestation_Endpoint")]
    public class AttestationEndpoint : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long AttestationId { get; set; }
        public EndpointType EndpointSystemType { get; set; }
        public long EndpointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("AttestationId")]
        public virtual Attestation Attestation { get; set; }
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndPoint { get; set; }
    }
}

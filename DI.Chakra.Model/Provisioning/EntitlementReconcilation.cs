using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_EntitlementReconcilation")]
    public class EntitlementReconcilation : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long AccountReconcilationId { get; set; }
        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public byte Version { get; set; }
        public int EntitlementType { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsManuallyAdded { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
    }
}

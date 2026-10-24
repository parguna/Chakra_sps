using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_AccountReconcilation")]
    public class AccountReconcilation : IAggregateRoot
    {

        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public byte Version { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsManuallyAdded { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
        public bool IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public MappingType MappingType { get; set; }
        public OffboardingAccountActionType OffboardingAccountAction { get; set; }

        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndPoint { get; set; }
    }
}

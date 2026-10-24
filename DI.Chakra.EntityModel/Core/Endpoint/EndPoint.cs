using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_EndpointSystem")]
    public class EndPoint : EntityBase,ICreatable,IUpdatable
    {
        public string Description { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public string DisplayName { get; set; }

        public long? OwnerUserId { get; set; }

        public string ApplicationCode { get; set; }

        public bool IsActive { get; set; }

        public bool IsTrustedSource { get; set; }

        public bool IsPasswordResetAllowed { get; set; }
        public bool IsUnlockAllowed { get; set; }

        public bool IncludeInAttestation { get; set; }
        public bool CanAutomate { get; set; }
        public bool IsFunctionLevel { get; set; }

        public string UserObjectName { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("OwnerUserId")]
        public User Owner { get; set; }

        public IList<AttributeMappingColumns> ListAttributeMappingColumns { get; set; }

        [ForeignKey("EndpointSystemTypeId")]
        public EndPointSystemType EndPointSystemType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool IsSodApplicable { get; set; }
        public bool SupportSecondaryAccount { get; set; }
    }

    [NotMapped]
    public class PresentationOtherSystemData
    {
        public long PermissionId { get; set; }
        public string Permission { get; set; }
        public string FullName { get; set; }
        public string EmployeeId { get; set; }
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public MappingType MappingType { get; set; }
        public string Entitlement { get; set; }
        public string Description { get; set; }

    }
    [NotMapped]
    public class PresentationOtherSystemUser
    {
        public long AccountReconcilationId { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string DisplayName { get; set; }
        public long EndpointUserId { get; set; }
        public long PermissionId { get; set; }
        public AccountType AccountType { get; set; }
    }
}

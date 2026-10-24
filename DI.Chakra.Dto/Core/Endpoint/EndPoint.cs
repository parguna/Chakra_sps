using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndPoint : DtoBase
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

        public User Owner { get; set; }

        public List<AttributeMappingColumns> ListAttributeMappingColumns { get; set; }

        public EndPointSystemType EndPointSystemType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool IsSodApplicable { get; set; }
        public bool SupportSecondaryAccount { get; set; }
        public string OwnerName { get; set; }
    }

    [Serializable]
    public class AccountStatus
    {
        public bool IsLocked { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class BooleanStatus
    {
        public bool CanProceed { get; set; }
        public string Message { get; set; }
         
    }
}

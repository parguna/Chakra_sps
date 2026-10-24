using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AccountReconcilation : DtoBase
    {
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public DateTime? AccountCreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }
        public bool IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public MappingType MappingType { get; set; }
        public OffboardingAccountActionType OffboardingAccountAction { get; set; }
        public string ProvisionedThrough { get; set; }

        public EndPoint EndPoint { get; set; }
        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Datetime at which this account was last reviewed via attestation.
        /// </summary>
        public DateTime? LastAttestedOn { get; set; }

        /// <summary>
        /// Endpoint Last Login data
        /// </summary>
        public DateTime? LastLogonDate { get; set; }

        public DateTime? ValidFromDate { get; set; }

        public DateTime? ValidToDate { get; set; }
        public long? AccessRequestId { get; set; }
        public DateTime? ExceptionOccuredDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }


    [Serializable]
    public class AccountReconDto
    {
        public Dictionary<long,string> AccountReconDictionary { get; set; }
        public List<EntitlementReconcilation> EntitlementReconcilations { get; set; }
    }
    [Serializable]
    public class AccountEntitlementRecon
    {
        public AccountReconcilation AccountRecon { get; set; }

        public List<EntitlementReconcilation> Entitlement { get; set; }

    }
}

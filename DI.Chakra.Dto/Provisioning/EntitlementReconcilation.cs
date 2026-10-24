using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class EntitlementReconcilation : DtoBase
    {
        public long AccountReconcilationId { get; set; }
        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public long EntitlementId { get; set; }
        public int EntitlementType { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string Functions { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }

        public string ProvisionedThrough { get; set; }

        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public AccountReconcilation AccountReconcilation { get; set; }

        public EndPoint EndPoint { get; set; }

        /// <summary>
        /// Datetime at which this entitlement was last reviewed via attestation.
        /// </summary>
        public DateTime? LastAttestedOn { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public long? AccessRequestId { get; set; }
        public DateTime? ExceptionOccuredDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAttestationEntitlementReviewDetail
    {
        public long Level { get; set; }

        public long BindingLevel { get; set; }

        public long? ParentBindingLevel { get; set; }

        public long KeyId { get; set; }

        public long EndpointSystemId { get; set; }

        public string EndpointSystemName { get; set; }

        public EndpointType EndpointType { get; set; }

        public long SnapshotActionId { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Type { get; set; }

        public string ReviewCategory { get; set; }
        public string DIUserName { get; set; }
        public string AccountName { get; set; }

        public bool Approved { get; set; }

        public bool Rejected { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public string Comments { get; set; }

        public string Entity { get; set; }
        public string ParentEntity { get; set; }

        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }

        public bool IsChanged { get; set; }

		public long? RequesterUserId { get; set; }

        public long RequesteeUserId { get; set; }

        public long EndpointSystemType { get; set; }

        public AdHocActionType AdhocActionType { get; set; }

        public long EntitlementId { get; set; }

        public string EntitlementDetail { get; set; }

        public AccountType AccountType { get; set; }

        public bool IsChangedInPreviousLevel { get; set; }
        public bool IsNextLevelAvailable { get; set; }

        public bool IsDelta { get; set; }
        public bool IsActionable { get; set; }

        public bool IsActionExtendable { get; set; }

        public string LastLogon { get; set; }
               
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
               
        public string Reserved3 { get; set; }
               
        public string Reserved4 { get; set; }
               
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string ActionReserved1 { get; set; }
        public string ActionReserved2 { get; set; }
        public string ActionReserved3 { get; set; }
        public bool AccountStatus { get; set; }
    }
}

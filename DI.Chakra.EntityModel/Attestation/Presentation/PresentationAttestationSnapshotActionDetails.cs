using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAttestationSnapshotActionDetails
    {
        public long Id { get; set; }
        public long AttestationSnapshotId { get; set; }
        public long TaskId { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public Nullable<long> ReviewerId { get; set; }
        public bool IsReviewerGroup { get; set; }
        public bool Expired { get; set; }
        public bool IsChanged { get; set; }
        public bool IsChangedInPreviousLevel { get; set; }
        public bool IsDelta { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActionable { get; set; }
        public bool IsActionExtendable { get; set; }
        public bool IsException { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public Nullable<long> RootSnapshotId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public AttestationRemediationStatus RemediationStatus { get; set; }
        public long SnapShotId { get; set; }
        public long AttestationCampaignId { get; set; }
        public long UserId { get; set; }
        public AccountType AccountType { get; set; }
        public long KeyId { get; set; }
        public string Entity { get; set; }
        /// <summary>
        /// Endpoint User/Entitlement Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Endpoint User/Entitlement Description
        /// </summary>
        public string Description { get; set; }
        public string AccountName { get; set; }
        public string DIUserName { get; set; }
        public string DIUserDisplayName { get; set; }
        public long EndpointId { get; set; }
        public string EndpointName { get; set; }
        public EndpointType EndpointType { get; set; }
        public string ParentEntity { get; set; }
        public Nullable<System.DateTime> LastLogon { get; set; }
        public long Level { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public bool HasRisk { get; set; }
        public bool AccountStatus { get; set; }
        public Nullable<long> SnapShotRootSnapshotId { get; set; }
        public string SnapShotReserved1 { get; set; }
        public string SnapShotReserved2 { get; set; }
        public string SnapShotReserved3 { get; set; }
        public string SnapShotReserved4 { get; set; }
        public string SnapShotReserved5 { get; set; }
        public string SnapShotReserved6 { get; set; }
        public string SnapShotReserved7 { get; set; }
        public string SnapShotReserved8 { get; set; }
        public string SnapShotReserved9 { get; set; }
        public string SnapShotReserved10 { get; set; }
        public string SnapShotCreatedBy { get; set; }
        public System.DateTime SnapShotCreatedDate { get; set; }
        public string SnapShotLastModifiedBy { get; set; }
        public Nullable<System.DateTime> SnapShotLastModifiedDate { get; set; }

        public AttestationSnapshot AttestationSnapshot
        {
            get
            {
                return new AttestationSnapshot()
                {
                    AccountName = this.AccountName,
                    AccountStatus = AccountStatus,
                    AccountType = AccountType,
                    AttestationCampaignId = AttestationCampaignId,
                    CreatedBy = SnapShotCreatedBy,
                    CreatedDate = SnapShotCreatedDate,
                    Description = Description,
                    DIUserDisplayName = DIUserDisplayName,
                    DIUserName = DIUserName,
                    EndpointId = EndpointId,
                    EndpointName = EndpointName,
                    EndpointType = EndpointType,
                    EntitlementType = EntitlementType,
                    Entity = Entity,
                    HasRisk = HasRisk,
                    Id = SnapShotId,
                    KeyId = KeyId,
                    LastLogon = LastLogon,
                    LastModifiedBy = SnapShotLastModifiedBy,
                    LastModifiedDate = SnapShotLastModifiedDate,
                    Level = Level,
                    Name = Name,
                    ParentEntity = ParentEntity,
                    Reserved1 = SnapShotReserved1,
                    Reserved2 = SnapShotReserved2,
                    Reserved3 = SnapShotReserved3,
                    Reserved4 = SnapShotReserved4,
                    Reserved5 = SnapShotReserved5,
                    Reserved6 = SnapShotReserved6,
                    Reserved7 = SnapShotReserved7,
                    Reserved8 = SnapShotReserved8,
                    Reserved9 = SnapShotReserved9,
                    Reserved10 = SnapShotReserved10,
                    RootSnapshotId = SnapShotRootSnapshotId,
                    UserId = UserId
                };
            }
        }
    }
}

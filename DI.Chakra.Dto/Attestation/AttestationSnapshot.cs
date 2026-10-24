using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationSnapshot : DtoBase
    {
        public long AttestationCampaignId { get; set; }
        public long UserId { get; set; }
        public AccountType AccountType { get; set; }
        public long KeyId { get; set; }
        public string Entity { get; set; }
        public string Name { get; set; }
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
        public Nullable<long> RootSnapshotId { get; set; }
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
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}

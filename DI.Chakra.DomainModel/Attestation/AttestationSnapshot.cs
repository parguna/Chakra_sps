using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AttestationSnapshot : DomainModelBase<AttestationSnapshot, EntityModel.AttestationSnapshot>
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
        public AttestationCampaign AttestationCampaign { get; set; }
        public static List<AttestationSnapshot> GetAttestationSnapshotParentObject(long campaignId, long endpointId, long level)
        {
            List<AttestationSnapshot> parentItems = new List<AttestationSnapshot>();
            using (var tran = new DataAccessTransaction(false))
            {
                parentItems = ORMapList(new AttestationSnapshotRepository(tran)
                                                            .Get(snapshot =>
                                                                        snapshot.AttestationCampaignId == campaignId &&
                                                                        snapshot.EndpointId == endpointId &&
                                                                        snapshot.Level == level, true).ToList());

                return parentItems;
            }
        }

        public static long GetMaxLevelInAttestationSnapshotForEndpoint(long campaignId, long endpointId)
        {
                if (campaignId == 0 || endpointId == 0) return 0;
            using (var tran = new DataAccessTransaction(false))
            {
                return (new AttestationSnapshotRepository(tran).GetMaxLevelByCampaignId(campaignId, endpointId));
                                                            //.Get(snapshot =>
                                                            //            snapshot.AttestationCampaignId == campaignId &&
                                                            //            snapshot.EndpointId == endpointId).ToList()).Select(a => a.Level).Max();
            }
        }

        public static long GetMinLevelInAttestationSnapshotForEndpoint(long campaignId, long endpointId)
        {
                if (campaignId == 0 || endpointId == 0) return 0;

            using (var tran = new DataAccessTransaction(false))
            {
                return (new AttestationSnapshotRepository(tran)
                                                            .Get(snapshot =>
                                                                        snapshot.AttestationCampaignId == campaignId &&
                                                                        snapshot.EndpointId == endpointId).ToList()).Select(a => a.Level).Min();
            }
        }

        public static bool IsAttestationSnapshotHasAnEntry(long CampaignId, long EndpointId, EntitlementType EntitlementType, string EntityName, string ParentEntityName, ApplicationType applnType,string reserved1)
        {
            bool IsAttestationSnapshotHasAnEntry = false;
            using (var tran = new DataAccessTransaction(false))
            {
                if(applnType != ApplicationType.SRM && applnType != ApplicationType.EAM)
                {
                    IsAttestationSnapshotHasAnEntry = (new AttestationSnapshotRepository(tran)
                                                            .Get(snapshot =>
                                                                        snapshot.AttestationCampaignId == CampaignId &&
                                                                        snapshot.EndpointId == EndpointId &&
                                                                        snapshot.EntitlementType == EntitlementType &&
                                                                        snapshot.Entity == EntityName &&
                                                                        snapshot.ParentEntity == ParentEntityName)).Any();
                }
                else
                {
                    IsAttestationSnapshotHasAnEntry = (new AttestationSnapshotRepository(tran)
                                                            .Get(snapshot =>
                                                                        snapshot.AttestationCampaignId == CampaignId &&
                                                                        snapshot.EndpointId == EndpointId &&
                                                                        snapshot.EntitlementType == EntitlementType &&
                                                                        snapshot.Entity == EntityName &&
                                                                        snapshot.ParentEntity == ParentEntityName && snapshot.Reserved1 == reserved1)).Any();
                }

                return IsAttestationSnapshotHasAnEntry;
            }
        }
        public static bool IsAllRoleSelectedForSRM(long campaignId,string accountName,string roleName, EntitlementType entitlementType)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotRepository(tran).CanWeRetainSRMRole(campaignId, accountName, roleName, entitlementType);
            }
                
        }
        public static bool IsResevedFiledHasValue(long parentCampaignId, string fieldName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotRepository(tran).IsResevedFiledHasValue(parentCampaignId, fieldName);
            }
        }
    }
}

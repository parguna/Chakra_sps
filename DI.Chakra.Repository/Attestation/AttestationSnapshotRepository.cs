using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class AttestationSnapshotRepository : RepositoryBase<AttestationSnapshot>
    {
        public AttestationSnapshotRepository(DataAccessTransaction transaction) : base(transaction) { }

        

        public AttestationSnapshot GetAttestationSnapshot(string accountName, long endpointSystemId, long campaignId)
        {
            return CurrentContext.AttestationSnapshots.FirstOrDefault(i=> i.AttestationCampaignId == campaignId && i.EndpointId == endpointSystemId && i.AccountName == accountName && i.EntitlementType == Infrastructure.Enums.EntitlementType.Account);
        }
        /// <summary>
        /// This method is used whether attestation review all the role selected or not.
        /// If all selected then no need to retain the business role
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="accountName"></param>
        /// <param name="roleName"></param>
        /// <param name="entitlementType"></param>
        /// <returns></returns>
        public bool CanWeRetainSRMRole(long campaignId, string accountName, string roleName, EntitlementType entitlementType)
        {
            return !(from snapAction in CurrentContext.AttestationSnapshotActions
                     join snap in CurrentContext.AttestationSnapshots on snapAction.AttestationSnapshotId equals snap.Id
                     where snap.AttestationCampaignId == campaignId && snap.AccountName == accountName && snap.Description == roleName && snap.EntitlementType == entitlementType
                     select snapAction.Revoke).All(i => i);
        }

        private static readonly HashSet<string> AllowedReservedFieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Reserved1", "Reserved2", "Reserved3", "Reserved4", "Reserved5",
            "Reserved6", "Reserved7", "Reserved8", "Reserved9", "Reserved10",
            "ActionReserved1", "ActionReserved2", "ActionReserved3"
        };

        public bool IsResevedFiledHasValue(long parentCampaignId, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || !AllowedReservedFieldNames.Contains(fieldName))
                throw new ArgumentException("Unrecognized reserved field name.", nameof(fieldName));

            var parameters = new[] { new System.Data.SqlClient.SqlParameter("@ParentCampaignId", parentCampaignId) };
            var recordCount = CurrentContext.Database.SqlQuery<int>($"Select ISNULL(Sum(CASE WHEN ISNULL([{fieldName}],'')='' THEN 0 ELSE 1 End),0) from DI_M_AttestationSnapshot where AttestationCampaignId in( Select Id from DI_M_AttestationCampaign WHERE ParentCampaign = @ParentCampaignId and [Status]=2)", parameters).FirstOrDefault();
            return recordCount > 0;
        }
        public long GetMaxLevelByCampaignId(long campainId,long endpointId)
        {
            var parameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@CampaignId", campainId),
                new System.Data.SqlClient.SqlParameter("@EndpointId", endpointId)
            };
            return CurrentContext.Database.SqlQuery<long>("select Max(Level) from DI_M_AttestationSnapshot where AttestationCampaignId = @CampaignId and EndpointId=@EndpointId", parameters).FirstOrDefault();
        }
    }
}

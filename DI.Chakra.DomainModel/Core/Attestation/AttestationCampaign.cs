using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class AttestationCampaign : DomainModelBase<AttestationCampaign, EntityModel.AttestationCampaign>
    {
        public long AttestationID { get; set; }
        public Nullable<long> ParentCampaign { get; set; }

        public Guid WorkflowInstanceID { get; set; }

        public string CampaignName { get; set; }

        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }

        public string Description { get; set; }
        public string SplitUsing { get; set; }

        public string Comments { get; set; }
        public AttestationCampaignStatus Status { get; set; }
        public long ChildSplitCount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }

        public string SnapshotFileName { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public Attestation Attestation { get; set; }

        public static AttestationCampaign GetByWorkflowInstanceID(Guid workflowInstanceId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.WorkflowInstanceID == workflowInstanceId).FirstOrDefault());
            }
        }

        public static AttestationCampaign GetByCampaignId(long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var campaign = ORMap(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.Id == campaignId, attpro => attpro.Attestation).FirstOrDefault());
                var attesEndpoints = AttestationEndpoint.GetByAttestationId(campaign.AttestationID);
                foreach (var attesEndpoint in attesEndpoints)
                {
                    campaign.Attestation.AttestationEndpoint.Add(attesEndpoint);
                }
                return campaign;
            }
        }

        public static List<AttestationCampaign> GetChildCampaigns(long campaignId)
        {
            using(var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.ParentCampaign.HasValue &&
                    attpro.ParentCampaign.Value == campaignId &&
                    attpro.Status != AttestationCampaignStatus.IgnoreNoUserFound).ToList());
            }
        }
        public static List<AttestationCampaign> GetChildCampaignsIncludingStatus(long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationCampaignRepository(tran)
                    .GetChildCampaignsIncludingStatus(campaignId).ToList());
            }
        }
        public static AttestationCampaign GetByCampaignName(string campaignName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.CampaignName == campaignName, true).FirstOrDefault());
            }
        }

        public static List<AttestationCampaign> GetAllWithCampaignName(string campaignName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.CampaignName.StartsWith(campaignName) && attpro.ParentCampaign == null, false).ToList()).ToList();
            }
        }

        public static List<AttestationCampaign> GetAllParentCampaign()
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.ParentCampaign == null && attpro.Status != AttestationCampaignStatus.Completed, false).ToList()).ToList();
            }
        }

        public static AttestationCampaign GetByCampaignIdIncludeAttestaion(long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.Id == campaignId, true).FirstOrDefault());
            }
        }
        public static List<AttestationCampaign> GetAllByCampaignId(long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                List<AttestationCampaign> campaigns = new List<AttestationCampaign>();
                var campaign = ORMap(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                        .Get(attpro => attpro.Id == campaignId && attpro.Status != AttestationCampaignStatus.Completed).FirstOrDefault());

                if (campaign != null)
                {
                    if (campaign.ChildSplitCount > 0)
                    {
                        var allCampaign = GetChildCampaigns(campaignId);
                        if (allCampaign.Any())
                        {
                            campaigns.AddRange(allCampaign);
                        }
                    }
                    campaigns.Add(campaign);
                }
                return campaigns;
            }
        }
        public static bool IsAnyChildCampaignInProgress(long parentCampaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                        .Any(attpro => attpro.ParentCampaign == parentCampaignId && attpro.Status == AttestationCampaignStatus.WorkInProgress);

            }
        }

        public static bool UpdateCampaignStatus(long campaignId, AttestationCampaignStatus status,string currentUser = "System")
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [Status] = {(int)status},[LastModifiedDate]=Getdate(),LastModifiedBy='System' WHERE ID = {campaignId}");
        }
        public static bool UpdateCampaignChildCount(long campaignId, long totalchildCount,string currentUser = "System")
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET ChildSplitCount={totalchildCount},[LastModifiedDate]=Getdate(),LastModifiedBy='System' WHERE ID = {campaignId}");
        }
        public static bool UpdateCampaignStatusWithComments(long campaignId, AttestationCampaignStatus status,string comments)
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [Status] = {(int)status},[Comments]='{comments.Replace("'","''")}',[LastModifiedDate]=Getdate(),LastModifiedBy='System' WHERE ID = {campaignId}");
        }
        public static bool UpdateCampaignFileName(long campaignId, string fileName)
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [SnapshotFileName] = '{fileName.Replace("'", "''")}',[LastModifiedDate]=Getdate(),LastModifiedBy='System' WHERE ID = {campaignId}");
        }
        public static bool UpdateCampaignEndDateWithStatus(long campaignId, AttestationCampaignStatus status)
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [EndDate] = GETDATE(),[Status]={(int)status},[LastModifiedDate]=Getdate(),LastModifiedBy='System' WHERE ID = {campaignId}");
        }
        public static bool UpdateCampaignWorkflowInstanceWithStatus(long campaignId,Guid workflowInstanceId, AttestationCampaignStatus status)
        {
            return ExecuteQuery($"UPDATE DI_M_AttestationCampaign SET [Status] = {(int)status},WorkflowInstanceID='{workflowInstanceId}',LastModifiedBy='System',[LastModifiedDate]=Getdate() WHERE ID = {campaignId}");
        }

        public static List<AttestationCampaign> GetParentCampaignForReport()
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationCampaignRepository(tran)
                        .GetAttestationCampaignsForReport().ToList());
            }
        }
        public static List<AttestationCampaign> GetCompletedChildCampaigns(long campaignId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationCampaign>(tran)
                    .Get(attpro => attpro.ParentCampaign.HasValue &&
                    attpro.ParentCampaign.Value == campaignId && (
                    attpro.Status == AttestationCampaignStatus.Remediation ||
                    attpro.Status == AttestationCampaignStatus.Completed)).ToList());
            }
        }
    }
}

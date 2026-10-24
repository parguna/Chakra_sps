using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationCampaign : DtoBase
    {
        public AttestationCampaign()
        {
        }

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
    }
}
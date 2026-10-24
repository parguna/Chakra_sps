using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAttetstationCampaign
    {
        public long AttestationCampaignId { get; set; }
        public Nullable<long> AttestationID { get; set; }
        public string AttestationName { get; set; }
        public string AttestationType { get; set; }
        public Guid WorkflowInstanceID { get; set; }
        public string CampaignName { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public int Status { get; set; }
        public int Level { get; set; }
        public long TotalItem { get; set; }
        public string OverallStatus { get; set; }
        public long CampaignSplit { get; set; }
        public string SnapshotFileName { get; set; }
        public string SnapshotFilePath { get; set; }
        public bool RequiresAttestationRemediation { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationReview : DtoBase 
    {
        public long UserId { get; set; }

        public string Comments { get; set; }

        public string SubmittedBy { get; set; }

        public Nullable<System.DateTime> SubmittedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string ActionId { get; set; }

        public string ActionTakenBy { get; set; }

        public string ActionComments { get; set; }

        public int HistoryId { get; set; }

        public bool IsChanged { get; set; }
        public bool HasRisk { get; set; }
        public AttestationRemediationStatus RemediationStatus { get; set; }

        public long ReviewerId;

        public bool IsGroupTask;

        public virtual List<ReviewEndpointSystem> ReviewEndpointSystems { get; set; }

        public virtual User User { get; set; }
    }
}

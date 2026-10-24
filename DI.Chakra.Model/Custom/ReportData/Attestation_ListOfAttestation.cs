using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class Attestation_ListOfAttestation
    {
        public long attestationId { get; set; }
        public string attestationName { get; set; }
        public string attestationDescription { get; set; }
        public DateTime? attestationProcessStartsAt { get; set; }
        public DateTime? attestationProcessEndsAt { get; set; }
        public string workFlowStatus { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string SubmittedStatus { get; set; }
        public byte  Level { get; set; }
        public long WorkFlowConfigId { get; set; }
        public string attestationReviewers { get; set; }
    }
}

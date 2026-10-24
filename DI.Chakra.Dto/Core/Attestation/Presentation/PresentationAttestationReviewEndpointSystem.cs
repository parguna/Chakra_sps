using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAttestationReviewEndpointSystem
    {
        public string EndPointUserName { get; set; }
        public string HostName { get; set; }
        public string TargetSystem { get; set; }

        public long ReviewEndpointId { get; set; }

        public string Approved { get; set; }

        public string Rejected { get; set; }

        public string Comments { get; set; }
    }
}

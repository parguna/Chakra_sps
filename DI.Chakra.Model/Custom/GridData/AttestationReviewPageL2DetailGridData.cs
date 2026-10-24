using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.GridData
{
    public class AttestationReviewPageL2DetailGridData
    {
        public int EndpointSystemId { get; set; }
        public long L2DetailId { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string AccessName { get; set; }
        public EntitlementType AccessType { get; set; }
        public string AccessDescription { get; set; }
        public string ResourceType { get; set; }
        public string ResourceCompletePath { get; set; }
        public string PreviousAccess { get; set; }
        public bool IsChanged { get; set; }
        public string Comments { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAttestationRequestDetail
    {
        public long RequesterUserId { get; set; }

        public long RequesteeUserId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long EndpointSystem { get; set; }

        public AdHocActionType AdhocActionType { get; set; }

        public AccountType AccountType { get; set; }

        //public AccessRequestStatus RequestStatus { get; set; }

        //public AccessRequestProgress ProgressStatus { get; set; }

        //public AccessRequestSource RequestSource { get; set; }

        //public DateTime? ValidFrom { get; set; }

        //public DateTime? ActivationDate { get; set; }

        public string Comments { get; set; }

        public string EntitlementDetail { get; set; }
    }
}

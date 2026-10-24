using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationADUserDetails
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }

        public string AccountName { get; set; }

        public long? WogUserId { get; set; }
        public string physicalDeliveryOfficeName { get; set; }

        public bool NeedO365License { get; set; }
    }
}

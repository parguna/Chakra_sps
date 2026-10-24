using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.AdHocRequest.Presentation
{
    public class SODAccessRequest
    {
        public long EndPointSystemId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public long RequesteeUserid { get; set; }

        public string EntitlementName { get; set; }

        public AccessItemType AccessItemType { get; set; }

    }
}

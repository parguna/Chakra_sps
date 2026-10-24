using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class EndPointAutoExpiry : DtoBase
    {

        public long EndpointSystemTypeId { get; set; }

        public Nullable<long> EndPointSystemId { get; set; }

        public bool IsAccountAutoExpirySupported { get; set; }

        public bool IsEntitlementAutoExpirySupported { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}

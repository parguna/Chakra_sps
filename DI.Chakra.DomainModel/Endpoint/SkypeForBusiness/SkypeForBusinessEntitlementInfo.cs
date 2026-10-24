using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessEntitlementInfo : IEndpointEntitlementInfo
    {
        public string EntitlementName
        {
            get;set;
        }

        public string EntitlementDesc
        {
            get; set;
        }

        public EntitlementType EntitlementType
        {
            get; set;
        }
        public long EndPointSystemId { get; set; }
        public long Id { get; set; }
    }
}

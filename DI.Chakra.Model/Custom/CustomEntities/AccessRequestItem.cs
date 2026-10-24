using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
   [Serializable]
    public class AccessRequestItem
    {
        public AccessRequestItemType AccessRequestItemType { get; set; }
        public AccessItemType AccessItemType { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public long KeyId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public EndpointType EndpointType { get; set; }
        public string ItemName { get; set; }
        public string ItemDetail { get; set; }
        public string ItemDescription { get; set; }
    }
}

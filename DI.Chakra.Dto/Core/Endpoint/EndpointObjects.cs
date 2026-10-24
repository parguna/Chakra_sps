using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointObjects : DtoBase
    {
        public int EndpointSystemTypeId { get; set; }

        public string ObjectName { get; set; }

        public bool IsTrustedSource { get; set; }
        public List<DiscoveryProcess> DiscoveryProcess { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}

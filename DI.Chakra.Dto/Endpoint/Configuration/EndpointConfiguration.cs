using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointConfiguration : DtoBase
    {
        public Nullable<long> EndpointSystemId { get; set; }

        public bool MapAll { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public List<EndpointConfigurationDetail> ConfigurationDetails { get; set; } 
    }
}

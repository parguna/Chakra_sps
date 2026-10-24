using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointUserViewConfiguration : DtoBase
    {
        public long EndpointSystemTypeId { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public string KeyName { get; set; }
        public string Caption { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndPoint EndpointSystem { get; set; }
        public EndPointSystemType EndpointSystemType { get; set; }
    }
}

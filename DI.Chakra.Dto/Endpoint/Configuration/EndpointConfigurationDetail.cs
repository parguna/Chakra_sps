using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointConfigurationDetail : DtoBase
    {
        public Nullable<long> EndpointConfigurationId { get; set; }

        public long KeyId { get; set; }

        public string Value { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndpointConfiguration EndpointConfiguration { get; set; }

        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

    }

}

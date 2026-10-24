using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointKeyConfiguration : DtoBase
    {
        public long EndpointSystemTypeId { get; set; }

        public long KeyId { get; set; }

        public int Order { get; set; }

        public string DataType { get; set; }

        public string RegExValidationPattern { get; set; }

        public string ClientScript { get; set; }

        public bool IsRequired { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

    }

}

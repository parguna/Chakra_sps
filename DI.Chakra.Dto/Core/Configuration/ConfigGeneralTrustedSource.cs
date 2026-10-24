using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    public class ConfigGeneralTrustedSource : DtoBase
    {
        public string AttributeName { get; set; }

        public EndpointType TrustedSource { get; set; }

        public string TrustedSourceAttribute { get; set; }

        public bool SelfUpdate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}

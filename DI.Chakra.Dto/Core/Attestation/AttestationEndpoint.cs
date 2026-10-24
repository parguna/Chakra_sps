using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace DI.Chakra.Dto
{
    public class AttestationEndpoint : DtoBase
    {
        public long AttestationId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long EndpointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  Attestation Attestation { get; set; }

        public  EndPoint EndPoint { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DI.Chakra.EntityModel
{
    [Serializable]
    [Table("DI_M_Attestation_Endpoint")]
    public class AttestationEndpoint : EntityBase, ICreatable, IUpdatable
    {
        public long AttestationId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long EndpointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("AttestationId")]
        public  Attestation Attestation { get; set; }

        [ForeignKey("EndpointSystemId")]
        public  EndPoint EndPoint { get; set; }
    }
}

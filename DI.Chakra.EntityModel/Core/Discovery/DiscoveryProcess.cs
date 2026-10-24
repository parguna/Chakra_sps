using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_DiscoveryProcess")]
    public class DiscoveryProcess : EntityBase, ICreatable, IUpdatable
    {
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public long Endpointsystemid { get; set; }

        public bool Default { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool DisableDeltaDiscovery { get; set; }

        [ForeignKey("Endpointsystemid")]
        public EndPoint Endpoint { get; set; }

        [DataMember]
        public List<EndpointObjects> EndpointObjects { get; set; }

        public Process process { get; set; }
    }
}

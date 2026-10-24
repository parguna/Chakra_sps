using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndpointObjects")]
    public class EndpointObjects : EntityBase,ICreatable,IUpdatable
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

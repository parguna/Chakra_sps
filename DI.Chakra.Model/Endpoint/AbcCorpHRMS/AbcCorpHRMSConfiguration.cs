using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    [Table("DI_COP_Configuration")]
    public class COPConfiguration : IAggregateRoot, IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public string WebAPI { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }

    }

}

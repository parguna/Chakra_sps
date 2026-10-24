using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Office365_ServicePlan")]
    public class Office365ServicePlan : EntityBase, ICreatable, IUpdatable
    { 
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}

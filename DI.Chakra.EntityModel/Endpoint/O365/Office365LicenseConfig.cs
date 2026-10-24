using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Office365_LicenseConfig")]
    public class Office365LicenseConfig : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public string LicenseDescription { get; set; }
        public string ServicePlan { get; set; }
        public string ServicePlanDescription { get; set; }     
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
     
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}

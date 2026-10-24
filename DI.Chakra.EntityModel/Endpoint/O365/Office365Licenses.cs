
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Office365_LicenseInfo")]
    public class Office365License : EntityBase, ICreatable, IUpdatable
    {      
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public int ActiveUnits { get; set; }
        public int WarningUnits { get; set; }
        public int ConsumedUnits { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<Office365ServicePlan> ServicePlans;

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}

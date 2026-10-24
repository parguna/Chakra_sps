
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    public class Office365License : DtoBase, IEndpointEntitlementInfo
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
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }

    }
}

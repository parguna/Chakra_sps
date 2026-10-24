using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    public class Office365ServicePlan : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
}

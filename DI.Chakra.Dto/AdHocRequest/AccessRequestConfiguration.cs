using System;
 

namespace DI.Chakra.Dto
{
    public class AccessRequestConfiguration : DtoBase
    {
        public long EndpointSystemId { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccountType AccountType { get; set; }
        public DI.Chakra.Infrastructure.Enums.AdHocActionType EnableActionType { get; set; }
        public EndPoint EndPointSystem { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}

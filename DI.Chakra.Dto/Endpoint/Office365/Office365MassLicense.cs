
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{  
    public class Office365MassLicense : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public bool IsMassAdd { get; set; }
        public bool IsCompleted { get; set; }
        public string Licenses { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
    
}

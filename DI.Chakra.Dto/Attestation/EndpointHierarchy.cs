using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Linq;
namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointHierarchy : DtoBase
    {        
        public EndpointType EndpointType { get; set; }

        public Nullable<long> EndpointId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public long EndpointObjectId { get; set; }
        public bool IsActionable { get; set; }
        public bool IsAssociatedWithUser { get; set; }
        public Nullable<long> ParentId { get; set; }
        public long Level { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}

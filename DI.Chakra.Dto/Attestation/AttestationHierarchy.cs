using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AttestationHierarchy : DtoBase
    {
        public Nullable<long> AttestationId { get; set; }
        public Nullable<long> EndpointHierarchyId { get; set; }
        public long EndpointId { get; set; }
     
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActionAllowed { get; set; }
        public AttestationDefaultReviewStatus DefaultAttestationAction { get; set; }
        public bool IsActionExtendable { get; set; }        
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public EndpointHierarchy EndpointHierarchy { get; set; }
    }
}

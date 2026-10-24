using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Infrastructure;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto
{
    public class Office365RoleAssignmentPolicy : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string PolicyId { get; set; }
        public string DistinguishedName { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public string Identity { get; set; }
        public DateTime? WhenCreated { get; set; }
        public bool? IsValid { get; set; }
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

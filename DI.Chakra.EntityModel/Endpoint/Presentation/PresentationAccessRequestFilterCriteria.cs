using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationAccessRequestFilterCriteria
    {
        public AccessItemType AccessType { get; set; }

        public List<EndpointType> ApplicationEndpointSystems { get; set; }

        public List<EndpointType> EntitlementEndpointSystems { get; set; }

        public bool IncludeApplications { get; set; }

        public bool IncludeEntitlements { get; set; }

        public bool RemoveAccess { get; set; }

        public int? EndpointUserId { get; set; }

        public long? DIUserId { get; set; }

        public long? DIRequesterUserId { get; set; }
        
        public string SearchKey { get; set; }

        public AccountType AccountType { get; set; }
    }
}

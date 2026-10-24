using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAccessRequestFilterCriteria
    {
        public PresentationAccessRequestFilterCriteria()
        {
            this.AccessType = AccessItemType.TechnicalRole;
            this.ApplicationEndpointSystems = new List<long>();
            this.EntitlementEndpointSystems = new List<long>();
            this.EntitlementTypes = new List<long>();
            IncludeApplications = true;
            IncludeEntitlements = true;
            RemoveAccess = false;
            SearchKey = string.Empty;
            EndpointUserId = null;
            DIUserId = null;
            DIRequesterUserId = null;
        }

        public AccessItemType AccessType { get; set; }

        public List<long> ApplicationEndpointSystems { get; set; }

        public List<long> EntitlementEndpointSystems { get; set; }

        public List<long> EntitlementTypes { get; set; }
        public bool IncludeApplications { get; set; }

        public bool IncludeEntitlements { get; set; }

        public bool RemoveAccess { get; set; }

        public int? EndpointUserId { get; set; }

        public long? DIUserId { get; set; }

        public long? DIRequesterUserId { get; set; }

        public string SearchKey { get; set; }

        public AccountType AccountType { get; set; }

        public bool IncludeEntitlementTypes { get; set; }
    }
}

using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAccessRequestFilterCriteria
    {
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

        public PresentationAccessRequestFilterCriteria()
        {
            this.AccessType = AccessItemType.TechnicalRole;

            this.ApplicationEndpointSystems = new List<long>();

            this.EntitlementEndpointSystems = new List<long>();

            IncludeApplications = true;

            IncludeEntitlements = true;

            RemoveAccess = false;

            SearchKey = string.Empty;

            EndpointUserId = null;

            DIUserId = null;

            DIRequesterUserId = null;
        }

        public static PresentationAccessRequestFilterCriteria ORMap(EntityModel.Presentation.PresentationAccessRequestFilterCriteria presentationAccessRequestItem)
        {
            if (presentationAccessRequestItem == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccessRequestFilterCriteria, PresentationAccessRequestFilterCriteria>(presentationAccessRequestItem);
        }

        public static List<PresentationAccessRequestFilterCriteria> ORMapList(IList<EntityModel.Presentation.PresentationAccessRequestFilterCriteria> presentationAccessRequestItems)
        {
            return presentationAccessRequestItems.Select(ORMap).ToList();
        }
    }
}

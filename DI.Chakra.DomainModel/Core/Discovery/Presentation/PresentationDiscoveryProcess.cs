using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationDiscoveryProcess : DiscoveryProcess
    {
        public long DiscoveryProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string EndPointSystem { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string ErrorMessage { get; set; }

        public string SchedulerDateTime { get; set; }

        public DateTime? LastRunDate { get; set; }

        public static List<PresentationDiscoveryProcess> GetDiscoveryProcessJobs(int startRow, int PageSize, string filter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new DiscoveryProcessRepository(dataAccessTransaction).GetDiscoveryProcessJobs(startRow, PageSize, filter));
            }
        }

        public static List<PresentationDiscoveryProcess> GetDiscoveryProcessJobsByEndPointSystemId(int endPointSystemId, int startRow, int PageSize, string filter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new DiscoveryProcessRepository(dataAccessTransaction).GetDiscoveryProcessJobsByEndPointSystemId(endPointSystemId, startRow, PageSize, filter));
            }
        }

        public static PresentationDiscoveryProcess ORMap(EntityModel.Presentation.PresentationDiscoveryProcess presentationPagePermissionEntityModel)
        {
            if (presentationPagePermissionEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationDiscoveryProcess, PresentationDiscoveryProcess>(presentationPagePermissionEntityModel);
        }

        public static List<PresentationDiscoveryProcess> ORMapList(IList<EntityModel.Presentation.PresentationDiscoveryProcess> presentationPagePermissionEntityModel)
        {
            return presentationPagePermissionEntityModel.Select(ORMap).ToList();
        }
    }
}

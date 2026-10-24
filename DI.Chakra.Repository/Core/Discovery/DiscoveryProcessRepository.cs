using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class DiscoveryProcessRepository : RepositoryBase<DiscoveryProcess>
    {
        public DiscoveryProcessRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationDiscoveryProcess> GetDiscoveryProcessJobs(int startRow, int PageSize, string filter)
        {

            List<PresentationDiscoveryProcess> jobs = new List<PresentationDiscoveryProcess>();




            jobs = (from discovery in CurrentContext.DiscoveryProcess
                    join endpoint in CurrentContext.EndPoint
                    on discovery.Endpointsystemid equals endpoint.Id
                    join pr in CurrentContext.Process
                    on discovery.Id equals pr.ProcessTypeKeyId
                    join s in CurrentContext.Scheduler
                    on pr.Scheduler.Id equals s.Id
                    where pr.ProcessType == ProcessType.Discovery
                    select new PresentationDiscoveryProcess
                    {
                        ProcessName = discovery.ProcessName,
                        Description = discovery.Description,
                        EndPointSystem = endpoint.DisplayName,
                        occurence = s.Occurence.Value,
                        DiscoveryProcessId = discovery.Id,
                        Status = pr.Status,
                        StartDateTime = pr.ProcessStartAt,
                        EndDateTime = pr.ProcessEndAt,
                        SchedulerDateTime = s.StartDate + " " + s.StartTime,
                        ErrorMessage = (pr.Status == "Completed") ? string.Empty : pr.ErrorMessage,
                        LastRunDate = pr.LastRunOn
                    }).ToList();

            return jobs;
        }


        public List<PresentationDiscoveryProcess> GetDiscoveryProcessJobsByEndPointSystemId(int endPointSystemId, int startRow, int PageSize, string filter)
        {


            List<PresentationDiscoveryProcess> jobs = new List<PresentationDiscoveryProcess>();

            jobs = (from discovery in CurrentContext.DiscoveryProcess
                    join endpoint in CurrentContext.EndPoint
                    on discovery.Endpointsystemid equals endpoint.Id
                    join pr in CurrentContext.Process
                    on discovery.Id equals pr.ProcessTypeKeyId
                    join s in CurrentContext.Scheduler
                    on pr.Scheduler.Id equals s.Id
                    where pr.ProcessType == ProcessType.Discovery
                      && discovery.Endpoint.EndPointSystemType.Id == endPointSystemId
                    select new PresentationDiscoveryProcess
                    {
                        ProcessName = discovery.ProcessName,
                        Description = discovery.Description,
                        EndPointSystem = endpoint.DisplayName,
                        occurence = s.Occurence.Value,
                        DiscoveryProcessId = discovery.Id,
                        Status = pr.Status,
                        StartDateTime = pr.ProcessStartAt,
                        EndDateTime = pr.ProcessEndAt,
                        SchedulerDateTime = s.StartDate + " " + s.StartTime,
                        ErrorMessage = (pr.Status == "Completed") ? string.Empty : pr.ErrorMessage,
                        LastRunDate = pr.LastRunOn
                    }).ToList();

            return jobs;
        }

        public override void Update(DiscoveryProcess discprocess)
        {
            DiscoveryProcess dbDiscoveryProcess = this.GetById(discprocess.Id, r => r.EndpointObjects);

            List<EndpointObjects> deletedEndpointObjects = dbDiscoveryProcess.EndpointObjects.Except(discprocess.EndpointObjects, new IdBaseEqualityComparer<EndpointObjects>()).ToList();

            List<EndpointObjects> addedEndpointObjects = discprocess.EndpointObjects.Except(dbDiscoveryProcess.EndpointObjects, new IdBaseEqualityComparer<EndpointObjects>()).ToList();

            deletedEndpointObjects.ForEach(epobj => dbDiscoveryProcess.EndpointObjects.Remove(epobj));

            foreach (EndpointObjects epobjects in addedEndpointObjects)
            {
                if (CurrentContext.Entry(epobjects).State == System.Data.Entity.EntityState.Detached)
                    CurrentContext.EndpointObjects.Attach(epobjects);

                dbDiscoveryProcess.EndpointObjects.Add(epobjects);
            }

            base.Update(discprocess);
        }




    }
}

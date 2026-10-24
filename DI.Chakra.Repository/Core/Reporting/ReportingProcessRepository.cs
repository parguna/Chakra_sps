using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class ReportingProcessRepository : RepositoryBase<Report>
    {
        public ReportingProcessRepository(DataAccessTransaction transaction) : base(transaction) { }
        public List<PresentationReportProcess> GetReportingJobs(int startRow, int PageSize, string filter)
        {
            List<PresentationReportProcess> jobs = new List<PresentationReportProcess>();

            return ( from report in CurrentContext.Report
                         join pr in CurrentContext.Process
                         on report.Id equals pr.ProcessTypeKeyId
                         join s in CurrentContext.Scheduler
                         on pr.Scheduler.Id equals s.Id
                         where pr.ProcessType == ProcessType.Reporting
                         //  && report.userNameName== userName
                         select new PresentationReportProcess
                         {
                             Name = report.Name,
                             occurence =  s.Occurence.HasValue?s.Occurence.Value:0,
                             ProcessId = report.Id,
                             Status = pr.Status,
                             StartDateTime = s.StartDate + " " + s.StartTime,
                             ErrorMessage = (pr.Status == "Completed") ? string.Empty : pr.ErrorMessage
                         }).ToList();
       
        }

        public List<PresentationReportProcess> GetReportingJobsByReportType(ReportType ReportType, int startRow, int PageSize, string filter)
        {
            List<PresentationReportProcess> jobs = new List<PresentationReportProcess>();

            return( from report in CurrentContext.Report
                        join pr in CurrentContext.Process
                        on report.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Reporting
                          && report.Type== ReportType
                        //  && report.userNameName== userName
                        select new PresentationReportProcess
                        {
                            Name = report.Name,
                            occurence = s.Occurence.Value,
                            ProcessId = pr.Id,
                            Id = pr.ProcessTypeKeyId,
                            Status = pr.Status,
                            StartDateTime = s.StartDate + " " + s.StartTime,
                            ErrorMessage = (pr.Status == "Completed") ? string.Empty : pr.ErrorMessage

                        }).ToList();

           // jobs = query.ToList();

            //return jobs;
        }

    }
}

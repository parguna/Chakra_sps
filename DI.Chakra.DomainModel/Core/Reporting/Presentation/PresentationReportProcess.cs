using AutoMapper;
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
    public class PresentationReportProcess : Report
    {
        public long ProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
        public string ErrorMessage { get; set; }

        public static List<PresentationReportProcess> GetReportingJobs(int startRow, int PageSize, string filter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ReportingProcessRepository(dataAccessTransaction).GetReportingJobs(startRow, PageSize, filter));
            }
        }

        public static List<PresentationReportProcess> GetReportingJobsByReportType(ReportType reportType, int startRow, int PageSize, string filter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ReportingProcessRepository(dataAccessTransaction).GetReportingJobsByReportType(reportType,startRow, PageSize, filter));
            }
        }


        public static PresentationReportProcess ORMap(EntityModel.Presentation.PresentationReportProcess presentationPagePermissionEntityModel)
        {
            if (presentationPagePermissionEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationReportProcess, PresentationReportProcess>(presentationPagePermissionEntityModel);
        }

        public static List<PresentationReportProcess> ORMapList(IList<EntityModel.Presentation.PresentationReportProcess> presentationPagePermissionEntityModel)
        {
            return presentationPagePermissionEntityModel.Select(ORMap).ToList();
        }
    }
}

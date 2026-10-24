using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationReportProcess")]
    public class PresentationReportProcessController : ReportController
    {
        [HttpGet]
        [Route("GetReportingJobs")]
        public List<PresentationReportProcess> GetReportingJobs(int startRow, int PageSize, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationReportProcess.GetReportingJobs(startRow, PageSize, filter));
        }

        [HttpGet]
        [Route("GetReportingJobsByReportType")]
        public List<PresentationReportProcess> GetReportingJobsByReportType(ReportType reportType, int startRow, int PageSize, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationReportProcess.GetReportingJobsByReportType(reportType, startRow, PageSize, filter));
        }

        public PresentationReportProcess DtoMap(DomainModel.Presentation.PresentationReportProcess tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationReportProcess, PresentationReportProcess>(tModel);
        }

        public List<PresentationReportProcess> DtoMapList(IList<DomainModel.Presentation.PresentationReportProcess> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}

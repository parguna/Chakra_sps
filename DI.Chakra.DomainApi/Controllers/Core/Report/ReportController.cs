using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Report")]
    public class ReportController : BaseController<DomainModel.Report, EntityModel.Report, Dto.Report>
    {
        [HttpGet]
        [Route("GetByReportName")]
        public Report GetByReportName(string reportName)
        {
            return DtoMap(DomainModel.Report.GetByReportName(reportName));
        }

        [HttpGet]
        [Route("GetReportId")]

        public long GetReportId(string reportName)
        {
            return DomainModel.Report.GetReportId(reportName);
        }

        [HttpGet]
        [Route("GetReportIncludingAttributes")]
        public Report GetReportIncludingAttributes(long reportId)
        {
            return DtoMap(DomainModel.Report.GetReportIncludingAttributes(reportId));
        }

        [HttpGet]
        [Route("GetReportList")]
        public List<Report> GetReportList(string userName)
        {
            return DtoMapList(DomainModel.Report.GetReportList(userName));
        }


        [HttpGet]
        [Route("DeleteReportProcess")]
        public void DeleteReportProcess(long processId)
        {
            DomainModel.Report.DeleteReportProcess(processId);
        }
    }

}
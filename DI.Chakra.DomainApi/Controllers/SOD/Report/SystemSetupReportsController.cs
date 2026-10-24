using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SystemSetupReports")]
    public class SystemSetupReportsController : ApiController
    {
        [HttpGet]
        [Route("GetComplianceReportResult")]
        public IList<object> GetComplianceReportResult(string ReportId)
        {
            return DomainModel.SystemSetupReports.GetComplianceReportResult(ReportId);
        }
    }
}
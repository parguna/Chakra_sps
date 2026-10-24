using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ReportHistory")]
    public class ReportHistoryController : BaseController<DomainModel.ReportHistory, EntityModel.ReportHistory, Dto.ReportHistory>
    {

        [HttpGet]
        [Route("GetReportHistoryByReportId")]
        public List<ReportHistory> GetReportHistoryByReportId(long reportId)
        {
            return DtoMapList(DomainModel.ReportHistory.GetReportHistoryByReportId(reportId));
        }


     
    }
}

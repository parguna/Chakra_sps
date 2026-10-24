using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ReportAttributes")]
    public class ReportAttributesController : BaseController<DomainModel.ReportAttributes, EntityModel.ReportAttributes, Dto.ReportAttributes>
    {
        [HttpPost]
        [Route("SaveReportAttributes")]
        public void SaveReportAttributes(List<ReportAttributes> reportAttributes)
        {

            DomainModel.ReportAttributes.SaveList(DtoUnMapList(reportAttributes),User.Identity.Name);
        }

        [HttpGet]
        [Route("GetReportAttributes")]
        public List<ReportAttributes> GetReportAttributes(long reportId)
        {
          return DtoMapList(DomainModel.ReportAttributes.GetByReportId(reportId));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODReport")]
    public class SODReportController : BaseController<DomainModel.SODReport, EntityModel.SODReport, Dto.SODReport>
    {
        [HttpGet]
        [Route("GetBySODRunPolicyIdIncludeSodFunctionItem")]
        public List<SODReport> GetBySODRunPolicyIdIncludeSodFunctionItem(long sodrunpolicyid)
        {
            return DtoMapList(DomainModel.SODReport.GetBySODRunPolicyIdIncludeSodFunctionItem(sodrunpolicyid));
        }
    }
}

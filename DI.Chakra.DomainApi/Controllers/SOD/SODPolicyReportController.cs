using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODPolicyReport")]
    public class SODPolicyReportController : BaseController<DomainModel.SODPolicyReport,EntityModel.SODPolicyReport,SODPolicyReport>
    {
        //[HttpGet]
        //[Route("GetBySODRunPolicyIdIncludeSodFunctionItem")]
        //public static List<SODPolicyReport> GetBySODRunPolicyIdIncludeSodFunctionItem(long sodRunPolicyId)
        //{
        //    return DtoMapList(DomainModel.SODPolicyReport.GetBySODRunPolicyIdIncludeSodFunctionItem(sodRunPolicyId));
        //}

        [HttpGet]
        [Route("GetPolicyReport")]
        public List<SODPolicyReport> GetPolicyReport(string UniqueRunCode)
        {
            return DtoMapList(DomainModel.SODPolicyReport.GetBySODRunPolicyIdIncludeSodFunctionItem(UniqueRunCode));
        }

        [HttpGet]
        [Route("GetUniqueRunCodes")]
        public List<string> GetUniqueRunCodes()
        {
            return DomainModel.SODPolicyReport.GetUniqueRunCodes();
        }
    }
}

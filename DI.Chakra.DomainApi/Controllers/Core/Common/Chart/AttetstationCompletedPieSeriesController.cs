using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationcompletedSourcePieSeries")]
    public class AttetstationCompletedPieSeriesController : BaseController<DomainModel.AttestationcompletedSourcePieSeries, EntityModel.AttestationcompletedSourcePieSeries, Dto.AttestationcompletedSourcePieSeries>
    {
        [HttpGet]
        [Route("GetPieChartData")]
        public IList<AttestationcompletedSourcePieSeries> GetPieChartData(string campaignName)
        {
            return DtoMapList(DomainModel.AttestationcompletedSourcePieSeries.GetPieChartData(campaignName));
        }
    }
}
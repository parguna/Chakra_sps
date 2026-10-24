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
    [RoutePrefix("AccessRequestSourcePieSeries")]
    public class AccessRequestSourcePieSeriesController : BaseController<DomainModel.AccessRequestSourcePieSeries, EntityModel.AccessRequestSourcePieSeries, Dto.AccessRequestSourcePieSeries>
    {
        [HttpGet]
        [Route("GetPieChartData")]
        public IList<AccessRequestSourcePieSeries> GetPieChartData()
        {
            return DtoMapList(DomainModel.AccessRequestSourcePieSeries.GetPieChartData());
        }

        [HttpGet]
        [Route("GetPieChartDataUser")]
        public IList<AccessRequestSourcePieSeries> GetPieChartDataUser(long UserId)
        {
            return DtoMapList(DomainModel.AccessRequestSourcePieSeries.GetPieChartDataUser(UserId));
        }
    }
}
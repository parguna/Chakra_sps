
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccessRequestSourceChartData")]
    public class AccessRequestSourceChartDataController : BaseController<DomainModel.AccessRequestSourceChartData, EntityModel.AccessRequestSourceChartData, Dto.AccessRequestSourceChartData>
    {
        [HttpGet]
        [Route("GetSeriesDataByAccessRequestSource")]
        public IList<AccessRequestSourceChartData> GetSeriesDataByAccessRequestSource(AccessRequestSource RequestSource)
        {
            return DtoMapList(DomainModel.AccessRequestSourceChartData.GetSeriesDataByAccessRequestSource(RequestSource));
        }

        [HttpGet]
        [Route("GetAccessRequestSourceByMonth")]
        public IList<AccessRequestSourceChartData> GetAccessRequestSourceByMonth(int Year, int Month, AccessRequestSource RequestSource)
        {
           return DtoMapList(DomainModel.AccessRequestSourceChartData.GetAccessRequestSourceByMonth(Year,Month,RequestSource));
        }
    }
}

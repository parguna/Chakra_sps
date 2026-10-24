using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UILog")]
    public class UILogController : BaseController<DomainModel.UILog, EntityModel.UILog, Dto.UILog>
    {
        [HttpGet]
        [Route("GetAllOrderByLogDate")]
        public PresentationGridPagedData<Dto.UILog> GetAllOrderByLogDate(int? pagesize, int? pagenumber, string FilterExpression, string sortExpression)
        {
            FilterExpression = UtilityMethods.StripLogForgingCharacters(FilterExpression);
            sortExpression = UtilityMethods.StripLogForgingCharacters(sortExpression);
            PresentationGridPagedData<Dto.UILog> result = new PresentationGridPagedData<Dto.UILog>();
            var data = DomainModel.UILog.GetAllOrderByLogDate(pagesize, pagenumber, FilterExpression, sortExpression);
            result.Data = DtoMapList(data.Data);
            result.TotalCount = data.TotalCount;
            return result;
        }

        [HttpGet]
        [Route("GetAllCount")]
        public long GetAllCount(int? pagesize, int? pagenumber, string FilterExpression, string sortExpression)
        {
            FilterExpression = UtilityMethods.StripLogForgingCharacters(FilterExpression);
            sortExpression = UtilityMethods.StripLogForgingCharacters(sortExpression);
            return DomainModel.UILog.GetAllCount(pagesize, pagenumber, FilterExpression, sortExpression);
        }

        [HttpPost]
        [Route("SaveUILog")]
        [AllowAnonymous]
        public void SaveUILog([FromBody] UILog uiLog)
        {
            var domain = Mapper.Map<DomainModel.UILog>(uiLog);
            DomainModel.UILog.SaveUILog(domain, User.Identity.Name);
        }
    }
}

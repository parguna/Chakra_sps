using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPGroupInfo")]
    public class SAPGroupInfoController : BaseController<DomainModel.SAPGroupInfo, EntityModel.SAPGroupInfo, Dto.SAPGroupInfo>
    {
        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public List<SAPGroupInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            return DtoMapList(DomainModel.SAPGroupInfo.GetByEndPointSystemId(endPointSystemId));
        }
    }
}

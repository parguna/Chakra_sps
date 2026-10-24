using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEPUserInfo")]
    public class SAPEPUserInfoController : BaseController<DomainModel.SAPEPUserInfo, EntityModel.SAPEPUserInfo, Dto.SAPEPUserInfo>
    {
        [HttpGet]
        [Route("DoesExists")]
        public  bool DoesExists(string userName, long endpointsystemId)
        {
            return DomainModel.SAPEPUserInfo.DoesExists(userName, endpointsystemId);
        }
    }
}

using DI.Chakra.DomainApi.Controllers;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controller 
{
    [RoutePrefix("GEMSUserInfo")]
    public class GEMSUserInfoController : BaseController<DomainModel.GEMSUserInfo, EntityModel.GEMSUserInfo, Dto.GEMSUserInfo>
    {
        [HttpGet]
        [Route("GetGEMSUsersByDIUserId")]
        public Dictionary<long, string> GetGEMSUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
           return DomainModel.GEMSUserInfo.GetGEMSUsersByDIUserId(userid, endpointsystemid, accountType);
        }
    }
}
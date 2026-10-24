using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PurgeConfig")]
    public class PurgeConfigController : BaseController<DomainModel.PurgeConfig, EntityModel.PurgeConfig, PurgeConfig>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string tableName,long purgeID)
        {
            return DomainModel.PurgeConfig.DoesExists(tableName, purgeID);
        }
    }
}

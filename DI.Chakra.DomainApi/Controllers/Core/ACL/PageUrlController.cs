using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PageUrl")]
    public class PageUrlController : BaseController<DomainModel.PageUrl, EntityModel.PageUrl, Dto.PageUrl>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string url)
        {
            return DomainModel.PageUrl.DoesExists(url);
        }

        [HttpGet]
        [Route("HasAnyWebPage")]
        public bool HasAnyWebPage(long pageUrlId)
        {
            return DomainModel.PageUrl.HasAnyWebPage(pageUrlId);
        }

        [HttpGet]
        [Route("GetByUrl")]
        public Dictionary<long, string> GetByUrl(string url)
        {
            return DomainModel.PageUrl.GetByUrl(url);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOrganisationHead")]
    public class NOrganisationHeadController : BaseController<DomainModel.NOrganisationHead, EntityModel.NOrganisationHead, Dto.NOrganisationHead>
    {
        [HttpGet]
        [Route("IsOrgHead")]
        public bool IsOrgHead(long userId)
        {
            return DomainModel.NOrganisationHead.IsOrgHead(userId);
        }

        [HttpGet]
        [Route("GetOrganisationHead")]
        public long GetOrganisationHead(long orgId)
        {
            return DomainModel.NOrganisationHead.GetOrganisationHead(orgId);
        }
    }
}

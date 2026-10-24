using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Tenant")]
    public class TenantController : BaseController<DomainModel.Tenant, EntityModel.Tenant, Dto.Tenant>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string tenantName)
        {
           return DomainModel.Tenant.DoesExists(tenantName);
        }
        [HttpGet]
        [Route("Name")]
        public string Name()
        {
            return DomainModel.Tenant.Name();
        }

    }
}

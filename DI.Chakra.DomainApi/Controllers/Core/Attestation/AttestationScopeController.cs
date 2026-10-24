using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationScope")]
    public class AttestationScopeController : BaseController<DomainModel.AttestationScope, EntityModel.AttestationScope, Dto.AttestationScope>
    {

    }
}

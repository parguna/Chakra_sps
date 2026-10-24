using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODRunPolicy")]
    public class SODRunPolicyController : BaseController<DomainModel.SODRunPolicy, EntityModel.SODRunPolicy, Dto.SODRunPolicy>
    {

    }
}

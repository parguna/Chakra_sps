using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("OffboardingAccountAction")]
    public class OffboardingAccountActionController : BaseController<DomainModel.OffboardingAccountAction, EntityModel.OffboardingAccountAction, Dto.OffboardingAccountAction>
    {


    }
}

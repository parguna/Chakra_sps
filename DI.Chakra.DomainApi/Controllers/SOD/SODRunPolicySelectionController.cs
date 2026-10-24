using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODRunPolicySelection")]
    public class SODRunPolicySelectionController : BaseController<DomainModel.SODRunPolicySelection, EntityModel.SODRunPolicySelection, Dto.SODRunPolicySelection>
    {
    }
}

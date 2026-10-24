using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODEndPointPolicySelection")]
    public class SODEndPointPolicySelectionController : BaseController<DomainModel.SODEndPointPolicySelection, EntityModel.SODEndPointPolicySelection, Dto.SODEndPointPolicySelection>
    {
    }
}

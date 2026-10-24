using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODExceptionItem")]
    public class SODExceptionItemController : BaseController<DomainModel.SODExceptionItem, EntityModel.SODExceptionItem, Dto.SODExceptionItem>
    {

    }
}

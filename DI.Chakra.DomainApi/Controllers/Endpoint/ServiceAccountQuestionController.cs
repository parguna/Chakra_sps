using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Endpoint
{
    [RoutePrefix("ServiceAccountQuestion")]
    public class ServiceAccountQuestionController : BaseController<DomainModel.ServiceAccountQuestion, EntityModel.ServiceAccountQuestion, Dto.ServiceAccountQuestion>
    {
    }
}
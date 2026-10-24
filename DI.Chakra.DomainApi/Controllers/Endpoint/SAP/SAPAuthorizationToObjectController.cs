using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPAuthorizationToObject")]
    public class SAPAuthorizationToObjectController : BaseController<DomainModel.SAPAuthorizationToObject, EntityModel.SAPAuthorizationToObject, Dto.SAPAuthorizationToObject>
    {
        [HttpGet]
        [Route("GetByObjectCode")]
        public SAPAuthorizationToObject GetByObjectCode(string objectCode, long endPointSystemId)
        {
            return DtoMap(DomainModel.SAPAuthorizationToObject.GetByObjectCode(objectCode, endPointSystemId));
        }
    }
}

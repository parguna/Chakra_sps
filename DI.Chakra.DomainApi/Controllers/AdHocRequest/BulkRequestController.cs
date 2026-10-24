using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.AdHocRequest
{
    [RoutePrefix("BulkRequest")]

    public class BulkRequestController : BaseController<DomainModel.BulkRequest, EntityModel.BulkRequest, Dto.BulkRequest>
    {
        [HttpGet]
        [Route("UpdateStatusWhenResolved")]
        public bool UpdateStatusWhenResolved(long bulkRequestId, string currentUser)
        {
            return DomainModel.BulkRequest.UpdateStatusWhenResolved(bulkRequestId, currentUser);
        }
    }
}
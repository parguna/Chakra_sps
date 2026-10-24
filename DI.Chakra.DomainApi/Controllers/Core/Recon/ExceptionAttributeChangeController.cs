using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ExceptionAttributeChange")]
    public class ExceptionAttributeChangeController : BaseController<DomainModel.ExceptionAttributeChange, EntityModel.ExceptionAttributeChange, ExceptionAttributeChange>
    {
        [HttpGet]
        [Route("GetByReconcilationId")]
        public List<ExceptionAttributeChange> GetByReconcilationId(long ReconcilationId)
        {
            return DtoMapList(DomainModel.ExceptionAttributeChange.GetByReconcilationId(ReconcilationId));
        }
    }
}

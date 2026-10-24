using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODConflict")]
    public class SODConflictController : BaseController<DomainModel.SODConflict, EntityModel.SODConflict, Dto.SODConflict>
    {
        [HttpGet]
        [Route("DeleteBySodConflictCode")]
        public void DeleteBySodConflictCode(string sodConflictCode)
        {
            DomainModel.SODConflict.DeleteBySodConflictCode(sodConflictCode);
        }

        [HttpGet]
        [Route("DeleteConflictByFunction")]
        public void DeleteConflictByFunction(long functionId)
        {
            DomainModel.SODConflict.DeleteConflictByFunction(functionId);
        }
    }
}

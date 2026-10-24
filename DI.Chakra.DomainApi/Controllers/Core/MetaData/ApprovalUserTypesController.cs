using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ApprovalUserTypes")]
    public class ApprovalUserTypesController : BaseController<DomainModel.ApprovalUserTypes, EntityModel.ApprovalUserTypes, Dto.ApprovalUserTypes>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<ApprovalUserTypes> GetByIsActive()
        {
            return DtoMapList(DomainModel.ApprovalUserTypes.GetByIsActive());
        }
    }
}

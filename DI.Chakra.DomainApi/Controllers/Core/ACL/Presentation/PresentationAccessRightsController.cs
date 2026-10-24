using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAccessRights")]
    public class PresentationAccessRightsController : BaseController<DomainModel.PresentationAccessRights, EntityModel.PresentationAccessRights, Dto.PresentationAccessRights>
    {
        [HttpGet]
        [Route("GetExportData")]
        public List<PresentationAccessRights> GetExportData(long roleId)
        {
            return DtoMapList(DomainModel.PresentationAccessRights.GetExportData(roleId));
        }
    }
}
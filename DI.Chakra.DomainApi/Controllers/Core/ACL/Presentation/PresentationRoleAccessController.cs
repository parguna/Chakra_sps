using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationRoleAccess")]
    public class PresentationRoleAccessController : ApiController
    {
        [HttpGet]
        [Route("GetAccessToRoles")]
        public IList<PresentationRoleAccess> GetAccessToRoles()
        {
            return DtoMapList(DomainModel.Presentation.PresentationRoleAccess.GetAccessToRoles());
        }
        public PresentationRoleAccess DtoMap(DomainModel.Presentation.PresentationRoleAccess tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationRoleAccess, PresentationRoleAccess>(tModel);
        }

        public List<PresentationRoleAccess> DtoMapList(IList<DomainModel.Presentation.PresentationRoleAccess> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationSapUserRoleTCodeAuthorization")]
    public class PresentationSapUserRoleTCodeAuthorizationController:AttestationController
    {
        [HttpGet]
        [Route("GetSapUserRoleTCodeAuthorization")]
        public IList<PresentationSapUserRoleTCodeAuthorization> GetSapUserRoleTCodeAuthorization(long taskId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSapUserRoleTCodeAuthorization.GetSapUserRoleTCodeAuthorization(taskId));
        }

        public  PresentationSapUserRoleTCodeAuthorization DtoMap(DomainModel.Presentation.PresentationSapUserRoleTCodeAuthorization tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSapUserRoleTCodeAuthorization, PresentationSapUserRoleTCodeAuthorization>(tModel);
        }

        public  List<PresentationSapUserRoleTCodeAuthorization> DtoMapList(IList<DomainModel.Presentation.PresentationSapUserRoleTCodeAuthorization> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
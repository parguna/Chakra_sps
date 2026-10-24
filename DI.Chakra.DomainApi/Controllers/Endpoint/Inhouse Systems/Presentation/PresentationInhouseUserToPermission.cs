using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationInhouseUserToPermission")]
    public class PresentationInhouseUserToPermissionController : ApiController
    {
        [HttpGet]
        [Route("GetUserToPermissionsReportData")]
        public List<Dto.Presentation.PresentationInhouseUserToPermission> GetUserToPermissionsReportData(long endpointSystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInhouseUserToPermission.GetUserToPermissionsReportData(endpointSystemid));
        }

        public  Dto.Presentation.PresentationInhouseUserToPermission DtoMap(DomainModel.Presentation.PresentationInhouseUserToPermission tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationInhouseUserToPermission, Dto.Presentation.PresentationInhouseUserToPermission>(tModel);
        }

        public  List<Dto.Presentation.PresentationInhouseUserToPermission> DtoMapList(IList<DomainModel.Presentation.PresentationInhouseUserToPermission> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}

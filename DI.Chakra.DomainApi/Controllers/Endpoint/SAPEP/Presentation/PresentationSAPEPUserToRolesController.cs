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
    [RoutePrefix("PresentationSAPEPUserToRoles")]

    public class PresentationSAPEPUserToRoles
    {
        public  PresentationSAPEPUserToRoles DtoMap(DomainModel.Presentation.PresentationSAPEPUserToRoles tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPEPUserToRoles, PresentationSAPEPUserToRoles>(tModel);
        }

        public  List<PresentationSAPEPUserToRoles> DtoMapList(IList<DomainModel.Presentation.PresentationSAPEPUserToRoles> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPEPUserToRolesReportData")]
        public IList<PresentationSAPEPUserToRoles> GetSAPEPUserToRolesReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPEPUserToRoles.GetSAPEPUserToRolesReportData(endpointSystemId));
        }
    }
}

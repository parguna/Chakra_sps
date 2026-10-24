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
    [RoutePrefix("PresentationOrganisation")]
    public class PresentationOrganisationController : ApiController
    {
        [HttpGet]
        [Route("GetAllOrganizationInfo")]
        public List<PresentationOrganisation> GetAllOrganizationInfo()
        {
            return DtoMapList(DomainModel.Presentation.PresentationOrganisation.GetAllOrganizationInfo());
        }
        public PresentationOrganisation DtoMap(DomainModel.Presentation.PresentationOrganisation tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationOrganisation, PresentationOrganisation>(tModel);
        }

        public List<PresentationOrganisation> DtoMapList(IList<DomainModel.Presentation.PresentationOrganisation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
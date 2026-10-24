using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.DomainModel;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOrganisationPresentation")]
    public class NOrganisationPresentationController : ApiController
    {
        [HttpGet]
        [Route("GetOrgChartForDisplay")]
        public List<DI.Chakra.Dto.NOrganisationPresentation> GetOrgChartForDisplay(long orgId)
        {
            var orgChartForDisplay = DomainModel.NOrganisationPresentation.GetOrgChart(orgId);
            List<DomainModel.NOrganisationPresentation> orgChartForDisplayWithOrder = new List<DomainModel.NOrganisationPresentation>();
            orgChartForDisplayWithOrder.Add(orgChartForDisplay.Last());
            orgChartForDisplayWithOrder.AddRange(orgChartForDisplay.Take(orgChartForDisplay.Count - 1));
            return DtoMapList(orgChartForDisplayWithOrder);
        }

        [HttpGet]
        [Route("GetOrgChart")]
        public List<DI.Chakra.Dto.NOrganisationPresentation> GetOrgChart(long orgId)
        {
            return DtoMapList(DomainModel.NOrganisationPresentation.GetOrgChart(orgId));
        }

        [HttpGet]
        [Route("GetUserOrgChart")]
        public List<DI.Chakra.Dto.NOrganisationPresentation> GetUserOrgChart(long UserId)
        {
            return DtoMapList(DomainModel.NOrganisationPresentation.GetUserOrgChart(UserId));
        }
        public DI.Chakra.Dto.NOrganisationPresentation DtoMap(DomainModel.NOrganisationPresentation tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.NOrganisationPresentation, DI.Chakra.Dto.NOrganisationPresentation>(tModel);
        }

        public List<DI.Chakra.Dto.NOrganisationPresentation> DtoMapList(IList<DomainModel.NOrganisationPresentation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationOrgChart")]
    public class PresentationOrgChartController : ApiController
    {
        [HttpGet]
        [Route("GetOrgChartNodes")]
        public List<PresentationOrgChart> GetOrgChartNodes(long usrId,long parentOrgId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationOrgChart.GetOrgChartNodes(usrId, parentOrgId));
        }

        public PresentationOrgChart DtoMap(DomainModel.Presentation.PresentationOrgChart tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationOrgChart, PresentationOrgChart>(tModel);
        }

        public List<PresentationOrgChart> DtoMapList(IList<DomainModel.Presentation.PresentationOrgChart> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}

using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSOUs")]
    public class PresentationADSOUsController :ApiController
    {
        public PresentationADSOUs DtoMap(DomainModel.Presentation.PresentationADSOUs tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationADSOUs, PresentationADSOUs>(tModel);
        }

        public List<PresentationADSOUs> DtoMapList(IList<DomainModel.Presentation.PresentationADSOUs> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public List<PresentationADSOUs> GetByEndPointSystemId(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSOUs.GetByEndPointSystemId(endpointsystemid));
        }
    }
}

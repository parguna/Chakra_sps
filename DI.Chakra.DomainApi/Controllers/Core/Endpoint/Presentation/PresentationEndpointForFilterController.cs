using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationEndpointForFilter")]
    public class PresentationEndpointForFilterController : ApiController
    {

        [HttpGet]
        [Route("GetRequestableEndpoints")]
        public List<DI.Chakra.Dto.Presentation.PresentationEndPointForFilter> GetRequestableEndpoints(bool IsAddEntitlement)
        {
             return DtoMapList(DomainModel.EndPoint.GetRequestableEndpointsAsDict(IsAddEntitlement));
        }

        public DI.Chakra.Dto.Presentation.PresentationEndPointForFilter DtoMap(DomainModel.Presentation.PresentationEndPointForFilter tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationEndPointForFilter, DI.Chakra.Dto.Presentation.PresentationEndPointForFilter>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationEndPointForFilter> DtoMapList(IList<DomainModel.Presentation.PresentationEndPointForFilter> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
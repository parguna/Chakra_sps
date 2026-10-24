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
    [RoutePrefix("PresentationSODEntitlementConflict")]
    public class PresentationSODEntitlementConflictController : ApiController
    {
        [HttpGet]
        [Route("GetSODEntitlementConflicts")]
        public List<PresentationSODEntitlementConflict> GetSODEntitlementConflicts(string Username, long ConflictId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSODEntitlementConflict.GetSODEntitlementConflicts(Username, ConflictId));
        }

        public PresentationSODEntitlementConflict DtoMap(DomainModel.Presentation.PresentationSODEntitlementConflict tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationSODEntitlementConflict, PresentationSODEntitlementConflict>(tModel);
        }

        public List<PresentationSODEntitlementConflict> DtoMapList(IList<DomainModel.Presentation.PresentationSODEntitlementConflict> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
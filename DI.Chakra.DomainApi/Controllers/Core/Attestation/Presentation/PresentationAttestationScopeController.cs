using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAttestationScope")]
    public class PresentationAttestationScopeController : ApiController
    {
        [HttpGet]
        [Route("GetAttestationScopeByAttestationId")]
        public IList<PresentationAttestationScope> GetAttestationScopeByAttestationId(long attestationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttestationScope.GetAttestationScopeByAttestationId(attestationId));
        }

        public PresentationAttestationScope DtoMap(DomainModel.Presentation.PresentationAttestationScope tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAttestationScope, PresentationAttestationScope>(tModel);
        }

        public List<PresentationAttestationScope> DtoMapList(IList<DomainModel.Presentation.PresentationAttestationScope> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}

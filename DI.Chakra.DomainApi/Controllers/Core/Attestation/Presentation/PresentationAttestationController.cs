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
    [RoutePrefix("PresentationAttestation")]
    public class PresentationAttestationController : ApiController
    {
        [HttpGet]
        [Route("GetAttestations")]
        public IList<PresentationAttestation> GetAttestations()
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttestation.GetAttestations());
        }

        public PresentationAttestation DtoMap(DomainModel.Presentation.PresentationAttestation tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAttestation, PresentationAttestation>(tModel);
        }

        public List<PresentationAttestation> DtoMapList(IList<DomainModel.Presentation.PresentationAttestation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
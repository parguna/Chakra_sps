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
    [RoutePrefix("PresentationListAttestation")]
    public class PresentationListAttestationController : AttestationController
    {
        [HttpGet]
        [Route("GetAllByEnpointSystemTypeId")]
        public IList<PresentationListAttestation> GetAllByEnpointSystemTypeId(int endpointSystemTypeId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationListAttestation.GetAllByEnpointSystemTypeId(endpointSystemTypeId));
        }

        public  PresentationListAttestation DtoMap(DomainModel.Presentation.PresentationListAttestation tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationListAttestation, PresentationListAttestation>(tModel);
        }

        public  List<PresentationListAttestation> DtoMapList(IList<DomainModel.Presentation.PresentationListAttestation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}

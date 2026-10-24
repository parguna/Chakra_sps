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
    [RoutePrefix("PresentationAttestationEndPoint")]
    public class PresentationAttestationEndPointController : ApiController
    {
        [HttpGet]
        [Route("GetAttestationEndPoints")]
        public IList<PresentationAttestationEndPoint> GetAttestationEndPoints(long AttestationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttestationEndPoint.GetAttestationEndPoints(AttestationId));
        }

        public PresentationAttestationEndPoint DtoMap(DomainModel.Presentation.PresentationAttestationEndPoint tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAttestationEndPoint, PresentationAttestationEndPoint>(tModel);
        }

        public List<PresentationAttestationEndPoint> DtoMapList(IList<DomainModel.Presentation.PresentationAttestationEndPoint> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
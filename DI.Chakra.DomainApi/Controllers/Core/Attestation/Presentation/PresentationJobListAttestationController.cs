using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationJobListAttestation")]
    public class PresentationJobListAttestationController : AttestationController
    {
        [HttpGet]
        [Route("GetAttestationProcessJobs")]
        public IList<PresentationJobListAttestation> GetAttestationProcessJobs(int pageSize, int pageNumber, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationJobListAttestation.GetAttestationProcessJobs(pageSize, pageNumber, filter));
        }
        [HttpGet]
        [Route("GetAttestationJobs")]
        public IList<PresentationJobListAttestation> GetAttestationJobs(string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationJobListAttestation.GetAttestationJobs(filter));
        }

        public PresentationJobListAttestation DtoMap(DomainModel.Presentation.PresentationJobListAttestation tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationJobListAttestation, PresentationJobListAttestation>(tModel);
        }

        public List<PresentationJobListAttestation> DtoMapList(IList<DomainModel.Presentation.PresentationJobListAttestation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

    }
}

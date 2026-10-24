using System;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers.Presentation
{
    [RoutePrefix("PresentationAttestationStatusSummary")]
    public class PresentationAttestationStatusSummaryController : BaseController<DomainModel.Attestation, EntityModel.Attestation, Dto.Attestation>
    {
        public  PresentationAttestationStatusSummary DtoMap(DomainModel.Presentation.PresentationAttestationStatusSummary tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationAttestationStatusSummary, PresentationAttestationStatusSummary>(tModel);
        }

        public  List<PresentationAttestationStatusSummary> DtoMapList(IList<DomainModel.Presentation.PresentationAttestationStatusSummary> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();

        }
        [HttpGet]
        [Route("GetSummaryByEndPointTypeName")]

        public IList<Dto.Presentation.PresentationAttestationStatusSummary> GetSummaryByEndPointTypeName(string endPointTypeName)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttestationStatusSummary.GetSummaryByEndPointTypeName(endPointTypeName));
        }

        [HttpGet]
        [Route("GetTaskSummary")]
        public IList<Dto.Presentation.PresentationAttestationStatusSummary> GetTaskSummary(string endPointTypeName)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttestationStatusSummary.GetTaskSummary(endPointTypeName));
        }
    }
}

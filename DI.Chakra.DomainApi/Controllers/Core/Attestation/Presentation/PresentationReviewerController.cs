using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationReviewer")]
    public class PresentationReviewerController : ApiController
    {
        [HttpGet]
        [Route("GetAttestationReviewers")]
        public List<DI.Chakra.Dto.Presentation.PresentationReviewer> GetAttestationReviewers(long AttestationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationReviewer.GetAttestationReviewers(AttestationId));
        }
        [HttpGet]
        [Route("GetAttestationRejectedReviewers")]
        public List<DI.Chakra.Dto.Presentation.PresentationReviewer> GetAttestationRejectedReviewers(long AttestationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationReviewer.GetAttestationRejectedReviewers(AttestationId));
        }
        public DI.Chakra.Dto.Presentation.PresentationReviewer DtoMap(DomainModel.Presentation.PresentationReviewer tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationReviewer, DI.Chakra.Dto.Presentation.PresentationReviewer>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationReviewer> DtoMapList(IList<DomainModel.Presentation.PresentationReviewer> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
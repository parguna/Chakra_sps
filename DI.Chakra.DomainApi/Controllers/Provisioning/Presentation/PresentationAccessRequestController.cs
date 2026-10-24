using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAccessRequest")]
    public class PresentationAccessRequestController : ApiController
    {
        [HttpGet]
        [Route("GetByProgressStatus")]
        public List<PresentationAccessRequest> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress status, bool isAdminMode)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccessRequest.GetByProgressStatus(status, isAdminMode));
        }

        [HttpGet]
        [Route("GetPagedAllRequestDataByProgressStatus")]
        public List<PresentationAccessRequest> GetPagedAllRequestDataByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus, long requestedFor, string fromDate, string toDate, long endpointSystemId = 0)
        {
            DateTime? dtFrom = UtilityMethods.GetDateFromString(fromDate, Constants.DateFormat);
            DateTime? dtTo = UtilityMethods.GetDateFromString(toDate, Constants.DateFormat);
            return DtoMapList(DomainModel.Presentation.PresentationAccessRequest.GetAllByProgressStatus(progressStatus, requestedFor, dtFrom, dtTo, endpointSystemId));
        }

        public PresentationAccessRequest DtoMap(DomainModel.Presentation.PresentationAccessRequest tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationAccessRequest, PresentationAccessRequest>(tModel);
        }

        public List<PresentationAccessRequest> DtoMapList(IList<DomainModel.Presentation.PresentationAccessRequest> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
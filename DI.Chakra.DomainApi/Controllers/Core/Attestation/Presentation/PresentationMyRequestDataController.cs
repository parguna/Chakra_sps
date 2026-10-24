using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationMyRequestData")]

    public class PresentationMyRequestDataController : ApiController
    {
        [HttpGet]
        [Route("GetByRequesterId")]
        public IList<PresentationMyRequestData> GetByRequesterId(List<long> requesterIDs)
        {
            return DtoMapList(DomainModel.Presentation.PresentationMyRequestData.GetByRequesterId(requesterIDs));
        }
        [HttpGet]
        [Route("GetByAdhocId")]
        public PresentationMyRequestData GetByAdhocId(long requestId, bool deepLoad)
        {
            return DtoMap(DomainModel.Presentation.PresentationMyRequestData.GetByAdhocId(requestId, deepLoad));
        }

        [HttpPost]
        [Route("GetPagedByActionUserId")]
        public Dto.Core.PresentationGridPagedData<PresentationMyRequestData> GetPagedByActionUserId(long actionUserId, Dto.Core.PresentationGridPagedDataParameter param)
        {
            var response = new Dto.Core.PresentationGridPagedData<PresentationMyRequestData>();
            var dmRsult = DomainModel.Presentation.PresentationMyRequestData.GetByActionUserId(actionUserId, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression);
            response.TotalCount = dmRsult.TotalCount;
            response.Data = DtoMapList(dmRsult.Data);
            return response;
        }

        [HttpGet]
        [Route("GetOnlyEventTriggedAccessRequests")]
        public IList<PresentationMyRequestData> GetOnlyEventTriggedAccessRequests()
        {
            return DtoMapList(DomainModel.Presentation.PresentationMyRequestData.GetOnlyEventTriggedAccessRequests());
        }

        [HttpGet]
        [Route("GetByActionUserDepartment")]
        public IList<PresentationMyRequestData> GetByActionUserDepartment(long actionUserId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationMyRequestData.GetByActionUserDepartment(actionUserId));
        }

        [HttpGet]
        [Route("GetByProgressStatus")]
        public IList<PresentationMyRequestData> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus)
        {
            return DtoMapList(DomainModel.Presentation.PresentationMyRequestData.GetByProgressStatus(progressStatus));
        }


        public PresentationMyRequestData DtoMap(DomainModel.Presentation.PresentationMyRequestData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationMyRequestData, PresentationMyRequestData>(tModel);
        }

        public List<PresentationMyRequestData> DtoMapList(IList<DomainModel.Presentation.PresentationMyRequestData> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
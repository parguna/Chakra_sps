using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AdHocAccessRequest")]
    public class AdHocAccessRequestController : BaseController<DomainModel.AdHocAccessRequest, EntityModel.AdHocAccessRequest, Dto.AdHocAccessRequest>
    {
        [HttpPost]
        [Route("GetListofEntitlementByRequesteeUserId")]
        public Dictionary<string, string> GetListofEntitlementByRequesteeUserId(int CurrentRequesteeUserId)
        {
            return DomainModel.AdHocAccessRequest.GetListofEntitlementByRequesteeUserId(CurrentRequesteeUserId);
        }

        [HttpGet]
        [Route("GetEntitlementDetailByEntitlement")]
        public List<KeyValuePair<string, string>> GetEntitlementDetailByEntitlement(long endpointSystemId, string entitlement)
        {
            return DomainModel.AdHocAccessRequest.GetEntitlementDetailByEntitlement(endpointSystemId, entitlement);
        }

        [HttpPost]
        [Route("DoesNewAccountExist")]
        public bool DoesNewAccountExist([FromBody]AdHocAccessRequest adHocAccessRequest)
        {
            return DomainModel.AdHocAccessRequest.DoesNewAccountExist(DtoUnMap(adHocAccessRequest));
        }

        [HttpPost]
        [Route("IsSameRequestPending")]
        public string IsSameRequestPending([FromUri] long requesterId, [FromUri] long accessRequestId, [FromBody]AdHocAccessRequest AdHocAccessRequest)
        {
            return DomainModel.AdHocAccessRequest.IsSameRequestPending(DtoUnMap(AdHocAccessRequest), requesterId, accessRequestId);
        }

        [HttpPost]
        [Route("IsDependentRequestPendingForEBSMSSL")]
        public bool IsDependentRequestPendingForEBSMSSL([FromBody]AdHocAccessRequest adhocAccessRequest, ApplicationType appType)
        {
            return DomainModel.AdHocAccessRequest.IsDependentRequestPendingForEBSMSSL(DtoUnMap(adhocAccessRequest), appType);
        }

        [HttpPost]
        [Route("IsSameRequestPendingSpecialACR")]

        public string IsSameRequestPendingSpecialACR([FromUri] long requesterId, [FromUri] long accessRequestId, [FromBody]AdHocAccessRequest AdHocAccessRequest)
        {
            return DomainModel.AdHocAccessRequest.IsSameRequestPendingSpecialACR(DtoUnMap(AdHocAccessRequest), requesterId, accessRequestId);
        }

        [HttpGet]
        [Route("GetPendingByRequesteeUserId")]
        public List<AdHocAccessRequest> GetPendingByRequesteeUserId(long requesteeUserId)
        {
            return DtoMapList(DomainModel.AdHocAccessRequest.GetPendingByRequesteeUserId(requesteeUserId));
        }
        [HttpPost]
        [Route("GetAccessRequestItemString")]
        public string GetAccessRequestItemString([FromBody]DI.Chakra.Dto.IAccessRequestItem value)
        {
            return DomainModel.AdHocAccessRequest.GetAccessRequestItemString(AutoMapperHelper.DtoUnMap<Dto.IAccessRequestItem,
                DomainModel.IAccessRequestItem>(value));
        }
        [HttpPost]
        [Route("GetAccessRequestItemObject")]
        public Dto.IAccessRequestItem GetAccessRequestItemObject([FromBody]AdHocAccessRequest adHocAccessRequest)
        {
            return AutoMapper.Mapper.Map<DomainModel.IAccessRequestItem, Dto.IAccessRequestItem>(DtoUnMap(adHocAccessRequest).GetAccessRequestItemObject());
        }
        [HttpGet]
        [Route("GetRequestsFromInterimStore")]
        public List<AdHocAccessRequest> GetRequestsFromInterimStore(long userId)
        {
            return DtoMapList(DomainModel.AdHocAccessRequest.GetRequestsFromInterimStore(userId));
        }
        [HttpGet]
        [Route("GetRemoveRequestsFromInterimStore")]
        public List<AdHocAccessRequest> GetRemoveRequestsFromInterimStore(long userId)
        {
            return DtoMapList(DomainModel.AdHocAccessRequest.GetRemoveRequestsFromInterimStore(userId));
        }

        [HttpGet]
        [Route("GetByRequesteeUserId")]
        public AdHocAccessRequest GetByRequesteeUserId(long requesteeUserId, string comments, long endpointSystemId)
        {
            var adhocRequest = DomainModel.AdHocAccessRequest.GetByRequesteeUserId(requesteeUserId, comments, endpointSystemId);
            return DtoMap(adhocRequest);
        }

        [HttpGet]
        [Route("GetEntitlementDetail")]
        public string GetEntitlementDetail(long adhocAccessRequestId)
        {
            var adhocRequest = DomainModel.AdHocAccessRequest.GetEntitlementDetail(adhocAccessRequestId);
            return adhocRequest;
        }

        [HttpGet]
        [Route("GetServiceAccountQuestions")]
        public List<SecurityResponseDetail> GetServiceAccountQuestions()
        {

            var list = DomainModel.ServiceAccountQuestion.Get(q => q.IsActive, false, 0, 0);
            return list.Select(q => new SecurityResponseDetail() { QuestionID = Convert.ToInt16(q.Id), Question = q.Description }).ToList();
        }
    }
}
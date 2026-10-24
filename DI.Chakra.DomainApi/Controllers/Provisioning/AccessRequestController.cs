using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra;
using DI.Chakra.Dto.Presentation;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccessRequest")]
    public class AccessRequestController : BaseController<DomainModel.AccessRequest, EntityModel.AccessRequest, Dto.AccessRequest>
    {
        [HttpGet]
        [Route("IsBulkRequestHasPendingChildRequests")]
        public bool IsBulkRequestHasPendingChildRequests(int bulkRequestId)
        {
            return DomainModel.AccessRequest.IsBulkRequestHasPendingChildRequests(bulkRequestId);
        }

        [HttpGet]
        [Route("GetFailedSystemRequests")]
        public List<FailedRequestGridData> GetFailedSystemRequests(AccessRequestProgress progressStatus)
        {
            return DomainModel.AccessRequest.GetFailedSystemRequests(progressStatus).Select(_ => AutoMapper.Mapper.Map<FailedRequestGridData>(_)).ToList();
        }

        [HttpGet]
        [Route("GetAdhocRequestsByProgressStatus")]
        public List<MyRequestsGridData> GetAdhocRequestsByProgressStatus(AccessRequestProgress progressStatus)
        {
            return DomainModel.AccessRequest.GetAdhocRequestsByProgressStatus(progressStatus).Select(_ => AutoMapper.Mapper.Map<MyRequestsGridData>(_)).ToList();
        }

        [HttpGet]
        [Route("GetYearsByActivationDate")]
        public IList<int> GetYearsByActivationDate()
        {
            return DomainModel.AccessRequest.GetYearsByActivationDate().ToList();
        }
        [HttpGet]
        [Route("GetLastSequenceNumber")]
        public string GetLastSequenceNumber(long endPointSystemId)
        {
            return DomainModel.AccessRequest.GetlastSequenceNumber(endPointSystemId);
        }
        [HttpGet]
        [Route("GetMonthsByActivationDate")]
        public IList<int> GetMonthsByActivationDate()
        {
            return DomainModel.AccessRequest.GetMonthsByActivationDate().ToList();
        }
        [HttpGet]
        [Route("CheckAllRequestCompletedForEmployeeEventTrigger")]
        public bool CheckAllRequestCompletedForEmployeeEventTrigger(long EmployeeEventId)
        {
            return DomainModel.AccessRequest.CheckAllRequestCompletedForEmployeeEventTrigger(EmployeeEventId);
        }
        [HttpGet]
        [Route("GetUserYears")]
        public IList<int> GetUserYears(long UserId)
        {
            return DomainModel.AccessRequest.GetUserYears(UserId);
        }

        [HttpGet]
        [Route("GetUsersMonths")]
        public IList<string> GetUsersMonths(long UserId)
        {
            return DomainModel.AccessRequest.GetUsersMonths(UserId);
        }

        [HttpGet]
        [Route("GetByIdIncludeAdHocAccessRequest")]
        public Dto.AccessRequest GetByIdIncludeAdHocAccessRequest(long id)
        {
            return DtoMap(DomainModel.AccessRequest.GetByIdIncludeAdHocAccessRequest(id));
        }
        [HttpGet]
        [Route("GetByTransactionId")]
        public List<AccessRequest> GetByTransactionId(Guid transactionId)
        {
            return DtoMapList(DomainModel.AccessRequest.GetByTransactionId(transactionId));
        }
        [HttpGet]
        [Route("GetByProgressStatus")]
        public List<AccessRequest> GetByProgressStatus(AccessRequestProgress status, bool isAdminMode)
        {
            return DtoMapList(DomainModel.AccessRequest.GetByProgressStatus(status, isAdminMode));
        }
        [HttpGet]
        [Route("GetStandingOrders")]
        public List<AccessRequest> GetStandingOrders(long endPointSystemId, DateTime FromDate, DateTime ToDate)
        {
            return DtoMapList(DomainModel.AccessRequest.GetStandingOrders(endPointSystemId, FromDate, ToDate));
        }


        [HttpGet]
        [Route("GetRiskAssessment")]
        public List<AccessRequest> GetRiskAssessment(long endPointSystemId, DateTime FromDate, DateTime ToDate)
        {
            return DtoMapList(DomainModel.AccessRequest.GetRiskAssessment(endPointSystemId, FromDate, ToDate));
        }


        [HttpGet]
        [Route("GetByUserId")]
        public List<AccessRequest> GetByUserId(long userId)
        {
            return DtoMapList(DomainModel.AccessRequest.GetByUserId(userId));
        }

        [HttpGet]
        [Route("GetByAdHocAccessRequestId")]
        public AccessRequest GetByAdHocAccessRequestId(long adhocAccessRequestId)
        {
            return DtoMap(DomainModel.AccessRequest.GetByAdHocAccessRequestId(adhocAccessRequestId));
        }

        [HttpGet]
        [Route("GetByAccountType")]
        public List<AccessRequest> GetByAccountType(long userId, Infrastructure.Enums.AccountType accountType, int endpointSystemId)
        {
            return DtoMapList(DomainModel.AccessRequest.GetByAccountType(userId, accountType, endpointSystemId).ToList());
        }

        [HttpPost]
        [Route("SaveAccessRequestList")]
        public List<long> SaveAccessRequestList(List<Dto.AccessRequest> accessRequestList)
        {
            return DomainModel.AccessRequest.SaveAccessRequestList(DtoUnMapList(accessRequestList));
        }

        [HttpPost]
        [Route("SaveAccessRequestListFromAdhocRequest")]
        public List<long> SaveAccessRequestListFromAdhocRequest(List<Dto.AccessRequest> accessRequestList)
        {
           var userIDs =  accessRequestList.Select(t => t.UserId).Distinct();
            foreach (var userId in userIDs)
            {
                var user = DomainModel.User.GetById(userId);
                accessRequestList.Where(t=>t.UserId == userId).Select(t=>t).ToList().ForEach(t => {
                    t.OrgBranch = user.OrgBranch;
                    t.OrgCompany = user.OrgCompany;
                    t.OrgDepartment = user.OrgDepartment;
                    t.OrgDivision = user.OrgDivision;
                    t.OrgSection = user.OrgSection;
                    t.OrgUnit = user.OrgUnit;
                    t.RESPONSETXID = user.Supervisor;
                });
            }
            return DomainModel.AccessRequest.SaveAccessRequestList(DtoUnMapList(accessRequestList));
        }

        [HttpGet]
        [Route("EmailCompose")]
        public bool EmailCompose(long delegateTaskId)
        {
            //new EmailComposer().Compose(EmailTemplate.DelegateAdhocProvisioningApproval, delegateTaskId);
            return true;
        }
        [HttpPost]
        [Route("SaveRemoveRequest")]
        [AllowAnonymous]
        public void SaveRemoveRequest(List<PresentationAttestationEntitlementReviewDetail> accessList, long endpointId)
        {
            DomainModel.AccessRequest.SaveRemoveRequest(DtoUnMapList(accessList), endpointId, User.Identity.Name);
        }

        [HttpGet]
        [Route("GetExtendedRequestById")]
        public IList<Dto.AccessRequest> GetExtendedRequestById(long extendedRequestId)
        {
            return DtoMapList(DomainModel.AccessRequest.GetExtendedRequestById(extendedRequestId));
        }

        [HttpPost]
        [Route("SaveExtendedRequest")]
        [AllowAnonymous]
        public bool SaveExtendedRequest(AccessRequest accessRequest, long endpointId)
        {
            bool isSuccess = DomainModel.AccessRequest.SaveExtendedRequest(DtoUnMap(accessRequest), endpointId);

            return isSuccess;

        }

        [HttpGet]
        [Route("GetAccessRequestById")]
        public AccessRequest GetAccessRequestById(long accessrequestId)
        {
            return DtoMap(DomainModel.AccessRequest.GetById(accessrequestId, true));
        }

        public static DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail DtoUnMap(Dto.Presentation.PresentationAttestationEntitlementReviewDetail dto)
        {
            return Mapper.Map<Dto.Presentation.PresentationAttestationEntitlementReviewDetail, DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail>(dto);
        }

        public static List<DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail> DtoUnMapList(List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> dtoList)
        {
            return dtoList.Select(DtoUnMap).ToList();
        }

        [HttpGet]
        [Route("GetIDASRequestByUserId")]
        public List<AccessRequest> GetIDASRequestByUserId(long userId)
        {
            return DtoMapList(DomainModel.AccessRequest.GetIDASRequestByUserId(userId));
        }
        [HttpGet]
        [Route("CreateADPropagationForEmployee")]
        public string CreateADPropagationForEmployee(long userId)
        {
            return DomainModel.AccessRequest.CreateADPropagationForEmployee(userId);
        }
        [HttpGet]
        [Route("UpdateUserTelephonenumberInHRMS")]
        public bool UpdateUserTelephonenumberInHRMS(long userId)
        {
            return DomainModel.AccessRequest.UpdateUserTelephonenumberInHRMS(userId);
        }
    }
}

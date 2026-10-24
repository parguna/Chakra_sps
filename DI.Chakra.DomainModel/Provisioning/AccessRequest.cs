using DI.Chakra.DomainModel.Presentation;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class AccessRequest : DomainModelBase<AccessRequest, EntityModel.AccessRequest>
    {
        public Nullable<long> RequesterId { get; set; }

        public long UserId { get; set; }

        public string Description { get; set; }

        public Nullable<long> AdHocAccessRequestId { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EmployeeEventTriggerId { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestProgress ProgressStatus { get; set; }

        public DI.Chakra.Infrastructure.Enums.AccessRequestSource RequestSource { get; set; }

        public Nullable<System.DateTime> ActivationDate { get; set; }

        public Nullable<System.DateTime> DeactivationDate { get; set; }

        public Nullable<System.DateTime> ExecutionTime { get; set; }

        public string Remarks { get; set; }

        public string ApproverType { get; set; }

        public Nullable<long> AttestationCampaignId { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Guid TransactionId { get; set; }

        public bool IsCurrentLevelApproved { get; set; }

        public string Comments { get; set; }
        /// <summary>
        /// For Inhouse system for OU Change To maintain AccountName and EndpointUserid
        /// </summary>
        public string AdditionalInfo { get; set; }

        public long? ParentRequestId { get; set; }

        public bool IsParentRequest { get; set; }

        public bool IsDependentRequest { get; set; }

        public bool IsDependentRequestCreated { get; set; }

        public AccountType AccountType { get; set; }
        public ApplicationType ApplicationType { get; set; }

        public Nullable<long> AttestationReviewId { get; set; }

        //user snapshot
        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }

        public AdHocAccessRequest AdHocAccessRequest { get; set; }

        public User User { get; set; }

        public User Requester { get; set; }

        public EndPoint Application { get; set; }

        public string InterimIds { get; set; }

        public string RESPONSETXID { get; set; }
        public Nullable<long> BulkRequestId { get; set; }
        public Nullable<long> EventUndoRequestId { get; set; }

        public EventUndoRequest EventUndoRequest { get; set; }

        public string ConflictsRemarks { get; set; }
        public string RequestSourceString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(
                    (DI.Chakra.Infrastructure.Enums.AccessRequestSource)this.RequestSource);
            }
        }

        public string ProgressStatusString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(
                    (DI.Chakra.Infrastructure.Enums.AccessRequestProgress)this.ProgressStatus);
            }
        }

        public static List<AccessRequest> GetByRemediationProgressStatus(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.ProgressStatus != AccessRequestProgress.Completed &&
                        accessRequest.RequestSource == AccessRequestSource.AttestationRemediation &&
                        accessRequest.AttestationCampaignId.HasValue &&
                        accessRequest.AttestationCampaignId == campaignId).
                        ToList());
            }
        }

        public static List<MyRequestsGridData> GetAdhocRequestsByProgressStatus(AccessRequestProgress progressStatus)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new AccessRequestRepository(dataAccessTransaction).GetAdhocRequestsByProgressStatus(progressStatus).Select(_ => AutoMapper.Mapper.Map<MyRequestsGridData>(_)).ToList();
            }
        }

        public static List<FailedRequestGridData> GetFailedSystemRequests(AccessRequestProgress progressStatus)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var requests = new AccessRequestRepository(dataAccessTransaction).GetFailedSystemRequests(progressStatus).Select(_ => AutoMapper.Mapper.Map<FailedRequestGridData>(_)).ToList();

                if (progressStatus == AccessRequestProgress.Executing)
                {
                    requests.RemoveAll(item => (DateTime.Now - item.RequestDate).TotalMinutes < 30);
                }

                return requests;
            }
        }

        public static bool IsBulkRequestHasPendingChildRequests(int bulkRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new AccessRequestRepository(dataAccessTransaction).IsBulkRequestHasPendingChildRequests(bulkRequestId);
            }
        }

        public static List<AccessRequest> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress status)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        status == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.None ? 1 == 1 :
                        accessRequest.ProgressStatus == status && accessRequest.AdHocAccessRequestId != null,
                        accessRequest => accessRequest.AdHocAccessRequest,
                        accessRequest => accessRequest.User).
                        ToList());
            }
        }
        public static List<AccessRequest> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress status, bool isAdminMode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                     Get(accessRequest =>
                        status == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.None ? 1 == 1 :
                        accessRequest.ProgressStatus == status && (!isAdminMode ? (accessRequest.AdHocAccessRequestId) != null :
                        (accessRequest.RequestSource != AccessRequestSource.AttestationRemediation || accessRequest.RequestSource == AccessRequestSource.None)
                        ),
                        accessRequest => accessRequest.AdHocAccessRequest,
                        accessRequest => accessRequest.User,
                        accessRequest => accessRequest.Application).
                        ToList());
            }
        }
        public static List<AccessRequest> GetByTransactionId(Guid transactionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.TransactionId == transactionId &&
                        accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Requested &&
                        accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.WaitingforApproval).
                        ToList());
            }
        }

        public static AccessRequest GetByIdIncludeAdHocAccessRequest(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.Id == id,
                            accessRequest => accessRequest.AdHocAccessRequest,
                            accessRequest => accessRequest.AdHocAccessRequest.EndPoint,
                            accessRequest => accessRequest.AdHocAccessRequest.EndPoint.EndPointSystemType
                           ).
                            FirstOrDefault());
            }
        }
        public static AccessRequest GetByIdIncludeAdHocAccessRequestAndUser(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                var request = new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                         Get(accessRequest =>
                             accessRequest.Id == id,
                             accessRequest => accessRequest.AdHocAccessRequest,
                             accessRequest => accessRequest.AdHocAccessRequest.EndPoint,
                             accessRequest => accessRequest.User,
                             accessRequest => accessRequest.Application,
                             accessRequest => accessRequest.Requester
                             ).FirstOrDefault();
                return ORMap(request);
            }
        }

        public static AccessRequest GetByIdIncludeAdHocAccessRequestAndUserEmail(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.Id == id,
                            accessRequest => accessRequest.AdHocAccessRequest,
                            accessRequest => accessRequest.AdHocAccessRequest.EndPoint,
                            accessRequest => accessRequest.User,
                            accessRequest => accessRequest.Application,
                            accessRequest => accessRequest.Requester,
                            accessRequest => accessRequest.User.NBusinessRole,
                            accessRequest => accessRequest.User.NOrganisation,
                            accessRequest => accessRequest.Requester.NBusinessRole,
                            accessRequest => accessRequest.Requester.NOrganisation
                            ).FirstOrDefault());
            }
        }
        public static AccessRequest GetByIdIncludeUser(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.Id == id,
                            accessRequest => accessRequest.User
                           ).
                            FirstOrDefault());
            }
        }

        public static List<AccessRequest> GetNewAccountByEndpointIdIncludeUser(long epId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var now = DateTime.Now.Date;
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.EndpointSystemId == epId && accessRequest.ProgressStatus == AccessRequestProgress.Completed &&
                            (accessRequest.RequestSource == AccessRequestSource.OnBoarding || accessRequest.RequestSource == AccessRequestSource.EmployeeCreation ||
                            accessRequest.RequestSource == AccessRequestSource.Rehire || accessRequest.RequestSource == AccessRequestSource.EmployeeConversion) &&
                            accessRequest.ExecutionTime.HasValue && accessRequest.ExecutionTime.Value >= now,
                            accessRequest => accessRequest.User
                           ).
                            ToList());
            }
        }

        public static List<AccessRequest> GetByAccountType(long userId, Infrastructure.Enums.AccountType accountType, int endpointSystemId)
        {
            List<AccessRequest> acceesRequestList = new List<AccessRequest>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                AdHocAccessRequest adhocRequest = AdHocAccessRequest.GetByAdhcoIdsAndActionType(Infrastructure.Enums.AdHocActionType.NewAccount, userId, endpointSystemId);
                if (adhocRequest != null)
                {
                    acceesRequestList = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.UserId == userId && accessRequest.AccountType == accountType &&
                            accessRequest.EndpointSystemId == endpointSystemId &&
                            accessRequest.AdHocAccessRequestId == adhocRequest.Id &&
                            (accessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval ||
                            accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute ||
                            accessRequest.ProgressStatus == AccessRequestProgress.WaitingForSync ||
                            accessRequest.ProgressStatus == AccessRequestProgress.Executing)).ToList());
                }
            }

            return acceesRequestList;
        }

        public static AccessRequest GetByAdHocAccessRequestId(long adhocAccessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.AdHocAccessRequestId == adhocAccessRequestId, accessRequest => accessRequest.AdHocAccessRequest).FirstOrDefault());
            }
        }

        public static AccessRequest GetByAdHocAccessRequestIdIncludeAdHocAccess(long adhocAccessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.AdHocAccessRequestId == adhocAccessRequestId, accessRequest => accessRequest.AdHocAccessRequest).FirstOrDefault());
            }
        }

        public static List<AccessRequest> GetByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.UserId == userId && accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute && accessRequest.EmployeeEventTriggerId != null).ToList());
            }
        }

        public static List<AccessRequest> GetByUserId(long userId, List<AccessRequestProgress> accessRequestProgresses)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.UserId == userId && accessRequestProgresses.Contains( accessRequest.ProgressStatus)).ToList());
            }
        }

        public static List<AccessRequest> GetAllByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.UserId == userId && accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute
                    ).ToList());
            }
        }

        public static List<AccessRequest> GetByParentRequestFoExtendedRequest(long parentRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.ParentRequestId == parentRequestId
                    && accessRequest.RequestSource == AccessRequestSource.ExtendedRequest,
                   accessRequest => accessRequest.AdHocAccessRequest).ToList());
            }
                
        }

        public static List<AccessRequest> GetByParentRequestId(long parentRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.ParentRequestId == parentRequestId,
                   accessRequest => accessRequest.AdHocAccessRequest).ToList());
            }
        }

        public static List<AccessRequest> GetByCreatedDate(int userId, DateTime fromDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (userId == 0)
                {
                    return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest => accessRequest.CreatedDate >= fromDate).ToList());
                }
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.UserId == userId && accessRequest.CreatedDate >= fromDate).ToList());
            }
        }

        public static List<AccessRequest> GetByProgressStatus()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest =>
                        accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ValidationFailed,
                        accessRequest => accessRequest.User,
                        accessRequest => accessRequest.AdHocAccessRequest).
                        ToList());
            }
        }

        /// <summary>
        ///  //First preference: Onboarding & AdhocRequest & Rehire & EmployeeCreation.          
        /// </summary>
        /// <returns></returns>
        private static AccessRequest GetValidAccessRequestType1(List<long> accessRequestToIgnored)
        {
            AccessRequest request = new AccessRequest();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                request = ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                  Get(accessRequest =>
                      accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                      accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                      accessRequest.ExecutionTime.Value <= DateTime.Now &&
                      !accessRequestToIgnored.Contains(accessRequest.Id) &&
                      (accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest ||
                      accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.EmployeeCreation ||
                      accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.OnBoarding ||
                      accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.Rehire ||
                      accessRequest.RequestSource == AccessRequestSource.ExtendedRequest ||
                      accessRequest.RequestSource == AccessRequestSource.EmployeeConversion
                      ), accessRequest => accessRequest.Application).
                      FirstOrDefault());
            }

            return request;
        }

        /// <summary>
        ///  //Next preference: OffBoarding & OUChange & ReOrganisation & GamingApplicationRemoval.       
        /// </summary>
        /// <returns></returns>
        private static AccessRequest GetValidAccessRequestType2(List<long> accessRequestToIgnored)
        {
            AccessRequest request = new AccessRequest();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                request = ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                           Get(accessRequest =>
                               accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                               accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                               accessRequest.ExecutionTime.Value <= DateTime.Now &&
                              !accessRequestToIgnored.Contains(accessRequest.Id) &&
                               (accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.OffBoarding ||
                               accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.OUChange ||
                               accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.ReOrganisation ||
                               accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.Promotion ||
                               accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.Demotion ||
                               accessRequest.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.GamingApplicationRemoval), accessRequest => accessRequest.Application).
                               FirstOrDefault());
            }

            return request;
        }

        /// <summary>
        ///  //Next preference: Rest.   
        /// </summary>
        /// <returns></returns>
        private static AccessRequest GetValidAccessRequestType3(List<long> accessRequestToIgnored)
        {
            AccessRequest request = new AccessRequest();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                request = ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                            Get(accessRequest =>
                                accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                                accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                                accessRequest.ExecutionTime.Value <= DateTime.Now &&
                                 !accessRequestToIgnored.Contains(accessRequest.Id),
                                 accessRequest => accessRequest.Application).
                                FirstOrDefault());
            }

            return request;
        }

        public static List<AccessRequest> GetApprovedAccessRequestBatch()
        {
            AccessRequest request = null;
            List<long> accessRequestToIgnored = new List<long>();
            bool isRequestAvailble = false;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                //Type1 == First preference: Onboarding & AdhocRequest & Rehire & EmployeeCreation && EmployeeConversion.              
                while (!isRequestAvailble)
                {
                    request = GetValidAccessRequestType1(accessRequestToIgnored);
                    if (request != null)
                    {
                        if (request.ParentRequestId.HasValue)
                        {
                            var parentRequestId = AccessRequest.GetById(request.ParentRequestId.Value);
                            if (parentRequestId != null && (parentRequestId.ProgressStatus == AccessRequestProgress.Completed || parentRequestId.ProgressStatus == AccessRequestProgress.ManuallyResolved))
                            {
                                isRequestAvailble = true;
                            }
                            else
                            {
                                accessRequestToIgnored.Add(request.Id);
                            }
                        }
                        else
                        {
                            isRequestAvailble = true;
                        }
                    }
                    else
                    {
                        isRequestAvailble = true;
                    }
                }

                if (request != null)
                {
                    return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                      Get(accessRequest =>
                          accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                          accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                          accessRequest.ExecutionTime.Value <= DateTime.Now &&
                          accessRequest.TransactionId == request.TransactionId,
                          accessRequest => accessRequest.AdHocAccessRequest, accessRequest => accessRequest.Application).
                          ToList());
                }

                isRequestAvailble = false;
                accessRequestToIgnored = new List<long>();
                request = null;

                //Type2 == Next preference: OffBoarding & OUChange & ReOrganisation & GamingApplicationRemoval.
                while (!isRequestAvailble)
                {
                    request = GetValidAccessRequestType2(accessRequestToIgnored);
                    if (request != null)
                    {
                        if (request.ParentRequestId.HasValue)
                        {
                            var parentRequestId = AccessRequest.GetById(request.ParentRequestId.Value);
                            if (parentRequestId != null && (parentRequestId.ProgressStatus == AccessRequestProgress.Completed || parentRequestId.ProgressStatus == AccessRequestProgress.ManuallyResolved))
                            {
                                isRequestAvailble = true; 
                            }
                            else
                            {
                                accessRequestToIgnored.Add(request.Id);
                            }
                        }
                        else
                        {
                            isRequestAvailble = true;
                        }
                    }
                    else
                    {
                        isRequestAvailble = true;
                    }
                }

                if (request != null)
                {
                    return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                            accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                            accessRequest.ExecutionTime.Value <= DateTime.Now &&
                            accessRequest.TransactionId == request.TransactionId,
                            accessRequest => accessRequest.AdHocAccessRequest, accessRequest => accessRequest.Application).
                            ToList());
                }

                isRequestAvailble = false;
                accessRequestToIgnored = new List<long>();
                request = null;

                //Type3 == Next preference: Rest.
                while (!isRequestAvailble)
                {
                    request = GetValidAccessRequestType3(accessRequestToIgnored);
                    if (request != null)
                    {
                        if (request.ParentRequestId.HasValue)
                        {
                            var parentRequestId = AccessRequest.GetById(request.ParentRequestId.Value);
                            if (parentRequestId != null && (parentRequestId.ProgressStatus == AccessRequestProgress.Completed || parentRequestId.ProgressStatus == AccessRequestProgress.ManuallyResolved))
                            {
                                isRequestAvailble = true;
                            }
                            else
                            {
                                accessRequestToIgnored.Add(request.Id); 
                            }
                        }
                        else
                        {
                            isRequestAvailble = true;
                        }
                    }
                    else
                    {
                        isRequestAvailble = true;
                    }
                }

                if (request != null)
                {
                    return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest =>
                            accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                            accessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute &&
                            accessRequest.ExecutionTime.Value <= DateTime.Now &&
                            accessRequest.TransactionId == request.TransactionId,
                            accessRequest => accessRequest.AdHocAccessRequest, accessRequest => accessRequest.Application).
                            ToList());
                }
                return null;
            }
        }

        public static List<AccessRequest> GetStandingOrders(long endPointSystemId, DateTime FromDate, DateTime ToDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var fullList = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EndpointSystemId == endPointSystemId, true).ToList());

                if (FromDate.Date != DateTime.MinValue.Date)
                    fullList = fullList.Where(a => a.CreatedDate >= FromDate).ToList();

                if (ToDate.Date != DateTime.MinValue.Date)
                    fullList = fullList.Where(a => a.CreatedDate <= ToDate).ToList();

                return fullList;
            }
        }

        public static string GetlastSequenceNumber(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EntityModel.AccessRequest request = new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EndpointSystemId == endPointSystemId, true).LastOrDefault();

                var requestInterim = AdHocAccessRequest.GetRequestsFromInterimStoreForEndpoint(endPointSystemId);
                long seqNumber = 0;
                if (requestInterim != null)
                {
                    var entitlementDetails = requestInterim.GetAccessRequestDetail();
                    if (entitlementDetails != null && entitlementDetails.Count > 0)
                    {
                        foreach (var entitlementDetail in entitlementDetails)
                        {
                            dynamic detail = JsonConvert.DeserializeObject(entitlementDetail.AdditionalInfo);
                            string sequenceNumber = detail.SequenceNumber; long tmpinterimValue = 0;
                            var splitSeq = sequenceNumber.Split('-');
                            tmpinterimValue = Convert.ToInt64(splitSeq.Length > 1 ? splitSeq[1] : "0");
                            if (tmpinterimValue > seqNumber)
                                seqNumber = tmpinterimValue;
                        }
                    }

                }

                if (request != null && request.CreatedDate.Year.Equals(DateTime.Now.Year) && request.AdHocAccessRequest != null)
                {
                    List<PresentationAccessRequestItem> items = (List<PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<PresentationAccessRequestItem>(request.AdHocAccessRequest.EntitlementDetail);
                    if (items.Count > 0)
                    {
                        long tmpRequestValue = 0;
                        dynamic detail = JsonConvert.DeserializeObject(items[0].AdditionalInfo);
                        string sequenceNumber = detail.SequenceNumber;
                        var splitSeq = sequenceNumber.Split('-');
                        sequenceNumber = splitSeq.Length > 1 ? splitSeq[1] : "000";
                        tmpRequestValue = Convert.ToInt64(sequenceNumber);
                        if (tmpRequestValue > seqNumber)
                            seqNumber = tmpRequestValue;
                    }
                }

                if (seqNumber > 0)
                    return seqNumber.ToString().PadLeft(3, '0');

                return "000";
            }
        }
        public static List<AccessRequest> GetRiskAssessment(long endPointSystemId, DateTime FromDate, DateTime ToDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EndpointSystemId == endPointSystemId && accessRequest.CreatedDate >= FromDate && accessRequest.CreatedDate <= ToDate && accessRequest.RequestStatus == AccessRequestStatus.Granted, true).ToList());
            }
        }
        public static IList<int> GetYearsByActivationDate()
        {
            List<AccessRequest> lstAcces = new List<AccessRequest>();
            IList<int> years = new List<int>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                lstAcces = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(AccesReq => AccesReq.ActivationDate != null).ToList());
                years = lstAcces.Select(m => m.ActivationDate.Value.Year).Distinct().ToList();
            }
            return years;
        }

        public static IList<int> GetMonthsByActivationDate()
        {
            List<AccessRequest> lstAcces = new List<AccessRequest>();
            IList<int> months = new List<int>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                lstAcces = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                     Get(AccesReq => AccesReq.ActivationDate != null).ToList());
                months = lstAcces.Select(m => m.ActivationDate.Value.Month).Distinct().ToList();
            }
            return months;
        }

        public static IList<int> GetUserYears(long UserId)
        {
            List<AccessRequest> lstAcces = new List<AccessRequest>();
            IList<int> years = new List<int>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                lstAcces = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                     Get(AccesReq => AccesReq.ActivationDate != null && AccesReq.UserId == UserId).ToList());
                years = lstAcces.Select(m => m.ActivationDate.Value.Year).Distinct().ToList();

            }

            return years;
        }


        public static IList<string> GetUsersMonths(long UserId)
        {
            List<AccessRequest> lstAcces = new List<AccessRequest>();
            IList<string> months = new List<string>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                lstAcces = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                     Get(AccesReq => AccesReq.ActivationDate != null && AccesReq.UserId == UserId).ToList());
                months = lstAcces.Select(m => m.ActivationDate.Value.Month.ToString()).Distinct().ToList();
            }

            return months;
        }


        public static List<AccessRequest> GetSubmittedAccessRequests()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.RequestStatus == AccessRequestStatus.Submitted && accessRequest.ProgressStatus == AccessRequestProgress.Submitted,
                    accessRequest => accessRequest.AdHocAccessRequest).ToList());
            }
        }
        public static bool CheckAllRequestCompletedForEmployeeEventTrigger(long EmployeeEventId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var inhouseEndPoints = EndPoint.GetAllByEndpointTypeId((long)EndpointType.InhouseSystems);
                var inHouseIds = inhouseEndPoints != null && inhouseEndPoints.Count > 0 ? inhouseEndPoints.Select(i => i.Id) : new List<long>();

                List<AccessRequest> requests = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EmployeeEventTriggerId == EmployeeEventId && !inHouseIds.Contains((long)accessRequest.EndpointSystemId)).ToList());

                if (requests.Count > 0)
                    return !requests.Any(r => r.ProgressStatus != AccessRequestProgress.Completed);
                else
                    return true;
            }
        }

        public static List<long> GetAllRequestCompletedForEmployeeEventTrigger(long EmployeeEventId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var inhouseEndPoints = EndPoint.GetAllByEndpointTypeId((long)EndpointType.InhouseSystems);
                var inHouseIds = inhouseEndPoints != null && inhouseEndPoints.Count > 0 ? inhouseEndPoints.Select(i => i.Id) : new List<long>();

                List<AccessRequest> requests = ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EmployeeEventTriggerId == EmployeeEventId && !inHouseIds.Contains((long)accessRequest.EndpointSystemId)).ToList());

                if (requests.Count > 0)
                    return requests.Select(r => r.Id).ToList();
                else
                    return new List<long>();
            }
        }

        public override long Save(DataAccessTransaction dataAccessTransaction)
        {
            base.Save(dataAccessTransaction);
            if (!IsNew)
            {
                DomainModel.AdHocAccessRequest adHocAccessRequest = this.AdHocAccessRequest;
                if (adHocAccessRequest != null && !adHocAccessRequest.IsNew)
                {
                    adHocAccessRequest.Save(dataAccessTransaction);
                }
            }
            return 0;
        }

        public static List<long> SaveAccessRequestList(List<AccessRequest> accessRequestList)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction("system"))
            {
                SaveList(dataAccessTransaction, accessRequestList);

                List<Attachment> requestAttachments = new List<Attachment>();
                foreach (var item in accessRequestList)
                {
                    if (string.IsNullOrEmpty(item.InterimIds)) continue;
                    List<int> interimIds = item.InterimIds.Split(',').Select(int.Parse).ToList();
                    foreach (var interimId in interimIds)
                    {
                        var interimAttachment = InterimStore.GetByTaskId(interimId, true, InterimStoreItemType.Attachment);
                        if (interimAttachment != null)
                        {
                            dataAccessTransaction.User = interimAttachment.CreatedBy;
                            var presentationInterimAttachment = UtilityMethods.Deserialize<Dto.Presentation.PresentationInterimAttachment>(interimAttachment.XmlData);
                            Attachment attachment = new Attachment
                            {
                                AccessRequestId = item.Id,
                                FileName = presentationInterimAttachment.FileName,
                                ContentType = presentationInterimAttachment.ContentType,
                                AttachmentFile = presentationInterimAttachment.AttachmentFile,
                                CreatedBy = interimAttachment.CreatedBy,
                                RequestTypeId = (long)RequestType.Provisioning
                            };
                            requestAttachments.Add(attachment);
                        }
                    }
                }
                Attachment.SaveList(dataAccessTransaction, requestAttachments);
                dataAccessTransaction.Commit();

                return accessRequestList.Select(ar => ar.Id).ToList();
            }

        }
        public static void SaveRemoveRequest(List<PresentationAttestationEntitlementReviewDetail> accessList, long endpointId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> accesslst = new List<PresentationAccessRequestItem>();
                AccessRequest accessRequest = new AccessRequest();
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();
                accesslst = accesslst;

                foreach (PresentationAttestationEntitlementReviewDetail getdata in accessList.Where(x => x.EndpointSystemId == endpointId))
                {
                    //if (getdata.ReviewCategory == "Account")
                    //{
                    adHocAccessRequest.RequesterUserId = (long)getdata.RequesterUserId;
                    adHocAccessRequest.RequesteeUserId = getdata.RequesteeUserId;
                    adHocAccessRequest.EndpointSystemId = getdata.EndpointSystemId;
                    adHocAccessRequest.EndpointSystemType = (EndpointType)getdata.EndpointSystemType;
                    adHocAccessRequest.AdhocActionType = getdata.AdhocActionType;
                    adHocAccessRequest.AccountType = getdata.AccountType;
                    adHocAccessRequest.ValidFrom = DateTime.Now;
                    adHocAccessRequest.Comments = "AdhocRequest created during attestation review";
                    //}
                    //else 
                    if (getdata.ReviewCategory == "Entitlement")
                    {
                        PresentationAccessRequestItem accessRequestItem = new PresentationAccessRequestItem();
                        accessRequestItem.AccessItemType = AccessItemType.TechnicalRole;
                        accessRequestItem.AccessRequestItemType = AccessRequestItemType.Entitlement;
                        accessRequestItem.EntitlementType = EntitlementType.Permission;
                        accessRequestItem.EndpointType = EndpointType.InhouseSystems;
                        accessRequestItem.KeyId = getdata.EntitlementId;
                        accessRequestItem.EndpointSystemId = getdata.EndpointSystemId;
                        accessRequestItem.EndpointSystemTypeId = (long)EndpointType.InhouseSystems;
                        accessRequestItem.ItemName = getdata.Name;
                        accessRequestItem.ItemDescription = getdata.DisplayName;
                        accessRequestItem.ItemDetail = getdata.Name;
                        accesslst.Add(accessRequestItem);
                    }
                }
                if (accesslst != null && accesslst.Count > 0)
                    adHocAccessRequest.EntitlementDetail = UtilityMethods.SerializeCollection<PresentationAccessRequestItem>(accesslst);
                adHocAccessRequest.Save(dataAccessTransaction);

                accessRequest.AdHocAccessRequestId = adHocAccessRequest.Id;
                accessRequest.RequesterId = adHocAccessRequest.RequesterUserId;
                accessRequest.UserId = adHocAccessRequest.RequesteeUserId;
                accessRequest.EndpointSystemId = adHocAccessRequest.EndpointSystemId;
                accessRequest.AccountType = adHocAccessRequest.AccountType;
                accessRequest.ApplicationType = adHocAccessRequest.ApplicationType;
                accessRequest.RequestStatus = AccessRequestStatus.Submitted;
                accessRequest.ProgressStatus = AccessRequestProgress.WaitingforApproval;
                accessRequest.RequestSource = AccessRequestSource.AttestationRemediation;
                accessRequest.ActivationDate = DateTime.Now;
                accessRequest.Comments = "Remove request created during attestation review";
                accessRequest.ExecutionTime = DateTime.Now;
                accessRequest.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public static bool SaveExtendedRequest(AccessRequest acceRequest, long accessRequestId)
        {
            DomainModel.AccessRequest newaccessRequest = new DomainModel.AccessRequest();
            DomainModel.AdHocAccessRequest newAdhocAccessRequest = new DomainModel.AdHocAccessRequest();
            DomainModel.AccessRequest oldaccessRequest = DomainModel.AccessRequest.GetById(acceRequest.Id);
            DomainModel.AdHocAccessRequest oldAdhocAccessRequest = DomainModel.AdHocAccessRequest.GetById((long)oldaccessRequest.AdHocAccessRequestId);
            EndPoint endpoint = null;

            if (endpoint == null && oldaccessRequest.EndpointSystemId.HasValue)
                endpoint = EndPoint.GetById(oldaccessRequest.EndpointSystemId.Value);

            string comments = string.Format("{0}{1}-{2}", "Extension Request For AccessRequest ID", acceRequest.Id, acceRequest.Comments);

            //For Extended Request - New Adhoch Request for reference to AccessRequest
            newAdhocAccessRequest.AccountType = oldAdhocAccessRequest.AccountType;
            newAdhocAccessRequest.AdhocActionType = AdHocActionType.ExtendedRequest;
            newAdhocAccessRequest.RequesteeUserId = oldAdhocAccessRequest.RequesteeUserId;
            newAdhocAccessRequest.RequesterUserId = oldAdhocAccessRequest.RequesterUserId;
            newAdhocAccessRequest.EndpointSystemId = oldAdhocAccessRequest.EndpointSystemId;
            newAdhocAccessRequest.EndpointSystemType = oldAdhocAccessRequest.EndpointSystemType;
            newAdhocAccessRequest.EntitlementDetail = oldAdhocAccessRequest.EntitlementDetail;
            newAdhocAccessRequest.IsSelfRequest = oldAdhocAccessRequest.IsSelfRequest;
            newAdhocAccessRequest.IsÉxpiryHandled = false;
            newAdhocAccessRequest.IsÉxpiryMailSend = false;
            newAdhocAccessRequest.CreatedBy = acceRequest.CreatedBy;
            newAdhocAccessRequest.CreatedDate = DateTime.Now;

            //ValidTo,Comments is coming from UI, ValidFrom is Today's date
            newAdhocAccessRequest.Comments = comments;
            newAdhocAccessRequest.ValidFrom = DateTime.Today;
            newAdhocAccessRequest.ValidTo = acceRequest.DeactivationDate;

            if (endpoint != null && !string.IsNullOrEmpty(newAdhocAccessRequest.EntitlementDetail))
            {
                var entitlementList = newAdhocAccessRequest.GetAccessRequestDetail();
                if (entitlementList != null && entitlementList.Count > 0)
                {
                    string lastSequenceNumber = AccessRequest.GetlastSequenceNumber(endpoint.Id);
                    int intLastSequenceNumber = Convert.ToInt32(lastSequenceNumber);

                    entitlementList.ForEach(a =>
                    {
                        if (!string.IsNullOrEmpty(a.AdditionalInfo))
                        {
                            intLastSequenceNumber += (int.Parse(lastSequenceNumber) + 1);
                            string runningNumber = string.Format(intLastSequenceNumber.ToString().PadLeft(3, '0'));
                            dynamic detail = new ExpandoObject();
                            dynamic additionInfo = JsonConvert.DeserializeObject(a.AdditionalInfo);
                            additionInfo.SequenceNumber = string.Format("{0}{1}-{2}", endpoint.ApplicationCode, DateTime.Now.Year, runningNumber.ToString());
                            additionInfo.ReferenceAccessRequetId = acceRequest.Id;
                            var updatedDetails = JsonConvert.SerializeObject(additionInfo);
                            a.AdditionalInfo = updatedDetails;
                        }
                    });

                    newAdhocAccessRequest.SetRequestedItems = entitlementList;
                }
            }

            newAdhocAccessRequest.Save("System");


            newaccessRequest.AccountType = oldaccessRequest.AccountType;
            newaccessRequest.ApplicationType = oldaccessRequest.ApplicationType;
            newaccessRequest.AdditionalInfo = oldaccessRequest.AdditionalInfo;
            newaccessRequest.Application = oldaccessRequest.Application;
            newaccessRequest.ApproverType = oldaccessRequest.ApproverType;
            newaccessRequest.EndpointSystemId = oldaccessRequest.EndpointSystemId;
            newaccessRequest.RequesterId = oldaccessRequest.RequesterId;
            newaccessRequest.UserId = oldaccessRequest.UserId;
            newaccessRequest.TransactionId = oldaccessRequest.TransactionId;

            newaccessRequest.IsCurrentLevelApproved = false;
            newaccessRequest.IsParentRequest = false;
            newaccessRequest.ParentRequestId = oldaccessRequest.Id;
            newaccessRequest.RequestSource = AccessRequestSource.ExtendedRequest;
            newaccessRequest.ProgressStatus = AccessRequestProgress.WaitingforApproval;
            newaccessRequest.RequestStatus = AccessRequestStatus.Submitted;
            newaccessRequest.Description = string.Format("Access Request Id : {0} requested to extended by user {1}.", acceRequest.Id, acceRequest.CreatedBy);
            newaccessRequest.ActivationDate = DateTime.Today;
            newaccessRequest.DeactivationDate = acceRequest.DeactivationDate;
            newaccessRequest.Comments = comments;
            newaccessRequest.AdHocAccessRequestId = newAdhocAccessRequest.Id;
            newaccessRequest.CreatedBy = acceRequest.CreatedBy;
            newaccessRequest.CreatedDate = DateTime.Now;

            newaccessRequest.Save("System");

            return true;
        }

        public static IList<AccessRequest> GetExtendedRequestById(long accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.ParentRequestId == accessRequestId
                    && accessRequest.RequestSource == AccessRequestSource.ExtendedRequest &&
                   (accessRequest.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequest.ProgressStatus == AccessRequestProgress.Executing || accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute)).ToList());
            }
        }

        public static bool CheckForExtendedRequestInProgress(AccessRequest parentaccessRequest)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                AccessRequest extendedrequests = ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                     Get(accessRequest => accessRequest.ParentRequestId == parentaccessRequest.Id).FirstOrDefault());

                if (extendedrequests != null && (extendedrequests.ProgressStatus == AccessRequestProgress.WaitingForSync ||
                                                 extendedrequests.ProgressStatus == AccessRequestProgress.Executing ||
                                                 extendedrequests.ProgressStatus == AccessRequestProgress.ReadytoExecute ||
                                                extendedrequests.ProgressStatus == AccessRequestProgress.WaitingforApproval
                   ))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        //public static AccessRequest GetCSELRequestByRequesteeAndEndpointId(long requesteeID, long endpointSystemId, AccountType accountType)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMap(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
        //            Get(accessRequest =>
        //                accessRequest.UserId == requesteeID
        //                && accessRequest.EndpointSystemId == endpointSystemId
        //                && accessRequest.AccountType == accountType
        //                && accessRequest.RequestSource == AccessRequestSource.GamingApplicationRemoval
        //                && (accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequest.ProgressStatus == AccessRequestProgress.Executing)).FirstOrDefault());
        //    }
        //}

        private static void setRequestItemDetail(AdHocAccessRequest adhocRequest, AccountReconcilation accountReconcilation)
        {
            dynamic detail = new ExpandoObject();

            detail = ExpandoHelper.AddProperty(detail, "AccountName", accountReconcilation == null ? "" : accountReconcilation.AccountName);
            string detailString = JsonConvert.SerializeObject(detail);

            PresentationAccessRequestItem arItem = new PresentationAccessRequestItem
            {
                EndpointSystemId = adhocRequest.EndpointSystemId.Value,
                AccessRequestItemType = AccessRequestItemType.Application,
                AdditionalInfo = detailString,
            };
            adhocRequest.SetRequestedItems = new List<PresentationAccessRequestItem>() { arItem };
        }

        public static IList<AccessRequest> GetByEndpointSystemIdWithTXData(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EndpointSystemId == endpointSystemId
                      && !string.IsNullOrEmpty(accessRequest.RESPONSETXID)).ToList());
            }
        }

        public static IList<AccessRequest> GetIDASRequestByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.AdHocAccessRequest.EndpointSystemType == EndpointType.IDMS && accessRequest.UserId == userId
                    && !(accessRequest.ProgressStatus == AccessRequestProgress.Completed ||
                   accessRequest.ProgressStatus == AccessRequestProgress.RequestRejected || accessRequest.ProgressStatus == AccessRequestProgress.Withdrawn)).ToList());
            }
        }

        public static AccessRequest PopulateNewFromOldAccessRequest(AccessRequest oldRequest)
        {
            return new AccessRequest()
            {
                ActivationDate = oldRequest.ActivationDate,
                ApproverType = oldRequest.ApproverType,
                Comments = oldRequest.Comments,
                CreatedBy = oldRequest.CreatedBy,
                CreatedDate = oldRequest.CreatedDate,
                DeactivationDate = oldRequest.DeactivationDate,
                Description = oldRequest.Description,
                ExecutionTime = oldRequest.ExecutionTime.Value,
                IsCurrentLevelApproved = oldRequest.IsCurrentLevelApproved,
                LastModifiedBy = oldRequest.LastModifiedBy,
                LastModifiedDate = oldRequest.LastModifiedDate,
                ProgressStatus = oldRequest.ProgressStatus,
                Remarks = oldRequest.Remarks,
                RequesterId = oldRequest.RequesterId,
                RequestSource = oldRequest.RequestSource,
                RequestStatus = oldRequest.RequestStatus,
                TransactionId = oldRequest.TransactionId,
                UserId = oldRequest.UserId,
                AccountType = oldRequest.AccountType,
                ApplicationType = oldRequest.ApplicationType,

                OrgBranch = oldRequest.OrgBranch,
                OrgCompany = oldRequest.OrgCompany,
                OrgDepartment = oldRequest.OrgDepartment,
                OrgDivision = oldRequest.OrgDivision,
                OrgSection = oldRequest.OrgSection,
                OrgUnit = oldRequest.OrgUnit,
                BulkRequestId = oldRequest.BulkRequestId,
                AttestationCampaignId = oldRequest.AttestationCampaignId,
                ConflictsRemarks = oldRequest.ConflictsRemarks
            };
        }

        public static AdHocAccessRequest PopulateNewFromOldAdhocRequest(AdHocAccessRequest adhocaccessrequest)
        {
            return new AdHocAccessRequest()
            {
                AccountType = adhocaccessrequest.AccountType,
                AdhocActionType = adhocaccessrequest.AdhocActionType,
                Comments = adhocaccessrequest.Comments,
                CreatedBy = adhocaccessrequest.CreatedBy,
                CreatedDate = adhocaccessrequest.CreatedDate,
                EndpointSystemType = adhocaccessrequest.EndpointSystemType,
                ApplicationType = adhocaccessrequest.ApplicationType,
                IsSelfRequest = adhocaccessrequest.IsSelfRequest,
                RequesteeUserId = adhocaccessrequest.RequesteeUserId,
                RequesterUserId = adhocaccessrequest.RequesterUserId,
                ValidFrom = adhocaccessrequest.ValidFrom,
                ValidTo = adhocaccessrequest.ValidTo,
                EntitlementDetail = adhocaccessrequest.EntitlementDetail,

            };
        }

        public static IList<AccessRequest> GetChildRequestByBulkRequestId(long bulkRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.BulkRequestId == bulkRequestId,
                      accessRequest => accessRequest.AdHocAccessRequest).ToList());
            }
        }


        public static Dto.BooleanStatus CheckForDuplicate(ApplicationType applicationType, long endpointSystemId, AdHocActionType actiontype, AccountType accountType, long userId)
        {
            Dto.BooleanStatus accountStatus = new Dto.BooleanStatus() { CanProceed = true };
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                string s = new AccessRequestRepository(dataAccessTransaction).CheckForDuplicateAccessRequest(applicationType, endpointSystemId, actiontype, accountType, userId);
                if (!string.IsNullOrEmpty(s))
                {
                    accountStatus.CanProceed = false;
                    accountStatus.Message = $"Same access request already raised in {s}";
                }
            }
            return accountStatus;
        }

        public static bool AddAccessRequest(IList<AccessRequest> list)
        {
            try
            {
                var userName = MasterConfigHelper.ServiceUser;
                foreach (AccessRequest ar in list)
                {
                    var user = DomainModel.User.GetById(ar.UserId);

                    if (user != null)
                    {
                        ar.OrgCompany = user.OrgCompany;
                        ar.OrgDepartment = user.OrgDepartment;
                        ar.OrgDivision = user.OrgDivision;
                        ar.OrgBranch = user.OrgBranch;
                        ar.OrgSection = user.OrgSection;
                        ar.OrgUnit = user.OrgUnit;
                    }
                    ar.ActivationDate = ar.ActivationDate == DateTime.MinValue ? null : ar.ActivationDate;
                    ar.DeactivationDate = ar.DeactivationDate == DateTime.MinValue ? null : ar.DeactivationDate;
                    ar.ExecutionTime = ar.ExecutionTime == DateTime.MinValue ? null : ar.ExecutionTime;

                    ar.Save(userName);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool AddAccessRequest(AccessRequest request)
        {
            try
            {
                if (request != null)
                {
                    var user = DomainModel.User.GetById(request.UserId);

                    if (user != null)
                    {
                        request.OrgCompany = user.OrgCompany;
                        request.OrgDepartment = user.OrgDepartment;
                        request.OrgDivision = user.OrgDivision;
                        request.OrgBranch = user.OrgBranch;
                        request.OrgSection = user.OrgSection;
                        request.OrgUnit = user.OrgUnit;
                    }
                    request.ActivationDate = request.ActivationDate == DateTime.MinValue ? null : request.ActivationDate;
                    request.DeactivationDate = request.DeactivationDate == DateTime.MinValue ? null : request.DeactivationDate;
                    request.ExecutionTime = request.ExecutionTime == DateTime.MinValue ? null : request.ExecutionTime;

                    request.Save("System");
                }

                return true;
            }
            catch
            {
                throw;
            }
        }
        public static bool IsSameAccessRequestExistsForBulkRequestId(long bulkrequestId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Any(accessRequest => accessRequest.BulkRequestId != null && accessRequest.BulkRequestId == bulkrequestId && accessRequest.UserId == userId);
            }
        }
        public static bool IsSameAccessRequestExistsForUser(AccessRequestSource requestSource, long userId, long employeeEventId,long endpointSystemId,DateTime? activationDate, ApplicationType applicationType, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Any(accessRequest => accessRequest.EmployeeEventTriggerId != null && accessRequest.RequestSource == requestSource && accessRequest.EmployeeEventTriggerId == employeeEventId && accessRequest.UserId == userId && accessRequest.EndpointSystemId == endpointSystemId && accessRequest.ActivationDate == activationDate && accessRequest.ApplicationType == applicationType && accessRequest.AccountType == accountType);
            }
        }
        public static bool IsSameOffboardAccessRequestExistsForUser(AccessRequestSource requestSource, long userId, long employeeEventId, long endpointSystemId, DateTime? deactiavetDate, ApplicationType applicationType, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Any(accessRequest => accessRequest.EmployeeEventTriggerId != null && accessRequest.RequestSource == requestSource && accessRequest.EmployeeEventTriggerId == employeeEventId && accessRequest.UserId == userId && accessRequest.EndpointSystemId == endpointSystemId && accessRequest.DeactivationDate == deactiavetDate && accessRequest.ApplicationType == applicationType && accessRequest.AccountType == accountType);
            }
        }
        public static List<AccessRequest> SameOffboardAccessRequestExistsForUser(AccessRequestSource requestSource, long userId, long employeeEventId, long endpointSystemId, DateTime? deactiavetDate, ApplicationType applicationType, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                    Get(accessRequest => accessRequest.EmployeeEventTriggerId != null && accessRequest.RequestSource == requestSource && accessRequest.EmployeeEventTriggerId == employeeEventId && accessRequest.UserId == userId && accessRequest.EndpointSystemId == endpointSystemId && accessRequest.DeactivationDate == deactiavetDate && accessRequest.ApplicationType == applicationType && accessRequest.AccountType == accountType).ToList());
            }
        }
        public static string CreateADPropagationForEmployee(long specificUserId=0)
        {
            List<User> usrs = new List<User>();
            List<AccountReconcilation> adAccounts = new List<AccountReconcilation>();
            try
            {
                if (MasterConfigHelper.CreateADPropagation)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "CreateADPropagation started.");
                    var ep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    if (ep != null && ep.IsActive)
                    {
                        usrs = User.GetActiveEmployees().Where(i => i.FullTimeEmployee).ToList();
                        adAccounts = AccountReconcilation.GetAllActiveByEndpointIdByAccountType(ep.Id, AccountType.User);
                        if (specificUserId > 0)
                        {
                            usrs = usrs.Where(i=> i.Id == specificUserId).ToList();
                            adAccounts = adAccounts.Where(i => i.UserId == specificUserId).ToList();
                        }
                        
                        if (adAccounts != null && adAccounts.Any() && usrs != null && usrs.Any())
                        {
                            var users = adAccounts.Where(i => usrs.Exists(j => i.UserId.Value == j.Id)).Select(i => i.UserId.Value).Distinct().ToList();
                            foreach (var userId in users)
                            {
                                try
                                {
                                    var evntId = EmployeeEventTrigger.CreateDummyPropagationTrigger(userId, "System", "For EEGroup propagation", "Bulk Propagation from IAM Team");
                                    if (evntId > 0)
                                    {
                                        var newRequest = new AccessRequest();
                                        newRequest.UserId = userId;
                                        newRequest.AccountType = AccountType.User;
                                        newRequest.ApplicationType = ep.ApplicationType;
                                        newRequest.EndpointSystemId = ep.Id;
                                        newRequest.RequestStatus = AccessRequestStatus.Granted;
                                        newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                        newRequest.RequestSource = AccessRequestSource.Propagation;
                                        newRequest.RequesterId = null;
                                        newRequest.ExecutionTime = DateTime.Now;
                                        newRequest.Description = "This request is created automatically for Propagation";
                                        newRequest.Remarks = string.Empty;
                                        newRequest.TransactionId = Guid.NewGuid();
                                        newRequest.EmployeeEventTriggerId = evntId;
                                        newRequest.CreatedBy = "Sysetm";
                                        newRequest.CreatedDate = DateTime.Now;
                                        newRequest.Save("System");
                                    }
                                    else
                                    {
                                        DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"CreateADPropagation: Employeevent did not create.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"CreateADPropagation failed to create trigger.");
                                }
                            }
                            users.Clear();
                            users = null;
                            usrs.Clear();
                            usrs = null;
                            adAccounts.Clear();
                            adAccounts = null;
                        }

                        DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "ADPropagation created successfully.");
                    }
                    else
                    {
                        DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "ADPropagation: Endpoint inactive. Cannot create propagation trigger.");
                    }
                    ADSUserInfo.ExecuteQuery("UPDATE DI_M_IMConfigurationMaster SET ConfigValue='0' WHERE ConfigKey='CreateADPropagation'");
                    return "AD Propagation Request created successfully.";
                }
                else
                {
                    return "Enable IMConfiguration(CreateADPropagation=1) before trigger AD Propagation.";
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "CreateADPropagation trigger Completed", ex);
                return "Error occured while creating AD Propagation request";
            }
            finally
            {
                usrs?.Clear();
                usrs = null;

                adAccounts?.Clear();
                adAccounts = null;
            }
        }

        public static List<AccessRequest> GetMSTeamRequestForApplyPolicy()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                        Get(accessRequest => accessRequest.ProgressStatus == AccessRequestProgress.WaitingForSync && accessRequest.ExecutionTime.Value <= DateTime.Now,
                            accessRequest => accessRequest.AdHocAccessRequest,
                            accessRequest => accessRequest.AdHocAccessRequest.EndPoint,
                            accessRequest => accessRequest.AdHocAccessRequest.EndPoint.EndPointSystemType
                           ).ToList());
            }
        }

        public static List<AccessRequest> GetAccessRequestByStatus(AccessRequestProgress accessRequestProgress, long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMapList(new RepositoryBase<EntityModel.AccessRequest>(dataAccessTransaction).
                       Get(accessRequest =>
                           accessRequest.EndpointSystemId == endpointId &&
                           accessRequest.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Granted &&
                           accessRequest.ProgressStatus == accessRequestProgress && accessRequest.AccountType == AccountType.User &&
                           accessRequest.ExecutionTime.Value <= DateTime.Now,
                           accessRequest => accessRequest.AdHocAccessRequest,
                           accessRequest => accessRequest.EventUndoRequest,
                           accessRequest => accessRequest.Application).
                           ToList());
            }
        }
        public static bool UpdateUserTelephonenumberInHRMS(long specificUserId)
        {
            bool isUpdated = false;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if(specificUserId > 0)
                {
                    isUpdated= ExecuteQuery($"UPDATE DI_M_User SET NeedTelephoneUpdateToHR = 1, NeedEmailUpdateToHR = 1,NeedFLINSLoginIdUpdateToHR = 1 WHERE ID={specificUserId}");
                }
            }
            return isUpdated;
        }
    }
}

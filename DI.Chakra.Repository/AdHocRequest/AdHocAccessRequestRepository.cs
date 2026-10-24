using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class AdHocAccessRequestRepository : RepositoryBase<AdHocAccessRequest>
    {
        public AdHocAccessRequestRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<AdHocAccessRequest> GetListofEntitlementByRequesteeUserId(int CurrentRequesteeUserId)
        {
            Dictionary<string, string> ListofEntiltement = new Dictionary<string, string>();

            List<AdHocAccessRequest> adhocRequest = (from source in CurrentContext.AdHocAccessRequests
                                                     join accessrequest in CurrentContext.AccessRequest on source.Id equals accessrequest.AdHocAccessRequestId
                                                     where source.RequesteeUserId == CurrentRequesteeUserId && source.ApplicationType == ApplicationType.HRMS_FLINS
                                                     && source.AdhocActionType == AdHocActionType.AmendmentAdd && accessrequest.ProgressStatus == AccessRequestProgress.WaitingforApproval
                                                     select source).ToList();

            return adhocRequest;
        }
        public List<PresentationAdHocAccessRequest> GetRequestsByActionUserId(long actionUserId)
        {
            List<PresentationAdHocAccessRequest> requestList = new List<PresentationAdHocAccessRequest>();

            requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                           join acssrqst in CurrentContext.AccessRequest
                           on rqst.Id equals acssrqst.AdHocAccessRequestId
                           join rusr in CurrentContext.User
                           on rqst.RequesteeUserId equals rusr.Id
                           join rqust in CurrentContext.User
                           on rqst.RequesterUserId equals rqust.Id
                           where rqst.RequesterUserId == actionUserId && !acssrqst.IsDependentRequest
                           select new PresentationAdHocAccessRequest
                           {
                               RequestSource = acssrqst.RequestSource.ToString(),
                               BulkRequestId = acssrqst.BulkRequestId,
                               AdHocAccessRequestId = rqst.Id,
                               Name = rqst.EndPoint.DisplayName,
                               RequestedDate = rqst.CreatedDate,
                               ExecutionDate = acssrqst.ExecutionTime,
                               RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                               Comments = rqst.Comments,
                               RequestStatus = acssrqst.RequestStatus,
                               AccessRequestID = acssrqst.Id,
                               RequesteeName = rusr.DisplayName,
                               RequesterName = rqust.DisplayName,
                               AccountType = rqst.AccountType,
                               ValidFrom = rqst.ValidFrom,
                               ValidTo = rqst.ValidTo,
                               AdhocActionType = rqst.AdhocActionType,
                               RequestProgress = acssrqst.ProgressStatus
                           }).ToList();


            foreach (PresentationAdHocAccessRequest _rqst in requestList)
            {
                _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
            }
            return requestList;
        }

        public List<PresentationAdHocAccessRequest> GetFailedRequestDetails()
        {
            List<PresentationAdHocAccessRequest> requestList = new List<PresentationAdHocAccessRequest>();
            try
            {
                requestList = (from acssrqst in CurrentContext.AccessRequest
                               join rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                    on acssrqst.AdHocAccessRequestId equals rqst.Id into failedAccessRequests
                               from failedRequest in failedAccessRequests.DefaultIfEmpty()
                               join rusr in CurrentContext.User
                                    on failedRequest.RequesteeUserId equals rusr.Id into emptyRequesteeUserId
                               from emptyRequestee in emptyRequesteeUserId.DefaultIfEmpty()
                               join rqust in CurrentContext.User
                                    on failedRequest.RequesterUserId equals rqust.Id into emptyRequesterUserId
                               from emptyRequester in emptyRequesterUserId.DefaultIfEmpty()
                               where (acssrqst.ProgressStatus == AccessRequestProgress.ExecutionFailed || acssrqst.ProgressStatus == AccessRequestProgress.ValidationFailed) && !acssrqst.IsDependentRequest
                               select new PresentationAdHocAccessRequest
                               {
                                   AdHocAccessRequestId = failedRequest != null ? failedRequest.Id : 0,
                                   Name = failedRequest != null ? failedRequest.EndPoint.DisplayName : "",
                                   RequestedDate = failedRequest != null ? failedRequest.CreatedDate : (acssrqst.ActivationDate.HasValue ? (DateTime)acssrqst.ActivationDate : DateTime.MaxValue),
                                   ExecutionDate = acssrqst.ExecutionTime,
                                   RequestType = failedRequest != null ? (((int)failedRequest.AdhocActionType == 2 || (int)failedRequest.AdhocActionType == 3) ? ((int)failedRequest.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)failedRequest.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID")) : acssrqst.RequestSource.ToString(),
                                   Comments = failedRequest != null ? failedRequest.Comments : "",
                                   RequestStatus = acssrqst.RequestStatus,
                                   AccessRequestID = acssrqst.Id,
                                   RequesteeName = emptyRequestee != null ? emptyRequestee.DisplayName : acssrqst.Requester.DisplayName,
                                   RequesterName = emptyRequester != null ? emptyRequester.DisplayName : acssrqst.Requester.DisplayName,
                                   AccountType = failedRequest != null ? failedRequest.AccountType : AccountType.User,
                                   ValidFrom = failedRequest != null ? failedRequest.ValidFrom : (acssrqst.ActivationDate.HasValue ? acssrqst.ActivationDate : null),
                                   ValidTo = failedRequest != null ? failedRequest.ValidTo : DateTime.MaxValue,
                                   AdhocActionType = failedRequest != null ? failedRequest.AdhocActionType : AdHocActionType.None,
                                   RequestProgress = acssrqst.ProgressStatus
                               }).ToList();


                foreach (PresentationAdHocAccessRequest _rqst in requestList)
                {
                    _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                }
            }
            catch (Exception ex)
            {

            }
            return requestList;
        }

        public List<PresentationAdHocRequest> GetPendingRequests(AdHocAccessRequest adhocAccessRequestObj)
        {
            List<PresentationAdHocRequest> returnList = new List<PresentationAdHocRequest>();
            if ((adhocAccessRequestObj.AccountType == AccountType.Service || adhocAccessRequestObj.AccountType == AccountType.Shared) && adhocAccessRequestObj.AdhocActionType == AdHocActionType.NewAccount)
            {
                var query = from accessRequests in this.CurrentContext.AccessRequest
                            join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                            on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                            where
                                (
                                //adhocAccessRequestObj.AdhocActionType == AdHocActionType.AmendmentAdd ?
                                //(adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentAdd || adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount) :
                                (adhocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType)
                                ) &&
                                //adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                                adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                                adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                                adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                                (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequests.ProgressStatus == AccessRequestProgress.Submitted || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync
                                || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                                || accessRequests.ProgressStatus == AccessRequestProgress.WorkflowNotFound
                                )
                            select new PresentationAdHocRequest
                            {
                                accessRequests = accessRequests,
                                adHocAccessRequests = adhocAccessRequests
                            };

                returnList = query.ToList();
            }
            else
            {

                var query = from accessRequests in this.CurrentContext.AccessRequest
                            join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                            on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                            where
                                (
                                //adhocAccessRequestObj.AdhocActionType == AdHocActionType.AmendmentAdd ?
                                //(adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentAdd || adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount) :
                                (adhocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType)
                                ) &&
                                adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                                adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                                adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                                adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                                (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequests.ProgressStatus == AccessRequestProgress.Submitted || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync
                                || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                                )
                            select new PresentationAdHocRequest
                            {
                                accessRequests = accessRequests,
                                adHocAccessRequests = adhocAccessRequests
                            };

                returnList = query.ToList();
                
            }
            return returnList;
        }
        public List<PresentationAdHocRequest> GetPendingRemediationRequests(AdHocAccessRequest adhocAccessRequestObj, long? attestationCampaignId,long userId)
        {
            List<PresentationAdHocRequest> returnList = new List<PresentationAdHocRequest>();
            
            var query = from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where
                            accessRequests.UserId == userId && accessRequests.AttestationCampaignId != null && accessRequests.AttestationCampaignId == attestationCampaignId &&
                            (
                            (adhocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType)
                            ) &&
                            adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                            adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                            adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                            adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                            (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequests.ProgressStatus == AccessRequestProgress.Submitted || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                            )
                        select new PresentationAdHocRequest
                        {
                            accessRequests = accessRequests,
                            adHocAccessRequests = adhocAccessRequests
                        };

            returnList = query.ToList();

            return returnList;
        }
        public List<PresentationAdHocRequest> GetRemediationDeleteAccountRequests(AdHocAccessRequest adhocAccessRequestObj, long? attestationCampaignId, long userId)
        {
            List<PresentationAdHocRequest> returnList = new List<PresentationAdHocRequest>();

            var query = from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where
                            accessRequests.UserId == userId && accessRequests.AttestationCampaignId != null && accessRequests.AttestationCampaignId == attestationCampaignId &&
                            adhocAccessRequests.AdhocActionType == AdHocActionType.DeleteAccount &&
                            adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                            adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                            adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                            adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                            (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequests.ProgressStatus == AccessRequestProgress.Submitted || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                            )
                        select new PresentationAdHocRequest
                        {
                            accessRequests = accessRequests,
                            adHocAccessRequests = adhocAccessRequests
                        };

            returnList = query.ToList();

            return returnList;
        }
        public List<PresentationAdHocRequest> GetPendingRequests(AdHocAccessRequest adhocAccessRequestObj, ApplicationType appType)
        {
            var query = from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where
                            (
                            //adhocAccessRequestObj.AdhocActionType == AdHocActionType.AmendmentAdd ?
                            //(adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentAdd || adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount) :
                            (adhocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType)
                            ) &&
                            adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                            adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                            adhocAccessRequests.ApplicationType == appType &&
                            (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute || accessRequests.ProgressStatus == AccessRequestProgress.Submitted)
                        select new PresentationAdHocRequest
                        {
                            accessRequests = accessRequests,
                            adHocAccessRequests = adhocAccessRequests
                        };

            var List = query.ToList();

            return List;
        }

        public List<PresentationAdHocRequest> GetPendingRequestsSpecialACR(AdHocAccessRequest adhocAccessRequestObj)
        {
            var List = (from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where
                            adhocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType &&
                            adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                            adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                            adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                            adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                            (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                            || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute)
                        select new PresentationAdHocRequest
                        {
                            accessRequests = accessRequests,
                            adHocAccessRequests = adhocAccessRequests
                        }).ToList();
            return List;
        }

        public List<PresentationAdHocRequest> GetNewAccount(AdHocAccessRequest adhocAccessRequestObj)
        {
            return (from accessRequests in CurrentContext.AccessRequest
                    join adhocAccessRequests in CurrentContext.AdHocAccessRequests
                    on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                    where
                        (
                        adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount &&
                        adhocAccessRequests.RequesteeUserId == adhocAccessRequestObj.RequesteeUserId &&
                        adhocAccessRequests.EndpointSystemType == adhocAccessRequestObj.EndpointSystemType &&
                        adhocAccessRequests.AccountType == adhocAccessRequestObj.AccountType &&
                        adhocAccessRequests.EndpointSystemId == adhocAccessRequestObj.EndpointSystemId &&
                        (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval
                        || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                        || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute))
                    select new PresentationAdHocRequest
                    {
                        accessRequests = accessRequests,
                        adHocAccessRequests = adhocAccessRequests
                    }).ToList();
        }



        public List<AdHocAccessRequest> GetPendingByRequesteeUserId(long requesteeUserId)
        {
            var preList = (from adhocAccessRequests in this.CurrentContext.AdHocAccessRequests.Include("EndPoint")
                           where
                              (
                              adhocAccessRequests.RequesteeUserId == requesteeUserId &&
                              adhocAccessRequests.AccountType == AccountType.User)
                           select adhocAccessRequests
                        ).ToList();

            var List = (from adhocAccessRequests in this.CurrentContext.AdHocAccessRequests.Include("EndPoint")
                        join accessRequests in this.CurrentContext.AccessRequest
                        on adhocAccessRequests.Id equals accessRequests.AdHocAccessRequestId
                        where
                            (
                            adhocAccessRequests.RequesteeUserId == requesteeUserId &&
                            adhocAccessRequests.AccountType == AccountType.User &&
                            (accessRequests.ProgressStatus == AccessRequestProgress.WaitingForSync || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval
                            || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment || accessRequests.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                            || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute))
                        select adhocAccessRequests
                        ).ToList();
            return List;
        }

        public List<AdHocAccessRequest> GetAdocRequestAboutToExpiry(int days)
        {
            var validTo = DateTime.Now.AddDays(days);
            var validfrom = DateTime.Now.Date;

            var List = (from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where ((validfrom < adhocAccessRequests.ValidTo.Value) && (validTo > adhocAccessRequests.ValidTo.Value) && (accessRequests.ProgressStatus == AccessRequestProgress.Completed) && (adhocAccessRequests.IsÉxpiryMailSend == false) &&
                        ((adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount) || (adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentAdd)))
                        select adhocAccessRequests
                        ).ToList();
            return List;
        }

        public List<AdHocAccessRequest> GetExpiredAdocRequest()
        {
            var List = (from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        where ((adhocAccessRequests.ValidTo < DateTime.Now) && (accessRequests.ProgressStatus == AccessRequestProgress.Completed)
                        && (adhocAccessRequests.IsÉxpiryHandled == false)
                        && ((adhocAccessRequests.AdhocActionType == AdHocActionType.NewAccount)
                            || (adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentAdd)))
                        select adhocAccessRequests
                        ).ToList();
            return List;
        }


        public bool GetExpiredRemoveAdhocRequest(AdHocAccessRequest adhocRequest)
        {
            bool canRequest = true;
            var List = (from accessRequests in this.CurrentContext.AccessRequest
                        join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
                        on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
                        join task in this.CurrentContext.Tasks
                        on adhocAccessRequests.Id equals task.AccessRequestId
                        where ((task.Action == StatusType.Pending || accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute) &&
                        (adhocAccessRequests.IsÉxpiryHandled == false) && ((adhocAccessRequests.AdhocActionType == AdHocActionType.DeleteAccount) || (adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentRemove)) &&
                        (adhocAccessRequests.RequesteeUserId == adhocRequest.RequesteeUserId) && (adhocAccessRequests.ValidTo >= adhocRequest.ValidTo) &&
                        ((adhocRequest.AdhocActionType == AdHocActionType.NewAccount ? (adhocAccessRequests.AdhocActionType == AdHocActionType.DeleteAccount ? true : false) : (adhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd ? (adhocAccessRequests.AdhocActionType == AdHocActionType.AmendmentRemove ? true : false) : false))))

                        select adhocAccessRequests
                        ).ToList();
            foreach (AdHocAccessRequest req in List)
            {
                canRequest = (GetAccessRequestDetail(req.EntitlementDetail) == GetAccessRequestDetail(adhocRequest.EntitlementDetail)) ? false : true;
            }
            return canRequest;
        }

        //public AdHocAccessRequest GetRemoveAdocRequest(DateTime validTo, AdHocActionType adhocActionType, long userId)
        //{
        //    var adHocAccessRequest = (from accessRequests in this.CurrentContext.AccessRequest
        //                              join adhocAccessRequests in this.CurrentContext.AdHocAccessRequests
        //                              on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.Id
        //                              where (((accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval) || (accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute) ||
        //                              (accessRequests.ProgressStatus == AccessRequestProgress.Executing)) && (adhocAccessRequests.AdhocActionType == adhocActionType) &&
        //                              (adhocAccessRequests.IsÉxpiryHandled == false) && (adhocAccessRequests.RequesteeUserId == userId))
        //                              select adhocAccessRequests
        //                ).FirstOrDefault();

        //    return adHocAccessRequest;
        //}

        public string GetAccessRequestDetail(string EntitlementDetail)
        {
            if (string.IsNullOrEmpty(EntitlementDetail))
                return null;
            return ((List<EntityModel.Presentation.PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<EntityModel.Presentation.PresentationAccessRequestItem>(EntitlementDetail)).FirstOrDefault().ItemName;
        }
    }
}

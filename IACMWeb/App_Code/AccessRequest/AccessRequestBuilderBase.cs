
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using Application;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.Web
{
    public class AccessRequestBuilderBase
    {
        public bool ConsolidateRequests = true;
        public string ValidationMessage = string.Empty;
        public bool IsEmergencyRequest { get; set; }
        public List<long> AllowedItem { get; set;}
        public bool IsCheckboxChecked { get; set; }
        public AccessRequestBuilderBase()
        {
            AllowedItem = new List<long>();
        }
        
        public SessionStatusImpl Session = new SessionStatusImpl();

        public Dto.AccessRequest GetUpdatedAccessRequest(AdHocAccessRequest adhocRequest)
        {

            Dto.AccessRequest accessrequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, new Dictionary<string, string> { { "adhocAccessRequestId", adhocRequest.Id.ToString() } });

            if (accessrequest == null) return GenerateAccessRequest(adhocRequest);

            accessrequest.Comments = adhocRequest.Comments;
            accessrequest.DeactivationDate = adhocRequest.ValidTo;
            accessrequest.RequestSource = AccessRequestSource.AdhocRequest;
            accessrequest.RequestStatus = AccessRequestStatus.Submitted;
            accessrequest.ProgressStatus = AccessRequestProgress.WaitingforApproval;
            accessrequest.UserId = adhocRequest.RequesteeUserId;
            accessrequest.EndpointSystemId = adhocRequest.EndpointSystemId.Value;
            accessrequest.RequesterId = Session.User.Id;
            accessrequest.AccountType = adhocRequest.AccountType;
            accessrequest.ApplicationType = adhocRequest.ApplicationType;
            accessrequest.AdHocAccessRequest = adhocRequest;
            accessrequest.InterimIds = adhocRequest.InterimStoreId.ToString();
            return accessrequest;
        }
        public Dto.AccessRequest GenerateAccessRequest(AdHocAccessRequest adhocRequest)
        {

            Dto.AccessRequest accessrequest = new Dto.AccessRequest
            {
                TransactionId = Guid.NewGuid(),
                ActivationDate = adhocRequest.ValidFrom,
                Comments = adhocRequest.Comments,
                DeactivationDate = adhocRequest.ValidTo,
                RequestSource = AccessRequestSource.AdhocRequest,
                RequestStatus = AccessRequestStatus.Submitted,
                ProgressStatus = AccessRequestProgress.WaitingforApproval,
                UserId = adhocRequest.RequesteeUserId,
                EndpointSystemId = adhocRequest.EndpointSystemId.Value,
                RequesterId = Session.User.Id,
                AccountType = adhocRequest.AccountType,
                ApplicationType = adhocRequest.ApplicationType,
                AdHocAccessRequest = adhocRequest,
                InterimIds = adhocRequest.InterimStoreId.ToString()
            };
            return accessrequest;
        }
        public IList<AdHocAccessRequest> GetFromInterim()
        {
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            IList<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();

            SessionStatusImpl sessionStatus = new SessionStatusImpl();

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", sessionStatus.User.Id.ToString());
            parameter.Add("isSaved", Boolean.TrueString);
            parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());

            IList<InterimStore> listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>(DomainApiAction.Tasks.InterimStore.GetByUserId, parameter);

            if (listinterim != null)
            {
                for (int i = 0; i < listinterim.Count; i++)
                {
                    adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);

                    adhocaccessrequest.Id = listinterim[i].Id;

                    Dictionary<string, string> eparameter = new Dictionary<string, string>();
                    eparameter.Add("id", adhocaccessrequest.EndpointSystemId.Value.ToString());

                    DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, eparameter);


                    listadhoc.Add(adhocaccessrequest);
                }
            }
            return listadhoc;
        }

        public virtual bool IsExists(AdHocAccessRequest adhocAccessRequest)
        {
            ValidationMessage = string.Empty;

            AdHocAccessRequest isExistsRequestInInterimStore;
            long RequesterID = 0;
            long AccessRequestID = 0;

            isExistsRequestInInterimStore = GetFromInterim().
                   FirstOrDefault(item =>
              item.RequesteeUserId == adhocAccessRequest.RequesteeUserId &&
              item.AdhocActionType == adhocAccessRequest.AdhocActionType &&
              item.EntitlementDetail == ((adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount) ? null : adhocAccessRequest.EntitlementDetail) &&
              item.EndpointSystemType == adhocAccessRequest.EndpointSystemType &&
              item.AccountType == adhocAccessRequest.AccountType &&
              item.EndpointSystemId == adhocAccessRequest.EndpointSystemId);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("requesterId", RequesterID.ToString());
            parameter.Add("accessRequestId", AccessRequestID.ToString());
            string ids = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<string>(DomainApiAction.AdhocRequest.IsSameRequestPending, parameter, adhocAccessRequest);
            bool isExistsAccessRequest = false;
            if (!String.IsNullOrEmpty(ids))
            {
                isExistsAccessRequest = true;
                RequesterID = Convert.ToInt64(ids.Split(';')[0]);
                AccessRequestID = Convert.ToInt64(ids.Split(';')[1]);
            }
            if (isExistsRequestInInterimStore != null)
            {
                RequesterID = isExistsRequestInInterimStore.RequesterUserId;
                ValidationMessage = string.Format("Same access already requested in the current request list");
                return true;
            }

            if (isExistsAccessRequest)
            {

                Dictionary<string, string> eparameter = new Dictionary<string, string>();
                eparameter.Add("endpointUserID", RequesterID.ToString());
                eparameter.Add("endpointSystemName", "DIUser");

                string RequesterName = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.GetEndPointDetailByID, eparameter);

                if (AccessRequestID > 0)
                    ValidationMessage = string.Format("Same access already requested by {0} Access Request Id {1}", RequesterName, AccessRequestID);
                return true;
            }


            return false;
        }
                                                                                                                                                                
        public virtual bool Save(Dto.AdHocAccessRequest adhocRequest)
        {
            Dto.InterimStore interimstore = new Dto.InterimStore();
            interimstore.InterimStoreItemType = InterimStoreItemType.AccessRequest;
            interimstore.IsActive = true;
            interimstore.IsSaved = true;
            interimstore.UserId = Session.User.Id;
            interimstore.XmlData = UtilityMethods.Serialize<Dto.AdHocAccessRequest>(adhocRequest);
            DomainApiClient<Dto.InterimStore>.Save(interimstore);
            return true;
        }
        public void CleanupInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Session.User.Id.ToString());
            parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());          
            DomainApiClient<Dto.InterimStore>.GetByAction<object>(DomainApiAction.Tasks.InterimStore.DeleteByUserId, parameter);
        }

        public void CleanUpInterinStoreByInterimID(List<long> allowedItems)
        {
           
                Dictionary<string, string> parameter = new Dictionary<string, string>();
               // parameter.Add("interimId", interimId.ToString());
                parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());
                DomainApiClient<Dto.InterimStore>.GetByAction<object>(DomainApiAction.Tasks.InterimStore.DeleteByInterimId, parameter, allowedItems);
            
        }
        
        public void CleanupInterimStoreDuringAttestationReview(List<long> userid)
        {
            foreach (long userId in userid)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", userId.ToString());
                DomainApiClient<Dto.InterimStore>.GetByAction<object>(DomainApiAction.Tasks.InterimStore.DeleteByUserId, parameter);
            }

        }


        public virtual string CreateRemoveAccessRequests(List<PresentationAttestationEntitlementReviewDetail> userDetail)
        {
            return string.Empty;
        }
        public virtual string CreateAccessRequests()
        {
            return string.Empty;
        }

        public List<Dto.AdHocAccessRequest> GetListFromInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Session.User.Id.ToString());


            List<Dto.AdHocAccessRequest> listAdHocAccessRequests = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<Dto.AdHocAccessRequest>>
                (DomainApiAction.AdhocRequest.GetRequestsFromInterimStore, parameter);


            return listAdHocAccessRequests;
        }
        public List<Dto.AdHocAccessRequest> GetRemoveListFromInterimStore(long userId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", userId.ToString());
            List<Dto.AdHocAccessRequest> listAdHocAccessRequests = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<Dto.AdHocAccessRequest>>
                (DomainApiAction.AdhocRequest.GetRemoveRequestsFromInterimStore, parameter);
            return listAdHocAccessRequests;
        }
    }
}
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web
{
    public class DefaultAccessRequestBuilder : AccessRequestBuilderBase
    {

        private Dto.AccessRequest SelectApplicationRequestTypeIfAny(List<Dto.AccessRequest> list, Dto.AdHocAccessRequest adhocRequest)
        {
            if (adhocRequest.AccountType != AccountType.User)
            {
                dynamic detail = JsonConvert.DeserializeObject(adhocRequest.RequestedItems.FirstOrDefault().AdditionalInfo);
                var adhocRequestAccountName = detail == null ? adhocRequest.RequestedItems.FirstOrDefault().EndpointAccountName : detail["AccountName"];

                return list.FirstOrDefault(lq => lq.AdHocAccessRequest.EndpointSystemId == adhocRequest.EndpointSystemId
                     && lq.AdHocAccessRequest.RequesteeUserId == adhocRequest.RequesteeUserId
                     && (lq.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || lq.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                     && lq.AdHocAccessRequest.AccountType == adhocRequest.AccountType
                     && lq.AdHocAccessRequest.RequestedItems.Where(a => a.EndpointUserId == adhocRequest.RequestedItems.First().EndpointUserId).Any()
                     && lq.AdHocAccessRequest.RequestedItems.Where(a => (string.IsNullOrEmpty(a.AdditionalInfo) ? a.EndpointAccountName : ((dynamic)JsonConvert.DeserializeObject(a.AdditionalInfo))["AccountName"]) == adhocRequestAccountName).Any());
            }
            else
            {
                return list.FirstOrDefault(lq => lq.AdHocAccessRequest.EndpointSystemId == adhocRequest.EndpointSystemId
                     && lq.AdHocAccessRequest.RequesteeUserId == adhocRequest.RequesteeUserId
                     && (lq.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || lq.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                     && lq.AdHocAccessRequest.AccountType == adhocRequest.AccountType
                     && (lq.AdHocAccessRequest.RequestedItems.Count == 0 ||
                     lq.AdHocAccessRequest.RequestedItems.Where(a => a.EndpointUserId == adhocRequest.RequestedItems.First().EndpointUserId).Any()));
            }
        }
        private Dto.AccessRequest SelectExistingRequestTypeIfAny(List<Dto.AccessRequest> list, Dto.AdHocAccessRequest adhocRequest)
        {
            if (adhocRequest.AccountType != AccountType.User)
            {
                dynamic detail = JsonConvert.DeserializeObject(adhocRequest.RequestedItems.FirstOrDefault().AdditionalInfo);
                var adhocRequestAccountName = detail == null ? adhocRequest.RequestedItems.FirstOrDefault().EndpointAccountName : detail["AccountName"];

                var req = list.FirstOrDefault(lq => lq.AdHocAccessRequest.EndpointSystemId == adhocRequest.EndpointSystemId
                            && lq.AdHocAccessRequest.RequesteeUserId == adhocRequest.RequesteeUserId
                            && lq.AdHocAccessRequest.AdhocActionType == adhocRequest.AdhocActionType
                            && lq.AdHocAccessRequest.AccountType == adhocRequest.AccountType
                            && lq.AdHocAccessRequest.RequestedItems.Where(a => a.EndpointUserId == adhocRequest.RequestedItems.First().EndpointUserId).Any()
                            && lq.AdHocAccessRequest.RequestedItems.Where(a => (string.IsNullOrEmpty(a.AdditionalInfo) ? a.EndpointAccountName : ((dynamic)JsonConvert.DeserializeObject(a.AdditionalInfo))["AccountName"]) == adhocRequestAccountName).Any());

                return req;
            }
            else
            {
                var req = list.FirstOrDefault(lq => lq.AdHocAccessRequest.EndpointSystemId == adhocRequest.EndpointSystemId
                            && lq.AdHocAccessRequest.RequesteeUserId == adhocRequest.RequesteeUserId
                            && lq.AdHocAccessRequest.AdhocActionType == adhocRequest.AdhocActionType
                            && lq.AdHocAccessRequest.AccountType == adhocRequest.AccountType
                            && lq.AdHocAccessRequest.RequestedItems.Where(a => a.EndpointUserId == adhocRequest.RequestedItems.First().EndpointUserId).Any());

                return req;
            }
        }

        private void GetAccountRequests(List<Dto.AccessRequest> listAccessRequests, List<Dto.AdHocAccessRequest> listAdhocAccessRequests, AdHocActionType adhocActionType)
        {
            var accountsRequest = new List<Dto.AdHocAccessRequest>();
            if (adhocActionType != AdHocActionType.None)
                accountsRequest = listAdhocAccessRequests.Where(a => a.AdhocActionType == adhocActionType).ToList();
            else
                accountsRequest = listAdhocAccessRequests.Where(a => a.AdhocActionType == AdHocActionType.NewAccount || a.AdhocActionType == AdHocActionType.DeleteAccount).ToList();
            if (accountsRequest.Any())
            {
                foreach (var acRequest in accountsRequest)
                {
                    var accessReqObject = GenerateAccessRequest(acRequest);

                    if (acRequest.RequestedItems != null && acRequest.RequestedItems.Count > 0)
                        accessReqObject.ParentRequestId = acRequest.RequestedItems.First().ParentAccessRequestId;

                    //accessReqObject.AccountType = accessReqObject.AdHocAccessRequest.AccountType;
                    listAccessRequests.Add(accessReqObject);
                }
            }


            //foreach (var acRequest in listAdhocAccessRequests)
            //{
            //    var accessReqObject = GenerateAccessRequest(acRequest);
            //    //accessReqObject.AccountType = accessReqObject.AdHocAccessRequest.AccountType;
            //    listAccessRequests.Add(accessReqObject);
            //}

        }
        private void ConsolidateAccessRequest(Dto.AccessRequest accessRequest, Dto.AdHocAccessRequest adhocAccessRequest)
        {
            var requestedItems = accessRequest.AdHocAccessRequest.RequestedItems;

            requestedItems.AddRange(adhocAccessRequest.RequestedItems);

            accessRequest.AdHocAccessRequest.SetRequestedItems = requestedItems;

            if (adhocAccessRequest.RequestedItems != null && adhocAccessRequest.RequestedItems.Count > 0)
            {
                if (!accessRequest.ParentRequestId.HasValue)
                    accessRequest.ParentRequestId = adhocAccessRequest.RequestedItems.First().ParentAccessRequestId;
            }

            accessRequest.InterimIds = accessRequest.InterimIds + "," + adhocAccessRequest.InterimStoreId.ToString();
        }
        private void BuildSingleAccessRequest(List<Dto.AccessRequest> listAccessRequests, Dto.AdHocAccessRequest adhocAccessRequest)
        {
            var accessReqObject = GenerateAccessRequest(adhocAccessRequest);
            accessReqObject.AccountType = accessReqObject.AdHocAccessRequest.AccountType;
            accessReqObject.ApplicationType = accessReqObject.AdHocAccessRequest.ApplicationType;

            if (adhocAccessRequest.RequestedItems != null && adhocAccessRequest.RequestedItems.Count > 0)
                accessReqObject.ParentRequestId = adhocAccessRequest.RequestedItems.First().ParentAccessRequestId;

            listAccessRequests.Add(accessReqObject);
        }
        private void GetEntitlementRequests(List<Dto.AccessRequest> listAccessRequests, List<Dto.AdHocAccessRequest> listAdhocAccessRequests, AdHocActionType adhocActionType)
        {
            var entitlementsRequest = new List<Dto.AdHocAccessRequest>();
            if (adhocActionType != AdHocActionType.None)
                entitlementsRequest = listAdhocAccessRequests.Where(a => a.AdhocActionType == adhocActionType).ToList();
            else
                entitlementsRequest = listAdhocAccessRequests.Where(a => a.AdhocActionType == AdHocActionType.AmendmentAdd || a.AdhocActionType == AdHocActionType.AmendmentRemove).ToList();


            if (entitlementsRequest.Any())
            {
                //load all the endpoints in the cart
                var endpointSystemIdList = listAdhocAccessRequests.Select(a => a.EndpointSystemId).Distinct().ToList();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                var epList = DomainApiClient<Dto.EndPoint>.
                GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetEndpointSystemByIds, parameter, endpointSystemIdList);

                foreach (var entRequest in entitlementsRequest)
                {
                    var existingAppRequest = SelectApplicationRequestTypeIfAny(listAccessRequests, entRequest);

                    if (existingAppRequest != null)
                    {
                        var epInProcess = epList.Where(a => a.Id == existingAppRequest.EndpointSystemId).FirstOrDefault();
                        var accessRequestDetails = entRequest.GetAccessRequestDetail();
                        if (accessRequestDetails != null && accessRequestDetails.First().AccessItemType == AccessItemType.Catalog)
                        {
                            BuildSingleAccessRequest(listAccessRequests, entRequest);
                        }
                        else
                        {
                            if (ConsolidateRequests == true)
                                ConsolidateAccessRequest(existingAppRequest, entRequest);
                            else
                                BuildSingleAccessRequest(listAccessRequests, entRequest);
                        }
                    }
                    else
                    {
                        var existingEntitlementRequest = SelectExistingRequestTypeIfAny(listAccessRequests, entRequest);
                        if (existingEntitlementRequest != null)
                        {
                            var epInProcess = epList.Where(a => a.Id == existingEntitlementRequest.EndpointSystemId).FirstOrDefault();
                            var accessRequestDetails = entRequest.GetAccessRequestDetail();
                            if (accessRequestDetails != null && accessRequestDetails.First().AccessItemType == AccessItemType.Catalog)
                            {
                                BuildSingleAccessRequest(listAccessRequests, entRequest);
                            }
                            else
                            {
                                if (ConsolidateRequests == true)
                                    ConsolidateAccessRequest(existingEntitlementRequest, entRequest);
                                else
                                    BuildSingleAccessRequest(listAccessRequests, entRequest);
                            }
                        }
                        else
                            BuildSingleAccessRequest(listAccessRequests, entRequest);
                    }

                }
            }
        }

        public override string CreateRemoveAccessRequests(List<PresentationAttestationEntitlementReviewDetail> userDetail)
        {
            try
            {
                List<long> userIds = userDetail.Select(x => x.RequesteeUserId).Distinct().ToList();
                foreach (long UserId in userIds)
                {
                    List<PresentationAttestationEntitlementReviewDetail> lst = userDetail.Where(x => x.RequesteeUserId == UserId).ToList();
                    List<long> endpointSystemIds = lst.Select(x => x.EndpointSystemId).ToList();
                    foreach (long endpointId in endpointSystemIds)
                    {
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("endpointId", endpointId.ToString());
                        DomainApiClient<Dto.AccessRequest>.GetByAction<System.Object>("SaveRemoveRequest", parameters, lst);
                    }
                }
                //CleanupInterimStoreDuringAttestationReview(userIds);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }
        }

        public override string CreateAccessRequests()
        {
            List<Dto.AccessRequest> finalAccessRequestsList = new List<Dto.AccessRequest>();

            var interimAdhocList = GetListFromInterimStore();
            foreach (var adhocRequest in interimAdhocList)
            {
                if (AllowedItem != null && AllowedItem.Count > 0)
                {
                    if (!AllowedItem.Contains(adhocRequest.InterimStoreId)) continue;

                    adhocRequest.ValidTo = adhocRequest.ValidTo.HasValue && adhocRequest.ValidTo.Value.ToShortDateString() == "31-12-9999" ? DateTime.MaxValue : adhocRequest.ValidTo;
                    if (adhocRequest.Id > 0)
                    {
                        Dto.AccessRequest accessrequest = GetUpdatedAccessRequest(adhocRequest);
                        accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
                        finalAccessRequestsList.Add(accessrequest);
                    }
                    else
                    {
                        Dto.AccessRequest accessrequest = GenerateAccessRequest(adhocRequest);
                        accessrequest.ProgressStatus = AccessRequestProgress.Submitted;
                        finalAccessRequestsList.Add(accessrequest);
                    }
                }

            }
            List<long> savedAccessRequestIdList = DomainApiClient<Dto.AccessRequest>.
                                                             GetByAction<List<long>>("SaveAccessRequestListFromAdhocRequest", null,
                                                             finalAccessRequestsList);

            if (IsCheckboxChecked == true)
            {
                if(interimAdhocList.Count > 0 && interimAdhocList.Count == AllowedItem.Count)
                    CleanupInterimStore();
                else
                {
                    CleanUpInterinStoreByInterimID(AllowedItem);
                }
            }
            else
            {
                CleanUpInterinStoreByInterimID(AllowedItem);
            }
            

            string retString = string.Empty;
            if (savedAccessRequestIdList != null)
                retString = string.Join(",", savedAccessRequestIdList);
            return retString;
        }
        public string CreateAccessRequests_Duplicate()
        {
            var interimAdhocList = GetListFromInterimStore();

            List<Dto.AccessRequest> finalccessRequestsList = new List<Dto.AccessRequest>();

            var adhocListByEdPoint = interimAdhocList.GroupBy(item => item.EndpointSystemId)
                                    .Select(group => new { EndpointSystemId = group.Key, Items = group.ToList() })
                                    .ToList();

            foreach (var adhocListEP in adhocListByEdPoint)
            {

                string endpointId = Convert.ToString(adhocListEP.EndpointSystemId);
                var adhocList = adhocListEP.Items.ToList();

                List<long> interimIds = new List<long>();

                if (!adhocList.Any(a => a.AdhocActionType == AdHocActionType.DeleteAccount))
                {
                    adhocList.Where(a => a.AdhocActionType != AdHocActionType.NewAccount
                                     && a.AdhocActionType != AdHocActionType.DeleteAccount).ToList()
                        .ForEach(i =>
                               interimIds.Add(i.InterimStoreId)
                                );
                }

                if (interimIds.Count > 0)
                {
                    Dictionary<string, string> parameter1 = new Dictionary<string, string>();
                    parameter1.Add("interims", string.Join(",", interimIds));
                    parameter1.Add("endPointSystem", endpointId);

                    var entitlementListByApproverLevel = DomainApiClient<Dto.Presentation.PresentationApprovalChain>.
                       GetByAction<Dictionary<string, string>>("GetGroupEntitleMentsByApprover", parameter1);


                    foreach (var approverlevelEntitlement in entitlementListByApproverLevel)
                    {
                        List<int> interimList = Convert.ToString(approverlevelEntitlement.Value).Split(',').Select(x => int.Parse(x)).ToList();

                        var tempAdhocList = adhocList.Where(a => interimList.Any(i => a.InterimStoreId == i)).ToList();

                        List<Dto.AccessRequest> listAccessRequests = new List<Dto.AccessRequest>();

                        var accountsRequest = new List<Dto.AdHocAccessRequest>();
                        accountsRequest = adhocList.Where(a => a.AdhocActionType == AdHocActionType.NewAccount || a.AdhocActionType == AdHocActionType.DeleteAccount).ToList();

                        if (accountsRequest != null && accountsRequest.Count > 0)
                        {
                            accountsRequest.ForEach(a => { if (a.AccountType == AccountType.User) { a.EntitlementDetail = string.Empty; } });
                        }

                        GetAccountRequests(listAccessRequests, accountsRequest, AdHocActionType.None);

                        GetEntitlementRequests(listAccessRequests, tempAdhocList, AdHocActionType.None);

                        if (IsEmergencyRequest)
                        {
                            foreach (var request in listAccessRequests)
                            {
                                request.RequestStatus = AccessRequestStatus.Granted;
                                request.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                request.ExecutionTime = DateTime.Now.AddMinutes(1);
                            }
                        }

                        List<long> savedAccessRequestIdList = DomainApiClient<Dto.AccessRequest>.
                                                              GetByAction<List<long>>("SaveAccessRequestList", null,
                                                              listAccessRequests);


                    }
                }
                else
                {
                    List<Dto.AccessRequest> listAccessRequests = new List<Dto.AccessRequest>();

                    GetAccountRequests(listAccessRequests, adhocList, AdHocActionType.None);

                    GetEntitlementRequests(listAccessRequests, adhocList, AdHocActionType.None);

                    if (IsEmergencyRequest)
                    {
                        foreach (var request in listAccessRequests)
                        {
                            request.RequestStatus = AccessRequestStatus.Granted;
                            request.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                            request.ExecutionTime = DateTime.Now.AddMinutes(1);
                        }
                    }

                    List<long> savedAccessRequestIdList = DomainApiClient<Dto.AccessRequest>.GetByAction<List<long>>("SaveAccessRequestList", null, listAccessRequests);
                }
            }
            CleanupInterimStore();

            return string.Empty;
        }
    }
}

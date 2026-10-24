using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationInterimAdhocRequest : InterimStore
    {

        public AdHocAccessRequest AdhocAccessRequest { get; set; }
        public bool HasAttachment { get; set; }
        public string AttachmentFileName { get; set; }
        public string AdditionalInfoFirst { get; set; }
        public string AccessItemType { get; set; }
        public string RequestedBy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public static IList<PresentationInterimAdhocRequest> GetByUser(long userId, bool isSaved, InterimStoreItemType itemType)
        {
            IList<PresentationInterimAdhocRequest> interimAdhocRequests = new List<PresentationInterimAdhocRequest>();

            var interimList = PresentationInterimStore.GetPresentationByUserId(userId, isSaved, itemType);

            foreach (var item in interimList)
            {
                AdHocAccessRequest adhocReq = null; string accountName = ""; string accessItemType = "";
                string requestedBy = "";
                //if (item.TaskId.HasValue)
                //{
                //    var attachment = UtilityMethods.Deserialize<PresentationInterimAttachment>(item.XmlData);
                //    if (attachment != null)
                //        fileName = attachment.FileName;
                //}
                //else
                adhocReq = UtilityMethods.Deserialize<DomainModel.AdHocAccessRequest>(item.XmlData);
                if (adhocReq != null)
                {
                    IEntitlementDetail entitlementDetail = adhocReq.GetEntitlementDetail();
                    if (entitlementDetail != null)
                    {
                        if (entitlementDetail.SharedAccountDetail != null)
                        {
                            accountName = entitlementDetail.SharedAccountDetail.AccountName;
                        }
                        if (string.IsNullOrWhiteSpace(accountName) && !string.IsNullOrWhiteSpace(entitlementDetail.DisplayAccountName))
                        {
                            accountName = entitlementDetail.DisplayAccountName;
                        }
                        //else if (entitlementDetail is DI.Chakra.DomainModel.AdHocSAPLocalRequest)
                        //    accountName = ((DI.Chakra.DomainModel.AdHocSAPLocalRequest)entitlementDetail).AccountName;
                        else
                        {
                            //if(adhocReq.ApplicationType == ApplicationType.SRM && adhocReq.AdhocActionType == AdHocActionType.NewAccount)
                            //{
                            //    accountName = ((AdHocSAPSRMRequest)entitlementDetail).AccountName;
                            //}
                            //if (adhocReq.EndpointSystemId.HasValue && entitlementDetail.EndpointUserId > 0)
                            //{
                            //    accountName = AccountReconcilation.GetByEndpointWithAccountType(entitlementDetail.EndpointUserId, adhocReq.EndpointSystemId.Value, adhocReq.AccountType)?.AccountName;
                            //}
                        }

                    }
                    requestedBy = User.GetDisplayName(adhocReq.RequesterUserId);
                    //accessItemType = adhocReq.RequestedItems.Any() ? (adhocReq.RequestedItems.First().AccessItemType == Infrastructure.Enums.AccessItemType.Catalog ? Infrastructure.Enums.AccessItemType.Catalog.ToString() : 
                    //    (adhocReq.RequestedItems.First().AccessItemType == Infrastructure.Enums.AccessItemType.Application ? "" : adhocReq.RequestedItems.First().EntitlementType.ToString())) : "";
                    //dynamic additionalInfoObj = JsonConvert.DeserializeObject(adhocReq.RequestedItems[0].AdditionalInfo);
                    //if (additionalInfoObj != null)
                    //{
                    //    foreach (dynamic entry in additionalInfoObj)
                    //    {
                    //        string name = entry.Name;
                    //        string value = Convert.ToString(entry.Value);
                    //        accountName = value;
                    //        if (!string.IsNullOrEmpty(value))
                    //            break;

                    //    }
                    //}
                }

                var tt = new PresentationInterimAdhocRequest
                {
                    Id = item.Id,
                    InterimStoreItemType = item.InterimStoreItemType,
                    TaskId = item.TaskId,
                    UserId = item.UserId,
                    IsActive = item.IsActive,
                    IsSaved = item.IsSaved,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    LastModifiedBy = item.LastModifiedBy,
                    LastModifiedDate = item.LastModifiedDate,
                    XmlData = item.XmlData,
                    AdhocAccessRequest = adhocReq,
                    AdditionalInfoFirst = accountName,
                    RequestedBy = requestedBy

                };

                if (adhocReq != null && adhocReq.ValidFrom.HasValue)
                    tt.StartDate = adhocReq.ValidFrom.Value;

                if (adhocReq != null && adhocReq.ValidTo.HasValue)
                {
                    if (adhocReq.ValidTo != null && adhocReq.ValidTo.Value != DateTime.MinValue)
                        tt.EndDate = adhocReq.ValidTo.Value;
                }
                else
                    tt.EndDate = null;

                interimAdhocRequests.Add(tt);
            }

            //update the request for here not from UI
            if (interimAdhocRequests != null && interimAdhocRequests.Count > 0)
            {
                var requesteeUsers = interimAdhocRequests.Select(a => a.AdhocAccessRequest.RequesteeUserId).Distinct().ToList();

                foreach (var requesteeUser in requesteeUsers)
                {
                    var user = User.GetById(requesteeUser);
                    if (user != null)
                    {
                        interimAdhocRequests.Where(a => a.AdhocAccessRequest.RequesteeUserId == requesteeUser).ToList().ForEach(a =>
                        {
                            a.AdhocAccessRequest.DisplayName = user.DisplayName;
                            // if (string.IsNullOrEmpty(a.AdditionalInfoFirst)) { a.AdditionalInfoFirst = user.UserName; }
                        });
                    }
                }
            }

            return interimAdhocRequests;
        }
    }
}

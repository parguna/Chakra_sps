using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAccessRequestItem : IAccessRequestItem
    {
        public PresentationAccessRequestItem()
        {
            this.IsApproved = true;
            this.AdditionalInfo = string.Empty;
            this.AccountType = AccountType.User;
            this.ACRCategory = ACRCategory.Default;
        }
        public AccountType AccountType { get; set; }
        public long EndpointUserId { get; set; }
        public string EndpointAccountName { get; set; }
        public AccessRequestItemType AccessRequestItemType { get; set; }
        public AccessItemType AccessItemType { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public long KeyId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public string EndpointName { get; set; }
        public string EndpointCode { get; set; }
        public EndpointType EndpointType { get; set; }
        public bool IsMockEndpoint { get; set; }
        public string ItemDisplayName { get; set; }
        public string ItemName { get; set; }
        public string ItemDetail { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDetailDescription { get; set; }
        public bool IsApproved { get; set; }
        public string AdditionalInfo { get; set; }
        public ACRCategory ACRCategory { get; set; }
        public long RequestFor { get; set; }
        public bool Enabled { get; set; }
        public bool RemoveAccess { get; set; }
        public dynamic AdditionalInfoObj
        {
            get
            {
                if (AdditionalInfo != null)
                    return JsonConvert.DeserializeObject(AdditionalInfo);

                else
                    return null;
            }
        }
        public long? ParentAccessRequestId { get; set; }

        public static List<PresentationAccessRequestItem> GetByCriteria(PresentationAccessRequestFilterCriteria criteria)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> listAccessRequestItemAC = new List<PresentationAccessRequestItem>();
                List<PresentationAccessRequestItem> listAccessRequestItemEN = new List<PresentationAccessRequestItem>();

                if (criteria.RemoveAccess)
                {
                    if (criteria.AccountType == AccountType.User)
                        listAccessRequestItemAC = GetAccessRequestItemsForRemoveRequestOld(criteria);
                    else
                        listAccessRequestItemAC = GetAccessRequestItemsForRemoveRequestOldSpecailACR(criteria);
                }
                else
                {
                    if (criteria.IncludeApplications == true)
                    {
                        if (criteria.ApplicationEndpointSystems.Count == 0)
                        {
                            listAccessRequestItemAC =
                                PresentationAccessRequestItem.GetAllApplicationAccessRequest(criteria.SearchKey, null);
                        }
                        else
                        {
                            listAccessRequestItemAC =
                                PresentationAccessRequestItem.GetAllApplicationAccessRequest(criteria.SearchKey,
                                    criteria.ApplicationEndpointSystems);
                        }
                    }

                    if (criteria.IncludeEntitlements == true)
                    {

                        //Add special accounts
                        var spclAccountlist = ConfigurationMaster.GetConfigKey("EndpointNeedSplAccounts", Product.IM).Split(',');
                        List<long> spclAccounts = spclAccountlist.Select(long.Parse).ToList();
                        List<long> splEndpointIds = new List<long>();
                        foreach (var item in spclAccounts)
                        {
                            var ep = EndPoint.GetAllByEndpointTypeId(item);
                            if (ep != null)
                                splEndpointIds.AddRange(ep.Select(a => a.Id));
                        }

                        if (criteria.EntitlementEndpointSystems.Count > 0)
                        {
                            splEndpointIds = splEndpointIds.Where(a => criteria.EntitlementEndpointSystems.Contains(a)).ToList();
                        }

                        if (criteria.EntitlementEndpointSystems.Count == 0 || splEndpointIds.Any(a => criteria.EntitlementEndpointSystems.Contains(a)))
                        {
                            listAccessRequestItemAC =
                                PresentationAccessRequestItem.GetAllApplicationAccessRequest(criteria.SearchKey,
                                    splEndpointIds);
                        }

                        if (criteria.EntitlementEndpointSystems.Count == 0)
                        {

                            listAccessRequestItemEN =
                                PresentationAccessRequestItem.GetAllEntitlementForAccessRequest(criteria.SearchKey, criteria.AccessType, null, (long)criteria.DIUserId);

                            if (criteria.DIRequesterUserId.HasValue && criteria.DIRequesterUserId != criteria.DIUserId)
                            {
                                listAccessRequestItemEN.Union(PresentationAccessRequestItem.GetAllEntitlementForAccessRequest(criteria.SearchKey, criteria.AccessType, null, (long)criteria.DIRequesterUserId));
                            }
                        }
                        else
                        {
                            listAccessRequestItemEN =
                                PresentationAccessRequestItem.GetAllEntitlementForAccessRequest(criteria.SearchKey, criteria.AccessType,
                                    criteria.EntitlementEndpointSystems, (long)criteria.DIUserId);

                            if (criteria.DIRequesterUserId.HasValue && criteria.DIRequesterUserId != criteria.DIUserId)
                            {
                                listAccessRequestItemEN.Union(PresentationAccessRequestItem.GetAllEntitlementForAccessRequest(criteria.SearchKey, criteria.AccessType,
                                    criteria.EntitlementEndpointSystems, (long)criteria.DIRequesterUserId));
                            }

                            if (criteria.IncludeEntitlementTypes && criteria.EntitlementTypes.Count > 0)
                            {
                                listAccessRequestItemEN = listAccessRequestItemEN.Where(en => criteria.EntitlementTypes.Contains((int)en.EntitlementType)).ToList();
                            }
                        }
                    }
                }

                var returnlist = listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
                var interimlist = GetFromInterim(criteria.DIUserId.Value, InterimStoreItemType.AccessRequest);
                var pendinglist = AdHocAccessRequest.GetPendingByRequesteeUserId(criteria.DIUserId.Value); //GetPendingRequest(this.AdhocAccessRequest.RequesteeUserId);



                //disable the user account add option by default, only need to show the spl account features
                if (returnlist.Any(a => a.AccessItemType == AccessItemType.Application) && !criteria.RemoveAccess)
                {
                    returnlist.Where(a => a.AccessItemType == AccessItemType.Application).ToList().ForEach(a => { a.Enabled = false; });
                }

                // InterimFilter   
                var interimAccountAccessRequestList = interimlist.Where(lq => (lq.AdhocActionType == (criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).ToList();
                var interimEntitlementAccessRequestList = interimlist.Where(lq => (lq.AdhocActionType == (criteria.RemoveAccess ? AdHocActionType.AmendmentRemove : AdHocActionType.AmendmentAdd))).ToList();

                //update the enable flag 
                foreach (var item in interimAccountAccessRequestList)
                {
                    var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                                && a.AccessRequestItemType == AccessRequestItemType.Application && item.EndpointSystemId == a.EndpointSystemId);
                    if (temp.Any())
                        temp.First().Enabled = false;
                }

                foreach (var item in interimAccountAccessRequestList)
                {
                    var entilements = item.GetAccessRequestDetail();
                    if (entilements == null) continue;
                    foreach (var entitlement in entilements)
                    {
                        //var requestedItem = item.GetAccessRequestDetail().First();
                        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                                && entitlement.AccessRequestItemType == a.AccessRequestItemType && entitlement.KeyId == a.KeyId
                                && entitlement.AccessItemType == a.AccessItemType
                                && entitlement.EntitlementType == a.EntitlementType);
                        if (temp.Any())
                            temp.First().Enabled = false;
                    }
                }

                foreach (var item in interimEntitlementAccessRequestList)
                {
                    var entilements = item.GetAccessRequestDetail();
                    if (entilements == null) continue;
                    foreach (var entitlement in entilements)
                    {
                        //var requestedItem = item.GetAccessRequestDetail().First();
                        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                                && entitlement.AccessRequestItemType == a.AccessRequestItemType && entitlement.KeyId == a.KeyId
                                && entitlement.AccessItemType == a.AccessItemType
                                && entitlement.EntitlementType == a.EntitlementType);
                        if (temp.Any())
                            temp.First().Enabled = false;
                    }
                }

                // End
                // RequestFilter
                var pendingAccountAccessRequestList = pendinglist.Where(lq => (lq.AdhocActionType == (criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).ToList();
                var pendingEntitlementAccessRequestList = pendinglist.Where(lq => (lq.AdhocActionType == (criteria.RemoveAccess ? AdHocActionType.AmendmentRemove : AdHocActionType.AmendmentAdd))).ToList();

                foreach (var item in pendingAccountAccessRequestList)
                {
                    var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                    && a.AccessRequestItemType == AccessRequestItemType.Application && item.EndpointSystemId == a.EndpointSystemId);
                    if (temp.Any())
                        temp.First().Enabled = false;
                }

                //update the enable flag 
                foreach (var item in pendingAccountAccessRequestList)
                {
                    var entilements = item.GetAccessRequestDetail();
                    if (entilements == null) continue;
                    foreach (var entitlement in entilements)
                    {
                        //var requestedItem = item.GetAccessRequestDetail().First();
                        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                            && a.AccessRequestItemType == entitlement.AccessRequestItemType && entitlement.KeyId == a.KeyId
                            && entitlement.AccessItemType == a.AccessItemType
                            && entitlement.EntitlementType == a.EntitlementType);
                        if (temp.Any())
                            temp.First().Enabled = false;
                    }
                }

                foreach (var item in pendingEntitlementAccessRequestList)
                {
                    var entilements = item.GetAccessRequestDetail();
                    if (entilements == null) continue;
                    foreach (var entitlement in entilements)
                    {
                        // var requestedItem = item.GetAccessRequestDetail().First();
                        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (criteria.RemoveAccess ? item.AccountType : AccountType.User)
                        && a.AccessRequestItemType == entitlement.AccessRequestItemType && entitlement.KeyId == a.KeyId
                        && entitlement.AccessItemType == a.AccessItemType
                        && entitlement.EntitlementType == a.EntitlementType);
                        if (temp.Any())
                            temp.First().Enabled = false;
                    }
                }

                // AccountFilter
                if (!criteria.RemoveAccess)
                {
                    var accountlist = AccountReconcilation.GetAllByUser(criteria.DIUserId.Value, false).Where(x => !x.IsDeleted && !x.IsLocked).ToList(); //Presentation.PresentationUserAccessDetail.GetUserAccessDetail(criteria.DIUserId.Value); // GetAccount(AdhocAccessRequest.RequesteeUserId);

                    //update the enable flag 
                    foreach (var item in accountlist)
                    {
                        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == AccountType.User
                        && a.AccessRequestItemType == AccessRequestItemType.Application);
                        if (temp.Any())
                            temp.First().Enabled = false;
                    }
                }

                return returnlist;
            }
        }

        private static List<AdHocAccessRequest> GetFromInterim(long UserID, InterimStoreItemType interimType)
        {
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            List<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();
            IList<InterimStore> listinterim = InterimStore.GetByType(interimType).ToList();

            if (listinterim != null)
            {
                for (int i = 0; i < listinterim.Count; i++)
                {
                    adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);
                    adhocaccessrequest.Id = listinterim[i].Id;
                    if (adhocaccessrequest.RequesteeUserId == UserID)
                        listadhoc.Add(adhocaccessrequest);
                }
            }
            return listadhoc;
        }

        public static PresentationAccessRequestItem ORMap(EntityModel.Presentation.PresentationAccessRequestItem presentationAccessRequestItem)
        {
            if (presentationAccessRequestItem == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccessRequestItem, PresentationAccessRequestItem>(presentationAccessRequestItem);
        }
        public static List<PresentationAccessRequestItem> ORMapList(IList<EntityModel.Presentation.PresentationAccessRequestItem> presentationAccessRequestItems)
        {
            return presentationAccessRequestItems.Select(ORMap).ToList();
        }
        public static List<PresentationAccessRequestItem> GetAccessRequestItemsForRemoveRequestOld(PresentationAccessRequestFilterCriteria criteria)
        {
            List<PresentationAccessRequestItem> listAccessRequestItemAC = new List<PresentationAccessRequestItem>();
            List<PresentationAccessRequestItem> listAccessRequestItemEN = new List<PresentationAccessRequestItem>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC =
                            PresentationAccessRequestItem.GetAllApplicationForRemoveRequest(criteria.SearchKey, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemAC =
                            PresentationAccessRequestItem.GetAllApplicationForRemoveRequest(criteria.SearchKey,
                                criteria.EntitlementEndpointSystems, criteria.DIUserId.Value);
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN =
                            PresentationAccessRequestItem.GetAllEntitlementForRemoveRequest(criteria.SearchKey, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            PresentationAccessRequestItem.GetAllEntitlementForRemoveRequest(criteria.SearchKey,
                                criteria.EntitlementEndpointSystems, criteria.DIUserId.Value);

                        if (criteria.IncludeEntitlementTypes && criteria.EntitlementTypes.Count > 0)
                        {
                            listAccessRequestItemEN = listAccessRequestItemEN.Where(en => criteria.EntitlementTypes.Contains((int)en.EntitlementType)).ToList();
                        }

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
        }
        public static List<PresentationAccessRequestItem> GetAccessRequestItemsForRemoveRequestOldSpecailACR(PresentationAccessRequestFilterCriteria criteria)
        {

            List<PresentationAccessRequestItem> listAccessRequestItemAC = new List<PresentationAccessRequestItem>();
            List<PresentationAccessRequestItem> listAccessRequestItemEN = new List<PresentationAccessRequestItem>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.ApplicationEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC =
                            PresentationAccessRequestItem.GetAllApplicationForRemoveRequestSpecailACR(criteria, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemAC =
                            PresentationAccessRequestItem.GetAllApplicationForRemoveRequestSpecailACR(criteria,
                                criteria.ApplicationEndpointSystems, criteria.DIUserId.Value);
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN =
                            PresentationAccessRequestItem.GetAllEntitlementForRemoveRequestSpecailACR(criteria, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            PresentationAccessRequestItem.GetAllEntitlementForRemoveRequestSpecailACR(criteria,
                                criteria.EntitlementEndpointSystems, criteria.DIUserId.Value);

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
        }
        public static List<PresentationAccessRequestItem> GetAllApplicationAccessRequest(string searchString, List<long> endpointSystemIdList)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {


                if (endpointSystemIdList == null || endpointSystemIdList.Count == 0)
                {
                    return (from items in EndPoint.GetRequestableEndpoints()
                            where items.DisplayName.ToUpper().Contains(searchString.ToUpper())
                            && items.IsActive
                            orderby items.DisplayName
                            select new PresentationAccessRequestItem
                            {
                                AccessItemType = AccessItemType.Application,
                                AccessRequestItemType = AccessRequestItemType.Application,
                                EndpointType = (EndpointType)items.EndpointSystemTypeId,
                                KeyId = items.Id,
                                EndpointSystemId = items.Id,
                                EndpointSystemTypeId = items.EndpointSystemTypeId.Value,
                                ItemName = items.DisplayName,
                                ItemDescription = items.Description,
                                ItemDetail = items.Description,
                                EndpointName = items.DisplayName,
                                Enabled = true,
                            }).ToList();

                }
                List<PresentationAccessRequestItem> ReturnList = new List<PresentationAccessRequestItem>();
                foreach (long endpointSystemId in endpointSystemIdList)
                {
                    ReturnList.AddRange((from items in EndPoint.GetAll(true).Where(lq => !(lq.IsTrustedSource) && lq.IsActive)
                                         where items.Id == endpointSystemId && items.DisplayName.ToUpper().Contains(searchString.ToUpper())
                                         orderby items.DisplayName
                                         select new PresentationAccessRequestItem
                                         {
                                             AccessItemType = AccessItemType.Application,
                                             AccessRequestItemType = AccessRequestItemType.Application,
                                             EndpointType = (EndpointType)items.EndpointSystemTypeId,
                                             KeyId = items.Id,
                                             EndpointSystemId = items.Id,
                                             EndpointSystemTypeId = items.EndpointSystemTypeId.Value,
                                             ItemName = items.DisplayName,
                                             ItemDescription = items.Description,
                                             ItemDetail = items.Description,
                                             EndpointName = items.DisplayName,
                                             Enabled = true,
                                         }).ToList());
                }
                return ReturnList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllEntitlementForAccessRequest(string searchString, AccessItemType accessType, List<long> endpointSystemIdList, long userId)
        {
            bool isCatalog = false;

            if (accessType == AccessItemType.Catalog)
                isCatalog = true;
            var TrustedSource = EndPoint.GetAllTrustedSource();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();
                List<PresentationAccessRequestItem> accessRequestItem = new List<PresentationAccessRequestItem>();
                List<PresentationAccessRequestItem> FilteredaccessRequestItem = new List<PresentationAccessRequestItem>();

                if (endpointSystemIdList == null || endpointSystemIdList.Count == 0)
                {
                    if (accessRequestItem != null)
                        ManipulationList.AddRange(accessRequestItem);
                }
                else
                {
                    if (accessRequestItem != null)
                        ManipulationList.AddRange(accessRequestItem.Where(a => endpointSystemIdList.Contains(a.EndpointSystemId)));
                }

                List<EntitlementReconcilation> getLst = EntitlementReconcilation.GetByUser(userId).Where(a => a.AccountReconcilation.AccountType == AccountType.User).ToList();
                getLst.ForEach(er =>
                {
                    ManipulationList.Where(par => par.EndpointSystemId == er.EndpointSystemId && par.ItemName == er.Entitlement).ToList().ForEach(b => b.Enabled = false);
                });

                //Make Enable entitlements of Endpoint always selectable in access request
                var makeEPEntitlementAlwaysRequestable = ConfigurationMaster.GetConfigKey("MakeEPEntitlementAlwaysRequestable", Product.IACM);
                if (!string.IsNullOrEmpty(makeEPEntitlementAlwaysRequestable))
                {
                    foreach (var val in makeEPEntitlementAlwaysRequestable.Split(','))
                    {
                        try
                        {
                            long endpointId = 0; int entitlementType = 0;
                            if (!string.IsNullOrEmpty(val))
                            {
                                var spliVal = val.Split('-');
                                endpointId = Convert.ToInt64(spliVal[0]);
                                if (spliVal.Count() > 1)
                                    entitlementType = Convert.ToInt32(spliVal[1]);
                                if (ManipulationList.Any(a => a.EndpointSystemId == endpointId))
                                {
                                    ManipulationList.Where(a => a.EndpointSystemId == endpointId
                                    && (entitlementType == 0 || a.EntitlementType == (EntitlementType)entitlementType)).ToList().ForEach(a => { a.Enabled = true; });
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                TrustedSource.ForEach(l =>
                    {
                        ManipulationList.RemoveAll(lq => lq.EndpointSystemId == l.Id);
                    });

                ManipulationList = ManipulationList.Distinct().ToList();
                return ManipulationList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllApplicationForRemoveRequest(string searchString, List<long> endpointSystemIdList, long DIUserId)
        {
            var TrustedSource = EndPoint.GetAllTrustedSource();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();
                var listOfActiveAppForUser = AccountReconcilation.GetAllByUser(DIUserId).Where(a => (!a.IsDeleted && !a.IsLocked) && a.AccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase));

                ManipulationList.AddRange((from accountDetail in listOfActiveAppForUser
                                           select new PresentationAccessRequestItem
                                           {
                                               AccessRequestItemType = AccessRequestItemType.Application,
                                               AccessItemType = AccessItemType.Application,
                                               EndpointType = (EndpointType)accountDetail.EndPoint.EndpointSystemTypeId,
                                               KeyId = accountDetail.Id,
                                               EndpointSystemId = accountDetail.EndpointSystemId,
                                               EndpointSystemTypeId = (long)accountDetail.EndPoint.EndpointSystemTypeId,
                                               ItemName = accountDetail.AccountName,
                                               ItemDescription = accountDetail.EndPoint.DisplayName,
                                               ItemDetail = accountDetail.EndPoint.Description,
                                               EndpointUserId = accountDetail.EndpointUserId,
                                               EndpointAccountName = accountDetail.AccountName,
                                               AccountType = accountDetail.AccountType,
                                               EndpointName = accountDetail.EndPoint.DisplayName,
                                               Enabled = true,
                                               ItemDisplayName = string.Format("{0}({1})", accountDetail.AccountName, accountDetail.AccountType.ToString()),
                                               ItemDetailDescription = string.Format("Created On-{0}, Attested on-{1}", accountDetail.AccountCreatedDate, accountDetail.LastAttestedOn),
                                           }).ToList());

                List<long> endpointList = new List<long>();
                foreach (var item in endpointSystemIdList)
                {
                    var endpoint = EndPoint.GetById(item);
                    if (endpoint != null)
                    {
                        endpointList.Add(item);
                    }
                }
                endpointList = endpointList.Distinct().ToList();

                if (endpointList != null && endpointList.Count > 0)
                {
                    ManipulationList = ManipulationList.Where(a => endpointList.Contains(a.EndpointSystemId)).ToList();
                }

                TrustedSource.ForEach(l =>
                {
                    ManipulationList.RemoveAll(lq => lq.EndpointSystemId == l.Id);
                });
                return ManipulationList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllEntitlementForRemoveRequest(string searchString, List<long> endpointSystemIdList, long DIUserId)
        {
            var TrustedSource = EndPoint.GetAllTrustedSource();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();

                var listOfAccountsForUser = EntitlementReconcilation.GetByUser(DIUserId).Where(a => !a.IsDeleted && !a.AccountReconcilation.IsDeleted && !a.AccountReconcilation.IsLocked && a.Entitlement.Contains(searchString, StringComparison.OrdinalIgnoreCase));

                ManipulationList.AddRange((from entitlementDetail in listOfAccountsForUser
                                           select new PresentationAccessRequestItem
                                           {
                                               AccessRequestItemType = AccessRequestItemType.Entitlement,
                                               AccessItemType = AccessItemType.TechnicalRole,
                                               EndpointType = (EndpointType)entitlementDetail.EndpointType,
                                               KeyId = entitlementDetail.EntitlementId == 0 ? entitlementDetail.Id : entitlementDetail.EntitlementId,
                                               EndpointSystemId = entitlementDetail.EndpointSystemId,
                                               EndpointSystemTypeId = (long)entitlementDetail.EndpointType,
                                               ItemName = entitlementDetail.Entitlement,
                                               ItemDescription = entitlementDetail.Entitlement,
                                               ItemDetail = entitlementDetail.Entitlement,
                                               EntitlementType = (EntitlementType)entitlementDetail.EntitlementType,
                                               EndpointUserId = entitlementDetail.EndpointUserId,
                                               EndpointAccountName = entitlementDetail.AccountName,
                                               AccountType = entitlementDetail.AccountReconcilation.AccountType,
                                               EndpointName = entitlementDetail.EndPoint.DisplayName,
                                               EndpointCode = entitlementDetail.EndPoint.ApplicationCode,
                                               Enabled = true,
                                               ItemDisplayName = string.Format("{0}({1})", entitlementDetail.Entitlement, entitlementDetail.AccountName),
                                               ItemDetailDescription = string.Format("{0} - Created On-{1}, Attested on-{2}", entitlementDetail.EntitlementDescription, entitlementDetail.CreatedDate, entitlementDetail.LastAttestedOn),
                                           }).ToList());


                List<long> endpointList = new List<long>();
                foreach (var item in endpointSystemIdList)
                {
                    var endpoint = EndPoint.GetById(item);
                    if (endpoint != null)
                    {
                        endpointList.Add(item);
                    }
                }
                endpointList = endpointList.Distinct().ToList();
                if (endpointList != null && endpointList.Count > 0)
                {
                    ManipulationList = ManipulationList.Where(a => endpointList.Contains(a.EndpointSystemId)).ToList();
                }

                TrustedSource.ForEach(l =>
                {
                    ManipulationList.RemoveAll(lq => lq.EndpointSystemId == l.Id);
                });
                return ManipulationList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllApplicationForRemoveRequestSpecailACR(PresentationAccessRequestFilterCriteria criteria, List<long> endpointSystemIdList, long DIUserId)
        {
            var TrustedSource = EndPoint.GetAllTrustedSource();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();

                TrustedSource.ForEach(l =>
                {
                    ManipulationList.RemoveAll(lq => lq.EndpointSystemId == l.Id);
                });
                return ManipulationList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllEntitlementForRemoveRequestSpecailACR(PresentationAccessRequestFilterCriteria criteria, List<long> endpointSystemIdList, long DIUserId)
        {
            var TrustedSource = EndPoint.GetAllTrustedSource();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();

                TrustedSource.ForEach(l =>
                {
                    ManipulationList.RemoveAll(lq => lq.EndpointSystemId == l.Id);
                });
                return ManipulationList;
            }
        }

        public static List<PresentationAccessRequestItem> GetAllEntitlementForRemoveByReconcilationId(long accountId, long userId, long epId)
        {
            List<PresentationAccessRequestItem> ManipulationList = new List<PresentationAccessRequestItem>();
            var endpoint = EndPoint.GetById(epId);

            var listOfAccountsForUser = EntitlementReconcilation.GetByUser(userId).Where(a => !a.IsDeleted && !a.AccountReconcilation.IsDeleted && !a.AccountReconcilation.IsLocked && a.AccountReconcilationId == accountId);

            ManipulationList.AddRange((from entitlementDetail in listOfAccountsForUser
                                       select new PresentationAccessRequestItem
                                       {
                                           AccessRequestItemType = AccessRequestItemType.Entitlement,
                                           AccessItemType = AccessItemType.TechnicalRole,
                                           EndpointType = (EndpointType)entitlementDetail.EndpointType,
                                           KeyId = entitlementDetail.EntitlementId == 0 ? entitlementDetail.Id : entitlementDetail.EntitlementId,
                                           EndpointSystemId = endpoint.Id,
                                           EndpointSystemTypeId = (long)entitlementDetail.EndpointType,
                                           ItemName = entitlementDetail.Entitlement,
                                           ItemDescription = entitlementDetail.Entitlement,
                                           ItemDetail = entitlementDetail.Entitlement,
                                           EntitlementType = (EntitlementType)entitlementDetail.EntitlementType,
                                           EndpointUserId = entitlementDetail.EndpointUserId,
                                           EndpointAccountName = entitlementDetail.AccountName,
                                           AccountType = entitlementDetail.AccountReconcilation.AccountType,
                                           EndpointName = endpoint.DisplayName,
                                           EndpointCode = endpoint.ApplicationCode,
                                           Enabled = true,
                                           ItemDisplayName = string.Format("{0}({1})", entitlementDetail.Entitlement, entitlementDetail.AccountName),
                                           ItemDetailDescription = string.Format("{0} - Created On-{1}, Attested on-{2}", entitlementDetail.EntitlementDescription, entitlementDetail.CreatedDate, entitlementDetail.LastAttestedOn),
                                       }).ToList());
            return ManipulationList;
        }
    }
}

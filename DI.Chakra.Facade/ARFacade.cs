using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Facade
{
    
    
    public class ARFacade
    {
        private ARDA ARDA = new ARDA();
        private ReconcilationFacade reconFacade = new ReconcilationFacade();

        public List<AccessRequestItem> GetAccessRequestItemsForRemoveRequestOld(AccessRequestFilterCriteria criteria)
        {
            try
            {
                List<AccessRequestItem> listAccessRequestItemAC = new List<AccessRequestItem>();
                List<AccessRequestItem> listAccessRequestItemEN = new List<AccessRequestItem>();

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.ApplicationEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationForRemoveRequest(criteria.SearchKey, null,criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationForRemoveRequest(criteria.SearchKey,
                                criteria.ApplicationEndpointSystems, criteria.DIUserId.Value);
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForRemoveRequest(criteria.SearchKey, null,criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForRemoveRequest(criteria.SearchKey,
                                criteria.EntitlementEndpointSystems, criteria.DIUserId.Value);

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<AccessRequestItem> GetAccessRequestItemsForRemoveRequestOldSpecailACR(AccessRequestFilterCriteria criteria)
        {
            try
            {
                List<AccessRequestItem> listAccessRequestItemAC = new List<AccessRequestItem>();
                List<AccessRequestItem> listAccessRequestItemEN = new List<AccessRequestItem>();

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.ApplicationEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationForRemoveRequestSpecailACR(criteria, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationForRemoveRequestSpecailACR(criteria,
                                criteria.ApplicationEndpointSystems, criteria.DIUserId.Value);
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForRemoveRequestSpecailACR(criteria, null, criteria.DIUserId.Value);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForRemoveRequestSpecailACR(criteria,
                                criteria.EntitlementEndpointSystems, criteria.DIUserId.Value);

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
            catch
            {
                throw;
            }

        }

        public List<AccessRequestItem> GetAccessRequestItemsForRemoveRequest(AccessRequestFilterCriteria criteria)
        {
            try
            {
                IList<AccessRequestItem> listAccessRequestItemAC = new List<AccessRequestItem>();
                List<AccessRequestItem> listAccessRequestItemEN = new List<AccessRequestItem>();

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.ApplicationEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC = reconFacade.GetAccountReconcilations(criteria.DIUserId.Value, AccountType.User)
                                                    .Select(acRecon => new AccessRequestItem
                                                    { 
                                                         AccessRequestItemType = AccessRequestItemType.Application,
                                                         EndpointType = (EndpointType)acRecon.EndPoint.EndpointSystemTypeId.Value,
                                                         KeyId = acRecon.EndpointSystemId,
                                                         EndpointSystemId = acRecon.EndpointSystemId,
                                                         EndpointSystemTypeId = acRecon.EndPoint.EndpointSystemTypeId.Value,
                                                         ItemName = acRecon.AccountName,
                                                         ItemDescription = acRecon.FullName
                                                    }).
                                                    Where(t=>t.ItemName.Contains(criteria.SearchKey)).ToList();
                          
                    }
                    else
                    {
                        foreach (EndpointType epType in criteria.ApplicationEndpointSystems)
                        {
                            listAccessRequestItemAC = reconFacade.GetAccountReconcilations(criteria.DIUserId.Value, AccountType.User)
                                                    .Select(acRecon => new AccessRequestItem
                                                    {
                                                        AccessRequestItemType = AccessRequestItemType.Application,
                                                        EndpointType = (EndpointType)acRecon.EndPoint.EndpointSystemTypeId.Value,
                                                        KeyId = acRecon.EndpointSystemId,
                                                        EndpointSystemId = acRecon.EndpointSystemId,
                                                        EndpointSystemTypeId = acRecon.EndPoint.EndpointSystemTypeId.Value,
                                                        ItemName = acRecon.AccountName,
                                                        ItemDescription = acRecon.FullName
                                                    }).
                                                    Where(t => t.ItemName.Contains(criteria.SearchKey)
                                                    && t.EndpointType == epType).ToList();

                            listAccessRequestItemAC.Union(listAccessRequestItemAC).ToList();

                        }
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN = reconFacade.GetEntitlementReconcilations(criteria.DIUserId.Value, AccountType.User)
                                                     .Select(enRecon => new AccessRequestItem
                                                     {
                                                         AccessRequestItemType = AccessRequestItemType.Entitlement,
                                                         EndpointType = enRecon.EndpointType,
                                                         KeyId = enRecon.EndpointSystemId,
                                                         EndpointSystemId = enRecon.EndpointSystemId,
                                                         EndpointSystemTypeId = (int)enRecon.EndpointType,
                                                         ItemName = enRecon.Entitlement,
                                                         ItemDescription = enRecon.Entitlement
                                                     }).
                                                     Where(t => t.ItemName.Contains(criteria.SearchKey)).ToList();
                    }
                    else
                    {
                        foreach (EndpointType epType in criteria.EntitlementEndpointSystems)
                        {
                            listAccessRequestItemEN = reconFacade.GetEntitlementReconcilations(criteria.DIUserId.Value, AccountType.User)
                                                 .Select(enRecon => new AccessRequestItem
                                                 {
                                                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                                                     EndpointType = enRecon.EndpointType,
                                                     KeyId = enRecon.EndpointSystemId,
                                                     EndpointSystemId = enRecon.EndpointSystemId,
                                                     EndpointSystemTypeId = (int)enRecon.EndpointType,
                                                     ItemName = enRecon.AccountName,
                                                     ItemDescription = enRecon.AccountName
                                                 }).
                                                 Where(t => t.ItemName.Contains(criteria.SearchKey) && t.EndpointType == epType).ToList();

                            listAccessRequestItemEN.Union(listAccessRequestItemEN).ToList();

                        }

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
            catch
            {
                throw;
            }

        }
        public List<AccessRequestItem> GetAccessRequestItems(AccessRequestFilterCriteria criteria)
        {
            try
            {
                if (criteria.RemoveAccess)
                {
                    if(criteria.AccountType == AccountType.User)
                        return GetAccessRequestItemsForRemoveRequestOld(criteria);
                    else
                        return GetAccessRequestItemsForRemoveRequestOldSpecailACR(criteria);
                }

                List<AccessRequestItem> listAccessRequestItemAC = new List<AccessRequestItem>();
                List<AccessRequestItem> listAccessRequestItemEN = new List<AccessRequestItem>();

                if (criteria.IncludeApplications == true)
                {
                    if (criteria.ApplicationEndpointSystems.Count == 0)
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationAccessRequest(criteria.SearchKey, null);
                    }
                    else
                    {
                        listAccessRequestItemAC =
                            ARDA.GetAllApplicationAccessRequest(criteria.SearchKey,
                                criteria.ApplicationEndpointSystems);
                    }
                }

                if (criteria.IncludeEntitlements == true)
                {
                    if (criteria.EntitlementEndpointSystems.Count == 0)
                    {

                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForAccessRequest(criteria.SearchKey, criteria.AccessType, null);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForAccessRequest(criteria.SearchKey,criteria.AccessType, 
                                criteria.EntitlementEndpointSystems);

                    }
                }
                return listAccessRequestItemAC.Union(listAccessRequestItemEN).ToList();
            }
            catch
            {
                throw;
            }

        }

        //public IList<AccessRequestItem> GetAccessRequestItems(string searchString, int? EndpointSystemId)
        //{
        //    try
        //    {

        //        return new ActiveDirectoryDA().GetAllADSGroupsForAccessRequest(searchString, null);
        //    }
        //    catch
        //    {
        //        throw;
        //    }

        //    return null;
        //}

        public bool IsSecondartAccount(AdHocAccessRequest AdHocAccessRequest)
        {
            return ARDA.IsSecondartAccount(AdHocAccessRequest);
        }
    }
}

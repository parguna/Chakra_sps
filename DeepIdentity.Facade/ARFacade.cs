using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.DataAccess;

namespace DeepIdentity.Facade
{
    
    
    public class ARFacade
    {
        private ARDA ARDA = new ARDA();

        public List<AccessRequestItem> GetAccessRequestItemsForRemoveRequest(AccessRequestFilterCriteria criteria)
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
        public List<AccessRequestItem> GetAccessRequestItems(AccessRequestFilterCriteria criteria)
        {
            try
            {
                if(criteria.RemoveAccess)
                    return GetAccessRequestItemsForRemoveRequest(criteria);

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
                            ARDA.GetAllEntitlementForAccessRequest(criteria.SearchKey, null);
                    }
                    else
                    {
                        listAccessRequestItemEN =
                            ARDA.GetAllEntitlementForAccessRequest(criteria.SearchKey,
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

using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using DI.Chakra.Model.GridData;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Facade
{
    public class AdHocRequestFacade
    {
        AdHocRequestDA _requestDA = new AdHocRequestDA();

        public AdHocAccessRequest GetAdHocAccessRequestById(long requestId, bool deepLoad)
        {
            try
            {
                return _requestDA.GetAdHocAccessRequestById(requestId, deepLoad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CreateAdHocAccessRequest(AdHocAccessRequest request)
        {
            try
            {
                _requestDA.CreateAdHocAccessRequest(request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool AddAdHocRequest(IList<AdHocAccessRequest> list)
        {
            try
            {
                foreach (AdHocAccessRequest ar in list)
                {
                    _requestDA.AddAdHocRequest(ar);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        public IList<AdhocRequestGridData> GetAdhocRequestDetails()
        {
            try
            {
                IList<AdhocRequestGridData> adhocrequests = _requestDA.GetAdhocRequestDetails();

                return adhocrequests;
            }
            catch
            {
                throw;
            }

        }

        public IList<AdHocGroupManagementGridData> GetAdHocGroupManagementGridData(int endPointTypeId, int endPntId)
        {
            try
            {
                return _requestDA.GetAdHocGroupManagementGridData(endPointTypeId, endPntId);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateADLNSAdHocRequestGroupDetail(int endPointTypeId, int endPntId, IList<int> groupIdList, bool canRequest)
        {
            try
            {
                _requestDA.UpdateADLNSAdHocRequestGroupDetail(endPointTypeId, endPntId, groupIdList, canRequest);
            }
            catch
            {
                throw;
            }
        }

        public bool IsUserIsInternal(int UserID)
        {
            try
            {
                return _requestDA.IsUserIsInternal(UserID);

            }
            catch
            {
                throw;
            }
        }

        public bool UpdateEntiltement(AdHocAccessRequest adhocAccessRequest)
        {
            return _requestDA.UpdateEntiltement(adhocAccessRequest);
        }

        public MyRequestsGridData GetAdHocAccessRequestByAdhocId(long requestId, bool deepLoad)
        {
            try
            {
                return _requestDA.GetAdHocAccessRequestByAdhocId(requestId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        //public Dictionary<int, string> GetBusinessRoleFromRoleCatalog(string searchString, string subModule, string Module)
        //{
        //    try
        //    {
        //        return _requestDA.GetBusinessRoleFromRoleCatalog(searchString, subModule, Module);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

    }
}

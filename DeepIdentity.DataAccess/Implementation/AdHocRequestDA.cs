using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.GridData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class AdHocRequestDA
    {

        public AdHocAccessRequest GetAdHocAccessRequestById(long requestId, bool deepLoad)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (!deepLoad)
                    {
                        return oIACMDBContext.AdHocAccessRequests.FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                    }
                    return oIACMDBContext.AdHocAccessRequests.Include("EndPoint").FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreateAdHocAccessRequest(AdHocAccessRequest request)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.AdHocAccessRequests.Add(request);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool AddAdHocRequest(AdHocAccessRequest newAR)
        {
            bool isDataSaved = false;
            if (newAR == null)
                return isDataSaved;

            //string query = "DI_spAddAccessRequest";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.AdHocAccessRequests.Add(newAR);
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while adding new AccessRequest.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public IList<AdhocRequestGridData> GetAdhocRequestDetails()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from AdhocRequest in _context.AdHocAccessRequests

                            select new AdhocRequestGridData
                            {
                                EndpointSystemId = AdhocRequest.EndpointSystemId.Value,
                                EndpointSystemType = AdhocRequest.EndpointSystemType,
                                AdhocActionType = AdhocRequest.AdhocActionType,
                                CreatedDate = AdhocRequest.CreatedDate,
                                EntitlementDetail = AdhocRequest.EntitlementDetail
                            }

                            ).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<AdHocGroupManagementGridData> grpList = new List<AdHocGroupManagementGridData>();
                    if (endPointTypeId > 0 && endPntId > 0)
                    {
                        switch (endPointTypeId)
                        {
                            case (int)EndpointType.ActiveDirectory:
                                grpList = (from grp in _context.ADSGroupInfos
                                           where grp.EndPointSystemId == endPntId
                                           select new AdHocGroupManagementGridData
                                           {
                                               GroupId = grp.GroupId,
                                               FullName = grp.cn,
                                               Description = grp.description,
                                               CreatedDate = grp.whenChanged,
                                               CanRequest = grp.CanRequest.HasValue ? grp.CanRequest.Value : false
                                           }).ToList();
                                break;
                            case (int)EndpointType.LotusNotes:
                                grpList = (from grp in _context.LotusGroupInfos
                                           where grp.EndPointSystemId == endPntId
                                           select new AdHocGroupManagementGridData
                                           {
                                               GroupId = grp.Id,
                                               FullName = grp.Group,
                                               Description = grp.Description,
                                               CreatedDate = grp.CreatedDate,
                                               CanRequest = grp.CanRequest.HasValue ? grp.CanRequest.Value : false
                                           }).ToList();
                                break;
                        }

                    }
                    return grpList;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (endPointTypeId > 0 && endPntId > 0)
                    {
                        switch (endPointTypeId)
                        {
                            case (int)EndpointType.ActiveDirectory:
                                foreach (int grpId in groupIdList)
                                {
                                    ADSGroupInfo adGrp = _context.ADSGroupInfos.Where(grp => grp.GroupId == grpId).FirstOrDefault();
                                    if (adGrp != null)
                                    {
                                        adGrp.CanRequest = canRequest;
                                    }
                                }
                                _context.SaveChanges();
                                break;
                            case (int)EndpointType.LotusNotes:
                                foreach (int grpId in groupIdList)
                                {
                                    LotusGroupInfo lnsGrp = _context.LotusGroupInfos.Where(grp => grp.Id == grpId).FirstOrDefault();
                                    if (lnsGrp != null)
                                    {
                                        lnsGrp.CanRequest = canRequest;
                                    }
                                }
                                _context.SaveChanges();
                                break;
                        }

                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetBusinessRoleFromRoleCatalog(string searchString, string subModule, string Module)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPRoleCatalogs.Where(item => (item.BusinessRoleName.Contains(searchString)) && (string.IsNullOrEmpty(Module) ? 1 == 1 : item.Module == Module) && (string.IsNullOrEmpty(subModule) ? 1 == 1 : item.SubModule == subModule))
                            .Select(x => new { Key = x.Id, Value = x.BusinessRoleName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public bool IsUserIsInternal(int UserID)
        {
            var result = false;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var data = (from source in _context.Users where source.UserId == UserID select source.FullTimeEmployee).FirstOrDefault();
                    result = data.HasValue ? data.Value ? true : false : false;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public bool UpdateEntiltement(AdHocAccessRequest adhocAccessRequest)
        {
            try
            {
                using (IACMDBContext _IACMDBContext = new IACMDBContext())
                {
                    _IACMDBContext.AdHocAccessRequests.Attach(adhocAccessRequest);
                    _IACMDBContext.Entry(adhocAccessRequest).State = EntityState.Modified;
                    _IACMDBContext.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public MyRequestsGridData GetAdHocAccessRequestByAdhocId(long requestId, bool deepLoad)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (!deepLoad)
                    {
                        //return oIACMDBContext.AdHocAccessRequests.FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                        return (from rqst in oIACMDBContext.AdHocAccessRequests
                                join acssrqst in oIACMDBContext.AccessRequests
                                    on rqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                                where rqst.AdHocAccessRequestId == requestId
                                select new MyRequestsGridData
                                {
                                    AdHocAccessRequestId = rqst.AdHocAccessRequestId,
                                    AccessRequestID = acssrqst.AccessRequestId,
                                    Name = rqst.EndPoint.DisplayName + " - " + (((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? "Amendment" : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                    RequestedDate = rqst.CreatedDate,
                                    RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                    Comments = rqst.Comments,
                                    RequestStatus = acssrqst.RequestStatus,
                                    AdhocActionType = rqst.AdhocActionType,
                                    EndpointSystemId = rqst.EndpointSystemId,
                                    EndPoint = rqst.EndPoint,
                                    AdHocAccessRequests = rqst,
                                    StartDate = rqst.ValidFrom,
                                    EndDate = rqst.ValidTo
                                }
                                   ).FirstOrDefault();
                    }
                    //return oIACMDBContext.AdHocAccessRequests.Include("EndPoint").Include("MiscSystem").FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                    return (from rqst in oIACMDBContext.AdHocAccessRequests.Include("EndPoint")
                        join acssrqst in oIACMDBContext.AccessRequests
                            on rqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                        where rqst.AdHocAccessRequestId == requestId
                        select new MyRequestsGridData
                        {
                            AdHocAccessRequestId = rqst.AdHocAccessRequestId,
                            AccessRequestID = acssrqst.AccessRequestId,
                            Name = rqst.EndPoint.DisplayName + " - " + (((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? "Amendment" : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                            RequestedDate = rqst.CreatedDate,
                            RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                            Comments = acssrqst.Comments,
                            RequestStatus = acssrqst.RequestStatus,
                            AdhocActionType = rqst.AdhocActionType,
                            EndpointSystemId = rqst.EndpointSystemId,
                            EndPoint = rqst.EndPoint,
                            AdHocAccessRequests = rqst,
                            StartDate = rqst.ValidFrom,
                            EndDate = rqst.ValidTo
                        }
                        ).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

    }
}

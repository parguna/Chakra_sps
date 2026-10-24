using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class OUChangeSnapshotsRepository : RepositoryBase<OUChangeSnapshot>
    {
        public OUChangeSnapshotsRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<OUChangeSnapshot> GetByUserId(long userId)
        {
            List<OUChangeSnapshot> ousnapShotList = new List<OUChangeSnapshot>();
            try
            {

                ousnapShotList = CurrentContext.OUChangeSnapshots.Where(item => item.UserId == userId).OrderByDescending(item => item.EnteredDate).ToList();
                if (ousnapShotList.Count == 0)
                    return ousnapShotList;

                string LatestBatch = ousnapShotList[0].BatchId;
                ousnapShotList = ousnapShotList.Where(item => item.BatchId == LatestBatch && item.EmployeeeEventType == ousnapShotList[0].EmployeeeEventType).ToList();
                return ousnapShotList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OUChangeSnapshot GetByUserIdAccessRequestId(long userId, long accessRequestId)
        {
            return CurrentContext.OUChangeSnapshots.Where(item => item.UserId == userId && item.RequestId == accessRequestId).FirstOrDefault();
        }

        public IList<PresentationOUChangeSnapshots> GetByUserActionUserId(List<long> actionUserIds, bool includeOldRecord = false)
        {
            var dateString = DateTime.Now.Date.AddDays(-30);
            IList<PresentationOUChangeSnapshots> data = new List<PresentationOUChangeSnapshots>();
            IList<PresentationOUChangeSnapshots> ouchangesnapshot = new List<PresentationOUChangeSnapshots>();
            if (actionUserIds != null && actionUserIds.Count > 0)
            {
                foreach (long userid in actionUserIds)
                {
                    ouchangesnapshot = (from OUC in CurrentContext.OUChangeSnapshots
                                join user in CurrentContext.User
                                      on OUC.UserId equals user.Id
                                join Endpnt in CurrentContext.EndPoint
                                    on OUC.EndpointSystemId equals Endpnt.Id
                                where OUC.UserId == userid && (includeOldRecord || (includeOldRecord == false && OUC.CreatedDate > dateString))
                                select new PresentationOUChangeSnapshots
                                {
                                    UserName = user.DisplayName,
                                    EmployeeNo = user.EmployeeId,
                                    EmailID = user.EmailId,
                                    EndpointsystemName = Endpnt.DisplayName,
                                    EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                    AccountName = OUC.AccountName,//.sAMAccountName,
                                    EndpointUserID = OUC.EndpointUserId,
                                    Entitlement = OUC.Entitlement,
                                    EntitlementType = OUC.EntitlementType,
                                    CreatedDate = OUC.EnteredDate,
                                    EmployeeEventType = OUC.EmployeeeEventType,
                                    ApplicationType = Endpnt.ApplicationType
                                }).ToList();
                    //foreach (PresentationOUChangeSnapshots lst in list)
                    //{
                    //    ouchangesnapshot.Add(lst);
                    //}
                }
            }
            else
            {
               
                ouchangesnapshot = (from OUC in CurrentContext.OUChangeSnapshots
                                    join user in CurrentContext.User
                                      on OUC.UserId equals user.Id
                                    join Endpnt in CurrentContext.EndPoint
                                        on OUC.EndpointSystemId equals Endpnt.Id
                                    where  (includeOldRecord || (includeOldRecord == false && OUC.CreatedDate > dateString))
                                    select new PresentationOUChangeSnapshots
                                    {
                                        UserName = user.DisplayName,
                                        EmployeeNo = user.EmployeeId,
                                        EmailID = user.EmailId,
                                        EndpointsystemName = Endpnt.DisplayName,
                                        EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                        AccountName = OUC.AccountName,//.sAMAccountName,
                                        EndpointUserID = OUC.EndpointUserId,
                                        Entitlement = OUC.Entitlement,
                                        EntitlementType = OUC.EntitlementType,
                                        CreatedDate = OUC.EnteredDate,
                                        EmployeeEventType = OUC.EmployeeeEventType,
                                        ApplicationType = Endpnt.ApplicationType

                                    }).ToList();
            }
            return ouchangesnapshot;

            //foreach (PresentationOUChangeSnapshots item in ouchangesnapshot)
            //{
            //    if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.ActiveDirectory))
            //    {
            //        var account = (from OUC in CurrentContext.OUChangeSnapshots
            //                       join user in CurrentContext.User
            //                           on OUC.UserId equals user.Id
            //                       join Endpnt in CurrentContext.EndPoint
            //                           on OUC.EndpointSystemId equals Endpnt.Id
            //                       join endpntacc in CurrentContext.ADSUserInfos
            //                            on OUC.EndpointUserId equals endpntacc.Id
            //                       where OUC.Id == item.SnapshotId
            //                       select new PresentationOUChangeSnapshots
            //                       {
            //                           UserName = user.DisplayName,
            //                           EmployeeNo = user.EmployeeId,
            //                           EmailID = user.EmailId,
            //                           EndpointsystemName = Endpnt.DisplayName,
            //                           EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
            //                           AccountName = endpntacc.sAMAccountName,
            //                           EndpointUserID = OUC.EndpointUserId,
            //                           Entitlement = OUC.Entitlement,
            //                           EntitlementType = OUC.EntitlementType,
            //                           CreatedDate = OUC.EnteredDate,
            //                           EmployeeEventType = OUC.EmployeeeEventType
            //                       }).FirstOrDefault();
            //        //foreach (PresentationOUChangeSnapshots accitem in account)
            //        //{
            //        //    data.Add(item);
            //        //}
            //        if (account != null)
            //        {
            //            data.Add(account);
            //        }
            //    }


            //    if (item.EndpointSystemTypeID == Convert.ToInt64(EndpointType.InhouseSystems))
            //    {
            //        var account = (from OUC in CurrentContext.OUChangeSnapshots
            //                       join user in CurrentContext.User
            //                           on OUC.UserId equals user.Id
            //                       join Endpnt in CurrentContext.EndPoint
            //                           on OUC.EndpointSystemId equals Endpnt.Id
            //                       join endpntacc in CurrentContext.InhouseUserInfos
            //                            on OUC.EndpointUserId equals endpntacc.Id
            //                       where OUC.Id == item.SnapshotId
            //                       select new PresentationOUChangeSnapshots
            //                       {
            //                           UserName = user.DisplayName,
            //                           EmployeeNo = user.EmployeeId,
            //                           EmailID = user.EmailId,
            //                           EndpointsystemName = Endpnt.DisplayName,
            //                           EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
            //                           AccountName = endpntacc.UserName,
            //                           EndpointUserID = OUC.EndpointUserId,
            //                           Entitlement = OUC.Entitlement,
            //                           EntitlementType = OUC.EntitlementType,
            //                           EmployeeEventType = OUC.EmployeeeEventType,
            //                           CreatedDate = OUC.EnteredDate
            //                       });
            //        foreach (PresentationOUChangeSnapshots accitem in account)
            //        {
            //            if (accitem != null)
            //            {
            //                data.Add(accitem);
            //            }
            //        }
            //    }
            //    if (item.EndpointSystemTypeID == Convert.ToInt64(EndpointType.SAP))
            //    {
            //        var account = (from OUC in CurrentContext.OUChangeSnapshots
            //                       join user in CurrentContext.User
            //                           on OUC.UserId equals user.Id
            //                       join Endpnt in CurrentContext.EndPoint
            //                           on OUC.EndpointSystemId equals Endpnt.Id
            //                       join endpntacc in CurrentContext.SAPUserinfos
            //                            on OUC.EndpointUserId equals endpntacc.Id
            //                       where OUC.Id == item.SnapshotId
            //                       select new PresentationOUChangeSnapshots
            //                       {
            //                           UserName = user.DisplayName,
            //                           EmployeeNo = user.EmployeeId,
            //                           EmailID = user.EmailId,
            //                           EndpointsystemName = Endpnt.DisplayName,
            //                           EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
            //                           AccountName = endpntacc.UserLogonName,
            //                           EndpointUserID = OUC.EndpointUserId,
            //                           Entitlement = OUC.Entitlement,
            //                           EntitlementType = OUC.EntitlementType,
            //                           CreatedDate = OUC.EnteredDate
            //                       });
            //        foreach (PresentationOUChangeSnapshots accitem in account)
            //        {
            //            if (accitem != null)
            //            {
            //                data.Add(accitem);
            //            }
            //        }
            //    }
            //    if (item.EndpointSystemTypeID == Convert.ToInt64(EndpointType.SAPEnterprisePortal))
            //    {
            //        var account = (from OUC in CurrentContext.OUChangeSnapshots
            //                       join user in CurrentContext.User
            //                           on OUC.UserId equals user.Id
            //                       join Endpnt in CurrentContext.EndPoint
            //                           on OUC.EndpointSystemId equals Endpnt.Id
            //                       join endpntacc in CurrentContext.SAPEPUserInfos
            //                            on OUC.EndpointUserId equals endpntacc.Id
            //                       where OUC.Id == item.SnapshotId
            //                       select new PresentationOUChangeSnapshots
            //                       {
            //                           UserName = user.DisplayName,
            //                           EmployeeNo = user.EmployeeId,
            //                           EmailID = user.EmailId,
            //                           EndpointsystemName = Endpnt.DisplayName,
            //                           EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
            //                           AccountName = endpntacc.UserName,
            //                           EndpointUserID = OUC.EndpointUserId,
            //                           Entitlement = OUC.Entitlement,
            //                           EntitlementType = OUC.EntitlementType,
            //                           CreatedDate = OUC.EnteredDate
            //                       });
            //        foreach (PresentationOUChangeSnapshots accitem in account)
            //        {
            //            if (accitem != null)
            //            {
            //                data.Add(accitem);
            //            }
            //        }
            //    }

            //}

            
        }
    }
}

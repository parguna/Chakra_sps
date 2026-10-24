using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPEPGroupInfo : DomainModelBase<SAPEPGroupInfo, EntityModel.SAPEPGroupInfo>, IComparer<SAPEPGroupInfo>, IXLData
    {

        public Nullable<long> EndPointSystemId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupUniqueName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }


        public IList<SAPEPUserInfo> SAPEPUserInfos { get; set; }


        public SAPEPGroupInfo()
        {
            SAPEPUserInfos = new List<SAPEPUserInfo>();
        }

        public override string ToString()
        {
            return this.GroupId.ToString();
        }
        public static SAPEPGroupInfo CreateSAPEPGroupInfo()
        {
            SAPEPGroupInfo sapEPGroupInfo = new SAPEPGroupInfo();
            return sapEPGroupInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPGroupInfo other = obj as SAPEPGroupInfo;
            if (obj != null)
            {
                return this.GroupId.Equals(other.GroupId);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.GroupId.GetHashCode() ^ this.GroupName.GetHashCode();
        }


        public int Compare(SAPEPGroupInfo x, SAPEPGroupInfo y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {

                cmprValue = string.Compare(y.GroupName, x.GroupName);
            }

            return cmprValue;
        }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }


        public static List<SAPEPGroupInfo> GetByEndPointId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static Dictionary<string, string> GetPortalGroups(long? endpointSystemId, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPGroupInfoRepository(dataAccessTransaction).GetPortalGroups(endpointSystemId, CheckNull.AsteriskToString(searchString));

            }
        }
        public static Dictionary<string, string> GetPortalGroupsByActionType(long endpointSystemId, string searchString, List<long> endpointUserId, AdHocActionType actionType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPGroupInfoRepository(dataAccessTransaction).GetPortalGroupsByActionType(endpointSystemId, CheckNull.AsteriskToString(searchString), endpointUserId, actionType);

            }
        }
        public static Dictionary<string, string> GetGroupSAPEP(long EndpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPGroupInfoRepository(dataAccessTransaction).GetGroupSAPEP(EndpointSystemId);
            }
        }

        public static SAPEPGroupInfo GetByName(long endpointSystemId, string groupName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.GroupName == groupName).FirstOrDefault());
            }
        }
        public static SAPEPGroupInfo GetGroupByGroupUniqueId(long endpointSystemId, string groupUniqueId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.GroupId == groupUniqueId).FirstOrDefault());
            }
        }

        public static SAPEPGroupInfo GetByUniqueName(long endpointSystemId, string uniquegroupName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.GroupUniqueName == uniquegroupName).FirstOrDefault());
            }
        }

        public static List<SAPEPGroupInfo> GetAll(long endpointSystemId, bool isXLUploaded)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.IsXLUploaded == isXLUploaded).ToList());
            }
        }

        public static List<SAPEPGroupInfo> GetDeletedGroups(long endpointId, Process process)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {

                if (process != null)
                    return ORMapList(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).
                        Get(sap => sap.EndPointSystemId == endpointId && sap.IsXLUploaded == false && ((sap.LastModifiedDate == null && sap.CreatedDate < process.ProcessStartAt) || sap.LastModifiedDate < process.ProcessStartAt)).ToList());
                else
                    return null;
            }
        }

        //private static Process GetProcess(long endpointId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endpointId);

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            return process;
        //        }
        //        else return null;
        //    }
        //}

        public static Dictionary<long, string> GetAllNamesByEndpoint(long endpointID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPGroupInfoRepository(dataAccessTransaction).GetAllNamesByEndpoint(endpointID);
            }
        }

        public static List<SAPEPGroupInfo> GetAllSAPEPGroups(long endpointSystemId, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPEPGroupInfo> tmpList1 = ORMapList(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endpointSystemId && grp.GroupName.Contains(searchString)).OrderBy(grp => grp.GroupName).Distinct().ToList());
                return tmpList1;
            }
        }
        public static List<SAPEPGroupInfo> GetAllSAPEPGroups(long endpointSystemId, bool isXLuploaded)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPGroupInfo>(transaction)
                    .Get(grp => grp.EndPointSystemId == endpointSystemId && grp.IsXLUploaded == isXLuploaded).ToList());
            }
        }
        public static bool DoesExist(string entitlement)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Any(grp => grp.GroupUniqueName == entitlement);
            }
        }
        public static List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            Group group;
            List<Group> grpList = new List<Group>();
            List<SAPEPGroupInfo> sapepList = SAPEPGroupInfo.GetAllSAPEPGroups(endPointID, CheckNull.AsteriskToString(searchString));
            var sapepGrpList = (from tmp in sapepList select new { GroupId = tmp.Id, GroupName = tmp.GroupName, Description = tmp.GroupDescription }).Distinct();

            if (sapepGrpList.Count() > 0)
            {
                foreach (var tLst in sapepGrpList)
                {
                    group = new Group();
                    group.Id = CheckNull.AgainstInt(tLst.GroupId);
                    group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                    group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                    grpList.Add(group);
                }
            }
            return grpList;
        }
        public static bool IsSAPEPGroupValid(long endpointSystemId, string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Any(grp => grp.GroupName == value && grp.EndPointSystemId == endpointSystemId);
            }
        }
        public static bool TruncateTable(long? endpointSystemId)
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).ExecuteQuery("update DI_SAPEP_GroupInfo set IsXLUploaded = 0 where  IsXLUploaded=1 and EndPointSystemId=" + endpointSystemId);
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPEPGroupInfo GetSAPEPGroupObject(long? endpointSystemId, string groupUniqueName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.GroupUniqueName == groupUniqueName).FirstOrDefault());
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPEPGroupInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString, long endpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPGroupInfoRepository(dataAccessTransaction).GetGroupsById(Id, actionType, searchString, endpointsystemId);
            }
        }




    }
}

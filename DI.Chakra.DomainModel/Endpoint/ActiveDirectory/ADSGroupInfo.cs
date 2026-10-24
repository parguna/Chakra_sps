using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class ADSGroupInfo : DomainModelBase<ADSGroupInfo, EntityModel.ADSGroupInfo>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string member { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public Nullable<System.DateTime> whenCreated { get; set; }

        public Nullable<System.DateTime> whenChanged { get; set; }

        public Nullable<long> uSNCreated { get; set; }

        public Nullable<long> uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string objectSid { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string groupType { get; set; }

        public string groupTypeDesc { get; set; }

        public string objectCategory { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public bool? CanRequest { get; set; }

        public List<ADSUserInfo> ADSUsers { get; set; }
        public string UserDefinedDesc { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public Nullable<long> Level1Approver { get; set; }

        public Nullable<long> Level2Approver { get; set; }

        public Nullable<bool> IsLevel1Group { get; set; }

        public Nullable<bool> IsLevel2Group { get; set; }

        public object[] memberOf;

        public string FormattedOU
        {
            get
            {
                return this.distinguishedName.Remove(0, this.distinguishedName.IndexOf(",") + 1);
            }
        }
        public override string ToString()
        {
            return this.name.ToString();
        }

        public static ADSGroupInfo CreateADSGroupInfo()
        {
            ADSGroupInfo adsUserInfo = new ADSGroupInfo();

            return adsUserInfo;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ADSGroupInfo)
            {
                ADSGroupInfo other = (ADSGroupInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.name.GetHashCode();
        }

        public string EntitlementName
        {
            get { return cn; }
        }

        public string EntitlementDesc
        {
            get { return description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }


        public static Dictionary<long, string> GetByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new ADSGroupInfoRepository(dataAccessTransaction).GetByEndpointId(endpointId);
            }
        }

        public static Dictionary<long, string> GetAllByName(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new ADSGroupInfoRepository(dataAccessTransaction).GetAllByName(CheckNull.AsteriskToString(searchString));
            }
        }
        public static List<string> GetNameByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction).GetAllColumnsFromQuery("Select Name from DI_ADS_GroupInfo where EndPointSystemId = @EndPointSystemId", new System.Data.SqlClient.SqlParameter("@EndPointSystemId", endpointSystemId));
            }
        }
        public static Dictionary<long, string> GetADSGroupBySearchString(string searchString, long EndpointId,long accessRequestId=0,bool isBulkRequest=false)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                AdHocActionType adHocActionType = AdHocActionType.None;
                var existingGroups = new List<string>();
                if (isBulkRequest == false)
                {
                    if (accessRequestId > 0)
                    {
                        var result = new AccessRequestRepository(dataAccessTransaction).GetById(accessRequestId, true);
                        adHocActionType = result.AdHocAccessRequest.AdhocActionType;
                        if (result.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            var tempObject = AutoMapper.Mapper.Map<DomainModel.AdHocAccessRequest>(result.AdHocAccessRequest);
                            var adEnpUserId = result.ApplicationType == ApplicationType.ADSingaporePowerLocal ? (tempObject.GetEntitlementDetail() as AdHocADRequest).EndpointUserId : (tempObject.GetEntitlementDetail() as AdHocSAPLocalRequest).EndpointUserId;
                            existingGroups = new ADSGroupInfoRepository(dataAccessTransaction).GetExistingGroupByEndpointUserId(result.EndpointSystemId.Value, new List<long>() { adEnpUserId });
                        }
                    }

                }
                else
                {
                    if (accessRequestId > 0)
                    {
                        var result = new BulkRequestRepository(dataAccessTransaction).GetById(accessRequestId, true);
                        adHocActionType = result.AdhocActionType;
                        if (result != null && (adHocActionType == AdHocActionType.AmendmentAdd || adHocActionType == AdHocActionType.AmendmentRemove))
                        {
                            var endpointUserIds = result.BulkRequestDetails.Select(i => i.EndpointUserId.Value).ToList();
                            if (result.AdhocActionType == AdHocActionType.AmendmentAdd)
                            {
                                existingGroups = new ADSGroupInfoRepository(dataAccessTransaction).GetExistingGroupByEndpointUserId(result.EndpointSystemId, endpointUserIds);
                            }
                            else if(result.AdhocActionType == AdHocActionType.AmendmentRemove)
                            {
                                existingGroups = new ADSGroupInfoRepository(dataAccessTransaction).GetExistingGroupByEndpointUserId(result.EndpointSystemId, endpointUserIds, AdHocActionType.AmendmentRemove);
                            }
                        }
                    }
                }
                if (isBulkRequest && adHocActionType == AdHocActionType.AmendmentRemove && existingGroups.Count ==0)
                {
                    return new Dictionary<long, string>();
                }
                var adGroups = new ADSGroupInfoRepository(dataAccessTransaction).GetByEndpointIdAndName(CheckNull.AsteriskToString(searchString), EndpointId);
                if(existingGroups != null && existingGroups.Count > 0 && adGroups.Count > 0)
                {
                    if (isBulkRequest && adHocActionType == AdHocActionType.AmendmentRemove)
                    {
                        adGroups = adGroups.Where(i => existingGroups.Contains(i.Value, StringComparer.InvariantCultureIgnoreCase)).ToDictionary(j => j.Key, k => k.Value);
                    }
                    else
                    {
                        if (isBulkRequest == false)
                            adGroups = adGroups.Where(i => !existingGroups.Contains(i.Value, StringComparer.InvariantCultureIgnoreCase)).ToDictionary(j => j.Key, k => k.Value);
                    }
                    
                }
                return adGroups;
            }
        }

        public static List<ADSGroupInfo> GetByIds(List<long> Ids, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(t => Ids.Contains(t.Id) && t.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static ADSGroupInfo GetByDistinguishedNameAndEndpointId(string distName, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(group => group.distinguishedName == distName && group.EndPointSystemId == endpointSystemId, group => group.ADSUsers).FirstOrDefault());
            }
        }

        public static List<ADSGroupInfo> GetByIdsIncludingUsers(List<long> Ids, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(t => Ids.Contains(t.Id) && t.EndPointSystemId == endpointSystemId,
                       t => t.ADSUsers).ToList());
            }
        }
        public static ADSGroupInfo GetByAccountName(string sAMAccountName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(t => t.sAMAccountName == sAMAccountName).FirstOrDefault());
            }
        }

        public static ADSGroupInfo GetByAccountName(string sAMAccountName, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(t => t.sAMAccountName == sAMAccountName && t.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static ADSGroupInfo GetByObjectId(string objectId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(grp => grp.objectSid == objectId).FirstOrDefault());
            }
        }

        public static ADSGroupInfo GetByCn(string cn)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(grp => grp.cn == cn).FirstOrDefault());
            }
        }

        public static ADSGroupInfo GetByName(string groupName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(g => g.name == groupName).FirstOrDefault());
            }
        }

        public static ADSGroupInfo GetByCnAndEndpointId(string cn, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(group => group.cn == cn && group.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public static ADSGroupInfo GetByCnAndEndpointIdForCompilance(string cn, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(group => group.cn == cn && group.EndPointSystemId == endpointSystemId, group => group.ADSUsers).FirstOrDefault());
            }
        }

        public static List<ADSGroupInfo> GetAllByCnAndEndpointId(string cn, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(group => group.cn == cn && group.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static ADSGroupInfo GetByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static List<ADSGroupInfo> GetAllByGroupType(string paramgroupType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(grp => grp.groupTypeDesc == paramgroupType).ToList());
            }
        }

        public static List<ADSGroupInfo> GetAllByGroupTypeAndEndpointId(string paramgroupType, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                      .Get(grp => grp.groupTypeDesc == paramgroupType && grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<ADSGroupInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<ADSGroupInfo> GetADGroupsByUserId(long userId)
        {
            ADSUserInfo adUserInfo = null;

           // using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                adUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(userId);

                if (adUserInfo != null)
                {
                    adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                }
                return adGroups;
            }
        }

        public static List<ADSGroupInfo> GetADGroupsByUserIdAndEndpointId(long userId, long EndpointId)
        {
            ADSUserInfo adUserInfo = null;

            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                adUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(userId);

                if (adUserInfo != null && adUserInfo.EndPointSystemId == EndpointId)
                {
                    adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                }
                return adGroups;
            }
        }
        public static Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? endpointUserId, bool existingEntitlements)
        {
            IList<ADSGroupInfo> list = null;
            Dictionary<long, string> result = null;

            if (existingEntitlements)
            {
                list = ADSGroupInfo.GetADGroupsByUserId(endpointUserId.Value);
                result = list.Where(item => item.sAMAccountName.Contains(searchString))
                .Select(x => new { Key = x.Id, Value = x.name })
                .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            else
            {
                result = ADSGroupInfo.GetAllByName(searchString);
            }
            return result;
        }
        public static List<ADSGroupInfo> GetADGroupsByDIUserId(long diUserId)
        {
            ADSUserInfo adUserInfo = null;

            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                adUserInfo = ADSUserInfo.GetByDIUserIdIncludeADSGroups(diUserId);
                if (adUserInfo != null)
                {
                    adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                }
                return adGroups;
            }
        }

        public static List<ADSGroupInfo> GetADGroupsByDIUserId(long userId, AccountType accounttype)
        {
            ADSUserInfo adUserInfo = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                adUserInfo = ADSUserInfo.GetByDIUserIdIncludeADSGroups(userId, accounttype);
                if (adUserInfo != null)
                {
                    adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                }
                return adGroups;
            }
        }

        public static bool IsValid(string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var entitlements = value.Split(',');
                var isExists = false;

                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = DoesExist(entitlement);
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
        }

        public static bool DoesExist(string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction).Any(item => item.name == value);
            }
        }
        public static bool IsADGroupValid(long endpointSystemId, string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction).Any(item => item.name == value && item.EndPointSystemId == endpointSystemId);
            }
        }

        public static IList<ADSGroupInfo> GetDeletedByEndPointSystemId(long endPointSystemId, long discProcessId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<ADSGroupInfo> listDeletedGroups = new List<ADSGroupInfo>();
                Process process = Process.GetByProcessId(discProcessId);
                var discoveryProcess = DiscoveryProcess.GetById(discProcessId);
                if (discoveryProcess != null && process != null)
                {
                    listDeletedGroups =
                        ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId &&
                         (grp.LastModifiedDate < process.ProcessStartAt ||
                                           (grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)
                        )).ToList());
                }

                return listDeletedGroups;
            }
        }

        public static void SaveGroups(IList<ADSGroupInfo> list, string currentUser, long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<ADSGroupInfo> listExistingADgroups = new List<ADSGroupInfo>();
                ADSGroupInfo adexistingGroups = null;
                listExistingADgroups = ADSGroupInfo.GetAllByEndpointSystemId(endpointId);
                foreach (ADSGroupInfo grp in list)
                {
                    adexistingGroups = listExistingADgroups.Where(t => t.sAMAccountName == grp.sAMAccountName).FirstOrDefault();
                    if (adexistingGroups == null)
                    {
                        grp.CanRequest = true;
                        grp.Save(dataAccessTransaction);
                    }
                    else
                    {
                        grp.Id = adexistingGroups.Id;
                        grp.CanRequest = adexistingGroups.CanRequest;
                        grp.UserDefinedDesc = adexistingGroups.UserDefinedDesc;
                        grp.CreatedBy = adexistingGroups.CreatedBy;
                        grp.CreatedDate = adexistingGroups.CreatedDate;

                        grp.Save(dataAccessTransaction);
                    }
                }

                dataAccessTransaction.Commit();
            }
        }

        public static List<ADSGroupInfo> GetAllADSGroups(long endPointSystemId, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.cn.Contains(searchString)).OrderBy(grp => grp.cn).ToList());

            }
        }


        public static List<ADSGroupInfo> GetByEndpointIdIncludeGroupWithNoAccounts(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(transaction)
                    .Get(group => group.EndPointSystemId == endPointSysemId
                    && group.ADSUsers.Count == 0).ToList());
            }

        }

        public static List<ADSGroupInfo> GetByEndpointSystem(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSGroupInfo>(transaction)
                    .Get(group => group.EndPointSystemId == endPointSysemId).ToList());
            }

        }

        public static List<ADSGroupInfo> GetByIdFromADSNestedGroups(long endpointSystemId, long groupId)
        {
            List<ADSGroupInfo> level1ADSGroups = new List<ADSGroupInfo>();
            List<ADSGroupInfo> resADSGroups = new List<ADSGroupInfo>();
            level1ADSGroups = ADSNestedGroup.GetMembersIncludingUsers(endpointSystemId, groupId);
            resADSGroups.AddRange(level1ADSGroups);
            foreach (var level1Group in level1ADSGroups)
            {
                var level2Group = ADSNestedGroup.GetMembersIncludingUsers(endpointSystemId, level1Group.Id);
                resADSGroups.AddRange(level2Group);
            }

            return resADSGroups;
        }

        public static List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            Group group;
            List<Group> grpList = new List<Group>();
            List<ADSGroupInfo> GrpList = ADSGroupInfo.GetAllADSGroups(endPointID, CheckNull.AsteriskToString(searchString));
            var adsGrpList = (from tmp in GrpList select new { GroupId = tmp.Id, GroupName = tmp.cn, Description = tmp.description }).Distinct();

            if (adsGrpList.Count() > 0)
            {
                foreach (var adsGrp in adsGrpList)
                {
                    group = new Group();
                    group.Id = CheckNull.AgainstInt(adsGrp.GroupId);
                    group.GroupName = CheckNull.AgainstString(adsGrp.GroupName);
                    group.GroupDescription = CheckNull.AgainstString(adsGrp.Description);
                    grpList.Add(group);
                }
            }
            return grpList;
        }
        public static ADSGroupInfo GetByNameWithEndpointId(long endPointID, string groupName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSGroupInfo>(dataAccessTransaction)
                       .Get(g => g.EndPointSystemId == endPointID && g.name == groupName).FirstOrDefault());
            }
        }
        public static string CheckADGroupExistsInOtherSystemMatrix(string selectedADGroup)
        {
            string message = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectedADGroup))
            {
                var matrix = InhousePermission.GetActiveRolesWithADGroups();
                if (matrix != null && matrix.Count > 0)
                {
                    List<string> allgroups = new List<string>();
                    matrix = matrix.Where(i => !string.IsNullOrWhiteSpace(i.ADGroups)).ToList();
                    if (matrix != null && matrix.Count > 0)
                    {
                        foreach (var item in matrix)
                        {
                            var data = item.ADGroups.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (data != null && data.Count > 0)
                            {
                                if (data.Any(j => !string.IsNullOrWhiteSpace(j) && j.Trim().ToUpper() == selectedADGroup.ToUpper()))
                                {
                                    return $"Application : {item.Description}";
                                }
                            }
                        }
                    }
                }
                var vpnDatas = VPNMatrix.GetVPNADGroups(selectedADGroup);
                if (vpnDatas != null && vpnDatas.Count > 0)
                {
                    foreach (var vpnData in vpnDatas)
                    {
                        var data = vpnData.ADGroups.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (data != null && data.Count > 0)
                        {
                            if (data.Any(j => !string.IsNullOrWhiteSpace(j) && j.Trim().ToUpper() == selectedADGroup.ToUpper()))
                            {
                                return $"Application : {vpnData.Name}";
                            }
                        }
                    }
                }
            }
            return message;
        }
    }
}

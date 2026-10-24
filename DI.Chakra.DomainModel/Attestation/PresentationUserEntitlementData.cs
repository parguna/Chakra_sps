using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class PresentationUserEntitlementData
    {
        public long AccountReconcilationId { get; set; }
        public string EmployeeName { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string SystemName { get; set; }
        public long EndpointId { get; set; }
        public long? UserId { get; set; }
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public bool IsActionable { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public static PresentationGridPagedData<ViewUserEntitlementData> GetAttestationUserComparisionData(long taskId, int draw, int start, int length, List<long> endpointIds, AccountType accountType, string search)
        {
            PresentationGridPagedData<ViewUserEntitlementData> data4 = new PresentationGridPagedData<ViewUserEntitlementData>();
            using (var tran = new DataAccessTransaction(false))
            {
                var result = new AttestationSnapshotActionRepository(tran).GetAttestationUserComparisionData(taskId, draw, start, length, endpointIds, accountType, search);
                var entitlementDataList = result.Data;
                data4.TotalCount = result.TotalCount;
                data4.Data = ORMapList(result.Data).ToList();
                return data4;
            }
        }

        public static ViewUserEntitlementData ORMap(EntityModel.ViewUserEntitlementData tEntity)
        {
            if (tEntity == null)
                return default(ViewUserEntitlementData);

            return Mapper.Map<EntityModel.ViewUserEntitlementData, ViewUserEntitlementData>(tEntity);
        }

        public static List<ViewUserEntitlementData> ORMapList(IList<EntityModel.ViewUserEntitlementData> tEntities)
        {
            return tEntities.Select(ORMap).ToList();
        }
        public static List<FilterData> GetFilterDataByType(long taskId, int filterType = 1, List<long> epIds = null, List<long> userIds = null, AccountType accountType = AccountType.User)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ConvertToDto(new AttestationSnapshotActionRepository(tran).GetFilterDataByType(taskId, filterType, epIds, userIds, accountType));
            }
        }
        public static Dictionary<string, List<FilterData>> GetEmployeeInformation(long taskId, AccountType accountType, List<long> epIds)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var Result = new Dictionary<string, List<FilterData>>();
                var data = new AttestationSnapshotActionRepository(tran).GetEmployeeInformation(taskId, accountType, epIds);
                foreach (var item in data)
                {
                    Result.Add(item.Key, ConvertToDto(item.Value));
                }
                return Result;
            }
        }
        public static List<FilterData> ConvertToDto(IList<EntityModel.FilterData> tModelList)
        {
            List<FilterData> filterDatas = new List<FilterData>();
            foreach (var item in tModelList)
            {
                filterDatas.Add(new FilterData { Id = item.Id, Name = item.Name, AdditionalInfo = item.AdditionalInfo, EndpointId = item.EndpointId, EndpointName = item.EndpointName, UserName = item.UserName, AccountStatus = item.AccountStatus, IsActionable = item.IsActionable, EntitlementType = item.EntitlementType });
            }
            return filterDatas;
        }

        public static bool UpdateAttestationTaskActionByUser(long taskId, long endpointId, long userId, string entitlement, string userName, AccountType accountType, string accessType, long actionUserId, int level, EntitlementType entitlementType)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotActionRepository(tran).UpdateAttestationTaskActionByUser(taskId, endpointId, userId, entitlement, userName, accountType, accessType, actionUserId, level, entitlementType);
            }
        }
        public static List<AccountType> GetAccountTypes(long taskId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationSnapshotActionRepository(tran).GetAccountTypes(taskId);
            }
        }
        public static List<FilterData> GetAccountStatusInformation(long taskId, AccountType accountType, long userId, string accountName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var Result = new List<FilterData>();
                var data = new AttestationSnapshotActionRepository(tran).GetAccountStatusInformation(taskId, accountType, userId, accountName);
                return ConvertToDto(data);
            }
        }

        public static List<AttestationTaskEntitlementUserComparision> GetAttestationTaskEntitlementUserComparision(long taskId, int level = 1)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                var result = new AttestationSnapshotActionRepository(tran).GetAttestationTaskEntitlementUserComparision(taskId);

                var mappeddata = ORMapList(result).ToList();

                return mappeddata;
            }

        }
        public static List<AttestationCampaignEntitlementUserComparision> GetAttestationTaskEntitlementUserComparisionByCampaignId(int pageNumber, int pageSize, long campaignId)
        {
            List<AttestationCampaignEntitlementUserComparision> data = new List<AttestationCampaignEntitlementUserComparision>();
            using (var tran = new DataAccessTransaction(false))
            {
                var result = new AttestationSnapshotActionRepository(tran).GetAttestationTaskEntitlementUserComparisionByCampaignId(pageNumber, pageSize, campaignId);
                foreach (var snap in result)
                {
                    data.Add(new AttestationCampaignEntitlementUserComparision() {
                        SnapshotActionId = snap.SnapshotActionId,
                        EndpointSystemName = snap.EndpointSystemName,
                        EndpointType = snap.EndpointType,
                        AccountType = snap.AccountType,
                        Entitlement = snap.Entitlement,
                        EntitlementDescription = snap.EntitlementDescription,
                        UserName = snap.UserName,
                        Name = snap.Name,
                        Approve = snap.Approve,
                        Revoke = snap.Revoke
                    });
                }
                return data;
            }
        }

        public static AttestationTaskEntitlementUserComparision ORMap(EntityModel.AttestationTaskEntitlementUserComparision tEntity)
        {
            if (tEntity == null)
                return default(AttestationTaskEntitlementUserComparision);

            return Mapper.Map<EntityModel.AttestationTaskEntitlementUserComparision, AttestationTaskEntitlementUserComparision>(tEntity);
        }

        public static List<AttestationTaskEntitlementUserComparision> ORMapList(IList<EntityModel.AttestationTaskEntitlementUserComparision> tEntities)
        {
            return tEntities.Select(ORMap).ToList();
        }
    }
}

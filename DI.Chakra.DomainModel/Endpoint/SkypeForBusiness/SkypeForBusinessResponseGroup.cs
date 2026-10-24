using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Dto;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessResponseGroupInfo : DomainModelBase<SkypeForBusinessResponseGroupInfo, EntityModel.SkypeForBusinessResponseGroupInfo>, IEndpointEntitlementInfo
    {
        public SkypeForBusinessResponseGroupInfo()
        {
            this.SkypeForBusinessUserInfo = new List<SkypeForBusinessUserInfo>();
        }

       

        public string OwnerPool { get; set; }
        public string AgentsByUri { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParticipationPolicy { get; set; }
        public string AgentAlertTime { get; set; }
        public string RoutingMethod { get; set; }
        public string DistributionGroupAddress { get; set; }
        public string Identity { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string DistributionGroupName { get; set; }

        public List<SkypeForBusinessUserInfo> SkypeForBusinessUserInfo { get; set; }

        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get
            {
                return Name;
            }
        }

        public string EntitlementDesc
        {
            get
            {
                return Name;
            }
        }

        public EntitlementType EntitlementType
        {
            get
            {
                return EntitlementType.ResponseGroup;
            }
        }
        //TODO: rewrite this
        public List<string> MemberList
        {
            get
            {
                var delimiter = new string[] { ","};
                var members = new List<string>();
                if (!string.IsNullOrWhiteSpace(this.AgentsByUri))
                {
                    this.AgentsByUri.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => members.Add($"{i.Trim()}"));
                }
                return members;
            }
        }
        public long EndPointSystemId { get; set; }
        public static List<SkypeForBusinessResponseGroupInfo> GetAllByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId).ToList());
            }
        }
        public static List<SkypeForBusinessResponseGroupInfo> GetAllByEndpointIdIncludeMember(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId,includeProperties: u=>u.SkypeForBusinessUserInfo).ToList());
            }
        }
        public static SkypeForBusinessResponseGroupInfo GetByIdentity(long endpointId,string identity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId && item.Identity == identity).FirstOrDefault());
            }
        }
        public static SkypeForBusinessResponseGroupInfo GetGroupByName(string name)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction).Get(item => item.Name == name).FirstOrDefault());
            }
        }
        public static void SaveGroups(IList<SkypeForBusinessResponseGroupInfo> list, long endpointId, string currentUser)
        {
            try
            {
                var allExistingGroups = SkypeForBusinessResponseGroupInfo.GetAllByEndpointId(endpointId);
                SkypeForBusinessResponseGroupInfo existingGroup = null;
                foreach (SkypeForBusinessResponseGroupInfo group in list)
                {
                    existingGroup = allExistingGroups.Where(grp => grp.Name == group.Name).FirstOrDefault();
                    if (existingGroup == null)
                    {
                        group.CreatedBy = currentUser;
                        group.CreatedDate = DateTime.Now;
                        group.Save(currentUser);
                        allExistingGroups.Add(group);
                    }
                    else
                    {
                        group.Id = existingGroup.Id;
                        group.CreatedDate = existingGroup.CreatedDate;
                        group.CreatedBy = existingGroup.CreatedBy;
                        group.Save(currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static SkypeForBusinessResponseGroupInfo SaveGroup(SkypeForBusinessResponseGroupInfo group, long endpointId, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var exisitingRecord = ORMap(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId && item.Identity == group.Identity).FirstOrDefault());
                if(exisitingRecord == null)
                {
                    exisitingRecord = group;
                    exisitingRecord.Save(currentUser);
                }
                else
                {
                    exisitingRecord.Name = group.Name;
                    exisitingRecord.OwnerPool = group.OwnerPool;
                    exisitingRecord.AgentsByUri = group.AgentsByUri;
                    exisitingRecord.Description = group.Description;
                    exisitingRecord.ParticipationPolicy = group.ParticipationPolicy;
                    exisitingRecord.RoutingMethod = group.RoutingMethod;
                    exisitingRecord.DistributionGroupAddress = group.DistributionGroupAddress;
                    exisitingRecord.Save(currentUser);
                }
                return exisitingRecord;
            }
        }
        public static IList<SkypeForBusinessResponseGroupInfo> GetDeletedGroups(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SkypeForBusinessResponseGroupInfo> listDeletedUsers = new List<SkypeForBusinessResponseGroupInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessResponseGroupInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }
        public static void UpdateResponseGroupMasked(long endPointSystemId, string userSIPAddress, string responseGroupName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).UpdateResponseGroupMasked(endPointSystemId, userSIPAddress, responseGroupName);
                dataAccessTransaction.Commit();
            }
        }
        public static void UpdateResponseGroupMaskedById(long endPointSystemId, long userid, long groupId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).UpdateResponseGroupMaskedById(endPointSystemId, userid, groupId);
                dataAccessTransaction.Commit();
            }
        }
        public static void RemoveSkypeForBusinessResponseGroups(List<SkypeForBusinessResponseGroupInfo> list, long endpointId)
        {
            foreach (SkypeForBusinessResponseGroupInfo group in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).DeleteGroupUser(group.Id, endpointId);
                    dataAccessTransaction.Commit();
                }
                var provisionalMatrix = SkypeForBusinessProvisioningMatrix.GetByName(group.Name, EntitlementType.ResponseGroup);
                if(provisionalMatrix != null)
                {
                    provisionalMatrix.Delete();
                }
                group.Delete();
            }
        }
        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }
        public static Dictionary<long, string> GetResponseGroup(long endPointUserId, long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            var returnValue = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetResponseGroup(endPointUserId, endpointId, searchString, adHocActionType);
            }
            return returnValue;
        }
        public static Dictionary<long, string> GetResponseGroupForBulkRequest(long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            var returnValue = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetResponseGroupForBulkRequest(endpointId, searchString, adHocActionType);
            }
            return returnValue;
        }
        public static Dictionary<string,string> GetResponseGroupWithADID(long endpointsystemId)
        {
            var returnValue = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetResponseGroupWithADID(endpointsystemId);
            }
            return returnValue;
        }
        public static List<PresentationResponseGroupMember> GetMaskedUser(long endpointsystemId,long userId, Nullable<bool> takeMaskedUser, string groupName=null)
        {
            var returnValue = new List<PresentationResponseGroupMember>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var results = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetMaskedUser(endpointsystemId, userId, takeMaskedUser, groupName );
                returnValue = ConvertToDomainModel(results);
            }
            return returnValue;
        }
        public static List<PresentationResponseGroupMember> GetResponseGroupWithMaskStatus(long endpointsystemId, long userId)
        {
            var returnValue = new List<PresentationResponseGroupMember>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var results = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetResponseGroupWithMaskStatus(endpointsystemId, userId);
                returnValue = ConvertToDomainModel(results);
            }
            return returnValue;
        }
        
        private static List<PresentationResponseGroupMember> ConvertToDomainModel(List<EntityModel.PresentationResponseGroupMember> entityResults)
        {
            var results = new List<PresentationResponseGroupMember>();
            foreach (var entityResult in entityResults)
            {
                results.Add(new PresentationResponseGroupMember()
                {
                    EndpointSystemId = entityResult.EndpointSystemId,
                    ADID = entityResult.ADID,
                    IsMasked = entityResult.IsMasked,
                    MemberPrincipalName = entityResult.MemberPrincipalName,
                    ResponseGroupName = entityResult.ResponseGroupName,
                    UserId = entityResult.UserId,
                });
            }
            return results;
        }
        public static List<PresentationResponseGroupUserMember> GetResponseGroupUser(string responsegroupname)
        {
            var results = new List<PresentationResponseGroupUserMember>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var items = new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetResponseGroupUser(responsegroupname).ToList();
                foreach (var entityResult in items)
                {
                    results.Add(new PresentationResponseGroupUserMember()
                    {
                        UserPrincipalName = entityResult.UserPrincipalName,
                        LastName = entityResult.LastName,
                        LineURI = entityResult.LineURI,
                    });
                }
            }
            return results;
        }

        public static List<string> GetDistributionGroup(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessResponseGroupRepository(dataAccessTransaction).GetDistributionGroup(endpointId).ToList();
            }
        }
    }
}

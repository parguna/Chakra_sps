using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Office365GroupInfo : DomainModelBase<Office365GroupInfo, EntityModel.Office365GroupInfo>, IEndpointEntitlementInfo
    {
        public Office365GroupInfo()
        {
            this.O365Users = new List<Office365UserInfo>();
        }

        public long EndPointSystemId { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public string member { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public Nullable<System.DateTime> whenCreated { get; set; }
        public Nullable<System.DateTime> whenChanged { get; set; }
        public string name { get; set; }
        public string objectSid { get; set; }
        public string sAMAccountName { get; set; }
        public string sAMAccountType { get; set; }
        public string groupType { get; set; }
        public string groupTypeDesc { get; set; }
        public string objectCategory { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<Office365UserInfo> O365Users { get; set; }

        public string EntitlementName
        {
            get
            {
                return displayName;
            }
        }

        public string EntitlementDesc
        {
            get
            {
                return description;
            }
        }

        public EntitlementType EntitlementType
        {
            get
            {
                return EntitlementType.Group;
            }
        }

        public static List<Office365GroupInfo> GetAllByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365GroupInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId).ToList());
            }
        }

        public static Office365GroupInfo GetGroupBysAMAccountName(string samAccountName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365GroupInfo>(dataAccessTransaction).Get(item => item.sAMAccountName == samAccountName).FirstOrDefault());
            }
        }

        public static void SaveGroups(IList<Office365GroupInfo> list, long endpointId, string currentUser)
        {
            try
            {
                var allExistingGroups = Office365GroupInfo.GetAllByEndpointId(endpointId);
                Office365GroupInfo existingGroup = null;
                foreach (Office365GroupInfo group in list)
                {
                    existingGroup = allExistingGroups.Where(grp => grp.sAMAccountName == group.sAMAccountName).FirstOrDefault();
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
            catch (Exception)
            {
                throw;
            }
        }

        public static IList<Office365GroupInfo> GetDeletedGroups(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<Office365GroupInfo> listDeletedUsers = new List<Office365GroupInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.Office365GroupInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveOffice365Groups(List<Office365GroupInfo> list, long endpointId)
        {
            foreach (Office365GroupInfo group in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new O365GroupInfoRepository(dataAccessTransaction).DeleteGroupUser(group.Id, endpointId);
                    dataAccessTransaction.Commit();
                }
                group.Delete();
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new O365GroupInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }
    }
}

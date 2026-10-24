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
    public class IACMGroupInfo : DomainModelBase<IACMGroupInfo, EntityModel.IACMGroupInfo>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<IACMUserInfo> Users { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public string UserDefinedDesc { get; set; }

        public bool? CanRequest { get; set; }

        public long? Level1Approver { get; set; }

        public long? Level2Approver { get; set; }

        public bool? IsLevel1Group { get; set; }

        public bool? IsLevel2Group { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementDesc { get; set; }

        public EntitlementType EntitlementType { get { return EntitlementType.Group; } }

        public static IACMGroupInfo GetByGroupName(string groupName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMGroupInfo>(tranaction).Get(role => role.Name == groupName).FirstOrDefault());
            }
        }
        public static IList<IACMGroupInfo> GetDeletedByEndPointSystemId(long endPointSystemId, long discProcessId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<IACMGroupInfo> listDeletedGroups = new List<IACMGroupInfo>();
                Process process = Process.GetByProcessId(discProcessId);
                var discoveryProcess = DiscoveryProcess.GetById(discProcessId);
                if (discoveryProcess != null && process != null)
                {
                    listDeletedGroups =
                        ORMapList(new RepositoryBase<EntityModel.IACMGroupInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId &&
                         (grp.LastModifiedDate < process.ProcessStartAt ||
                                           (grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)
                        )).ToList());
                }

                return listDeletedGroups;
            }
        }
        //public static IList<IACMGroupInfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<IACMGroupInfo> listDeletedGroups = new List<IACMGroupInfo>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.IACMGroupInfo>
        //                    (dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId
        //                    && (grp.LastModifiedDate < process.ProcessStartAt ||
        //                    grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)).ToList());
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}

        public static List<IACMGroupInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.IACMGroupInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<Group> GetAllByEndpointSystemId(long endpointSystemId, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                Group group;
                List<Group> grpList = new List<Group>();

                var grps = ORMapList(new RepositoryBase<EntityModel.IACMGroupInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());

                if (grps != null && grps.Count() > 0)
                {
                    foreach (var adsGrp in grps)
                    {
                        group = new Group();
                        group.Id = CheckNull.AgainstInt(adsGrp.Id);
                        group.GroupName = CheckNull.AgainstString(adsGrp.Name);
                        group.GroupDescription = CheckNull.AgainstString(adsGrp.Description);
                        grpList.Add(group);
                    }
                }
                return grpList;
            }
        }

        public static Dictionary<long, string> GetAllByName(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new ADSGroupInfoRepository(dataAccessTransaction).GetAllByName(CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<IACMGroupInfo> GetADGroupsByUserId(long userId)
        {
            IACMUserInfo adUserInfo = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<IACMGroupInfo> adGroups = new List<IACMGroupInfo>();
                adUserInfo = IACMUserInfo.GetByIdIncludeRolesGroups(userId);

                if (adUserInfo != null)
                {
                    adGroups = (List<IACMGroupInfo>)adUserInfo.Groups;
                }
                return adGroups;
            }
        }

        public static Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? endpointUserId, bool existingEntitlements)
        {
            IList<IACMGroupInfo> list = null;
            Dictionary<long, string> result = null;

            if (existingEntitlements)
            {
                list = IACMGroupInfo.GetADGroupsByUserId(endpointUserId.Value);
                result = list.Where(item => item.Name.Contains(searchString))
                .Select(x => new { Key = x.Id, Value = x.Name })
                .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            else
            {
                result = IACMGroupInfo.GetAllByName(searchString);
            }
            return result;
        }

        public static void SaveGroups(IList<IACMGroupInfo> list, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<IACMGroupInfo> listExistingCMSRoles = new List<IACMGroupInfo>();
                IACMGroupInfo adexistingGroups = null;
                listExistingCMSRoles = IACMGroupInfo.GetAll(false);
                foreach (IACMGroupInfo grp in list)
                {
                    adexistingGroups = listExistingCMSRoles.Where(t => t.Name == grp.Name).FirstOrDefault();
                    if (adexistingGroups == null)
                    {
                        grp.CreatedDate = DateTime.Now;
                        grp.CreatedBy = currentUser;
                        grp.Save(currentUser);
                    }
                    else
                    {
                        grp.Id = adexistingGroups.Id;
                        grp.CreatedDate = adexistingGroups.CreatedDate;
                        grp.CreatedBy = adexistingGroups.CreatedBy;
                        grp.LastModifiedDate = DateTime.Now;
                        grp.LastModifiedBy = currentUser;
                        grp.Save(currentUser);
                    }
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void RemoveRoles(List<IACMGroupInfo> roleList)
        {
            string executeQuery = string.Empty;
            foreach (IACMGroupInfo role in roleList)
            {
                executeQuery = string.Format("Delete from IACMUserInfoIACMGroupInfo where IACMGroupInfo_Id ={0}", role.Id);
                IACMGroupInfo.ExecuteQuery(executeQuery);
                role.Delete();
            }
        }
    }
}

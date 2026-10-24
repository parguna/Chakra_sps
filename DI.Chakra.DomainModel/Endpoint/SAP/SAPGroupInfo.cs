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
    public class SAPGroupInfo : DomainModelBase<SAPGroupInfo, EntityModel.SAPGroupInfo>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }

        public string GROUP_CODE { get; set; }

        public string GROUP_DESC { get; set; }

        public Nullable<int> TargetSysId { get; set; } 

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPUserinfo> SAPUserinfos { get; set; }

        public string EntitlementName
        {
            get { return GROUP_CODE; }
        }

        public string EntitlementDesc
        {
            get { return GROUP_DESC; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }


        public string Reserved1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved3
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved4
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved5
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public string Reserved6
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved7
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved8
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved9
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Reserved10
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static SAPGroupInfo GetByEndPointSystemId(long endPointSystemId,string groupCode)    
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction)
                       .Get(group => group.GROUP_CODE == groupCode && group.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static SAPGroupInfo GetByGroupCode(string groupCode) 
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction)
                       .Get(group => group.GROUP_CODE == groupCode).FirstOrDefault());
            }
        }

        public static void UpdateSAPUserinfoSAPGroupInfoByEndPointSystemId(long endPointSystemId, long id) 
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(endPointSystemId, id);
                dataAccessTransaction.Commit();
            }
        }


        public static List<SAPGroupInfo> GetByEndPointSystemId(long endPointSystemId)  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction)
                       .Get(sapGroup => sapGroup.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static void SaveGroups(IList<SAPGroupInfo> list, long endpointSystemId,string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPGroupInfo> listExistingSAPgroups = new List<SAPGroupInfo>();
                SAPGroupInfo sapexistingGroups = null;
                listExistingSAPgroups = SAPGroupInfo.GetByEndPointSystemId(endpointSystemId);
                foreach (SAPGroupInfo grp in list)
                {
                    sapexistingGroups = listExistingSAPgroups.Where(t => t.GROUP_CODE == grp.GROUP_CODE).FirstOrDefault();
                    if (sapexistingGroups == null)
                    {
                        grp.Save(dataAccessTransaction);
                    }
                    else
                    {
                        grp.Id = sapexistingGroups.Id;
                        grp.GROUP_DESC = sapexistingGroups.GROUP_DESC;
                        grp.CreatedDate = sapexistingGroups.CreatedDate;
                        grp.CreatedBy = sapexistingGroups.CreatedBy;
                        grp.Save(dataAccessTransaction);
                    }
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void SaveGroups(IList<SAPGroupInfo> list, string currentUser, Dictionary<string, SAPGroupInfo> dictExistingGroups)
        {
            SAPGroupInfo existingGroup = null;
            foreach (SAPGroupInfo grp in list)
            {
                // if existing group
                if (dictExistingGroups.ContainsKey(grp.GROUP_CODE))
                {
                    existingGroup = dictExistingGroups[grp.GROUP_CODE];
                    existingGroup.GROUP_CODE = grp.GROUP_CODE;
                    existingGroup.GROUP_DESC = grp.GROUP_DESC;
                    existingGroup.Save(currentUser);
                    existingGroup = null;
                }
                else // if new group
                {
                    grp.Save(currentUser);
                    dictExistingGroups.Add(grp.GROUP_CODE, grp);
                }                
            }

            dictExistingGroups = null;
        }

        public static IList<SAPGroupInfo> GetDeletedByEndPointSystemId(long endpointsystemId) 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPGroupInfo> listDeletedGroups = new List<SAPGroupInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointsystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedGroups=ORMapList(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction)
                       .Get(d => d.EndPointSystemId == endpointsystemId && d.LastModifiedDate < process.ProcessStartAt).ToList());
                    }
                }
                return listDeletedGroups;
            }
        }

        public static List<SAPGroupInfo> GetAllSAPGroups(long endPointSystemId, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPGroupInfo> tmpList1 = ORMapList(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.GROUP_CODE.Contains(searchString)).OrderBy(grp => grp.GROUP_CODE).Distinct().ToList());
                return tmpList1;
            }
        }
        public static List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            Group group;
            List<Group> grpList = new List<Group>();
            List<SAPGroupInfo> sapList = SAPGroupInfo.GetAllSAPGroups(endPointID, CheckNull.AsteriskToString(searchString));
            var sapGrpList = (from tmp in sapList select new { GroupId = tmp.Id, GroupName = tmp.GROUP_CODE, Description = tmp.GROUP_DESC }).Distinct();

            if (sapGrpList.Count() > 0)
            {
                foreach (var tLst in sapGrpList)
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

        public static IList<SAPGroupInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPGroupInfo>(dataAccessTransaction).Get(groupInfo => groupInfo.EndPointSystemId == endPointSystemId
                && ((groupInfo.LastModifiedDate == null && groupInfo.CreatedDate < modifiedDate) || (groupInfo.LastModifiedDate != null && groupInfo.LastModifiedDate < modifiedDate))).ToList());
            }
        }
    }
}

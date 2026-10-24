using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{

    public class GEMSRoleInfo : DomainModelBase<GEMSRoleInfo, EntityModel.GEMSRoleInfo>, IEndpointEntitlementInfo
    {
        public GEMSRoleInfo()
        {
            this.GEMSUserInfos = new List<GEMSUserInfo>();
        }

        public int RoleId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Display_Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Extension_ID { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<GEMSUserInfo> GEMSUserInfos { get; set; }
        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return this.Name; }
        }

        public string EntitlementDesc
        {
            get { return this.Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
        

        public static GEMSRoleInfo GetByRoleId(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction)
                      .Get(grp => grp.RoleId == roleId).FirstOrDefault());
            }
        }

        public static GEMSRoleInfo GetByRoleName(string roleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction)
                      .Get(grp => grp.Name == roleName).FirstOrDefault());
            }
        }

        //public static IList<GEMSRoleInfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<GEMSRoleInfo> listDeletedGroups = new List<GEMSRoleInfo>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction)
        //                    .Get(grp => grp.EndPointSystemId == endPointSystemId && ((grp.LastModifiedDate == null && grp.CreatedDate < process.ProcessStartAt ) || grp.LastModifiedDate < process.ProcessStartAt)).ToList());
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}

        public static List<GEMSRoleInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveRoles(IList<GEMSRoleInfo> list, string currentUser, long endpointId)
        {
            try
            {
                IList<GEMSRoleInfo> listExistingRoles = new List<GEMSRoleInfo>();
                GEMSRoleInfo existingRoles = null;
                listExistingRoles = GEMSRoleInfo.GetAllByEndpointSystemId(endpointId);
                foreach (GEMSRoleInfo grp in list)
                {
                    try
                    {
                        existingRoles = listExistingRoles.Where(t => t.Name == grp.Name).FirstOrDefault();
                        if (existingRoles == null)
                        {
                            grp.Save(currentUser);
                        }
                        else
                        {
                            grp.Id = existingRoles.Id;
                            grp.CreatedBy = existingRoles.CreatedBy;
                            grp.CreatedDate = existingRoles.CreatedDate;

                            grp.Save(currentUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers.", ex);
            }
        }

        public static List<GEMSRoleInfo> GetAllADSGroups(long endPointSystemId, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.Name.Contains(searchString)).OrderBy(grp => grp.Name).ToList());

            }
        }

        public static void RemoveGEMSRoles(List<GEMSRoleInfo> list)
        {
            foreach (GEMSRoleInfo role in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM GEMSUserInfoGEMSRoleInfo WHERE GEMSRoleInfo_Id=" + role.Id + " and EndpointSystemId = " + role.EndPointSystemId + ""));
                    dataAccessTransaction.Commit();
                }
                role.Delete();
            }
        }

        public static Dictionary<long, string> GetByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new ADSGroupInfoRepository(dataAccessTransaction).GetByEndpointId(endpointId);
            }
        }
        public static IList<GEMSRoleInfo> GetDeletedByEndPointSystemId(long endPointSystemId, long discProcessId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<GEMSRoleInfo> listDeletedGroups = new List<GEMSRoleInfo>();
                Process process = Process.GetByProcessId(discProcessId);
                var discoveryProcess = DiscoveryProcess.GetById(discProcessId);
                if (discoveryProcess != null && process != null)
                {
                    listDeletedGroups =
                        ORMapList(new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId &&
                         (grp.LastModifiedDate < process.ProcessStartAt ||
                                           (grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)
                        )).ToList());
                }

                return listDeletedGroups;
            }
        }
    }
}

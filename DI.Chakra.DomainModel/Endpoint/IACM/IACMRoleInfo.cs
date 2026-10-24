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
    public class IACMRoleInfo : DomainModelBase<IACMRoleInfo, EntityModel.IACMRoleInfo>, IEndpointEntitlementInfo
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

        public bool? CanRequest { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public string UserDefinedDesc { get; set; }

        public Nullable<long> Level1Approver { get; set; }

        public Nullable<long> Level2Approver { get; set; }

        public Nullable<bool> IsLevel1Group { get; set; }

        public Nullable<bool> IsLevel2Group { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementDesc { get; set; }

        public EntitlementType EntitlementType { get { return EntitlementType.Role; } }

        public static IACMRoleInfo GetByRoleName(string roleName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMRoleInfo>(tranaction).Get(role => role.Name == roleName).FirstOrDefault());
            }
        }

        //public static IList<IACMRoleInfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<IACMRoleInfo> listDeletedGroups = new List<IACMRoleInfo>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.IACMRoleInfo>
        //                    (dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId
        //                    && (grp.LastModifiedDate < process.ProcessStartAt ||
        //                    grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)).ToList()); 
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}
        public static IList<IACMRoleInfo> GetDeletedByEndPointSystemId(long endPointSystemId, long discProcessId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<IACMRoleInfo> listDeletedGroups = new List<IACMRoleInfo>();
                Process process = Process.GetByProcessId(discProcessId);
                var discoveryProcess = DiscoveryProcess.GetById(discProcessId);
                if (discoveryProcess != null && process != null)
                {
                    listDeletedGroups =
                        ORMapList(new RepositoryBase<EntityModel.IACMRoleInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId &&
                         (grp.LastModifiedDate < process.ProcessStartAt ||
                                           (grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null)
                        )).ToList());
                }

                return listDeletedGroups;
            }
        }
        public static void SaveRoles(IList<IACMRoleInfo> list, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<IACMRoleInfo> listExistingCMSRoles = new List<IACMRoleInfo>();
                IACMRoleInfo adexistingGroups = null;
                listExistingCMSRoles = IACMRoleInfo.GetAll();
                foreach (IACMRoleInfo grp in list)
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

        public static void RemoveRoles(List<IACMRoleInfo> roleList)
        {
            string executeQuery = string.Empty;
            foreach (IACMRoleInfo role in roleList)
            {
                executeQuery = string.Format("Delete from IACMRoleInfoIACMUserInfo where IACMRoleInfo_Id ={0}", role.Id);
                IACMRoleInfo.ExecuteQuery(executeQuery);
                role.Delete();
            }
        }

    }
}

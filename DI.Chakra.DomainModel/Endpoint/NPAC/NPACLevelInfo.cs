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

    public class NPACLevelInfo : DomainModelBase<NPACLevelInfo, EntityModel.NPACLevelInfo>, IEndpointEntitlementInfo
    {
        public NPACLevelInfo()
        {
            this.NPACUserInfos = new List<NPACUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<NPACUserInfo> NPACUserInfos { get; set; }
        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return Convert.ToString(this.Level); }
        }

        public string EntitlementDesc
        {
            get { return this.Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Level; }
        }

        public static NPACLevelInfo GetByLevel(int level)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NPACLevelInfo>(dataAccessTransaction)
                      .Get(grp => grp.Level == level).FirstOrDefault());
            }
        }     

        //public static IList<NPACLevelInfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<NPACLevelInfo> listDeletedGroups = new List<NPACLevelInfo>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.NPACLevelInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.LastModifiedDate < process.ProcessStartAt).ToList());
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}

        public static List<NPACLevelInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NPACLevelInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveRoles(IList<NPACLevelInfo> list, string currentUser, long endpointId)
        {
            try
            {
                IList<NPACLevelInfo> listExistingRoles = new List<NPACLevelInfo>();
                NPACLevelInfo existingRoles = null;
                listExistingRoles = NPACLevelInfo.GetAllByEndpointSystemId(endpointId);
                foreach (NPACLevelInfo grp in list)
                {
                    try
                    {
                        existingRoles = listExistingRoles.Where(t => t.Level == grp.Level).FirstOrDefault();
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
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SAVE NPAC Level.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers.", ex);
            }
        }
    }
}

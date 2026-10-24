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
    public class SMSAccessLevel : DomainModelBase<SMSAccessLevel, EntityModel.SMSAccessLevel>, IEndpointEntitlementInfo
    {
        public SMSAccessLevel()
        {
            this.SMSUserInfo = new List<SMSUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public int AccessLevelId { get; set; }
        public string Name { get; set; }
        public long SegmentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public List<SMSUserInfo> SMSUserInfo { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName
        {
            get { return this.Name; }
        }
        public string EntitlementDesc
        {
            get { return this.Name; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.AccessLevel; }
        }

        public static IList<SMSAccessLevel> GetDeletedByEndPointSystemId(long endPointSystemId, Process process)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<SMSAccessLevel> listDeletedGroups = new List<SMSAccessLevel>();

                if (process != null)
                {
                    listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.SMSAccessLevel>(dataAccessTransaction)
                        .Get(grp => grp.EndPointSystemId == endPointSystemId && ((grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null) || grp.LastModifiedDate < process.ProcessStartAt)).ToList());
                }

                return listDeletedGroups;
            }
        }
        public static List<SMSAccessLevel> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSAccessLevel>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<SMSAccessLevel> GetAllByEndpointSystemId(long endpointSystemId, string[] accessLevel)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSAccessLevel>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId && accessLevel.Contains(grp.Name)).ToList());
            }
        }

        public static SMSAccessLevel GetAccessLevelByName(string accessLevel)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSAccessLevel>(dataAccessTransaction)
                      .Get(grp => grp.Name == accessLevel).FirstOrDefault());
            }
        }

        public static void SaveAccessLevels(IList<SMSAccessLevel> list, long endpointId, string currentUser)
        {
            try
            {
                var allexistingAccessLevels = new List<SMSAccessLevel>();

                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    allexistingAccessLevels= ORMapList(new RepositoryBase<EntityModel.SMSAccessLevel>(dataAccessTransaction)
                          .Get(grp => grp.EndPointSystemId == endpointId).ToList());
                }

                if (allexistingAccessLevels == null)
                    allexistingAccessLevels = new List<SMSAccessLevel>();

                SMSAccessLevel existingaccess = null;
                foreach (SMSAccessLevel accesslevel in list)
                {
                    existingaccess = allexistingAccessLevels.Where(c => c.AccessLevelId == accesslevel.AccessLevelId).FirstOrDefault();
                    if (existingaccess == null)
                    {
                        accesslevel.CreatedBy = "System";
                        accesslevel.CreatedDate = DateTime.Now;
                        accesslevel.Save("System");
                        allexistingAccessLevels.Add(accesslevel);
                    }
                    else
                    {
                        accesslevel.Id = existingaccess.Id;
                        accesslevel.CreatedBy = "System";
                        accesslevel.CreatedDate = DateTime.Now;

                        accesslevel.Save("System");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void RemoveSMSAccessLevels(List<SMSAccessLevel> list)
        {
            foreach (SMSAccessLevel access in list)
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    new SMSUserInfoRepository(dataAccessTransaction).DeleteUserAccessDataByAccessLevelId(access.EndPointSystemId, access.Id);
                    dataAccessTransaction.Commit();
                }

                access.Delete();
            }
        }
    }
}

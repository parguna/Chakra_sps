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
    public class SMSBadgeInfo : DomainModelBase<SMSBadgeInfo, EntityModel.SMSBadgeInfo>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int BadgeKey { get; set; }
        public long BadgeNumber { get; set; }
        //card holder id
        public long UserId { get; set; }
        public int STATUS { get; set; }
        public int BadgeType { get; set; }
        public int DEST_EXEMPT { get; set; }
        public string ACTIVATE { get; set; }
        public string DEACTIVATE { get; set; }
        #endregion
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return Convert.ToString(this.BadgeKey); }
        }

        public string EntitlementDesc
        {
            get { return Convert.ToString(this.BadgeNumber); }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Badge; }
        }

        public static IList<SMSBadgeInfo> GetDeletedByEndPointSystemId(long endPointSystemId, Process process)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                IList<SMSBadgeInfo> listDeletedGroups = new List<SMSBadgeInfo>();
                if (process != null)
                {
                    listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                        .Get(grp => grp.EndPointSystemId == endPointSystemId &&((grp.CreatedDate < process.ProcessStartAt && grp.LastModifiedDate == null) || grp.LastModifiedDate < process.ProcessStartAt)).ToList());
                }
                return listDeletedGroups;
            }
        }
        public static List<SMSBadgeInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        public static void SaveBadges(IList<SMSBadgeInfo> list, long endpointId, string currentUser)
        {
            try
            {
                var allexistingBadges = SMSBadgeInfo.GetAllByEndpointSystemId(endpointId);
                if (allexistingBadges == null)
                    allexistingBadges = new List<SMSBadgeInfo>();
                SMSBadgeInfo existingBadge = null;

                foreach (SMSBadgeInfo badge in list)
                {
                    existingBadge = allexistingBadges.Where(c => c.BadgeKey == badge.BadgeKey).FirstOrDefault();
                    if (existingBadge == null)
                    {
                        badge.CreatedBy = currentUser;
                        badge.CreatedDate = DateTime.Now;
                        badge.Save(currentUser);

                        allexistingBadges.Add(badge);
                    }
                    else
                    {
                        badge.Id = existingBadge.Id;
                        badge.CreatedBy = currentUser;
                        badge.CreatedDate = DateTime.Now;

                        badge.Save(currentUser);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void RemoveSMSBadges(List<SMSBadgeInfo> list)
        {
            foreach (SMSBadgeInfo badge in list)
            {
                badge.Delete();
            }
        }
        public static SMSBadgeInfo GetByBadgeUserId(int badgeUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserId == badgeUserId && (grp.STATUS == 1)).FirstOrDefault());
            }
        }
        public static List<SMSBadgeInfo> GetByBadgesByUserId(int badgeUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserId == badgeUserId ).ToList());
            }
        }
        /// <summary>
        /// There might be the case where Status = 5 Not returned. hence commented status check.
        /// </summary>
        /// <param name="badgeUserId"></param>
        /// <returns></returns>
        public static SMSBadgeInfo GetByBadgeUserIdRejoin(int badgeUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserId == badgeUserId 
                      //&& (grp.STATUS == 1 || grp.STATUS == 3)
                      ).FirstOrDefault());
            }
        }
        public static SMSBadgeInfo GetByBadgeKey(int badgeKey)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSBadgeInfo>(dataAccessTransaction)
                      .Get(grp => grp.BadgeKey == badgeKey).FirstOrDefault());
            }
        }
        public static void SaveBadges(IList<SMSBadgeInfo> list)
        {
            try
            {
                SMSBadgeInfo existingBadge = null;
                foreach (SMSBadgeInfo badge in list)
                {

                    existingBadge = SMSBadgeInfo.GetByBadgeKey(badge.BadgeKey);
                    if (existingBadge == null)
                    {
                        badge.CreatedBy = "System";
                        badge.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        badge.Id = existingBadge.Id;
                        badge.CreatedDate = existingBadge.CreatedDate;
                        badge.CreatedBy = existingBadge.CreatedBy;
                    }
                    badge.Save("System");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

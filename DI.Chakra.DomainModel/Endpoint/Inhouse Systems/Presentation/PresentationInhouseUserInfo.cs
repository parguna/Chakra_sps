using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationInhouseUserInfo
    {
        public long ReconciliationId { get; set; }
        public long UserId { get; set; }
        public long Id { get; set; }
        public long? DIUserID { get; set; }
        public string UserName { get; set; }
        public string InhouseSystemCode { get; set; }
        public string DisplayName { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public int InhouseSystemId { get; set; }
        public Nullable<int> EndpointSystemId { get; set; }
        public string SystemName { get; set; }
        public string ApplicationOwner { get; set; }
        public string Permission { get; set; }
        public string SystemCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public AccountType AccountType { get; set; }
        public LockScenario Lockscenario { get; set; }


        public System.DateTime? AccountCreatedDate { get; set; }
        public System.DateTime? LastLoginDate { get; set; }
        public System.DateTime? DisabledDate { get; set; }
        public System.DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }

        public new static List<PresentationInhouseUserInfo> GetByInhousesystemcode(string diuserid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByInhousesystemcode(diuserid));
            }
        }

        public static List<PresentationInhouseUserInfo> GetByEndpointIdAndAccountWithNoGroups(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction())
            {
                IList<PresentationInhouseUserInfo> presentationINHUsersReportDataList = new List<PresentationInhouseUserInfo>();

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    presentationINHUsersReportDataList = ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByEndpointIdAndAccountWithNoGroups(endPointSysemId));
                }

                return presentationINHUsersReportDataList.ToList();
            }

        }
        public static List<PresentationInhouseUserInfo> GetInhouseUserInfoBySystemCode(string inhousesystemcode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var inhUsers = new InhouseUserInfoRepository(dataAccessTransaction).GetInhouseUserInfoBySystemCode(inhousesystemcode).ToList();
                var entitlements = EntitlementReconcilation.GetAllByEndpointSystemTypeId(Convert.ToInt32(EndpointType.InhouseSystems));
                IEnumerable<string> userEntitlements;
                foreach (var inhUser in inhUsers)
                {
                    userEntitlements = entitlements.Where(p => p.IsDeleted == false && p.AccountReconcilationId == inhUser.ReconciliationId && !p.IsDeleted).Select(p => p.Entitlement);
                    inhUser.Permission = string.Join(",", userEntitlements);
                }
                return ORMapList(inhUsers);
            }
        }
        public static IList<PresentationInhouseUserInfo> GetByEndpointSystemId(long endpointSystemid)
        {
            IList<PresentationInhouseUserInfo> presentationINHUsersReportDataList = new List<PresentationInhouseUserInfo>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                presentationINHUsersReportDataList = ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByEndpointSystemIdIncludeEndPointSystem(endpointSystemid));
            }

            return presentationINHUsersReportDataList;
        }

        public static PresentationInhouseUserInfo ORMap(EntityModel.Presentation.PresentationInhouseUserInfo presentationTabMenuEntityModel)
        {
            if (presentationTabMenuEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationInhouseUserInfo, PresentationInhouseUserInfo>(presentationTabMenuEntityModel);
        }

        public static List<PresentationInhouseUserInfo> ORMapList(IList<EntityModel.Presentation.PresentationInhouseUserInfo> presentationInhouseGridEntityModels)
        {
            return presentationInhouseGridEntityModels.Select(ORMap).ToList();
        }

    }
}

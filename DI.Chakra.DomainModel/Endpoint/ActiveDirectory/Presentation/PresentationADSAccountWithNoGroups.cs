using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSAccountWithNoGroups
    {
        public long UserId { get; set; }

        public string sAMAccountName { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public string whenCreated { get; set; }

        public string PasswordExpirationDate { get; set; }

        public string lastLogon { get; set; }

        public string pwdLastSet { get; set; }

        public string accountExpires { get; set; }

        public string logonCount { get; set; }

        public bool IsAccountDisabled { get; set; }

        public bool IsAccountLocked { get; set; }

        public string cn { get; set; }

        public string HostName { get; set; }

        public string Institution { get; set; }

        public string IsMapped { get; set; }

        public string Owner { get; set; }

        public string physicalDeliveryOfficeName { get; set; }

        public string company { get; set; }

        public string pager { get; set; }

        public string businessCategory { get; set; }

        public string division { get; set; }

        public string mail { get; set; }

        public string title { get; set; }

        public string IsLocked { get; set; }

        public LockScenario LockScenario { get; set; }

        public string OU { get; set; }

        public static PresentationADSAccountWithNoGroups ORMap(EntityModel.Presentation.PresentationADSAccountWithNoGroups presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSAccountWithNoGroups, PresentationADSAccountWithNoGroups>(presentationADSUsersEntityModel);
        }

        public static List<PresentationADSAccountWithNoGroups> ORMapList(IList<EntityModel.Presentation.PresentationADSAccountWithNoGroups> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSAccountWithNoGroups> GetByEndpointIdIncludeAccountWithoutGroups(long endpointSystemid)
        {
            
            IList<PresentationADSAccountWithNoGroups> presentationADSUsersReportDataList = new List<PresentationADSAccountWithNoGroups>();
            PresentationADSAccountWithNoGroups PresentationADSAccountWithNoGroups;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<ADSUserInfo> adsUserInfoList = ADSUserInfo.GetByEndpointIdAndAccountWithNoGroups(endpointSystemid);
                foreach (ADSUserInfo usr in adsUserInfoList)
                {
                    PresentationADSAccountWithNoGroups = new PresentationADSAccountWithNoGroups();
                    PresentationADSAccountWithNoGroups.UserId = usr.Id;
                    PresentationADSAccountWithNoGroups.sAMAccountName = usr.sAMAccountName;
                    PresentationADSAccountWithNoGroups.displayName = usr.displayName;
                    PresentationADSAccountWithNoGroups.physicalDeliveryOfficeName = usr.physicalDeliveryOfficeName;
                    PresentationADSAccountWithNoGroups.company = usr.company;
                    PresentationADSAccountWithNoGroups.pager = usr.pager;
                    PresentationADSAccountWithNoGroups.businessCategory = usr.businessCategory;
                    PresentationADSAccountWithNoGroups.division = usr.division;
                    PresentationADSAccountWithNoGroups.mail = usr.mail;
                    PresentationADSAccountWithNoGroups.title = usr.title;
                    PresentationADSAccountWithNoGroups.whenCreated = usr.whenCreated.HasValue ? usr.whenCreated.Value.ToString() : string.Empty;
                    PresentationADSAccountWithNoGroups.PasswordExpirationDate = usr.PasswordExpirationDate.HasValue ? usr.PasswordExpirationDate.Value.ToString() : string.Empty;
                    PresentationADSAccountWithNoGroups.lastLogon = usr.lastLogon.HasValue ? usr.lastLogon.Value.ToString() : string.Empty;
                    PresentationADSAccountWithNoGroups.pwdLastSet = usr.pwdLastSet.HasValue ? usr.pwdLastSet.Value.ToString() : string.Empty;
                    PresentationADSAccountWithNoGroups.accountExpires = usr.accountExpires.HasValue ? usr.accountExpires.Value.ToString() : string.Empty;
                    PresentationADSAccountWithNoGroups.logonCount = usr.logonCount;
                    PresentationADSAccountWithNoGroups.IsAccountDisabled = usr.IsAccountDisabled;
                    PresentationADSAccountWithNoGroups.cn = usr.cn;
                    PresentationADSAccountWithNoGroups.OU = usr.ou;
                    PresentationADSAccountWithNoGroups.HostName = EndPoint.GetById(endpointSystemid).DisplayName;
                    //below code is added to show the owner name in the report
                    var owner = AccountReconcilation.GetByEndpointUserId(PresentationADSAccountWithNoGroups.UserId, endpointSystemid);

                    if (owner != null)
                    {
                        if (owner.UserId.HasValue)
                        {
                            var user = User.GetById(owner.UserId.Value);
                            PresentationADSAccountWithNoGroups.Owner = usr.displayName;
                            PresentationADSAccountWithNoGroups.IsMapped = bool.TrueString;
                        }
                    }
                    presentationADSUsersReportDataList.Add(PresentationADSAccountWithNoGroups);
                }
            }
            return presentationADSUsersReportDataList;
        }
    }
}

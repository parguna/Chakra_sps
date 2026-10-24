using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSLockedDisabledAccount
    {
        public long UserId { get; set; }

        public string sAMAccountName { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public Nullable<DateTime> whenCreated { get; set; }

        public Nullable<DateTime> PasswordExpirationDate { get; set; }

        public Nullable<DateTime> lastLogon { get; set; }

        public Nullable<DateTime> pwdLastSet { get; set; }

        public Nullable<DateTime> accountExpires { get; set; }

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

        public string IsDisabled { get; set; }

        public string OU { get; set; }

        public static PresentationADSLockedDisabledAccount ORMap(EntityModel.Presentation.PresentationADSLockedDisabledAccount presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSLockedDisabledAccount, PresentationADSLockedDisabledAccount>(presentationADSUsersEntityModel);
        }

        public static List<PresentationADSLockedDisabledAccount> ORMapList(IList<EntityModel.Presentation.PresentationADSLockedDisabledAccount> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSLockedDisabledAccount> GetByEndpointIdIncludeLockedDisabledAccounts(long endpointSystemid)
        {
            
            IList<PresentationADSLockedDisabledAccount> presentationADSUsersReportDataList = new List<PresentationADSLockedDisabledAccount>();
            PresentationADSLockedDisabledAccount PresentationADSLockedDisabledAccount;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<ADSUserInfo> adsUserInfoList = ADSUserInfo.GetByEndpointIdAndLockedDisabledAccounts(endpointSystemid);
                foreach (ADSUserInfo usr in adsUserInfoList)
                {
                    PresentationADSLockedDisabledAccount = new PresentationADSLockedDisabledAccount();
                    PresentationADSLockedDisabledAccount.UserId = usr.Id;
                    PresentationADSLockedDisabledAccount.sAMAccountName = usr.sAMAccountName;
                    PresentationADSLockedDisabledAccount.displayName = usr.displayName;
                    PresentationADSLockedDisabledAccount.physicalDeliveryOfficeName = usr.physicalDeliveryOfficeName;
                    PresentationADSLockedDisabledAccount.company = usr.company;
                    PresentationADSLockedDisabledAccount.pager = usr.pager;
                    PresentationADSLockedDisabledAccount.businessCategory = usr.businessCategory;
                    PresentationADSLockedDisabledAccount.division = usr.division;
                    PresentationADSLockedDisabledAccount.mail = usr.mail;
                    PresentationADSLockedDisabledAccount.title = usr.title;
                    //PresentationADSLockedDisabledAccount.whenCreated = usr.whenCreated.HasValue ? usr.whenCreated.Value.ToString() : string.Empty;
                    //PresentationADSLockedDisabledAccount.PasswordExpirationDate = usr.PasswordExpirationDate.HasValue ? usr.PasswordExpirationDate.Value.ToString() : string.Empty;
                    //PresentationADSLockedDisabledAccount.lastLogon = usr.lastLogon.HasValue ? usr.lastLogon.Value.ToString() : string.Empty;
                    //PresentationADSLockedDisabledAccount.pwdLastSet = usr.pwdLastSet.HasValue ? usr.pwdLastSet.Value.ToString() : string.Empty;
                    //PresentationADSLockedDisabledAccount.accountExpires = usr.accountExpires.HasValue ? usr.accountExpires.Value.ToString() : string.Empty;
                    if (usr.PasswordExpirationDate.HasValue)
                        PresentationADSLockedDisabledAccount.PasswordExpirationDate = usr.PasswordExpirationDate;
                    if (usr.accountExpires.HasValue)
                        PresentationADSLockedDisabledAccount.accountExpires = usr.accountExpires;
                    PresentationADSLockedDisabledAccount.logonCount = usr.logonCount;
                    PresentationADSLockedDisabledAccount.IsAccountDisabled = usr.IsAccountDisabled; 
                    PresentationADSLockedDisabledAccount.IsDisabled = usr.IsAccountDisabled ? "Yes" : "No";
                    PresentationADSLockedDisabledAccount.IsAccountLocked = usr.IsAccountLocked;
                    PresentationADSLockedDisabledAccount.IsLocked = usr.IsAccountLocked ? "Yes" : "No";
                    PresentationADSLockedDisabledAccount.cn = usr.cn;
                    PresentationADSLockedDisabledAccount.OU = usr.ou;
                    PresentationADSLockedDisabledAccount.HostName = EndPoint.GetById(endpointSystemid).DisplayName;
                    presentationADSUsersReportDataList.Add(PresentationADSLockedDisabledAccount);
                }
            }
            return presentationADSUsersReportDataList;
        }
    }
}

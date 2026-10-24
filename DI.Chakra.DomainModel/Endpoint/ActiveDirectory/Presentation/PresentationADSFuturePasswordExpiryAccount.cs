using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSFuturePasswordExpiryAccount
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

        public string IsAccountDisabled { get; set; }

        public string IsAccountLocked { get; set; }

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

        public static PresentationADSFuturePasswordExpiryAccount ORMap(EntityModel.Presentation.PresentationADSFuturePasswordExpiryAccount presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSFuturePasswordExpiryAccount, PresentationADSFuturePasswordExpiryAccount>(presentationADSUsersEntityModel);
        }

        public static List<PresentationADSFuturePasswordExpiryAccount> ORMapList(IList<EntityModel.Presentation.PresentationADSFuturePasswordExpiryAccount> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSFuturePasswordExpiryAccount> GetByEndpointSystemIdAndPasswordExpiryDays(long endpointSystemid, int passwordExpiryDays)
        {

            IList<PresentationADSFuturePasswordExpiryAccount> presentationADSFuturePasswordExpiryAccountReportDataList = new List<PresentationADSFuturePasswordExpiryAccount>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                presentationADSFuturePasswordExpiryAccountReportDataList = ORMapList(new ADSUserInfoRepository(dataAccessTransaction).GetByEndpointIdAndPasswordExpiryDays(endpointSystemid, passwordExpiryDays));
            }

            return presentationADSFuturePasswordExpiryAccountReportDataList;
        }
    }
}

using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSUsers
    {
        public long UserId { get; set; }

        public string sAMAccountName { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? PasswordExpirationDate { get; set; }

        public DateTime? lastLogon { get; set; }

        public DateTime? pwdLastSet { get; set; }

        public DateTime? accountExpires { get; set; }

        public string logonCount { get; set; }

        //public bool IsAccountDisabled { get; set; }
        public string IsAccountDisabled { get; set; }

        //public bool IsAccountLocked { get; set; }
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
        public string description { get; set; }

        public static PresentationADSUsers ORMap(EntityModel.Presentation.PresentationADSUsers presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSUsers, PresentationADSUsers>(presentationADSUsersEntityModel);
        }

        public static List<PresentationADSUsers> ORMapList(IList<EntityModel.Presentation.PresentationADSUsers> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSUsers> GetByEndpointSystemId(long endpointSystemid)
        {
            IList<PresentationADSUsers> presentationADSUsersReportDataList = new List<PresentationADSUsers>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                presentationADSUsersReportDataList = ORMapList(new ADSUserInfoRepository(dataAccessTransaction).GetByEndpointSystemIdIncludeEndPointSystem(endpointSystemid));
                        }

            return presentationADSUsersReportDataList;
        }
    }
}

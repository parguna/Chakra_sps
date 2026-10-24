using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSGroupWithNoAccounts
    {

        public long GroupId { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string member { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public Nullable<System.DateTime> whenCreated { get; set; }

        public Nullable<System.DateTime> whenChanged { get; set; }

        public Nullable<long> uSNCreated { get; set; }

        public Nullable<long> uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string objectSid { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string groupType { get; set; }

        public string groupTypeDesc { get; set; }

        public string objectCategory { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public bool? CanRequest { get; set; }
        public static PresentationADSGroupWithNoAccounts ORMap(EntityModel.Presentation.PresentationADSGroupWithNoAccounts presentationADSGroupWithNoAccountsEntityModel)
        {
            if (presentationADSGroupWithNoAccountsEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSGroupWithNoAccounts, PresentationADSGroupWithNoAccounts>(presentationADSGroupWithNoAccountsEntityModel);
        }

        public static List<PresentationADSGroupWithNoAccounts> ORMapList(IList<EntityModel.Presentation.PresentationADSGroupWithNoAccounts> presentationADSGroupWithNoAccountsEntityModels)
        {
            return presentationADSGroupWithNoAccountsEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSGroupWithNoAccounts> GetByEndpointIdIncludeGroupWithNoAccounts(long endpointSystemid)
        {
            
            IList<PresentationADSGroupWithNoAccounts> presentationADSGroupWithNoAccountsReportDataList = new List<PresentationADSGroupWithNoAccounts>();
            PresentationADSGroupWithNoAccounts PresentationADSGroupWithNoAccounts;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<ADSGroupInfo> adsGroupInfoList = ADSGroupInfo.GetByEndpointIdIncludeGroupWithNoAccounts(endpointSystemid);
                foreach (ADSGroupInfo group in adsGroupInfoList)
                {
                    PresentationADSGroupWithNoAccounts = new PresentationADSGroupWithNoAccounts();
                    PresentationADSGroupWithNoAccounts.GroupId = group.Id;
                    PresentationADSGroupWithNoAccounts.sAMAccountName = group.sAMAccountName;
                    PresentationADSGroupWithNoAccounts.whenCreated = group.whenCreated.HasValue ? group.whenCreated : null;
                    PresentationADSGroupWithNoAccounts.cn = group.cn;
                    presentationADSGroupWithNoAccountsReportDataList.Add(PresentationADSGroupWithNoAccounts);
                }
            }
            return presentationADSGroupWithNoAccountsReportDataList;
        }
    }
}

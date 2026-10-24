using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DI.Chakra.DomainModel
{
    [DataContract]
    [Serializable]
    public class AdHocO365Request : IEntitlementDetail
    {
        [DataMember]
        public string DisplayAccountName { get; set; }

        [DataMember]
        public long EndpointUserId { get; set; }

        [DataMember]
        public ExternalEmailActionType ExternalEmailAction { get; set; }
        [DataMember]
        public LicenseActionType LicenseAction { get; set; }
        [DataMember]
        public List<AdHocLicenseInfo> LicenseInfo { get; set; }
        [DataMember]
        public MailBoxConversionType MailBoxConversionType { get; set; }
        [DataMember]
        public SharedMailBoxActionType SharedMailBoxAction { get; set; }
        [DataMember]
        public Dictionary<string, string> Members { get; set; }
        [DataMember]
        public AdHocO365SharedAccount AdHocO365SharedAccount { get; set; }
        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocO365SharedAccount;
            }
        } 
        public string Entitlement { get; set; }
        public string RequesterCostCenter { get; set; }
        public string EntityId { get; set; }
    }



    [DataContract]
    [Serializable]
    public class AdHocLicenseInfo
    {
        [DataMember]
        public string AccountSkuId { get; set; }
        [DataMember]
        public string LicenseDescription { get; set; }
        [DataMember]
        public string ServiceName { get; set; }
        [DataMember]
        public string ServiceDescription { get; set; }
        [DataMember]
        public bool IsRemoveLicense { get; set; }
    }

}

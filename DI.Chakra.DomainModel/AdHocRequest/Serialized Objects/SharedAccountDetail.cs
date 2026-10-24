using DI.Chakra.Infrastructure.Enums;
using System;
using System.Runtime.Serialization;
 
namespace DI.Chakra.DomainModel
{
    public class AdHocADSharedAccount : ISharedAccountDetail
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string OU { get; set; }
    }


    public class AdHocSAPSharedAccount : ISharedAccountDetail, ISAPUserType
    {
        public SAPUserType SAPUserType { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
    }


    [Serializable]
    public class AdHocSAPEPSharedAccount : ISharedAccountDetail
    {
        public string AccountName { get; set; }
        public string Description { get; set; }
    }


    public class AdHocGEMSSharedAccount : ISharedAccountDetail
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
    }

    [DataContract]
    [Serializable]
    public class AdHocO365SharedAccount : ISharedAccountDetail
    {
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string AccountName { get; set; }
    }

    public class AdHocINHSharedAccount : ISharedAccountDetail
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
    }


}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Runtime.Serialization;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocADSharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string OU { get; set; }
    }

    [Serializable]
    public class AdHocSAPSharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        public SAPUserType SAPUserType { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }

        public string DisplayName { get { return Description; } set { Description = value; } }

    }


    [Serializable]
    public class AdHocSAPEPSharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        public string AccountName { get; set; }
        public string Description { get; set; }


        public string DisplayName { get { return Description; }  set { Description = value; } }

    }

    [Serializable]
    public class AdHocGEMSSharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
    }

    [DataContract]
    [Serializable]
    public class AdHocO365SharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string AccountName { get; set; }
    }
    [Serializable]
    public class AdHocINHSharedAccount : ISharedAccountDetail, ISharedAccountDisplay
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Table("DI_ADS_UserInfoHistory")]
    public class ADSUserInfoHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string cn { get; set; }
        public string sn { get; set; }
        public string ou { get; set; }
        public int PwdResetCnt { get; set; }
        public string st { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string postalAddress { get; set; }
        public string postalCode { get; set; }
        public string postOfficeBox { get; set; }
        public string physicalDeliveryOfficeName { get; set; }
        public string telephoneNumber { get; set; }
        public string facsimileTelephoneNumber { get; set; }
        public string givenName { get; set; }
        public string initials { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public DateTime? whenCreated { get; set; }
        public DateTime? whenChanged { get; set; }
        public string displayName { get; set; }
        public Int64 uSNCreated { get; set; }
        public string info { get; set; }
        public Int64 uSNChanged { get; set; }
        public string co { get; set; }
        public string department { get; set; }
        public string company { get; set; }
        public string manager { get; set; }
        public string streetAddress { get; set; }
        public string wWWHomePage { get; set; }
        public string name { get; set; }
        public string objectGUID { get; set; }
        public string userAccountControl { get; set; }
        //AccountOptions

        public bool IsAccountDisabled { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPasswordRequired { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }

        public string badPwdCount { get; set; }
        public string codePage { get; set; }
        public string countryCode { get; set; }
        public DateTime? badPasswordTime { get; set; }
        public DateTime? lastLogoff { get; set; }
        public DateTime? lastLogon { get; set; }
        public DateTime? pwdLastSet { get; set; }
        public string primaryGroupID { get; set; }
        public string objectSid { get; set; }
        public DateTime? accountExpires { get; set; }
        public string logonCount { get; set; }
        public string sAMAccountName { get; set; }
        public string sAMAccountType { get; set; }
        public string managedObjects { get; set; }
        public string ipPhone { get; set; }
        public string objectCategory { get; set; }
        public string mail { get; set; }
        public string homePhone { get; set; }
        public string mobile { get; set; }
        public string pager { get; set; }
        public string nTSecurityDescriptor { get; set; }
        public string MappingType { get; set; }
        public int? DIUserId { get; set; }
        public Nullable<int> ExUserId { get; set; }
       
        public DateTime? IACMLastPasswordChange { get; set; }
        public string NewUsrPassword;
        public string profilePath;
        public string IfUnmappedMapTo { get; set; }
        public int Version { get; set; }

        public string businessCategory { get; set; }
        public string division { get; set; }
        public string homeDrive { get; set; }
        public string homeDirectory { get; set; }

        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }

        public bool IsHomeFolderPermissionSetFailed { get; set; }

        public string AuditUser { get; set; }
        public DateTime AuditDate { get; set; }
        [ForeignKey("UserId")]
        public virtual ADSUserInfo ADSUserInfo { get; set; }

    }

}

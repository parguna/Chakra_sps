using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_ADS_UserInfo")]
    public class ADSUserInfo : IEndpointUserInfo, IAggregateRoot
    {
        public ADSUserInfo()
        {
            this.ADSGroups = new List<ADSGroupInfo>();
            this.Groups = new List<string>();
        }

        [Key]
        public long Id { get; set; }
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
        public bool IsAccountLocked { get; set; }
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
        public long? DIUserId { get; set; }
        public Nullable<int> ExUserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
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
        public bool IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
        
        public bool IsHomeFolderPermissionSetFailed { get; set; }

        public List<string> Groups;

        public virtual EndPoint EndPointSystem { get; set; }
        public List<ADSGroupInfo> ADSGroups { get; set; }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();

                foreach (var item in ADSGroups)
                    li.Add(item);

                return li;
            }
        }


        public override string ToString()
        {
            return this.name;
        }
        public static ADSUserInfo CreateADSUserInfo()
        {
            ADSUserInfo adsUserInfo = new ADSUserInfo();

            return adsUserInfo;
        }


        /// <summary>
        /// Checks if the given instance is equals to the current one.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if they equals, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ADSUserInfo)
            {
                ADSUserInfo other = (ADSUserInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>
        /// <returns>The category hash code.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.name.GetHashCode();
        }

        public string AccountName
        {
            get
            {
                return this.sAMAccountName;
            }
        }

        public string AccountPassword
        {
            get { return this.NewUsrPassword; }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }
    }

}

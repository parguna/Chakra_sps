using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_ADS_UserInfo")]
    public class ADSUserInfo : EntityBase,ICreatable,IUpdatable
    {

        public ADSUserInfo()
        {
            this.ADSGroups = new List<ADSGroupInfo>();
        }

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

        public Nullable<int> ExUserId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }

        public string NewUsrPassword;

        public string profilePath;

        public string businessCategory { get; set; }

        public string division { get; set; }

        public string homeDrive { get; set; }

        public string homeDirectory { get; set; }

        public bool IsHomeFolderPermissionSetFailed { get; set; }

        public string otherPager { get; set; }
        public string otherFascimileTelephoneNumber { get; set; }     

        public string extensionAttribute1 { get; set; }
        public string extensionAttribute2 { get; set; }
        public string extensionAttribute3 { get; set; }
        public string extensionAttribute4 { get; set; }
        public string extensionAttribute5 { get; set; }
        public bool NeedO365License { get; set; }
        public string extensionAttribute6 { get; set; }
        public string extensionAttribute7 { get; set; }
        public string extensionAttribute8 { get; set; }
        public string extensionAttribute9 { get; set; }
        public string extensionAttribute10 { get; set; }
        public string extensionAttribute11 { get; set; }
        public string extensionAttribute12 { get; set; }
        public string extensionAttribute13 { get; set; }

        public List<string> Groups;

        public EndPoint EndPointSystem { get; set; }

        public List<ADSGroupInfo> ADSGroups { get; set; }

        public Nullable<long> WogUserId { get; set; }

    }
}

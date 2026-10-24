using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_SAPEP_UserInfo", "DI_SAPEP_UserInfo")]
    [Table("DI_SAPEP_UserInfo")]
    public class SAPEPUserInfo : IEndpointUserInfo
    {

        public SAPEPUserInfo()
        {
            this.Roles = new List<string>();
            this.Groups = new List<string>();

            this.SAPEPGroups = new List<SAPEPGroupInfo>();
            this.SAPEPRoles = new List<SAPEPRoleInfo>();
        }

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string UserName { get; set; }
        public string UniqueName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Cellphone { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> UserLastLogOnTime { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> IsCompanyUser { get; set; }
        public string UniqueId { get; set; }
        //public string MappingType { get; set; }
        public Nullable<long> DIUserId { get; set; }
        public int PwdResetCnt { get; set; } // Nullable<int> 
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public DateTime? IACMLastPasswordChange { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string MappingType { get; set; }
        public string IfUnmappedMapTo { get; set; }
        public int Version { get; set; }

        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }
        public bool IsLocked { get; set; }
        
        public List<string> Groups;
        public List<string> Roles;    
 
        public virtual EndPoint EndPointSystem { get; set; }

        public virtual IList<SAPEPRoleInfo> SAPEPRoles { get; set; } // From Old // List<SAPEPRoleInfo> Roles;
        public virtual IList<SAPEPGroupInfo> SAPEPGroups { get; set; } // List<SAPEPGroupInfo> Groups;

        public override string ToString()
        {
            return this.UserName;
        }
        public static SAPEPUserInfo CreateSAPEPUserInfo()
        {
            SAPEPUserInfo sapEPUserInfo = new SAPEPUserInfo();

            return sapEPUserInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPUserInfo other = obj as SAPEPUserInfo;
            if (obj != null)
            {
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.UserName.GetHashCode();
        }


        public string AccountName
        {
            get { return this.UserName; }
        }

        public string AccountPassword
        {
            get
            {
                return this.Password;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                return (List<IEndpointEntitlementInfo>)SAPEPRoles;
            }
        }
    }
}

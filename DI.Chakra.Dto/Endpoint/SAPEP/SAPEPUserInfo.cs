using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SAPEPUserInfo : DtoBase, IEndpointUserInfo
    {
        

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

        public int PwdResetCnt { get; set; } 
 
        public string Extra1 { get; set; }

        public string Extra2 { get; set; }

        public string Extra3 { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public List<string> Groups;

        public List<string> Roles;

        public EndPoint EndPointSystem { get; set; }

        public  List<SAPEPRoleInfo> SAPEPRoles { get; set; } 

        public  List<SAPEPGroupInfo> SAPEPGroups { get; set; }

        public string AccountName { get; set; }

        public string AccountPassword { get; set; }

        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
    }
}

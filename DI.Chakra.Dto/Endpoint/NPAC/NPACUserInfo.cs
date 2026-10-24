using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    public class NPACUserInfo : DtoBase, IEndpointUserInfo
    {
        public long EndPointSystemId { get; set; }
        public string Emp_Id { get; set; }
        public string UserIdentity { get; set; }
        public string UserName { get; set; }
        public string Passwds { get; set; }
        public string Designation { get; set; }
        public string Phone { get; set; }
        public string Pager { get; set; }
        public string Fax { get; set; }
        public string Groups { get; set; }
        public Nullable<int> Levels { get; set; }
        public string Section { get; set; }
        public string Grade { get; set; }
        public string Zone { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Invalid_Date { get; set; }
        public string Encrypt_Pwd { get; set; }
        public DateTime? Valid_Date { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string DisplayName { get; set; }
        public string UserADID { get; set; }
        public string EmailId { get; set; }
        public string CompanyName { get; set; }
        public string SectionName { get; set; }

    }
}

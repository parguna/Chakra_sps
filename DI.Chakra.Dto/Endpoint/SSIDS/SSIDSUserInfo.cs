using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    public class SSIDSUserInfo : DtoBase, IEndpointUserInfo
    {
        public SSIDSUserInfo()
        {
            this.SSIDSRolelnfos = new List<SSIDSRolelnfo>();
        }
        public long EndPointSystemId { get; set; }
        public string User_Id { get; set; }
        public string UserName { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string Division { get; set; }
        public string OfficeTelePhone { get; set; }
        public string HandPhone { get; set; }
        public string Fax { get; set; }
        public string ImageLink { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Encrypt_Pwd { get; set; }
        public DateTime? ValidDate { get; set; }
        public DateTime? InValidDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public IList<SSIDSRolelnfo> SSIDSRolelnfos { get; set; }
        public string AccountName
        {
            get
            {
                return this.User_Id;
            }
        }

        public string AccountPassword
        {
            get { return this.Encrypt_Pwd; }
        }

        public string DisplayName
        {
            get
            {
                return this.UserName;
            }
        }

        public List<string> Roles;
        public string UserValidFrom;
        public string UserValidTo;
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SSIDSRolelnfos != null)
                {
                    foreach (var item in SSIDSRolelnfos)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

    }
}

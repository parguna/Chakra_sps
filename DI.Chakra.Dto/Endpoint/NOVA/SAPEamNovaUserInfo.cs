using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPEamNovaUserInfo : DtoBase, IEndpointUserInfo
    {
        public SAPEamNovaUserInfo()
        {
            this.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();
        }
        public long EndPointSystemId { get; set; }
        public long UserId { get; set; }
        public string FLINS_UserLogonName { get; set; }
        public long FLINS_SAPUserInfoId { get; set; }
        public string FLINS_BI_UserLogonName { get; set; }
        public long FLINS_BI_SAPUserInfoId { get; set; }
        public string AD_UserLogonName { get; set; }
        public long ADS_UserInfoId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<SAPEamNovaUserRoleInfo> SAPEamNovaUserRoles { get; set; }

        public string DisplayName
        {
            get
            {
                return $"FLINS Account:{FLINS_UserLogonName}, FLINS BI Account:{FLINS_BI_UserLogonName}, AD Account:{AD_UserLogonName}";
            }
        }

        public string AccountName
        {
            get
            {
                return $"FLINS Account:{FLINS_UserLogonName}, FLINS BI Account:{FLINS_BI_UserLogonName}, AD Account:{AD_UserLogonName}";
            }
        }
        public string AccountPassword { get; }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SAPEamNovaUserRoles != null)
                {
                    foreach (var item in SAPEamNovaUserRoles)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }
    }
}

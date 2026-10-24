using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SMSUserInfo : DtoBase, IEndpointUserInfo
    {
        public SMSUserInfo()
        {
            this.SMSAccessLevels = new List<SMSAccessLevel>();
            this.SMSBadgeInfos = new List<SMSBadgeInfo>();
            this.AccessLevels = new List<string>();
        }
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int UserId { get; set; }
        //first name
        public string FIRSTNAME { get; set; }
        //last name
        public string LASTNAME { get; set; }
        //known as
        public string MIDNAME { get; set; }
        //PERNR
        public string SSNO { get; set; }

        //department/branch
        public string Addr1 { get; set; }
        //section
        public string City { get; set; }
        //nric
        public string Zip { get; set; }
        //AD ID
        public string OPHONE { get; set; }
        //email id
        public string EMAIL { get; set; }

        //lenel company id
        public int DEPT { get; set; }

        #endregion

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public List<SMSAccessLevel> SMSAccessLevels { get; set; }
        public List<SMSBadgeInfo> SMSBadgeInfos { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string DisplayName { get; set; }
        public string NewPernr { get; set; }
        public string Password { get; set; }
        public string UserValidFrom { get; set; }
        public string UserValidTo { get; set; }
        public List<string> AccessLevels { get; set; }
    }
}

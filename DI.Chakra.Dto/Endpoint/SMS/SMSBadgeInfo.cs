using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SMSBadgeInfo : DtoBase, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int BadgeKey { get; set; }
        public int BadgeNumber { get; set; }
        //card holder id
        public long UserId { get; set; }
        public int STATUS { get; set; }
        public int BadgeType { get; set; }
        public int DEST_EXEMPT { get; set; }
        public string ACTIVATE { get; set; }
        public string DEACTIVATE { get; set; }
        #endregion
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }      
        public EndPoint EndPoint { get; set; }
        public string EntitlementName { get; set; }
        public string EntitlementDesc { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
}

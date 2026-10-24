using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SMS_BadgeInfo")]
    public class SMSBadgeInfo : EntityBase, ICreatable, IUpdatable
    {

        public SMSBadgeInfo()
        {

        }
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int BadgeKey { get; set; }
        public long BadgeNumber { get; set; }
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

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}

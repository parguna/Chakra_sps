using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SMS_AccessLevel")]
    public class SMSAccessLevel : EntityBase, ICreatable, IUpdatable
    {
        public SMSAccessLevel()
        {
            this.SMSUserInfo = new List<SMSUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public int AccessLevelId { get; set; }
        public string Name { get; set; }
        public long SegmentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<SMSUserInfo> SMSUserInfo { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}

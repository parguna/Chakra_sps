using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_VPN_UserInfo")]
    public class VPNUserInfo : EntityBase, ICreatable, IUpdatable
    {
        public VPNUserInfo()
        {
        }
        public string OneKeyTokenId { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPointSystem { get; set; }
    }
}

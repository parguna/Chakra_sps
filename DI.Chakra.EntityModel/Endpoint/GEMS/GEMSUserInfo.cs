using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_GEMS_UserInfo")]
    public class GEMSUserInfo : EntityBase, ICreatable, IUpdatable
    {
        public GEMSUserInfo()
        {
            this.GEMSRoles = new List<GEMSRoleInfo>();
        }

        public int UserId { get; set; }
        public long EndPointSystemId { get; set; }     
        public string USER_NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string EMAIL { get; set; }
        public Nullable<DateTime> DATEMODIFIED { get; set; }
        public string LASTUSER { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsDummyEntry { get; set; }
        public List<GEMSRoleInfo> GEMSRoles { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}

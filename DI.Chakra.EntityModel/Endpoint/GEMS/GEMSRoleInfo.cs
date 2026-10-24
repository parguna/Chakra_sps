using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_GEMS_RoleInfo")]
    public class GEMSRoleInfo : EntityBase, ICreatable, IUpdatable
    {
        public GEMSRoleInfo()
        {
            this.GEMSUserInfos = new List<GEMSUserInfo>();
        }
        public int RoleId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Display_Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Extension_ID { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }   
        public IList<GEMSUserInfo> GEMSUserInfos { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}

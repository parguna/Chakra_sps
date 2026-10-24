using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SSIDS_RoleInfo")]
    public class SSIDSRolelnfo : EntityBase, ICreatable, IUpdatable
    {
        public SSIDSRolelnfo()
        {
            this.SSIDSUserInfos = new List<SSIDSUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<SSIDSUserInfo> SSIDSUserInfos { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}

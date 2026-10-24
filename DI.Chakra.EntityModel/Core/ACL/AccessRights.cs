using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Serializable]
    [Table("DI_M_AccessRights")] 
    public class AccessRights : EntityBase, ICreatable, IUpdatable
    {
        public long RoleId { get; set; }
        public long? PagePermissionId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PagePermissionId")]
        public virtual PagePermission PagePermission { get; set; }
    }
}

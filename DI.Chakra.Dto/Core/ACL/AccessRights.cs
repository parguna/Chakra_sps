using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AccessRights : DtoBase
    {
        public long RoleId { get; set; }
        public long? PagePermissionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual Role Role { get; set; }

        public virtual PagePermission PagePermission { get; set; }
    }
}

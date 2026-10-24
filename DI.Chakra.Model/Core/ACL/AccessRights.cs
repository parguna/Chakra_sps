using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_AccessRights")] 
    public class AccessRights : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long? PagePermissionId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PagePermissionId")]
        public virtual PagePermission PagePermission { get; set; }
    }
}

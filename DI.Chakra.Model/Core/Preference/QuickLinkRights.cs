using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_QuickLinkAccessRights")]
    [Serializable]
    public class QuickLinkAccessRights : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long QuickLinkId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("QuickLinkId")]
        public virtual QuickLink QuickLink { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}

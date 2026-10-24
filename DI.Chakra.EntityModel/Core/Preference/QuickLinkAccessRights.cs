using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_QuickLinkAccessRights")]
    public class QuickLinkAccessRights:EntityBase,ICreatable,IUpdatable
    {
       
        public long RoleId { get; set; }

        public long QuickLinkId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("QuickLinkId")]
        public  QuickLink QuickLink { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

    }
}

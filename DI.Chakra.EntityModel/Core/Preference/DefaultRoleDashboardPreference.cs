using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
      [Table("DI_M_DefaultRoleDashboard")]
   public  class DefaultRoleDashboardPreference:EntityBase,ICreatable,IUpdatable
    {
        
        public long RoleId { get; set; }

        public long DashBoardId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("DashBoardId")]
        public  Dashboard Dashboard { get; set; }

        [ForeignKey("RoleId")]
        public  Role Role { get; set; }
    }
}

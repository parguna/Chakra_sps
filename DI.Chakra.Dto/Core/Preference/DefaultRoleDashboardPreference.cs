using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
   public  class DefaultRoleDashboardPreference:DtoBase
    {
        
        public long RoleId { get; set; }

        public long DashBoardId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  Dashboard Dashboard { get; set; }

        public  Role Role { get; set; }
    }
}

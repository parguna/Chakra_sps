using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
  public  class UserDashboardPreference:DtoBase

    {
     
        public long UserId { get; set; }

        public string Name { get; set; }

        public long DashboardId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

       
        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public  User User { get; set; }

        public  Dashboard Dashboard { get; set; }
    }
}

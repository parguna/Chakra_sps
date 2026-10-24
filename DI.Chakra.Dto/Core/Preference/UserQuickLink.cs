using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
   public  class UserQuickLink:DtoBase
    {

        public long UserId { get; set; }

        public string Name { get; set; }

        public long QuickLinkId { get; set; }

        public int Position { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public  User User { get; set; }

        public  QuickLink QuickLink { get; set; }
    }
}

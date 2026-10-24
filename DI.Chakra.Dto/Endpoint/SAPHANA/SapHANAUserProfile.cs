using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SapHANAUserProfile : DtoBase
    {
        public string UserName { get; set; }

        public string ProfileName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public Nullable<System.DateTime> ModifiedDate { get; set; }

        

        public int? Version { get; set; }
    }
}

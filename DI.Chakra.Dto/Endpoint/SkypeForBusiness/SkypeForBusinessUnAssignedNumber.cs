using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SkypeForBusinessUnAssignedNumber:DtoBase
    {
        public int StartRange { get; set; }
        public int EndRange { get; set; }
        public int Priority { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
    }
}

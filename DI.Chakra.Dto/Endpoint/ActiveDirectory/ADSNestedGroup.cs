using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class ADSNestedGroup : DtoBase
    {

        public long EndPointSystemId { get; set; }
        public string Supergroup { get; set; }
        public string Subgroup { get; set; }
        public int Depth { get; set; }
        public EndPoint EndPointSystem { get; set; }
    }
        
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_ADS_NestedGroup")]
    public class ADSNestedGroup : EntityBase
    {
        public long EndPointSystemId { get; set; }
        public string Supergroup { get; set; }
        public string Subgroup { get; set; }
        public int Depth { get; set; }
        public EndPoint EndPointSystem { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.EntityModel
{
  public class Attention : EntityBase
    {
        public string  Name { get; set; }
        public string CommandName { get; set; }
        public long Count { get; set; }
    }
}

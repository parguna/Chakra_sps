using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class AccessRequestSourceByMonth : DtoBase
    {
        public AccessRequestSource Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }
    }
}

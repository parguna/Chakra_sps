using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class AccessRequestProgressStatusByMonth : EntityBase
    {
        public AccessRequestProgress Name { get; set; }
        public decimal Y { get; set; }
        public AccessRequestProgress LabelName { get; set; }

    }
}

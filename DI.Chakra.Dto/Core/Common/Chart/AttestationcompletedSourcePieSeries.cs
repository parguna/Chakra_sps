using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto
{
   public class AttestationcompletedSourcePieSeries : DtoBase
    {
        public StatusType Name { get; set; }
        public decimal Y { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
   public class AccessRequestSourceChartData : DtoBase
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
    }
}

using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class AccessRequestSourceChartData : EntityBase
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }

    }
}

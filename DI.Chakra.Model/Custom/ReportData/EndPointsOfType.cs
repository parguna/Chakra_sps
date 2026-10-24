using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class EndPointsOfType
    {
        public long endPointId { get; set; }
        public string endPointTypeName { get; set; }
        public string endPointDomainName { get; set; }
        public string endPointHostName { get; set; }
        public string endPointIPAddress { get; set; }
    }
}

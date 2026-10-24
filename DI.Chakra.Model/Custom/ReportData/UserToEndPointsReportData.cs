using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserToEndPointsReportData
    {
        public string userName { get; set; }

        public string EndpointSystemName { get; set; }
        public string EndpointHostName { get; set; }

        public string EndpointUserName { get; set; }
        public string OrganisationName { get; set; }
    }
}

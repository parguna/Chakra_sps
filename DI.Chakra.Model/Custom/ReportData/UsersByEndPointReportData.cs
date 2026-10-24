using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class UsersByEndPointReportData
    {
        public long userId { get; set; }
        public string userDisplayName { get; set; }
        public string userMailId { get; set; }
        public string userEmployeeId { get; set; }
        public string userUserName { get; set; }
        public string usrFirstName { get; set; }
        public string usrLastName { get; set; }

        public string endPointName { get; set; }
    }
}

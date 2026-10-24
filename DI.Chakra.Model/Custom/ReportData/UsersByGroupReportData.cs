using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class UsersByGroupReportData
    {
        public long userId { get; set; }
        public string userDisplayName { get; set; }
        public string userMailId { get; set; }
        public string userEmployeeId { get; set; }
        public string grpName { get; set; }
    }
}

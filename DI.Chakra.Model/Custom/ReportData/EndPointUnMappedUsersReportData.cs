using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class EndPointUnMappedUsersReportData
    {
        public int endPointUserId { get; set; }
        public string endPointUserName { get; set; }
        public string endPointDisplayName { get; set; }
        public DateTime? endPointLastLogon { get; set; }
        public string endPointAccountStatus { get; set; }
        public string endPointUnMapReason { get; set; }
    }
}

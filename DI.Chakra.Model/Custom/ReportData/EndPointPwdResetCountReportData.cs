using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class EndPointPwdResetCountReportData
    {
        public long endPointUserId { get; set; }
        public string endPointUserName { get; set; }
        public string  endPointType { get; set; }
        public int endPointPwdResetCount { get; set; }
    }
}

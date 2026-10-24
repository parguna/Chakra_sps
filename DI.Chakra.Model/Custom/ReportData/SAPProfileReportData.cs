using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class SAPProfileReportData
    {
        public int SAP_PROFILEID { get; set; }
        public string PROFILE_CODE { get; set; }
        public string PROFILE_DESC { get; set; }
        public string Institution { get; set; }
        public string HostName { get; set; }
    }
}

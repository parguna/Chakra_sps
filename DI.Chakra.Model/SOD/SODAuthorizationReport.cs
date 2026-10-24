using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class SODAuthorizationReport
    {
        public int SODAuthorizationReportID { get; set; }
        public long SODReportId { get; set; }
        public string AuthorizationCode{ get; set; }
        public int LeftOrRight { get; set; }

    }
}

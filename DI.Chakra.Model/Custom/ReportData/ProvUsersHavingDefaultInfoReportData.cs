using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class ProvUsersHavingDefaultInfoReportData
    {
        public int userId { get; set; }
        public string displayName { get; set; }
        public int organisationId { get; set; }
        public string organisationName { get; set; }
        public string defaultADGroup { get; set; }
        public string defaultLNGroup { get; set; }
        public string defaultLNOU { get; set; }
        public string SAPPrinterLongName { get; set; }
    }
}

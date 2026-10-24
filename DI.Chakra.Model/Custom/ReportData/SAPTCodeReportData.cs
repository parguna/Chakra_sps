using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class SAPTCodeReportData
    {
        public int TCODE_Id { get; set; }
        public string TCODE_Code { get; set; }
        public string TCODE_Desc { get; set; }
        public string Institution { get; set; }
        public string HostName { get; set; }
    }
}

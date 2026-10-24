using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class ADSComputerObjectsReportData
    {
        public long objectID { get; set; }
        public string objectInstitutionName { get; set; }
        public string objectName { get; set; }
        public DateTime objectLastLogOn { get; set; }
        public DateTime objectLastLogoff { get; set; }
        public DateTime objectWhenCreated { get; set; }
        public string objectHostName { get; set; }
        public string objectLocation { get; set; }
    }
}

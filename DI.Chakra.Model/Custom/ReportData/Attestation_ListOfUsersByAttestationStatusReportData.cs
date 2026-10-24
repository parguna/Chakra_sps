using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class Attestation_ListOfUsersByAttestationStatusReportData
    {
        public long userId { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public string endPointUserName { get; set; }
        public string employeeId { get; set; }
        public string organizationId { get; set; }
        public string endPointSystemName { get; set; }
        public string endPointHostName { get; set; }
        public string attestationStatus { get; set; }


        public long endPointTypeId { get; set; }
        public long endPointSystemId { get; set; }

        public long WorkflowConfigurationId { get; set; }
        public byte Level { get; set; }
        public string ActionUser { get; set; }
    }
}

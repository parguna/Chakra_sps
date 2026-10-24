using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor.AttestationReport
{
    public class AttestationReportService
    {
        public AttestationReportProcess attestationReportProcess { get; private set; }
        public AttestationReportService()
        {
            attestationReportProcess = new AttestationReportProcess();
        }

        public void Start()
        {
            attestationReportProcess.Start();
        }

        public void Stop()
        {
            attestationReportProcess.Stop();
        }
    }
}

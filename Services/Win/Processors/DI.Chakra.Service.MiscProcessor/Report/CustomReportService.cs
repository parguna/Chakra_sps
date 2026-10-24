using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class CustomReportService
    {
        public CustomReportProcess customReportProcess { get; private set; }
        public CustomReportService()
        {
            customReportProcess = new CustomReportProcess();
        }

        public void Start()
        {
            customReportProcess.Start();
        }

        public void Stop()
        {
            customReportProcess.Stop();
        }
    }
}

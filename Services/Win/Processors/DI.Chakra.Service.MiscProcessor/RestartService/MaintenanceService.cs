using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class MaintenanceService
    {
        public MaintenanceProcess maintenanceProcess { get; private set; }
        public MaintenanceService()
        {
            maintenanceProcess = new MaintenanceProcess();
        }

        public void Start()
        {
            maintenanceProcess.Start();
        }

        public void Stop()
        {
            maintenanceProcess.Stop();
        }
    }
}

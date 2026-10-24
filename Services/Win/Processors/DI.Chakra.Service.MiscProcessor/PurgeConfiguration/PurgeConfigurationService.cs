using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
   public class PurgeConfigurationService
    {
        public PurgeConfigurationProcess purgeConfigurationProcess { get; private set; }
        public PurgeConfigurationService()
        {
            purgeConfigurationProcess = new PurgeConfigurationProcess();
        }

        public void Start()
        {
            purgeConfigurationProcess.Start();
        }

        public void Stop()
        {
            purgeConfigurationProcess.Stop();
        }
    }
}

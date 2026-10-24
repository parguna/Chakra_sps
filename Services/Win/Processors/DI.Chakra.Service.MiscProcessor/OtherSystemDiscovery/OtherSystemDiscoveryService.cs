using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class OtherSystemDiscoveryService
    {
        public OtherSystemDiscoveryProcess otherSystemDiscoveryProcess { get; private set; }
        public OtherSystemDiscoveryService()
        {
            otherSystemDiscoveryProcess = new OtherSystemDiscoveryProcess();
        }

        public void Start()
        {

            otherSystemDiscoveryProcess.Start();
        }

        public void Stop()
        {
            otherSystemDiscoveryProcess.Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{

    public class DecommissionService
    {
        public DecommissionProcess decommissionProcess { get; private set; }
        public DecommissionService()
        {
            decommissionProcess = new DecommissionProcess();
        }

        public void Start()
        {
            decommissionProcess.Start();
        }

        public void Stop()
        {
            decommissionProcess.Stop();
        }
    }
}

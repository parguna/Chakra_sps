using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class UpdateADManagerService
    {
        public UpdateADManagerProcess updateADManagerProcess { get; private set; }
        public UpdateADManagerService()
        {
            updateADManagerProcess = new UpdateADManagerProcess();
        }

        public void Start()
        {
            updateADManagerProcess.Start();
        }

        public void Stop()
        {
            updateADManagerProcess.Stop();
        }
    }
}

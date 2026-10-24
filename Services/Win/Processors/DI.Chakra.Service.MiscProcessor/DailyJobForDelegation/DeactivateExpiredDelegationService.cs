using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor.DailyJobForDelegation
{
    public class DeactivateExpiredDelegationService
    {
        public DeactivateExpiredDelegationProcess deactivateExpiredDelegationProcess { get; private set; }
        public DeactivateExpiredDelegationService()
        {
            deactivateExpiredDelegationProcess = new DeactivateExpiredDelegationProcess();
        }

        public void Start()
        {
            deactivateExpiredDelegationProcess.Start();
        }

        public void Stop()
        {
            deactivateExpiredDelegationProcess.Stop();
        }
    }
}

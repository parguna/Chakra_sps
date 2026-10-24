using System;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;

namespace DI.Chakra.Services.DiscoveryProcessor
{
    public class DiscoveryEngine
    {
        public void StartProcess(DiscoveryServiceInstance discServiceInstaceType)
        {
            var currentJobs = Process.GetCurrentJobs(ProcessType.Discovery);
            if (currentJobs != null && currentJobs.Count > 0)
            {
                currentJobs = currentJobs.
                    Where(item => item.DiscoveryInstance == (int)discServiceInstaceType).ToList();

                string processId = currentJobs[0].Id.ToString();
                DILog.DiscoveryLogger.WriteLog((int)currentJobs[0].Id, LogLevelType.Info, "Start Process id " + processId);
            }

            BackgroundWorkerHelper bg;
            foreach (Process job in currentJobs)
            {
                bg = new BackgroundWorkerHelper();
                bg.StartProcessThread(job, "Discovery");
            }
        }
    }
}

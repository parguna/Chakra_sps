using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class AttestationEngine
    {
        public void StartProcess()
        {
            var currentJobs = Process.GetCurrentJobs(ProcessType.Attestation);
            BackgroundWorkerHelper bg;

            if (currentJobs != null && currentJobs.Count > 0)
            {
                foreach (Process job in currentJobs)
                {
                    bg = new BackgroundWorkerHelper();
                    bg.StartProcessThread(job , "Attestation");
                }
            }
            

        }
    }
}

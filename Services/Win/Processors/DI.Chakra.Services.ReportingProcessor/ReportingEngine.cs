using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Services.ReportingProcessor
{
    public class ReportingEngine
    {
        public void StartProcess()
        {
            var currentJobs = Process.GetCurrentJobs(ProcessType.Reporting);
            if (currentJobs.Count > 0)
            {
                string processId = currentJobs[0].Id.ToString();
                DILog.ReportingLogger.WriteLog((int)currentJobs[0].Id, LogLevelType.Info, "Start Process id " + processId);
            }
            BackgroundWorkerHelper bg;
            foreach (Process job in currentJobs)
            {
                bg = new BackgroundWorkerHelper();
                bg.StartProcessThread(job, "Reporting");
            }

            var currentAdhocJobs = Process.GetCurrentJobs(ProcessType.JobReport);
            if (currentAdhocJobs.Count > 0)
            {
                string adHocProcessId = currentAdhocJobs[0].Id.ToString();
                DILog.ReportingLogger.WriteLog((int)currentAdhocJobs[0].Id, LogLevelType.Info, "Start Process id " + adHocProcessId);
            }
            foreach (Process job in currentAdhocJobs)
            {
                bg = new BackgroundWorkerHelper();
                bg.StartProcessThread(job, "JobReport");
            }
        }
    }
}

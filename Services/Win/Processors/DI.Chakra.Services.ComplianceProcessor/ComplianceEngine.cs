//using DI.Chakra.Facade;
using DI.Chakra.Logger;
//using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.ComplianceProcessor
{
    public class ComplianceEngine
    {
        public void StartProcess()
        {
            //SchedulerFacade schedulerFacade = new SchedulerFacade();

            var currentJobs = Process.GetCurrentJobs(ProcessType.Compliance);
            if (currentJobs.Count > 0)
            {
                string processId = currentJobs[0].Id.ToString();
                DILog.ComplianceLogger.WriteLog(LogLevelType.Info, "Compliance engine Start Process id _" + processId);
            }
            BackgroundWorkerHelper bg;

            foreach (Process job in currentJobs)
            {
                bg = new BackgroundWorkerHelper();
                bg.StartProcessThread(job, "Compliance");
            }
        }
    }
}

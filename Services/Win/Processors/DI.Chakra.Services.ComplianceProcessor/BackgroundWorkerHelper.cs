using DI.Chakra.Logger;
//using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.ComplianceProcessor
{
    public class BackgroundWorkerHelper
    {
        private const string KEY_SERVICENAME = @"System";
        private delegate void startProcessDelegate(Process process);
        private startProcessDelegate startProcess;
        private BackgroundWorker bgWorker;

        public BackgroundWorkerHelper()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }

        public void StartProcessThread(Process process, string ProcessName)
        {
            switch (ProcessName)
            {
                case "Compliance": startProcess = new startProcessDelegate(ComplianceProcess);
                    break;
                default: startProcess = new startProcessDelegate(DefaultProcess);
                    break;
            }

            bgWorker.RunWorkerAsync(process);
        }
        private void ComplianceProcess(Process process)
        {
            ComplianceService service = new ComplianceService();
            service.Start(process.ProcessTypeKeyId, process);
           
            DILog.ComplianceLogger.WriteLog(process.ProcessTypeKeyId,LogLevelType.Info, string.Format("Completed processing compliance process id: {0}", process.ProcessTypeKeyId.ToString()));
        }
        private void DefaultProcess(Process process)
        {
            //DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Background thread called without process information:", null, "Compliance",Convert.ToInt32( schedulerData.ComplianceProcessId),0,"Scheduler Service");
        }
        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process process = e.Argument as Process;
            startProcess(process);
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}

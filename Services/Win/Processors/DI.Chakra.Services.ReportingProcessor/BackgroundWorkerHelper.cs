using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.ComponentModel;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Services.ReportingProcessor
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
        }


        public void StartProcessThread(Process process, string processName)
        {
            switch (processName)
            {
                case "Reporting":
                    startProcess = new startProcessDelegate(ReportingProcess);
                    break;
                case "JobReport":
                    startProcess = new startProcessDelegate(AdhocReportingProcess);
                    break;
                default:
                    break;
            }
            bgWorker.RunWorkerAsync(process);
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process process = e.Argument as Process;
            startProcess(process);
        }

        private void ReportingProcess(Process process)
        {
            ReportingService reporingService = new ReportingService();
            process.ProcessStartAt = DateTime.Now;
            process.Save(MasterConfigHelper.ServiceUser);
            reporingService.Start(process);
        }

        private void AdhocReportingProcess(Process process)
        {
            ReportingService reporingService = new ReportingService();
            process.ProcessStartAt = DateTime.Now;
            process.Save(MasterConfigHelper.ServiceUser);
            reporingService.StartAdHocReport(process);
        }
    }
}

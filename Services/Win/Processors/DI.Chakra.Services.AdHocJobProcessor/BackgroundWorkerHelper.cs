using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.ComponentModel;
using DI.Chakra.DBConfigHelper;
namespace DI.Chakra.Services.AdHocJobProcessor
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
                case "AdHocJob":
                    startProcess = new startProcessDelegate(AdHocJobProcess);
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

        private void AdHocJobProcess(Process process)
        {
            AdHocJobService adHocJobService = new AdHocJobService();
            process.ProcessStartAt = DateTime.Now;
            process.Save(MasterConfigHelper.ServiceUser);
            adHocJobService.Start(process);
        }
    }
}

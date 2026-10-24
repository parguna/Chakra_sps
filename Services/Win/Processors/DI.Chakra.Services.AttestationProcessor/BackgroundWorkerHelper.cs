using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.ComponentModel;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class BackgroundWorkerHelper
    {
        #region private members
        private delegate void startProcessDelegate(Process process);
        private startProcessDelegate startProcess;
        private BackgroundWorker bgWorker;
        #endregion

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
                case "Attestation":
                    startProcess = new startProcessDelegate(AttestationProcess);
                    break;
                default:
                    startProcess = new startProcessDelegate(DefauldProcess);
                    break;
            }

            bgWorker.RunWorkerAsync(process);
        }

        private void AttestationProcess(Process process)
        {
            //TODO: CHECK HARI
            AttestationService service = new AttestationService();
            service.StartWorkFlow(process);

        }
        
        private void DefauldProcess(Process process)
        {

            DILog.AttestationLogger.WriteLog(LogLevelType.Info, "Background thread called without process information:");
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

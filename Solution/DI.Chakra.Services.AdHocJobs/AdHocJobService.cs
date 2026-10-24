using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DI.Chakra.Logger;
using DI.Chakra.Services.AdHocJobProcessor;

namespace DI.Chakra.Services.AdHocJobs
{
    public partial class AdHocJobService : ServiceBase
    {
        Timer reportingTimer;
        int minuteMultipier = 60000;
        public AdHocJobService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            int reportingTimerInterval = AppSettingsHelper.AdHocJobTimerIntervalInMinutes * minuteMultipier;
            reportingTimer = new Timer(reportingTimerInterval);
            reportingTimer.Enabled = true;
            reportingTimer.Elapsed += AdHocJobTimer_Elapsed;
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "AdHocJob engine started.");
        }

        protected override void OnStop()
        {

        }

        private void AdHocJobTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Code to write Reporting Engine;
            AdHocJobEngine reportingEngine = new AdHocJobEngine();
            reportingEngine.StartProcess();
        }
    }
}

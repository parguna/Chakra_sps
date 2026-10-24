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
using DI.Chakra.Services.ReportingProcessor;

namespace DI.Chakra.Services.Reporting
{
    partial class Reporting : ServiceBase
    {
        Timer reportingTimer;
        int minuteMultipier = 60000;
        public Reporting()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            // TODO: Add code here to start your service.
            int reportingTimerInterval = AppSettingsHelper.ReportingTimerIntervalInMinutes * minuteMultipier;
            reportingTimer = new Timer(reportingTimerInterval);
            reportingTimer.Enabled = true;
            reportingTimer.Elapsed += ReportingTimer_Elapsed;
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Reporting engine started.");
        }

        protected override void OnStop()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Reporting engine stopped.");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        private void ReportingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            //Code to write  Reporting Engine;
            ReportingEngine reportingEngine = new ReportingEngine();
            reportingEngine.StartProcess();
        }
    }
}

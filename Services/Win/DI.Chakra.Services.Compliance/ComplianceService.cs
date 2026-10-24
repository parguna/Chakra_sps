using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.ComplianceProcessor;
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

namespace DI.Chakra.Services.Compliance
{
    public partial class ComplianceService : ServiceBase
    {
        Timer complianceTimer;

        bool isComplianceInProgress = false;

        int minuteMultiplier = 60000;
        public ComplianceService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int complianceTimerInterval = AppSettingsHelper.ComplianceTimerIntervalInMinutes * minuteMultiplier;


            complianceTimer = new Timer(complianceTimerInterval);
            complianceTimer.Enabled = true;
            complianceTimer.Elapsed += new ElapsedEventHandler(complianceTimer_Elapsed);


            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Compliance engine started.");
        }

        protected override void OnStop()
        {
        }

        void complianceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isComplianceInProgress)
            {
                isComplianceInProgress = true;

                ComplianceEngine complianceEngine = new ComplianceEngine();
                complianceEngine.StartProcess();
                isComplianceInProgress = false;
            }
        }
    }
}

using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.HouseKeepingProcessor;
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

namespace DI.Chakra.Services.HouseKeeping
{
    partial class HouseKeepingService : ServiceBase
    {
        public HouseKeepingService()
        {
            InitializeComponent();
        }

        Timer purgingTimer;
        bool isHouseKeepoingInProgress = false;
        int minuteMultiplier = 60000;


        protected override void OnStart(string[] args)
        {
            int housekeepingTimerInterval = AppSettingsHelper.HousekeepingTimerIntervalInMinutes * minuteMultiplier;

            purgingTimer = new Timer(housekeepingTimerInterval);
            purgingTimer.Enabled = true;
            purgingTimer.Elapsed += new ElapsedEventHandler(purgingTimer_Elapsed);

            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Housekeeping agent started.");
        }

        protected override void OnStop()
        {
             DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Housekeeping agent stopped.");
        }
        
        void purgingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isHouseKeepoingInProgress)
            {
                isHouseKeepoingInProgress = true;

                PurgeAgent purgeAgent = new PurgeAgent();
                purgeAgent.StartProcess();

                isHouseKeepoingInProgress = false;
            }
        }
    }
}

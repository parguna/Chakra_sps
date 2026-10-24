
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.DiscoveryProcessor;
using System.ServiceProcess;
using System.Timers;

namespace DI.Chakra.Services.Discovery
{
    public partial class DiscoveryService : ServiceBase
    {

        Timer discoveryTimer;
        int minuteMultiplier = 60000;

        public DiscoveryService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int discoveryTimerInterval = AppSettingsHelper.DiscoveryTimerIntervalInMinutes * minuteMultiplier;

            discoveryTimer = new Timer(discoveryTimerInterval);
            discoveryTimer.Enabled = true;
            discoveryTimer.Elapsed += new ElapsedEventHandler(discoveryTimer_Elapsed);

            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Discovery engine started.");
        }

        protected override void OnStop()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Discovery engine stopped.");
        }

        void discoveryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
          //  DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Discovery engine in ticks.");
            DI.Chakra.Infrastructure.Enums.DiscoveryServiceInstance serviceInstance = (DiscoveryServiceInstance)AppSettingsHelper.DiscoveryInstance;
            DiscoveryEngine discoveryEngine = new DiscoveryEngine();
            discoveryEngine.StartProcess(serviceInstance);
        }
    }
}

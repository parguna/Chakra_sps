
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.AttestationProcessor;
using System;
using System.ServiceProcess;
using System.Timers;

namespace DI.Chakra.Services.Attestation
{
    public partial class AttestationService : ServiceBase
    {

        Timer attestationTimer;
        int minuteMultiplier = 60000;
        bool isAttestationProcessInProgress = false;

        public AttestationService()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "AttestationService  initialation started.");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Attestation engine started initializing timer.");
                int attestationTimerInterval = AppSettingsHelper.AttestationTimerIntervalInMinutes * minuteMultiplier;

                attestationTimer = new Timer(attestationTimerInterval);
                attestationTimer.Enabled = true;
                attestationTimer.Elapsed += new ElapsedEventHandler(attestationTimer_Elapsed);

                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Attestation engine started.");
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Attestation engine on start exception : " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Attestation engine stopped.");
        }

        void attestationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isAttestationProcessInProgress)
            {
                AttestationEngine attestationEngine = null;
                try
                {
                    isAttestationProcessInProgress = true;
                    attestationEngine = new AttestationEngine();
                    attestationEngine.StartProcess();
                    
                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Attestation engine exception : " + ex.Message);
                }
                finally
                {
                    attestationEngine = null;
                    isAttestationProcessInProgress = false;
                }
            }
            
        }
    }
}

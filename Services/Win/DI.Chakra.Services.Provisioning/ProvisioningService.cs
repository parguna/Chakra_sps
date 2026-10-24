
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor;
using System.ServiceProcess;
using System.Timers;
using System;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Services.Provisioning
{
    public partial class ProvisioningService : ServiceBase
    {

        Timer provisioningTimer;
        Timer trustedSourceChangePropagationTimer;
        Timer miscProcessTimer;
        Timer telephoneDirectorySyncTimer;
        Timer nonEmployeeValidationTimer;
        
        bool isProvisioningInProgress = false;
        bool isPropagationInProgress = false;
        bool isMiscProcessInProgress = false;
        bool nonEmployeeValidationInProgess = false;
        int minuteMultiplier = 60000;
        bool telephoneDirectorySyncInProgress = false;
        DateTime _scheduleTime;
        DateTime _scheduleNonEmployeeTime;
        public ProvisioningService()
        {
            InitializeComponent();
            int telephoneDirectorySyncTimeHour = AppSettingsHelper.TelephoneDirectorySyncTimeInHourOfDay;
            _scheduleTime = DateTime.Today.AddHours(telephoneDirectorySyncTimeHour); // Schedule to run once a day as per config.
            _scheduleNonEmployeeTime = DateTime.Today.AddMinutes(5); // Schedule to run once a day as per config.
        }

        protected override void OnStart(string[] args)
        {
            double provisioningTimerInterval = AppSettingsHelper.ProvisioningTimerIntervalInMinutes * minuteMultiplier;
            int trustedSourceChangePropagationTimerInterval = AppSettingsHelper.TrustedSourceChangePropagationTimerIntervalInMinutes * AppSettingsHelper.MinuteMultiplier;
            int miscProcessTimerInterval = AppSettingsHelper.MiscProcessTimerIntervalInMinutes * minuteMultiplier;
            var telephoneDirectorySyncTimerInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            //  var nonEmployeeValidationTimerInterval = _scheduleNonEmployeeTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            var nonEmployeeValidationTimerInterval = AppSettingsHelper.MiscProcessTimerIntervalInMinutes * minuteMultiplier;
            
            provisioningTimer = new Timer(provisioningTimerInterval);
            provisioningTimer.Enabled = true;
            provisioningTimer.Elapsed += new ElapsedEventHandler(provisioningTimer_Elapsed);

            trustedSourceChangePropagationTimer = new Timer(trustedSourceChangePropagationTimerInterval);
            trustedSourceChangePropagationTimer.Enabled = true;
            trustedSourceChangePropagationTimer.Elapsed += new ElapsedEventHandler(trustedSourceChangePropagationTimer_Elapsed);

            miscProcessTimer = new Timer(miscProcessTimerInterval);
            miscProcessTimer.Enabled = true;
            miscProcessTimer.Elapsed += new ElapsedEventHandler(miscProcessTimer_Elapsed);

            if (telephoneDirectorySyncTimerInterval < 0)
            {
                int telephoneDirectorySyncTimeHour = AppSettingsHelper.TelephoneDirectorySyncTimeInHourOfDay;
                _scheduleTime = DateTime.Today.AddDays(1).AddHours(telephoneDirectorySyncTimeHour); // Schedule to run once a day as per config.
                telephoneDirectorySyncTimerInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            }

            if (nonEmployeeValidationTimerInterval < 0)
            {
                _scheduleNonEmployeeTime = DateTime.Today.AddDays(1).AddHours(6); // Schedule to run once a day as per config.
                nonEmployeeValidationTimerInterval = AppSettingsHelper.MiscProcessTimerIntervalInMinutes * minuteMultiplier;
            }

            telephoneDirectorySyncTimer = new Timer(telephoneDirectorySyncTimerInterval);
            telephoneDirectorySyncTimer.Enabled = true;
            telephoneDirectorySyncTimer.Elapsed += new ElapsedEventHandler(telephoneDirectorySyncTimer_Elapsed);

            nonEmployeeValidationTimer = new Timer(nonEmployeeValidationTimerInterval);
            nonEmployeeValidationTimer.Enabled = true;
            nonEmployeeValidationTimer.Elapsed += new ElapsedEventHandler(nonEmployeeValidationTimer_Elapsed);


            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Provisioning engine started.");
        }

        protected override void OnStop()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Provisioning engine stopped.");
        }

        void provisioningTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isProvisioningInProgress)
            {
                ProvisioningEngine provisioningEngine = null;
                try
                {
                    isProvisioningInProgress = true;
                    provisioningEngine = new ProvisioningEngine();
                    provisioningEngine.StartProvisioning();
                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Provisioning Service failue.", ex);
                }
                finally
                {
                    provisioningEngine = null;
                    isProvisioningInProgress = false;
                }
            }
        }

        void trustedSourceChangePropagationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isPropagationInProgress)
            {
                ProvisioningEngine provisioningEngine = null;
                try
                {
                    isPropagationInProgress = true;

                    provisioningEngine = new ProvisioningEngine();
                    provisioningEngine.StartTrustedSourcePropagation();


                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Propogation failure.",ex);
                }
                finally
                {
                    provisioningEngine = null;
                    isPropagationInProgress = false;
                }
            }
        }

        void miscProcessTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isMiscProcessInProgress)
            {
                ProvisioningEngine provisioningEngine = null;
                try
                {
                    isMiscProcessInProgress = true;

                    provisioningEngine = new ProvisioningEngine();
                    provisioningEngine.StartMiscProcess();
                    provisioningEngine = null;
                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "MiscProcess failure.", ex);
                }
                finally
                {
                    provisioningEngine = null;
                    isMiscProcessInProgress = false;
                }
            }
        }

        void telephoneDirectorySyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!telephoneDirectorySyncInProgress)
            {
                telephoneDirectorySyncInProgress = true;
                ProvisioningEngine provisioningEngine = null;
                try
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Telephone Directory Timer Elapsed");

                    // If tick for the first time, reset next run to every 24 hours
                    if (telephoneDirectorySyncTimer.Interval != 24 * 60 * 60 * 1000)
                    {
                        telephoneDirectorySyncTimer.Interval = 24 * 60 * 60 * 1000;
                    }

                    provisioningEngine = new ProvisioningEngine();
                    provisioningEngine.PrepareTelephoneDirectorySyncData();
                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "telephoneDirectorySync failure.", ex);
                }
                finally
                {
                    provisioningEngine = null;
                    telephoneDirectorySyncInProgress = false;
                }
            }
           
        }

        void nonEmployeeValidationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!nonEmployeeValidationInProgess)
            {
                nonEmployeeValidationInProgess = true;
                ProvisioningEngine provisioningEngine = null;
                try
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Non employee expiry validation timer elapsed.");

                    // If tick for the first time, reset next run to every 24 hours
                    if (nonEmployeeValidationTimer.Interval != 24 * 60 * 60 * 1000)
                    {
                        nonEmployeeValidationTimer.Interval = 24 * 60 * 60 * 1000;
                    }

                    provisioningEngine = new ProvisioningEngine();
                    provisioningEngine.NotifyNonEmployeeExpiration();
                    //provisioningEngine.UpdateADManagerAttribute();
                    provisioningEngine.UpdateBadgeNumberInADAttribute();
                    provisioningEngine.ResetO365IDMSAccountPassword();
                }
                catch (Exception ex)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Misc Service: NotifyNonEmployeeExpiration/UpdateADManagerAttribute/UpdateBadgeNumberInADAttribute/ResetO365IDMSAccountPassword failure.", ex);
                }
                finally
                {
                    nonEmployeeValidationInProgess = false;
                    provisioningEngine = null;
                }
            }
        }
        
    }
}

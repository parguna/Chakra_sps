using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
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
using DI.Chakra.Services.MiscProcessor;
using DI.Chakra.Service.MiscProcessor;
using DI.Chakra.DomainModel;
using DI.Chakra.Service.MiscProcessor.AttestationReport;
using DI.Chakra.Service.MiscProcessor.DailyJobForDelegation;

namespace DI.Chakra.Services.Misc
{
    partial class MiscService : ServiceBase
    {
        readonly PUBService pUBService;
        readonly WorkflowReminderService workflowReminderService;
        readonly AttestationReportService attestationReportService;
        readonly CustomReportService customReportService;
        readonly UpdateADManagerService updateADManagerService;
        readonly DecommissionService decommissionService;
        readonly MaintenanceService maintenanceService;
        readonly PurgeConfigurationService purgeConfigurationService;

        readonly DeactivateExpiredDelegationService deactivateExpiredDelegationService;
        readonly OtherSystemDiscoveryService otherSystemDiscoveryService;
        public MiscService()
        {
            try
            {
                pUBService = new PUBService();
                
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous pUBService failed.", ex);
            }

            try
            {
                workflowReminderService = new WorkflowReminderService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous workflowReminderService failed.", ex);
            }
            try
            {
                attestationReportService = new AttestationReportService();

            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous AttestationReportService failed.", ex);
            }
            try
            {
                customReportService = new CustomReportService();

            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous customReportService failed.", ex);
            }
            try
            {
                updateADManagerService = new UpdateADManagerService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous UpdateADManagerService failed.", ex);
            }
            try
            {
                decommissionService = new DecommissionService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous decommissionService failed.", ex);
            }
            try
            {
                purgeConfigurationService = new PurgeConfigurationService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous purgeConfigurationService failed.", ex);
            }
            try
            {
                maintenanceService = new MaintenanceService();

            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous maintenanceService failed.", ex);
            }
            try
            {
                deactivateExpiredDelegationService = new DeactivateExpiredDelegationService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous DeactivateExpiredDelegationService failed.", ex);
            }
            try
            {
                otherSystemDiscoveryService = new OtherSystemDiscoveryService();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous otherSystemDiscoveryService start failed.", ex);
            }
            InitializeComponent();
        }

        Timer purgingTimer;
        bool isMiscInProgress = false;
        int minuteMultiplier = 60000;

        Timer wakeupWorkflowTimer;
        bool isWakeUpInProgress = false;

        Timer wakeupServiceTimer;
        bool isWakeUpServiceInProgress = false;

        Timer healthCheckServiceTimer;
        bool isHealthCheckInProgress = false;
        Timer tcodeGeneratorTimer;
        bool isTCodeGeneratiorInProgress = false;
        bool iHRMSUpdateProgress = false;
        Timer workflowReminderValidationTimer;
        bool isADPropagationRunning = false;
        bool iMaintenanceServiceRunning = false;
        protected override void OnStart(string[] args)
        {
            try
            {
                pUBService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous pUBService failed.",ex);
            }
            try
            {
                workflowReminderService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous pUBService failed.", ex);
            }
            try
            {
                attestationReportService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous AttestationReportService failed.", ex);
            }
            try
            {
                customReportService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous customReportService failed.", ex);
            }
            try
            {
                updateADManagerService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous updateADManagerService failed.", ex);
            }
            try
            {
                decommissionService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous decommissionService failed.", ex);
            }
            try
            {
                purgeConfigurationService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous purgeConfigurationService start failed.", ex);
            }
            try
            {
                maintenanceService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous maintenanceService failed.", ex);
            }
            try
            {
                deactivateExpiredDelegationService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous DeactivateExpiredDelegationService failed.", ex);
            }
            try
            {
                otherSystemDiscoveryService.Start();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous otherSystemDiscoveryService failed.", ex);
            }
            
            var hrmsTimer = new Timer(DomainModel.MasterConfigHelper.HRMSDataUpdateTimer); //one hour in milliseconds
            hrmsTimer.Elapsed += UpdateHRMSUserDataTimer_Elapsed;
            hrmsTimer.Enabled = true;

            //var adPropagation = new Timer(DomainModel.MasterConfigHelper.ADPropagationTimer); //one hour in milliseconds
            //adPropagation.Elapsed += ADPropagation_Elapsed;
            //adPropagation.Enabled = true;

            int miscellaneoustimerInterval = AppSettingsHelper.MiscellaneousTimerIntervalInMinutes * minuteMultiplier;

            purgingTimer = new Timer(miscellaneoustimerInterval);
            purgingTimer.Elapsed += purgingTimer_Elapsed;
            purgingTimer.Enabled = true;

            int wakeUptimerInterval = AppSettingsHelper.WakeupWorkflowTimerIntervalInMinutes * minuteMultiplier;
            wakeupWorkflowTimer = new Timer(wakeUptimerInterval);
            wakeupWorkflowTimer.Elapsed += WakeupWorkflowTimer_Elapsed;
            wakeupWorkflowTimer.Enabled = true;


            int wakeUpServicetimerInterval = AppSettingsHelper.WakeupServiceTimerIntervalInMinutes * minuteMultiplier;
            wakeupServiceTimer = new Timer(wakeUpServicetimerInterval);
            wakeupServiceTimer.Elapsed += WakeupServicesTimer_Elapsed;
            wakeupServiceTimer.Enabled = true;

            var timer = getTimerForNextWeekDay();// DomainModel.MasterConfigHelper.TCodeGenerationTimer;
            tcodeGeneratorTimer = new Timer(timer); 
            tcodeGeneratorTimer.Elapsed += new ElapsedEventHandler(TcodeGeneratorTimer_Elapsed);
            tcodeGeneratorTimer.Enabled = true;

           
            //int healthCheckServicetimerInterval = AppSettingsHelper.HealthCheckServiceTimerIntervalInMinutes * minuteMultiplier;
            //healthCheckServiceTimer = new Timer(healthCheckServicetimerInterval);
            //healthCheckServiceTimer.Elapsed += HealthCheckServicesTimer_Elapsed;
            //healthCheckServiceTimer.Enabled = true;

            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Miscellaneous agent started.");
        }

        long getTimerForNextWeekDay()
        {
            DateTime today = DateTime.Today;
            int dateofWeek = (int)DomainModel.MasterConfigHelper.TCodeGenerationTimer;

            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilday = (dateofWeek - (int)today.DayOfWeek + 7) % 7;
            if(daysUntilday <= 0)
            {
                daysUntilday = 1;
            }
            DateTime nextday = today.AddDays(daysUntilday);

            TimeSpan diff = nextday - today;
            
            int millisceonds = (int)diff.TotalMilliseconds;
            return millisceonds;
        }


        private void TcodeGeneratorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!isTCodeGeneratiorInProgress)
                {
                    isTCodeGeneratiorInProgress = true;
                    WakeupServices provisioningEngine = new WakeupServices();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "TCode generation started");
                    provisioningEngine.GenerateTCode();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "TCode generation Completed");
                    isTCodeGeneratiorInProgress = false;
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "TcodeGeneratorTimer failed.", ex);
                isTCodeGeneratiorInProgress = false;
            }
            
        }
        private void WakeupWorkflowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PingWorkflow();
        }

        private void WakeupServicesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PingServices();
        }

        private void HealthCheckServicesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PingHealthCheckServices();
        }

        private void PingWorkflow()
        {
            // calls the work flow service to keep it alive
            if (!isWakeUpInProgress)
            {
                try
                {
                    isWakeUpInProgress = true;
                    WakeUpWorkflow wakeUpworkflow = new WakeUpWorkflow();
                    wakeUpworkflow.Browse();
                }
                finally
                {
                    isWakeUpInProgress = false;
                }
            }
        }
        private void PingServices()
        {
            // calls the work flow service to keep it alive
            if (!isWakeUpServiceInProgress)
            {
                try
                {
                    isWakeUpServiceInProgress = true;
                    WakeupServices wakeupServices = new WakeupServices();
                    wakeupServices.Start();
                }
                finally
                {
                    isWakeUpServiceInProgress = false;
                }
            }
        }

        private void PingHealthCheckServices()
        {
            // calls the work flow service to keep it alive
            if (!isHealthCheckInProgress && !isWakeUpServiceInProgress)
            {
                try
                {
                    isHealthCheckInProgress = true;
                    WakeupServices wakeupServices = new WakeupServices();
                    wakeupServices.StartHealthCheck();
                }
                finally
                {
                    isHealthCheckInProgress = false;
                }
            }
        }
        protected override void OnStop()
        {
            try
            {
                pUBService.Stop();

                
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous Stop exception.{ex.Message}",ex);
            }
            try
            {
                workflowReminderService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous Stop exception.{ex.Message}", ex);
            }
            try
            {
                customReportService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous customReportService Stop exception.{ex.Message}", ex);
            }
            try
            {
                updateADManagerService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous updateADManagerService Stop exception.{ex.Message}", ex);
            }
            try
            {
                attestationReportService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous attestationReportService Stop exception.{ex.Message}", ex);
            }
            try
            {
                decommissionService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous decommissionService Stop exception.{ex.Message}", ex);
            }
            try
            {
                purgeConfigurationService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "Miscellaneous purgeConfigurationService stop failed.", ex);
            }
            try
            {
                maintenanceService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous maintenanceService Stop exception.{ex.Message}", ex);
            }
            try
            {
                otherSystemDiscoveryService.Stop();
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Miscellaneous otherSystemDiscoveryService Stop exception.{ex.Message}", ex);
            }
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Miscellaneous agent stopped.");
        }

        void purgingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isMiscInProgress)
            {
                isMiscInProgress = true;

                //PurgeAgent purgeAgent = new PurgeAgent();
                //purgeAgent.StartProcess();


                isMiscInProgress = false;
            }
        }

        void UpdateHRMSUserDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                
                if (!iHRMSUpdateProgress)
                {
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "UpdateHRMSUserDataTimer_Elapsed started.");
                    iHRMSUpdateProgress = true;
                    WakeupServices discoveryInstance = new WakeupServices();
                    discoveryInstance.UpdateHRMSUserDataInIDMS();
                    iHRMSUpdateProgress = false;
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "UpdateHRMSUserDataTimer_Elapsed failed.",ex);
                iHRMSUpdateProgress = false;
            }
        }
        void ADPropagation_Elapsed(object sender, ElapsedEventArgs e)
        {
            //try
            //{
            //    if (!isADPropagationRunning)
            //    {
            //        isADPropagationRunning = true;
            //        WakeupServices adPropagation = new WakeupServices();
            //        adPropagation.CreateADPropagation();
            //        isADPropagationRunning = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "ADPropagation_Elapsed failed.", ex);
            //    isADPropagationRunning = false;
            //}
        }
    }
}

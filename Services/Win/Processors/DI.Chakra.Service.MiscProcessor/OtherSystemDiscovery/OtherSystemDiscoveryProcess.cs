using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class OtherSystemDiscoveryProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string crReminderIdentity = "SPOtherSystemDiscoveryProcess";
        public OtherSystemDiscoveryProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_OtherSystemDiscoveryTimer;
        }
        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"OtherSystemDiscoveryProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(crReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<OtherSystemDiscoveryProcessJob>().WithIdentity(crReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"OtherSystemDiscoveryProcess start failed.{ex.Message}", ex);
            }
        }
        public async void Stop()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                //sched.Standby();
                await sched.UnscheduleJob(new TriggerKey(crReminderIdentity));
                _logger.WriteLog(LogLevelType.Info, $"OtherSystemDiscoveryProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"OtherSystemDiscoveryProcess Stop failed.{ex.Message}", ex);
            }
        }
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }
            _disposed = true;
        }
    }
    public class OtherSystemDiscoveryProcessJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public OtherSystemDiscoveryProcessJob()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {
                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                try
                {
                    _logger.WriteLog(LogLevelType.DebugStep, $"OtherSystemDiscoveryProcessJob executing");
                    DiscoverOtherSystem();
                    
                    _logger.WriteLog(LogLevelType.DebugStep, $"OtherSystemDiscoveryProcessJob completed");
                    
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"OtherSystemDiscoveryProcessJob Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }

        public static void DiscoverOtherSystem()
        {
            DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
            try
            {
                if (MasterConfigHelper.CanRunOtherSystemReverseEngineeringService)
                {
                    var fromDate = DateTime.Now;
                    var slogger = new SchedulerLogger();
                    slogger.WriteLog(LogLevelType.Info, "Started DiscoverOtherSystem Reverse Engineering");
                    var endPoints = EndPoint.GetActiveOtherSystemAutomatedEndpoints();
                    foreach (var endPoint in endPoints)
                    {
                        try
                        {
                            IdmsLogger logger = new DiscoveryLogger(0, endPoint.Id, EndpointType.InhouseSystems, string.Empty);
                            var connector = ConnectorFactory.GetConnector(endPoint, null, logger);
                            connector.GetData();
                        }
                        catch (Exception ex)
                        {
                            _logger?.WriteLog(LogLevelType.Info, $"DiscoverOtherSystem :{endPoint.DisplayName} failed.{ex.Message}", ex);
                        }
                    }

                    endPoints?.Clear();
                    endPoints = null;

                    var toDate = DateTime.Now;

                    var aduits = PresentationAccountEntitlementAuditTrial.GetAuditTrailForEmail(fromDate, toDate);
                    if(aduits != null && aduits.Count > 0)
                        ProvisioningEmailHelper.NotifyITAdminForReverseEngineering(aduits, _logger);

                    
                }
                try
                {
                    InhouseUserInfo.UpdateAllINHReconEntitlementDescription();
                }
                catch (Exception ex)
                {
                    _logger?.WriteLog(LogLevelType.Info, $"UpdateAllINHReconEntitlementDescription :{ex} failed.", ex);
                }
            }
            catch (Exception ex)
            {
                _logger?.WriteLog(LogLevelType.Info, $"DiscoverOtherSystem failed.{ex.Message}", ex);
            }
        }
    }
}

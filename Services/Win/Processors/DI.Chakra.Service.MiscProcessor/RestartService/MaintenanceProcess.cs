using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class MaintenanceProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string crReminderIdentity = "MaintenanceProcess";
        public MaintenanceProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_DiscoveryRestart;
        }

        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"MaintenanceProcessJob started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(crReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<MaintenanceProcessJob>().WithIdentity(crReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"MaintenanceProcessJob start failed.{ex.Message}", ex);
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
                _logger.WriteLog(LogLevelType.Info, $"MaintenanceProcessJob stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"MaintenanceProcessJob Stop failed.{ex.Message}", ex);
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
    public class MaintenanceProcessJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public MaintenanceProcessJob()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {
                try
                {
                    RestartService();
                }
                catch
                {
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }

        public void RestartService()
        {
            List<string> services = null;
            DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
            try
            {
                var discovery1 = MasterConfigHelper.DiscoveryServiceController;
                var discovery2 = MasterConfigHelper.DiscoveryServiceController2;
                services = new List<string> { discovery1, discovery2 };

                var timeoutMilliseconds = 40000;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                foreach (var serviceName in services)
                {
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    ServiceController service = null;
                    var serviceDName = serviceName;
                    try
                    {
                        service = new ServiceController(serviceName);
                        if (service != null)
                        {
                            serviceDName = service.DisplayName;
                            _logger.WriteLog(null, LogLevelType.Info, $"Restart Discovery Service :{serviceDName} initiated");
                            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                            System.Threading.Thread.Sleep(timeoutMilliseconds);

                            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                            _logger.WriteLog(null, LogLevelType.Info, $"Restart Discovery Service :{serviceDName} Completed");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteLog(null, LogLevelType.Error, $"Restart Discovery Service :{serviceDName} failed.", ex);
                    }
                    finally
                    {
                        service = null;
                    }
                    System.Threading.Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"Restart Discovery Service  failed.{ex.Message}", ex);
            }
            finally
            {
                services?.Clear();
                services = null;
                _logger = null;
            }
        }
    }
}

using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor.DailyJobForDelegation
{
    public class DeactivateExpiredDelegationProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string crReminderIdentity = "DeactivateExpiredDelegationProcess";
        public DeactivateExpiredDelegationProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_DeactivateExpiredAdvancedDelegation;
        }

        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"DeactivateExpiredDelegationProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(crReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<DeactivateExpiredDelegationProcessJob>().WithIdentity(crReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"DeactivateExpiredDelegationProcess start failed.{ex.Message}", ex);
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
                _logger.WriteLog(LogLevelType.Info, $"DeactivateExpiredDelegationProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"DeactivateExpiredDelegationProcess Stop failed.{ex.Message}", ex);
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
    public class DeactivateExpiredDelegationProcessJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public DeactivateExpiredDelegationProcessJob()
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
                    DoDeactivateExpiredDelegation();
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"DeactivateExpiredDelegationProcessJob Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }
        public static void DoDeactivateExpiredDelegation()
        {
            IdmsLogger idmsLogger = null;
            try
            {
                TaskDelegation.DeactivateAdvancedDelegation();
                //PresentationSystemDecommission.StartDecommisson();
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while DoDeactivateExpiredDelegation.", ex);
            }
        }
    }
}

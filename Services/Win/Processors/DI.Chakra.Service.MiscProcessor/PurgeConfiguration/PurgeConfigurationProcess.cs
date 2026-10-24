using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Services.MiscProcessor;

namespace DI.Chakra.Service.MiscProcessor
{
   public class PurgeConfigurationProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string purgeConfigurationIdentity = "PurgeConfiguration";


        public PurgeConfigurationProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_PurgeConfigTimer;
        }


        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"PurgeConfigurationProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(purgeConfigurationIdentity)
                     .WithCronSchedule(_CronScheduleString)  
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<PurgeConfigurationJob>().WithIdentity(purgeConfigurationIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"PurgeConfigurationProcess start failed.{ex.Message}", ex);
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
                await sched.UnscheduleJob(new TriggerKey(purgeConfigurationIdentity));
                _logger.WriteLog(LogLevelType.Info, $"PurgeConfigurationProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"PurgeConfigurationProcess Stop failed.{ex.Message}", ex);
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

    public class PurgeConfigurationJob : IJob
    {
        Dictionary<long, string> TaskEndorsedBy = new Dictionary<long, string>();
        Dictionary<long, int> TaskState = new Dictionary<long, int>();
        

        readonly string _masterConfigHelperServiceUser;
        public PurgeConfigurationJob()
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
                    _logger.WriteLog(LogLevelType.Info, $"PurgeConfigurationJob executing");
                    DoPurgeConfiguration();
                    _logger.WriteLog(LogLevelType.Info, $"PurgeConfigurationJob completed");
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"PurgeConfigurationJob Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }


        public static void DoPurgeConfiguration()
        {
            IdmsLogger idmsLogger = null;
            try
            {
                PurgeAgent purgeAgent = new PurgeAgent();
                purgeAgent.StartProcess();
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while DoPurgeConfiguration.", ex);
            }
        }


    }
}


using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DI.Chakra.Service.MiscProcessor
{
    public class CustomReportProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string crReminderIdentity = "SPCustomReportProcess";
        public CustomReportProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_CustomReportTimer;
        }
        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"CustomReportProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(crReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<CustomReportJob>().WithIdentity(crReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"CustomReportProcess start failed.{ex.Message}", ex);
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
                _logger.WriteLog(LogLevelType.Info, $"CustomReportProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"CustomReportProcess Stop failed.{ex.Message}", ex);
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
    public class CustomReportJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public CustomReportJob()
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
                    _logger.WriteLog(LogLevelType.DebugStep, $"CustomReportJob executing");
                    GenerateCustomReport();
                    _logger.WriteLog(LogLevelType.DebugStep, $"CustomReportJob completed");
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"CustomReportJob Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }
        public static void GenerateCustomReport()
        {
            List<PresentationAccessRequest> accessRequests = null;
            DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
            try
            {
                _logger.WriteLog(LogLevelType.DebugStep, $"AccessRequest Report started");
                accessRequests = PresentationAccessRequest.GetAccessRequestForReportExtraction();
                if(accessRequests != null && accessRequests.Count > 0)
                {
                    //ExportReport
                    var path = MasterConfigHelper.AccessRequestReportFilePath;
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    try
                    {
                        var date = DateTime.Now.ToString("ddMMyyyyhhmmss");
                        var filePath = $"{path}\\AccessRequest_Report_{date}.csv";
                        using (StreamWriter stream = new StreamWriter(filePath))
                        {
                            StringBuilder objWriter = new StringBuilder();
                            stream.WriteLine(string.Format("\"\t{0}\",\"\t{1}\",\"\t{2}\",\"\t{3}\",\"\t{4}\",\"\t{5}\",\"\t{6}\",\"\t{7}\"", "Request ID","Application Name","Request Type","Request For","Requested ID","Account Type","Request Source","Execution Date/Time"));
                            foreach (var data in accessRequests)
                            {
                                stream.WriteLine(
                                string.Format("\"\t{0}\",\"\t{1}\",\"\t{2}\",\"\t{3}\",\"\t{4}\",\"\t{5}\",\"\t{6}\",\"\t{7}\"", data.Id, data.EndpointName ?? "", data.RequestType ?? "", data.RequesteeUserName ?? "", data.RequesterUserName ?? "", data.AccountType.ToString(), data.RequestSource??"", data.ExecutionDate?.ToString("dd -MM-yyyy hh:mm:ss") ?? ""));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteLog(LogLevelType.Info, $"AccessRequest Report failed.{ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, $"AccessRequest Report failed.{ex.Message}", ex);
            }
            finally
            {
                accessRequests?.Clear();
                accessRequests = null;
            }
        }
       
    }
}

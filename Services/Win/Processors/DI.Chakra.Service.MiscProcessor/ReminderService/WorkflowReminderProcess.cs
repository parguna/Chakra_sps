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
    public class WorkflowReminderProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string wfReminderIdentity = "WorkflowReminderEmail";
        public WorkflowReminderProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_WorkflowReminderTimer;
        }
        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.DebugStep, $"WorkflowReminderProcess started");
                await scheduler.Start();
                
                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(wfReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<ReminderJob>().WithIdentity(wfReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"WorkflowReminderProcess start failed.{ex.Message}", ex);
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
                await sched.UnscheduleJob(new TriggerKey(wfReminderIdentity));
                _logger.WriteLog(LogLevelType.DebugStep, $"WorkflowReminderProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"WorkflowReminderProcess Stop failed.{ex.Message}", ex);
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
    public class ReminderJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public ReminderJob()
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
                    _logger.WriteLog(LogLevelType.DebugStep, $"ReminderJob executing");
                    SendTaskReminder();
                    _logger.WriteLog(LogLevelType.DebugStep, $"ReminderJob completed");
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"WorkflowReminderProcess Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }
        public void SendTaskReminder()
        {
            List<DomainModel.Task> allPendingTask = new List<DomainModel.Task>();
            try
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"SendTaskReminder trigger started");

                if (MasterConfigHelper.AllowAttestationTaskReminder)
                {
                    allPendingTask = DomainModel.Task.GetAllPendingAttestationTask();
                }
                if (MasterConfigHelper.AllowProvisioningTaskReminder)
                {
                    var provisioningTsk = DomainModel.Task.GetAllPendingProvisioningTaskForReminder();
                    if(provisioningTsk != null && provisioningTsk.Count > 0)
                    {
                        allPendingTask.AddRange(provisioningTsk);
                    }
                }
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"SendReminderOrEsclation trigger for OnlyAttestationTask");
                if (allPendingTask != null && allPendingTask.Count > 0)
                {
                    DomainModel.Task deepLoadedTask = null;
                    RequestType requestType = RequestType.None;
                    foreach (var tsk in allPendingTask)
                    {
                        deepLoadedTask = null;
                        try
                        {
                            if (tsk != null && tsk.Action == StatusType.Pending && tsk.WorkFlowConfiguration != null && tsk.WorkFlowConfiguration.WorkflowConfigurationDetails != null && tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.Any(i => i.SequenceNumber == tsk.WorkflowStateId))
                            {
                                requestType = (RequestType)tsk.RequestTypeId;
                                var wfDetail = tsk.WorkFlowConfiguration.WorkflowConfigurationDetails.FirstOrDefault(i => i.SequenceNumber == tsk.WorkflowStateId);

                                var reminderTimeSpan = (double)wfDetail.ReminderIntervalInDays;
                                var timeoutTimespan = (double)wfDetail.ResponseTimeInDays;
                                var totalRemindr = (int)Math.Floor((double)timeoutTimespan / (double)reminderTimeSpan);
                                double afterEsclation = 0;
                                if (reminderTimeSpan > 0 && wfDetail.DaysToWaitAfterEscalation.HasValue)
                                {
                                    afterEsclation = (double)(wfDetail.DaysToWaitAfterEscalation.Value / reminderTimeSpan);
                                    totalRemindr = (int)totalRemindr + (int)afterEsclation;
                                }

                                var tskReminder = TaskReminder.GetByTaskId(tsk.Id);
                                if (wfDetail.IsReminderRequired && wfDetail.ReminderIntervalInDays > 0)
                                {
                                    var totalDays = DateTime.Now.Subtract(tsk.CreatedDate).TotalDays;
                                    if (tskReminder != null)
                                    {
                                        if ((int)Math.Floor((double)(totalDays / reminderTimeSpan)) == tskReminder.ReminderCount)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (totalDays < reminderTimeSpan)
                                        {
                                            continue;
                                        }
                                    }

                                    if (tskReminder == null)
                                    {
                                        tskReminder = new TaskReminder();
                                        tskReminder.TaskId = tsk.Id;
                                        tskReminder.IsActive = true;
                                        tskReminder.ReminderCount = 0;
                                        tskReminder.RequestType = (tsk.AttestationId == null && tsk.AttestationCampaignId == null) ? RequestType.Provisioning : RequestType.Attestation;
                                        tskReminder.CreatedDate = tsk.CreatedDate;
                                    }
                                    if ((tskReminder.Id == 0 || tskReminder.ReminderCount < totalRemindr) && tskReminder.IsActive)
                                    {
                                        #region For Attesttaion
                                        if (tsk.AttestationId != null && tsk.AttestationCampaignId != null)
                                        {
                                            deepLoadedTask = DomainModel.Task.GetAttestationTask(tsk.Id, true);
                                            if (deepLoadedTask != null && ProvisioningEmailHelper.SendReminderEmailForAttestation(deepLoadedTask, tsk.WorkFlowConfiguration, wfDetail))
                                            {
                                                tskReminder.ReminderCount += 1;
                                                tskReminder.Save("System");
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Attesttaion - Reminder Email sent successfully for TaskId:{tsk.Id}");
                                            }
                                            else
                                            {
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"Attesttaion - Reminder Email failed for TaskId:{tsk.Id}");
                                            }
                                        }
                                        #endregion
                                        else if(tsk.AccessRequestId != null && requestType == RequestType.Provisioning)
                                        {
                                            #region Reminder for Provisioning
                                            deepLoadedTask = DomainModel.Task.GetProvisioningTask(tsk.Id);
                                            if (deepLoadedTask != null && ProvisioningEmailHelper.SendEmailForAdhocProvisioning(deepLoadedTask))
                                            {
                                                tskReminder.ReminderCount += 1;
                                                tskReminder.Save("System");
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"Provisioning Reminder Email sent successfully for TaskId:{tsk.Id}");
                                            }
                                            else
                                            {
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"Provisioning - Reminder Email failed for TaskId:{tsk.Id}");
                                            }
                                            #endregion
                                        }
                                        else if (tsk.BulkRequestId != null && requestType == RequestType.BulkRequest)
                                        {
                                            #region Reminder for Provisioning
                                            deepLoadedTask = DomainModel.Task.GetBulkRequestTask(tsk.Id);
                                            if (deepLoadedTask != null && ProvisioningEmailHelper.SendEmailForBulkRequest(deepLoadedTask,tsk.WorkFlowConfiguration))
                                            {
                                                tskReminder.ReminderCount += 1;
                                                tskReminder.Save("System");
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"BulkRequest - Reminder Email sent successfully for TaskId:{tsk.Id}");
                                            }
                                            else
                                            {
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"BulkRequest - Reminder Email failed for TaskId:{tsk.Id}");
                                            }
                                            #endregion
                                        }
                                        else if (tsk.EmployeeEventTriggerId != null && requestType == RequestType.EmployeeEvent)
                                        {
                                            #region Reminder for EmployeeEvent
                                            var trigger = EmployeeEventTrigger.GetById(tsk.EmployeeEventTriggerId.Value);
                                            if (trigger != null && ProvisioningEmailHelper.SendEmailForEmployeeEvent(tsk,tsk.WorkFlowConfiguration, trigger))
                                            {
                                                tskReminder.ReminderCount += 1;
                                                tskReminder.Save("System");
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"EmployeeEventTrigger - Reminder Email sent successfully for TaskId:{tsk.Id}");
                                            }
                                            else
                                            {
                                                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"EmployeeEventTrigger - Reminder Email failed for TaskId:{tsk.Id}");
                                            }
                                            #endregion
                                        }
                                    }
                                }

                                if(wfDetail.IsEscalationRequired && tsk.IsEscalated == false && DateTime.Now.Subtract(tsk.CreatedDate).TotalDays > timeoutTimespan)
                                {
                                    deepLoadedTask = DomainModel.Task.GetAttestationTask(tsk.Id, true);
                                    var attest = Attestation.GetByIdWithDependent(deepLoadedTask.AttestationId.Value);
                                    deepLoadedTask.Attestation = attest;
                                    if (deepLoadedTask != null && 
                                        ProvisioningEmailHelper.SendEsclationEmailForAttestation(deepLoadedTask, tsk.WorkFlowConfiguration, attest))
                                    {
                                        DomainModel.Task.ExecuteQuery($"Update DI_M_Task Set IsEscalated = 1 where Id = {tsk.Id}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"SendReminderOrEsclation trigger Task failed. TaskId:{tsk.Id} {ex.Message}", ex);
                        }
                    }
                }

                TaskReminder.ExecuteQueryWithCommandTimeout("Update [dbo].[DI_M_TaskReminder]  set IsActive=0,LastModifiedDate=GETDATE(),LastModifiedBy='System' where TaskId in(select Id from DI_M_Task WHERE [Action] != 1)", 0);
                //}
                //else if (MasterConfigHelper.AllowProvisioningTaskReminder)
                //{
                //    //For Future implementation
                //    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"SendReminderOrEsclation trigger for OnlyProvisioningTask");
                //}
                DILog.SchedulerLogger.WriteLog(LogLevelType.DebugStep, $"SendTaskReminder trigger completed");
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"SendTaskReminder trigger failed.{ex.Message}", ex);
            }
        }
    }
}

using System;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.ComponentModel; 




namespace DI.Chakra.Services.DiscoveryProcessor
{
    public class BackgroundWorkerHelper
    {
        private delegate void startProcessDelegate(Process process);
        private startProcessDelegate startProcess;
        private BackgroundWorker bgWorker;

        public BackgroundWorkerHelper()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            
        }

        public void StartProcessThread(Process process,string ProcessName)
        {
            switch (ProcessName)
            {
                case "Discovery": startProcess = new startProcessDelegate(discoveryProcess);
                    break;
                default: startProcess = new startProcessDelegate(DefaultProcess);
                    break;
            }

            bgWorker.RunWorkerAsync(process);
        }

       
        private void discoveryProcess(Process process)
        {
            DiscoveryService service = null;
            try
            {
                service = new DiscoveryService(process);
                process.ProcessStartAt = DateTime.Now;
                process.Save(MasterConfigHelper.ServiceUser);
                service.Start();
            }
            catch (Exception)
            {
            }
            finally
            {
                service = null;
            }
            
            //TODO: CHECK HARI
            //DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Start processing discovery process id: {0}", schedulerData.DiscoveryProcessId.ToString()), null, "Discovery", Convert.ToInt32(schedulerData.DiscoveryProcessId), 0, "Scheduler Service");

            //SchedulerDA m_schedulerDA = new SchedulerDA();
            //DiscoveryService service = new DiscoveryService();
            //bool IsRunEveryDay = schedulerData.RunEveryDay == null ? false : schedulerData.RunEveryDay.Value;
            //if (IsRunEveryDay)
            //    UpdateSchedulerForEveryDayRun(schedulerData);
            //service.Start(schedulerData.DiscoveryProcessId.Value);

            
            //Scheduler sch = this.updateSchedulerForNextRun(schedulerData);
            //sch.LastModifiedBy = KEY_SERVICENAME;
            //sch.LastModifiedDate = DateTime.Now;
            //m_schedulerDA.UpdateScheduler(sch);

            //DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Completed processing discovery process id: {0}", schedulerData.DiscoveryProcessId.ToString()), null, "Discovery", Convert.ToInt32(schedulerData.DiscoveryProcessId), 0, "Scheduler Service");

        }

        private void UpdateSchedulerForEveryDayRun(Scheduler _scheduler)
        {
            //TODO: CHECK HARI
            //var date = DateTime.Now;
            //var dateWithTime = new DateTime(date.Year, date.Month, date.Day, _scheduler.StartDate.Value.Hour, _scheduler.StartDate.Value.Minute, _scheduler.StartDate.Value.Second);
            //_scheduler.RunDate = dateWithTime;
            //_scheduler.LastModifiedBy = KEY_SERVICENAME;
            //_scheduler.LastModifiedDate = DateTime.Now;
            //SchedulerDA m_schedulerDA = new SchedulerDA();
            //m_schedulerDA.UpdateScheduler(_scheduler);
        }

        private Scheduler updateSchedulerForNextRun(Scheduler _scheduler)
        {
            return null;
            //TODO: CHECK HARO
            //if (null == _scheduler)
            //    return null;

            //if (_scheduler.RunEveryDay.Value)
            //{
                 
               
            //}
            //else if (_scheduler.RunEveryWeek.Value)
            //{
            //    DayOfWeek weekDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), _scheduler.RunEveryWeekDay);

            //    if (weekDay == DateTime.Now.DayOfWeek)
            //        _scheduler.RunDate = DateTime.Now.AddDays(7);

            //    else
            //    {
            //        int requiredDay = Convert.ToInt32(weekDay);
            //        int currentDay = Convert.ToInt32(DateTime.Now.DayOfWeek);

            //        int noOfDays = requiredDay - currentDay;

            //        if (noOfDays < 0)
            //            noOfDays = 7 + noOfDays;

            //        _scheduler.RunDate = DateTime.Now.AddDays(noOfDays);
            //    }
            //}
            //else if (_scheduler.RunEveryMonth.Value)
            //{
            //    _scheduler.RunDate = _scheduler.StartDate.Value.AddMonths(1);
            //}
            //else if (_scheduler.RunSpecifiedTime.Value)
            //{
            //    DateTime tomorrow = DateTime.Now.AddDays(1);

            //    if (_scheduler.SpecifiedToTime.Value.Hours <= DateTime.Now.Hour)
            //        _scheduler.RunDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, _scheduler.SpecifiedFromTime.Value.Hours, 0, 0);
            //    else
            //        _scheduler.RunDate = DateTime.Now.AddHours(_scheduler.SpecifiedEveryHour.Value);
            //}
            //return _scheduler;
        }

        private void DefaultProcess(Process process)
        {
            //DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Background thread called without process information:", null, "Discovery",Convert.ToInt32( schedulerData.DiscoveryProcessId),0,"Scheduler Service");
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process process = e.Argument as Process;
            startProcess(process);
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

    }
}

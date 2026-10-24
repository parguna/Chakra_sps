using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data.SqlClient;
using System.Data;
using DI.Chakra.Infrastructure.Enums;
using System.Data.Entity.SqlServer;


namespace DI.Chakra.DataAccess
{
    public class SchedulerDA// : ISchedulerDA
    {
               
        public void UpdateScheduler(Scheduler _scheduler)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    Scheduler objScheduler = new Scheduler();
                    objScheduler = c.Scheduler.Find(_scheduler.Id);

                    if (objScheduler != null)
                    {
                        c.Entry(objScheduler).CurrentValues.SetValues(_scheduler);
                        c.SaveChanges();
                    }


                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<DiscoveryProcessHistory> GetLastDiscovery(int endpointSystemId)
        {

            IList<Scheduler> list = new List<Scheduler>();
            try
            {

                using (IACMDBContext c = new IACMDBContext())
                {

                    var discovery = c.DiscoveryProcessHistorys.Where(x => x.EndpointSystemId == endpointSystemId).ToList();
                    return discovery;

                }
  
            }
            catch (Exception)
            {
                throw;
            }
           

        }


        public IList<Scheduler> GetAllScheduler()
        {

            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    IList<Scheduler> list = new List<Scheduler>();
                    list = c.Scheduler.ToList();
                    return list;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        public Scheduler GetSchedulerById(int id)
        {

            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.Scheduler.Include("DiscoveryProcess").Where(sc => sc.Id == id).FirstOrDefault();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        public IList<Scheduler> GetCurrentJob()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    //IList<Scheduler> list = new List<Scheduler>();
                    //list = oIACMDBContext.Scheduler.ToList();
                    //var result = (from c in list
                    //             where (DateTime.Now.Subtract(Convert.ToDateTime(c.RunDate))).Minutes == 0 && (DateTime.Now.Subtract(Convert.ToDateTime(c.StartDate))).Days >= 0
                    //             select c).ToList();

                    //return result;

                    IList<Scheduler> list = new List<Scheduler>();
                    string query = "declare @dt datetime	set @dt=getdate()	select * from DI_M_Scheduler 	where DateDiff(mi,@dt,RunDate)=0 and DateDiff(dd,@dt,StartDate)<=0";

                    list = c.Scheduler.SqlQuery(query).ToList();
                    return list;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
       
        public void ChangeJobStatus()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    IList<DiscoveryProcess> listDiscoveryProcess = new List<DiscoveryProcess>();
                    IList<AttestationProcess> listAttestationProcess = new List<AttestationProcess>();

                    listDiscoveryProcess = c.DiscoveryProcess.ToList();
                    listAttestationProcess = c.AttestationProcesses.ToList();

                    //var DiscoveryProcessprogress = (from l in listDiscoveryProcess where l.Status == "InProgress" select l);
                    //var AttestationProcessprogress = (from l in listAttestationProcess where l.Status == "InProgress" select l);

                    //foreach (DiscoveryProcess dp in DiscoveryProcessprogress)
                    //{
                    //    DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                    //    objDiscoveryProcess = oIACMDBContext.DiscoveryProcess.Find(dp.DiscoveryProcessId);
                    //    objDiscoveryProcess.Status = "Failed";
                    //    objDiscoveryProcess.Message = "Service is stopped";
                    //}

                    //foreach (AttestationProcess ap in AttestationProcessprogress)
                    //{
                    //    AttestationProcess objAttestationProcess = new AttestationProcess();
                    //    objAttestationProcess = oIACMDBContext.AttestationProcesses.Find(ap.AttestationProcessId);
                    //    objAttestationProcess.Status = "Failed";
                    //    objAttestationProcess.WorkflowStatus = "Failed";
                    //}

                    c.SaveChanges();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<T> GetCurrentJobs<T>(ProcessType jobType)
        {
            List<T> list = new List<T>();
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    
                        DateTime @bod = DateTime.Now.Date;
                        DateTime @eod = DateTime.Now;
                        int i = (int)DateTime.Now.DayOfWeek;
                        string DayOfWeek = (i + 1).ToString();

                        #region RunNow
                        var RunNow_Query = from dp in c.Process.Include("Scheduler")
                                            join s in c.Scheduler
                                            on dp.Scheduler.Id equals s.Id
                                            where dp.ProcessType == jobType &&
                                            s.Occurence == Occurence.RunNow && dp.ProcessStartAt == null &&
                                            System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                            && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                            select dp;
                        var RunNowResultlist = RunNow_Query.Cast<T>().ToList();
                        if (RunNowResultlist.Count > 0)
                        {
                            list.AddRange(RunNowResultlist);
                        }

                        #endregion
                        #region OneTime

                        var OneTime_Query = from dp in c.Process.Include("Scheduler")
                                            join s in c.Scheduler
                                            on dp.Scheduler.Id  equals s.Id
                                            where dp.ProcessType == jobType &&
                                            s.Occurence == Occurence.OneTime && dp.ProcessStartAt == null &&
                                            System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                            && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                            select dp;
                        var OneTimeResultlist = OneTime_Query.Cast<T>().ToList();
                        if (OneTimeResultlist.Count > 0)
                        {
                            list.AddRange(OneTimeResultlist);
                        }

                        #endregion

                        #region Daily
                        var Daily_NewProcess_Query = from dp in c.Process
                                                     join s in c.Scheduler
                                                     on dp.Scheduler.Id equals s.Id
                                                     where dp.ProcessType == jobType &&
                                                     s.Occurence == Occurence.Daily && dp.ProcessStartAt == null &&
                                                     System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                     s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                                     && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                     s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                                     select dp;
                        var DailyNewProcessResultlist = Daily_NewProcess_Query.Cast<T>().ToList();
                        if (DailyNewProcessResultlist.Count > 0)
                        {
                            list.AddRange(DailyNewProcessResultlist);
                        }

                        var Daily_ExistingProcess_Query = from dp in c.Process
                                                          join s in c.Scheduler
                                                          on dp.Scheduler.Id equals s.Id
                                                          where dp.ProcessType == jobType &&
                                                          s.Occurence == Occurence.Daily && dp.ProcessStartAt != null &&
                                                          SqlFunctions.DateDiff("hh", dp.ProcessStartAt, DateTime.Now) >= 24 * s.ReccurEveryDays
                                                          select dp;

                        var DailyExistingProcessResultlist = Daily_ExistingProcess_Query.Cast<T>().ToList();
                        if (DailyExistingProcessResultlist.Count > 0)
                        {
                            list.AddRange(DailyExistingProcessResultlist);
                        }
                        #endregion

                        #region Weekly
                        var Weekly_NewProcess_Query = from dp in c.Process
                                                      join s in c.Scheduler
                                                      on dp.Scheduler.Id equals s.Id
                                                      where dp.ProcessType == jobType &&
                                                      s.Occurence == Occurence.Weekly && dp.ProcessStartAt == null &&
                                                      System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                      s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                                      && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                      s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                                      select dp;
                        var WeeklyNewProcessResultlist = Weekly_NewProcess_Query.Cast<T>().ToList();
                        if (WeeklyNewProcessResultlist.Count > 0)
                        {
                            list.AddRange(WeeklyNewProcessResultlist);
                        }

                        var Weekly_WeekDayNull_Query = from dp in c.Process
                                                       join s in c.Scheduler
                                                       on dp.Scheduler.Id equals s.Id
                                                       where dp.ProcessType == jobType &&
                                                       s.Occurence == Occurence.Weekly && dp.ProcessStartAt != null &&
                                                       SqlFunctions.DateDiff("hh", dp.ProcessStartAt, DateTime.Now) >= 168 * s.ReccurEveryDays
                                                       && s.WeekDay == null
                                                       select dp;

                        var WeeklyWeekDayNullResultlist = Weekly_WeekDayNull_Query.Cast<T>().ToList();
                        if (WeeklyWeekDayNullResultlist.Count > 0)
                        {
                            list.AddRange(WeeklyWeekDayNullResultlist);
                        }

                        var Weekly_WeekDayNotNull_Query = from dp in c.Process
                                                          join s in c.Scheduler
                                                          on dp.Scheduler.Id  equals s.Id
                                                          where dp.ProcessType == jobType &&
                                                          s.Occurence == Occurence.Weekly && dp.ProcessStartAt != null &&
                                                          (SqlFunctions.DatePart("weekday",dp.ProcessStartAt.Value) != SqlFunctions.DatePart("weekday",DateTime.Now) ||
                                                           System.Data.Entity.DbFunctions.CreateDateTime(dp.ProcessStartAt.Value.Year,
                                            dp.ProcessStartAt.Value.Month, dp.ProcessStartAt.Value.Day, 0, 0, 0) != System.Data.Entity.DbFunctions.CreateDateTime(DateTime.Now.Year,
                                            DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                                                          && s.WeekDay != null
                                                          && s.WeekDay.Contains(DayOfWeek)
                                                          select dp;

                        var WeeklyWeekDayNotNullResultlist = Weekly_WeekDayNotNull_Query.Cast<T>().ToList();
                        if (WeeklyWeekDayNotNullResultlist.Count > 0)
                        {
                            list.AddRange(WeeklyWeekDayNotNullResultlist);
                        }

                        #endregion

                        #region Monthly
                        var Monthly_NewProcess_Query = from dp in c.Process
                                                       join s in c.Scheduler
                                                       on dp.Scheduler.Id  equals s.Id
                                                       where dp.ProcessType == jobType 
                                                       && s.Occurence == Occurence.Monthly  && dp.ProcessStartAt == null &&
                                                       System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                       s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                                       && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                                       s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                                       select dp;
                        var MonthlyNewProcessResultlist = Monthly_NewProcess_Query.Cast<T>().ToList();
                        if (MonthlyNewProcessResultlist.Count > 0)
                        {
                            list.AddRange(MonthlyNewProcessResultlist);
                        }

                        var Monthly_ExistingProcess_Query = from dp in c.Process
                                                            join s in c.Scheduler
                                                            on dp.Scheduler.Id  equals s.Id
                                                            where dp.ProcessType == jobType 
                                                            && s.Occurence == Occurence.Monthly  && dp.ProcessStartAt != null &&
                                                            ((double?)SqlFunctions.DateDiff("dd", dp.ProcessStartAt, DateTime.Now)) / 30 >= 1 * s.ReccurEveryMonths
                                                            select dp;

                        var MonthlyExistingProcessResultlist = Monthly_ExistingProcess_Query.Cast<T>().ToList();
                        if (MonthlyExistingProcessResultlist.Count > 0)
                        {
                            list.AddRange(MonthlyExistingProcessResultlist);
                        }

                        #endregion
                   
                }
            }
            catch (Exception ex)
            {

            }
            //TODO: CHECK HARI
            //try
            //{
            //    using (oIACMDBContext = new IACMDBContext())
            //    {
            //        string query = string.Empty;

            //        if (jobType == ScheduleJobType.Attestation)
            //        {
            //            query = "declare @dt datetime	set @dt=getdate()	select * from DI_M_Scheduler 	where DateDiff(mi,@dt,RunDate)=0 and DateDiff(dd,@dt,StartDate)<=0 and AttestationProcessId is not null";
            //            return oIACMDBContext.Scheduler.SqlQuery(query).ToList();
            //        }
            //        else if (jobType == ScheduleJobType.Discovery)
            //        {

            //            query = "declare @dt datetime	set @dt=getdate()	select * from DI_M_Scheduler 	where DateDiff(mi,@dt,RunDate)=0 and DateDiff(dd,@dt,StartDate)<=0 and RunEveryDay=0 and isactive=1 and DiscoveryProcessId is not null";
            //            List<Scheduler> CurrentJobsList = oIACMDBContext.Scheduler.SqlQuery(query).ToList();
            //            TimeSpan CurrentTime = DateTime.Now.TimeOfDay;
            //            var EveryDayJobs = oIACMDBContext.Scheduler
            //                                                    .Where(sch => sch.RunEveryDay == true && sch.IsActive == true);

            //            string WeekDay = DateTime.Now.DayOfWeek.ToString();
            //            var WeeklyJobs = oIACMDBContext.Scheduler
            //                                            .Where(sch => sch.RunEveryWeek == true && sch.RunEveryWeekDay == WeekDay && sch.IsActive == true);


            //            List<Scheduler> CurrentJobs = EveryDayJobs.Union(WeeklyJobs).ToList();

            //            CurrentJobs = CurrentJobs.Where(jobs => Math.Abs((jobs.StartDate.Value.TimeOfDay.TotalSeconds - CurrentTime.TotalSeconds)) <= 60)
            //                                                    .ToList();

            //            List<Scheduler> TotalCurrentJobs = CurrentJobsList.Union(CurrentJobs).ToList();

            //            return TotalCurrentJobs;
            //        }

            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return list;
        }

    }
}

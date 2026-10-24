using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System.Data.Entity.SqlServer;

namespace DI.Chakra.Repository
{
    public class ProcessRepository : RepositoryBase<Process>
    {
        public ProcessRepository(DataAccessTransaction transaction) : base(transaction) { }



        public List<Process> GetCurrentJobs(ProcessType processType)
        {
            List<Process> list = new List<Process>();
            DateTime @bod = DateTime.Now.Date;
            DateTime @eod = DateTime.Now;
            int i = (int)DateTime.Now.DayOfWeek;
            string DayOfWeek = (i).ToString();

            #region RunNow
            var RunNow_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                join s in CurrentContext.Scheduler
                                on dp.Scheduler.Id equals s.Id
                                where dp.ProcessType == processType &&
                                s.Occurence == Occurence.RunNow && dp.ProcessStartAt == null &&
                                System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                select dp;
            var RunNowResultlist = RunNow_Query.Cast<Process>().ToList();
            if (RunNowResultlist.Count > 0)
            {
                list.AddRange(RunNowResultlist);
            }

            #endregion
            #region OneTime

            var OneTime_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                join s in CurrentContext.Scheduler
                                on dp.Scheduler.Id equals s.Id
                                where dp.ProcessType == processType &&
                                s.Occurence == Occurence.OneTime && dp.ProcessStartAt == null &&
                                System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                select dp;
            var OneTimeResultlist = OneTime_Query.Cast<Process>().ToList();
            if (OneTimeResultlist.Count > 0)
            {
                list.AddRange(OneTimeResultlist);
            }

            #endregion

            #region Daily
            var Daily_NewProcess_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                         join s in CurrentContext.Scheduler
                                            on dp.Scheduler.Id equals s.Id
                                            where dp.ProcessType == processType &&
                                            s.Occurence == Occurence.Daily && dp.ProcessStartAt == null &&
                                            System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                            && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                            select dp;
            var DailyNewProcessResultlist = Daily_NewProcess_Query.Cast<Process>().ToList();
            if (DailyNewProcessResultlist.Count > 0)
            {
                list.AddRange(DailyNewProcessResultlist);
            }

            var Daily_ExistingProcess_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                              join s in CurrentContext.Scheduler
                                                on dp.Scheduler.Id equals s.Id
                                                where dp.ProcessType == processType &&
                                                s.Occurence == Occurence.Daily && dp.ProcessStartAt != null &&
                                                SqlFunctions.DateDiff("MINUTE", System.Data.Entity.DbFunctions.CreateDateTime(
                                                    dp.ProcessStartAt.Value.Year, dp.ProcessStartAt.Value.Month, dp.ProcessStartAt.Value.Day,
                                                    s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds), DateTime.Now) >= 1440 * s.ReccurEveryDays // 1440 minutes = 24 hrs * 60 minutes (1 day in minutes) 
                                                select dp;

            var DailyExistingProcessResultlist = Daily_ExistingProcess_Query.Cast<Process>().ToList();
            if (DailyExistingProcessResultlist.Count > 0)
            {
                list.AddRange(DailyExistingProcessResultlist);
            }
            #endregion

            #region Weekly
            var Weekly_NewProcess_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                          join s in CurrentContext.Scheduler
                                            on dp.Scheduler.Id equals s.Id
                                            where dp.ProcessType == processType &&
                                            s.Occurence == Occurence.Weekly && dp.ProcessStartAt == null &&
                                            System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                            && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                            select dp;
            var WeeklyNewProcessResultlist = Weekly_NewProcess_Query.Cast<Process>().ToList();
            if (WeeklyNewProcessResultlist.Count > 0)
            {
                list.AddRange(WeeklyNewProcessResultlist);
            }

            var Weekly_WeekDayNull_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                           join s in CurrentContext.Scheduler
                                            on dp.Scheduler.Id equals s.Id
                                            where dp.ProcessType == processType &&
                                            s.Occurence == Occurence.Weekly && dp.ProcessStartAt != null &&
                                            SqlFunctions.DateDiff("MINUTE", System.Data.Entity.DbFunctions.CreateDateTime(
                                                    dp.ProcessStartAt.Value.Year, dp.ProcessStartAt.Value.Month, dp.ProcessStartAt.Value.Day,
                                                    s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds), DateTime.Now) >= 10080 * s.ReccurEveryWeeks // 10080 = 7 days * 24 hrs * 60 minutes (1 week in minutes)
                                            && (s.WeekDay == null || s.WeekDay == "")
                                            select dp;

            var WeeklyWeekDayNullResultlist = Weekly_WeekDayNull_Query.Cast<Process>().ToList();
            if (WeeklyWeekDayNullResultlist.Count > 0)
            {
                list.AddRange(WeeklyWeekDayNullResultlist);
            }

            var Weekly_WeekDayNotNull_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                              join s in CurrentContext.Scheduler
                                                on dp.Scheduler.Id equals s.Id
                                                where dp.ProcessType == processType &&
                                                s.Occurence == Occurence.Weekly && dp.ProcessStartAt != null &&
                                                (SqlFunctions.DatePart("weekday", dp.ProcessStartAt.Value) != SqlFunctions.DatePart("weekday", DateTime.Now) ||
                                                System.Data.Entity.DbFunctions.CreateDateTime(dp.ProcessStartAt.Value.Year,
                                dp.ProcessStartAt.Value.Month, dp.ProcessStartAt.Value.Day, 0, 0, 0) != System.Data.Entity.DbFunctions.CreateDateTime(DateTime.Now.Year,
                                DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)) &&
                                SqlFunctions.DateDiff("MINUTE", System.Data.Entity.DbFunctions.CreateDateTime(
                                                    dp.ProcessStartAt.Value.Year, dp.ProcessStartAt.Value.Month, dp.ProcessStartAt.Value.Day,
                                                    s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds), DateTime.Now) >= 10080 * s.ReccurEveryWeeks // 10080 = 7 days * 24 hrs * 60 minutes (1 week in minutes)
                                                && s.WeekDay != null
                                                && s.WeekDay.Contains(DayOfWeek)
                                                select dp;

            var WeeklyWeekDayNotNullResultlist = Weekly_WeekDayNotNull_Query.Cast<Process>().ToList();
            if (WeeklyWeekDayNotNullResultlist.Count > 0)
            {
                list.AddRange(WeeklyWeekDayNotNullResultlist);
            }

            #endregion

            #region Monthly
            var Monthly_NewProcess_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                           join s in CurrentContext.Scheduler
                                            on dp.Scheduler.Id equals s.Id
                                            where dp.ProcessType == processType
                                            && s.Occurence == Occurence.Monthly && dp.ProcessStartAt == null &&
                                            System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) >= @bod
                                            && System.Data.Entity.DbFunctions.CreateDateTime(s.StartDate.Year,
                                            s.StartDate.Month, s.StartDate.Day, s.StartTime.Hours, s.StartTime.Minutes, s.StartTime.Seconds) <= @eod
                                            select dp;
            var MonthlyNewProcessResultlist = Monthly_NewProcess_Query.Cast<Process>().ToList();
            if (MonthlyNewProcessResultlist.Count > 0)
            {
                list.AddRange(MonthlyNewProcessResultlist);
            }

            var Monthly_ExistingProcess_Query = from dp in CurrentContext.Process.Include("Scheduler")
                                                join s in CurrentContext.Scheduler
                                                on dp.Scheduler.Id equals s.Id
                                                where dp.ProcessType == processType
                                                && s.Occurence == Occurence.Monthly && dp.ProcessStartAt != null &&
                                                ((double?)SqlFunctions.DateDiff("dd", dp.ProcessStartAt, DateTime.Now)) / 30 >= 1 * s.ReccurEveryMonths
                                                select dp;

            var MonthlyExistingProcessResultlist = Monthly_ExistingProcess_Query.Cast<Process>().ToList();
            if (MonthlyExistingProcessResultlist.Count > 0)
            {
                list.AddRange(MonthlyExistingProcessResultlist);
            }
            #endregion
            return list;
        }
    }
}

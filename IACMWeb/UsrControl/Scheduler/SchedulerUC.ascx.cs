using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Scheduler
{
    /// <summary>
    /// Created Seperate User Control for Scheduler in order to achieve loose coupling.
    /// Later we can attach this User Control to Discovery Process,Compliance,Provisioning etc.
    /// Author : Harihara 6/7/2015.
    /// </summary>
    public partial class SchedulerUC : UIBaseUC
    {
        private Dto.Scheduler _SchedulerConfig = null;
        private bool isEdit = false;
        public DateTime scheduledDate;
        public void InitilizePage()
        {
            pnlDaily.Visible = false;
            pnlWeekly.Visible = false;
            pnlMonthly.Visible = false;
            //rblOneTime.Checked = true;
            if (_SchedulerConfig != null)
            {
                DateTime dt = _SchedulerConfig.StartDate.Add(_SchedulerConfig.StartTime);
                dtStartDate.MinDate = dt < DateTime.Now ? dt : DateTime.Now; //dt  min value should be current date and not start date;
                dtStartDate.SelectedDate = dt;
                switch (_SchedulerConfig.Occurence.Value)
                {
                    case Occurence.OneTime:
                        rblOneTime.Checked = true;
                        SetOneTimeVisibility();
                        break;
                    case Occurence.Daily:
                        rblDaily.Checked = true;
                        SchedulerDailyUC.RecurDay = _SchedulerConfig.ReccurEveryDays.ToString();
                        SetDailyVisibility();
                        break;
                    case Occurence.Monthly:
                        rblMonthly.Checked = true;
                        SchedulerMonthlyUC.ReccurMonths = _SchedulerConfig.ReccurEveryMonths.ToString();
                        SetMonthlyVisibility();
                        break;
                    case Occurence.Weekly:
                        rblWeekly.Checked = true;
                        SchedulerWeeklyUC.RecurWeeks = _SchedulerConfig.ReccurEveryWeeks.ToString();
                        SchedulerWeeklyUC.SetWeekDays(_SchedulerConfig.WeekDay);
                        SetWeeklyVisibility();
                        break;
                    case Occurence.RunNow:
                        rblRunNow.Checked = true;
                        SetOneTimeVisibility();
                        break;
                }
            }
            else
            {
                rblOneTime.Checked = true;
                dtStartDate.MinDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            }
            SchedulerMonthlyUC.InitilizePage();
        }
        public Dto.Scheduler SchedulerConfig
        {
            set { _SchedulerConfig = value; }
            get { return _SchedulerConfig; }
        }
        public bool IsEdit
        {
            set { isEdit = value; }
            get { return isEdit; }
        }

        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        private void SetOccurenceParameters(Dto.Scheduler scheduler)
        {
            if (rblOneTime.Checked)
            {
                scheduler.Occurence = Occurence.OneTime;
            }
            else if (rblDaily.Checked)
            {
                scheduler.Occurence = Occurence.Daily;
                scheduler.ReccurEveryDays = Convert.ToInt32(SchedulerDailyUC.RecurDay);
            }
            else if (rblWeekly.Checked)
            {
                scheduler.Occurence = Occurence.Weekly;
                scheduler.ReccurEveryWeeks = Convert.ToInt32(SchedulerWeeklyUC.RecurWeeks);
                scheduler.WeekDay = SchedulerWeeklyUC.GetCheckedWeekDays();
            }
            else if (rblMonthly.Checked)
            {
                scheduler.Occurence = Occurence.Monthly;
                scheduler.ReccurEveryMonths = Convert.ToInt32(SchedulerMonthlyUC.ReccurMonths);
            }
            else if (rblRunNow.Checked)
            {
                scheduler.Occurence = Occurence.RunNow;
            }
        }
        public DateTime? SelectedDate
        {
            get
            {
                if (!rblRunNow.Checked)
                    return dtStartDate.SelectedDate;
                else
                    return DateTime.Now;
            }
        }
        public Dto.Scheduler getScheduler()
        {
            Dto.Scheduler scheduler = new Dto.Scheduler();
            SessionStatusImpl session = new SessionStatusImpl();
            if (rblOneTime.Checked && dtStartDate.SelectedDate == null)
            {
                Session["StartDate"] = 0;
                return null;
            }
            else
            {
                Session["StartDate"] = 1;
            }

            if (!rblRunNow.Checked)
            {
                scheduler.StartDate = dtStartDate.SelectedDate == null ? DateTime.Now : dtStartDate.SelectedDate.Value.Date;
                DateTime time = dtStartDate.SelectedDate == null ? scheduler.StartDate : dtStartDate.SelectedDate.Value;
                scheduler.StartDate = dtStartDate.SelectedDate == null ? DateTime.Now.Date : dtStartDate.SelectedDate.Value.Date;
                scheduler.StartTime = new TimeSpan(time.Hour, time.Minute, time.Second);
            }
            else
            {
                scheduler.StartDate = DateTime.Now.Date;
                scheduler.StartTime = new TimeSpan(DateTime.Now.Hour, (DateTime.Now.Minute + 1), DateTime.Now.Second);
            }
            scheduledDate = scheduler.StartDate;
            SetOccurenceParameters(scheduler);
            return scheduler;
        }

        public bool WeekDaysvalidation()
        {
            bool flag = true;
            if (rblWeekly.Checked)
            {
                flag = SchedulerWeeklyUC.ValidateScreen();
            }
            else
            {
                return flag = true;
            }
            return flag;
        }

        protected void rblDaily_CheckedChanged(object sender, EventArgs e)
        {
            if (rblDaily.Checked)
            {
                SetDailyVisibility();
            }
        }

        protected void rblWeekly_CheckedChanged(object sender, EventArgs e)
        {
            if (rblWeekly.Checked)
            {
                SetWeeklyVisibility();
            }
        }

        protected void rblMonthly_CheckedChanged(object sender, EventArgs e)
        {
            if (rblMonthly.Checked)
            {
                SetMonthlyVisibility();
            }
        }
        protected void rblOneTime_CheckedChanged(object sender, EventArgs e)
        {
            if (rblOneTime.Checked)
            {
                SetOneTimeVisibility();
            }
        }

        private void SetOneTimeVisibility()
        {
            pnlDaily.Visible = false;
            pnlWeekly.Visible = false;
            pnlMonthly.Visible = false;
            dtStartDate.Enabled = true;
        }

        private void SetMonthlyVisibility()
        {
            pnlDaily.Visible = false;
            pnlWeekly.Visible = false;
            pnlMonthly.Visible = true;
            dtStartDate.Enabled = true;
        }

        private void SetWeeklyVisibility()
        {
            pnlDaily.Visible = false;
            pnlWeekly.Visible = true;
            pnlMonthly.Visible = false;
            dtStartDate.Enabled = true;
        }

        private void SetDailyVisibility()
        {
            pnlDaily.Visible = true;
            pnlWeekly.Visible = false;
            pnlMonthly.Visible = false;
            dtStartDate.Enabled = true;
        }
        protected void rblRunNow_CheckedChanged(object sender, EventArgs e)
        {
            if (rblRunNow.Checked)
            {
                SetOneTimeVisibility();
                dtStartDate.Enabled = false;
            }
        }
    }
}
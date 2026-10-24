using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Scheduler
{
    public partial class SchedulerWeekly : DI.Chakra.Web.UIBaseUC
    {
        public int dayscount = 0;
        public string RecurWeeks
        {
            get
            {
                return txtRecurWeeks.Text;
            }
            set
            {
                txtRecurWeeks.Text = value;
            }
        }

        public int WeeklyDaysCount
        {
            set
            {
                dayscount = value;
            }
        }
        public bool ValidateScreen()
        {
             bool flag = false;
             int chkcount = 0; int recurcount = 0;

             if ((txtRecurWeeks.Text!=null)&&(txtRecurWeeks.Text!=""))
             {
                 recurcount=Convert.ToInt32(txtRecurWeeks.Text);
             }

             if (chkSunday.Checked == true)
             {
                 chkcount++;
             }
             if (chkMonday.Checked == true)
             {
                 chkcount++;
             }
             if (chkTuesday.Checked == true)
             {
                 chkcount++;
             }
             if (chkWednesday.Checked == true)
             {
                 chkcount++;
             }
             if (chkThursday.Checked == true)
             {
                 chkcount++;
             }
             if (chkFriday.Checked == true)
             {
                 chkcount++;
             }
             if (chkSaturday.Checked == true)
             {
                 chkcount++;
             }
             if (recurcount == chkcount) return flag = true;
             else return flag = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidateScreen();
        }
        public void SetWeekDays(string WeekDay)
        {
            if (WeekDay != "")
            {
                string[] weekDays = WeekDay.Split(',');
                if (weekDays.Length > 0)
                {
                    foreach (string day in weekDays)
                    {
                        switch (day)
                        {
                            case "0":
                                chkSunday.Checked = true;
                                break;
                            case "1":
                                chkMonday.Checked = true;
                                break;
                            case "2":
                                chkTuesday.Checked = true;
                                break;
                            case "3":
                                chkWednesday.Checked = true;
                                break;
                            case "4":
                                chkThursday.Checked = true;
                                break;
                            case "5":
                                chkFriday.Checked = true;
                                break;
                            case "6":
                                chkSaturday.Checked = true;
                                break;

                        }
                    }
                }
            }
        }
        public string GetCheckedWeekDays()
        {
            string strWeekText = "";
            string delimitter = "";
            if (chkSunday.Checked)
            {
                strWeekText = strWeekText + delimitter + "0";
                delimitter = ",";
            }
            if (chkMonday.Checked)
            {
                strWeekText = strWeekText + delimitter + "1";
                delimitter = ",";
            }
            if (chkTuesday.Checked)
            {
                strWeekText = strWeekText + delimitter + "2";
                delimitter = ",";
            }
            if (chkWednesday.Checked)
            {
                strWeekText = strWeekText + delimitter + "3";
                delimitter = ",";
            }
            if (chkThursday.Checked)
            {
                strWeekText = strWeekText + delimitter + "4";
                delimitter = ",";
            }
            if (chkFriday.Checked)
            {
                strWeekText = strWeekText + delimitter + "5";
                delimitter = ",";
            }
            if (chkSaturday.Checked)
            {
                strWeekText = strWeekText + delimitter + "6";
                delimitter = ",";
            }
            return strWeekText;
        }
    }
}
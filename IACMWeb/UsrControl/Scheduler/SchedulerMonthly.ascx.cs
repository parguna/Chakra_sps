using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Scheduler
{
    public partial class SchedulerMonthly : DI.Chakra.Web.UIBaseUC
    {
        public void InitilizePage()
        {
            //LoadMonths();
            //LoadDays();
            //LoadWeeks();
        }
        public string ReccurMonths
        {
            get
            {
                return txtRecurMonth.Text;
            }
            set
            {
                txtRecurMonth.Text = value;
            }
        }
    }
}
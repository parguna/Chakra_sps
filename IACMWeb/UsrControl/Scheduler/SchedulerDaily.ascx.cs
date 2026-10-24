using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Scheduler
{
    public partial class SchedulerDaily : DI.Chakra.Web.UIBaseUC
    {
        public string RecurDay
        {
            get
            {
                return txtRecurDay.Text;
            }
            set
            {
                txtRecurDay.Text = value;
            }
        }
    }
}
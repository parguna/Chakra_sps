using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Master.UC
{
    public partial class NFooter : System.Web.UI.UserControl
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblLastLogin.Text = sessionStatus.User.LastLogonDate.ToString();
            }
        }
    }
}
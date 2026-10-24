using Application;
using System;
using System.Reflection;

namespace DI.Chakra.Web.Master.UC
{
    public partial class Footer : DI.Chakra.Web.UIBaseUC
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblLastLogin.Text = sessionStatus.User.LastLogonDate.ToString();
                //lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }            
        }
    }
}

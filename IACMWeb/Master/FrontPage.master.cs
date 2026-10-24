using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Master
{
    public partial class FrontPage : System.Web.UI.MasterPage
    {
        AppTextSource appTextSource = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            appTextSource = AppTextSource.Instance;

            if (!IsPostBack)
            {
                imgLogo.ImageUrl = string.Format("~/Images/Client/{0}", T("App.Branding.Logo"));
            }
        }
        public string T(string key)
        {
            return appTextSource.GetString(key);
        }

        public void ShowAlert(AlertType AlertType, string Message)
        {
            AlertMessage.AlertType = AlertType;
            AlertMessage.Message = Message;
            AlertMessageUpdatePanel.Update();
            AlertMessage.Show();
        }
    }
}
using DI.Chakra.Infrastructure;
using System;
using System.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class MessageBox : DI.Chakra.Web.UIBaseUC
    {
        public string Message { get; set; }
        public AlertType AlertType { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Show()
        {
            var title = Convert.ToString(AlertType);
            myModalLabel.InnerText = title;
            idMessageContent.Text = System.Web.HttpUtility.HtmlEncode(Message);
            IDMesssage.CssClass = title.ToLower();
            IDMesssage.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + IDMesssage.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            //btnOk.Focus();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            IDMesssage.VisibleOnPageLoad = false;
        }
    }
}
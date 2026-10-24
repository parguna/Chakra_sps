using DI.Chakra.Infrastructure;
using System;
using System.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ConfirmBox : DI.Chakra.Web.UIBaseUC
    {
        public string Message { get; set; }
        public AlertType AlertType { get; set; }
        public event EventHandler ConfirmYesClick;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Show()
        {
            myModalLabel.InnerText = "Confirm";
            idMessageContent.Text = Message;
            IDMesssage.CssClass = "error";
            IDMesssage.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + IDMesssage.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            //btnOk.Focus();
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            IDMesssage.VisibleOnPageLoad = false;
            ConfirmYesClick(sender, e);
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            IDMesssage.VisibleOnPageLoad = false;
        }
    }
}
using System;
using System.Web.UI;
using DI.Chakra.Infrastructure;


namespace DI.Chakra.Web.UserControls
{
    public partial class AlertMessage : DI.Chakra.Web.UIBaseUC
    {
        public AlertType AlertType { get; set; }
        public string Message { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                message.Style["display"] = "none";
            }
        }

        public void Show()
        {
            //if (!Page.Request.Url.LocalPath.Contains("Login.aspx"))
            //{
            //    TextBox focus = new TextBox();
            //    focus.ID = "focus";
            //    focus.Attributes.Add("onfocusout", "return myFunction();");
            //    focus.Attributes.Add("onmousewheel", "return myFunction();");
            //    TextBox isControlExist = (TextBox)message.FindControl("focus");
            //    if (isControlExist == null)
            //    {
            //        message.Controls.Add(focus);
            //        focus.Focus();
            //    }
            //}
            lblMessage.Text = this.Message;
            if (lblMessage.Text != "")
            {
                message.Style["display"] = "block";
            }
            else 
            {
                message.Style["display"] = "none";
            }

            if (this.AlertType == DI.Chakra.Infrastructure.AlertType.Error)
            {
                lblTitle.Text = "Error";
                innermessage.Attributes["class"] = "alert alert-danger col-md-3";
            }
            else if (this.AlertType == DI.Chakra.Infrastructure.AlertType.Warning)
            {
                lblTitle.Text = "Warning";
                innermessage.Attributes["class"] = "alert alert-warning col-md-3";
            }
            else
            {
                lblTitle.Text = "Success";
                innermessage.Attributes["class"] = "alert alert-success col-md-3";
            }
        }
       
    }
}

        
    

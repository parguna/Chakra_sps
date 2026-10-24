using System;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Non_Employee
{
    public partial class Add : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddForm.Initialize();
            }
            AddForm.HideOrgChartLink();
            AddForm.ShowPasswordPolicyButton(false);
            this.MasterPage.ActionClicked += ActionClicked;
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            bool isErrorOccured = AddForm.SaveUser();
            if (!isErrorOccured)
            {
                //MasterPage.ShowAlert(AlertType.Success, "Non Employee created successfully");
                // Page.ClientScript.RegisterStartupScript(this.GetType(), "PageScript", "Redirect();", true);
                Response.Redirect("~/NonEmployee/List.aspx");
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NonEmployee/List.aspx");
        }
    }
}
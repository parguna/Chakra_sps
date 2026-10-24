using System;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Employee
{
    public partial class Add : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddForm.IsFullTimeEmployee = true;
                AddForm.Initialize();
            }
            AddForm.HideOrgChartLink();
            AddForm.ShowPasswordPolicyButton(false);
            this.MasterPage.ActionClicked += ActionClicked;
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                bool isErrorOccured = AddForm.SaveUser();
                //if (!isErrorOccured)
                //{
                //    MasterPage.ShowAlert(AlertType.Success, "Employee created successfully");
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), "PageScript", "Redirect();", true);
                //    Response.Redirect("List.aspx");
                //}

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx");
        }
    }
}
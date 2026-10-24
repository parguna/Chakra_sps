using System;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Employee
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EditForm.IsEdit = true;
                EditForm.IsFullTimeEmployee = true;
                EditForm.Initialize();
            }
            EditForm.HideOrgChartLink();
            this.MasterPage.ActionClicked += ActionClicked;
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                bool isErrorOccured = EditForm.SaveUser();
                if (!isErrorOccured)
                {
                    MasterPage.ShowAlert(AlertType.Success, "Employee updated successfully");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "PageScript", "Redirect();", true);
                    Response.Redirect("List.aspx");
                }
                    
            }
        }
    }
}
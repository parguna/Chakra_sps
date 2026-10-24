using System;

namespace DI.Chakra.Web.Admin.AppText
{
    public partial class List : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }
    }
}
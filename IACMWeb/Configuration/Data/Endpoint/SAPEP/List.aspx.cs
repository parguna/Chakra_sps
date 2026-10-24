using System;

namespace DI.Chakra.Web.Configuration.Data.XLUpload.SAPEP
{
    public partial class List : UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}
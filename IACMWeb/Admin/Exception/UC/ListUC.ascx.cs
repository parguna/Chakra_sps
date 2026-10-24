using System;

namespace DI.Chakra.Web.Admin.Exception.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}
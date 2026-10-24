using System;

namespace DI.Chakra.Web.Delegation.UC
{
    public partial class DetailUC : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Delegation/List.aspx");
        }
    }
}
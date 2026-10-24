using System;
namespace DI.Chakra.Web.Account.UC
{
    public partial class DetailUC : UIBaseUC
    {
        protected void btnOk_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/List.aspx");
        }
    }
}
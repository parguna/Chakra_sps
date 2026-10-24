using System;
using System.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App
{
    public partial class CartItems : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        public void Show()
        {
            CartItemUC.Initialize();
            CartItem.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + this.CartItem.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            CartItem.VisibleOnPageLoad = false;
        }
    }
}
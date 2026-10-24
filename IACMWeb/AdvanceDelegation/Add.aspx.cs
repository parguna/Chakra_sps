using System;

namespace DI.Chakra.Web.AdvanceDelegation
{
    public partial class Add : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddForm.Initialize();
            }
        }
    }
}
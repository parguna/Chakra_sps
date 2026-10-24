using System;

namespace DI.Chakra.Web.AdvanceDelegation
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EditForm.EntityId = ContextObjectId == null ? 0 : Convert.ToInt64(ContextObjectId);
                EditForm.Initialize();
            }
        }
    }
}
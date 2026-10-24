using System;

namespace DI.Chakra.Web.Non_Employee
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session != null && ContextObjectId != null)
                {
                    DetailUC.EntityId = Convert.ToInt64(ContextObjectId);
                    DetailUC.ShowAccess = true;
                }
            }
        }
    }
}
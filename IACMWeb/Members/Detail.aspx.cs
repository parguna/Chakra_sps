using System;

namespace DI.Chakra.Web.Members
{
    public partial class Detail : UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session != null && ContextObjectId != null)
                {
                    EmployeeAccountUC.EntityId = Convert.ToInt64(ContextObjectId);
                    
                }
            }
        }
    }
}
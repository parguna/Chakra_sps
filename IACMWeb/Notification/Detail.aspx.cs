using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Notification
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        SessionStatusImpl _m_session = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            _m_session.RedirectFrom = "MyNotifications";
            if (Request.QueryString["Id"] != null)
            {
                var id = Request.QueryString["Id"].ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    ContextObjectId = Convert.ToInt64(id);
                }
            }
        }
    }
}
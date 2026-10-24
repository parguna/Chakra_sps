using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.PUB
{
    public partial class List : DI.Chakra.Web.UIBasePage
    {
         

        SessionStatusImpl _m_session = new SessionStatusImpl();

        protected void Page_Load(object sender, EventArgs e)
        {
            _m_session.RedirectFrom = "PUB Files";
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.ListUC.Refresh();
        }

    }
}
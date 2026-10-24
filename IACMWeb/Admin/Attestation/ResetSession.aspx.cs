using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class ResetSession : B5UIBasePage, IContextObjectRequired
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionUser.User == null)
            {
                Response.Write(false);
                Response.End();
            }
            else
            {
                Response.Write(true);
                Response.End();
            }
        }
    }
}
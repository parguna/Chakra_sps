using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Prov
{
    public partial class SubmitFailedAdhocRequest : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var _sessionStatus = new SessionStatusImpl();
            _sessionStatus.RedirectFrom = "SubmitFailedAdhocRequest";
        }
    }
}
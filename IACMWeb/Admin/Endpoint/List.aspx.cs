using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.Endpoint
{
    public partial class List : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Title = T("App.Common.Endpoint") + " List";
            }
        }
    }
}
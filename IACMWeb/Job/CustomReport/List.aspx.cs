using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Job.CustomReport
{
    public partial class List : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ListUC.Refresh();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Job.Compliance
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ComplianceWizardUC.InitilizePage(true, false);
            }
        }
    }
}
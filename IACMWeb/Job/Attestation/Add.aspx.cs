using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DI.Chakra.Web.Job.Attestation
{
    public partial class Add : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SchedulerWizardUC.InitializePage(false,false);
            }
        }
    }
}
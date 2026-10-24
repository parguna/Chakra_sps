using System;
using System.Web.UI;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AttestationWizardUC.InitializeControl();
            }
        }
    }
}
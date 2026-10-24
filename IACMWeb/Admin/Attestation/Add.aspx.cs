using System;
using System.Web.UI;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class Add : DI.Chakra.Web.UIBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AttestationWizardUC.InitializeControl(); ;
            }
        }
    }
}
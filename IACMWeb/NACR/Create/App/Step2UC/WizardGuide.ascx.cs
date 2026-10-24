using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Provisioning_adhocuc_WizardGuide : System.Web.UI.UserControl
{
    public delegate void NextHandler();
    public event NextHandler NextClicked;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnStartWizard_Click(object sender, EventArgs e)
    {
        NextClicked();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {

    }
}
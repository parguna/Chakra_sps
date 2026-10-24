using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.PasswordMgmt.Forgot
{
    public partial class SecurityQuestion : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                SetQuestion();

            }
        }

        private void SetQuestion()
        {
            SecurityQA.EntityId = 1;
        }

    }
}
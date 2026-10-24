using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Comp.PolicyExecution
{
    public partial class Edit : UIBasePage, IContextObjectRequired
    {
        #region Page Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //{
            //    PolicyExecutionWizard.InitilizePage(true, false);
            //}
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Comp.Exception
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        #region Page Events
        /// <summary>
        /// Intializing ExpWizard in View Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //ExpDet.InitializePage(false,true);

        }
        #endregion
    }
}
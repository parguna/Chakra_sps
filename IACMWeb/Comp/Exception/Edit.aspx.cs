
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Exception
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        #region Page Events
        /// <summary>
        /// Intializing Exception Wizard in Edit Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //ExpEditId.InitializePage(true, false);

        }
        #endregion
    }
}
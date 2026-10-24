using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Master
{
    public partial class ExportStatusMoniter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod]
        public static string GetExportStatus()
        {
            return Convert.ToString(HttpContext.Current.Session["ExportStatus"]);
        }
    }
}
using System;
using System.Web;
using System.Web.Services;

namespace DI.Chakra.Web.NACR
{
    public partial class CreateCart : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod(EnableSession = true)]
        public static string SetModuleIDSubModule(string subModuleID)
        {
            HttpContext.Current.Session["SubModuleID"] = subModuleID;
            return "";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DI.Chakra.DomainApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            DomainModel.AutoMapperHelper.BootStrap();

            AutoMapperHelper.BootStrap();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            DomainModel.UILog uilog = new DomainModel.UILog(HttpContext.Current.Server.GetLastError().InnerException);
            DomainModel.UILog.SaveUILog(uilog, User.Identity.Name);
        }
    }
}

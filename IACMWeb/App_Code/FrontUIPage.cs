using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Application;
using DI.Chakra.Web.Master;

namespace DI.Chakra.Web
{
    public class FrontUIPage : System.Web.UI.Page
    {
        AppTextSource appTextSource = null;
        SessionStatusImpl sessionStatus = new SessionStatusImpl();

        public FrontUIPage()
        {
            appTextSource = AppTextSource.Instance;
        }

        public string T(string key)
        {
            return appTextSource.GetString(key);
        }


        public FrontPage MasterPage
        {

            get
            {
                return (FrontPage)Page.Master;
            }
        }
       
    }
}
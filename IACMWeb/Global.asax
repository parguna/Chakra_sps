<%@ Application Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="DI.Chakra.DBConfigHelper" %>
<%@ Import Namespace="DI.Chakra.Dto" %>
<%@ Import Namespace="DI.Chakra.Logger" %>
<%@ Import Namespace="DI.Chakra.Web" %>
<%@ Import Namespace="DI.Chakra.Infrastructure" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        if(AppSettingsHelper.EnableBundling == "1")
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);  

        DI.Chakra.Infrastructure.AppTextLocation.Path = Server.MapPath("~/assets/apptexts/AppTexts.xml");

        AppSettingsHelper.IsFirstTimeDBExecute = true;

        try
        {
            UILog log = new UILog();

            log.StackTrace = "Application Started";
            log.LogDate = DateTime.Now;
            log.LogLevel = "ApplicationStarted";
            log.UserName = "System";
            DomainApiClient<UILog>.GetByAction<object>(DomainApiAction.Log.UILog.SaveUILog, null, log);
        }
        catch (Exception)
        {
            // Application_Start runs once per AppDomain and is never retried; a failure here
            // (e.g. the backend being briefly unreachable at startup) must not permanently
            // fail every request for the lifetime of the app pool. This log write is best-effort.
        }

        AppSettingsHelper.IsFirstTimeDBExecute = false;
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {


        Application["error"] = HttpContext.Current.Server.GetLastError().InnerException;
        Exception ex = HttpContext.Current.Server.GetLastError().InnerException;
        UILog log = new UILog();
        if (null != ex)
        {
            log.ExceptionMessage = ex.Message;
            log.ExceptionSource = ex.Source;
            log.StackTrace = ex.StackTrace;
            if (null != ex.InnerException)
                log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                    ex.InnerException.StackTrace, ex.InnerException.Message);
        }
        log.LogDate = DateTime.Now;
        log.LogLevel = "Error";
        if (System.Web.HttpContext.Current.Session != null)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.Session["LoginUserId"]);
            log.UserId = userId;
            log.UserName = Convert.ToString(System.Web.HttpContext.Current.Session["LoginUserName"]);
        }
        // DomainApiClient<UILog>.Save(log);
        DomainApiClient<UILog>.GetByAction<object>(DomainApiAction.Log.UILog.SaveUILog, null, log);
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Session["Culture"] = null;

    }

    void Session_End(object sender, EventArgs e)
    {
#if DEBUG == false
        System.Web.Security.FormsAuthentication.SignOut();
        System.Web.Security.FormsAuthentication.RedirectToLoginPage();
#endif
    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        //Response.Cache.SetCacheability(HttpCacheability.Public);
        //Response.Cache.SetMaxAge(new TimeSpan(31536000));

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
    }
    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        if (System.Web.HttpContext.Current.Session != null)
        {
            if (Session["Culture"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = (System.Globalization.CultureInfo)Session["Culture"];
                System.Threading.Thread.CurrentThread.CurrentUICulture = (System.Globalization.CultureInfo)Session["Culture"];
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(MasterConfigHelper.ApplicationCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(MasterConfigHelper.ApplicationCulture);

                System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                newCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                newCulture.DateTimeFormat.DateSeparator = "-";
                System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
            }
        }
    }

    private void Page_Error(object sender, EventArgs e)
    {
        //Exception exc = Server.GetLastError();

        //if (exc is HttpUnhandledException)
        //{
        //    // Pass the error on to the error page.
        //    Server.Transfer("Error.aspx?handler=Application_Error%20-%20Global.asax", true);
        //}
    }

</script>

using Application;
using DI.Chakra.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Master
{
    public partial class ProdAttestation : System.Web.UI.MasterPage
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        public bool DisableRedirect
        {
            get
            {
                if (ViewState["DisableRedirect"] == null)
                    return false;

                return (bool)ViewState["DisableRedirect"];
            }
            set
            {
                ViewState["DisableRedirect"] = value;
            }
        }
        public string AdditionalQueryString
        {
            get
            {
                if (ViewState["AdditionalQueryString"] == null)
                    return null;

                return ViewState["AdditionalQueryString"].ToString();
            }
            set
            {
                ViewState["AdditionalQueryString"] = value;
            }
        }
        public long? ContextObjectId
        {
            get
            {
                if (ViewState["CONTEXTOBJECTID"] == null)
                    return null;

                return (long)ViewState["CONTEXTOBJECTID"];
            }
            set
            {
                ViewState["CONTEXTOBJECTID"] = value;
            }
        }

        public string TypeofRequest
        {
            get
            {
                if (ViewState["TypeofRequest"] == null)
                    return "";

                return (string)ViewState["TypeofRequest"];
            }
            set
            {
                ViewState["TypeofRequest"] = value;
            }
        }

        public long PageId
        {
            get
            {
                if (ViewState["PageId"] == null)
                    return 0;

                return (long)ViewState["PageId"];
            }
            set
            {
                ViewState["PageId"] = value;
            }
        }
        public string CommandArgumentsNotRequiresRedirection { get; set; } = "/DataManagement/";

        public event EventHandler<AccessActionEventArgs> ActionClicked;
        public RadScriptManager Radscriptmanager
        {
            get
            {
                return this.RadScriptManager1;
            }
        }
        /// <summary>
        /// page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool DisableImageButton
        {
            get
            {
                if (ViewState["DisableImageButton"] == null)
                    return false;

                return (bool)ViewState["DisableImageButton"];
            }
            set
            {
                ViewState["DisableImageButton"] = value;
            }

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //if (sessionStatus.User == null)
            //{
            //    Response.Redirect("~/Login.aspx");
            //}

            LoginHelper.ValidateSessionAndAuthendicationCookie();
        }
        public string ServerErrors(ArrayList err)
        {
            IEnumerator enumerr = err.GetEnumerator();
            int slno = 0;
            string ErrString = "";
            while (enumerr.MoveNext())
            {
                slno = slno + 1;
                if (err.Count > 1)
                    ErrString = ErrString + Environment.NewLine + slno + ". " + (string)enumerr.Current;
                else
                    ErrString = ErrString + slno + ". " + (string)enumerr.Current;
            }
            return ErrString;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("absolutePath", Request.Url.AbsolutePath.ToString());
                WebPage page = DomainApiClient<Dto.WebPage>.GetByAction<Dto.WebPage>
                    (DomainApiAction.ACL.WebPage.GetByAbsolutePath, parameter);



                if (null == page)
                    return;


                PageId = page.Id;
                var pageName = page.Name;
                Page.Title = pageName;

                List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
                bool hasAccess = pagePermissions.Any(x => x.PageId == page.Id);
                if (!hasAccess && page.PageUrl != null && page.PageUrl.Url != null && (page.PageUrl.Url.Contains("Detail.aspx") || page.PageUrl.Url.Contains("GroupTasks.aspx")))
                    hasAccess = true;
                if (!hasAccess)
                {
                    Response.Redirect("~/AccessDenied.aspx");
                }

                if (this.Page is IContextObjectEnabled)
                {
                    if (!string.IsNullOrEmpty(sessionStatus.ReturnErrorMessage))
                    {
                        sessionStatus.ReturnErrorMessage = string.Empty;
                    }
                }
            }
        }
    }
}
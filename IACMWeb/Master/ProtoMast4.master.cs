using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Master
{
    public partial class ProtoMast4 : System.Web.UI.MasterPage//, IAccessControl
    {
        #region static Members
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        public RadGrid gridid { get; set; }
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
        public int ScreenWidth
        {
            get
            {
                if (string.IsNullOrWhiteSpace(hdnScreenWidth.Value))
                    return 1300;
                else
                    return Convert.ToInt32(hdnScreenWidth.Value);
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

        //private List<string> _commandArgumentsNotRequiresRedirection;

        //public List<string> CommandArgumentsNotRequiresRedirection
        //{
        //    get {
        //        if(_commandArgumentsNotRequiresRedirection == null)
        //        {
        //            _commandArgumentsNotRequiresRedirection = new List<string>();
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/ADProvisioningMatrix/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAPFLINS/SAPRoleBasedonOrgMgt/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAPBIRoleCategories/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAP_PIP_SAP_PI_Role/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAPFLINS/SAPRoleBasedOnEEGroup/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAPFLINS/SAPRoleBasedonPosition/List.aspx");
        //            _commandArgumentsNotRequiresRedirection.Add("/DataManagement/SAPFLINS/SAPHRFLINSRoleMatrix/List.aspx");
        //        }
        //        return _commandArgumentsNotRequiresRedirection;
        //    }
        //    set { _commandArgumentsNotRequiresRedirection = value; }
        //}
        public string CommandArgumentsNotRequiresRedirection { get; set; } = "/DataManagement/";
        #endregion

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
        public string PreferredPageTitle
        {
            get
            {
                if (ViewState["PreferredPageTitle"] != null)
                {
                    return Convert.ToString(ViewState["PreferredPageTitle"]);
                }
                return String.Empty;
            }
            set
            {
                ViewState["PreferredPageTitle"] = value;
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(lblPageTitle.Text))
                    lblPageTitle.Text = value;
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

        public void ShowAlert(AlertType alertType, string Message ,bool showWarning = false )
        {
            AlertMessage1.AlertType = alertType == AlertType.Warning ? AlertType.Error : alertType;
            if (showWarning)
            {
                AlertMessage1.AlertType = alertType;
            }
            AlertMessage1.Message = Message;
            AlertMessageUpdatePanel.Update();
            AlertMessage1.Show();
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
                int sessionTimeoutMinutes = System.Web.HttpContext.Current.Session.Timeout;
                // Render session timeout value as a JavaScript variable
                string jsVariable = $";setSessionTimeout({sessionTimeoutMinutes});";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "sessionTimeout1", jsVariable, true);
                // this is need, in case if we want to set the custom page title
                // then set blank page name in DB, it will take the value of page title. 
                this.lblPageTitle.Text = Page.Title;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("absolutePath", Request.Url.AbsolutePath.ToString());
                WebPage page = DomainApiClient<Dto.WebPage>.GetByAction<Dto.WebPage>
                    (DomainApiAction.ACL.WebPage.GetByAbsolutePath, parameter);



                if (null == page)
                    return;


                AccessPanel.PageId = PageId = page.Id;
                var pageName = page.Name;
                if (string.IsNullOrEmpty(pageName))
                    pageName = this.lblPageTitle.Text;
                this.lblPageTitle.Text = pageName;
                // setting the Page Title from the page name in the database
                Page.Title = pageName;

                List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
                bool hasAccess = pagePermissions.Any(x => x.PageId == page.Id);
                if (!hasAccess && page.PageUrl != null && page.PageUrl.Url != null && (page.PageUrl.Url.Contains("Detail.aspx") || page.PageUrl.Url.Contains("GroupTasks.aspx")))
                    hasAccess = true;
                //Dictionary<string, string> param = new Dictionary<string, string>();
                //param.Add("pageId", page.Id.ToString());
                //param.Add("userId", new SessionStatusImpl().UserId.ToString());
                //bool hasAccess = DomainApiClient<Dto.PagePermission>.GetByAction<bool>
                //    (DomainApiAction.ACL.PagePermission.HasAccess, param);

                if (!hasAccess)
                {
                    Response.Redirect("~/AccessDenied.aspx");
                }

                if (this.Page is IContextObjectEnabled)
                {
                    if (!string.IsNullOrEmpty(sessionStatus.ReturnErrorMessage))
                    {
                        ShowAlert(AlertType.Error, sessionStatus.ReturnErrorMessage);
                        sessionStatus.ReturnErrorMessage = string.Empty;
                    }
                }
            }
            Page.Title = this.lblPageTitle.Text;

            // if we have set a Preferred page title for the page different from th db title
            // like , in case of Attestation review page where we show Camapign name in title
            if (!String.IsNullOrWhiteSpace(this.PreferredPageTitle))
            {
                // show the Preferred title
                this.lblPageTitle.Text = this.PreferredPageTitle;
            }

            // Need to revisit
            //if (sessionStatus.EntitlementmanagementNavigatorHelper != null && Page.Title != "Entitlement Management")
            //{
            //    sessionStatus.EntitlementmanagementNavigatorHelper = null;
            //}


        }

        protected void AccessPanel_ActionClicked1(object sender, EventArgs e)
        {
            if (ActionClicked == null)
                return;
            ACLAction action = (ACLAction)Enum.Parse(typeof(ACLAction), ((ImageButton)sender).CommandName);

            if (action == ACLAction.Add)
            {
                UIBasePage basePage = this.Page as UIBasePage;

                basePage.ContextObjectId = null;
            }
            if (!DisableRedirect && !string.IsNullOrWhiteSpace(((ImageButton)sender).CommandArgument))
                checkIfRedirectRequired(((ImageButton)sender).CommandArgument);

            if (IsRedirectAction(action) && !string.IsNullOrWhiteSpace(((ImageButton)sender).CommandArgument))
            {
                if (!DisableRedirect)
                    RedirectTo(((ImageButton)sender).CommandArgument, action);
                else
                {
                    ActionClicked(sender, new AccessActionEventArgs(action));
                }
            }
            else
            {
                ActionClicked(sender, new AccessActionEventArgs(action));
            }
        }

        protected void checkIfRedirectRequired(string commandArgument)
        {
            DisableRedirect = commandArgument.Contains(CommandArgumentsNotRequiresRedirection);
        }

        public void RedirectTo(string url, ACLAction action)
        {
            if (action == ACLAction.Add)
                Response.Redirect(url);
            else
            {
                string sUrl = url;
                if (this.ContextObjectId == null)
                {
                    ShowAlert(AlertType.Error, "Please select an item.");
                    return;
                }
                else
                    sUrl = sUrl + "?contextId=" + this.ContextObjectId.ToString();


                if (string.IsNullOrEmpty(this.TypeofRequest) && Request.Url.ToString().Contains("Task/List.aspx"))
                {
                    ShowAlert(AlertType.Error, "Please select an item.");
                    return;
                }
                else
                    sUrl = sUrl + "&TypeofRequest=" + this.TypeofRequest;

                if (!string.IsNullOrWhiteSpace(this.AdditionalQueryString))
                {
                    sUrl = sUrl + this.AdditionalQueryString;
                }
                Response.Redirect(sUrl);

            }
        }

        private bool IsRedirectAction(ACLAction action)
        {
            if (action == ACLAction.Add || action == ACLAction.Edit || action == ACLAction.Detail ||
                action == ACLAction.Clone || action == ACLAction.AccessRights || action == ACLAction.PagePermission ||
                action == ACLAction.ViewSchedule || action == ACLAction.GeneratePair)
                return true;
            else
                return false;
        }

        #region IACMRadGridControl specific logic

        protected void hdnButton_Click(object sender, EventArgs e)
        {
            string gridId = Session["gridcustom"].ToString();
            RadGrid radGrid = FindControlRecursive(this, gridId) as RadGrid;
            if (radGrid != null)
            {
                DeleteGridColumnInformation(gridId, sessionStatus.User.Id);
                ArrayList lstColumns = new ArrayList();
                if (radGrid.MasterTableView.EnableHeaderContextMenu)
                {
                    foreach (GridColumn gridColumn in radGrid.MasterTableView.RenderColumns)
                    {
                        if (gridColumn.Display)
                        {
                            lstColumns.Add(gridColumn.UniqueName);
                            SaveColumnChanges(gridId, sessionStatus.User.Id, gridColumn.UniqueName, gridColumn.OrderIndex);
                        }
                    }
                }
            }
        }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn =
                    FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        public void SaveColumnChanges(string GridID, long userId, string ColumnName, int columnOrder)
        {
            if (columnOrder > -1)
            {
                Dto.UserGridColumnInfo columns = new Dto.UserGridColumnInfo();
                columns.UserId = sessionStatus.User.Id;
                columns.GridId = GridID;
                columns.ColumnName = ColumnName;
                columns.OrderIndex = columnOrder;
                columns.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                DomainApiClient<Dto.UserGridColumnInfo>.Save(columns);
            }
        }

        public void DeleteGridColumnInformation(string GridID, long UserID)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString(UserID));
            parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            parameter.Add("gridId", GridID);
            DomainApiClient<Dto.UserGridColumnInfo>.GetByAction<Dto.UserGridColumnInfo>
                (DomainApiAction.Preference.UserColumnInfo.Delete, parameter);
        }

        #endregion

    }
}
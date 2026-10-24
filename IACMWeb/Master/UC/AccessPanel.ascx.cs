using Application;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace DI.Chakra.Web.Master.UC
{
    public partial class AccessPanel : DI.Chakra.Web.UIBaseUC
    {
        public event EventHandler AccessActionClicked;
        private const string V_PAGEID = "V_PAGEID";
        private const string V_PAGENAME = "V_PAGENAME";

        public long PageId
        {
            get
            {
                if (ViewState[V_PAGEID] == null)
                    return -1;
                return (long)ViewState[V_PAGEID];
            }
            set
            {
                ViewState[V_PAGEID] = value;
            }
        }
        public string PageName
        {
            get
            {
                if (ViewState[V_PAGENAME] == null)
                    return string.Empty;
                return (string)ViewState[V_PAGENAME];
            }
            set
            {
                ViewState[V_PAGENAME] = value;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];

                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("pageId", PageId.ToString());
                //parameter.Add("userId", new SessionStatusImpl().UserId.ToString());
                ////parameter.Add("user", new SessionStatusImpl().User.ToString());
                //rptPermissions.DataSource = DomainApiClient<Dto.PagePermission>.GetByAction<IList<Dto.PagePermission>>
                //    (DomainApiAction.ACL.PagePermission.GetPagePermissions, parameter);


                rptPermissions.DataSource = pagePermissions.Where(x => x.PageId == PageId && x.Permission.Name != "View" && x.Permission.Name.ToLower() != "showallnonemployees").ToList();
                rptPermissions.DataBind();
                if (this.MasterPage.DisableImageButton)
                {
                    AccessControls.Visible = false;
                }
                else
                    AccessControls.Visible = rptPermissions.Items.Count > 0;
            }
            //else
            //{
            //    UpdateAccessPanel.Update();
            //}
        }

        protected void lkAction_Click(object sender, ImageClickEventArgs e)
        {
            if (AccessActionClicked == null)
                return;
            AccessActionClicked(sender, e);
        }

        protected void rptPermissions_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var lkActionE = e.Item.FindControl("lkActionE") as ImageButton;
            var lkActionC = e.Item.FindControl("lkActionC") as ImageButton;
            if (MasterPage != null && MasterPage.Radscriptmanager != null)
            {
                if (lkActionE != null)
                {
                    MasterPage.Radscriptmanager.RegisterPostBackControl(lkActionE);
                }
                if (lkActionE != null)
                {
                    MasterPage.Radscriptmanager.RegisterPostBackControl(lkActionC);
                }
            }
        }
    }
}
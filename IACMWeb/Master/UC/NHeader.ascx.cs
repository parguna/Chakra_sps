using Application;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Master.UC
{
    public partial class NHeader : UIBaseUC
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        private const string V_TABMENUID = "V_TABMENUID";
        private const string V_TABMENU = "ADMIN MODE";

        public long TabMenuId
        {
            get
            {
                if (ViewState[V_TABMENUID] == null)
                    return 1;
                return (long)ViewState[V_TABMENUID];
            }
            set
            {
                ViewState[V_TABMENUID] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                imgLogo.ImageUrl = string.Format("~/Images/Client/{0}", T("App.Branding.Logo"));

                if (sessionStatus.User != null)
                    lblProfile.Text = sessionStatus.User.DisplayName;
                BindMenu();
            }
        }
        private void BindMenu()
        {
            if (sessionStatus.User != null)
            {
                //Dictionary<string, string> parameter = new Dictionary<string, string>();

                //parameter.Add("userId", sessionStatus.UserId.ToString());
                //IList<TabMenu> menuItems = DomainApiClient<Dto.TabMenu>.GetByAction<List<TabMenu>>
                //    (DomainApiAction.ACL.TabMenu.GetTabMenu, parameter);

                IList<TabMenu> menuItems = (IList<TabMenu>)Session["TabMenus"];
                TabMenu menu = menuItems.Where(x => x.UniqueName == V_TABMENU).FirstOrDefault();
                menuItems.Remove(menuItems.Where(x => x.UniqueName == V_TABMENU).FirstOrDefault());
                mnuMain.DataSource = menuItems;
                mnuMain.DataBind();
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            //Perform cleanup - grid filter, sort etc...
            if (sessionStatus != null && sessionStatus.UserId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", sessionStatus.UserId.ToString());
                DomainApiClient<DI.Chakra.Dto.User>.GetByAction<bool>(DomainApiAction.User.PerformLogoutCleanup, parameter);
            }

            //Calling this method to clear the existing session if any
            LoginHelper.ClearUserSessionAndAuthendicationCookie();

            Response.Redirect("~/Login.aspx");
        }

        protected void mnuMain_ItemClick(object sender, RadMenuEventArgs e)
        {
            this.TabMenuId = Convert.ToInt64(e.Item.Value);
            if (e.Item.Attributes["NavigateUrl"].ToString() != "#")
            {
                Response.Redirect(e.Item.Attributes["NavigateUrl"].ToString());
            }
        }

        protected void mnuMain_ItemDataBound(object sender, RadMenuEventArgs e)
        {
            e.Item.Attributes["NavigateUrl"] = e.Item.NavigateUrl;
            //e.Item.NavigateUrl = string.Empty;
        }
    }
}
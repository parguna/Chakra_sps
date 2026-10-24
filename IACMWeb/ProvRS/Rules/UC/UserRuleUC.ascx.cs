using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ProvRS.Rules.UC
{
    public partial class UserRuleUC : UIBaseUC, IAccessControl
    {
        #region Members
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        const string V_USERID = "USERID";
        private static IList<Dto.NRuleset> ruleSets;

        long UserId
        {
            get
            {
                if (ViewState[V_USERID] == null)
                    return 0;
                return (long)ViewState[V_USERID];
            }
            set
            {
                ViewState[V_USERID] = value;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            UserNameAutoCompleteBox.WebServiceSettings.Method = "GetUsers";
            if (!Page.IsPostBack)
            {
                
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                NUserRuleGrid.ExportSettings.FileName = "UserRule";
                NUserRuleGrid.Export(NUserRuleGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                NUserRuleGrid.Export<IList<Dto.NRuleset>>(ruleSets, e.ActionType, "NUserRuleGrid");
            }
        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (UserNameAutoCompleteBox.Entries.Count > 0)
            {
                this.UserId = Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value);
                User usr = DomainApiClient<User>.GetById((long)this.UserId, true);
            }
        }

        protected void UserNameAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            string redirectTarget = Request.Url.PathAndQuery;
            if (DI.Chakra.Infrastructure.UtilityMethods.IsLocalUrl(redirectTarget))
                Response.Redirect(redirectTarget);
            else
                Response.Redirect("~/Default.aspx");
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            if (this.UserNameAutoCompleteBox.Entries.Count > 0)
            {
                NUserRuleGrid.Rebind();
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "select user");
            }
        }


        protected void NUserRuleGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.UserNameAutoCompleteBox.Entries.Count > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", Convert.ToString(UserNameAutoCompleteBox.Entries[0].Value));
                List<Dto.NRuleset> Rulesets = DomainApiClient<Dto.NRuleset>.GetByAction<List<Dto.NRuleset>>("GetUserUniqueRulesets", parameter);
                NUserRuleGrid.DataSource = Rulesets;
                ruleSets = Rulesets;
            }
        }
    }
}
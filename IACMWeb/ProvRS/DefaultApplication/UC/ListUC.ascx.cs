using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ProvRS.DefaultApplication.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {

        #region Member
        private const string V_SELECTEDAPPLICATION = "V_SELECTEDAPPLICATION";
        private const string V_RULEID = "V_RULEID";
        private const string V_ENDPOINTID = "V_ENDPOINTID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        #endregion

        #region Properties
        private List<Dto.PresentationNRulesetForApplication> SelectedApplications
        {
            get
            {
                if (ViewState[V_SELECTEDAPPLICATION] != null)
                {
                    return (List<Dto.PresentationNRulesetForApplication>)ViewState[V_SELECTEDAPPLICATION];
                }
                return new List<Dto.PresentationNRulesetForApplication>();
            }
            set
            {
                ViewState[V_SELECTEDAPPLICATION] = value;
            }
        }

        public long EndPointSystemId
        {
            get
            {
                if (ViewState[V_ENDPOINTID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTID];
            }
            set
            {
                ViewState[V_ENDPOINTID] = value;

            }
        }

        public long RuleId
        {
            get
            {
                if (ViewState[V_RULEID] == null)
                    return 0;
                return (long)ViewState[V_RULEID];
            }
            set
            {
                ViewState[V_RULEID] = value;
            }
        }
        #endregion

        #region Method
        private void Delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select a Value.");
                return;
            }
            DomainApiClient<Dto.NRulesetForApplication>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Application Default Deleted Successfully");
            RuleSetApplicationGrid.Rebind();
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                RuleSetApplicationGrid.Rebind();
            }
        }

        protected void NRuleSetApplicationGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rulesetId", RuleId.ToString());
            this.SelectedApplications = DomainApiClient<Dto.PresentationNRulesetForApplication>.GetByAction<List<Dto.PresentationNRulesetForApplication>>(DomainApiAction.Ruleset.PresentationNRulesetForApplication.GetByRuleset, parameters);
            RuleSetApplicationGrid.DataSource = this.SelectedApplications;
        }

        protected void NRuleSetApplicationGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RuleSetApplicationGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Application Defaults - Delete");
                Delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RuleSetApplicationGrid.ExportSettings.FileName = "ApplicationDefaults";
                RuleSetApplicationGrid.Export(RuleSetApplicationGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RuleSetApplicationGrid.Export<IList<Dto.PresentationNRulesetForApplication>>(this.SelectedApplications, e.ActionType, "ApplicationDefaults");
            }
        }

        #endregion
    }
}
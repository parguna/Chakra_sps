using Application;
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
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
  
        private static IList<Dto.NRuleset> ruleSets;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                NRuleSetGrid.Rebind();
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                NRuleSetGrid.ExportSettings.FileName = "NRuleSet";
                NRuleSetGrid.Export(NRuleSetGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                NRuleSetGrid.Export<IList<Dto.NRuleset>>(ruleSets, e.ActionType, "NRuleSet");
            }
            else if(e.ActionType == ACLAction.Delete)
            {
               
                if (!ContextObjectId.HasValue)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                    return;
                }
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ruleId", ContextObjectId.ToString());
                DomainApiClient<Dto.NRuleset>.GetByAction<int>("DeleteRuleSetsByRuleId", parameters);
                NRuleSetGrid.Rebind();
            }
        }
        
        protected void NRuleSetGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            List<Dto.NRuleset> Rulesets = DomainApiClient<Dto.NRuleset>.GetByAction<List<Dto.NRuleset>>("GetUniqueRulesets", null);
            NRuleSetGrid.DataSource = Rulesets;
            ruleSets = Rulesets; 
        }

        protected void NRuleSetGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = NRuleSetGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("RuleId");
        }
    }
}
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

namespace DI.Chakra.Web.ProvRS.Rules.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
      
        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
       
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return 0;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
            }
        }
        private List<Dto.NRuleset> SelectedRuleSets
        {
            get
            {
                if (ViewState["SelectedRuleSets"] != null)
                {
                    return (List<Dto.NRuleset>)ViewState["SelectedRuleSets"];
                }
                return new List<Dto.NRuleset>();
            }
            set
            {
                ViewState["SelectedRuleSets"] = value;
            }
        }
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    this.EntityId = this.ContextObjectId.Value;
                    SetPage(UIMode);
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }
        private void ViewOnlyMode()
        {
            contentWrapper.Enabled = false;
        }
        private void FormGridEnable()
        {
            if ((UIMode == UIMode.Edit) || (UIMode == UIMode.Add))
            {
                if (grdRulesets.Items.Count == 0)
                {
                    grdRulesets.Visible = false;
                }
                else
                {
                    grdRulesets.Visible = true;
                }
            }
            if (UIMode == UIMode.View)
            {
                grdRulesets.Enabled = false;
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Rule - Save");
                Save();
                Response.Redirect("~/ProvRS/Rules/List.aspx");
            }
        }
        private void SetPage(UIMode UMode)
        {
            this.EntityId = Convert.ToInt64(this.ContextObjectId);
            txtCode.Enabled = false;         
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("RuleId", this.EntityId.ToString());

            var NRuleSets = DomainApiClient<Dto.NRuleset>.GetByAction<List<Dto.NRuleset>>("GetRuleId", parameters);
            if (NRuleSets.Count > 0)
            {          
                var ruleSet = NRuleSets.FirstOrDefault();
                txtCode.Text = ruleSet.Code;
                hfRuleId.Value = ruleSet.RuleId.ToString();
                txtDescription.Text = ruleSet.Description;
                this.SelectedRuleSets = NRuleSets;
                this.RebindRuleSetsGrid();
                FormGridEnable();
            }

        }
        private void RebindRuleSetsGrid()
        {
            this.grdRulesets.DataSource = this.SelectedRuleSets;
            this.grdRulesets.Rebind();
        }
        public void Intialize()
        {
            BindRuleSetType();
            if(UIMode == UIMode.Add)
            {
                long maxRuleid = DomainApiClient<Dto.NRuleset>.GetByAction<long>("GetMaxRuleId", null);
                hfRuleId.Value = maxRuleid.ToString();
            }
        }

        private void BindRuleSetType()
        {
            var ruleTypeList = DomainApiClient<Dto.NRulesetType>.GetAll();
            drpRuleTypes.DataSource = ruleTypeList;
            drpRuleTypes.DataTextField = "RulesetType";
            drpRuleTypes.DataValueField = "Id";
            drpRuleTypes.DataBind();
            drpRuleTypes.Items.Insert(0, "--Select--");
        }
        private void Save()
        {
            Dto.NRuleset ruleset = new Dto.NRuleset();
            ruleset.Description = txtDescription.Text;

            if (this.SelectedRuleSets != null)
            {
                var distinctRow = SelectedRuleSets.Select(a => new { ruleId = a.RuleId, code = a.Code }).Distinct().ToList();
                var finalRuleCode = txtCode.Text; var finalDesc = txtDescription.Text;
                if (distinctRow.Count > 1)
                {
                    SelectedRuleSets.ForEach(a => { a.Code = finalRuleCode; a.Description = finalDesc; });
                }
            }

            ruleset.SelectedRuleSets = SelectedRuleSets;
            if (UIMode == UIMode.Edit)
            {
                ruleset.Id = this.EntityId;
            }
           
            DomainApiClient<Dto.NRuleset>.Save(ruleset);
        }

        protected void grdRulesets_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long RuleId = (long)Convert.ChangeType(item.GetDataKeyValue("RuleId"), typeof(long));
                long TypeId = (long)Convert.ChangeType(item.GetDataKeyValue("TypeId"), typeof(long));
                string Code = Convert.ToString(item.GetDataKeyValue("Code"));
                string Value = Convert.ToString(item.GetDataKeyValue("Value"));
                var ruleSets = this.SelectedRuleSets;
                var rule = ruleSets.Where(m => m.RuleId == RuleId && m.TypeId == TypeId && m.Code == Code && m.Value == Value).FirstOrDefault();
                ruleSets.Remove(rule);
                this.SelectedRuleSets = ruleSets;
                this.RebindRuleSetsGrid();
                FormGridEnable();
            }

        }

        protected void drpRuleTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["RuleSetType"] = drpRuleTypes.SelectedItem.Text;
            ValueAutoCompleteBox.Entries.Clear();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            string value = string.Empty;
            if (String.IsNullOrEmpty(txtCode.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter ruleset code");
                return;
            }
            else if (!String.IsNullOrEmpty(txtCode.Text) && UIMode == UIMode.Add)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("Code", txtCode.Text);
                bool isExists = DomainApiClient<Dto.NRuleset>.GetByAction<bool>("DoesCodeExists", parameters);
                if (isExists)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Ruleset code already exists.Please enter different code");
                    return;
                }
            }
            if (drpRuleTypes.SelectedIndex <= 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select ruleset type");
                return;
            }
            if (ValueAutoCompleteBox.Entries.Count > 0)
            {
                value = ValueAutoCompleteBox.Entries[0].Text.Split(new string[] { " - " }, StringSplitOptions.None)[0].Trim();
            }
            else
            {
                if (!chkAll.Checked)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter ruleset value");
                    return;
                }
            }
            if (this.SelectedRuleSets.Any(m => m.Code == txtCode.Text
            && m.TypeId == Convert.ToInt64(drpRuleTypes.SelectedValue)))
            {
                MasterPage.ShowAlert(AlertType.Error, "Ruleset already contains type " + drpRuleTypes.SelectedItem.Text);
                return;
            }
            Dto.NRuleset rule = new Dto.NRuleset();
            rule.RuleId = Convert.ToInt64(hfRuleId.Value);
            rule.Code = txtCode.Text.Trim();
            rule.Description = txtDescription.Text.Trim();
            var NRuleSetType = DomainApiClient<Dto.NRulesetType>.GetById(Convert.ToInt64(drpRuleTypes.SelectedValue));
            rule.TypeId = Convert.ToInt64(drpRuleTypes.SelectedValue);
            rule.RuleSetType = NRuleSetType;
            rule.Operator = "Equals";
            if (ValueAutoCompleteBox.Entries.Count > 0)
            {
                var splitValue = ValueAutoCompleteBox.Entries[0].Text.Split(new string[] { " - " }, StringSplitOptions.None);
                rule.Value = splitValue[0].Trim();
                if(splitValue.Length > 1)
                    rule.ValueDescription = string.Join("", splitValue.Skip(1).ToList());
            }
            else
            {
                if (!chkAll.Checked)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter ruleset value");
                    return;
                }
                else
                {
                    rule.Value = "*";
                    rule.ValueDescription = "*";
                }
            }
            rule.CreatedDate = DateTime.Now;
            rule.CreatedBy = new SessionStatusImpl().User.DisplayName;
            var rules = this.SelectedRuleSets;
            rules.Add(rule);
            this.SelectedRuleSets = rules;
            this.RebindRuleSetsGrid();
            FormGridEnable();
            drpRuleTypes.SelectedIndex = 0;
            ValueAutoCompleteBox.Entries.Clear();
            chkAll.Checked = false;
            Session["RuleSetType"] = string.Empty;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ProvRS/Rules/List.aspx");
        }

        protected void grdRulesets_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            this.grdRulesets.DataSource = this.SelectedRuleSets;
        }
    }
}
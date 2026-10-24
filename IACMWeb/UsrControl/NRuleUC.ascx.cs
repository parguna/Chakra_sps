using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DI.Chakra.Dto;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl
{
    public partial class NRuleUC : System.Web.UI.UserControl
    {
        #region variables
        public const string TOENABLE = "TOENABLE";



        #endregion
        #region Delegate

        public delegate void SelectedRuleChanged(NRuleset ruleset);
        public event SelectedRuleChanged RuleChanged;
        private NRuleset _mRule = null;

        #endregion

        #region Properties

        public bool ToEnable
        {
            get {
                if (ViewState[TOENABLE] == null)
                    return (bool)true;
                else
                    return (bool)ViewState[TOENABLE];
                }
            set
            {
                ViewState[TOENABLE] = value;
                RuleAutoCompleteBox.Enabled = value;
            }
        }
        public string SetClass
        {
            get
            {
                if (ViewState["SETCLASS"] == null)
                    return "col-xs-7";
                return (string)ViewState["SETCLASS"];
            }
            set
            {
                setClass.Attributes.Add("class", value);
            }
        }
        #endregion

        #region Events

        protected void RuleAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (RuleChanged != null)
                RuleChanged(SelectedRuleSet);
        }
        protected void RuleAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (RuleChanged != null)
                RuleChanged(SelectedRuleSet);
        }

        protected void InfoButton_Click(object sender, ImageClickEventArgs e)
        {
            if (RuleAutoCompleteBox != null && this.RuleAutoCompleteBox.Entries.Count > 0)
            {
                BindRule(this.RuleAutoCompleteBox.Entries[0].Value);
                RulesetRadwindow.VisibleOnPageLoad = true;
            }

        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            RulesetRadwindow.VisibleOnPageLoad = false;
        }

        #endregion

        #region Method
        public NRuleset SelectedRuleSet
        {
            get
            {
                if (RuleAutoCompleteBox.Entries.Count <= 0)
                    return null;
                string selectRule = this.RuleAutoCompleteBox.Entries[0].Value;
                string Text = this.RuleAutoCompleteBox.Entries[0].Text;
                string Code = Text.Split('-')[0].ToString();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("RuleId", selectRule);
                parameter.Add("rulesetCode", Code.Trim());
                _mRule = DomainApiClient<Dto.NRuleset>.GetByAction<NRuleset>(DomainApiAction.Ruleset.NRuleset.GetRuleSetByCodeAndRuleId, parameter);
                return _mRule;
            }
            set
            {
                _mRule = value;
                if (_mRule != null)
                {
                    RuleAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_mRule.Code, _mRule.RuleId.ToString()));
                }
                else
                {
                    Clear();
                }
            }
        }

        private void BindRule(string ruleid)
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("RuleId",ruleid.ToString());
            var ruleSet = DomainApiClient<Dto.NRuleset>.GetByAction<Dictionary<string, string>>(DomainApiAction.Ruleset.NRuleset.GetRulesByRuleID, parameter);
            RuleGrid.DataSource = ruleSet;
            RuleGrid.DataBind();
        }

        public void Clear()
        {
            RuleAutoCompleteBox.Entries.Clear();
        }


        public void DisableMode()
        {
            RuleAutoCompleteBox.Enabled = false;
        }
        #endregion

        protected void RuleGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

        }


    }
}
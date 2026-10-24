using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class RulesetSearchUC : DI.Chakra.Web.UIBaseUC
    {
        public delegate void SelectedRulesetChanged(Object sender);

        public event SelectedRulesetChanged RulesetChanged;

        private List<NRuleset> _mRuleset = null;
        public bool DisableMyAutocompleteBox
        {
            set { RulesetAutoCompleteBox.Enabled = value; }
        }


        public string DataSourceMethod
        {
            set
            {
                RulesetAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }
        public bool AllowSingleOrg
        {
            set
            {
                if (value == true)
                    RulesetAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    RulesetAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                RulesetAutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                RulesetAutoCompleteBox.Enabled = !value;
            }
        }
        public short TabIndexUC
        {
            set
            {
                RulesetAutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("OrgAutoCompleteBox")) ? true : false;
            }
        }
        public List<NRuleset> SelectedRuleset
        {
            get
            {
                if (RulesetAutoCompleteBox.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("RuleId", (string.IsNullOrEmpty(RulesetAutoCompleteBox.Entries[0].Value))? "0" : RulesetAutoCompleteBox.Entries[0].Value);
                _mRuleset = DomainApiClient<Dto.NRuleset>.GetByAction<List<NRuleset>>(DomainApiAction.OrgRevamp.NRuleset.GetByRuleId, parameter);

                return _mRuleset;
            }
            set
            {
                _mRuleset = value;
                if (_mRuleset != null)
                {
                    RulesetRepeater.DataSource = _mRuleset;
                    RulesetRepeater.DataBind();
                }
                else
                {
                    RulesetAutoCompleteBox.Entries.Clear();
                }
            }
        }

        public void Clear()
        {
            RulesetAutoCompleteBox.Entries.Clear();
        }

        protected void RulesetAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.RulesetAutoCompleteBox.Entries.Count > 0)
            {
                string selectedOrg = this.RulesetAutoCompleteBox.Entries[0].Value;
                LinkButtonRuleset.Text = this.RulesetAutoCompleteBox.Entries[0].Text;

                RulesetRepeater.DataSource = SelectedRuleset;
                RulesetRepeater.DataBind();
            }
            else
            {
                LinkButtonRuleset.Text = string.Empty;
            }
            if (RulesetChanged != null)
                RulesetChanged(sender);
           
        }
    }
}
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

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class NRulesetBuilderUC : DI.Chakra.Web.UIBaseUC
    {
        public bool DisableMyAutocompleteBox
        {
            set {
                RulesetTypeAutoCompleteBox.Enabled = value;
                RulesetValueAutoCompleteBox.Enabled = value;
                OperatorField.Enabled = value;
                btnAdd.Enabled = value;
            }
        }

        public List<NRuleset> RulesetDefinition
        {
            get
            {
                if (ViewState["RulesetDefinition"] != null)
                {
                    return (List<NRuleset>)ViewState["RulesetDefinition"];
                }
                return new List<NRuleset>();
            }
            set
            {
                ViewState["RulesetDefinition"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperatorField.Items.Clear();
                foreach (RulesetOperator item in Enum.GetValues(typeof(RulesetOperator)))
                {
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, description);
                    if(description == "Equals")
                        listItem.Selected = true;
                    OperatorField.Items.Add(listItem);
                }
                this.RebindFormsGrid();
            }            
        }

        public string DataSourceMethod
        {
            set
            {
                RulesetTypeAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }

        public bool AllowSingleOrg
        {
            set
            {
                if (value == true)
                    RulesetTypeAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    RulesetTypeAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                RulesetTypeAutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                RulesetTypeAutoCompleteBox.Enabled = !value;
                RulesetValueAutoCompleteBox.Enabled = !value;
                OperatorField.Enabled = !value;
                btnAdd.Enabled = !value;
            }
        }
        public short TabIndexUC
        {
            set
            {
                RulesetTypeAutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("OrgAutoCompleteBox")) ? true : false;
            }
        }

        public void Clear()
        {
            RulesetTypeAutoCompleteBox.Entries.Clear();
            RulesetValueAutoCompleteBox.Entries.Clear();
            OperatorField.SelectedIndex = OperatorField.Items.Count > 1 ? 1 : 0;
        }

        protected void RulesetTypeAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            RulesetValueAutoCompleteBox.Entries.Clear();
            RulesetValueAutoCompleteBox.WebServiceSettings.Method = "";
            OperatorField.ClearSelection();
            OperatorField.SelectedIndex = OperatorField.Items.Count > 1 ? 1 : 0;

            if (this.RulesetTypeAutoCompleteBox.Entries.Count > 0)
            {
                string selectedOrg = this.RulesetTypeAutoCompleteBox.Entries[0].Text;                

                switch (selectedOrg)
                {
                    case "Organisation":
                        RulesetValueAutoCompleteBox.WebServiceSettings.Method = "GetNOrganisations";
                        break;
                    case "Designation":
                        RulesetValueAutoCompleteBox.WebServiceSettings.Method = "GetNDesignations";
                        break;
                    case "EmployeeType":
                        RulesetValueAutoCompleteBox.WebServiceSettings.Method = "GetNEmployeeTypes";
                        break;
                    case "EventTrigger":
                        RulesetValueAutoCompleteBox.WebServiceSettings.Method = "GetNEmployeeEventTypes";
                        break;
                }                
            } 
        }

        protected void RulesetValueAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.RulesetTypeAutoCompleteBox.Entries.Count > 0)
            {
                string selectedOrg = this.RulesetTypeAutoCompleteBox.Entries[0].Value;
            }            
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //NRuleset ruleset = new NRuleset();
            //ruleset.TypeId = Convert.ToInt64(this.RulesetTypeAutoCompleteBox.Entries[0].Value);
            //ruleset.Value = this.RulesetValueAutoCompleteBox.Entries[0].Value;
            //ruleset.Operator = OperatorField.SelectedValue;
            //var forms = this.RulesetDefinition;
            //this.RulesetDefinition = forms;
            this.RebindFormsGrid();
            //if (this.RulesetTypeAutoCompleteBox.Entries.Count != 0 && this.RulesetValueAutoCompleteBox.Entries.Count != 0)
            //{
            //    var entryId = 1;
            //    NRuleset ruleset = new NRuleset();
            //    ruleset.TypeId = Convert.ToInt64(this.RulesetTypeAutoCompleteBox.Entries[0].Value);
            //    ruleset.Value = this.RulesetValueAutoCompleteBox.Entries[0].Value;
            //    ruleset.Operator = OperatorField.SelectedValue;

            //    var forms = this.RulesetDefinition;
            //    if (true)
            //    {
            //        forms.Add(ruleset);
            //        this.RulesetDefinition = forms;
            //        this.RebindFormsGrid();
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Warning, "Rule already added");
            //    }
            //    this.RulesetValueAutoCompleteBox.Entries.Clear();
            //}
        }

        protected void FormGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
        }

        private void RebindFormsGrid()
        {
            NRuleset ruleset = new NRuleset();
            ruleset.TypeId = 1;
            ruleset.Value = "summa";
            ruleset.Operator = "Equals";

            List<NRuleset> hmm = new List<NRuleset>();
            hmm.Add(ruleset);
            var forms = this.RulesetDefinition;
            forms.Add(ruleset);
            this.RulesetDefinition = forms;

            this.RulesetGrid.DataSource = RulesetDefinition;
            //this.RulesetGrid.DataSource = hmm;
            this.RulesetGrid.DataBind();
        }
    }
}
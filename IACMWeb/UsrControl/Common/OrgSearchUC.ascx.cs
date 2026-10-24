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
    public partial class OrgSearchUC : DI.Chakra.Web.UIBaseUC
    {
        public delegate void SelectedOrgChanged(Object sender);

        public event SelectedOrgChanged OrgChanged;

        private NOrganisation _mOrganisation = null;
        public bool DisableMyAutocompleteBox
        {
            set { OrgAutoCompleteBox.Enabled = value; }
        }


        public string DataSourceMethod
        {
            set
            {
                OrgAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }
        public bool AllowSingleOrg
        {
            set
            {
                if (value == true)
                    OrgAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    OrgAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                OrgAutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                OrgAutoCompleteBox.Enabled = !value;
            }
        }
        public short TabIndexUC
        {
            set
            {
                OrgAutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("OrgAutoCompleteBox")) ? true : false;
            }
        }
        public NOrganisation SelectedOrganisation
        {
            get
            {
                if (OrgAutoCompleteBox.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("Id", (string.IsNullOrEmpty(OrgAutoCompleteBox.Entries[0].Value))?"0" : OrgAutoCompleteBox.Entries[0].Value);
                _mOrganisation = DomainApiClient<Dto.NOrganisation>.GetByAction<NOrganisation>(DomainApiAction.Org.NOrganisation.GetById,parameter);

                return _mOrganisation;
            }
            set
            {
                _mOrganisation = value;
                if (_mOrganisation != null)
                {
                    OrgAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_mOrganisation.Name, _mOrganisation.Id.ToString()));
                    NOrgDetail.OrganisationId = _mOrganisation.Id;
                }
                else
                {
                    OrgAutoCompleteBox.Entries.Clear();
                }
            }
        }

        public void Clear()
        {
            OrgAutoCompleteBox.Entries.Clear();
        }

        protected void OrgAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.OrgAutoCompleteBox.Entries.Count > 0)
            {
                string selectedOrg = this.OrgAutoCompleteBox.Entries[0].Value;
                NOrgDetail.OrganisationId = Convert.ToInt64(selectedOrg);
            }
            if (OrgChanged != null)
                OrgChanged(sender);
           
        }
    }
}
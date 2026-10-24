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
    public partial class DesignationSearchUC : DI.Chakra.Web.UIBaseUC
    {
        public delegate void SelectedDesignationChanged(Object sender);

        public event SelectedDesignationChanged DesignationChanged;

        private NDesignation _mNDesignation = null;
        public bool DisableMyAutocompleteBox
        {
            set { DesignationAutoCompleteBox.Enabled = value; }
        }


        public string DataSourceMethod
        {
            set
            {
                DesignationAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }
        public bool AllowSingleDesignation
        {
            set
            {
                if (value == true)
                    DesignationAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    DesignationAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                DesignationAutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                DesignationAutoCompleteBox.Enabled = !value;
            }
        }
        public short TabIndexUC
        {
            set
            {
                DesignationAutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("DesignationAutoCompleteBox")) ? true : false;
            }
        }
        public NDesignation SelectedDesignation
        {
            get
            {
                if (DesignationAutoCompleteBox.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("Id", (string.IsNullOrEmpty(DesignationAutoCompleteBox.Entries[0].Value))?"0" : DesignationAutoCompleteBox.Entries[0].Value);
                _mNDesignation = DomainApiClient<Dto.NDesignation>.GetByAction<NDesignation>(DomainApiAction.NDesignation.GetById,parameter);

                return _mNDesignation;
            }
            set
            {
                _mNDesignation = value;
                if (_mNDesignation != null)
                {
                    DesignationAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_mNDesignation.Description, _mNDesignation.Id.ToString()));
                }
                else
                {
                    DesignationAutoCompleteBox.Entries.Clear();
                }
            }
        }

        public void Clear()
        {
            DesignationAutoCompleteBox.Entries.Clear();
        }

        protected void DesignationAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.DesignationAutoCompleteBox.Entries.Count > 0)
            {
                string selectedDesignation = this.DesignationAutoCompleteBox.Entries[0].Value;
            }
            if (DesignationChanged != null)
                DesignationChanged(sender);
           
        }
    }
}
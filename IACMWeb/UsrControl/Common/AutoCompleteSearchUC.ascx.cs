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
    public partial class AutoCompleteSearchUC : DI.Chakra.Web.UIBaseUC
    {
        public delegate void SelectedObjectChanged(Object sender);

        public event SelectedObjectChanged ObjectChanged;

        private long _mObject = 0;
        public bool DisableMyAutocompleteBox
        {
            set { AutoCompleteBox.Enabled = value; }
        }

        public string DatasourceFilter
        {
            set { Filter.Value = value; }
        }

        public string DataSourceMethod
        {
            set
            {
                AutoCompleteBox.WebServiceSettings.Method = value;
            }
        }
        public bool AllowSingleUser
        {
            set
            {
                if (value == true)
                    AutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    AutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                AutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                AutoCompleteBox.Enabled = !value;
            }
        }
        public short TabIndexUC
        {
            set
            {
                AutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("AutoCompleteBox")) ? true : false;
            }
        }
        public long SelectedObjectId
        {
            get
            {
                if (AutoCompleteBox.Entries.Count <= 0)
                    return 0;
              
                _mObject = Convert.ToInt64(AutoCompleteBox.Entries[0].Value);

                return _mObject;
            }
            set
            {
                _mObject = value;
                if (_mObject <= 0)
                {
                    AutoCompleteBox.Entries.Clear();
                }
            }
        }

        public void Clear()
        {
            AutoCompleteBox.Entries.Clear();
        }

        protected void AutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.AutoCompleteBox.Entries.Count > 0)
            {
                SelectedObjectId = Convert.ToInt64(AutoCompleteBox.Entries[0].Value);
                if (ObjectChanged != null)
                {
                    ObjectChanged(sender);
                }
            }
        }
    }
}
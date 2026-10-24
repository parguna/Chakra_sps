using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class EntitlementUC : DI.Chakra.Web.UIBaseUC
    {
        private string V_ENDPOINTTYPE = "V_ENDPOINTTYPE";
        private string V_ENDPOINTSYSTEM = "V_ENDPOINTSYSTEM";
        private string V_ENTITLEMENTTYPE = "V_ENTITLEMENTTYPE";
        private string V_ENTITLEMENT = "V_ENTITLEMENT";
        private string V_ENTITLEMENTID = "V_ENTITLEMENTID";
        private string V_ENTITLEMENTNAME = "V_ENTITLEMENTNAME";
        private string V_CLEAR = "V_CLEAR";
        public delegate void EntitlementTypeIndexChanged(string EntitlementType);
        public event EntitlementTypeIndexChanged EntitlementTypeChanged;

        public delegate void AddClick(object sender, EventArgs e);
        public event AddClick AddButtonClick;


        public bool Clear
        {
            get
            {
                if (ViewState[V_CLEAR] == null)
                    return false;
                return (bool)ViewState[V_CLEAR];
            }
            set
            {
                clearLeft.Visible = value;
            }
        }
        public EndpointType ApplicationType
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPE] == null)
                    return EndpointType.None;
                return (EndpointType)ViewState[V_ENDPOINTTYPE];
            }
            set
            {
                ViewState[V_ENDPOINTTYPE] = value;
                Dictionary<String, string> parameter = new Dictionary<String, string>();
                if (value != EndpointType.None)
                {
                    BindEntitlementType(value);
                }
                else
                {
                    EntitlementType.Items.Clear();
                    //EntitlementNameAutoCompleteBox.Enabled = false;
                }
            }
        }
        public bool DisabledEntitlementType
        {
            set
            {
                EntitlementType.Enabled = value;
            }
        }
        public bool DisabledEntitlement
        {
            set
            {
                EntitlementNameAutoCompleteBox.Enabled = value;
            }
        }
        public bool DisplayAdd
        {
            set
            {
                Add.Visible = value;
            }
        }
        public string entitlementType
        {
            get
            {
                if (ViewState[V_ENTITLEMENTTYPE] == null)
                    return "";
                return ViewState[V_ENTITLEMENTTYPE].ToString();
            }
            set
            {
                ViewState[V_ENTITLEMENTTYPE] = value;
                if (!string.IsNullOrEmpty(value))
                {
                    EntitlementType.SelectedValue = value;
                    EntitlementType.SelectedItem.Text = value;
                }
            }
        }
        public long entitlementId
        {
            get
            {
                if (ViewState[V_ENTITLEMENTID] == null)
                    return 0;
                return (long)ViewState[V_ENTITLEMENTID];
            }
            set
            {
                ViewState[V_ENTITLEMENTID] = value;
                if (value > 0)
                {
                    EntitlementNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(entitlementName, value.ToString()));
                }
            }
        }
        public string entitlementName
        {
            get
            {
                if (ViewState[V_ENTITLEMENTNAME] == null)
                    return "";
                return ViewState[V_ENTITLEMENTNAME].ToString();
            }
            set
            {
                ViewState[V_ENTITLEMENTNAME] = value;
            }
        }

        public long EndpointSystemId
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEM] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTSYSTEM];
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEM] = value;
            }
        }

        public string SetHiddenValue {            
            set
            {
                HiddenValue.Value = value;
            }
        }

        public void ClearEntitlement()
        {
            EntitlementType.Items.Clear();
            EntitlementNameAutoCompleteBox.Entries.Clear();
        }
        public void ClearEntitlementName()
        {
            EntitlementNameAutoCompleteBox.Entries.Clear();
            this.entitlementId = 0;
            //  this.entitlementType = string.Empty;
        }
        private void BindEntitlementType(EndpointType value)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("applicationType", value.ToString());
            List<EntitlementType> _entitlementType = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<List<EntitlementType>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetEntitlementList, parameter);
            EntitlementType.DataSource = _entitlementType;
            EntitlementType.DataBind();
            EntitlementType.Items.Insert(0, "--Select--");
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void EntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EntitlementNameAutoCompleteBox.Entries.Clear();
            LoadEntitlementName();
        }
        public void LoadEntitlementName()
        {
            if (EntitlementType.SelectedItem.Text != null && EntitlementType.SelectedItem.Text != "--Select--")
            {
                HiddenValue.Value = this.ApplicationType + "," + this.EndpointSystemId + "," + EntitlementType.SelectedItem.Text;
                this.entitlementType = EntitlementType.SelectedItem.Text;

                if (EntitlementTypeChanged != null)
                    EntitlementTypeChanged(this.entitlementType);
            }
            else
                this.entitlementType = null;
        }
        protected void EntitlementNameAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (EntitlementType.SelectedItem.Text == null || EntitlementType.SelectedItem.Text == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select Entitlement Type");
                return;
            }
            this.entitlementId = Convert.ToInt64(e.Entry.Value);
            this.entitlementName = e.Entry.Text;
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            if (AddButtonClick != null)
            {
                AddButtonClick(sender, e);
            }
        }

        protected void EntitlementNameAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            this.entitlementId = 0;
            this.entitlementName = "";
        }
    }
}
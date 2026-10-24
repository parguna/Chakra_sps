using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.AdminUI
{
    public partial class MassPropagateO365License : DI.Chakra.Web.UIBasePage
    {
        const string SELECTED_LICENSE ="";
        private SessionStatusImpl sessionStatus;

        public string SelectedLicense
        {
            get
            {
                if (LicenseAutoCompleteBox.Entries.Count > 0)
                    return LicenseAutoCompleteBox.Entries[0].Value;
                else
                {
                    return string.Empty;
                }
            }
        }

        public Dictionary<string, string> SelectedServicePlan
        {
            get
            {
                Dictionary<string, string> selecteditems = new Dictionary<string, string>();

                foreach (var checkeditem in ServicePlanComboBox.CheckedItems)
                {
                    selecteditems.Add(checkeditem.Value, checkeditem.Text);
                }
                return selecteditems;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            sessionStatus = new SessionStatusImpl();
            if (!Page.IsPostBack)
            {
                Savebtn.Visible = false;
                Session[SELECTED_LICENSE] = null;
            }
        }

        protected void LicenseAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (this.Removechkbox.Checked)
            {
                chkboxRemoveLicense.Visible = false;
                chkboxRemoveLicense.Checked = false;
            }
            btnAdd.Enabled = false;
            BindServicePlanComboBox();

        }
        protected void LicenseAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            ServicePlanComboBox.Enabled = true;
            if (LicenseAutoCompleteBox.Entries.Count > 1)
            {
                LicenseAutoCompleteBox.Entries.RemoveAt(1);
            }
            if (this.Removechkbox.Checked)
            {
                chkboxRemoveLicense.Visible = true;
                chkboxRemoveLicense.Checked = false;
                ServicePlanComboBox.Enabled = true;
            }
            BindServicePlanComboBox();
        }
        public void BindServicePlanComboBox()
        {
            if (this.SelectedLicense != string.Empty)
            {
                List<Dto.Office365LicenseConfig> list = DomainApiClient<Dto.Office365LicenseConfig>.GetByAction<List<Dto.Office365LicenseConfig>>(DomainApiAction.Endpoint.O365.GetOffice365ServicePlansforBulkReq,
                    new Dictionary<string, string> {    { "License", this.SelectedLicense }
                    });

                ServicePlanComboBox.DataSource = list;
                ServicePlanComboBox.DataTextField = "ServicePlanDescription";
                ServicePlanComboBox.DataValueField = "ServicePlan";
                ServicePlanComboBox.DataBind();
            }
            else
            {
                ServicePlanComboBox.DataSource = string.Empty;
                ServicePlanComboBox.DataBind();
            }
        }

        protected void ServicePlanComboBox_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ServicePlanComboBox.CheckedItems.Count > 0)
            {
                btnAdd.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }
        protected void chkboxRemoveLicense_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxRemoveLicense.Checked)
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = false;
                btnAdd.Enabled = true;
            }
            else
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = true;
                btnAdd.Enabled = false;

            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ProvMassLicenseInfoDetail _gridObj = new ProvMassLicenseInfoDetail();
            _gridObj.License = this.SelectedLicense;
            if (chkboxRemoveLicense.Checked)
            {
                _gridObj.IsRemoveLicense = true;
            }
            else
            {
                _gridObj.IsRemoveLicense = false;
                var list = this.SelectedServicePlan.Select(item => item.Value).ToList();
                _gridObj.ServiceName = string.Join(",", list.ToArray());
            }

            ServicePlanGrid.Visible = true;
            UpdateSessionObject(_gridObj);
            ServicePlanGrid.DataSource = (List<ProvMassLicenseInfoDetail>)Session[SELECTED_LICENSE];
            ServicePlanGrid.DataBind();
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.DataSource = string.Empty;
            ServicePlanComboBox.DataBind();
            btnAdd.Enabled = false;
            chkboxRemoveLicense.Checked = false;
            chkboxRemoveLicense.Visible = false;
            Savebtn.Visible = true;
        }
        protected void ServicePlanGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                chkboxRemoveLicense.Visible = false;
                var AccountSkuId = (string)((GridDataItem)e.Item).GetDataKeyValue("License");
                List<ProvMassLicenseInfoDetail> _sessionObj = (List<ProvMassLicenseInfoDetail>)Session[SELECTED_LICENSE];
                _sessionObj.RemoveAll(item => item.License == AccountSkuId);
                Session[SELECTED_LICENSE] = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    ServicePlanGrid.DataSource = (List<ProvMassLicenseInfoDetail>)Session[SELECTED_LICENSE];
                    ServicePlanGrid.DataBind();
                }
                else
                {
                    ServicePlanGrid.Visible = false;
                    btnAdd.Enabled = false;
                }
            }
        }
        protected void Savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session[SELECTED_LICENSE] != null)
                {
                    ProvMassLicenseInfo _massLicenseInfo = new ProvMassLicenseInfo();
                    _massLicenseInfo.LicenseDetails = (List<ProvMassLicenseInfoDetail>)Session[SELECTED_LICENSE];
                    string License = UtilityMethods.Serialize(_massLicenseInfo);
                    var o365Param = new Dictionary<string, string>();
                    o365Param.Add("applicationType", ((int)ApplicationType.Office365).ToString());
                    var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, o365Param);
                    Office365MassLicense massLicense = new Office365MassLicense();
                    massLicense.Licenses = License;
                    massLicense.EndPointSystemId = endPoint.Id;
                    massLicense.IsMassAdd = Addchkbox.Checked ? true : false;
                    massLicense.CreatedDate = DateTime.Now;
                    massLicense.CreatedBy = sessionStatus.User.UserName;
                    DomainApiClient<Office365MassLicense>.Save(massLicense);
                    MasterPage.ShowAlert(AlertType.Success, "Propagation details added sucessfully");
                    ServicePlanGrid.Visible = false;
                    Savebtn.Visible = false;
                    Session[SELECTED_LICENSE] = null;
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        protected void Addchkbox_CheckedChanged(object sender, EventArgs e)
        {
            Session[SELECTED_LICENSE] = null;
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.Items.Clear();
            ServicePlanGrid.Visible = false;
            btnAdd.Enabled = false;
            Savebtn.Visible = false;
            if (Addchkbox.Checked)
            {
                chkboxRemoveLicense.Checked = false;
                chkboxRemoveLicense.Visible = false;
            }
        }

        private void UpdateSessionObject(ProvMassLicenseInfoDetail currentobj)
        {
            if (Session[SELECTED_LICENSE] != null)
            {
                List<ProvMassLicenseInfoDetail> _sessionObj = (List<ProvMassLicenseInfoDetail>)Session[SELECTED_LICENSE];
                _sessionObj.Add(currentobj);
                Session[SELECTED_LICENSE] = _sessionObj;
            }
            else
            {
                List<ProvMassLicenseInfoDetail> _sessionObj = new List<ProvMassLicenseInfoDetail>();
                _sessionObj.Add(currentobj);
                Session[SELECTED_LICENSE] = _sessionObj;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminUI/List.aspx");
        }
    }
}
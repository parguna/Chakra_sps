using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocOffice365Step2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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
        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {

                ClearFields();
                BindRepeater();
                Session["EndpointUserId"] = null;
                Session["SelectedLicenseInfo"] = null;

                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    lblAllowExternalEmail.InnerText = "Enable Exernal Email";
                    UserAccPanel.Visible = true;
                    LicensePanel.Visible = false;
                }

                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        Session["AdhocActionType"] = AdHocActionType.AmendmentAdd.ToString(); ;
                        lblAllowExternalEmail.InnerText = "Enable Exernal Email";
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        Session["AdhocActionType"] = AdHocActionType.AmendmentRemove.ToString();
                        lblAllowExternalEmail.InnerText = "Disable External Email";
                    }
                    UserAccPanel.Visible = true;
                    LicensePanel.Visible = false;
                }
            }
        }

        bool UpdateBulkReqAccountDetail()
        {
            foreach (GridDataItem dataItem in AccountRepeater.Items)
            {

                if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                {
                    var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                    var SelectedAccount = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Text;
                    var endpointuserid = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Value;
                    if (Convert.ToInt32(endpointuserid) == 0)
                    {
                        return false;
                    }
                    AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                    AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                }
            }
            return true;
        }

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                trEndPointUser.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                {
                    var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", AdhocRequest.EndpointSystemId.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };

                    List<AccountReconcilation> userAccounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                    GetByAction<List<Dto.AccountReconcilation>>(
                                                                                    DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param);


                    if (userAccounts != null && userAccounts.Count > 0)
                    {
                        var userNames = new Dictionary<long, string>();

                        var items = userAccounts.Distinct();
                        if (items.Count() > 1)
                        {
                            userNames.Add(0, "--Please Select Account--");
                        }
                        foreach (var i in items)
                        {
                            userNames.Add(i.EndpointUserId, i.AccountName);
                        }
                        repeaterData.Add(item.Name, userNames);
                    }
                });

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();

            }
            else
            {
                AccountRepeater.Visible = false;
                trEndPointUser.Visible = false;
            }
        }

        public override bool Validate()
        {
            return validate() && UpdateBulkReqAccountDetail();
        }


        bool validate()
        {
            bool isValid = true;
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                if (Session["SelectedLicenseInfo"] != null)
                {
                    isValid =  true;
                }
                else
                {
                    if (!chkboxExtenalEmail.Checked)// && LicenseAutoCompleteBox.Entries.Count == 0
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, $"Please select any option for {this.AdhocRequest.AdhocActionType}.");
                        isValid = false;
                    }
                }
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }

        protected override void SetEntitlementDetail()
        {
            AdHocO365Request office365 = new AdHocO365Request();
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                office365.LicenseInfo = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
                office365.LicenseAction = Session["SelectedLicenseInfo"] != null ? LicenseActionType.Add : LicenseActionType.None;
                office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Enable : ExternalEmailActionType.None;
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                office365.LicenseInfo = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
                office365.LicenseAction = Session["SelectedLicenseInfo"] != null ? LicenseActionType.Remove : LicenseActionType.None;
                office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Disable : ExternalEmailActionType.None;
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Enable : ExternalEmailActionType.None;
            }

            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(office365);
        }
        protected void LicenseAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                chkboxRemoveLicense.Visible = false;
                chkboxRemoveLicense.Checked = false;
            }
            btnAdd.Enabled = false;
            BindServicePlanComboBox();
        }
        protected void LicenseAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (LicenseAutoCompleteBox.Entries.Count > 1)
            {
                LicenseAutoCompleteBox.Entries.Remove(LicenseAutoCompleteBox.Entries[1]);
            }
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                chkboxRemoveLicense.Visible = true;
                chkboxRemoveLicense.Checked = false;
                ServicePlanComboBox.Enabled = true;
            }
            BindServicePlanComboBox();
            btnAdd.Enabled = true;
        }

        protected void chkboxRemoveLicense_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxRemoveLicense.Checked)
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = false;
                //  btnAdd.Enabled = true;
            }
            else
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = true;
                // btnAdd.Enabled = false;

            }
        }
        protected void ServicePlanGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                chkboxRemoveLicense.Visible = false;
                var AccountSkuId = (string)((GridDataItem)e.Item).GetDataKeyValue("AccountSkuId");
                List<AdHocLicenseInfo> _sessionObj = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
                _sessionObj.RemoveAll(item => item.AccountSkuId == AccountSkuId);
                Session["SelectedLicenseInfo"] = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    ServicePlanGrid.DataSource = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
                    ServicePlanGrid.DataBind();
                }
                else
                {
                    ServicePlanGrid.Visible = false;
                    btnAdd.Enabled = false;
                }
            }
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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AdHocLicenseInfo _gridObj = new AdHocLicenseInfo();
            _gridObj.AccountSkuId = this.SelectedLicense;
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
            ServicePlanGrid.DataSource = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
            ServicePlanGrid.DataBind();
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.DataSource = string.Empty;
            ServicePlanComboBox.DataBind();
            btnAdd.Enabled = false;
            chkboxRemoveLicense.Checked = false;
            chkboxRemoveLicense.Visible = false;
        }

        void UpdateSessionObject(AdHocLicenseInfo currentobj)
        {
            if (Session["SelectedLicenseInfo"] != null)
            {
                List<AdHocLicenseInfo> _sessionObj = (List<AdHocLicenseInfo>)Session["SelectedLicenseInfo"];
                _sessionObj.Add(currentobj);
                Session["SelectedLicenseInfo"] = _sessionObj;
            }
            else
            {
                List<AdHocLicenseInfo> _sessionObj = new List<AdHocLicenseInfo>();
                _sessionObj.Add(currentobj);
                Session["SelectedLicenseInfo"] = _sessionObj;
            }
        }
        void ClearFields()
        {
            Session["SelectedLicenseInfo"] = null;
            ServicePlanGrid.Visible = false;
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.Items.Clear();
            chkboxRemoveLicense.Checked = false;
            chkboxRemoveLicense.Visible = false;
            chkboxExtenalEmail.Checked = false;
        }


    }
}
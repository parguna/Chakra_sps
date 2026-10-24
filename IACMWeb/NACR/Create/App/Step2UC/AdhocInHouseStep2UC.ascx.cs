using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocInHouseStep2UC : UIBaseStep2UC
    {

        long EndpointUserId
        {
            get { return ddlEPUserName.SelectedIndex > 0 ? Convert.ToInt32(this.ddlEPUserName.SelectedValue) : 0; }
        }

        private EndPoint _selectedEntpoint;

        private EndPoint SelectedEntpoint
        {
            get
            {
                if (_selectedEntpoint == null)
                    _selectedEntpoint = DomainApiClient<EndPoint>.GetById(AdhocRequest.EndpointSystemId.Value);
                return _selectedEntpoint;
            }
            set { _selectedEntpoint = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override bool NeedToBeSavedOnGoBack()
        {
            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
            {
                Session["AdhocrequestObj"] = AdhocRequest;
            }
            return AdhocRequest != null && AdhocRequest.GetEntitlementDetail() != null;
        }

        #region Members 

        private const string V_REQUEST = "V_REQUEST";

        #endregion

        #region Properties

        private List<KeyValuePair<string, string>> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<KeyValuePair<string, string>>)ViewState[V_REQUEST];
                }
                return new List<KeyValuePair<string, string>>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

        #endregion

        #region Events

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocINHSharedAccount sharedAccount = new AdHocINHSharedAccount();

            if (isServiceOrSharedAccountRequest() && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {
                sharedAccount.AccountName = txtAcntName.Text;
                sharedAccount.DisplayName = txtDisplayName.Text;
                sharedAccount.Description = txtDiscription.Text;
            }

            string strEntitlements = string.Empty;

            if (SelectedEntpoint.IsFunctionLevel)
                strEntitlements = string.Join("|", EntitlementRequest.Select(p => p.Key + Constants.SPLITTER_FUCNTIONS + p.Value));
            else
                strEntitlements = string.Join(",", EntitlementRequest.Select(p => p.Key));
            AdHocINHRequest entitlementDetail = new AdHocINHRequest
            {
                EntitlementType = EntitlementType.Permission,
                EndpointUserId = EndpointUserId,
                Entitlement = strEntitlements,
                AdHocINHSharedAccount = (isServiceOrSharedAccountRequest() && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount) ? sharedAccount : null
            };

            if (ddlEPUserName.SelectedItem != null)
                entitlementDetail.DisplayAccountName = ddlEPUserName.SelectedItem.Text;


            return entitlementDetail;
        }

        void ShowFunctionColumn(bool hide)
        {
            RequestGrid.MasterTableView.GetColumn("Function").Display = hide;
            RequestGrid.MasterTableView.Columns.FindByUniqueName("Function").Visible = hide;
        }
        protected override void InitializeStep()
        {
            Session["Module"] = null;
            Session["SubModule"] = null;
            Session["EndpointUserId"] = null;
            Session["AdhocrequestObj"] = AdhocRequest;
            Session["endpointSystemId"] = AdhocRequest.EndpointSystemId;
            pnlSecurityQstns.Visible = false;
            ddlEPUserName.Items.Clear();
            // AddButton.Visible = true;
            EntitlementRequest.Clear();
            FillRequestGrid();
            if (AdhocRequest != null)
            {
                ResetPage();
                Session["EndpointUserId"] = EndpointUserId;
                var request = AdhocRequest;
                RolesAutoCompleteBox.WebServiceSettings.Method = "GetEntitlementInHouse";
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest())
                {
                    SharedPanelInhouse.Visible = true;
                    GroupPanel.Visible = true;
                    OtherControl.Visible = true;
                    txtAcntName.Text = "";
                    txtDiscription.Text = "";
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        pnlSecurityQstns.Visible = true;
                    }
                }
                else
                {
                    SharedPanelInhouse.Visible = false;
                    GroupPanel.Visible = true;
                    OtherControl.Visible = true;
                }

                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    GroupPanel.Visible = true;
                    OtherControl.Visible = true;
                    RolesAutoCompleteBox.WebServiceSettings.Method = "GetEntitlementInHouseExisting";
                }
                else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    GroupPanel.Visible = true;
                    OtherControl.Visible = true;
                    RolesAutoCompleteBox.WebServiceSettings.Method = "GetAllPermissionsExceptExisting";
                }
                else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    GroupPanel.Visible = true;
                    OtherControl.Visible = false;
                    Button1.Visible = false;
                }

                divEndpointUser.Visible = !(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount);

                Button1.Visible = (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount);

                if (SelectedEntpoint.IsFunctionLevel && !SelectedEntpoint.CanAutomate)
                {
                    divFunctions.Visible = true;
                    ShowFunctionColumn(true);
                }
                else
                {
                    divFunctions.Visible = false;
                    ShowFunctionColumn(false);
                }
                //this.UsernameSpan.Visible = AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount;
                //this.permissionSpan.Visible = AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.NewAccount;
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    LoadControl();
                }

                if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
                {
                    BindAdhocRequest();
                }
            }

        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocINHRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEPUserName.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEPUserName.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                ddlEPUserName_SelectedIndexChanged(null, null);
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                {
                    var requests = new List<KeyValuePair<string, string>>();
                    foreach (var item in Entitlement.Entitlement.Split('|'))
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        if (ent.Length > 1)
                            requests.Add(new KeyValuePair<string, string>(ent[0], ent[1]));
                        else
                            requests.Add(new KeyValuePair<string, string>(ent[0], string.Empty));
                    }
                    EntitlementRequest = requests;
                }
                FillRequestGrid();
                if (Entitlement.AdHocINHSharedAccount != null)
                {
                    txtAcntName.Text = Entitlement.AdHocINHSharedAccount.AccountName;
                    txtDisplayName.Text = Entitlement.AdHocINHSharedAccount.DisplayName;
                    txtDiscription.Text = Entitlement.AdHocINHSharedAccount.Description;
                }
            }
        }

        protected void ddlEPUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["EndpointUserId"] = EndpointUserId;
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                RolesAutoCompleteBox.WebServiceSettings.Method = "GetEntitlementInHouseExisting";
            }
        }
        #endregion

        bool isServiceOrSharedAccountRequest()
        {
            return AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary;
        }

        bool IsAmendRequest()
        {
            return AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
        }

        bool IsDeleteRequest()
        {
            return AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount;
        }
        private void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<KeyValuePair<string, string>>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            divGrid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }

        protected override void afterSave()
        {
            base.afterSave();
            ClearControls();
            if (EntitlementRequest != null)
            {
                EntitlementRequest.Clear();
            }
            FillRequestGrid();
        }

        private void ClearControls()
        {
            txtAcntName.Text = "";
            txtDisplayName.Text = "";
            txtDiscription.Text = "";
            ResetPage();
        }
        private void ResetPage()
        {
            ddlFunctions?.Items?.Clear();
            RolesAutoCompleteBox?.Entries?.Clear();
            lblFunctionDescription.Text = "";
            lblPermissionDescription.Text = "";
        }

        private void ClearComboBox(RadComboBox Control)
        {
            foreach (RadComboBoxItem items in Control.Items)
            {
                items.Checked = false;
            }
        }

        private string EntiltementString(object Control)
        {
            string entiltment = "";
            RadComboBox control = (RadComboBox)Control;
            foreach (RadComboBoxItem checkedItems in control.CheckedItems)
            {
                entiltment = entiltment == "" ? checkedItems.Text : entiltment + "," + checkedItems.Text;
            }
            return entiltment;
        }

        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEPUserName.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEPUserName, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
            }
        }
        private bool ValidationMessage()
        {
            try
            {
                bool result = true;
                string Message = "Please select the ";
                List<string> lstFields = new List<string>();
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    if (isServiceOrSharedAccountRequest() && string.IsNullOrWhiteSpace(txtAcntName.Text))
                    {
                        lstFields.Add("Account Name");
                    }
                    if (isServiceOrSharedAccountRequest() && string.IsNullOrWhiteSpace(txtDisplayName.Text))
                    {
                        lstFields.Add("Display Name");
                    }
                }

                if (ddlEPUserName.SelectedIndex <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    lstFields.Add("EndPoint User");
                }
                if (lstFields.Count > 0)
                {
                    ShowAlertMessage(AlertType.Error, $"{Message} {string.Join(",", lstFields)}.");
                    return false;
                }
                if (EntitlementRequest.Count == 0 && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                    return false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        if (string.IsNullOrWhiteSpace(AdhocRequest.SecurityQuestions))
                        {
                            ShowAlertMessage(AlertType.Error, $"{Message} Service account compliance check.");
                            return false;
                        }
                    }
                    if (isServiceOrSharedAccountRequest())
                    {
                        string ValidationMsg = string.Empty;
                        result = AdhocAccessRequestValidation.ValidateAccountName(txtAcntName.Text, Convert.ToInt64(AdhocRequest.EndpointSystemId), out ValidationMsg, AdhocRequest.RequesteeUserId, AdhocRequest.AccountType);
                        if (!string.IsNullOrWhiteSpace(ValidationMsg))
                        {
                            ShowAlertMessage(AlertType.Error, ValidationMsg);
                            return result;
                        }
                        else if (SelectedEntpoint.CanAutomate && this.EntitlementRequest != null && this.EntitlementRequest.Count > 0)
                        {
                            var permissions = this.EntitlementRequest.Select(p => p.Key);
                            result = AdhocAccessRequestValidation.ValidateAccountNameInTheDependentSystemsINH(txtAcntName.Text, Convert.ToInt64(AdhocRequest.EndpointSystemId), string.Join(",", permissions), AdhocRequest.AccountType);
                            if (!result)
                            {
                                ShowAlertMessage(AlertType.Error, "Account name does not exists in the dependent systems.");
                            }
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }
        bool DoesUsernameExist(string username, long endpointSystemId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", endpointSystemId.ToString());
            parameter.Add("userName", username);
            return DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>
                (DomainApiAction.Endpoint.InhouseSystems.InhouseUserInfo.DoesExists, parameter);
        }
        private void LoadControl()
        {
            if (AdhocRequest != null)
            {
                this.ddlEPUserName.Enabled = true;
                BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                Session["EndpointUserId"] = EndpointUserId;
            }
        }
        protected void txtAcntName_TextChanged(object sender, EventArgs e)
        {

        }
        protected void RequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
            }
        }

        public override bool Validate()
        {
            return ValidationMessage();
        }

        private bool EntitlementExists(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value == value)
                        return true;
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value.Split(',').Contains(value))
                        return true;
                }
            }

            return false;
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                return;

            if (SelectedEntpoint.IsFunctionLevel && !SelectedEntpoint.CanAutomate)
            {
                AddRequestForFunctions();
            }
            else
            {
                AddRequestForPermissionOnly();
            }
            lblPermissionDescription.Text = string.Empty;
            lblFunctionDescription.Text = string.Empty;
        }

        string CheckAccountExistsInDependentSystems(long permissionId)
        {
            var dict = new Dictionary<string, string>();
            string accountName = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? txtAcntName.Text : ddlEPUserName.SelectedItem.Text;

            dict.Add("permissionId", permissionId.ToString());
            dict.Add("userId", AdhocRequest.RequesteeUserId.ToString());
            dict.Add("accountType", Convert.ToInt32(AdhocRequest.AccountType).ToString());
            dict.Add("accountName", accountName);
            var message = DomainApiClient<InhousePermission>.GetByAction<string>(DomainApiAction.Endpoint.InhouseSystems.CheckAccountExistsInDependentSystems, dict);
            return message;
        }

        void AddRequestForPermissionOnly()
        {
            if (RolesAutoCompleteBox.Entries.Count > 0)
            {
                var requests = new List<KeyValuePair<string, string>>();
                string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();

                if (SelectedEntpoint.CanAutomate)
                {
                    var permissionId = Convert.ToInt64(RolesAutoCompleteBox.Entries[0].Value);
                    var message = CheckAccountExistsInDependentSystems(permissionId);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        MasterPage.ShowAlert(AlertType.Error, message);
                        return;
                    }
                }

                string newFunctions = string.Empty;
                requests = this.EntitlementRequest;
                if (!EntitlementExists(newPermission, newFunctions))
                {
                    requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
                    this.EntitlementRequest = requests;
                    FillRequestGrid();
                    RolesAutoCompleteBox.Entries.Clear();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "The permission is already in the request list.");
                }
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select the permission.");
                return;
            }
        }
        void AddRequestForFunctions()
        {
            if (RolesAutoCompleteBox.Entries.Count > 0 && ddlFunctions.CheckedItems.Count > 0)
            {
                var requests = new List<KeyValuePair<string, string>>();
                string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
                List<string> lstFunctions = new List<string>();
                bool exists = false;
                foreach (var function in ddlFunctions.CheckedItems)
                {
                    exists = EntitlementExists(newPermission, function.Text);
                    if (exists)
                    {
                        break;
                    }
                    lstFunctions.Add(function.Text);
                }

                if (!exists)
                {
                    string newFunctions = string.Join(",", lstFunctions);
                    requests = this.EntitlementRequest;
                    requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
                    this.EntitlementRequest = requests;
                    FillRequestGrid();
                    RolesAutoCompleteBox.Entries.Clear();
                    ddlFunctions.Items.Clear();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "The permission and function are already in the request list.");
                }
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select the permission and function.");
            }
        }

        protected void RolesAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            lblPermissionDescription.Text = string.Empty;
            if (RolesAutoCompleteBox.Entries.Count > 1)
            {
                RolesAutoCompleteBox.Entries.RemoveAt(1);
                return;
            }

            if (RolesAutoCompleteBox.Entries.Count == 1)
            {
                var permissionId = Convert.ToInt64(RolesAutoCompleteBox.Entries[0].Value);
                var permission = DomainApiClient<Dto.InhousePermission>.GetById(permissionId);
                lblPermissionDescription.Text = permission != null ? permission.Description : string.Empty;
            }

            if (!SelectedEntpoint.IsFunctionLevel)
                return;

            if (RolesAutoCompleteBox.Entries.Count == 1)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointId", Convert.ToString(AdhocRequest.EndpointSystemId.Value));
                parameter.Add("permission", RolesAutoCompleteBox.Entries[0].Text);
                parameter.Add("endpointUserId", Convert.ToString(EndpointUserId));
                parameter.Add("type", Convert.ToInt32(AdhocRequest.AdhocActionType).ToString());
                var functions = DomainApiClient<InhouseFunction>.
                    GetByAction<List<InhouseFunction>>(DomainApiAction.Endpoint.InhouseSystems.GetInhouseFunctionActive, parameter);
                ddlFunctions.DataSource = functions;
                ddlFunctions.DataTextField = "FunctionName";
                ddlFunctions.DataValueField = "Id";
                ddlFunctions.DataBind();
                //foreach (RadComboBoxItem itm in ddlFunctions.Items)
                //{
                //    itm.Checked = true;
                //}
            }
            else
            {
                ddlFunctions.Items.Clear();
            }
        }

        protected void RolesAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (RolesAutoCompleteBox.Entries.Count == 0)
            {
                ddlFunctions.Items.Clear();
                lblPermissionDescription.Text = string.Empty;
            }
        }

        protected void ddlFunctions_ItemChecked(object sender, RadComboBoxItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                var functionId = Convert.ToInt64(e.Item.Value);
                var function = DomainApiClient<Dto.InhouseFunction>.GetById(functionId);
                lblFunctionDescription.Text = function != null ? function.Description : string.Empty;
            }
            else
                lblFunctionDescription.Text = string.Empty;
        }
    }
}
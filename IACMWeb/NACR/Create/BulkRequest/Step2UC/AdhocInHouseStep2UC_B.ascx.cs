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

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocInHouseStep2UC_B : UIBaseStep2UC_B
    {
        private EndPoint _selectedEntpoint;
        private EndPoint SelectedEntpoint
        {
            get
            {
                if (_selectedEntpoint == null)
                    _selectedEntpoint = DomainApiClient<EndPoint>.GetById(AdhocRequest.EndpointSystemId);
                return _selectedEntpoint;
            }
            set { _selectedEntpoint = value; }
        }
        const string V_REQUEST = "V_REQUEST";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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

        protected void RequestGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
            }
        }
        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {
                lblNewAccountWarning.Text = "";
                //SetMandatory( RoleAstreix, true);
                EntitlementRequest.Clear();
                Session["EndpointUserId"] = null;
                RolesAutoCompleteBox.Entries.Clear();
                FillRequestGrid();
                BindRepeater();
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        GroupPanelINH.Visible = true;
                    }
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    GroupPanelINH.Visible = false;
                    AddRequestButton.Visible = false;
                }
                else
                {
                    AddRequestButton.Visible = true;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    GroupPanelINH.Visible = true;
                    //SetMandatory( RoleAstreix, false);
                }
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
            } 
        }

        void ShowFunctionColumn(bool hide)
        {
            RequestGrid.MasterTableView.GetColumn("Function").Display = hide;
            RequestGrid.MasterTableView.Columns.FindByUniqueName("Function").Visible = hide;
        }

        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocINHRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                {
                    foreach (var item in Entitlement.Entitlement.Split('|'))
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        if (ent.Length > 1)
                            EntitlementRequest.Add(new KeyValuePair<string, string>(ent[0], ent[1]));
                        else
                            EntitlementRequest.Add(new KeyValuePair<string, string>(ent[0], string.Empty));
                    }
                }
                FillRequestGrid();
            }
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            lblNewAccountWarning.Text = "";
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

        private bool EntitlementExists(string key, string value)
        {
            foreach (KeyValuePair<string, string> req in EntitlementRequest)
            {
                if (req.Key == key && req.Value == value)
                    return true;
            }
            return false;
        }
        string CheckAccountExistsInDependentSystems(long permissionId)
        {
            var validationMessage = string.Empty;
            var messages = new List<string>();
            foreach (var bulkDetail in AdhocRequest.BulkRequestDetails)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("permissionId", permissionId.ToString());
                dict.Add("userId", bulkDetail.UserId.ToString());
                dict.Add("accountType", Convert.ToInt32(AccountType.User).ToString());
                var message = DomainApiClient<InhousePermission>.GetByAction<string>(DomainApiAction.Endpoint.InhouseSystems.CheckAccountExistsInDependentSystems, dict);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    messages.Add($"User:{bulkDetail.Name} {message}");
                }
            }
            if(messages.Count > 0)
            {
                validationMessage = string.Join("<br/>", messages);
            }
            return validationMessage;
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
                        lblNewAccountWarning.Text= message;
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
            }
        }
        void AddRequestForFunctions()
        {
            if (RolesAutoCompleteBox.Entries.Count > 0 && ddlFunctions.CheckedItems.Count > 0)
            {
                var requests = new List<KeyValuePair<string, string>>();
                string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
                List<string> lstFunctions = new List<string>();
                foreach (var function in ddlFunctions.CheckedItems)
                {
                    lstFunctions.Add(function.Text);
                }
                string newFunctions = string.Join(",", lstFunctions);
                requests = this.EntitlementRequest;
                if (!EntitlementExists(newPermission, newFunctions))
                {
                    requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
                    this.EntitlementRequest = requests;
                    FillRequestGrid();
                    RolesAutoCompleteBox.Entries.Clear();
                    ddlFunctions.ClearCheckedItems();
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

        void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<KeyValuePair<string, string>>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            trGrdid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
        bool UpdateBulkReqAccountDetail()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
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
                            ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Account Information."); 
                            return false;
                        }
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                Accountpanel.Visible = true;

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
                Accountpanel.Visible = false;
            }
        }

        public override bool Validate()
        {
            if ((AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && EntitlementRequest.Count <= 0))
            {
                string Message = "Please Select the ";
                string ControlMessage = "";
                if (EntitlementRequest.Count <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Permission" : ControlMessage + " and Permission";
                }
                if (ControlMessage != "")
                {
                    Message = Message + ControlMessage + (ControlMessage.IndexOf(",") > 0 ? ";" : ".");
                    ShowAlertMessage(Infrastructure.AlertType.Error, Message + ".");
                    return false;
                }
                return UpdateBulkReqAccountDetail() ;
            }
            if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && !UpdateBulkReqAccountDetail())
            {
                return false;
            }
            return UpdateBulkReqAccountDetail();
        }

        protected override void SetEntitlementDetail()
        {
            string strEntitlements = string.Empty;
            if (SelectedEntpoint.IsFunctionLevel)
                strEntitlements = string.Join("|", EntitlementRequest.Select(p => p.Key + Constants.SPLITTER_FUCNTIONS + p.Value));
            else
                strEntitlements = string.Join(",", EntitlementRequest.Select(p => p.Key));
            AdHocINHRequest request = new AdHocINHRequest
            {
                Entitlement = strEntitlements
            };
             
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
        }

        protected override void afterSave()
        {
            EntitlementRequest.Clear();
            FillRequestGrid();
        }

        protected void RolesAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (RolesAutoCompleteBox.Entries.Count == 0)
            {
                ddlFunctions.Items.Clear();
                lblPermissionDescription.Text = string.Empty;
            }
            this.lblNewAccountWarning.Text = "";
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
                parameter.Add("endpointId", Convert.ToString(AdhocRequest.EndpointSystemId));
                parameter.Add("permission", RolesAutoCompleteBox.Entries[0].Text);
                parameter.Add("endpointUserId", "0");
                parameter.Add("type", Convert.ToInt32(AdHocActionType.NewAccount).ToString());
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
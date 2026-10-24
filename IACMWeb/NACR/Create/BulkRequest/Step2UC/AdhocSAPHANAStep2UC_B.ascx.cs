using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocSAPHANAStep2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        const string V_REQUEST = "V_REQUEST";
        private List<string> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<string>)ViewState[V_REQUEST];
                }
                return new List<string>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

        protected override void InitializeStep()
        {
            Session["EndpointUserId"] = null;
            Session["ApplicationType"] = null;
            Session["SelectedPrefix"] = null;
            EntitlementRequest.Clear();
            PositionAutocomplete.Entries.Clear();
            lblReallocationMsg.Visible = false;
            FillRequestGrid();
            BindRepeater();
            if (AdhocRequest != null)
            {
                HdnfieldEndpointSystemId.Value = AdhocRequest.EndpointSystemId.ToString();
                GroupPanelSAPMSSL.Visible = true;
                LoadControl();
                if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    PositionAddRadioButton.Checked = true;
                    PositionReaalocateRadioButton.Checked = false;
                    BindRoles(AdhocRequest.ApplicationType);
                    FullPositionPanel.Visible = true;
                    BindPoistions(AdhocRequest.ApplicationType);
                    AddRequestButton.Visible = true;
                }
                else
                {
                    AddRequestButton.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    PostionAddRemovePanel.Visible = true;
                    lblReallocationMsg.Visible = PositionReaalocateRadioButton.Checked;
                }
                else
                {
                    PostionAddRemovePanel.Visible = false;
                }
                SetRequiredForPositionAndRoles();
            }
        }



        void LoadControl()
        {
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    OtherControl.Visible = false;
                }
                else
                {
                    OtherControl.Visible = true;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    PostionAddRemovePanel.Visible = true;
                }
                else
                {
                    PostionAddRemovePanel.Visible = false;
                }
            }
        }
        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocEBSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (!string.IsNullOrEmpty(Entitlement.Position) && !string.IsNullOrEmpty(Entitlement.PositionValue))
                {
                    PositionAutocomplete.Entries.Add(new AutoCompleteBoxEntry(Entitlement.Position, Entitlement.PositionValue));
                    PositionAutocomplete_EntryAdded(null, null);
                }
                PositionReaalocateRadioButton.Checked = Entitlement.IsReallocate;
                PositionAddRadioButton.Checked = !Entitlement.IsReallocate;
                if (!string.IsNullOrEmpty(Entitlement.Roles))
                    EntitlementRequest = Entitlement.Roles.Split(',').Select(l => l).ToList();
                FillRequestGrid();
            }
        }
        void FillRequestGrid()
        {
            lblReallocationMsg.Visible = PositionReaalocateRadioButton.Checked;
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<string>();
            }
            RequestGrid.Visible = this.EntitlementRequest.Count > 0 ? true : false;
            RequestGrid.DataSource = this.EntitlementRequest;
            RequestGrid.DataBind();
        }
        private void BindRepeater()
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

        protected void PositionAutocomplete_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (PositionAutocomplete.Entries.Count > 0)
            {

                if (PositionAutocomplete.Entries.Count == 2)
                {
                    PositionAutocomplete.Entries.Remove(PositionAutocomplete.Entries[1]);
                    return;
                }

                string PositionText = PositionAutocomplete.Entries[0].Text;
                var SelectedPosition = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<Dto.SAPHANAXLPositionsMatrix>(DomainApiAction.Matrix.SAPHANA.GetSAPHANAPositionMatrixByPosition,
                         new Dictionary<string, string> { { "position", PositionText }, { "endpointSystemId", AdhocRequest.EndpointSystemId.ToString() } });
                if (SelectedPosition != null)
                {
                    bool IsPositionPresentInISU = String.IsNullOrWhiteSpace(SelectedPosition.Position) ? false : true;
                    
                    if (IsPositionPresentInISU)
                        AssignPositionLable.Text = "Note: The requested position applies in both ISU";
                    
                    var positionPrefix = AdhocRequest.IsEmployee ? SelectedPosition.Prefix_Employees : SelectedPosition.Prefix_TempStaff;
                    if (string.IsNullOrWhiteSpace(positionPrefix) && PositionReaalocateRadioButton.Checked == false && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        PositionAutocomplete.Entries.Clear();
                        ShowAlertMessage(AlertType.Error, $"Position Prefix is empty. Please contact IAM Team");
                        return;
                    }
                    if (AccountRepeater != null && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        var uniqueAccounts = new List<string>();
                        foreach (GridDataItem dataItem in AccountRepeater.Items)
                        {
                            if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                            {
                                var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                                var SelectedAccount = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Text;
                                if (!string.IsNullOrWhiteSpace(SelectedAccount) && SelectedAccount.Length > 3)
                                    uniqueAccounts.Add(SelectedAccount.Substring(0, SelectedAccount.Length - 3));
                            }
                        }
                        if (uniqueAccounts != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && uniqueAccounts.Distinct().Count() > 1 && PositionReaalocateRadioButton.Checked == false)
                        {
                            PositionAutocomplete.Entries.Clear();
                            ShowAlertMessage(AlertType.Error, $"Please select the account name(s) which start with same prefix");
                            return;
                        }
                        else if (uniqueAccounts != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && uniqueAccounts.Distinct().Count() == 1 && !string.IsNullOrWhiteSpace(positionPrefix) && uniqueAccounts.FirstOrDefault() != positionPrefix && PositionReaalocateRadioButton.Checked == false)
                        {
                            PositionAutocomplete.Entries.Clear();
                            ShowAlertMessage(AlertType.Error, $"The selected Position prefix and account name(s) prefix did not match");
                            return;
                        }
                    }
                }
            }
        }

        private void BindPoistions(ApplicationType EntiltmentType)
        {
            Session["ApplicationType"] = EntiltmentType;
        }
        private void BindRoles(ApplicationType ApplicationName)
        {
            Session["ApplicationType"] = ApplicationName;
        }
        private void BindViewDetails()
        {
            string positionCode = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<string>(DomainApiAction.Matrix.SAPHANA.GetPositionSAPHANA, new Dictionary<string, string> { { "ID", Convert.ToInt32(PositionAutocomplete.Entries[0].Value).ToString() } });
            string postionCode = positionCode;
            List<string> data = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAPHANA.GetSAPHANARolesByPosition, new Dictionary<string, string> { { "position", postionCode }, { "endpointSystemId", Convert.ToInt32(AdhocRequest.EndpointSystemId).ToString() } });
            ViewTechnicalSource.DataSource = data;
            ViewTechnicalSource.DataBind();
            ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewTechnicalSourceWindow.VisibleOnPageLoad = false;
        }
        protected void ViewDeatilsLinkButton_Click(object sender, EventArgs e)
        {
            if (PositionAutocomplete.Entries.Count > 0)
            {
                BindViewDetails();
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select position.");
            }
        }

        protected void RoleAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (RoleAutoCompleteBox.Entries.Count > 0)
            {
                if (RoleAutoCompleteBox.Entries.Count == 2)
                {
                    RoleAutoCompleteBox.Entries.Remove(RoleAutoCompleteBox.Entries[1]);
                    return;
                }
            }
        }

        protected override void afterSave()
        {
            base.afterSave();
            if (EntitlementRequest != null)
            {
                EntitlementRequest.Clear();
            }
            FillRequestGrid();
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
        protected void RequestGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label EntitlementLabel = (Label)item.FindControl("EntitlementLabel");
                Label TechnicalRoleLabel = (Label)item.FindControl("TechnicalRoleLabel");
                if (EntitlementLabel != null && TechnicalRoleLabel != null)
                {
                    var roles = DomainApiClient<Dto.SAPHANAXLRolesMatrix>.GetByAction<IList<SAPHANAXLRolesMatrix>>(DomainApiAction.Matrix.SAPHANA.BindViewDetailsSAPHANA, new Dictionary<string, string> { { "Role", EntitlementLabel.Text } });
                    TechnicalRoleLabel.Text = string.Join(",", roles.Select(lq => lq.TechnicalRole));
                }
            }
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (RoleAutoCompleteBox.Entries.Count != 0)
            {
                List<string> requests = new List<string>();
                string newRequest = RoleAutoCompleteBox.Text.Replace(";", "").Trim();
                requests = this.EntitlementRequest;
                if (!EntitlementRequest.Contains(newRequest) && !string.IsNullOrEmpty(newRequest))
                {
                    requests.Add(newRequest);
                    this.EntitlementRequest = requests;
                    FillRequestGrid();
                    RoleAutoCompleteBox.Entries.Clear();
                }
                else
                {
                    ShowAlertMessage(AlertType.Error, "The role is already in the request list.");
                }
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "Please select the role name.");
            }
        }

        private bool UpdateBulkReqAccountDetail()
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
                            ShowAlertMessage(AlertType.Error, "Please enter the Account Information.");
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

        public override bool Validate()
        {
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (PositionAutocomplete.Entries.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select position.");
                    return false;
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                if (PositionReaalocateRadioButton.Checked && PositionAutocomplete.Entries.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select the position");
                    return false;
                }
                else if (RequestGrid.Items.Count == 0 && PositionAutocomplete.Entries.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select the position or role.");
                    return false;
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                if (RequestGrid.Items.Count == 0 && PositionAutocomplete.Entries.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select position or role");
                    return false;
                }
            }

            return UpdateBulkReqAccountDetail();
        }

        protected override void SetEntitlementDetail()
        {
            AdHocSAPHANARequest request = new AdHocSAPHANARequest();
            request.Roles = string.Join(",", EntitlementRequest);
            if (PositionAutocomplete.Entries.Count > 0)
            {
                string prefix = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<string>(DomainApiAction.Matrix.SAPHANA.GetPrefixSAPHANA,
                new Dictionary<string, string> { { "Id", Convert.ToInt32(PositionAutocomplete.Entries[0].Value).ToString() } });
                string[] prefixSplitter = prefix.Split(',');
                request.PositionPrefix = AdhocRequest.IsEmployee == true ? prefixSplitter[0] : prefixSplitter[1];
                request.Position = PositionAutocomplete.Entries[0].Text;
                request.PositionValue = PositionAutocomplete.Entries[0].Value;
                request.IsReallocate = PositionReaalocateRadioButton.Checked;
            }
            var ep = DomainApiClient<Dto.EndPoint>.GetById(AdhocRequest.EndpointSystemId);
            request.ApplicationName = ep.ApplicationCode;
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
        }

        protected void PositionAddRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            PositionAutocomplete.Entries.Clear();
            SetRequiredForPositionAndRoles();
            lblReallocationMsg.Visible = PositionReaalocateRadioButton.Checked;
        }

        void SetRequiredForPositionAndRoles()
        {
            lblPosition.Attributes["class"] = "col-xs-3 control-label";
            if (IsPositionMandatory())
                lblPosition.Attributes["class"] = "col-xs-3 control-label required";
        }

        bool IsPositionMandatory()
        {
            if (AdhocRequest == null)
                return false;

            return (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount) || (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && PositionReaalocateRadioButton.Checked);
        }

        protected void ddlAccounts_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            PositionAutocomplete.Entries.Clear();
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                EntitlementRequest = new List<string>();
            FillRequestGrid();
        }

    }
}
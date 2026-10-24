using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSapMsslStep2UC : UIBaseStep2UC
    {
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //SetRequiredForPositionAndRoles();
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

            return (AdhocRequest.AccountType == AccountType.User && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount) || (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && PositionReaalocateRadioButton.Checked);
        }

        protected override void InitializeStep()
        {
            trSecurityQstns.Visible = false;

            Session["EndpointUserId"] = null;
            Session["UserAccountName"] = null;
            Session["ApplicationType"] = null;
            Session["SelectedPrefix"] = null;
            Session["employeeType"] = null;
            EntitlementRequest.Clear();
            PositionAutocomplete.Entries.Clear();
            txtDiscription.Attributes.Add("maxlength", "40");
            FillRequestGrid();
            if (AdhocRequest != null)
            {
                SpecialAccountWarning.ShowWarningMessage(AdhocRequest);
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    User user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
                    Session["employeeType"] = user.EmployeeType;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary))
                {
                    SharedPanelSAPMSSL.Visible = true;
                    GroupPanelSAPMSSL.Visible = true;
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        trSecurityQstns.Visible = true;
                    }
                    BindSAPUserType(AdhocRequest.AccountType);
                    txtAcntName.Text = "";
                    txtDiscription.Text = "";
                }
                else
                {
                    SharedPanelSAPMSSL.Visible = false;
                    GroupPanelSAPMSSL.Visible = true;
                }
                LoadControl();
                if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    PositionAddRadioButton.Checked = true;
                    PositionReaalocateRadioButton.Checked = false;
                    BindRoles(AdhocRequest.ApplicationType);
                    FullPositionPanel.Visible = true;
                    BindPoistions(AdhocRequest.ApplicationType);

                }
                AddRequestButton.Visible = AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount;

                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    PostionAddRemovePanel.Visible = true;
                }
                else
                {
                    PostionAddRemovePanel.Visible = false;
                }
                //bool visible = !(AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount);
                //SetMandatory(this.Role, visible);
                //SetMandatory(this.Position, visible);

                if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
                SetRequiredForPositionAndRoles();
            }
        }

        void BindAdhocRequest()
        {
            var Entitlement = (AdHocMSSLRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (EndPointUserDropDownList.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    EndPointUserDropDownList.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                EndPointUserDropDownList_SelectedIndexChanged(null, null);
                if (!string.IsNullOrEmpty(Entitlement.Position))
                {
                    PositionAutocomplete.Entries.Add(new AutoCompleteBoxEntry(Entitlement.Position, Entitlement.PositionValue));
                    PositionAutocomplete_EntryAdded(null, null);
                }
                PositionReaalocateRadioButton.Checked = Entitlement.IsReallocate;
                PositionAddRadioButton.Checked = !Entitlement.IsReallocate;
                if (!string.IsNullOrEmpty(Entitlement.Roles))
                    EntitlementRequest = Entitlement.Roles.Split(',').Select(l => l).ToList();
                FillRequestGrid();
                if (Entitlement.AdHocSAPMSSLSharedAccount != null)
                {
                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPMSSLSharedAccount.SAPUserType)) != null)
                    {
                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPMSSLSharedAccount.SAPUserType)).Selected = true;
                    }
                    txtAcntName.Text = Entitlement.AdHocSAPMSSLSharedAccount.AccountName;
                    txtDiscription.Text = Entitlement.AdHocSAPMSSLSharedAccount.Description;
                }

            }
        }

        protected override void afterSave()
        {
            EntitlementRequest =null;
            PositionReaalocateRadioButton.Checked = false;
            txtAcntName.Text = "";
            txtDiscription.Text = "";
            PositionAutocomplete.Entries.Clear();
            SAPUserTypeDropDown.SelectedIndex = -1;
            if (this.AdhocRequest != null && (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
            {
                PositionAddRadioButton.Checked = true;
                PositionReaalocateRadioButton.Checked = false;
                lblReallocationMsg.Visible = false;
            }
            FillRequestGrid();
        }

        void BindViewDetails()
        {
            string[] positionCodeSplitter = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<string>(DomainApiAction.Matrix.SAP_MSSL.GetPositionCode, new Dictionary<string, string> { { "ID", Convert.ToInt32(PositionAutocomplete.Entries[0].Value).ToString() } }).Split(',');
            string postionCode = positionCodeSplitter != null && AdhocRequest.ApplicationType == ApplicationType.MSSL_ISU ? positionCodeSplitter[0] : positionCodeSplitter[1];
            List<string> data = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_MSSL.GetRolesByPosition, new Dictionary<string, string> { { "position", postionCode }, { "endpointSystemId", Convert.ToInt32(AdhocRequest.EndpointSystemId).ToString() } });
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
                MasterPage.ShowAlert(AlertType.Warning, "Please select position.");
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
                SAPXLMSSLPositionsMatrix SelectedPosition = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<Dto.SAPXLMSSLPositionsMatrix>(DomainApiAction.Matrix.SAP_MSSL.GetMSSLPositionMatrixByPosition,
                         new Dictionary<string, string> { { "position", PositionText } });
                if (SelectedPosition != null)
                {
                    bool IsPositionPresentInISU = String.IsNullOrWhiteSpace(SelectedPosition.Position_MSP) ? false : true;
                    bool IsPositionPresentInCRM = string.IsNullOrWhiteSpace(SelectedPosition.Position_MCP) ? false : true;
                    if (IsPositionPresentInCRM && IsPositionPresentInISU)
                        AssignPositionLable.Text = "Note: The requesting Position applies in both ISU and CRM.";
                    else if (IsPositionPresentInISU)
                        AssignPositionLable.Text = "Note: The requesting position applies in ISU only.";
                    else if (IsPositionPresentInCRM)
                        AssignPositionLable.Text = "Note: The requesting position applies in CRM only.";
                }
            }
        }

        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["SelectedPrefix"] = null;
            Session["EndpointUserId"] = null;
            Session["UserAccountName"] = null;
            if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                Session["UserAccountName"] = EndPointUserDropDownList.SelectedItem.Text;
                BindRoles(AdhocRequest.ApplicationType);
                BindPoistions(AdhocRequest.ApplicationType);
            }
            
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                Session["SelectedPrefix"] = GetPrefix();
            }
            
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                PositionAutocomplete.Entries.Clear();
                EntitlementRequest = new List<string>();
                RoleAutoCompleteBox.Entries.Clear();
            }

            FillRequestGrid();
        }
        void LoadControl()
        {
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    trEndPointUser.Visible = true;
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                }
                else
                {
                    trEndPointUser.Visible = false;
                }
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
        void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
            Session["EndpointUserId"] = null;
            if (list != null && list.AccountReconDictionary != null)
            {
                EndPointUserDropDownList.Items.Clear();
                DropDownHelper.DropDownListBind(EndPointUserDropDownList, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                    BindRoles(AdhocRequest.ApplicationType);
                    BindPoistions(AdhocRequest.ApplicationType);
                }
            }
        }

        void BindRoles(ApplicationType ApplicationName)
        {
            bool BindAllRoles = true;
            Session["ApplicationType"] = ApplicationName;

            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    BindAllRoles = false;
                }
            }

            if (BindAllRoles)
            {
                RoleAutoCompleteBox.WebServiceSettings.Method = "GetAllRolesMSSL";
            }
            else
            {
                RoleAutoCompleteBox.WebServiceSettings.Method = "GetExistingRolesMSSL";
            }

        }
        string GetPrefix()
        {
            if (EndPointUserDropDownList.SelectedIndex <= 0 || (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedItem.Text.Length < 4))
                return string.Empty;
            else if (AdhocRequest.AccountType == AccountType.User)
                return EndPointUserDropDownList.SelectedItem.Text.Substring(0, 4);
            else
                return string.Empty;
        }
        void BindPoistions(ApplicationType EntiltmentType)
        {
            bool BindAllPositions = true;
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    BindAllPositions = false;
                }
            }

            Session["ApplicationType"] = EntiltmentType;
            Session["SelectedPrefix"] = GetPrefix();
            if (BindAllPositions)
            {
                PositionAutocomplete.WebServiceSettings.Method = "GetAllPositionMSSL";
            }
            else
            {
                PositionAutocomplete.WebServiceSettings.Method = "GetExistingPositionMSSL";
            }
        }
        void BindSAPUserType(AccountType accountType)
        {
            try
            {
                SAPUserTypeDropDown.Items.Clear();
                SAPUserTypeDropDown.Items.Insert(0, new ListItem("-- Select SAP UserType --", "-1"));

                foreach (SAPUserType item in Enum.GetValues(typeof(SAPUserType)))
                {
                    if (item == SAPUserType.Reference || (accountType == AccountType.Shared && item == SAPUserType.Communication))
                        continue;

                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    if (accountType == AccountType.Service && (((SAPUserType)item) == SAPUserType.Service || ((SAPUserType)item) == SAPUserType.System))
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);
                    }
                    if ((accountType == AccountType.Shared || accountType == AccountType.Secondary) && item == SAPUserType.Dialog)
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);
                        SAPUserTypeDropDown.SelectedIndex = 1;
                        //SAPUserTypeDropDown.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<string>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            trGrdid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
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
                //AddRequestButton.Enabled = true;
            }
            //else { AddRequestButton.Enabled = false; }
        }

        protected void AutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            //RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            //if (box.Entries.Count > 0)
            //{
            //    AddRequestButton.Enabled = true;
            //}
            //else
            //{
            //    AddRequestButton.Enabled = false;
            //}
        }

        public override bool Validate()
        {
            var request = this.AdhocRequest;
            if (!ValidateDataForSave())
                return false;

            if (request.AdhocActionType == AdHocActionType.NewAccount && (request.AccountType == AccountType.Service || request.AccountType == AccountType.Shared || request.AccountType == AccountType.Secondary))
            {
                string msgValidation = string.Empty;
                var flag = AdhocAccessRequestValidation.ValidateAccountName(txtAcntName.Text, request.EndpointSystemId.Value, out msgValidation,request.RequesteeUserId,request.AccountType);

                if (!flag && !string.IsNullOrEmpty(msgValidation))
                {
                    ShowAlertMessage(AlertType.Error, msgValidation);
                    return false;
                }
                var message = AdhocAccessRequestValidation.ValidateAccountNameForMSSLAndEBS(request.AccountType, txtAcntName.Text);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    ShowAlertMessage(AlertType.Warning, message);
                    return false;
                }
            }

            return true;
        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocMSSLRequest request = new AdHocMSSLRequest();
            User user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);

            request.EndpointUserId = EndPointUserDropDownList.SelectedIndex > 0 ? Convert.ToInt32(EndPointUserDropDownList.SelectedValue) : 0;

            if (EndPointUserDropDownList.SelectedItem != null)
            {
                request.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;
            }


            request.AccName = EndPointUserDropDownList.SelectedIndex > 0 ? EndPointUserDropDownList.SelectedItem.Text : string.Empty;
            request.Roles = string.Join(",", EntitlementRequest);
            if (PositionAutocomplete.Entries.Count > 0)
            {
                string prefix = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<string>(DomainApiAction.Matrix.SAP_MSSL.GetPrefix,
                    new Dictionary<string, string> { { "Id", Convert.ToInt32(PositionAutocomplete.Entries[0].Value).ToString() } });
                string[] prefixSplitter = prefix.Split(',');

                request.PositionPrefix = user.FullTimeEmployee == true ? prefixSplitter[0] : prefixSplitter[1];
                request.Position = PositionAutocomplete.Entries[0].Text;
                request.PositionValue = PositionAutocomplete.Entries[0].Value;
                request.IsReallocate = PositionReaalocateRadioButton.Checked;
            }
            request.ApplicationName = AdhocRequest.ApplicationType == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : AdhocRequest.ApplicationType == ApplicationType.MSSL_CRM ? Constants.MSSLMCP : Constants.MSSLBI;
            AdHocSAPSharedAccount sharedDetails = new AdHocSAPSharedAccount();
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary))
            {
                if (SAPUserTypeDropDown.SelectedIndex > 0)
                {
                    sharedDetails.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                }
                sharedDetails.AccountName = txtAcntName.Text;
                sharedDetails.Description = txtDiscription.Text;
                request.AccName = txtAcntName.Text;
            }
            request.AdHocSAPMSSLSharedAccount = (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? sharedDetails : null;


            

            return request;

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
                    var roles = DomainApiClient<Dto.SAPXLMSSLRolesMatrix>.GetByAction<IList<SAPXLMSSLRolesMatrix>>(DomainApiAction.Matrix.SAP_MSSL.BindViewDetails, new Dictionary<string, string> { { "Role", EntitlementLabel.Text } });
                    TechnicalRoleLabel.Text = string.Join(",", roles.Select(lq => lq.TechnicalRole));
                }
            }
        }
        bool ValidateDataForSave()
        {
            if (AdhocRequest.AccountType == AccountType.User)
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
                    if (EndPointUserDropDownList.SelectedIndex <= 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select the account.");
                        return false;
                    }
                    else if (PositionReaalocateRadioButton.Checked && PositionAutocomplete.Entries.Count == 0)
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
                else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (EndPointUserDropDownList.SelectedIndex <= 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select the account.");
                        return false;
                    }
                }
            }
            else
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    if (SAPUserTypeDropDown.SelectedIndex <= 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select SAP user type.");
                        return false;
                    }
                    else if (string.IsNullOrWhiteSpace(txtAcntName.Text))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please enter the account name.");
                        txtAcntName.Focus();
                        return false;
                    }
                    else if (RequestGrid.Items.Count == 0 && PositionAutocomplete.Entries.Count == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select the position or role.");
                        return false;
                    }
                    else if (AdhocRequest.AccountType == AccountType.Service && string.IsNullOrWhiteSpace(AdhocRequest.SecurityQuestions))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please enter Service account compliance check.");
                        return false;
                    }
                }
                else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    if (EndPointUserDropDownList.SelectedIndex <= 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select the account.");
                        return false;
                    }
                    else if (PositionReaalocateRadioButton.Checked && PositionAutocomplete.Entries.Count == 0)
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
                else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (EndPointUserDropDownList.SelectedIndex <= 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select the account.");
                        return false;
                    }
                }
            }
            return true;
        }
        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (RoleAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the role.");
                return;
            }

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

        protected void PositionAddRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            PositionAutocomplete.Entries.Clear();
            SetRequiredForPositionAndRoles();
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                Session["SelectedPrefix"] = PositionReaalocateRadioButton.Checked ? null : GetPrefix();
                lblReallocationMsg.Visible = PositionReaalocateRadioButton.Checked;
            }
        }

        protected void PositionAutocomplete_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if(PositionAutocomplete.Entries.Count > 0)
            {
                PositionAutocomplete.Entries.Clear();
            }
        }
    }
}
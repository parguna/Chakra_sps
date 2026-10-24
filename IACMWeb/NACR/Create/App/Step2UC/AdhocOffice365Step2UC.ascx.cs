using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocOffice365Step2UC : UIBaseStep2UC
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public Dictionary<string, string> SelectedMembers
        {
            get
            {
                if (ViewState["V_SELECTEDMEMBERS"] != null)
                {
                    return (Dictionary<string, string>)ViewState["V_SELECTEDMEMBERS"];
                }
                return null;
            }
            set
            {
                ViewState["V_SELECTEDMEMBERS"] = value;
            }
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

        Dto.Office365UserInfo getUserinfo(long endpointUserId, AccountType accountType, long endpointSystemId)
        {
            return DomainApiClient<Dto.Office365UserInfo>.GetByAction<Dto.Office365UserInfo>(DomainApiAction.Endpoint.O365.GetO365UserByUserId,
                new Dictionary<string, string> {    { "userId", endpointUserId.ToString() },
                                                    { "accountType", ((int)accountType).ToString() },
                                                    { "endpointsystemid", endpointSystemId.ToString() }
                                                });
        }
        protected override void afterSave()
        {
            SelectedMembers = null;
            MemeberGrid.Visible = false;
            MemeberGrid.DataSource = null;
            MemeberGrid.DataBind();
            base.afterSave();
            Session["AdHocLicenseInfo"] = null;
            this.SelectedMembers = null;
            MembersAutoCompletebox.Entries.Clear();
            txtAlias.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.Items.Clear();
        }

        protected override void InitializeStep()
        {

            ClearFields();
            if (AdhocRequest != null)
            {
                Session["AdHocLicenseInfo"] = null;
                ServicePlanGrid.Visible = false;
                LoadControl();
                if (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary)
                {
                    btnAddMember.Visible = true;
                    btnAddMember.Enabled = true;
                    Session["EndpointSystemId"] = AdhocRequest.EndpointSystemId;
                    if (ddlEndpointUser.SelectedIndex != 0)
                    {
                        LicenseAutoCompleteBox.Enabled = true;
                        Dto.Office365UserInfo userinfo = getUserinfo(Convert.ToInt32(ddlEndpointUser.SelectedValue), AdhocRequest.AccountType, (int)this.AdhocRequest.EndpointSystemId);
                        Dto.Office365UserServicePlan obj = userinfo?.ServicePlanObject;
                        if (obj != null)
                        {
                            List<Dto.ServicePlanDetail> _existingServicePlanDetails = obj.PlanDetails.Where(item => item.ProvisioningStatus != "Disabled").ToList();
                            Session["ExistingServicePlanDetail"] = _existingServicePlanDetails;
                        }
                    }
                }
                else if (AdhocRequest.AccountType == AccountType.User)
                {
                    btnAddMember.Visible = false;
                    btnAddMember.Enabled = false;
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        if (ddlEndpointUser.SelectedIndex != 0)
                        {
                            LicenseAutoCompleteBox.Enabled = true;
                            Dto.Office365UserInfo userinfo = getUserinfo(Convert.ToInt32(ddlEndpointUser.SelectedValue), AccountType.User, (int)this.AdhocRequest.EndpointSystemId);
                            Dto.Office365UserServicePlan obj = userinfo?.ServicePlanObject;
                            if (obj != null)
                            {
                                List<Dto.ServicePlanDetail> _existingServicePlanDetails = obj.PlanDetails.Where(item => item.ProvisioningStatus != "Disabled").ToList();
                                Session["ExistingServicePlanDetail"] = _existingServicePlanDetails;
                            }

                        }
                    }
                }

                if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    trEndPointUser.Visible = true;
                }
                else
                {
                    trEndPointUser.Visible = false;
                }


                if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
                {
                    BindAdhocRequest();
                }
            }
        }

        void BindAdhocRequest()
        {
            var Entitlement = (Dto.AdHocO365Request)AdhocRequest.GetEntitlementDetail();
           
        }

        void ClearFields()
        {
            Session["AdHocLicenseInfo"] = null;
            this.SelectedMembers = null;
            MemeberGrid.Visible = false;
            btnAddMember.Enabled = false;
            btnAdd.Enabled = false;
            ServicePlanGrid.Visible = false;
            MembersAutoCompletebox.Entries.Clear();
            txtAlias.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            //ddlEndpointUser.Items.Clear();
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.Items.Clear();
            chkboxRemoveLicense.Checked = false;
            chkboxRemoveLicense.Visible = false;
            chkboxExtenalEmail.Checked = false;


        }

        void LoadControl()
        {

            if (AdhocRequest != null)
            {
                var accountRecon = BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);

                if (AdhocRequest.AccountType == AccountType.User)
                {
                    if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                    {

                        LicenseAutoCompleteBox.Enabled = false;
                        UserAccPanel.Visible = true;
                        SharedAccPannel.Visible = false;
                        //V5 - Kanchana wants to hide this option
                        LicensePanel.Visible = false;
                        CommonPanel.Visible = true;
                        MemberPanel.Visible = false;
                        ConversionPanel.Visible = false;
                        if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            Session["AdhocActionType"] = (int)AdHocActionType.AmendmentAdd;
                            lblAllowExternalEmail.InnerText = "Enable Exernal Email";
                        }
                        else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            Session["AdhocActionType"] = (int)AdHocActionType.AmendmentRemove;
                            lblAllowExternalEmail.InnerText = "Disable External Email";
                        }
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        CommonPanel.Visible = false;
                        LicensePanel.Visible = false;
                        MemberPanel.Visible = false;
                        UserAccPanel.Visible = true;
                        ConversionPanel.Visible = false;
                        lblAllowExternalEmail.InnerText = "Enable Exernal Email";
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        CommonPanel.Visible = true;
                        LicensePanel.Visible = false;
                        MemberPanel.Visible = false;
                        UserAccPanel.Visible = false;
                        ConversionPanel.Visible = false;

                    }
                }
                if (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        LicensePanel.Visible = false;
                        SharedAccPannel.Visible = true;
                        MemberPanel.Visible = true;
                        MembersAutoCompletebox.Enabled = true;
                        CommonPanel.Visible = false;
                        UserAccPanel.Visible = false;
                        ConversionPanel.Visible = false;
                        InternalRadioButton.InnerText = MasterConfigHelper.RegularMailboxText;
                        SharedRadioButton.InnerText = MasterConfigHelper.SharedMailboxText;
                        trMailConversionType.Visible = false;
                        Session["AdhocActionType"] = (int)AdHocActionType.NewAccount;

                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                        LicenseAutoCompleteBox.Enabled = ddlEndpointUser.SelectedIndex == 0 ? false : true;
                        SharedAccPannel.Visible = false;
                        UserAccPanel.Visible = false;
                        LicensePanel.Visible = false;
                        MemberPanel.Visible = false;
                        ConversionPanel.Visible = true;
                        trMailConversionType.Visible = false;
                        Button1.Enabled = true;
                        if (ddlEndpointUser.SelectedIndex > 0)
                        {

                            trMailConversionType.Visible = true;

                            var o365user = getUserinfo(Convert.ToInt32(this.ddlEndpointUser.SelectedValue), AdhocRequest.AccountType, (int)this.AdhocRequest.EndpointSystemId);
                            if (o365user != null && (o365user.ServicePlanObject != null) && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                            {
                                LblMailBoxConversionType.InnerText = "Convert to Internal Shared Mail";
                                chkboxMailConversionType.Checked = true;
                            }
                            else if (o365user != null && (o365user.ServicePlanObject == null) && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                            {
                                LblMailBoxConversionType.InnerText = "Convert to License Shared Mail";
                                chkboxMailConversionType.Checked = true;
                            }
                            else
                            {
                                trMailConversionType.Visible = false;
                                chkboxMailConversionType.Checked = false;
                                ShowAlertMessage(Infrastructure.AlertType.Warning, $"{AdhocRequest.AdhocActionType.ToString()} is not applicable for selected mail");
                                Button1.Enabled = false;
                            }
                        }
                        CommonPanel.Visible = true;
                        if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            Session["AdhocActionType"] = (int)AdHocActionType.AmendmentAdd;
                        }
                        else
                        {
                            Session["AdhocActionType"] = (int)AdHocActionType.AmendmentRemove;
                        }
                    }

                    else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        CommonPanel.Visible = true;
                        MemberPanel.Visible = false;
                        SharedAccPannel.Visible = false;
                        UserAccPanel.Visible = false;
                        LicensePanel.Visible = false;
                        ConversionPanel.Visible = false;
                    }
                }
            }
        }

        Dto.AccountReconDto BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.AccountReconDto>("GetADUsersByDIUserId", dict);
            Session["EndpointUserId"] = null;
            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
                }

                if (list.EntitlementReconcilations != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {

                }
            }
            return list;

        }


        public override bool Validate()
        {
            bool result = false;
            string Message = "Please enter";
            string ControlMessage = "";

            if (AdhocRequest.AccountType == AccountType.Shared)
            {
                if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                {
                    if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Display Name" : ControlMessage + ", Display Name";
                    }
                    if (string.IsNullOrWhiteSpace(txtAlias.Text))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Alias" : ControlMessage + ", Alias";
                    }
                    else
                    {
                        string ValidationMsg = string.Empty;
                        var length = MasterConfigHelper.Office365AccountNameMinimumLength;
                        if (txtAlias.Text.Trim().Length < length)
                        {
                            ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, $"Account Name is too short. Minimum length should be {length}.");
                            return false;
                        }
                        var AccountName = string.Format("{0}@{1}", txtAlias.Text.Trim(), MasterConfigHelper.O365EmailDomainName);
                        var EndpointSystemId = this.AdhocRequest.EndpointSystemId.Value;
                        var DiUserId = AdhocRequest.RequesteeUserId;
                        var accountType = this.AdhocRequest.AccountType;
                        //var flag = AdhocAccessRequestValidation.ValidateAccountName(string.Format("{0}@{1}", txtAlias.Text.Trim(), MasterConfigHelper.O365EmailDomainName), this.AdhocRequest.EndpointSystemId.Value, out msgValidation, AdhocRequest.RequesteeUserId, this.AdhocRequest.AccountType);
                        #region 
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("accountName", AccountName);
                        parameter.Add("EndpointSystemId", EndpointSystemId.ToString());

                        var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                                    (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);
                        AccountName = txtAlias.Text.Trim();
                        
                        string spc = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";
                        if (accountRecon != null && (accountRecon.IsLocked == false && accountRecon.IsDeleted == false))
                        {
                            ShowAlertMessage(AlertType.Error, "Account Name already exist, please try another name.");
                            return false;
                        }
                        else if (accountRecon != null && (accountRecon.IsLocked == true && accountRecon.IsDeleted == true))
                        {
                            ValidationMsg = string.Empty;
                            if (accountRecon.UserId == null)
                            {
                                ShowAlertMessage(AlertType.Error, "Account Name already exists as inactive status and is not mapped. \nPlease contact IAM team for further assistance.");
                                return false;
                            }

                            List<long> UserIds = new List<long>();

                            if (DiUserId > 0 && accountRecon.UserId.HasValue)
                            {
                                if (accountType == AccountType.Service || accountType == AccountType.Shared)
                                {
                                    UserIds = DomainApiClient<Dto.User>.GetByAction<List<long>>(DomainApiAction.User.GetSameOUUserIDs, new Dictionary<string, string> { { "DiUserId", DiUserId.ToString() } });

                                    if (UserIds.Contains(DiUserId))
                                    {
                                        UserIds.Remove(DiUserId);
                                    }
                                }

                                if (accountRecon.UserId.Value != DiUserId && !UserIds.Contains(accountRecon.UserId.Value))
                                {
                                    var usr = DomainApiClient<Dto.User>.GetById(accountRecon.UserId.Value);
                                    if (usr != null)
                                    {
                                        ShowAlertMessage(AlertType.Error, $"Account Name already exists as inactive status and mapped to {usr.DisplayName}. \nPlease contact IAM team for further assistance.");
                                        return false;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < spc.Length; i++)
                        {
                            if (AccountName.Contains(spc[i]))
                            {
                                ShowAlertMessage(AlertType.Error, "Special characters not allowed in Account Name.");
                                return false;
                            }
                        }

                        if (AccountName.Any(x => Char.IsWhiteSpace(x)))
                        {
                            ShowAlertMessage(AlertType.Error, "Space is not allowed in Account Name.");
                            return false;
                        }
                        #endregion
                        
                    }
                    if (!(this.SelectedMembers != null && this.SelectedMembers.Count > 0))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Members" : ControlMessage + ", Members";
                    }
                    
                }
                if (!(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                {
                    if (ddlEndpointUser.SelectedIndex <= 0)
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + ", EndPoint User";
                    }
                    if (!chkboxMailConversionType.Checked && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Mailbox Conversion Type" : ControlMessage + ", Mailbox Conversion Type";

                    }

                }
            }
            else if (AdhocRequest.AccountType == AccountType.User)
            {
                if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                {
                    if (ddlEndpointUser.SelectedIndex <= 0)
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + ", EndPoint User";

                }
                if (!(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                {
                    if (ddlEndpointUser.SelectedIndex <= 0)
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + ", EndPoint User";

                }
            }

            if (ControlMessage == "")
            {

                List<Dto.AdHocLicenseInfo> selecteditems = new List<Dto.AdHocLicenseInfo>();
                if (Session["AdHocLicenseInfo"] != null)
                {
                    selecteditems = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                }
                if (this.SelectedMembers == null)
                {
                    this.SelectedMembers = new Dictionary<string, string>();
                }

                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {

                    if (AdhocRequest.AccountType == AccountType.Shared)
                    {
                        if (!chkboxMailConversionType.Checked)
                        {
                            Message = "";
                            ControlMessage = "Please select any option for amendment.";
                        }
                    }
                    if (AdhocRequest.AccountType == AccountType.User)
                    {
                        if(AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            var dict = new Dictionary<string, string>();
                            dict.Add("userid", AdhocRequest.RequesteeUserId.ToString());
                            dict.Add("mail", ddlEndpointUser.SelectedItem.Text.Trim());
                            var isAllowed = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>("IsAllwedExternalEmailAccess", dict);
                            if(isAllowed == false)
                            {
                                Message = "";
                                ControlMessage = $"Cannot request Enable Exernal Email. Exernal Email is already Enabled for the selected account.";
                            }
                        }

                        if (!chkboxExtenalEmail.Checked)//Commented because Kanchana dont wants this feature && LicenseAutoCompleteBox.Entries.Count == 0 && selecteditems.Count == 0)
                        {
                            Message = "";
                            ControlMessage = $"Please select any option for {this.AdhocRequest.AdhocActionType}.";
                        }
                    }
                }
                if (LicenseAutoCompleteBox.Entries.Count > 0)
                {
                    Message = "";
                    ControlMessage = "Please click add button to add the selected licenses before clicking save";
                }
                if (MembersAutoCompletebox.Entries.Count > 0)
                {
                    Message = "";
                    ControlMessage = "Please click add button to add the selected member before clicking save";
                }
            }
            if (ControlMessage != "")
            {
                Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                ShowAlertMessage(Infrastructure.AlertType.Error, Message + ".");
                result = false;
            }
            else if (txtAlias.Visible)
            {
                if (!ValidateAliasName())
                    return false;
                return true;
            }
            else
            {

                result = true;

            }

            return result;
        }

        bool ValidateAliasName()
        {

            if (txtAlias.Text.Contains(" "))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Space not allowed in Alias name.");
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            if (ddlEndpointUser != null && ddlEndpointUser.SelectedIndex > 0)
            {
                if (this.AdhocRequest.AccountType == AccountType.Shared || this.AdhocRequest.AccountType == AccountType.Secondary)
                {

                    trMailConversionType.Visible = false;
                    Button1.Enabled = true;

                    if (ddlEndpointUser.SelectedIndex > 0)
                    {
                        trMailConversionType.Visible = true;

                        var o365user = getUserinfo(Convert.ToInt32(this.ddlEndpointUser.SelectedValue), this.AdhocRequest.AccountType, (int)this.AdhocRequest.EndpointSystemId);
                        if (o365user != null && o365user.ServicePlanObject != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            LblMailBoxConversionType.InnerText = "Convert to Internal Shared Mail";
                            chkboxMailConversionType.Checked = true;
                        }
                        else if (o365user != null && (o365user.ServicePlanObject == null) && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            LblMailBoxConversionType.InnerText = "Convert to License Shared Mail";
                            chkboxMailConversionType.Checked = true;
                        }
                        else if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                        {
                            trMailConversionType.Visible = false;
                            chkboxMailConversionType.Checked = false;
                            ShowAlertMessage(Infrastructure.AlertType.Error, string.Format("{0} is not applicable for selected mail", AdhocRequest.AdhocActionType.ToString()));
                            Button1.Enabled = false;
                        }
                        else if(AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                        {
                            chkboxMailConversionType.Checked = (o365user != null && o365user.ServicePlanObject != null);
                        }
                    }


                }

                LicenseAutoCompleteBox.Enabled = true;
                Dto.Office365UserInfo userinfo = getUserinfo(Convert.ToInt32(ddlEndpointUser.SelectedValue), this.AdhocRequest.AccountType, (int)this.AdhocRequest.EndpointSystemId);
                Dto.Office365UserServicePlan obj = userinfo.ServicePlanObject;
                List<Dto.ServicePlanDetail> _existingServicePlanDetails = obj == null ? new List<Dto.ServicePlanDetail>() : obj.PlanDetails.Where(item => item.ProvisioningStatus != "Disabled").ToList();
                Session["ExistingServicePlanDetail"] = _existingServicePlanDetails;

            }
            else
            {
                LicenseAutoCompleteBox.Enabled = false;
            }

        }
        public void BindServicePlanComboBox()
        {
            if (this.SelectedLicense != string.Empty)
            {

                List<Dto.Office365LicenseConfig> list = DomainApiClient<Dto.Office365UserInfo>.GetByAction<List<Dto.Office365LicenseConfig>>(DomainApiAction.Endpoint.O365.GetAdhocOffice365ServicePlans,
                    new Dictionary<string, string> {    { "License", this.SelectedLicense },
                                                        { "endpointUser", Convert.ToInt32(ddlEndpointUser.SelectedValue).ToString() },
                                                        {"accountType", Convert.ToString((int)AdhocRequest.AccountType) },
                                                        {"EndpointSystemId",Convert.ToString((int)this.AdhocRequest.EndpointSystemId) },
                                                        { "adhocActionType",Convert.ToString((int)this.AdhocRequest.AdhocActionType) }
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
            btnAdd.Enabled = true;
            BindServicePlanComboBox();
        }

        protected void ServicePlanGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                chkboxRemoveLicense.Visible = false;
                var AccountSkuId = (string)((GridDataItem)e.Item).GetDataKeyValue("AccountSkuId");
                List<Dto.AdHocLicenseInfo> _sessionObj = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                _sessionObj.RemoveAll(item => item.AccountSkuId == AccountSkuId);
                Session["AdHocLicenseInfo"] = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    ServicePlanGrid.DataSource = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                    ServicePlanGrid.DataBind();
                }
                else
                {
                    ServicePlanGrid.Visible = false;
                    btnAdd.Enabled = false;
                }
            }
        }

        protected override Dto.IEntitlementDetail GetEntitlementDetail()
        {
            Dto.AdHocO365Request office365 = new Dto.AdHocO365Request();
            Dto.AdHocO365SharedAccount sharedOffice365 = new Dto.AdHocO365SharedAccount();
            if (AdhocRequest != null)
            {

                if (this.ddlEndpointUser.SelectedIndex > 0)
                {
                    office365.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0;
                }
                if (AdhocRequest.AccountType == AccountType.User)
                {

                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                        office365.LicenseInfo = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                        office365.LicenseAction = Session["AdHocLicenseInfo"] != null ? LicenseActionType.Add : LicenseActionType.None;
                        office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Enable : ExternalEmailActionType.None;
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                        office365.LicenseInfo = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                        office365.LicenseAction = Session["AdHocLicenseInfo"] != null ? LicenseActionType.Remove : LicenseActionType.None;
                        office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Disable : ExternalEmailActionType.None;
                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        office365.ExternalEmailAction = chkboxExtenalEmail.Checked ? ExternalEmailActionType.Enable : ExternalEmailActionType.None;
                    }else if(AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                    }
                }
                if (AdhocRequest.AccountType == AccountType.Shared)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        Dto.AdHocO365SharedAccount _sharedAccCreatoion = new Dto.AdHocO365SharedAccount();

                        office365.MailBoxConversionType = rbtnshared.Checked ? MailBoxConversionType.UserToShared : MailBoxConversionType.SharedToUser;
                        _sharedAccCreatoion.DisplayName = txtDisplayName.Text.Trim();
                        _sharedAccCreatoion.AccountName = txtAlias.Text.Trim();
                        office365.AdHocO365SharedAccount = _sharedAccCreatoion;
                        office365.Members = this.SelectedMembers;
                        office365.SharedMailBoxAction = (this.SelectedMembers != null && this.SelectedMembers.Count > 0) ? SharedMailBoxActionType.MemberAddition : SharedMailBoxActionType.None;

                    }

                    else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        if (ddlEndpointUser.SelectedItem != null)
                        {
                            office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                        }
                        // office365.MailBoxConversionType = chkboxMailConversionType.Checked ? (LblMailBoxConversionType.Text == "Convert to Regular Mailbox" ? MailBoxConversionType.SharedToUser : MailBoxConversionType.UserToShared) : MailBoxConversionType.None;
                        office365.MailBoxConversionType = MailBoxConversionType.UserToShared;

                    }
                    else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {

                        if (ddlEndpointUser.SelectedItem != null)
                        {
                            office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                        }
                        // office365.MailBoxConversionType = chkboxMailConversionType.Checked ? (LblMailBoxConversionType.Text == "Convert to Regular Mailbox" ? MailBoxConversionType.SharedToUser : MailBoxConversionType.UserToShared) : MailBoxConversionType.None;
                        office365.MailBoxConversionType = MailBoxConversionType.SharedToUser;


                    }
                    else if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        //Added below code because if user has licensed and requeste for license then it should routed to O365Admin(Procurement Team)
                        Dto.Office365UserInfo userinfo = getUserinfo(Convert.ToInt32(ddlEndpointUser.SelectedValue), AdhocRequest.AccountType, (int)this.AdhocRequest.EndpointSystemId);
                        Dto.Office365UserServicePlan obj = userinfo.ServicePlanObject;
                        office365.MailBoxConversionType = obj != null ? MailBoxConversionType.UserToShared : MailBoxConversionType.SharedToUser;
                        office365.EndpointUserId = Convert.ToInt32(ddlEndpointUser.SelectedValue);

                        if (ddlEndpointUser.SelectedItem != null)
                        {
                            office365.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                        }
                    }

                }

            }

            return office365;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Dto.AdHocLicenseInfo _gridObj = new Dto.AdHocLicenseInfo();
            _gridObj.AccountSkuId = this.SelectedLicense;
            if(LicenseAutoCompleteBox.Entries.Count > 0)
                _gridObj.LicenseDescription = LicenseAutoCompleteBox.Entries[0].Text;

            if (chkboxRemoveLicense.Checked)
            {
                _gridObj.IsRemoveLicense = true;
            }
            else
            {
                _gridObj.IsRemoveLicense = false;
                var serviceNames = this.SelectedServicePlan.Select(item => item.Key).ToList();
                var serviceDesc = this.SelectedServicePlan.Select(item => item.Value).ToList();
                _gridObj.ServiceName = string.Join(",", serviceNames.ToArray());
                _gridObj.ServiceDescription = string.Join(",", serviceDesc.ToArray());
            }

            ServicePlanGrid.Visible = true;
            UpdateSessionObject(_gridObj);
            ServicePlanGrid.DataSource = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
            ServicePlanGrid.DataBind();
            LicenseAutoCompleteBox.Entries.Clear();
            ServicePlanComboBox.DataSource = string.Empty;
            ServicePlanComboBox.DataBind();
            btnAdd.Enabled = false;
            chkboxRemoveLicense.Checked = false;
            chkboxRemoveLicense.Visible = false;
        }
        private void UpdateSessionObject(Dto.AdHocLicenseInfo currentobj)
        {
            if (Session["AdHocLicenseInfo"] != null)
            {
                List<Dto.AdHocLicenseInfo> _sessionObj = (List<Dto.AdHocLicenseInfo>)Session["AdHocLicenseInfo"];
                _sessionObj.Add(currentobj);
                Session["AdHocLicenseInfo"] = _sessionObj;
            }
            else
            {
                List<Dto.AdHocLicenseInfo> _sessionObj = new List<Dto.AdHocLicenseInfo>();
                _sessionObj.Add(currentobj);
                Session["AdHocLicenseInfo"] = _sessionObj;
            }
        }


        protected void chkboxRemoveLicense_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxRemoveLicense.Checked)
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = false;
                //btnAdd.Enabled = true;
            }
            else
            {
                BindServicePlanComboBox();
                ServicePlanComboBox.Enabled = true;
                //btnAdd.Enabled = false;

            }
        }

        protected void btnAddMember_Click(object sender, EventArgs e)
        {
            btnAddMember.Enabled = false;
            MemeberGrid.Visible = true;
            if (this.SelectedMembers == null)
            {
                this.SelectedMembers = new Dictionary<string, string>();
            }
            if (this.SelectedMembers.ContainsKey(MembersAutoCompletebox.Entries[0].Value))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Selected Member was already added");

            }
            else
            {
                this.SelectedMembers.Add(MembersAutoCompletebox.Entries[0].Value, MembersAutoCompletebox.Entries[0].Text);
                MemeberGrid.DataSource = this.SelectedMembers;
                MemeberGrid.DataBind();
                MembersAutoCompletebox.Entries.Clear();
            }

        }
        protected void MemeberGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var key = (string)((GridDataItem)e.Item).GetDataKeyValue("key");
                this.SelectedMembers.Remove(key);
                if (SelectedMembers.Count > 0)
                {
                    MemeberGrid.DataSource = SelectedMembers;
                    MemeberGrid.DataBind();
                }
                else
                {
                    MemeberGrid.Visible = false;
                    MemeberGrid.Enabled = false;
                }
            }
        }
        protected void MembersAutoCompletebox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (MembersAutoCompletebox.Entries.Count > 1)
            {
                MembersAutoCompletebox.Entries.Remove(MembersAutoCompletebox.Entries[1]);
            }
            btnAddMember.Enabled = true;
        }
        protected void MembersAutoCompletebox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            btnAddMember.Enabled = false;

        }

    }
}
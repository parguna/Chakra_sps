using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocADStep2UC : UIBaseStep2UC
    {
        #region Members
        public delegate void BackHandler();
        public event AddHandler AddClicked;
        public event BackHandler BackClicked;
        private const string V_ADHOCREQUEST = "V_ADHOCREQUEST";
        public delegate void AddHandler(IEntitlementDetail ientitlement, AdHocAccessRequest adhocRequest);
        #endregion

       

        #region Methods
        protected override void InitializeStep()
        {
             
            txtRequesterComments.Text = "";
            txtDiscription.Attributes.Add("maxlength", "255");
            pnlSecurityQstns.Visible = false;
            AdHocAccessRequest request = AdhocRequest;
            if (AdhocRequest != null)
            {
                if (request.AdhocActionType == AdHocActionType.NewAccount && (request.AccountType == AccountType.Service || request.AccountType == AccountType.Shared || request.AccountType == AccountType.Secondary))
                {
                    
                    SharedPanelAD.Visible = true;
                    GroupPanelAD.Visible = true;
                    txtAcntName.Text = "";
                    txtDisplayName.Text = "";
                    txtDiscription.Text = "";
                    OU_panel.Visible = false;
                    if (request.AccountType == AccountType.Service)
                    {
                        pnlSecurityQstns.Visible = true;
                        BindConfigMasterOU();
                        OUAutoCompleteBox.Enabled = false;
                    }
                    else if (request.AccountType == AccountType.Shared)
                    {
                        BindUserInfoOU(request.RequesterUserId, request.EndpointSystemId.Value);
                        OUAutoCompleteBox.Enabled = true;
                    }
                    pnlEndPointUserName.Visible = false;
                }
                else
                {
                    if (request.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        pnlEndPointUserName.Visible = true;
                    }
                    else
                    {
                        pnlEndPointUserName.Visible = false;
                    }
                    SharedPanelAD.Visible = false;
                    GroupPanelAD.Visible = true;
                }
                LoadControl();
                if (AdhocRequest.GetEntitlementDetail()!=null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }

        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocADRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                
                txtRequesterComments.Text = Entitlement.RequesterComment;
                if (Entitlement.AdHocADSharedAccount == null)
                    return;
                txtAcntName.Text = Entitlement.AdHocADSharedAccount.AccountName;
                txtDisplayName.Text = Entitlement.AdHocADSharedAccount.DisplayName;
                txtDiscription.Text = Entitlement.AdHocADSharedAccount.Description;
                if (!string.IsNullOrEmpty(Entitlement.AdHocADSharedAccount.OU))
                {
                    OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(Entitlement.AdHocADSharedAccount.OU));
                }
            }
        }
        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary!=null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (list.AccountReconDictionary.Count() == 1)
                {
                    ddlEndpointUser.SelectedIndex = 1;
                }
            }
        }
        private void BindUserInfoOU(long requesterid, long endpointsystemid)
        {
            Dto.IConfigurationMaster config = getConfigurationMaster(ConfigConstants.AD_SHARED_ACCOUNT_OU);             
            if (config == null)
                OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(config.ConfigValue));
        }
        private void BindConfigMasterOU()
        {
            Dto.IConfigurationMaster config = getConfigurationMaster(ConfigConstants.AD_SERVICE_ACCOUNT_OU);
            if (config == null)
                OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(config.ConfigValue));
        }
        
        Dto.IConfigurationMaster getConfigurationMaster(string configKey)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", configKey);
            parameter.Add("product", Product.IM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            return config;
        }
        
        public override bool Validate()
        {
            bool result = false;
            
            string Message = "Please enter ";
            string ControlMessage = "";
            AdHocAccessRequest request = AdhocRequest;
            if ((request.AdhocActionType == AdHocActionType.NewAccount) && (request.AccountType == AccountType.Shared || request.AccountType == AccountType.Service || request.AccountType == AccountType.Secondary))
            {
                if (string.IsNullOrWhiteSpace(txtAcntName.Text))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Account Name" : ControlMessage + ", Account Name";
                }
                if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Display Name" : ControlMessage + ", Display Name";
                }
                if (string.IsNullOrWhiteSpace(txtDiscription.Text))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Description" : ControlMessage + ", Description";
                }
                if (request.AccountType == AccountType.Service)
                {
                    if (string.IsNullOrEmpty(request.SecurityQuestions))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                    }
                }
            }
            if (request.AdhocActionType != AdHocActionType.NewAccount && ddlEndpointUser.SelectedIndex <= 0)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + ", EndPoint User";
            }

            if(request.AccountType == AccountType.User)
            {
                if ((request.AdhocActionType == AdHocActionType.AmendmentAdd || request.AdhocActionType == AdHocActionType.AmendmentRemove) && string.IsNullOrWhiteSpace(txtRequesterComments.Text))
                {

                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Comments for Group Add/Remove" : ControlMessage + ", Comments for Group Add/Remove";
                }
            }

            

            if (ControlMessage != "")
            {
                Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                ShowAlertMessage(AlertType.Error, Message + ".");                
                return false;
            }

            if (request.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("userid", request.RequesteeUserId.ToString());
                dict.Add("endpointsystemid", request.EndpointSystemId.ToString());
                dict.Add("accountType", Convert.ToString((int)request.AccountType));
                dict.Add("endpointUserId", Convert.ToString((ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0)));
                Dto.AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetAccountReconDtoByEndpointUserId", dict);

                if (list != null && list.EntitlementReconcilations != null && list.EntitlementReconcilations.Count() == 0)
                { 
                    ShowAlertMessage(AlertType.Error, "User does not have any entitlements to remove.");
                    return false;
                }
            }

            result = true;
            
            return result;
        }

        protected override void afterSave()
        {
            base.afterSave(); 
            txtAcntName.Text = "";
            txtDisplayName.Text = "";
            txtDiscription.Text = "";
            txtRequesterComments.Text = "";
            OUAutoCompleteBox.Entries.Clear(); 
        }
        private void LoadControl()
        {
            if (AdhocRequest != null)
            {
                this.ddlEndpointUser.Enabled = true;
                if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                }

                this.groupComments.Visible = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    divComments.Visible = true;
                    if (!(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                         this.SetMandatory(divComments, true);
                    else
                        this.SetMandatory(divComments, false);
                }
                else
                {
                    divComments.Visible = false;
                    this.SetMandatory(divComments, false);
                }
            }
        }

        bool IsUserFullTimeEmployee(long UserId)
        {
            var usr = DomainApiClient<User>.GetById(UserId);
            return usr != null && usr.FullTimeEmployee;
        }
        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocADRequest adhocAD = null;
            AdHocADSharedAccount sharedAccount = new AdHocADSharedAccount();             
            AdHocAccessRequest request = AdhocRequest;
            if (request != null)
            {
                adhocAD = new AdHocADRequest();
                if ((request.AccountType == AccountType.Service || request.AccountType == AccountType.Shared || request.AccountType == AccountType.Secondary) && (request.AdhocActionType == AdHocActionType.NewAccount))
                {
                    sharedAccount.AccountName = txtAcntName.Text;
                    sharedAccount.DisplayName = txtDisplayName.Text;
                    sharedAccount.Description = txtDiscription.Text;
                    if (OUAutoCompleteBox.Entries.Count > 0)
                    {
                        sharedAccount.OU = OUAutoCompleteBox.Entries[0].Text;
                    }
                }
                if (request.AdhocActionType != AdHocActionType.NewAccount)
                {
                    if (this.ddlEndpointUser.SelectedIndex > 0)
                    {
                        adhocAD.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0;

                        if (ddlEndpointUser.SelectedItem != null)
                            adhocAD.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;

                    }
                }

                adhocAD.EntitlementType = EntitlementType.Group;
                adhocAD.Entitlement = "";
                adhocAD.RequesterComment = txtRequesterComments.Text;
                if (!string.IsNullOrWhiteSpace(adhocAD.RequesterComment))
                {
                    adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br>");
                }
                adhocAD.AdHocADSharedAccount = ((request.AccountType == AccountType.Service || request.AccountType == AccountType.Shared || request.AccountType == AccountType.Secondary) && (request.AdhocActionType == AdHocActionType.NewAccount)) ? sharedAccount : null;

            }
            return adhocAD;
        }


        #endregion

        #region Events
        public void btnSave_Click(object sender, EventArgs e)
        {
        //    if (Validatation() || sender == null)
        //    {               
        //        AdHocADRequest adhocAD = new AdHocADRequest();
        //        AdHocADSharedAccount sharedAccount = new AdHocADSharedAccount();
        //        if (AdhocRequest != null)
        //        {
        //            if ((AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared) && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
        //            {
        //                sharedAccount.AccountName = txtAcntName.Text;
        //                sharedAccount.DisplayName = txtDisplayName.Text;
        //                sharedAccount.Description = txtDiscription.Text;
        //                if (OUAutoCompleteBox.Entries.Count > 0)
        //                {
        //                    sharedAccount.OU = OUAutoCompleteBox.Entries[0].Text;
        //                }
        //            }
        //            if (this.ddlEndpointUser.SelectedIndex > 0)
        //            {
        //                adhocAD.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0;
        //            }
        //            adhocAD.EntitlementType = EntitlementType.Group;
        //            adhocAD.Entitlement = "";
        //            adhocAD.RequesterComment = txtRequesterComments.Text;
        //            if (adhocAD.RequesterComment != null)
        //            {
        //                adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br>");
        //            }
        //            adhocAD.AdHocADSharedAccount = ((AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared) && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)) ? sharedAccount : null;
        //            if (sender == null)
        //            {
        //                if (string.IsNullOrEmpty(adhocAD.Entitlement) && (AdhocRequest.AccountType == AccountType.User) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
        //                    AdhocRequest.EntitlementDetail = null;
        //                else
        //                    AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(adhocAD);
        //            }
        //            else
        //            {
        //                if (string.IsNullOrEmpty(adhocAD.Entitlement) && (AdhocRequest.AccountType == AccountType.User) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
        //                {
        //                    AddClicked(null, this.AdhocRequest);
        //                }
        //                else
        //                {
        //                    AddClicked(adhocAD, this.AdhocRequest);
        //                }
        //                txtAcntName.Text = "";
        //                txtDisplayName.Text = "";
        //                txtDiscription.Text = "";
        //                txtRequesterComments.Text = "";
        //                OUAutoCompleteBox.Entries.Clear();

        //            }
        //        }
        //    }
        //}
        //protected void btnBack_Click(object sender, EventArgs e)
        //{
        //    if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
        //    {
        //        btnSave_Click(null, null);
        //        Session["AdhocrequestObj"] = AdhocRequest;
        //    }
        //    BackClicked();
        }
        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEndpointUser.SelectedIndex > 0)
            {
                 
            }

        }





        #endregion

        protected void ddlEndpointUser_SelectedIndexChanged1(object sender, EventArgs e)
        {
            afterSave();
        }
    }
}
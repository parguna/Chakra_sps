using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSapLocalStep2UC : UIBaseStep2UC
    {


        #region Methods
        protected override void InitializeStep()
        {
            
            txtDiscription.Attributes.Add("maxlength", "255");
            trSecurityQstns.Visible = false;
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    
                    SharedPanelAD.Visible = true;
                    GroupPanelAD.Visible = true;
                    txtAcntName.Text = "";
                    txtDisplayName.Text = "";
                    txtDiscription.Text = "";
                    ouTR.Visible = false;
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        trSecurityQstns.Visible = true;
                        BindConfigMasterOU();
                        OUAutoCompleteBox.Enabled = false;
                    }
                    else if (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary || AdhocRequest.AccountType == AccountType.User)
                    {
                        //BindUserInfoOU(AdhocRequest.RequesterUserId, AdhocRequest.EndpointSystemId.Value);
                        string accountOU = string.Empty;
                        if(AdhocRequest.AccountType == AccountType.User)
                        {
                            accountOU = ConfigConstants.AD_SAP_LOCAL_USER_ACCOUNT_OU;
                        }
                        else if (AdhocRequest.AccountType == AccountType.Secondary)
                        {
                            accountOU = ConfigConstants.AD_SAP_LOCAL_SECONDARY_ACCOUNT_OU;
                        }
                        else if (AdhocRequest.AccountType == AccountType.Shared)
                        {
                            accountOU = ConfigConstants.AD_SAP_LOCAL_SHARED_ACCOUNT_OU;
                        }
                        else if (AdhocRequest.AccountType == AccountType.Service)
                        {
                            accountOU = ConfigConstants.AD_SAP_LOCAL_SERVICE_ACCOUNT_OU;
                        }
                        var sharedAccountOU = MasterConfigHelper.GetConfigValue(accountOU, Product.IM).ConfigValue;
                        OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(sharedAccountOU));
                        OUAutoCompleteBox.Enabled = false;
                    }
                    trEndPointUser.Visible = false;
                }
                else
                {

                    trEndPointUser.Visible = true;
                    SharedPanelAD.Visible = false;
                    GroupPanelAD.Visible = true;
                }
                LoadControl();
                if(AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }

        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPLocalRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                ddlEndpointUser_SelectedIndexChanged(null, null);
                // txtRequesterComments.Text = Entitlement.RequesterComment;

                txtAcntName.Text = Entitlement.AccountName;
                txtDisplayName.Text = Entitlement.DisplayName;
                txtDiscription.Text = Entitlement.Description;
                if (!string.IsNullOrWhiteSpace(Entitlement.OU))
                {
                    OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(Entitlement.OU));
                }
            }
        }
        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
             
            if (list != null && list.AccountReconDictionary!=null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
            }
        }
        private void BindUserInfoOU(long requesterid, long endpointsystemid)
        {
            string sharedAccountOU = MasterConfigHelper.GetConfigValue(ConfigConstants.AD_SAP_LOCAL_USER_ACCOUNT_OU, Product.IM).ConfigValue;
            OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(sharedAccountOU));
        }

        private void BindConfigMasterOU()
        {
           // bool isSDC = AdhocRequest.ApplicationType == ApplicationType.ADSDCLocal;

            OUAutoCompleteBox.Entries.Clear();
            //if (isSDC)
            //    OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(MasterConfigHelper.ADSDCServiceAccountOU));
            //else
            string sharedAccountOU = MasterConfigHelper.GetConfigValue(ConfigConstants.AD_SAP_LOCAL_SERVICE_ACCOUNT_OU, Product.IM).ConfigValue;
            
            OUAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(sharedAccountOU));

        }
        private bool ValidateAccountName()
        {
            string validationMsg = string.Empty;
            bool isExists = false;
            isExists = AdhocAccessRequestValidation.ValidateAccountName(txtAcntName.Text.Trim(), AdhocRequest.EndpointSystemId.Value, out validationMsg, AdhocRequest.RequesteeUserId, this.AdhocRequest.AccountType);

            if (!string.IsNullOrWhiteSpace(validationMsg))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, validationMsg);
            }
            return isExists;
        }

        public override bool Validate()
        {
            bool result = false;
            string Message = "Please enter";
            string ControlMessage = "";
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrWhiteSpace(txtAcntName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Account Name" : ControlMessage + ", Account Name";
                }else
                {
                    if (txtAcntName.Text.Length < MasterConfigHelper.AccountNameMinimumLength)
                    {
                        ControlMessage ="Account Name is too short.";
                    }
                }
                if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Display Name" : ControlMessage + ", Display Name";
                }
                if (string.IsNullOrWhiteSpace(txtDiscription.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Description" : ControlMessage + ", Description";
                }
                if (AdhocRequest.AccountType == AccountType.Service)
                {
                    if (string.IsNullOrWhiteSpace(AdhocRequest.SecurityQuestions))
                    {
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                    }
                }
            }
            if (!(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {
                if (ddlEndpointUser.SelectedIndex <= 0)
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " EndPoint User" : ControlMessage + ", EndPoint User";

            }
            if (ControlMessage != "")
            {
                Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
               
                ShowAlertMessage(Infrastructure.AlertType.Error, $"{Message}.");
                result = false;
            }
            else
            {
                //if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                //{
                //    result = ValidateAccountName();
                //}
                //else
                //{
                    result = true;
                //}
            }
            return result;
        }
        private void LoadControl()
        {
            if (AdhocRequest != null)
            {
                this.ddlEndpointUser.Enabled = true;
                if(AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                //this.tdGroupAdd.Visible = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                //CommentsforGroupAddRemoveAsterix.Visible = !(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount);
            }
        }


        #endregion

        #region Events
        protected override IEntitlementDetail GetEntitlementDetail()
        {

            AdHocSAPLocalRequest adhocAD = new AdHocSAPLocalRequest();

            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    adhocAD.AccountName = txtAcntName.Text;
                    adhocAD.DisplayName = txtDisplayName.Text;
                    adhocAD.Description = txtDiscription.Text;

                    string configKeyValue = string.Empty;

                    if (AdhocRequest.AccountType == AccountType.User)
                    {
                        configKeyValue = ConfigConstants.AD_SAP_LOCAL_USER_ACCOUNT_OU;
                    }
                    else if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        configKeyValue = ConfigConstants.AD_SAP_LOCAL_SERVICE_ACCOUNT_OU;
                    }
                    else if (AdhocRequest.AccountType == AccountType.Shared)
                    {
                        configKeyValue = ConfigConstants.AD_SAP_LOCAL_SHARED_ACCOUNT_OU;
                    }
                    else if (AdhocRequest.AccountType == AccountType.Secondary)
                    {
                        configKeyValue = ConfigConstants.AD_SAP_LOCAL_SECONDARY_ACCOUNT_OU;
                    }

                    if (!string.IsNullOrWhiteSpace(configKeyValue))
                    {
                        adhocAD.OU = MasterConfigHelper.GetConfigValue(configKeyValue, Product.IM)?.ConfigValue;
                    }
                }

                if (this.ddlEndpointUser.SelectedIndex > 0)
                {
                    adhocAD.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0;

                    if(ddlEndpointUser.SelectedItem!=null)
                      adhocAD.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                }

                adhocAD.EntitlementType = EntitlementType.Group;
                adhocAD.Entitlement = "";
                adhocAD.RequesterComment = this.AdhocRequest.Comments;
                // adhocAD.RequesterComment = txtRequesterComments.Text;
                if (adhocAD.RequesterComment != null)
                {
                    adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br>");
                }

            }
            return adhocAD;
        }

        protected override void afterSave()
        {
            base.afterSave();
            txtAcntName.Text = "";
            txtDisplayName.Text = "";
            txtDiscription.Text = "";
            OUAutoCompleteBox.Entries.Clear();
        }

 
        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            if (ddlEndpointUser.SelectedIndex > 0)
            {
                 
            }

        }
       
        #endregion




    }
}
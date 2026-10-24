using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSsidsStep2UC : UIBaseStep2UC
    {

        public Dictionary<string, string> CurrentEntitlement
        {
            get
            {
                if (ViewState["V_CurrentEntitlement"] != null)
                {
                    return (Dictionary<string, string>)ViewState["V_CurrentEntitlement"];
                }
                return null;
            }
            set
            {
                ViewState["V_CurrentEntitlement"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitializeStep()
        {
            if (AdhocRequest == null) return;
            var action = AdhocRequest.AdhocActionType;
            var actions = new[] { AdHocActionType.DeleteAccount, AdHocActionType.AmendmentAdd, AdHocActionType.AmendmentRemove };
            if (actions.Contains(action))
            {
                pnlEndPointUserName.Visible = true;
                BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
            }
            pnlSMS.Visible = pnlGUI.Visible = false;
            var applicationType = AdhocRequest.ApplicationType;
            if (applicationType == ApplicationType.SSIDS_SMS && action != AdHocActionType.DeleteAccount)
            {
                var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
                pnlSMS.Visible = true;
                txtSMS.Text = user?.MobileNumber?.Trim();
            }
            if (applicationType == ApplicationType.SSIDS_GUI && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                pnlGUI.Visible = true;

            }

            BindRoles();

            if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
            {
                BindAdhocRequest();
            }
        }

        protected override void afterSave()
        {
            base.afterSave();
            if(CurrentEntitlement != null)
                CurrentEntitlement.Clear();
            if (ddlRole.Items.Count > 0)
                ddlRole.SelectedIndex = 0;
        }
        void BindRoles()
        {
            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_GUI)
            {
                Dictionary<string, string> allEntitlements = DomainApiClient<Dto.SSIDSRolelnfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SSIDS.GetAllRoles, null);
                ddlRole.Items.Clear();
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    DropDownHelper.DropDownListBind(ddlRole, allEntitlements, "Value", "Key", null, true, "-- Select Role --", false);
                    return;
                }


                var existingEntitlements = CurrentEntitlement;
                if (existingEntitlements != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    DropDownHelper.DropDownListBind(ddlRole, existingEntitlements, "Value", "Key", null, true, "-- Select Role --", false);
                    return;
                }

                if (existingEntitlements != null && allEntitlements != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    DropDownHelper.DropDownListBind(ddlRole, allEntitlements.Where(item => !existingEntitlements.Keys.Contains(item.Key)), "Value", "Key", null, true, "-- Select Role --", false);
                    return;
                }
            }
            else if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                pnlGUI.Visible = false;
                if (CurrentEntitlement != null && CurrentEntitlement.Count>0)
                {
                    txtSMS.Text = CurrentEntitlement.Keys.First().Trim();
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

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);

                CurrentEntitlement = list.EntitlementReconcilations.ToDictionary(item => item.Entitlement, item => item.Entitlement);
            }
        }


        public override bool Validate()
        {
            var returnValue = true;
            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && !UtilityMethods.IsValidMobileNumber(txtSMS.Text))
            {
                returnValue = false;
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter a valid Singapore mobile number (Length must be 8 characters only & Starting with either 8 or 9)");
            }

            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_GUI && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if (ddlRole.SelectedIndex <= 0)
                {
                    returnValue = false;
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please select a role.");
                }
            }

            if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount && ddlEndpointUser.SelectedIndex <= 0)
            {
                returnValue = false;
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the user.");
            }
            return returnValue;
        }
        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocSSIDSRequest request = new AdHocSSIDSRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = (ddlEndpointUser.SelectedIndex > 0) ? Convert.ToInt32(ddlEndpointUser.SelectedItem.Value) : 0;

                if (ddlEndpointUser.SelectedItem != null)
                {
                    request.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                }

                if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    request.EntitlementType = EntitlementType.HandPhone;
                    request.Entitlement = txtSMS.Text.Trim();

                }
                else if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_GUI)
                {
                    request.EntitlementType = EntitlementType.Role;
                    request.Entitlement = ddlRole.SelectedValue;
                }
            }

            return request;
        }

        void BindAdhocRequest()
        {
            var Entitlement = (AdHocSSIDSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null && !string.IsNullOrWhiteSpace(Entitlement.Entitlement))
            {
                if (Entitlement.EntitlementType == EntitlementType.Role)
                {
                    ddlRole.SelectedValue = Entitlement.Entitlement;
                }

                if (Entitlement.EntitlementType == EntitlementType.HandPhone)
                {
                    txtSMS.Text = Entitlement.Entitlement.Trim();
                }
            }
        }

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
        }
    }
}
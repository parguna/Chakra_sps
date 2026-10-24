using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.NACR.Create.App.Step2UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocSsidsStep2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        const string V_SMSDATA = "V_SMSDATA";
        public Dictionary<string, string> V_SmsData
        {
            get
            {
                if (ViewState[V_SMSDATA] == null)
                    return new Dictionary<string, string>();
                return (Dictionary<string, string>)ViewState[V_SMSDATA];
            }
            set
            {
                ViewState[V_SMSDATA] = value;
            }
        }

        protected override void InitializeStep()
        {
            if (AdhocRequest == null) return;
            BindEndpointUsers();
            var applicationType = AdhocRequest.ApplicationType;
            if(applicationType == ApplicationType.SSIDS_SMS && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                pnlSMS.Visible = true;
            }
            else
            {
                pnlSMS.Visible = false;
            }

            if (applicationType == ApplicationType.SSIDS_GUI && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                pnlGUI.Visible = true;
                Dictionary<string, string> allEntitlements = DomainApiClient<Dto.SSIDSRolelnfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SSIDS.GetAllRoles, null);
                ddlRole.Items.Clear();
                DropDownHelper.DropDownListBind(ddlRole, allEntitlements, "Value", "Key", null, true, "-- Select Role --", false);
            }
            else
            {
                pnlGUI.Visible = false;
            }
        }

        void BindEndpointUsers()
        {
            var applicationType = AdhocRequest.ApplicationType;
            Dictionary<string, string> smsData = new Dictionary<string, string>();
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                pnlEndPointUserName.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();

                string endpointSystemId = AdhocRequest.EndpointSystemId.ToString();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                {
                    var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", endpointSystemId},
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
                        string sms = string.Empty;
                        foreach (var i in items)
                        {
                            userNames.Add(i.EndpointUserId, i.AccountName);
                            if (applicationType == ApplicationType.SSIDS_SMS)
                            {

                                List<EntitlementReconcilation> list = DomainApiClient<Dto.EntitlementReconcilation>.GetByAction<List<EntitlementReconcilation>>(DomainApiAction.Provisioning.EntitlementReconcilation.GetByReconcilationId,
                                    new Dictionary<string, string> { { "endpointSystemId", endpointSystemId }, { "reconcilationId", i.Id.ToString() } });


                                sms = string.Join(", ", list.Select(a => a.Entitlement));

                            }
                        }
                        repeaterData.Add(item.Name, userNames);
                        smsData.Add(item.Name, sms);
                    }
                });

                V_SmsData = smsData;

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();


            }
            else
            {
                if (V_SmsData.Count == 0)
                {
                    AdhocRequest.BulkRequestDetails.ForEach(item =>
                    {
                        smsData.Add(item.Name, string.Empty);
                    });

                    V_SmsData = smsData;
                }
            }
            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS)
            {
                RadGridSMS.DataSource = V_SmsData; ;
                RadGridSMS.DataBind();
            }
        }


        bool ValidateRole()
        {
            var returnValue = true;
            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_GUI && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if (ddlRole.SelectedIndex <= 0)
                {
                    returnValue = false;
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please select a role.");
                }
            }

            return returnValue;
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
        bool UpdateSMSDetail()
        {
            if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                var smsData = new Dictionary<string, string>();

                foreach (GridDataItem dataItem in RadGridSMS.Items)
                {
                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                        var sms = ((TextBox)dataItem.FindControl("txtSMS")).Text.Trim();
                        if (!string.IsNullOrWhiteSpace(sms) && !UtilityMethods.IsValidMobileNumber(sms))
                        {
                            sms = string.Empty;
                        }
                        smsData.Add(userName, sms);
                    }
                }

                V_SmsData = smsData;
                foreach (GridDataItem dataItem in RadGridSMS.Items)
                {
                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                        var sms = ((TextBox)dataItem.FindControl("txtSMS")).Text.Trim();

                        if (string.IsNullOrWhiteSpace(sms) || !UtilityMethods.IsValidMobileNumber(sms))
                        {
                            ShowAlertMessage(AlertType.Error, $"Please enter a valid Singapore mobile number (Length must be 8 characters only & Starting with either 8 or 9) {userName}.");
                            return false;
                        }
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).HandPhone = sms;
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
            return UpdateSMSDetail() && ValidateRole() && UpdateBulkReqAccountDetail();
        }


        protected override void SetEntitlementDetail()
        {
            AdHocSSIDSRequest request = new AdHocSSIDSRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = 0;
                if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_SMS)
                {
                    request.EntitlementType = EntitlementType.HandPhone;
                    //request.Entitlement = txtSMS.Text;

                }
                else if (AdhocRequest.ApplicationType == ApplicationType.SSIDS_GUI)
                {
                    request.EntitlementType = EntitlementType.Role;
                    request.Entitlement = ddlRole.SelectedValue;
                }
            }
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);

        }


    }
}
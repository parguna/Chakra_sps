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
    public partial class AdhocGemsStep2UC : UIBaseStep2UC
    {
        const string V_REQUEST = "V_REQUEST";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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
        protected override void afterSave()
        {
            base.afterSave();
            RolesAutoCompleteBox.Entries.Clear();
            txtAccountName.Text = "";
            txtEmail.Text = "";
            txtDescription.Text = "";
            txtDisplayName.Text = "";
            EntitlementRequest = new List<string>();
            RequestGrid.DataSource = this.EntitlementRequest;
            RequestGrid.DataBind();
            trGrdid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            Session["EndpointUserId"] = null;
        }

        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {
                trSecurityQstns.Visible = false;

                RoleAstreix.Visible = true;
                EntitlementRequest.Clear();
                SetMandatory(trEndPointUser, true);
                Session["EndpointUserId"] = null;

                Session["AdhocActionType"] = Convert.ToInt32(AdhocRequest.AdhocActionType);

                RolesAutoCompleteBox.Entries.Clear();
                txtDescription.Attributes.Add("maxlength", "255");
                FillRequestGrid();
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary))
                {
                    var requester = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesterUserId);
                    SharedPanelGEMS.Visible = true;
                    GroupPanelGEMS.Visible = true;
                    txtAccountName.Text = "";
                    txtEmail.Text = requester.EmailId;
                    txtDescription.Text = "";
                    txtDisplayName.Text = "";
                    trEndPointUser.Visible = false;
                    trEndPointUser.Visible = true;
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        trSecurityQstns.Visible = true;
                    }
                }
                else
                {
                    if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        trEndPointUser.Visible = true;
                    }
                    else
                    {
                        trEndPointUser.Visible = false;
                    }
                    trEndPointUser.Visible = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                    SharedPanelGEMS.Visible = false;
                    GroupPanelGEMS.Visible = true;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    trEndPointUser.Visible = true;
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        GroupPanelGEMS.Visible = true;
                    }
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    GroupPanelGEMS.Visible = false;
                    SetMandatory(trEndPointUser, false);
                    AddRequestButton.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary))
                {
                    trEndPointUser.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    trEndPointUser.Visible = false;
                    GroupPanelGEMS.Visible = true;
                    SetMandatory(RoleAstreix, false);
                }
                if(AdhocRequest.AccountType == AccountType.Service)
                {
                    txtAccountName.MaxLength = 60;
                }
            }
            if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
            {
                BindAdhocRequest();
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
                if (list.AccountReconDictionary.Count() > 0)
                {
                    EndPointUserDropDownList.SelectedIndex = 1;
                }
                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);                     
                }
            } 
 
        }

        string ValidGemsRole()
        {
            var validationMsg = string.Empty;
            var businessRole= Convert.ToInt64(RolesAutoCompleteBox.Entries[0].Value) ;
            //var matrix = DomainApiClient<Dto.GEMSRoleMatrix>.GetByAction<GEMSRoleMatrix>(DomainApiAction.Matrix.GEMS.GetByBusinessRole, new Dictionary<string, string> { { "GetByBusinessRole", businessRole } });


            var matrix = DomainApiClient<Dto.GEMSRoleMatrix>.GetById(businessRole);
            if (matrix!=null && !string.IsNullOrWhiteSpace(matrix.ADGroups))
            {

                var adParam = new Dictionary<string, string>();
                adParam.Add("applicationType", ((int)ApplicationType.ADSingaporePowerLocal).ToString());
                var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);


                var param = new Dictionary<string, string>
                    {
                        { "userId", AdhocRequest.RequesteeUserId.ToString()},
                        { "endpointSystemId", endPoint.Id.ToString()},
                        { "accountType", AdhocRequest.AccountType.ToString()}
                    };
                var apiMethod = string.Empty;
                var accountType = this.AdhocRequest.AccountType;
                var accName = this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount ? EndPointUserDropDownList.SelectedItem.Text : txtAccountName.Text;
                if (this.AdhocRequest.AccountType == AccountType.User)
                {
                    apiMethod = DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId;
                }
                else
                {
                    param.Add("accountName", accName);
                    apiMethod = DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemIdAndAccountName;
                }
                List<AccountReconcilation> accounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                GetByAction<List<Dto.AccountReconcilation>>(
                                                                                apiMethod, param);
                if (accounts == null || accounts.Count == 0)
                {
                    validationMsg = "Cannot raise request for this entitlement as it needs AD account";
                    return validationMsg;
                }

                if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (accountType == AccountType.Service || accountType == AccountType.Shared))
                {
                    var DiUserId = AdhocRequest.RequesteeUserId;
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accountName", accName);
                    parameter.Add("EndpointSystemId", this.AdhocRequest.EndpointSystemId.ToString());

                    var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                                (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);

                    string spc = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";
                    if (accountRecon != null && (accountRecon.IsLocked == false && accountRecon.IsDeleted == false))
                    {
                        validationMsg = "Account Name already exists, please try another name.";
                        return validationMsg;
                    }
                    else if (accountRecon != null && (accountRecon.IsLocked == true && accountRecon.IsDeleted == true))
                    {
                        var ValidationMsg = string.Empty;
                        if (accountRecon.UserId == null)
                        {
                            validationMsg = "Account Name already exists as inactive status and is not mapped. \nPlease contact IAM team for further assistance.";
                            return validationMsg;
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
                                    validationMsg = $"Account Name already exists as inactive status and mapped to {usr.DisplayName}. \nPlease contact IAM team for further assistance.";
                                    return validationMsg;
                                }
                            }
                        }
                    }
                }
               
            }
            return validationMsg;
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (RolesAutoCompleteBox.Entries.Count != 0)
            {
                if(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType != AccountType.User && string.IsNullOrWhiteSpace(txtAccountName.Text))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please enter account Name");
                    return;
                }
                if(AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && AdhocRequest.AccountType != AccountType.User && EndPointUserDropDownList.SelectedIndex <= 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please Select the EndPoint User");
                    return;
                }
                if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                {
                    
                    string msg = ValidGemsRole();
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Warning, msg);
                        return;
                    }
                }

                List<string> requests = new List<string>();
                requests = this.EntitlementRequest;
                foreach (AutoCompleteBoxEntry item in RolesAutoCompleteBox.Entries)
                {
                    string newRequest = item.Text.Replace(";", "").Trim();
                    List<string> listofOldEntiltement = this.EntitlementRequest.Select(entitlement => entitlement).ToList();
                    if (!EntitlementRequest.Contains(newRequest) && !string.IsNullOrEmpty(newRequest))
                    {
                        requests.Add(newRequest);
                        this.EntitlementRequest = requests;
                        FillRequestGrid();
                    }
                    else
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "The role is already in the request list.");
                        return;
                    }
                }
                RolesAutoCompleteBox.Entries.Clear();


            }
            else
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the role name.");
            }
        }
        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["EndpointUserId"] = null;
            if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
            {
                Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
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

        void BindAdhocRequest()
        {
            var Entitlement = (AdHocGEMSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (EndPointUserDropDownList.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    EndPointUserDropDownList.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                EndPointUserDropDownList_SelectedIndexChanged(null, null);
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                    EntitlementRequest = Entitlement.Entitlement.Split(',').Select(l => l).ToList();
                FillRequestGrid();
                if (Entitlement.AdHocGEMSSharedAccount != null)
                {
                    txtAccountName.Text = Entitlement.AdHocGEMSSharedAccount.AccountName;
                    txtDisplayName.Text = Entitlement.AdHocGEMSSharedAccount.DisplayName;
                    txtDescription.Text = Entitlement.AdHocGEMSSharedAccount.Description;
                    txtEmail.Text = Entitlement.AdHocGEMSSharedAccount.EmailAddress;
                }
            }
        }

        public override bool Validate()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && this.EntitlementRequest.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                return false;
            }
            if (EndPointUserDropDownList.SelectedIndex <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please Select the EndPoint User");
                return false;
            }
            string Message = "Please Select the ";
            string ControlMessage = "";

            if (this.AdhocRequest.AccountType != AccountType.User && this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrWhiteSpace(txtAccountName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Account Name" : ControlMessage + " , Account Name";
                }
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Email Id" : ControlMessage + " , Email Id";
                }
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!regex.IsMatch(txtEmail.Text))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter a valid email Id.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Display Name" : ControlMessage + " , Display Name";
                }
                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Description " : ControlMessage + " , Description";
                }
            }
            if ((this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && EntitlementRequest.Count <= 0))
            {
                ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Role" : ControlMessage + " , Role";
            }
            if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                {
                    ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                }
            }
            if (ControlMessage != "")
            {
                int lastIndex = ControlMessage.LastIndexOf(",");
                Message += lastIndex > 0 ? ControlMessage.Substring(0, lastIndex) + " and" + ControlMessage.Substring(lastIndex + 1) : ControlMessage;
                ShowAlertMessage(Infrastructure.AlertType.Error, Message + ".");
                return false;
            }
            //else
            //{
            //    if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount) && (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service))
            //    {
            //        return ValidateAccountName();
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            return true;
        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocGEMSRequest request = new AdHocGEMSRequest
            {
                EndpointUserId = (EndPointUserDropDownList.SelectedIndex > 0) ? Convert.ToInt32(EndPointUserDropDownList.SelectedItem.Value) : 0,
                Entitlement = string.Join(",", EntitlementRequest),
                EntitlementType = EntitlementType.Role
            };

            if (EndPointUserDropDownList.SelectedItem != null)
                request.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;

            AdHocGEMSSharedAccount gemsShared = new AdHocGEMSSharedAccount();
            if ((AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary) && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {
                gemsShared.AccountName = txtAccountName.Text;
                gemsShared.EmailAddress = txtEmail.Text;
                gemsShared.Description = txtDescription.Text;
                gemsShared.DisplayName = txtDisplayName.Text;
            }
            request.AdHocGEMSSharedAccount = ((AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary) && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)) ? gemsShared : null;

            return request;
        }
    }
}
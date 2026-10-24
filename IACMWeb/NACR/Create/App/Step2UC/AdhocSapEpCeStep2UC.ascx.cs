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

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSapEpCeStep2UC : UIBaseStep2UC
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitializeStep()
        {
            Session["EndpointUserId"] = null;
            pnlSecurityQstns.Visible = false;
            txtDiscription.Attributes.Add("maxlength", "255");
            trGrdid.Visible = false;
            AdhocRequestDetails = new List<AdHocIACMRequestDetail>();
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary))
                {
                    SharedPanelPLRELearn.Visible = true;
                    GroupPanelPLRELearn.Visible = true;
                    txtAcntName.Text = "";
                    txtDiscription.Text = "";
                }
                else
                {
                    SharedPanelPLRELearn.Visible = false;
                    txtRequestDetails.Text = "";
                    GroupPanelPLRELearn.Visible = true;
                }
                divEntitlementType.Visible = false;
                //BindEntitlementType();
                LoadControl();
                if(AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
                Session["Action"] = (AdHocActionType)AdhocRequest.AdhocActionType;
                Session["EndPointSystemId"] = AdhocRequest.EndpointSystemId.Value;
            }
        }

        void BindEntitlementType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem() { Text = "--Select--", Value = Convert.ToString((int)EntitlementType.None) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Role", Value = Convert.ToString((int)EntitlementType.Role) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Group", Value = Convert.ToString((int)EntitlementType.Group) });
            divRole.Visible = false;
            divGroup.Visible = false;
            btnGroupAdd.Enabled = false;
            btnRoleAdd.Enabled = false;
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove)
            {
                btnGroupAdd.Text = "Remove Group";
                btnRoleAdd.Text = "Remove Role";
            }
        }
        public List<AdHocIACMRequestDetail> AdhocRequestDetails
        {
            get
            {
                if (ViewState["AdHocIACMRequestDetail"] != null)
                {
                    return (List<AdHocIACMRequestDetail>)ViewState["AdHocIACMRequestDetail"];
                }
                return new List<AdHocIACMRequestDetail>();
            }
            set
            {
                ViewState["AdHocIACMRequestDetail"] = value;
            }
        }

        void BindAdhocRequest()
        {
            var Entitlement = (AdhocSAPEPRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                EndPointUserDropDownList_SelectedIndexChanged(null, null);
                txtRequestDetails.Text = Entitlement.RequestDetail;
                //var detail = AdhocRequestDetails;
                //if (Entitlement.Roles != null)
                //{
                //    List<string> roles = Entitlement.Roles.Split(new Char[] { ',' }).ToList();
                //    roles.ForEach(r =>
                //    {

                //        detail.Add(new AdHocIACMRequestDetail() { Entitlement = r, EntitlementType = EntitlementType.Role });

                //    });
                //}

                //if (Entitlement.Groups != null)
                //{
                //    List<string> groups = Entitlement.Groups.Split(new Char[] { ',' }).ToList();
                //    groups.ForEach(g =>
                //    {

                //        detail.Add(new AdHocIACMRequestDetail() { Entitlement = g, EntitlementType = EntitlementType.Group });

                //    });
                //}

                //AdhocRequestDetails = detail;

                //FillRequestGrid();
            }
        }

        protected void ddlEntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEntitlementType.SelectedItem != null && ddlEntitlementType.SelectedIndex > 0)
            {
                divRole.Visible = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role;
                divGroup.Visible = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group;
            }
        }

        protected void AutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count == 2)
            {
                box.Entries.Remove(box.Entries[1]);
            }
            if (box.Entries.Count == 1)
            {
                btnRoleAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role ? true : false;
                btnGroupAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group ? true : false;
            }
        }

        protected void AutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            btnRoleAdd.Enabled = false;
            btnGroupAdd.Enabled = false;
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count > 0)
            {
                btnRoleAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role ? true : false;
                btnGroupAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group ? true : false;
            }
        }

        protected void RequestGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.AdhocRequestDetails;
                requests.RemoveAt(e.Item.ItemIndex);
                this.AdhocRequestDetails = requests;
                FillRequestGrid();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            EntitlementType entitlementType = (EntitlementType)Convert.ToInt32(ddlEntitlementType.SelectedValue);
            if (divGroup.Visible && GroupAutoCompleteBox.Entries.Count > 0 && entitlementType == EntitlementType.Group && btn.ID == "btnGroupAdd")
            {
                string entitlement = GroupAutoCompleteBox.Entries[0].Text;
                if (!AdhocRequestDetails.Any(item => item.EntitlementType == entitlementType && item.Entitlement == entitlement))
                {
                    List<AdHocIACMRequestDetail> list = AdhocRequestDetails;
                    list.Add(new AdHocIACMRequestDetail { Entitlement = entitlement, EntitlementType = entitlementType });
                    AdhocRequestDetails = list;
                    FillRequestGrid();

                }
                else
                {
                    ShowAlertMessage(AlertType.Error, $"The Group - {entitlement} is exists in the request list.");
                }
                GroupAutoCompleteBox.Entries.Clear();
            }
            else if (divRole.Visible && RolesAutoCompleteBox.Entries.Count > 0 && entitlementType == EntitlementType.Role && btn.ID == "btnRoleAdd")
            {
                string entitlement = RolesAutoCompleteBox.Entries[0].Text;
                if (!AdhocRequestDetails.Any(item => item.EntitlementType == entitlementType && item.Entitlement == entitlement))
                {
                    List<AdHocIACMRequestDetail> list = AdhocRequestDetails;
                    list.Add(new AdHocIACMRequestDetail { Entitlement = entitlement, EntitlementType = entitlementType });
                    AdhocRequestDetails = list;
                    FillRequestGrid();
                }
                else
                {
                    ShowAlertMessage(AlertType.Error, $"The Role - {entitlement} is exists in the request list.");
                }
                RolesAutoCompleteBox.Entries.Clear();
            }
            btn.Enabled = false;
        }

        void FillRequestGrid()
        {
            var list = (List<AdHocIACMRequestDetail>)this.AdhocRequestDetails;
            RequestGrid.DataSource = list;
            trGrdid.Visible = list.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
        private void LoadControl()
        {
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    pnlSecurityQstns.Visible = true;
                }
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
                    OtherControls.Visible = false;
                }
                else
                {
                    OtherControls.Visible = true;
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

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
            }

            if (list.AccountReconDictionary.Count() == 1)
            {
                ddlEndpointUser.SelectedIndex = 1;
            }

            if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
            {
                Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
            }
        }

        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["EndpointUserId"] = null;
            if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
            }
        }

        public override bool Validate()
        {
            bool result = true;
            var requested = this.AdhocRequest;
            string Message = "Please select the ";
            string ControlMessage = "";

            if (SharedPanelPLRELearn.Visible && string.IsNullOrEmpty(txtAcntName.Text))
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Account Name" : ControlMessage + " , Account Name";
            }
            if (trEndPointUser.Visible && ddlEndpointUser.SelectedIndex <= 0 && requested.AdhocActionType != AdHocActionType.NewAccount)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + " , EndPoint User";
            }
            if (OtherControls.Visible && string.IsNullOrEmpty(txtRequestDetails.Text))
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Request Details" : ControlMessage + " , Request Details";
            }
            if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                }
            }
            if (ControlMessage != "")
            {
                int lastIndex = ControlMessage.LastIndexOf(",");
                Message += lastIndex > 0 ? ControlMessage.Substring(0, lastIndex) + " and" + ControlMessage.Substring(lastIndex + 1) : ControlMessage;
                ShowAlertMessage(Infrastructure.AlertType.Error, Message + ".");
                result = false;
            }

            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount && (ddlEndpointUser.SelectedItem==null || ddlEndpointUser.SelectedItem.Value == "-- Select EndPoint User --"))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the EndPoint User.");
                result = false;
            }
            if ((this.AdhocRequest.AdhocActionType== AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && string.IsNullOrWhiteSpace(txtRequestDetails.Text))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Request Info.");
                result = false;
            }

            return result;
        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdhocSAPEPRequest sap = new AdhocSAPEPRequest()
            {
                EndpointUserId = (ddlEndpointUser.SelectedIndex > 0) ? Convert.ToInt32(ddlEndpointUser.SelectedItem.Value) : 0,
                RequestDetail = txtRequestDetails.Text,
                Roles = string.Join(",", AdhocRequestDetails.Where(r => r.EntitlementType == EntitlementType.Role).Select(r => r.Entitlement)),
                Groups   = string.Join(",", AdhocRequestDetails.Where(r => r.EntitlementType == EntitlementType.Group).Select(r => r.Entitlement)),
            };

            if (ddlEndpointUser.SelectedItem != null)
            {
                sap.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
            }

            AdHocSAPEPSharedAccount sharedDetails = new AdHocSAPEPSharedAccount();
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary))
            {
                sharedDetails.AccountName = txtAcntName.Text;
                sharedDetails.Description = txtDiscription.Text; 
            }
            sap.AdHocSAPEPSharedAccount = (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? sharedDetails : null;
            return sap;
        }
        protected override void afterSave()
        { 
            //if (OtherControls.Visible)
            {
                txtRequestDetails.Text = "";
            }
            //if (SharedPanelPLRELearn.Visible)
            {
                txtAcntName.Text = "";
                txtDiscription.Text = "";
            } 
        }


    }
}